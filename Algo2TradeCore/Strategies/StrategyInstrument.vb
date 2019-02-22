Imports System.ComponentModel
Imports System.ComponentModel.DataAnnotations
Imports System.Net.Http
Imports System.Threading
Imports Algo2TradeCore.Adapter
Imports Algo2TradeCore.Entities
Imports Algo2TradeCore.Exceptions
Imports NLog
Imports Utilities
Imports Utilities.ErrorHandlers
Imports Utilities.Numbers.NumberManipulation
Imports Algo2TradeCore.ChartHandler.ChartStyle

Namespace Strategies
    Public MustInherit Class StrategyInstrument
        Implements INotifyPropertyChanged
        Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged
        Protected Sub NotifyPropertyChanged(ByVal p As String)
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(p))
        End Sub

#Region "Events/Event handlers"
        Public Event DocumentDownloadCompleteEx(ByVal source As List(Of Object))
        Public Event DocumentRetryStatusEx(ByVal currentTry As Integer, ByVal totalTries As Integer, ByVal source As List(Of Object))
        Public Event HeartbeatEx(ByVal msg As String, ByVal source As List(Of Object))
        Public Event WaitingForEx(ByVal elapsedSecs As Integer, ByVal totalSecs As Integer, ByVal msg As String, ByVal source As List(Of Object))
        'The below functions are needed to allow the derived classes to raise the above two events
        Protected Overridable Sub OnDocumentDownloadCompleteEx(ByVal source As List(Of Object))
            If source IsNot Nothing Then source = New List(Of Object)
            source.Add(Me)
            RaiseEvent DocumentDownloadCompleteEx(source)
        End Sub
        Protected Overridable Sub OnDocumentRetryStatusEx(ByVal currentTry As Integer, ByVal totalTries As Integer, ByVal source As List(Of Object))
            If source IsNot Nothing Then source = New List(Of Object)
            source.Add(Me)
            RaiseEvent DocumentRetryStatusEx(currentTry, totalTries, source)
        End Sub
        Protected Overridable Sub OnHeartbeatEx(ByVal msg As String, ByVal source As List(Of Object))
            If source IsNot Nothing Then source = New List(Of Object)
            source.Add(Me)
            If TradableInstrument IsNot Nothing Then
                RaiseEvent HeartbeatEx(String.Format("{0}:{1}", TradableInstrument.InstrumentIdentifier, msg), source)
            Else
                RaiseEvent HeartbeatEx(String.Format("{0}:{1}", "No instrument", msg), source)
            End If
        End Sub
        Protected Overridable Sub OnWaitingForEx(ByVal elapsedSecs As Integer, ByVal totalSecs As Integer, ByVal msg As String, ByVal source As List(Of Object))
            If source IsNot Nothing Then source = New List(Of Object)
            source.Add(Me)
            If TradableInstrument IsNot Nothing Then
                RaiseEvent WaitingForEx(elapsedSecs, totalSecs, String.Format("{0}-{1}", TradableInstrument.InstrumentIdentifier, msg), source)
            Else
                RaiseEvent WaitingForEx(elapsedSecs, totalSecs, String.Format("{0}-{1}", "No instrument", msg), source)
            End If
        End Sub
        Protected Overridable Sub OnDocumentDownloadComplete()
            RaiseEvent DocumentDownloadCompleteEx(New List(Of Object) From {Me})
        End Sub
        Protected Overridable Sub OnDocumentRetryStatus(ByVal currentTry As Integer, ByVal totalTries As Integer)
            RaiseEvent DocumentRetryStatusEx(currentTry, totalTries, New List(Of Object) From {Me})
        End Sub
        Protected Overridable Sub OnHeartbeat(ByVal msg As String)
            If TradableInstrument IsNot Nothing Then
                RaiseEvent HeartbeatEx(String.Format("{0}:{1}", TradableInstrument.InstrumentIdentifier, msg), New List(Of Object) From {Me})
            Else
                RaiseEvent HeartbeatEx(String.Format("{0}:{1}", "No instrument", msg), New List(Of Object) From {Me})
            End If
        End Sub
        Protected Overridable Sub OnWaitingFor(ByVal elapsedSecs As Integer, ByVal totalSecs As Integer, ByVal msg As String)
            If TradableInstrument IsNot Nothing Then
                RaiseEvent WaitingForEx(elapsedSecs, totalSecs, String.Format("{0}-{1}", TradableInstrument.InstrumentIdentifier, msg), New List(Of Object) From {Me})
            Else
                RaiseEvent WaitingForEx(elapsedSecs, totalSecs, String.Format("{0}-{1}", "No instrument", msg), New List(Of Object) From {Me})
            End If
        End Sub
#End Region

#Region "Logging and Status Progress"
        Public Shared logger As Logger = LogManager.GetCurrentClassLogger
#End Region

#Region "Enum"
        Public Enum ExecuteCommands
            PlaceBOLimitMISOrder = 1
            PlaceBOSLMISOrder
            PlaceCOMarketMISOrder
            ModifyStoplossOrder
            CancelBOOrder
            CancelCOOrder
            ForceCancelBOOrder
            ForceCancelCOOrder
        End Enum
