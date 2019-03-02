Imports System.ComponentModel.DataAnnotations
Imports System.Threading
Imports Algo2TradeCore
Imports Algo2TradeCore.Adapter
Imports Algo2TradeCore.Entities
Imports Algo2TradeCore.Strategies
Imports Utilities.Numbers
Imports NLog

Public Class MomentumReversalStrategyInstrument
    Inherits StrategyInstrument
    Implements IDisposable

#Region "Logging and Status Progress"
    Public Shared Shadows logger As Logger = LogManager.GetCurrentClassLogger
#End Region

    Public Sub New(ByVal associatedInstrument As IInstrument, ByVal associatedParentStrategy As Strategy, ByVal canceller As CancellationTokenSource)
        MyBase.New(associatedInstrument, associatedParentStrategy, canceller)
        Select Case Me.ParentStrategy.ParentController.BrokerSource
            Case APISource.Zerodha
                _APIAdapter = New ZerodhaAdapter(ParentStrategy.ParentController, _cts)
            Case APISource.Upstox
                Throw New NotImplementedException
            Case APISource.None
                Throw New NotImplementedException
        End Select
        AddHandler _APIAdapter.Heartbeat, AddressOf OnHeartbeat
        AddHandler _APIAdapter.WaitingFor, AddressOf OnWaitingFor
        AddHandler _APIAdapter.DocumentRetryStatus, AddressOf OnDocumentRetryStatus
        AddHandler _APIAdapter.DocumentDownloadComplete, AddressOf OnDocumentDownloadComplete
        RawPayloadConsumers = New List(Of IPayloadConsumer)
        If Me.ParentStrategy.IsStrategyCandleStickBased Then
            If Me.ParentStrategy.UserSettings.SignalTimeFrame > 0 Then
                RawPayloadConsumers.Add(New PayloadToChartConsumer(Me.ParentStrategy.UserSettings.SignalTimeFrame))
            Else
                Throw New ApplicationException(String.Format("Signal Timeframe is 0 or Nothing, does not adhere to the strategy:{0}", Me.ParentStrategy.ToString))
            End If
        End If
    End Sub
    Public Overrides Async Function MonitorAsync() As Task
        Try
            Dim slDelayCtr As Integer = 0
            While True
                If Me.ParentStrategy.ParentController.OrphanException IsNot Nothing Then
                    Throw Me.ParentStrategy.ParentController.OrphanException
                End If
                _cts.Token.ThrowIfCancellationRequested()
                Dim placeOrderDetails As Object = Nothing
                Dim placeOrderTrigger As Tuple(Of Boolean, PlaceOrderParameters) = IsTriggerReceivedForPlaceOrder()
                If placeOrderTrigger IsNot Nothing AndAlso placeOrderTrigger.Item1 = True Then
                    placeOrderDetails = Await ExecuteCommandAsync(ExecuteCommands.PlaceBOSLMISOrder, Nothing).ConfigureAwait(False)
                    'To store signal candle in order collection
                    Dim businessOrder As IBusinessOrder = New BusinessOrder With {
                            .ParentOrderIdentifier = placeOrderDetails("data")("order_id"),
                            .SignalCandle = placeOrderTrigger.Item2.SignalCandle
                        }
                    businessOrder = Me.OrderDetails.GetOrAdd(businessOrder.ParentOrderIdentifier, businessOrder)
                    businessOrder.SignalCandle = placeOrderTrigger.Item2.SignalCandle
                    Me.OrderDetails.AddOrUpdate(businessOrder.ParentOrderIdentifier, businessOrder, Function(key, value) businessOrder)
                End If
                _cts.Token.ThrowIfCancellationRequested()
                If slDelayCtr = 3 Then
                    slDelayCtr = 0
                    Dim modifyStoplossOrderTrigger As List(Of Tuple(Of Boolean, String, Decimal)) = IsTriggerReceivedForModifyStoplossOrder()
                    If modifyStoplossOrderTrigger IsNot Nothing AndAlso modifyStoplossOrderTrigger.Count > 0 Then
                        Await ExecuteCommandAsync(ExecuteCommands.ModifyStoplossOrder, Nothing).ConfigureAwait(False)
                    End If
                End If
                _cts.Token.ThrowIfCancellationRequested()
                Dim exitOrderTrigger As List(Of Tuple(Of Boolean, String, String)) = IsTriggerReceivedForExitOrder()
                If exitOrderTrigger IsNot Nothing AndAlso exitOrderTrigger.Count > 0 Then
                    Await ExecuteCommandAsync(ExecuteCommands.CancelBOOrder, Nothing).ConfigureAwait(False)
                End If
                _cts.Token.ThrowIfCancellationRequested()
                Await Task.Delay(1000).ConfigureAwait(False)
                slDelayCtr += 1
            End While
        Catch ex As Exception
            'To log exceptions getting created from this function as the bubble up of the exception
            'will anyways happen to Strategy.MonitorAsync but it will not be shown until all tasks exit
            logger.Error("Strategy Instrument:{0}, error:{1}", Me.ToString, ex.ToString)
            Throw ex
        End Try
    End Function
    Protected Overrides Function IsTriggerReceivedForPlaceOrder() As Tuple(Of Boolean, PlaceOrderParameters)
        Dim ret As Tuple(Of Boolean, PlaceOrderParameters) = Nothing
        Dim MRUserSettings As MomentumReversalUserInputs = Me.ParentStrategy.UserSettings
        Dim tradeStartTime As Date = New Date(Now.Year, Now.Month, Now.Day, 9, 20, 0)
        Dim lastTradeEntryTime As Date = New Date(Now.Year, Now.Month, Now.Day, 14, 30, 0)
        Dim runningCandlePayload As OHLCPayload = GetXMinuteCurrentCandle(MRUserSettings.SignalTimeFrame)

        If Now < lastTradeEntryTime AndAlso runningCandlePayload IsNot Nothing AndAlso runningCandlePayload.SnapshotDateTime >= tradeStartTime AndAlso
            runningCandlePayload.PayloadGeneratedBy = IPayload.PayloadSource.CalculatedTick AndAlso runningCandlePayload.PreviousPayload IsNot Nothing AndAlso
            GetActiveOrder(APIAdapter.TransactionType.None) Is Nothing AndAlso Me.TotalTrades <= MRUserSettings.NumberOfTarde AndAlso
            Not IsAnyTradeExitedInCurrentTimeframeCandle(MRUserSettings.SignalTimeFrame, runningCandlePayload.SnapshotDateTime) Then

            Dim benchmarkWicksSize As Double = runningCandlePayload.PreviousPayload.CandleRange * MRUserSettings.CandleWickSizePercentage / 100
            If runningCandlePayload.PreviousPayload.CandleRangePercentage > MRUserSettings.MinCandleRangePercentage Then
                If runningCandlePayload.PreviousPayload.CandleWicks.Top > benchmarkWicksSize Then
                    Dim MRTradePrice As Decimal = runningCandlePayload.PreviousPayload.HighPrice
                    Dim price As Decimal = MRTradePrice + Math.Round(ConvertFloorCeling(MRTradePrice * 0.3 / 100, Convert.ToDouble(TradableInstrument.TickSize), RoundOfType.Celing), 2)
                    Dim triggerPrice As Decimal = MRTradePrice + CalculateBuffer(MRTradePrice, RoundOfType.Celing)
                    Dim stoplossPrice As Decimal = runningCandlePayload.PreviousPayload.LowPrice - CalculateBuffer(runningCandlePayload.PreviousPayload.LowPrice, RoundOfType.Celing)
                    Dim target As Decimal = Math.Round(ConvertFloorCeling((triggerPrice - stoplossPrice) * MRUserSettings.TargetMultiplier, Convert.ToDouble(TradableInstrument.TickSize), RoundOfType.Celing), 2)
                    Dim quantity As Integer = Nothing
                    Dim tag As String = GenerateTag()
                    If Me.TradableInstrument.InstrumentType.ToUpper = "FUT" Then
                        quantity = Me.TradableInstrument.LotSize
                    Else
                        quantity = MRUserSettings.InstrumentsData(Me.TradingSymbol).Quantity
                        'quantity = 1
                    End If
                    Dim stoploss As Decimal = GetModifiedStoploss(triggerPrice, stoplossPrice, quantity)

                    Dim parameters As New PlaceOrderParameters With
                           {.EntryDirection = APIAdapter.TransactionType.Buy,
                           .Quantity = quantity,
                           .Price = price,
                           .TriggerPrice = triggerPrice,
                           .SquareOffValue = target,
                           .StoplossValue = stoploss,
                           .Tag = tag,
                           .SignalCandle = runningCandlePayload.PreviousPayload}
                    ret = New Tuple(Of Boolean, PlaceOrderParameters)(True, parameters)
                ElseIf runningCandlePayload.PreviousPayload.CandleWicks.Bottom > benchmarkWicksSize Then
                    Dim MRTradePrice As Decimal = runningCandlePayload.PreviousPayload.LowPrice
                    Dim price As Decimal = MRTradePrice - Math.Round(ConvertFloorCeling(MRTradePrice * 0.3 / 100, Convert.ToDouble(TradableInstrument.TickSize), RoundOfType.Celing), 2)
                    Dim triggerPrice As Decimal = MRTradePrice - CalculateBuffer(MRTradePrice, RoundOfType.Celing)
                    Dim stoplossPrice As Decimal = runningCandlePayload.PreviousPayload.HighPrice + CalculateBuffer(runningCandlePayload.PreviousPayload.HighPrice, RoundOfType.Celing)
                    Dim target As Decimal = Math.Round(ConvertFloorCeling((stoplossPrice - triggerPrice) * MRUserSettings.TargetMultiplier, Convert.ToDouble(TradableInstrument.TickSize), RoundOfType.Celing), 2)
                    Dim quantity As Integer = Nothing
                    Dim tag As String = GenerateTag()
                    If Me.TradableInstrument.InstrumentType.ToUpper = "FUT" Then
                        quantity = Me.TradableInstrument.LotSize
                    Else
                        quantity = MRUserSettings.InstrumentsData(Me.TradingSymbol).Quantity
                        'quantity = 1
                    End If
                    Dim stoploss As Decimal = GetModifiedStoploss(stoplossPrice, triggerPrice, quantity)

                    Dim parameters As New PlaceOrderParameters With
                           {.EntryDirection = APIAdapter.TransactionType.Sell,
                           .Quantity = quantity,
                           .Price = price,
                           .TriggerPrice = triggerPrice,
                           .SquareOffValue = target,
                           .StoplossValue = stoploss,
                           .Tag = tag,
                           .SignalCandle = runningCandlePayload.PreviousPayload}
                    ret = New Tuple(Of Boolean, PlaceOrderParameters)(True, parameters)
                End If
            End If
        End If
        Dim encryptionString As String = Nothing
        If ret IsNot Nothing AndAlso ret.Item2 IsNot Nothing Then
            logger.Warn("Before Collection checking:{0}", Utilities.Strings.JsonSerialize(RequestResponseForPlaceOrder))
            encryptionString = Utilities.Strings.Encrypt(ret.Item2.ToString, "Algo2TradePlaceOrder")
            logger.Warn("Encryption String:{0}", encryptionString)
            If RequestResponseForPlaceOrder IsNot Nothing AndAlso RequestResponseForPlaceOrder.Count > 0 AndAlso
            RequestResponseForPlaceOrder.ContainsKey(encryptionString) Then
                ret = Nothing
            End If
        End If
        Return ret
    End Function
    Protected Overrides Function IsTriggerReceivedForModifyStoplossOrder() As List(Of Tuple(Of Boolean, String, Decimal))
        Dim ret As List(Of Tuple(Of Boolean, String, Decimal)) = Nothing
        If OrderDetails IsNot Nothing AndAlso OrderDetails.Count > 0 Then
            Dim currentTime As Date = Now
            For Each parentOrderId In OrderDetails.Keys
                Dim parentBusinessOrder As IBusinessOrder = OrderDetails(parentOrderId)
                If parentBusinessOrder.ParentOrder IsNot Nothing AndAlso parentBusinessOrder.SignalCandle IsNot Nothing AndAlso
                    parentBusinessOrder.ParentOrder.Status = "COMPLETE" AndAlso
                    parentBusinessOrder.SLOrder IsNot Nothing AndAlso parentBusinessOrder.SLOrder.Count > 0 Then
                    Dim parentOrderPrice As Decimal = parentBusinessOrder.ParentOrder.AveragePrice
                    Dim potentialSLPrice As Decimal = Nothing
                    Dim triggerPrice As Decimal = Nothing
                    If parentBusinessOrder.ParentOrder.TransactionType = "BUY" Then
                        potentialSLPrice = parentBusinessOrder.SignalCandle.LowPrice - CalculateBuffer(parentBusinessOrder.SignalCandle.LowPrice, RoundOfType.Celing)
                        triggerPrice = GetModifiedStoploss(parentOrderPrice, potentialSLPrice, parentBusinessOrder.ParentOrder.Quantity)
                        triggerPrice = Math.Round(ConvertFloorCeling(parentOrderPrice - triggerPrice, Convert.ToDouble(TradableInstrument.TickSize), RoundOfType.Celing), 2)
                    ElseIf parentBusinessOrder.ParentOrder.TransactionType = "SELL" Then
                        potentialSLPrice = parentBusinessOrder.SignalCandle.HighPrice + CalculateBuffer(parentBusinessOrder.SignalCandle.HighPrice, RoundOfType.Celing)
                        triggerPrice = GetModifiedStoploss(potentialSLPrice, parentOrderPrice, parentBusinessOrder.ParentOrder.Quantity)
                        triggerPrice = Math.Round(ConvertFloorCeling(parentOrderPrice + triggerPrice, Convert.ToDouble(TradableInstrument.TickSize), RoundOfType.Celing), 2)
                    End If

                    Dim potentialStoplossPrice As Decimal = Nothing
                    For Each slOrder In parentBusinessOrder.SLOrder
                        If Not slOrder.Status = "COMPLETE" AndAlso Not slOrder.Status = "CANCELLED" AndAlso Not slOrder.Status = "REJECTED" Then
                            If slOrder.TriggerPrice <> triggerPrice Then
                                If RequestResponseForModifyOrder IsNot Nothing AndAlso RequestResponseForModifyOrder.Count > 0 AndAlso
                                    RequestResponseForModifyOrder.ContainsKey(Utilities.Strings.Encrypt(triggerPrice, slOrder.OrderIdentifier)) Then
                                    Continue For
                                End If
                                If ret Is Nothing Then ret = New List(Of Tuple(Of Boolean, String, Decimal))
                                ret.Add(New Tuple(Of Boolean, String, Decimal)(True, slOrder.OrderIdentifier, triggerPrice))
                            Else
                                Debug.WriteLine(String.Format("Stoploss modified {0} Quantity:{1}, ID:{2}", Me.GenerateTag(), slOrder.Quantity, slOrder.OrderIdentifier))
                            End If
                        End If
                    Next
                End If
            Next
        End If
        Return ret
    End Function
    Protected Overrides Function IsTriggerReceivedForExitOrder() As List(Of Tuple(Of Boolean, String, String))
        Dim ret As List(Of Tuple(Of Boolean, String, String)) = Nothing
        Dim allActiveOrders As List(Of IOrder) = GetAllActiveOrders(APIAdapter.TransactionType.None)
        If allActiveOrders IsNot Nothing AndAlso allActiveOrders.Count > 0 Then
            Dim parentOrders As List(Of IOrder) = allActiveOrders.FindAll(Function(x)
                                                                              Return x.ParentOrderIdentifier Is Nothing AndAlso
                                                                              x.Status = "TRIGGER PENDING"
                                                                          End Function)
            If parentOrders IsNot Nothing AndAlso parentOrders.Count > 0 Then
                For Each parentOrder In parentOrders
                    Dim parentBussinessOrder As IBusinessOrder = OrderDetails(parentOrder.OrderIdentifier)
                    Dim runningCandle As OHLCPayload = GetXMinuteCurrentCandle(Me.ParentStrategy.UserSettings.SignalTimeFrame)
                    If runningCandle IsNot Nothing AndAlso runningCandle.PayloadGeneratedBy = IPayload.PayloadSource.CalculatedTick AndAlso
                        runningCandle.PreviousPayload IsNot Nothing AndAlso runningCandle.PreviousPayload.PreviousPayload IsNot Nothing Then
                        If parentBussinessOrder.SignalCandle IsNot Nothing AndAlso
                            parentBussinessOrder.SignalCandle.SnapshotDateTime = runningCandle.PreviousPayload.PreviousPayload.SnapshotDateTime Then
                            If RequestResponseForCancelOrder IsNot Nothing AndAlso RequestResponseForCancelOrder.Count > 0 AndAlso
                                RequestResponseForCancelOrder.ContainsKey(Utilities.Strings.Encrypt("Algo2TradeParentCancel", parentBussinessOrder.ParentOrderIdentifier)) Then
                                Continue For
                            End If
                            If ret Is Nothing Then ret = New List(Of Tuple(Of Boolean, String, String))
                            ret.Add(New Tuple(Of Boolean, String, String)(True, parentBussinessOrder.ParentOrderIdentifier, Nothing))
                        End If
                    End If
                Next
            End If
        End If
        Return ret
    End Function
    Private Function GetModifiedStoploss(ByVal entryPrice As Decimal, ByVal stoplossPrice As Decimal, ByVal quantity As Integer) As Decimal
        Dim ret As Decimal = Nothing
        Dim MRUserSettings As MomentumReversalUserInputs = Me.ParentStrategy.UserSettings
        Dim capitalRequiredWithMargin As Decimal = (entryPrice * quantity / 30)
        Dim pl As Decimal = Me._APIAdapter.CalculatePLWithBrokerage(Me.TradingSymbol, entryPrice, stoplossPrice, quantity, Me.TradableInstrument.Exchange)
        If Math.Abs(pl) > capitalRequiredWithMargin * MRUserSettings.MaxStoplossPercentage / 100 Then
            ret = capitalRequiredWithMargin * MRUserSettings.MaxStoplossPercentage / 100 / quantity
        Else
            ret = Math.Abs(entryPrice - stoplossPrice)
        End If
        ret = Math.Round(ConvertFloorCeling(ret, Convert.ToDouble(TradableInstrument.TickSize), RoundOfType.Celing), 2)
        Return ret
    End Function
    Private Function IsAnyTradeExitedInCurrentTimeframeCandle(ByVal timeFrame As Integer, ByVal currentTimeframeCandleTime As Date) As Boolean
        Dim ret As Boolean = False
        If OrderDetails IsNot Nothing AndAlso OrderDetails.Count > 0 Then
            For Each parentOrderId In OrderDetails.Keys
                Dim parentBusinessOrder As IBusinessOrder = OrderDetails(parentOrderId)
                If parentBusinessOrder IsNot Nothing AndAlso parentBusinessOrder.ParentOrder IsNot Nothing Then
                    'If parentBusinessOrder.ParentOrder.Status = "COMPLETE" OrElse parentBusinessOrder.ParentOrder.Status = "OPEN" Then
                    If Not parentBusinessOrder.ParentOrder.Status = "REJECTED" Then
                        If parentBusinessOrder.AllOrder IsNot Nothing AndAlso parentBusinessOrder.AllOrder.Count > 0 Then
                            For Each slOrder In parentBusinessOrder.AllOrder
                                If slOrder.Status = "COMPLETE" OrElse slOrder.Status = "CANCELLED" Then
                                    Dim orderExitBlockTime As Date = New Date(slOrder.TimeStamp.Year,
                                                                            slOrder.TimeStamp.Month,
                                                                            slOrder.TimeStamp.Day,
                                                                            slOrder.TimeStamp.Hour,
                                                                            Math.Floor(slOrder.TimeStamp.Minute / timeFrame) * timeFrame, 0)

                                    If orderExitBlockTime = currentTimeframeCandleTime Then
                                        ret = True
                                        Exit For
                                    End If
                                End If
                            Next
                        End If
                    End If
                End If
            Next
        End If
        Return ret
    End Function

