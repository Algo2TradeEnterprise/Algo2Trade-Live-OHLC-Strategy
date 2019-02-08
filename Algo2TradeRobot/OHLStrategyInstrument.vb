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

    <Display(Name:="OHL", Order:=10)>
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
        _APIAdapter = New ZerodhaAdapter(ParentStrategy.ParentController, _cts)
        AddHandler _APIAdapter.Heartbeat, AddressOf OnHeartbeat
        AddHandler _APIAdapter.WaitingFor, AddressOf OnWaitingFor
        AddHandler _APIAdapter.DocumentRetryStatus, AddressOf OnDocumentRetryStatus
        AddHandler _APIAdapter.DocumentDownloadComplete, AddressOf OnDocumentDownloadComplete
    End Sub
    Public Overrides Function ToString() As String
        Return String.Format("{0}_{1}", ParentStrategy.ToString, TradableInstrument.ToString)
    End Function
    Public Overrides Function GenerateTag() As String
        Return String.Format("{0}_{1}", ParentStrategy.StrategyIdentifier, TradableInstrument.TradingSymbol)
    End Function
    Public Overrides Async Function HandleTickTriggerToUIETCAsync() As Task
        'logger.Debug("ProcessTickAsync, tickData:{0}", Utilities.Strings.JsonSerialize(tickData))
        _cts.Token.ThrowIfCancellationRequested()
        '_LastTick = tickData
        NotifyPropertyChanged("OHL")
        Await MyBase.HandleTickTriggerToUIETCAsync().ConfigureAwait(False)
        _cts.Token.ThrowIfCancellationRequested()
    End Function
    Public Overrides Async Function ProcessOrderAsync(ByVal orderData As IBusinessOrder) As Task
        _cts.Token.ThrowIfCancellationRequested()
        Await MyBase.ProcessOrderAsync(orderData).ConfigureAwait(False)
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
                Dim placeOrderTrigger As Tuple(Of Boolean, PlaceOrderParameters) = IsTriggerReceivedForPlaceOrder()
                If placeOrderTrigger IsNot Nothing AndAlso placeOrderTrigger.Item1 = True AndAlso _OHLStrategyProtector = 0 Then
                    Interlocked.Increment(_OHLStrategyProtector)
                    Try
                        orderDetails = Await ExecuteCommandAsync(ExecuteCommands.PlaceBOLimtMISOrder, Nothing).ConfigureAwait(False)
                    Catch ex As Exception
                        logger.Error(ex)
                        Dim exceptionResponse As Tuple(Of String, Strategy.ExceptionResponse) = Me.ParentStrategy.GetKiteExceptionResponse(ex)
                        If exceptionResponse IsNot Nothing Then
                            Select Case exceptionResponse.Item2
                                Case Strategy.ExceptionResponse.Ignore
                                    OnHeartbeat(String.Format("{0}. Will not retry.", exceptionResponse.Item1))
                                Case Strategy.ExceptionResponse.Retry
                                    OnHeartbeat(String.Format("{0}. Will retry.", exceptionResponse.Item1))
                                Case Strategy.ExceptionResponse.NotKiteException
                                    Throw ex
                            End Select
                        End If
                    End Try
                End If
                _cts.Token.ThrowIfCancellationRequested()
                If slDelayCtr = 3 Then
                    slDelayCtr = 0
                    Dim modifyStoplossOrderTrigger As List(Of Tuple(Of Boolean, String, Decimal)) = IsTriggerReceivedForModifyStoplossOrder()
                    If modifyStoplossOrderTrigger IsNot Nothing AndAlso modifyStoplossOrderTrigger.Count > 0 Then
                        Try
                            Await ExecuteCommandAsync(ExecuteCommands.ModifyStoplossOrder, Nothing).ConfigureAwait(False)
                        Catch ex As Exception
                            logger.Error(ex)
                            Dim exceptionResponse As Tuple(Of String, Strategy.ExceptionResponse) = Me.ParentStrategy.GetKiteExceptionResponse(ex)
                            If exceptionResponse IsNot Nothing Then
                                Select Case exceptionResponse.Item2
                                    Case Strategy.ExceptionResponse.Ignore
                                        OnHeartbeat(String.Format("{0}. Will not retry.", exceptionResponse.Item1))
                                    Case Strategy.ExceptionResponse.Retry
                                        OnHeartbeat(String.Format("{0}. Will retry.", exceptionResponse.Item1))
                                    Case Strategy.ExceptionResponse.NotKiteException
                                        Throw ex
                                End Select
                            End If
                        End Try
                    End If
                End If
                _cts.Token.ThrowIfCancellationRequested()
                Await Task.Delay(1000)
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
        Dim currentTime As Date = Now
        If TradableInstrument.LastTick.Timestamp IsNot Nothing AndAlso
        currentTime.Hour = 9 AndAlso currentTime.Minute = 15 AndAlso currentTime.Second >= 10 Then
            Dim OHLTradePrice As Decimal = TradableInstrument.LastTick.LastPrice
            Dim buffer As Decimal = Math.Round(ConvertFloorCeling(OHLTradePrice * 0.005, Convert.ToDouble(TradableInstrument.TickSize), RoundOfType.Floor), 2)
            Dim entryPrice As Decimal = Nothing
            Dim target As Decimal = Nothing
            Dim stoploss As Decimal = Nothing
            Dim quantity As Integer = Nothing
            Dim tag As String = GenerateTag()
            If Math.Round(TradableInstrument.LastTick.Open, 0) = TradableInstrument.LastTick.High AndAlso
                TradableInstrument.LastTick.Open = TradableInstrument.LastTick.High Then
                entryPrice = OHLTradePrice - buffer
                quantity = Math.Floor(2000 * 13 / entryPrice)
                target = Math.Round(ConvertFloorCeling(OHLTradePrice * 0.005, Convert.ToDouble(TradableInstrument.TickSize), RoundOfType.Celing), 2)
                stoploss = If(Math.Abs(TradableInstrument.LastTick.Open - entryPrice) = 0, Convert.ToDouble(TradableInstrument.TickSize) * 2, Math.Abs(TradableInstrument.LastTick.Open - entryPrice))
                Dim parameters As New PlaceOrderParameters With
                {.EntryDirection = APIAdapter.TransactionType.Sell,
                .Quantity = quantity,
                .Price = entryPrice,
                .TriggerPrice = Nothing,
                .SquareOffValue = target,
                .StoplossValue = stoploss,
                .Tag = tag}
                ret = New Tuple(Of Boolean, PlaceOrderParameters)(True, parameters)
            ElseIf Math.Round(TradableInstrument.LastTick.Open, 0) = TradableInstrument.LastTick.Low AndAlso
                TradableInstrument.LastTick.Open = TradableInstrument.LastTick.Low Then
                entryPrice = OHLTradePrice + buffer
                quantity = Math.Floor(2000 * 13 / entryPrice)
                target = Math.Round(ConvertFloorCeling(OHLTradePrice * 0.005, Convert.ToDouble(TradableInstrument.TickSize), RoundOfType.Celing), 2)
                stoploss = If(Math.Abs(entryPrice - TradableInstrument.LastTick.Open) = 0, Convert.ToDouble(TradableInstrument.TickSize) * 2, Math.Abs(entryPrice - TradableInstrument.LastTick.Open))
                Dim parameters As New PlaceOrderParameters With
                    {.EntryDirection = APIAdapter.TransactionType.Buy,
                    .Quantity = quantity,
                    .Price = entryPrice,
                    .TriggerPrice = Nothing,
                    .SquareOffValue = target,
                    .StoplossValue = stoploss,
                    .Tag = tag}
                ret = New Tuple(Of Boolean, PlaceOrderParameters)(True, parameters)
            End If
        End If
        Return ret
    End Function
    Protected Overrides Function IsTriggerReceivedForModifyStoplossOrder() As List(Of Tuple(Of Boolean, String, Decimal))
        Dim ret As List(Of Tuple(Of Boolean, String, Decimal)) = Nothing
        If OrderDetails IsNot Nothing AndAlso OrderDetails.Count > 0 Then
            For Each parentOrderId In OrderDetails.Keys
                Dim parentBusinessOrder As IBusinessOrder = OrderDetails(parentOrderId)
                If parentBusinessOrder.ParentOrder.Status = "COMPLETE" AndAlso
                    parentBusinessOrder.SLOrder IsNot Nothing AndAlso parentBusinessOrder.SLOrder.Count > 0 Then
                    'If parentBusinessOrder.ParentOrder.Tag.Substring(GenerateTag().Count + 1) = "1" Then
                    Dim parentOrderPrice As Decimal = parentBusinessOrder.ParentOrder.AveragePrice
                    Dim triggerPrice As Decimal = TradableInstrument.LastTick.Open
                    Dim buffer As Decimal = CalculateBuffer(triggerPrice, RoundOfType.Floor)
                    Dim potentialStoplossPrice As Decimal = Nothing
                    For Each slOrder In parentBusinessOrder.SLOrder
                        If Not slOrder.Status = "COMPLETE" AndAlso Not slOrder.Status = "CANCELLED" AndAlso Not slOrder.Status = "REJECTED" Then
                            If parentBusinessOrder.ParentOrder.TransactionType = "BUY" Then
                                triggerPrice -= buffer
                                potentialStoplossPrice = Math.Round(ConvertFloorCeling(parentOrderPrice - parentOrderPrice * 0.005, Convert.ToDouble(TradableInstrument.TickSize), RoundOfType.Celing), 2)
                                If slOrder.TriggerPrice < potentialStoplossPrice Then
                                    triggerPrice = potentialStoplossPrice
                                End If
                            ElseIf parentBusinessOrder.ParentOrder.TransactionType = "SELL" Then
                                triggerPrice += buffer
                                potentialStoplossPrice = Math.Round(ConvertFloorCeling(parentOrderPrice + parentOrderPrice * 0.005, Convert.ToDouble(TradableInstrument.TickSize), RoundOfType.Celing), 2)
                                If slOrder.TriggerPrice > potentialStoplossPrice Then
                                    triggerPrice = potentialStoplossPrice
                                End If
                            End If
                            If slOrder.TriggerPrice <> triggerPrice Then
                                If ret Is Nothing Then ret = New List(Of Tuple(Of Boolean, String, Decimal))
                                ret.Add(New Tuple(Of Boolean, String, Decimal)(True, slOrder.OrderIdentifier, triggerPrice))
                            End If
                        End If
                    Next
                    'End If
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
