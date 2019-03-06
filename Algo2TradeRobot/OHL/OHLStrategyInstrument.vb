Imports System.ComponentModel
Imports System.ComponentModel.DataAnnotations
Imports System.Threading
Imports Algo2TradeCore
Imports Algo2TradeCore.Adapter
Imports Algo2TradeCore.Entities
Imports Algo2TradeCore.Strategies
Imports NLog
Imports Utilities.Numbers.NumberManipulation

Public Class OHLStrategyInstrument
    Inherits StrategyInstrument
    Implements IDisposable

#Region "Logging and Status Progress"
    Public Shared Shadows logger As Logger = LogManager.GetCurrentClassLogger
#End Region

    Private _OHLStrategyProtector As Integer = 0

    <Display(Name:="OHL", Order:=14)>
    Public ReadOnly Property OHL As String
        Get
            If Me.OpenPrice = Me.LowPrice Then
                Return "O=L"
            ElseIf Me.OpenPrice = Me.HighPrice Then
                Return "O=H"
            Else
                Return Nothing
            End If
        End Get
    End Property

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
    Public Overrides Async Function HandleTickTriggerToUIETCAsync() As Task
        'logger.Debug("ProcessTickAsync, tickData:{0}", Utilities.Strings.JsonSerialize(tickData))
        _cts.Token.ThrowIfCancellationRequested()
        '_LastTick = tickData
        NotifyPropertyChanged("OHL")
        Await MyBase.HandleTickTriggerToUIETCAsync().ConfigureAwait(False)
        _cts.Token.ThrowIfCancellationRequested()
    End Function
    Public Overrides Async Function MonitorAsync() As Task
        Try
            Dim slDelayCtr As Integer = 0
            While True
                If Me.ParentStrategy.ParentController.OrphanException IsNot Nothing Then
                    Throw Me.ParentStrategy.ParentController.OrphanException
                End If
                _cts.Token.ThrowIfCancellationRequested()
                Dim orderDetails As Object = Nothing
                Dim placeOrderTrigger As Tuple(Of ExecuteCommandAction, PlaceOrderParameters) = Await IsTriggerReceivedForPlaceOrderAsync().ConfigureAwait(False)
                If placeOrderTrigger IsNot Nothing AndAlso placeOrderTrigger.Item1 = ExecuteCommandAction.Take AndAlso _OHLStrategyProtector = 0 Then
                    Interlocked.Increment(_OHLStrategyProtector)
                    orderDetails = Await ExecuteCommandAsync(ExecuteCommands.PlaceBOLimitMISOrder, Nothing).ConfigureAwait(False)
                End If
                _cts.Token.ThrowIfCancellationRequested()
                If slDelayCtr = 10 Then
                    slDelayCtr = 0
                    Dim modifyStoplossOrderTrigger As List(Of Tuple(Of ExecuteCommandAction, IOrder, Decimal)) = Await IsTriggerReceivedForModifyStoplossOrderAsync().ConfigureAwait(False)
                    If modifyStoplossOrderTrigger IsNot Nothing AndAlso modifyStoplossOrderTrigger.Count > 0 Then
                        'Interlocked.Increment(_OHLStrategyProtector)
                        Await ExecuteCommandAsync(ExecuteCommands.ModifyStoplossOrder, Nothing).ConfigureAwait(False)
                    End If
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
    Protected Overrides Async Function IsTriggerReceivedForPlaceOrderAsync() As Task(Of Tuple(Of ExecuteCommandAction, PlaceOrderParameters))
        Await Task.Delay(0).ConfigureAwait(False)
        Dim ret As Tuple(Of ExecuteCommandAction, PlaceOrderParameters) = Nothing
        Dim currentTime As Date = Now
        If TradableInstrument.LastTick.Timestamp IsNot Nothing AndAlso
        currentTime.Hour = 9 AndAlso currentTime.Minute = 15 AndAlso currentTime.Second >= 10 Then
            Dim OHLTradePrice As Decimal = TradableInstrument.LastTick.LastPrice
            Dim buffer As Decimal = Math.Round(ConvertFloorCeling(OHLTradePrice * 0.003, Convert.ToDouble(TradableInstrument.TickSize), RoundOfType.Floor), 2)
            Dim entryPrice As Decimal = Nothing
            Dim target As Decimal = Nothing
            Dim stoploss As Decimal = Nothing
            Dim quantity As Integer = Nothing
            Dim tag As String = GenerateTag(Now)
            If Math.Round(TradableInstrument.LastTick.Open, 0) = TradableInstrument.LastTick.High AndAlso
                TradableInstrument.LastTick.Open = TradableInstrument.LastTick.High Then
                entryPrice = OHLTradePrice - buffer
                quantity = Math.Floor(2000 * 13 / entryPrice)
                target = Math.Round(ConvertFloorCeling(OHLTradePrice * 0.015, Convert.ToDouble(TradableInstrument.TickSize), RoundOfType.Celing), 2)
                stoploss = If(Math.Abs(TradableInstrument.LastTick.Open - entryPrice) = 0, Convert.ToDouble(TradableInstrument.TickSize) * 2, Math.Abs(TradableInstrument.LastTick.Open - entryPrice))
                Dim parameters As New PlaceOrderParameters With
                {.EntryDirection = APIAdapter.TransactionType.Sell,
                .Quantity = quantity,
                .Price = entryPrice,
                .TriggerPrice = Nothing,
                .SquareOffValue = target,
                .StoplossValue = stoploss,
                .Tag = tag}
                ret = New Tuple(Of ExecuteCommandAction, PlaceOrderParameters)(ExecuteCommandAction.Take, parameters)
            ElseIf Math.Round(TradableInstrument.LastTick.Open, 0) = TradableInstrument.LastTick.Low AndAlso
                TradableInstrument.LastTick.Open = TradableInstrument.LastTick.Low Then
                entryPrice = OHLTradePrice + buffer
                quantity = Math.Floor(2000 * 13 / entryPrice)
                target = Math.Round(ConvertFloorCeling(OHLTradePrice * 0.015, Convert.ToDouble(TradableInstrument.TickSize), RoundOfType.Celing), 2)
                stoploss = If(Math.Abs(entryPrice - TradableInstrument.LastTick.Open) = 0, Convert.ToDouble(TradableInstrument.TickSize) * 2, Math.Abs(entryPrice - TradableInstrument.LastTick.Open))
                Dim parameters As New PlaceOrderParameters With
                    {.EntryDirection = APIAdapter.TransactionType.Buy,
                    .Quantity = quantity,
                    .Price = entryPrice,
                    .TriggerPrice = Nothing,
                    .SquareOffValue = target,
                    .StoplossValue = stoploss,
                    .Tag = tag}
                ret = New Tuple(Of ExecuteCommandAction, PlaceOrderParameters)(ExecuteCommandAction.Take, parameters)
            End If
        End If
        Return ret
    End Function
    Protected Overrides Async Function IsTriggerReceivedForModifyStoplossOrderAsync() As Task(Of List(Of Tuple(Of ExecuteCommandAction, IOrder, Decimal)))
        Await Task.Delay(0).ConfigureAwait(False)
        Dim ret As List(Of Tuple(Of ExecuteCommandAction, IOrder, Decimal)) = Nothing
        If OrderDetails IsNot Nothing AndAlso OrderDetails.Count > 0 Then
            Dim currentTime As Date = Now
            For Each parentOrderId In OrderDetails.Keys
                Dim parentBusinessOrder As IBusinessOrder = OrderDetails(parentOrderId)
                If parentBusinessOrder.ParentOrder.Status = "COMPLETE" AndAlso
                    parentBusinessOrder.SLOrder IsNot Nothing AndAlso parentBusinessOrder.SLOrder.Count > 0 Then
                    'If parentBusinessOrder.ParentOrder.Tag.Substring(GenerateTag().Count + 1) = "1" Then
                    Dim parentOrderPrice As Decimal = parentBusinessOrder.ParentOrder.AveragePrice
                    Dim triggerPrice As Decimal = TradableInstrument.LastTick.Open
                    Dim buffer As Decimal = CalculateBuffer(triggerPrice, RoundOfType.Floor)
                    If parentBusinessOrder.ParentOrder.TransactionType = "BUY" Then
                        triggerPrice -= buffer
                    ElseIf parentBusinessOrder.ParentOrder.TransactionType = "SELL" Then
                        triggerPrice += buffer
                    End If

                    Dim potentialStoplossPrice As Decimal = Nothing
                    For Each slOrder In parentBusinessOrder.SLOrder
                        If Not slOrder.Status = "COMPLETE" AndAlso Not slOrder.Status = "CANCELLED" AndAlso Not slOrder.Status = "REJECTED" Then
                            If parentBusinessOrder.ParentOrder.TransactionType = "BUY" Then
                                potentialStoplossPrice = Math.Round(ConvertFloorCeling(parentOrderPrice - parentOrderPrice * 0.005, Convert.ToDouble(TradableInstrument.TickSize), RoundOfType.Celing), 2)
                                If currentTime.Hour = 9 AndAlso currentTime.Minute >= 16 AndAlso triggerPrice < potentialStoplossPrice Then
                                    triggerPrice = potentialStoplossPrice
                                End If
                            ElseIf parentBusinessOrder.ParentOrder.TransactionType = "SELL" Then
                                potentialStoplossPrice = Math.Round(ConvertFloorCeling(parentOrderPrice + parentOrderPrice * 0.005, Convert.ToDouble(TradableInstrument.TickSize), RoundOfType.Celing), 2)
                                If currentTime.Hour = 9 AndAlso currentTime.Minute >= 16 AndAlso triggerPrice > potentialStoplossPrice Then
                                    triggerPrice = potentialStoplossPrice
                                End If
                            End If
                            If slOrder.TriggerPrice <> triggerPrice Then
                                If ret Is Nothing Then ret = New List(Of Tuple(Of ExecuteCommandAction, IOrder, Decimal))
                                ret.Add(New Tuple(Of ExecuteCommandAction, IOrder, Decimal)(True, slOrder, triggerPrice))
                                'Else
                                '    Debug.WriteLine(String.Format("Stoploss modified {0} Quantity:{1}, ID:{2}", Me.GenerateTag(), slOrder.Quantity, slOrder.OrderIdentifier))
                            End If
                        End If
                    Next
                    'End If
                End If
            Next
        End If
        Return ret
    End Function
    Protected Overrides Function IsTriggerReceivedForExitOrder() As List(Of Tuple(Of Boolean, String, String))
        Dim ret As List(Of Tuple(Of Boolean, String, String)) = Nothing
        Throw New NotImplementedException
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