#Region "IDisposable Support"
    Private disposedValue As Boolean ' To detect redundant calls

    ' IDisposable
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                ' TODO: dispose managed state (managed objects).
                If _APIAdapter IsNot Nothing Then
                    RemoveHandler _APIAdapter.Heartbeat, AddressOf OnHeartbeat
                    RemoveHandler _APIAdapter.WaitingFor, AddressOf OnWaitingFor
                    RemoveHandler _APIAdapter.DocumentRetryStatus, AddressOf OnDocumentRetryStatus
                    RemoveHandler _APIAdapter.DocumentDownloadComplete, AddressOf OnDocumentDownloadComplete
                End If
            End If

            ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
            ' TODO: set large fields to null.
        End If
        disposedValue = True
    End Sub

    ' TODO: override Finalize() only if Dispose(disposing As Boolean) above has code to free unmanaged resources.
    'Protected Overrides Sub Finalize()
    '    ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
    '    Dispose(False)
    '    MyBase.Finalize()
    'End Sub

    ' This code added by Visual Basic to correctly implement the disposable pattern.
    Public Sub Dispose() Implements IDisposable.Dispose
        ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
        Dispose(True)
        ' TODO: uncomment the following line if Finalize() is overridden above.
        ' GC.SuppressFinalize(Me)
    End Sub
#End Region
End Class