#End Region

        Protected _cts As CancellationTokenSource
        Protected _APIAdapter As APIAdapter
        Protected _MaxReTries As Integer = 20
        Protected _WaitDurationOnConnectionFailure As TimeSpan = TimeSpan.FromSeconds(5)
        Protected _WaitDurationOnServiceUnavailbleFailure As TimeSpan = TimeSpan.FromSeconds(30)
        Protected _WaitDurationOnAnyFailure As TimeSpan = TimeSpan.FromSeconds(10)
        Public Sub New(ByVal associatedInstrument As IInstrument, ByVal associatedParentStrategy As Strategy, ByVal canceller As CancellationTokenSource)
            TradableInstrument = associatedInstrument
            Me.ParentStrategy = associatedParentStrategy
            _cts = canceller
            OrderDetails = New Concurrent.ConcurrentDictionary(Of String, IBusinessOrder)
        End Sub

        <System.ComponentModel.Browsable(False)>
        Public Property ParentStrategy As Strategy
        <System.ComponentModel.Browsable(False)>
        Public Property TradableInstrument As IInstrument
        Public Property OrderDetails As Concurrent.ConcurrentDictionary(Of String, IBusinessOrder)
        Public Property RawPayloadConsumers As List(Of IPayloadConsumer)

        Protected RequestResponseForPlaceOrder As Concurrent.ConcurrentDictionary(Of String, String)
        Protected RequestResponseForModifyOrder As Concurrent.ConcurrentDictionary(Of String, String)
        Protected RequestResponseForCancelOrder As Concurrent.ConcurrentDictionary(Of String, String)

#Region "UI Properties"
        <Display(Name:="Symbol", Order:=0)>
        Public Overridable ReadOnly Property TradingSymbol As String
            Get
                If TradableInstrument IsNot Nothing Then
                    Return TradableInstrument.TradingSymbol
                Else
                    Return Nothing
                End If
            End Get
        End Property
        Private _OpenPrice As Decimal
        <Display(Name:="Open", Order:=1)>
        Public Overridable ReadOnly Property OpenPrice As Decimal
            Get
                If TradableInstrument.LastTick IsNot Nothing Then
                    _OpenPrice = TradableInstrument.LastTick.Open
                    Return _OpenPrice
                Else
                    Return Nothing
                End If
            End Get
        End Property
        Private _HighPrice As Decimal
        <Display(Name:="High", Order:=2)>
        Public Overridable ReadOnly Property HighPrice As Decimal
            Get
                If TradableInstrument.LastTick IsNot Nothing Then
                    _HighPrice = TradableInstrument.LastTick.High
                    Return _HighPrice
                Else
                    Return Nothing
                End If
            End Get
        End Property
        Private _LowPrice As Decimal
        <Display(Name:="Low", Order:=3)>
        Public Overridable ReadOnly Property LowPrice As Decimal
            Get
                If TradableInstrument.LastTick IsNot Nothing Then
                    _LowPrice = TradableInstrument.LastTick.Low
                    Return _LowPrice
                Else
                    Return Nothing
                End If
            End Get
        End Property
        Private _Volume As Long
        <Display(Name:="Volume", Order:=4)>
        Public Overridable ReadOnly Property Volume As Long
            Get
                If TradableInstrument.LastTick IsNot Nothing Then
                    _Volume = TradableInstrument.LastTick.Volume
                    Return _Volume
                Else
                    Return Nothing
                End If
            End Get
        End Property
        Private _AveragePrice As Decimal
        <Display(Name:="Average Price", Order:=5)>
        Public Overridable ReadOnly Property AveragePrice As Decimal
            Get
                If TradableInstrument.LastTick IsNot Nothing Then
                    _AveragePrice = TradableInstrument.LastTick.AveragePrice
                    Return _AveragePrice
                Else
                    Return Nothing
                End If
            End Get
        End Property
        Private _LastPrice As Decimal
        <Display(Name:="Last Price", Order:=6)>
        Public Overridable ReadOnly Property LastPrice As Decimal
            Get
                If TradableInstrument.LastTick IsNot Nothing Then
                    _LastPrice = TradableInstrument.LastTick.LastPrice
                    Return _LastPrice
                Else
                    Return Nothing
                End If
            End Get
        End Property
        Private _Timestamp As Date?
        <Display(Name:="Timestamp", Order:=7)>
        Public Overridable ReadOnly Property Timestamp As Date?
            Get
                If TradableInstrument.LastTick IsNot Nothing Then
                    _Timestamp = TradableInstrument.LastTick.Timestamp
                    Return _Timestamp
                Else
                    Return Nothing
                End If
            End Get
        End Property

        <Display(Name:="Last Candle Time", Order:=8)>
        Public ReadOnly Property LastCandleTime As Date
            Get
                If TradableInstrument.RawPayloads IsNot Nothing AndAlso TradableInstrument.RawPayloads.Count > 0 Then
                    Return TradableInstrument.RawPayloads.Keys.Max
                Else
                    Return New Date
                End If
            End Get
        End Property
        <Display(Name:="Active Instrument", Order:=9)>
        Public ReadOnly Property ActiveInstrument As Boolean
            Get
                Dim ret As Boolean = False
                Dim allActiveOrders As List(Of IOrder) = GetAllActiveOrders(APIAdapter.TransactionType.None)
                ret = allActiveOrders IsNot Nothing AndAlso allActiveOrders.Count > 0
                Return ret
            End Get
        End Property
        <Display(Name:="Total Trades", Order:=10)>
        Public ReadOnly Property TotalTrades As Integer
            Get
                Dim tradeCount As Integer = 0
                If OrderDetails IsNot Nothing AndAlso OrderDetails.Count > 0 Then
                    For Each parentOrderId In OrderDetails.Keys
                        Dim parentBusinessOrder As IBusinessOrder = OrderDetails(parentOrderId)
                        If parentBusinessOrder.ParentOrder.Status = "COMPLETE" Then
                            tradeCount += 1
                        End If
                    Next
                End If
                Return tradeCount
            End Get
        End Property

        Private _PL As Decimal
        <Display(Name:="Profit & Loss", Order:=11)>
        Public ReadOnly Property PL As Decimal
            Get
                _PL = CalculatePL()
                Return _PL
            End Get
        End Property

        Private _Tradabale As Boolean
        <Display(Name:="Tradable", Order:=12)>
        Public Overridable ReadOnly Property Tradabale As Boolean
            Get
                If TradableInstrument.LastTick IsNot Nothing Then
                    _Tradabale = TradableInstrument.LastTick.Tradable
                    Return _Tradabale
                Else
                    Return Nothing
                End If
            End Get
        End Property

        Private _ClosePrice As Decimal
        <Display(Name:="Previous Close", Order:=13)>
        Public Overridable ReadOnly Property ClosePrice As Decimal
            Get
                If TradableInstrument.LastTick IsNot Nothing Then
                    _ClosePrice = TradableInstrument.LastTick.Close
                    Return _ClosePrice
                Else
                    Return Nothing
                End If
            End Get
        End Property

