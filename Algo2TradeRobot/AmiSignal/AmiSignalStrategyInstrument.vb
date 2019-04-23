Imports System.Threading
Imports Algo2TradeCore.Adapter
Imports Algo2TradeCore.Entities
Imports Algo2TradeCore.Strategies
Imports NLog

Public Class AmiSignalStrategyInstrument
    Inherits StrategyInstrument
    Implements IDisposable

#Region "Logging and Status Progress"
    Public Shared Shadows logger As Logger = LogManager.GetCurrentClassLogger
#End Region

    Public EntrySignals As Concurrent.ConcurrentDictionary(Of String, AmiSignal)
    Public TargetSignals As Concurrent.ConcurrentDictionary(Of String, AmiSignal)
    Public StoplossSignals As Concurrent.ConcurrentDictionary(Of String, AmiSignal)

    Public Sub New(ByVal associatedInstrument As IInstrument,
                   ByVal associatedParentStrategy As Strategy,
                   ByVal isPairInstrumnet As Boolean,
                   ByVal canceller As CancellationTokenSource)
        MyBase.New(associatedInstrument, associatedParentStrategy, isPairInstrumnet, canceller)
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
        If Me.ParentStrategy.IsStrategyCandleStickBased Then
            RawPayloadDependentConsumers = New List(Of IPayloadConsumer)
            If Me.ParentStrategy.UserSettings.SignalTimeFrame > 0 Then
                RawPayloadDependentConsumers.Add(New PayloadToChartConsumer(Me.ParentStrategy.UserSettings.SignalTimeFrame))
            Else
                Throw New ApplicationException(String.Format("Signal Timeframe is 0 or Nothing, does not adhere to the strategy:{0}", Me.ParentStrategy.ToString))
            End If
        End If
    End Sub
    Public Overrides Async Function ProcessOrderAsync(ByVal orderData As IBusinessOrder) As Task
        _cts.Token.ThrowIfCancellationRequested()
        Await MyBase.ProcessOrderAsync(orderData).ConfigureAwait(False)
        _cts.Token.ThrowIfCancellationRequested()
        DeleteProcessedOrderAsync(orderData)
    End Function
    Public Overrides Async Function MonitorAsync() As Task
        Try
            While True
                If Me.ParentStrategy.ParentController.OrphanException IsNot Nothing Then
                    Throw Me.ParentStrategy.ParentController.OrphanException
                End If
                _cts.Token.ThrowIfCancellationRequested()
                Dim orderDetails As Object = Nothing
                Dim placeOrderTrigger As Tuple(Of ExecuteCommandAction, PlaceOrderParameters, String) = Await IsTriggerReceivedForPlaceOrderAsync(False).ConfigureAwait(False)
                If placeOrderTrigger IsNot Nothing AndAlso placeOrderTrigger.Item1 = ExecuteCommandAction.Take Then
                    If EntrySignals IsNot Nothing AndAlso EntrySignals.Count > 0 AndAlso
                       EntrySignals.FirstOrDefault.Value.SignalCandle.SnapshotDateTime = placeOrderTrigger.Item2.SignalCandle.SnapshotDateTime AndAlso
                       placeOrderTrigger.Item2.Price = Decimal.MinValue AndAlso placeOrderTrigger.Item2.TriggerPrice = Decimal.MinValue Then
                        orderDetails = Await ExecuteCommandAsync(ExecuteCommands.PlaceNRMLMarketMISOrder, Nothing).ConfigureAwait(False)
                        EntrySignals.FirstOrDefault.Value.OrderTimestamp = Now()
                        EntrySignals.FirstOrDefault.Value.OrderID = orderDetails("data")("order_id")
                    End If
                    If TargetSignals IsNot Nothing AndAlso TargetSignals.Count > 0 AndAlso
                       TargetSignals.FirstOrDefault.Value.SignalCandle.SnapshotDateTime = placeOrderTrigger.Item2.SignalCandle.SnapshotDateTime AndAlso
                       placeOrderTrigger.Item2.Price <> Decimal.MinValue AndAlso placeOrderTrigger.Item2.TriggerPrice = Decimal.MinValue Then
                        orderDetails = Await ExecuteCommandAsync(ExecuteCommands.PlaceNRMLLimitMISOrder, Nothing).ConfigureAwait(False)
                        TargetSignals.FirstOrDefault.Value.OrderTimestamp = Now()
                        TargetSignals.FirstOrDefault.Value.OrderID = orderDetails("data")("order_id")
                    End If
                    If StoplossSignals IsNot Nothing AndAlso StoplossSignals.Count > 0 AndAlso
                       StoplossSignals.FirstOrDefault.Value.SignalCandle.SnapshotDateTime = placeOrderTrigger.Item2.SignalCandle.SnapshotDateTime AndAlso
                       placeOrderTrigger.Item2.Price = Decimal.MinValue AndAlso placeOrderTrigger.Item2.TriggerPrice <> Decimal.MinValue Then
                        orderDetails = Await ExecuteCommandAsync(ExecuteCommands.PlaceNRMLSLMMISOrder, Nothing).ConfigureAwait(False)
                        StoplossSignals.FirstOrDefault.Value.OrderTimestamp = Now()
                        StoplossSignals.FirstOrDefault.Value.OrderID = orderDetails("data")("order_id")
                    End If
                End If
                _cts.Token.ThrowIfCancellationRequested()
                Await Task.Delay(1000, _cts.Token).ConfigureAwait(False)
            End While
        Catch ex As Exception
            'To log exceptions getting created from this function as the bubble up of the exception
            'will anyways happen to Strategy.MonitorAsync but it will not be shown until all tasks exit
            logger.Error("Strategy Instrument:{0}, error:{1}", Me.ToString, ex.ToString)
            Throw ex
        End Try
    End Function
    Protected Overrides Async Function IsTriggerReceivedForPlaceOrderAsync(ByVal forcePrint As Boolean) As Task(Of Tuple(Of ExecuteCommandAction, PlaceOrderParameters, String))
        Dim ret As Tuple(Of ExecuteCommandAction, PlaceOrderParameters, String) = Nothing
        Await Task.Delay(0, _cts.Token).ConfigureAwait(False)
        Dim amiUserSettings As AmiSignalUserInputs = Me.ParentStrategy.UserSettings

        Dim parameters As PlaceOrderParameters = Nothing
        If Now >= amiUserSettings.TradeStartTime AndAlso Now <= amiUserSettings.LastTradeEntryTime Then
            If EntrySignals IsNot Nothing AndAlso EntrySignals.Count > 0 Then
                Dim currentEntrySignal As AmiSignal = EntrySignals.FirstOrDefault.Value
                If currentEntrySignal.OrderType = TypeOfOrder.Market AndAlso currentEntrySignal.OrderTimestamp = Date.MinValue Then
                    parameters = New PlaceOrderParameters(currentEntrySignal.SignalCandle) With
                                 {
                                    .EntryDirection = currentEntrySignal.Direction,
                                    .Quantity = currentEntrySignal.Quantity
                                 }
                End If
            ElseIf TargetSignals IsNot Nothing AndAlso TargetSignals.Count > 0 Then
                Dim currentTargetSignal As AmiSignal = TargetSignals.FirstOrDefault.Value
                If currentTargetSignal.OrderType = TypeOfOrder.Limit AndAlso currentTargetSignal.OrderTimestamp = Date.MinValue Then
                    parameters = New PlaceOrderParameters(currentTargetSignal.SignalCandle) With
                                 {
                                    .EntryDirection = currentTargetSignal.Direction,
                                    .Quantity = currentTargetSignal.Quantity,
                                    .Price = currentTargetSignal.Price
                                 }
                End If
            ElseIf StoplossSignals IsNot Nothing AndAlso StoplossSignals.Count > 0 Then
                Dim currentStoplossSignal As AmiSignal = StoplossSignals.FirstOrDefault.Value
                If currentStoplossSignal.OrderType = TypeOfOrder.SLM AndAlso currentStoplossSignal.OrderTimestamp = Date.MinValue Then
                    parameters = New PlaceOrderParameters(currentStoplossSignal.SignalCandle) With
                                 {
                                    .EntryDirection = currentStoplossSignal.Direction,
                                    .Quantity = currentStoplossSignal.Quantity,
                                    .TriggerPrice = currentStoplossSignal.Price
                                 }
                End If
            End If
        End If

        'Below portion have to be done in every place order trigger
        If parameters IsNot Nothing Then
            Dim currentSignalActivities As IEnumerable(Of ActivityDashboard) = Me.ParentStrategy.SignalManager.GetSignalActivities(parameters.SignalCandle.SnapshotDateTime, Me.TradableInstrument.InstrumentIdentifier)
            If currentSignalActivities IsNot Nothing AndAlso currentSignalActivities.Count > 0 Then
                If currentSignalActivities.FirstOrDefault.EntryActivity.RequestStatus = ActivityDashboard.SignalStatusType.Discarded AndAlso
                    currentSignalActivities.FirstOrDefault.EntryActivity.LastException IsNot Nothing AndAlso
                    currentSignalActivities.FirstOrDefault.EntryActivity.LastException.Message.ToUpper.Contains("TIME") Then
                    ret = New Tuple(Of ExecuteCommandAction, PlaceOrderParameters, String)(ExecuteCommandAction.WaitAndTake, parameters, "Condition Satisfied")
                ElseIf currentSignalActivities.FirstOrDefault.EntryActivity.RequestStatus = ActivityDashboard.SignalStatusType.Discarded Then
                    ret = New Tuple(Of ExecuteCommandAction, PlaceOrderParameters, String)(ExecuteCommandAction.Take, parameters, "Condition Satisfied")
                    'ElseIf currentSignalActivities.FirstOrDefault.EntryActivity.RequestStatus = ActivityDashboard.SignalStatusType.Rejected Then
                    '    ret = New Tuple(Of ExecuteCommandAction, PlaceOrderParameters)(ExecuteCommandAction.Take, parameters)
                Else
                    ret = New Tuple(Of ExecuteCommandAction, PlaceOrderParameters, String)(ExecuteCommandAction.DonotTake, Nothing, "Condition Satisfied")
                End If
            Else
                ret = New Tuple(Of ExecuteCommandAction, PlaceOrderParameters, String)(ExecuteCommandAction.Take, parameters, "Condition Satisfied")
            End If
        Else
            ret = New Tuple(Of ExecuteCommandAction, PlaceOrderParameters, String)(ExecuteCommandAction.DonotTake, Nothing, "")
        End If
        Return ret
    End Function
    Protected Overrides Async Function IsTriggerReceivedForModifyStoplossOrderAsync(ByVal forcePrint As Boolean) As Task(Of List(Of Tuple(Of ExecuteCommandAction, IOrder, Decimal, String)))
        Dim ret As List(Of Tuple(Of ExecuteCommandAction, IOrder, Decimal, String)) = Nothing
        Await Task.Delay(0, _cts.Token).ConfigureAwait(False)
        Throw New NotImplementedException
        Return ret
    End Function
    Protected Overrides Async Function IsTriggerReceivedForExitOrderAsync(ByVal forcePrint As Boolean) As Task(Of List(Of Tuple(Of ExecuteCommandAction, IOrder, String)))
        Dim ret As List(Of Tuple(Of ExecuteCommandAction, IOrder, String)) = Nothing
        Await Task.Delay(0, _cts.Token).ConfigureAwait(False)
        Throw New NotImplementedException
        Return ret
    End Function
    Protected Overrides Async Function ForceExitSpecificTradeAsync(order As IOrder, ByVal reason As String) As Task
        If order IsNot Nothing AndAlso Not order.Status = "COMPLETE" Then
            Dim cancellableOrder As New List(Of Tuple(Of ExecuteCommandAction, IOrder, String)) From
            {
                New Tuple(Of ExecuteCommandAction, IOrder, String)(ExecuteCommandAction.Take, order, reason)
            }
            Await ExecuteCommandAsync(ExecuteCommands.ForceCancelBOOrder, cancellableOrder).ConfigureAwait(False)
        End If
    End Function

    Private Async Function DeleteProcessedOrderAsync(ByVal orderData As IBusinessOrder) As Task
        'logger.Debug("DeleteProcessedOrderAsync, parameters:{0}", Utilities.Strings.JsonSerialize(orderData))
        Await Task.Delay(0).ConfigureAwait(False)
        _cts.Token.ThrowIfCancellationRequested()
        Try
            If EntrySignals IsNot Nothing AndAlso EntrySignals.Count > 0 Then
                Dim entrySignal As AmiSignal = EntrySignals.FirstOrDefault.Value
                If entrySignal.OrderID IsNot Nothing AndAlso
                    orderData.ParentOrderIdentifier.ToUpper = entrySignal.OrderID.ToUpper AndAlso
                    Not entrySignal.OrderTimestamp = Date.MinValue Then
                    EntrySignals.TryRemove(Me.TradableInstrument.InstrumentIdentifier, entrySignal)
                End If
            End If

            If TargetSignals IsNot Nothing AndAlso TargetSignals.Count > 0 Then
                Dim targetSignal As AmiSignal = TargetSignals.FirstOrDefault.Value
                If targetSignal.OrderID IsNot Nothing AndAlso
                    orderData.ParentOrderIdentifier.ToUpper = targetSignal.OrderID.ToUpper AndAlso
                    Not targetSignal.OrderTimestamp = Date.MinValue Then
                    TargetSignals.TryRemove(Me.TradableInstrument.InstrumentIdentifier, targetSignal)
                End If
            End If

            If StoplossSignals IsNot Nothing AndAlso StoplossSignals.Count > 0 Then
                Dim stoplossSignal As AmiSignal = StoplossSignals.FirstOrDefault.Value
                If stoplossSignal.OrderID IsNot Nothing AndAlso
                    orderData.ParentOrderIdentifier.ToUpper = stoplossSignal.OrderID.ToUpper AndAlso
                    Not stoplossSignal.OrderTimestamp = Date.MinValue Then
                    StoplossSignals.TryRemove(Me.TradableInstrument.InstrumentIdentifier, stoplossSignal)
                End If
            End If
        Catch ex As Exception
            logger.Error(ex)
        End Try
        'If ExitSignals IsNot Nothing AndAlso ExitSignals.Count > 0 Then
        '    Dim exitSignal As AmiSignal = ExitSignals.FirstOrDefault.Value
        '    If Not exitSignal.OrderTimestamp = Date.MinValue Then
        '        ExitSignals.TryRemove(Me.TradableInstrument.InstrumentIdentifier, exitSignal)
        '    End If
        'End If
    End Function

    Public Async Function PopulateExternalSignalAsync(ByVal signal As String) As Task
        logger.Info("PopulateExternalSignalAsync, parameters:{0}", signal)
        Await Task.Delay(0, _cts.Token).ConfigureAwait(False)
        Try
            Dim currentSignal As AmiSignal = Nothing
            If EntrySignals Is Nothing Then EntrySignals = New Concurrent.ConcurrentDictionary(Of String, AmiSignal)
            If TargetSignals Is Nothing Then TargetSignals = New Concurrent.ConcurrentDictionary(Of String, AmiSignal)
            If StoplossSignals Is Nothing Then StoplossSignals = New Concurrent.ConcurrentDictionary(Of String, AmiSignal)
            Dim signalarr() As String = signal.Trim.Split(" ")
            Dim returnedSignal As AmiSignal = Nothing

            Dim dummyPayload As OHLCPayload = Nothing

            Select Case signalarr(2).ToUpper()
                Case "BUY"
                    currentSignal = New AmiSignal
                    dummyPayload = New OHLCPayload(OHLCPayload.PayloadSource.None) With {.SnapshotDateTime = Now}
                    With currentSignal
                        .InstrumentIdentifier = Me.TradableInstrument.InstrumentIdentifier
                        .Direction = APIAdapter.TransactionType.Buy
                        .OrderType = TypeOfOrder.Market
                        .Price = signalarr(4)
                        .Quantity = signalarr(5)
                        .SignalCandle = dummyPayload
                    End With
                    returnedSignal = EntrySignals.GetOrAdd(currentSignal.InstrumentIdentifier, currentSignal)
                    If Not returnedSignal.SignalCandle.SnapshotDateTime = currentSignal.SignalCandle.SnapshotDateTime Then
                        logger.Error(String.Format("{0} - Previous signal still exists", Me.TradableInstrument.TradingSymbol))
                    End If
                Case "SELL"
                    Select Case signalarr(3).ToUpper()
                        Case "LIMIT"
                            currentSignal = New AmiSignal
                            dummyPayload = New OHLCPayload(OHLCPayload.PayloadSource.None) With {.SnapshotDateTime = Now.AddSeconds(3)}
                            With currentSignal
                                .InstrumentIdentifier = Me.TradableInstrument.InstrumentIdentifier
                                .Direction = APIAdapter.TransactionType.Sell
                                .OrderType = TypeOfOrder.Limit
                                .Price = signalarr(4)
                                .Quantity = signalarr(5)
                                .SignalCandle = dummyPayload
                            End With
                            returnedSignal = TargetSignals.GetOrAdd(currentSignal.InstrumentIdentifier, currentSignal)
                            If Not returnedSignal.SignalCandle.SnapshotDateTime = currentSignal.SignalCandle.SnapshotDateTime Then
                                logger.Error(String.Format("{0} - Previous signal still exists", Me.TradableInstrument.TradingSymbol))
                            End If
                        Case "SL-M", "SLM"
                            currentSignal = New AmiSignal
                            dummyPayload = New OHLCPayload(OHLCPayload.PayloadSource.None) With {.SnapshotDateTime = Now.AddSeconds(6)}
                            With currentSignal
                                .InstrumentIdentifier = Me.TradableInstrument.InstrumentIdentifier
                                .Direction = APIAdapter.TransactionType.Sell
                                .OrderType = TypeOfOrder.SLM
                                .Price = signalarr(4)
                                .Quantity = signalarr(5)
                                .SignalCandle = dummyPayload
                            End With
                            returnedSignal = StoplossSignals.GetOrAdd(currentSignal.InstrumentIdentifier, currentSignal)
                            If Not returnedSignal.SignalCandle.SnapshotDateTime = currentSignal.SignalCandle.SnapshotDateTime Then
                                logger.Error(String.Format("{0} - Previous signal still exists", Me.TradableInstrument.TradingSymbol))
                            End If
                        Case Else
                            logger.Error(String.Format("{0} Invalid Signal Details. {1}", Me.TradableInstrument.TradingSymbol, signal))
                    End Select
                Case "SHORT"
                    currentSignal = New AmiSignal
                    dummyPayload = New OHLCPayload(OHLCPayload.PayloadSource.None) With {.SnapshotDateTime = Now}
                    With currentSignal
                        .InstrumentIdentifier = Me.TradableInstrument.InstrumentIdentifier
                        .Direction = APIAdapter.TransactionType.Sell
                        .OrderType = TypeOfOrder.Market
                        .Price = signalarr(4)
                        .Quantity = signalarr(5)
                        .SignalCandle = dummyPayload
                    End With
                    returnedSignal = EntrySignals.GetOrAdd(currentSignal.InstrumentIdentifier, currentSignal)
                    If Not returnedSignal.SignalCandle.SnapshotDateTime = currentSignal.SignalCandle.SnapshotDateTime Then
                        logger.Error(String.Format("{0} - Previous signal still exists", Me.TradableInstrument.TradingSymbol))
                    End If
                Case "COVER"
                    Select Case signalarr(3).ToUpper()
                        Case "LIMIT"
                            currentSignal = New AmiSignal
                            dummyPayload = New OHLCPayload(OHLCPayload.PayloadSource.None) With {.SnapshotDateTime = Now.AddSeconds(3)}
                            With currentSignal
                                .InstrumentIdentifier = Me.TradableInstrument.InstrumentIdentifier
                                .Direction = APIAdapter.TransactionType.Buy
                                .OrderType = TypeOfOrder.Limit
                                .Price = signalarr(4)
                                .Quantity = signalarr(5)
                                .SignalCandle = dummyPayload
                            End With
                            returnedSignal = TargetSignals.GetOrAdd(currentSignal.InstrumentIdentifier, currentSignal)
                            If Not returnedSignal.SignalCandle.SnapshotDateTime = currentSignal.SignalCandle.SnapshotDateTime Then
                                logger.Error(String.Format("{0} - Previous signal still exists", Me.TradableInstrument.TradingSymbol))
                            End If
                        Case "SL-M", "SLM"
                            currentSignal = New AmiSignal
                            dummyPayload = New OHLCPayload(OHLCPayload.PayloadSource.None) With {.SnapshotDateTime = Now.AddSeconds(6)}
                            With currentSignal
                                .InstrumentIdentifier = Me.TradableInstrument.InstrumentIdentifier
                                .Direction = APIAdapter.TransactionType.Buy
                                .OrderType = TypeOfOrder.SLM
                                .Price = signalarr(4)
                                .Quantity = signalarr(5)
                                .SignalCandle = dummyPayload
                            End With
                            returnedSignal = StoplossSignals.GetOrAdd(currentSignal.InstrumentIdentifier, currentSignal)
                            If Not returnedSignal.SignalCandle.SnapshotDateTime = currentSignal.SignalCandle.SnapshotDateTime Then
                                logger.Error(String.Format("{0} - Previous signal still exists", Me.TradableInstrument.TradingSymbol))
                            End If
                        Case Else
                            logger.Error(String.Format("{0} Invalid Signal Details. {1}", Me.TradableInstrument.TradingSymbol, signal))
                    End Select
                Case Else
                    logger.Error(String.Format("{0} Invalid Signal Details. {1}", Me.TradableInstrument.TradingSymbol, signal))
            End Select
        Catch ex As Exception
            logger.Error(ex)
        End Try
    End Function

#Region "AmiSignal"
    <Serializable>
    Public Class AmiSignal
        Public InstrumentIdentifier As String
        Public Direction As APIAdapter.TransactionType
        Public OrderType As TypeOfOrder
        Public Price As Decimal
        Public Quantity As Integer
        Public SignalCandle As OHLCPayload
        Public OrderTimestamp As Date = Date.MinValue
        Public OrderID As String
    End Class
    Public Enum TypeOfOrder
        Market = 1
        Limit
        SLM
        None
    End Enum
#End Region

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