﻿Imports System.ComponentModel
Imports System.ComponentModel.DataAnnotations
Imports System.Threading
Imports Algo2TradeCore
Imports Algo2TradeCore.Adapter
Imports Algo2TradeCore.Entities
Imports Algo2TradeCore.Strategies
Imports NLog
Imports Utilities.Numbers.NumberManipulation

Public Class AmiSignalStrategyInstrument
    Inherits StrategyInstrument
    Implements IDisposable

#Region "Logging and Status Progress"
    Public Shared Shadows logger As Logger = LogManager.GetCurrentClassLogger
#End Region

    'Private _StrategyProtector As Integer = 1
    Public EntrySignals As Concurrent.ConcurrentDictionary(Of String, AmiSignal)
    Public ExitSignals As Concurrent.ConcurrentDictionary(Of String, AmiSignal)

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
    End Sub
    Public Overrides Async Function ProcessOrderAsync(ByVal orderData As IBusinessOrder) As Task
        _cts.Token.ThrowIfCancellationRequested()
        Await MyBase.ProcessOrderAsync(orderData).ConfigureAwait(False)
        _cts.Token.ThrowIfCancellationRequested()
        DeleteProcessedOrderAsync(orderData)
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
                Dim placeOrderTrigger As Tuple(Of Boolean, PlaceOrderParameters) = Await IsTriggerReceivedForPlaceOrderAsync().ConfigureAwait(False)
                If placeOrderTrigger IsNot Nothing AndAlso placeOrderTrigger.Item1 = True Then
                    'Interlocked.Increment(_StrategyProtector)
                    orderDetails = Await ExecuteCommandAsync(ExecuteCommands.PlaceBOLimitMISOrder, Nothing).ConfigureAwait(False)
                    EntrySignals.FirstOrDefault.Value.OrderTimestamp = Now()
                    EntrySignals.FirstOrDefault.Value.OrderID = orderDetails("data")("order_id")
                End If
                _cts.Token.ThrowIfCancellationRequested()
                'TODO: Delete slDelayCtr
                If slDelayCtr = 5 Then
                    slDelayCtr = 0
                    'Dim exitOrderDetails As Object = Nothing
                    Dim exitOrderTrigger As List(Of Tuple(Of Boolean, String, String)) = Await IsTriggerReceivedForExitOrderAsync().ConfigureAwait(False)
                    If exitOrderTrigger IsNot Nothing AndAlso exitOrderTrigger.Count > 0 Then
                        Await ExecuteCommandAsync(ExecuteCommands.CancelBOOrder, Nothing).ConfigureAwait(False)
                        ExitSignals.FirstOrDefault.Value.OrderTimestamp = Now()
                    End If
                End If
                _cts.Token.ThrowIfCancellationRequested()
                slDelayCtr += 1
                Await Task.Delay(1000)
            End While
        Catch ex As Exception
            'To log exceptions getting created from this function as the bubble up of the exception
            'will anyways happen to Strategy.MonitorAsync but it will not be shown until all tasks exit
            logger.Error("Strategy Instrument:{0}, error:{1}", Me.ToString, ex.ToString)
            Throw ex
        End Try
    End Function
    Protected Overrides Async Function IsTriggerReceivedForPlaceOrderAsync() As Task(Of Tuple(Of Boolean, PlaceOrderParameters))
        Await Task.Delay(0).ConfigureAwait(False)
        Dim ret As Tuple(Of Boolean, PlaceOrderParameters) = Nothing
        If Me.EntrySignals IsNot Nothing AndAlso Me.EntrySignals.Count = 1 Then
            Dim currentEntrySignal As AmiSignal = EntrySignals.FirstOrDefault.Value
            If currentEntrySignal.SignalType = TypeOfSignal.Entry AndAlso currentEntrySignal.OrderTimestamp = Date.MinValue Then
                Dim amiSignalTradePrice As Decimal = TradableInstrument.LastTick.LastPrice
                Dim quantity As Integer = Nothing
                Dim triggerPrice As Decimal = Nothing
                Dim tag As String = GenerateTag()
                Dim amiUserSettings As AmiSignalUserInputs = Me.ParentStrategy.UserSettings
                If Me.TradableInstrument.InstrumentType.ToUpper = "FUT" Then
                    quantity = Me.TradableInstrument.LotSize
                Else
                    'quantity = (amiUserSettings.MaxCapitalPerTrade * amiUserSettings.MaxStoplossPercentage / 100) / (amiSignalTradePrice * amiUserSettings.MaxStoplossPercentage / 100)
                    quantity = 1
                End If

                If currentEntrySignal.Direction = APIAdapter.TransactionType.Buy Then
                    triggerPrice = amiSignalTradePrice - amiSignalTradePrice * amiUserSettings.MaxStoplossPercentage / 100
                    triggerPrice = Math.Round(ConvertFloorCeling(triggerPrice, Convert.ToDouble(TradableInstrument.TickSize), RoundOfType.Celing), 2)
                    Dim parameters As New PlaceOrderParameters With
                       {.EntryDirection = APIAdapter.TransactionType.Buy,
                       .Quantity = quantity,
                       .Price = Nothing,
                       .TriggerPrice = Nothing,
                       .SquareOffValue = Nothing,
                       .StoplossValue = Nothing,
                       .Tag = tag}
                    ret = New Tuple(Of Boolean, PlaceOrderParameters)(True, parameters)
                ElseIf currentEntrySignal.Direction = APIAdapter.TransactionType.Sell Then
                    triggerPrice = amiSignalTradePrice + amiSignalTradePrice * amiUserSettings.MaxStoplossPercentage / 100
                    triggerPrice = Math.Round(ConvertFloorCeling(triggerPrice, Convert.ToDouble(TradableInstrument.TickSize), RoundOfType.Celing), 2)
                    Dim parameters As New PlaceOrderParameters With
                      {.EntryDirection = APIAdapter.TransactionType.Sell,
                      .Quantity = quantity,
                      .Price = Nothing,
                      .TriggerPrice = triggerPrice,
                      .SquareOffValue = Nothing,
                      .StoplossValue = Nothing,
                      .Tag = tag}
                    ret = New Tuple(Of Boolean, PlaceOrderParameters)(True, parameters)
                End If
            End If
        End If
        Return ret
    End Function
    Protected Overrides Async Function IsTriggerReceivedForModifyStoplossOrderAsync() As Task(Of List(Of Tuple(Of Boolean, String, Decimal)))
        Await Task.Delay(0).ConfigureAwait(False)
        Dim ret As List(Of Tuple(Of Boolean, String, Decimal)) = Nothing
        Throw New NotImplementedException
        Return ret
    End Function
    Protected Overrides Async Function IsTriggerReceivedForExitOrderAsync() As Task(Of List(Of Tuple(Of Boolean, String, String)))
        Dim ret As List(Of Tuple(Of Boolean, String, String)) = Nothing
        If Me.ExitSignals IsNot Nothing AndAlso Me.ExitSignals.Count = 1 Then
            Dim currentExitSignal As AmiSignal = ExitSignals.FirstOrDefault.Value
            Dim allActiveOrders As List(Of IOrder) = Nothing
            If currentExitSignal.Direction = APIAdapter.TransactionType.Buy Then
                ret = Await GetAllCancelableOrdersAsync(APIAdapter.TransactionType.Sell).ConfigureAwait(False)
            ElseIf currentExitSignal.Direction = APIAdapter.TransactionType.Sell Then
                ret = Await GetAllCancelableOrdersAsync(APIAdapter.TransactionType.Buy).ConfigureAwait(False)
            End If
        Else
            Return Nothing
        End If
        Return ret
    End Function

    Private Async Function DeleteProcessedOrderAsync(ByVal orderData As IBusinessOrder) As Task
        'logger.Debug("DeleteProcessedOrderAsync, parameters:{0}", Utilities.Strings.JsonSerialize(orderData))
        Await Task.Delay(0).ConfigureAwait(False)
        _cts.Token.ThrowIfCancellationRequested()

        If EntrySignals IsNot Nothing AndAlso EntrySignals.Count > 0 Then
            Dim entrySignal As AmiSignal = EntrySignals.FirstOrDefault.Value
            If orderData.ParentOrderIdentifier.ToUpper = entrySignal.OrderID.ToUpper AndAlso Not entrySignal.OrderTimestamp = Date.MinValue Then
                logger.Info("Parent Order Quantity: {0}, Filled Quantity:{1}", orderData.ParentOrder.Quantity, orderData.ParentOrder.FilledQuantity)
                EntrySignals.TryRemove(Me.TradableInstrument.InstrumentIdentifier, entrySignal)
            End If
        End If

        If ExitSignals IsNot Nothing AndAlso ExitSignals.Count > 0 Then
            Dim exitSignal As AmiSignal = ExitSignals.FirstOrDefault.Value
            If Not exitSignal.OrderTimestamp = Date.MinValue Then
                ExitSignals.TryRemove(Me.TradableInstrument.InstrumentIdentifier, exitSignal)
            End If
        End If
    End Function


    Public Async Function PopulateExternalSignalAsync(ByVal signal As String) As Task
        'logger.Debug("PopulateExternalSignalAsync, parameters:{0}", signal)
        Await Task.Delay(0).ConfigureAwait(False)
        Dim currentSignal As AmiSignal = Nothing
        If EntrySignals Is Nothing Then EntrySignals = New Concurrent.ConcurrentDictionary(Of String, AmiSignal)
        If ExitSignals Is Nothing Then ExitSignals = New Concurrent.ConcurrentDictionary(Of String, AmiSignal)
        Dim signalarr() As String = signal.Trim.Split(" ")
        Dim returnedSignal As AmiSignal = Nothing
        If Me.ParentStrategy.ActiveInstruments >= CType(Me.ParentStrategy.UserSettings, AmiSignalUserInputs).NumberOfTrade Then
            logger.Error(New ApplicationException(String.Format("{0} {1} Number of trade is running. So this signal cannot execute. {2}", Me.TradableInstrument.InstrumentIdentifier, Me.ParentStrategy.ActiveInstruments, signal)))
            Exit Function
        End If

        Select Case signalarr(0).ToUpper()
            Case "BUY"
                currentSignal = New AmiSignal
                With currentSignal
                    .Direction = APIAdapter.TransactionType.Buy
                    .InstrumentIdentifier = Me.TradableInstrument.InstrumentIdentifier
                    .SignalType = TypeOfSignal.Entry
                    .SignalTimestamp = Now()
                End With
                If Me.ActiveInstrument Then
                    If GetActiveOrderAsync(APIAdapter.TransactionType.Buy) IsNot Nothing Then
                        logger.Info(New ApplicationException(String.Format("{0} Buy signal running", Me.TradableInstrument.InstrumentIdentifier)))
                        Exit Function
                    End If
                    Dim exitSignal As Boolean = False
                    While Me.ActiveInstrument
                        If Not exitSignal Then
                            exitSignal = Await GenerateExitSignalAsync().ConfigureAwait(False)
                        End If
                        Await Task.Delay(100).ConfigureAwait(False)
                    End While
                End If
                returnedSignal = EntrySignals.GetOrAdd(currentSignal.InstrumentIdentifier, currentSignal)
                If Not returnedSignal.SignalTimestamp = currentSignal.SignalTimestamp Then
                    logger.Error(New ApplicationException(String.Format("{0} Previous signal still exists", Me.TradableInstrument.InstrumentIdentifier)))
                End If
                'Interlocked.Decrement(_StrategyProtector)
            Case "SELL"
                currentSignal = New AmiSignal
                With currentSignal
                    .Direction = APIAdapter.TransactionType.Sell
                    .InstrumentIdentifier = Me.TradableInstrument.InstrumentIdentifier
                    .SignalType = TypeOfSignal.Exit
                    .SignalTimestamp = Now()
                End With
                returnedSignal = ExitSignals.GetOrAdd(currentSignal.InstrumentIdentifier, currentSignal)
                If Not returnedSignal.SignalTimestamp = currentSignal.SignalTimestamp Then
                    logger.Error(New ApplicationException(String.Format("{0} Previous signal still exists", Me.TradableInstrument.InstrumentIdentifier)))
                End If
            Case "SHORT"
                currentSignal = New AmiSignal
                With currentSignal
                    .Direction = APIAdapter.TransactionType.Sell
                    .InstrumentIdentifier = Me.TradableInstrument.InstrumentIdentifier
                    .SignalType = TypeOfSignal.Entry
                    .SignalTimestamp = Now()
                End With
                If Me.ActiveInstrument Then
                    If GetActiveOrderAsync(APIAdapter.TransactionType.Sell) IsNot Nothing Then
                        logger.Info(New ApplicationException(String.Format("{0} Short signal running", Me.TradableInstrument.InstrumentIdentifier)))
                        Exit Function
                    End If
                    Dim exitSignal As Boolean = False
                    While Me.ActiveInstrument
                        If Not exitSignal Then
                            exitSignal = Await GenerateExitSignalAsync().ConfigureAwait(False)
                        End If
                    End While
                End If
                returnedSignal = EntrySignals.GetOrAdd(currentSignal.InstrumentIdentifier, currentSignal)
                If Not returnedSignal.SignalTimestamp = currentSignal.SignalTimestamp Then
                    logger.Error(New ApplicationException(String.Format("{0} Previous signal still exists", Me.TradableInstrument.InstrumentIdentifier)))
                End If
                'Interlocked.Decrement(_StrategyProtector)
            Case "COVER"
                currentSignal = New AmiSignal
                With currentSignal
                    .Direction = APIAdapter.TransactionType.Buy
                    .InstrumentIdentifier = Me.TradableInstrument.InstrumentIdentifier
                    .SignalType = TypeOfSignal.Exit
                    .SignalTimestamp = Now()
                End With
                returnedSignal = ExitSignals.GetOrAdd(currentSignal.InstrumentIdentifier, currentSignal)
                If Not returnedSignal.SignalTimestamp = currentSignal.SignalTimestamp Then
                    logger.Error(New ApplicationException(String.Format("{0} Previous signal still exists", Me.TradableInstrument.InstrumentIdentifier)))
                End If
            Case Else
                logger.Error(New ApplicationException(String.Format("{0} Invalid Signal Details. {1}", Me.TradableInstrument.InstrumentIdentifier, signal)))
        End Select
    End Function

    Private Async Function GenerateExitSignalAsync() As Task(Of Boolean)
        'logger.Debug("GenerateExitSignal, parameters:Nothing")
        Dim ret As Boolean = False
        Dim runningOrder As IBusinessOrder = GetActiveOrderAsync(APIAdapter.TransactionType.Buy)
        If runningOrder IsNot Nothing Then
            Await PopulateExternalSignalAsync(String.Format("SELL {0}", Me.TradingSymbol)).ConfigureAwait(False)
            ret = True
        Else
            runningOrder = GetActiveOrderAsync(APIAdapter.TransactionType.Sell)
            If runningOrder IsNot Nothing Then
                Await PopulateExternalSignalAsync(String.Format("COVER {0}", Me.TradingSymbol)).ConfigureAwait(False)
                ret = True
            End If
        End If
        Return ret
    End Function

#Region "AmiSignal"
    <Serializable>
    Public Class AmiSignal
        Public InstrumentIdentifier As String
        Public Direction As APIAdapter.TransactionType
        Public SignalType As TypeOfSignal
        Public SignalTimestamp As Date
        Public OrderTimestamp As Date = Date.MinValue
        Public OrderID As String
        Public Overrides Function Equals(obj As Object) As Boolean
            Dim compareWith As AmiSignal = obj
            With Me
                Return .InstrumentIdentifier = compareWith.InstrumentIdentifier And
                .Direction = compareWith.Direction And
                .SignalType = compareWith.SignalType
            End With
        End Function
    End Class
    Public Enum TypeOfSignal
        Entry = 1
        [Exit]
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