#End Region

        Private Function CalculatePL() As Decimal
            logger.Debug("CalculatePL, parameters:Nothing")
            Dim plOfDay As Decimal = 0
            If OrderDetails IsNot Nothing AndAlso OrderDetails.Count > 0 Then
                For Each parentOrderId In OrderDetails.Keys
                    Dim parentBusinessOrder As IBusinessOrder = OrderDetails(parentOrderId)
                    Dim calculateWithLTP As Boolean = False
                    If parentBusinessOrder.SLOrder IsNot Nothing AndAlso parentBusinessOrder.SLOrder.Count > 0 Then
                        'For Each slOrder In parentBusinessOrder.SLOrder
                        '    If slOrder.Status = "CANCELLED" OrElse slOrder.Status = "COMPLETE" Then
                        '        If slOrder.TransactionType = "BUY" Then
                        '            plOfDay += slOrder.AveragePrice * slOrder.Quantity * -1
                        '        ElseIf slOrder.TransactionType = "SELL" Then
                        '            plOfDay += slOrder.AveragePrice * slOrder.Quantity
                        '        End If
                        '    ElseIf Not slOrder.Status = "REJECTED" Then
                        '        calculateWithLTP = True
                        '    End If
                        'Next
                        calculateWithLTP = True
                    Else
                        'calculateWithLTP = True
                    End If
                    If parentBusinessOrder.TargetOrder IsNot Nothing AndAlso parentBusinessOrder.TargetOrder.Count > 0 Then
                        'For Each targetOrder In parentBusinessOrder.TargetOrder
                        '    If targetOrder.Status = "CANCELLED" OrElse targetOrder.Status = "COMPLETE" Then
                        '        If targetOrder.TransactionType = "BUY" Then
                        '            plOfDay += targetOrder.AveragePrice * targetOrder.Quantity * -1
                        '        ElseIf targetOrder.TransactionType = "SELL" Then
                        '            plOfDay += targetOrder.AveragePrice * targetOrder.Quantity
                        '        End If
                        '    ElseIf Not targetOrder.Status = "REJECTED" Then
                        '        calculateWithLTP = True
                        '    End If
                        'Next
                        calculateWithLTP = True
                    Else
                        'calculateWithLTP = True
                    End If

                    If parentBusinessOrder.AllOrder IsNot Nothing AndAlso parentBusinessOrder.AllOrder.Count > 0 Then
                        For Each order In parentBusinessOrder.AllOrder
                            If order.Status = "CANCELLED" OrElse order.Status = "COMPLETE" Then
                                If order.TransactionType = "BUY" Then
                                    plOfDay += order.AveragePrice * order.Quantity * -1
                                ElseIf order.TransactionType = "SELL" Then
                                    plOfDay += order.AveragePrice * order.Quantity
                                End If
                            ElseIf Not order.Status = "REJECTED" Then
                                calculateWithLTP = True
                            End If
                        Next
                    Else
                        calculateWithLTP = True
                    End If

                    If parentBusinessOrder.ParentOrder.TransactionType = "BUY" Then
                        plOfDay += parentBusinessOrder.ParentOrder.AveragePrice * parentBusinessOrder.ParentOrder.Quantity * -1
                    ElseIf parentBusinessOrder.ParentOrder.TransactionType = "SELL" Then
                        plOfDay += parentBusinessOrder.ParentOrder.AveragePrice * parentBusinessOrder.ParentOrder.Quantity
                    End If
                    If calculateWithLTP AndAlso parentBusinessOrder.ParentOrder.Status = "COMPLETE" Then
                        If parentBusinessOrder.ParentOrder.TransactionType = "BUY" Then
                            plOfDay += Me.LastPrice * parentBusinessOrder.ParentOrder.Quantity
                        ElseIf parentBusinessOrder.ParentOrder.TransactionType = "SELL" Then
                            plOfDay += Me.LastPrice * parentBusinessOrder.ParentOrder.Quantity * -1
                        End If
                    End If
                Next
                Return plOfDay
            Else
                Return Nothing
            End If
        End Function
        Public Overrides Function ToString() As String
            Return String.Format("{0}_{1}", ParentStrategy.ToString, TradableInstrument.ToString)
        End Function
        Public Overridable Function GenerateTag() As String
            Return Hex(Convert.ToDecimal(String.Format("{0}{1}", ParentStrategy.StrategyIdentifier, TradableInstrument.InstrumentIdentifier)))
        End Function
        Public Overridable Async Function HandleTickTriggerToUIETCAsync() As Task
            'logger.Debug("HandleTickTriggerToUIETCAsync, parameters:Nothing")
            Try
                Await Task.Delay(0).ConfigureAwait(False)

                If TradableInstrument.LastTick IsNot Nothing AndAlso TradableInstrument.LastTick.LastPrice <> _LastPrice Then NotifyPropertyChanged("LastPrice")
                If TradableInstrument.LastTick IsNot Nothing AndAlso TradableInstrument.LastTick.Tradable <> _Tradabale Then NotifyPropertyChanged("Tradable")
                If TradableInstrument.LastTick IsNot Nothing AndAlso TradableInstrument.LastTick.Open <> _OpenPrice Then NotifyPropertyChanged("OpenPrice")
                If TradableInstrument.LastTick IsNot Nothing AndAlso TradableInstrument.LastTick.High <> _HighPrice Then NotifyPropertyChanged("HighPrice")
                If TradableInstrument.LastTick IsNot Nothing AndAlso TradableInstrument.LastTick.Low <> _LowPrice Then NotifyPropertyChanged("LowPrice")
                If TradableInstrument.LastTick IsNot Nothing AndAlso TradableInstrument.LastTick.Close <> _ClosePrice Then NotifyPropertyChanged("ClosePrice")
                If TradableInstrument.LastTick IsNot Nothing AndAlso TradableInstrument.LastTick.Volume <> _Volume Then NotifyPropertyChanged("Volume")
                If TradableInstrument.LastTick IsNot Nothing AndAlso TradableInstrument.LastTick.AveragePrice <> _AveragePrice Then NotifyPropertyChanged("AveragePrice")
                If TradableInstrument.LastTick IsNot Nothing AndAlso TradableInstrument.LastTick.Timestamp <> _Timestamp Then NotifyPropertyChanged("Timestamp")
                If TradableInstrument.LastTick IsNot Nothing AndAlso TradableInstrument.LastTick.LastPrice <> _LastPrice Then NotifyPropertyChanged("PL")
                If TradableInstrument.LastTick IsNot Nothing AndAlso TradableInstrument.LastTick.Timestamp IsNot Nothing AndAlso TradableInstrument.LastTick.Timestamp.HasValue AndAlso Utilities.Time.IsDateTimeEqualTillMinutes(TradableInstrument.LastTick.Timestamp.Value, Me.LastCandleTime) Then NotifyPropertyChanged("LastCandleTime")
            Catch cex As OperationCanceledException
                logger.Error(cex)
                Me.ParentStrategy.ParentController.OrphanException = cex
            Catch ex As Exception
                logger.Error("Strategy Instrument:{0}, error:{1}", Me.ToString, ex.ToString)
                Throw ex
            End Try
        End Function
        Public Overridable Async Function PopulateChartAndIndicatorsAsync(ByVal candleCreator As Chart, ByVal currentCandle As OHLCPayload) As Task
            'logger.Debug("PopulateChartAndIndicatorsAsync, parameters:{0},{1}", candleCreator.ToString, currentCandle.ToString)
            If RawPayloadConsumers IsNot Nothing AndAlso RawPayloadConsumers.Count > 0 Then
                For Each runningRawPayloadConsumer In RawPayloadConsumers
                    If runningRawPayloadConsumer.TypeOfConsumer = IPayloadConsumer.ConsumerType.Chart Then
                        Await candleCreator.ConvertTimeframeAsync(CType(runningRawPayloadConsumer, PayloadToChartConsumer).Timeframe,
                                                                    currentCandle,
                                                                    runningRawPayloadConsumer).ConfigureAwait(False)
                    End If
                Next
            End If
        End Function

        Public Overridable Async Function ProcessOrderAsync(ByVal orderData As IBusinessOrder) As Task
            'logger.Debug("ProcessOrderAsync, parameters:{0}", Utilities.Strings.JsonSerialize(orderData))
            Await Task.Delay(0).ConfigureAwait(False)
            _cts.Token.ThrowIfCancellationRequested()
            If OrderDetails.ContainsKey(orderData.ParentOrderIdentifier) Then
                orderData.SignalCandle = OrderDetails(orderData.ParentOrderIdentifier).SignalCandle
            End If
            OrderDetails.AddOrUpdate(orderData.ParentOrderIdentifier, orderData, Function(key, value) orderData)
        End Function
        Protected Function CalculateBuffer(ByVal price As Double, ByVal floorOrCeiling As RoundOfType) As Double
            'logger.Debug("CalculateBuffer, parameters:{0},{1}", price, floorOrCeiling)
            Dim bufferPrice As Double = Nothing
            'Assuming 1% target, we can afford to have buffer as 2.5% of that 1% target
            bufferPrice = ConvertFloorCeling(price * 0.01 * 0.025, 0.05, floorOrCeiling)
            Return bufferPrice
        End Function
        Public MustOverride Async Function MonitorAsync() As Task
        Public Overridable Async Function ForceExitAllTradesAsync() As Task
            'logger.Debug("ForceExitAllTradesAsync, parameters:Nothing")
            Try
                Await ExecuteCommandAsync(ExecuteCommands.ForceCancelBOOrder, Nothing).ConfigureAwait(False)
                Await ExecuteCommandAsync(ExecuteCommands.ForceCancelCOOrder, Nothing).ConfigureAwait(False)
            Catch cex As OperationCanceledException
                logger.Error(cex)
                Me.ParentStrategy.ParentController.OrphanException = cex
            Catch ex As Exception
                logger.Error("Strategy Instrument:{0}, error:{1}", Me.ToString, ex.ToString)
                Throw ex
            End Try
        End Function
        Protected MustOverride Function IsTriggerReceivedForPlaceOrder() As Tuple(Of Boolean, PlaceOrderParameters)
        Protected MustOverride Function IsTriggerReceivedForModifyStoplossOrder() As List(Of Tuple(Of Boolean, String, Decimal))
        Protected MustOverride Function IsTriggerReceivedForExitOrder() As List(Of Tuple(Of Boolean, String, String))
        Protected Overridable Function GetAllActiveOrders(ByVal signalDirection As APIAdapter.TransactionType) As List(Of IOrder)
            Dim ret As List(Of IOrder) = Nothing
            Dim direction As String = Nothing
            If signalDirection = APIAdapter.TransactionType.Buy Then
                direction = "BUY"
            ElseIf signalDirection = APIAdapter.TransactionType.Sell Then
                direction = "SELL"
            End If
            If OrderDetails IsNot Nothing AndAlso OrderDetails.Count > 0 Then
                For Each parentOrderId In OrderDetails.Keys
                    Dim parentBusinessOrder As IBusinessOrder = OrderDetails(parentOrderId)
                    If direction Is Nothing OrElse parentBusinessOrder.ParentOrder.TransactionType.ToUpper = direction.ToUpper Then
                        If parentBusinessOrder.ParentOrder.Status = "COMPLETE" OrElse parentBusinessOrder.ParentOrder.Status = "OPEN" Then
                            If parentBusinessOrder.SLOrder IsNot Nothing AndAlso parentBusinessOrder.SLOrder.Count > 0 Then
                                Dim parentNeedToInsert As Boolean = False
                                For Each slOrder In parentBusinessOrder.SLOrder
                                    If Not slOrder.Status = "COMPLETE" AndAlso Not slOrder.Status = "CANCELLED" Then
                                        If ret Is Nothing Then ret = New List(Of IOrder)
                                        ret.Add(slOrder)
                                        parentNeedToInsert = True
                                    End If
                                Next
                                If parentNeedToInsert Then ret.Add(parentBusinessOrder.ParentOrder)
                            End If
                            If parentBusinessOrder.ParentOrder.Status = "OPEN" Then ret.Add(parentBusinessOrder.ParentOrder)
                        End If
                    End If
                Next
            End If
            Return ret
        End Function
        Protected Overridable Function GetActiveOrder(ByVal signalDirection As APIAdapter.TransactionType) As IBusinessOrder
            'logger.Debug("GetActiveOrder, parameters:Nothing")
            Dim ret As IBusinessOrder = Nothing
            Dim allActiveOrders As List(Of IOrder) = GetAllActiveOrders(signalDirection)
            If allActiveOrders IsNot Nothing AndAlso allActiveOrders.Count > 0 Then
                Dim parentOrders As List(Of IOrder) = allActiveOrders.FindAll(Function(x)
                                                                                  Return x.ParentOrderIdentifier Is Nothing
                                                                              End Function)
                If parentOrders IsNot Nothing AndAlso parentOrders.Count > 0 Then
                    ret = OrderDetails(parentOrders.FirstOrDefault.OrderIdentifier)
                End If
            End If
            Return ret
        End Function
        Protected Overridable Function GetAllCancelableOrders(ByVal signalDirection As APIAdapter.TransactionType) As List(Of Tuple(Of Boolean, String, String))
            Dim ret As List(Of Tuple(Of Boolean, String, String)) = Nothing
            Dim allActiveOrders As List(Of IOrder) = GetAllActiveOrders(signalDirection)
            If allActiveOrders IsNot Nothing AndAlso allActiveOrders.Count > 0 Then
                For Each activeOrder In allActiveOrders
                    If activeOrder.Status <> "COMPLETE" Then
                        If ret Is Nothing Then ret = New List(Of Tuple(Of Boolean, String, String))
                        ret.Add(New Tuple(Of Boolean, String, String)(True, activeOrder.OrderIdentifier, activeOrder.ParentOrderIdentifier))
                    End If
                Next
            End If
            Return ret
        End Function

        ''' <summary>
        ''' To run in diffrent thread it is not defined in Strategy level
        ''' </summary>
        ''' <param name="command"></param>
        ''' <param name="data"></param>
        ''' <returns></returns>
        Protected Async Function ExecuteCommandAsync(ByVal command As ExecuteCommands, ByVal data As Object) As Task(Of Object)
            'logger.Debug("ExecuteCommandAsync, parameters:{0},{1}", command, Utilities.Strings.JsonSerialize(data))
            Dim ret As Object = Nothing
            If RequestResponseForPlaceOrder Is Nothing Then RequestResponseForPlaceOrder = New Concurrent.ConcurrentDictionary(Of String, String)
            If RequestResponseForModifyOrder Is Nothing Then RequestResponseForModifyOrder = New Concurrent.ConcurrentDictionary(Of String, String)
            If RequestResponseForCancelOrder Is Nothing Then RequestResponseForCancelOrder = New Concurrent.ConcurrentDictionary(Of String, String)

            Dim lastException As Exception = Nothing
            Dim allOKWithoutException As Boolean = False

            Using Waiter As New Waiter(_cts)
                AddHandler Waiter.Heartbeat, AddressOf OnHeartbeat
                AddHandler Waiter.WaitingFor, AddressOf OnWaitingFor
                Dim apiConnectionBeingUsed As IConnection = Me.ParentStrategy.ParentController.APIConnection
                For retryCtr = 1 To _MaxReTries
                    _cts.Token.ThrowIfCancellationRequested()
                    lastException = Nothing
                    While Me.ParentStrategy.ParentController.APIConnection Is Nothing OrElse apiConnectionBeingUsed Is Nothing OrElse
                        (Me.ParentStrategy.ParentController.APIConnection IsNot Nothing AndAlso apiConnectionBeingUsed IsNot Nothing AndAlso
                        Not Me.ParentStrategy.ParentController.APIConnection.Equals(apiConnectionBeingUsed))
                        apiConnectionBeingUsed = Me.ParentStrategy.ParentController.APIConnection
                        _cts.Token.ThrowIfCancellationRequested()
                        logger.Debug("Waiting for fresh token before running command:{0}", command.ToString)
                        Await Task.Delay(500).ConfigureAwait(False)
                        _cts.Token.ThrowIfCancellationRequested()
                    End While
                    _APIAdapter.SetAPIAccessToken(Me.ParentStrategy.ParentController.APIConnection.AccessToken)

                    logger.Debug("Firing command:{0}", command.ToString)
                    OnDocumentRetryStatus(retryCtr, _MaxReTries)
                    Try
                        _cts.Token.ThrowIfCancellationRequested()
                        Select Case command
                            Case ExecuteCommands.PlaceBOLimitMISOrder
                                Dim placeOrderTrigger As Tuple(Of Boolean, PlaceOrderParameters) = IsTriggerReceivedForPlaceOrder()
                                If placeOrderTrigger IsNot Nothing AndAlso placeOrderTrigger.Item1 = True Then
                                    Dim placeOrderResponse As Dictionary(Of String, Object) = Nothing
                                    placeOrderResponse = Await _APIAdapter.PlaceBOLimitMISOrderAsync(tradeExchange:=Me.TradableInstrument.Exchange,
                                                                                                       tradingSymbol:=TradingSymbol,
                                                                                                       transaction:=placeOrderTrigger.Item2.EntryDirection,
                                                                                                       quantity:=placeOrderTrigger.Item2.Quantity,
                                                                                                       price:=placeOrderTrigger.Item2.Price,
                                                                                                       squareOffValue:=placeOrderTrigger.Item2.SquareOffValue,
                                                                                                       stopLossValue:=placeOrderTrigger.Item2.StoplossValue,
                                                                                                       tag:=placeOrderTrigger.Item2.Tag).ConfigureAwait(False)
                                    If placeOrderResponse IsNot Nothing Then
                                        logger.Debug("Place order is completed, placeOrderResponse:{0}", Strings.JsonSerialize(placeOrderResponse))
                                        RequestResponseForPlaceOrder.GetOrAdd(placeOrderTrigger.Item2.ToString(), placeOrderResponse("data")("order_id"))
                                        lastException = Nothing
                                        allOKWithoutException = True
                                        _cts.Token.ThrowIfCancellationRequested()
                                        ret = placeOrderResponse
                                        _cts.Token.ThrowIfCancellationRequested()
                                        Exit For
                                    Else
                                        Throw New ApplicationException(String.Format("Place order did not succeed"))
                                    End If
                                Else
                                    lastException = Nothing
                                    allOKWithoutException = True
                                    _cts.Token.ThrowIfCancellationRequested()
                                    Exit For
                                End If
                            Case ExecuteCommands.ModifyStoplossOrder
                                Dim modifyStoplossOrderTriggers As List(Of Tuple(Of Boolean, String, Decimal)) = IsTriggerReceivedForModifyStoplossOrder()
                                If modifyStoplossOrderTriggers IsNot Nothing AndAlso modifyStoplossOrderTriggers.Count > 0 Then
                                    For Each modifyStoplossOrderTrigger In modifyStoplossOrderTriggers
                                        If modifyStoplossOrderTrigger.Item1 = True Then
                                            Dim modifyStoplossOrderResponse As Dictionary(Of String, Object) = Nothing
                                            modifyStoplossOrderResponse = Await _APIAdapter.ModifyStoplossOrderAsync(orderId:=modifyStoplossOrderTrigger.Item2,
                                                                                                                     triggerPrice:=modifyStoplossOrderTrigger.Item3).ConfigureAwait(False)
                                            If modifyStoplossOrderResponse IsNot Nothing Then
                                                logger.Debug("Modify stoploss order is completed, modifyStoplossOrderResponse:{0}", Strings.JsonSerialize(modifyStoplossOrderResponse))
                                                lastException = Nothing
                                                allOKWithoutException = True
                                                _cts.Token.ThrowIfCancellationRequested()
                                                ret = modifyStoplossOrderResponse
                                                _cts.Token.ThrowIfCancellationRequested()
                                                Exit For
                                            Else
                                                Throw New ApplicationException(String.Format("Modify stoploss order did not succeed"))
                                            End If
                                        Else
                                            lastException = Nothing
                                            allOKWithoutException = True
                                            _cts.Token.ThrowIfCancellationRequested()
                                            Exit For
                                        End If
                                    Next
                                Else
                                    lastException = Nothing
                                    allOKWithoutException = True
                                    _cts.Token.ThrowIfCancellationRequested()
                                    Exit For
                                End If
                            Case ExecuteCommands.CancelBOOrder, ExecuteCommands.CancelCOOrder, ExecuteCommands.ForceCancelBOOrder, ExecuteCommands.ForceCancelCOOrder
                                Dim cancelOrderTriggers As List(Of Tuple(Of Boolean, String, String)) = Nothing
                                Select Case command
                                    Case ExecuteCommands.ForceCancelBOOrder, ExecuteCommands.ForceCancelCOOrder
                                        cancelOrderTriggers = GetAllCancelableOrders(APIAdapter.TransactionType.None)
                                    Case ExecuteCommands.CancelBOOrder, ExecuteCommands.CancelCOOrder
                                        cancelOrderTriggers = IsTriggerReceivedForExitOrder()
                                End Select
                                If cancelOrderTriggers IsNot Nothing AndAlso cancelOrderTriggers.Count > 0 Then
                                    For Each cancelOrderTrigger In cancelOrderTriggers
                                        If cancelOrderTrigger.Item1 = True Then
                                            Dim cancelOrderResponse As Dictionary(Of String, Object) = Nothing
                                            Select Case command
                                                Case ExecuteCommands.CancelBOOrder, ExecuteCommands.ForceCancelBOOrder
                                                    cancelOrderResponse = Await _APIAdapter.CancelBOOrderAsync(orderId:=cancelOrderTrigger.Item2,
                                                                                                       parentOrderID:=cancelOrderTrigger.Item3).ConfigureAwait(False)
                                                Case ExecuteCommands.CancelCOOrder, ExecuteCommands.ForceCancelCOOrder
                                                    cancelOrderResponse = Await _APIAdapter.CancelCOOrderAsync(orderId:=cancelOrderTrigger.Item2,
                                                                                                       parentOrderID:=cancelOrderTrigger.Item3).ConfigureAwait(False)
                                            End Select
                                            If cancelOrderResponse IsNot Nothing Then
                                                logger.Debug("Cancel order is completed, modifyStoplossOrderResponse:{0}", Strings.JsonSerialize(cancelOrderResponse))
                                                lastException = Nothing
                                                allOKWithoutException = True
                                                _cts.Token.ThrowIfCancellationRequested()
                                                ret = cancelOrderResponse
                                                _cts.Token.ThrowIfCancellationRequested()
                                                Exit For
                                            Else
                                                Throw New ApplicationException(String.Format("Cancel order did not succeed"))
                                            End If
                                        Else
                                            lastException = Nothing
                                            allOKWithoutException = True
                                            _cts.Token.ThrowIfCancellationRequested()
                                            Exit For
                                        End If
                                    Next
                                Else
                                    lastException = Nothing
                                    allOKWithoutException = True
                                    _cts.Token.ThrowIfCancellationRequested()
                                    Exit For
                                End If
                            Case ExecuteCommands.PlaceBOSLMISOrder
                                Dim placeOrderTrigger As Tuple(Of Boolean, PlaceOrderParameters) = IsTriggerReceivedForPlaceOrder()
                                If placeOrderTrigger IsNot Nothing AndAlso placeOrderTrigger.Item1 = True Then
                                    Dim placeOrderResponse As Dictionary(Of String, Object) = Nothing
                                    placeOrderResponse = Await _APIAdapter.PlaceBOSLMISOrderAsync(tradeExchange:=Me.TradableInstrument.Exchange,
                                                                                                    tradingSymbol:=TradingSymbol,
                                                                                                    transaction:=placeOrderTrigger.Item2.EntryDirection,
                                                                                                    quantity:=placeOrderTrigger.Item2.Quantity,
                                                                                                    price:=placeOrderTrigger.Item2.Price,
                                                                                                    triggerPrice:=placeOrderTrigger.Item2.TriggerPrice,
                                                                                                    squareOffValue:=placeOrderTrigger.Item2.SquareOffValue,
                                                                                                    stopLossValue:=placeOrderTrigger.Item2.StoplossValue,
                                                                                                    tag:=placeOrderTrigger.Item2.Tag).ConfigureAwait(False)
                                    If placeOrderResponse IsNot Nothing Then
                                        logger.Debug("Place order is completed, placeOrderResponse:{0}", Strings.JsonSerialize(placeOrderResponse))
                                        RequestResponseForPlaceOrder.GetOrAdd(placeOrderTrigger.Item2.ToString(), placeOrderResponse("data")("order_id"))
                                        lastException = Nothing
                                        allOKWithoutException = True
                                        _cts.Token.ThrowIfCancellationRequested()
                                        ret = placeOrderResponse
                                        _cts.Token.ThrowIfCancellationRequested()
                                        Exit For
                                    Else
                                        Throw New ApplicationException(String.Format("Place order did not succeed"))
                                    End If
                                Else
                                    lastException = Nothing
                                    allOKWithoutException = True
                                    _cts.Token.ThrowIfCancellationRequested()
                                    Exit For
                                End If
                            Case ExecuteCommands.PlaceCOMarketMISOrder
                                Dim placeOrderTrigger As Tuple(Of Boolean, PlaceOrderParameters) = IsTriggerReceivedForPlaceOrder()
                                If placeOrderTrigger IsNot Nothing AndAlso placeOrderTrigger.Item1 = True Then
                                    Dim placeOrderResponse As Dictionary(Of String, Object) = Nothing
                                    placeOrderResponse = Await _APIAdapter.PlaceCOMarketMISOrderAsync(tradeExchange:=Me.TradableInstrument.Exchange,
                                                                                                    tradingSymbol:=TradingSymbol,
                                                                                                    transaction:=placeOrderTrigger.Item2.EntryDirection,
                                                                                                    quantity:=placeOrderTrigger.Item2.Quantity,
                                                                                                    triggerPrice:=placeOrderTrigger.Item2.TriggerPrice,
                                                                                                    tag:=placeOrderTrigger.Item2.Tag).ConfigureAwait(False)
                                    If placeOrderResponse IsNot Nothing Then
                                        logger.Debug("Place order is completed, placeOrderResponse:{0}", Strings.JsonSerialize(placeOrderResponse))
                                        RequestResponseForPlaceOrder.GetOrAdd(placeOrderTrigger.Item2.ToString(), placeOrderResponse("data")("order_id"))
                                        lastException = Nothing
                                        allOKWithoutException = True
                                        _cts.Token.ThrowIfCancellationRequested()
                                        ret = placeOrderResponse
                                        _cts.Token.ThrowIfCancellationRequested()
                                        Exit For
                                    Else
                                        Throw New ApplicationException(String.Format("Place order did not succeed"))
                                    End If
                                Else
                                    lastException = Nothing
                                    allOKWithoutException = True
                                    _cts.Token.ThrowIfCancellationRequested()
                                    Exit For
                                End If
                        End Select
                    Catch aex As AdapterBusinessException
                        logger.Error(aex)
                        lastException = aex
                        Select Case aex.ExceptionType
                            Case AdapterBusinessException.TypeOfException.TokenException
                                Continue For
                            Case AdapterBusinessException.TypeOfException.DataException
                                Continue For
                            Case AdapterBusinessException.TypeOfException.NetworkException
                                Continue For
                            Case Else
                                Exit For
                        End Select
                    Catch opx As OperationCanceledException
                        logger.Error(opx)
                        lastException = opx
                        If Not _cts.Token.IsCancellationRequested Then
                            _cts.Token.ThrowIfCancellationRequested()
                            If Not Waiter.WaitOnInternetFailure(_WaitDurationOnConnectionFailure) Then
                                'Provide required wait in case internet was already up
                                logger.Debug("HTTP->Task was cancelled without internet problem:{0}",
                                             opx.Message)
                                _cts.Token.ThrowIfCancellationRequested()
                                Waiter.SleepRequiredDuration(_WaitDurationOnAnyFailure.TotalSeconds, "Non-explicit cancellation")
                                _cts.Token.ThrowIfCancellationRequested()
                            Else
                                logger.Debug("HTTP->Task was cancelled due to internet problem:{0}, waited prescribed seconds, will now retry",
                                             opx.Message)
                                'Since internet was down, no need to consume retries
                                retryCtr -= 1
                            End If
                        End If
                    Catch hex As HttpRequestException
                        logger.Error(hex)
                        lastException = hex
                        If ExceptionExtensions.GetExceptionMessages(hex).Contains("trust relationship") Then
                            Throw New ForbiddenException(hex.Message, hex, ForbiddenException.TypeOfException.PossibleReloginRequired)
                        End If
                        _cts.Token.ThrowIfCancellationRequested()
                        If Not Waiter.WaitOnInternetFailure(_WaitDurationOnConnectionFailure) Then
                            If hex.Message.Contains("429") Or hex.Message.Contains("503") Then
                                logger.Debug("HTTP->429/503 error without internet problem:{0}",
                                             hex.Message)
                                _cts.Token.ThrowIfCancellationRequested()
                                Waiter.SleepRequiredDuration(_WaitDurationOnServiceUnavailbleFailure.TotalSeconds, "Service unavailable(429/503)")
                                _cts.Token.ThrowIfCancellationRequested()
                                'Since site service is blocked, no need to consume retries
                                retryCtr -= 1
                            ElseIf hex.Message.Contains("404") Then
                                logger.Debug("HTTP->404 error without internet problem:{0}",
                                             hex.Message)
                                _cts.Token.ThrowIfCancellationRequested()
                                'No point retrying, exit for
                                Exit For
                            Else
                                If ExceptionExtensions.IsExceptionConnectionRelated(hex) Then
                                    logger.Debug("HTTP->HttpRequestException without internet problem but of type internet related detected:{0}",
                                                 hex.Message)
                                    _cts.Token.ThrowIfCancellationRequested()
                                    Waiter.SleepRequiredDuration(_WaitDurationOnConnectionFailure.TotalSeconds, "Connection HttpRequestException")
                                    _cts.Token.ThrowIfCancellationRequested()
                                    'Since exception was internet related, no need to consume retries
                                    retryCtr -= 1
                                Else
                                    'Provide required wait in case internet was already up
                                    logger.Debug("HTTP->HttpRequestException without internet problem:{0}",
                                                 hex.Message)
                                    _cts.Token.ThrowIfCancellationRequested()
                                    Waiter.SleepRequiredDuration(_WaitDurationOnAnyFailure.TotalSeconds, "Unknown HttpRequestException:" & hex.Message)
                                    _cts.Token.ThrowIfCancellationRequested()
                                End If
                            End If
                        Else
                            logger.Debug("HTTP->HttpRequestException with internet problem:{0}, waited prescribed seconds, will now retry",
                                         hex.Message)
                            'Since internet was down, no need to consume retries
                            retryCtr -= 1
                        End If
                    Catch ex As Exception
                        logger.Error(ex)
                        lastException = ex
                        'Exit if it is a network failure check and stop retry to avoid stack overflow
                        'Need to relogin, no point retrying
                        If ExceptionExtensions.GetExceptionMessages(ex).Contains("disposed") Then
                            Throw New ForbiddenException(ex.Message, ex, ForbiddenException.TypeOfException.ExceptionInBetweenLoginProcess)
                        End If
                        _cts.Token.ThrowIfCancellationRequested()
                        If Not Waiter.WaitOnInternetFailure(_WaitDurationOnConnectionFailure) Then
                            'Provide required wait in case internet was already up
                            _cts.Token.ThrowIfCancellationRequested()
                            If ExceptionExtensions.IsExceptionConnectionRelated(ex) Then
                                logger.Debug("HTTP->Exception without internet problem but of type internet related detected:{0}",
                                             ex.Message)
                                _cts.Token.ThrowIfCancellationRequested()
                                Waiter.SleepRequiredDuration(_WaitDurationOnConnectionFailure.TotalSeconds, "Connection Exception")
                                _cts.Token.ThrowIfCancellationRequested()
                                'Since exception was internet related, no need to consume retries
                                retryCtr -= 1
                            Else
                                logger.Debug("HTTP->Exception without internet problem of unknown type detected:{0}",
                                             ex.Message)
                                _cts.Token.ThrowIfCancellationRequested()
                                Waiter.SleepRequiredDuration(_WaitDurationOnAnyFailure.TotalSeconds, "Unknown Exception")
                                _cts.Token.ThrowIfCancellationRequested()
                            End If
                        Else
                            logger.Debug("HTTP->Exception with internet problem:{0}, waited prescribed seconds, will now retry",
                                         ex.Message)
                            'Since internet was down, no need to consume retries
                            retryCtr -= 1
                        End If
                    Finally
                        OnDocumentDownloadComplete()
                    End Try
                    _cts.Token.ThrowIfCancellationRequested()
                    If ret IsNot Nothing Then
                        Exit For
                    End If
                    GC.Collect()
                Next
                RemoveHandler Waiter.Heartbeat, AddressOf OnHeartbeat
                RemoveHandler Waiter.WaitingFor, AddressOf OnWaitingFor
            End Using
            _cts.Token.ThrowIfCancellationRequested()
            If Not allOKWithoutException Then Throw lastException
            Return ret
        End Function
        Public Class PlaceOrderParameters
            Public Property EntryDirection As APIAdapter.TransactionType = APIAdapter.TransactionType.None
            Public Property Quantity As Integer = 0
            Public Property Price As Decimal = Nothing
            Public Property TriggerPrice As Decimal = Nothing
            Public Property SquareOffValue As Decimal = Nothing
            Public Property StoplossValue As Decimal = Nothing
            Public Property Tag As String = Nothing
            Public Property SignalCandle As IPayload = Nothing
            Public Overrides Function ToString() As String
                Return String.Format("{0}{1}{2}{3}{4}{5}{6}", EntryDirection.ToString(), Price, TriggerPrice, SquareOffValue, StoplossValue, Tag, SignalCandle.ToString)
            End Function
        End Class
    End Class
End Namespace
