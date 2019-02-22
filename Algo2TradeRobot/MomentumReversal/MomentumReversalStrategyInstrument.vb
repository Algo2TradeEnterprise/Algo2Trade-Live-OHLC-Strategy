Imports System.ComponentModel.DataAnnotations
Imports System.Threading
Imports Algo2TradeCore
Imports Algo2TradeCore.Adapter
Imports Algo2TradeCore.Entities
Imports Algo2TradeCore.Strategies
Imports NLog

Public Class MomentumReversalStrategyInstrument
    Inherits StrategyInstrument
    Implements IDisposable

#Region "Logging and Status Progress"
    Public Shared Shadows logger As Logger = LogManager.GetCurrentClassLogger
#End Region

    Private _MRStrategyProtector As Integer = 0
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
        RawPayloadConsumers.Add(New PayloadToChartConsumer(Me.ParentStrategy.UserSettings.SignalTimeFrame))
    End Sub
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
                If placeOrderTrigger IsNot Nothing AndAlso placeOrderTrigger.Item1 = True AndAlso _MRStrategyProtector = 0 Then
                    Interlocked.Increment(_MRStrategyProtector)
                    orderDetails = Await ExecuteCommandAsync(ExecuteCommands.PlaceBOLimitMISOrder, Nothing).ConfigureAwait(False)
                    Interlocked.Decrement(_MRStrategyProtector)
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
        If Me.RawPayloadConsumers IsNot Nothing AndAlso Me.RawPayloadConsumers.Count > 0 Then
            For Each runningRawPayloadConsumer In RawPayloadConsumers
                If runningRawPayloadConsumer.TypeOfConsumer = IPayloadConsumer.ConsumerType.Chart AndAlso
                    CType(runningRawPayloadConsumer, PayloadToChartConsumer).Timeframe = MRUserSettings.SignalTimeFrame Then
                    Dim XMinutePayloadConsumer As PayloadToChartConsumer = runningRawPayloadConsumer

                    Dim runningCandlePayload As OHLCPayload = Nothing
                    Dim lastExistingPayloads As IEnumerable(Of KeyValuePair(Of Date, OHLCPayload)) =
                        XMinutePayloadConsumer.ChartPayloads.Where(Function(y)
                                                                       Return Utilities.Time.IsDateTimeEqualTillMinutes(y.Key, XMinutePayloadConsumer.ChartPayloads.Keys.Max)
                                                                   End Function)

                    If lastExistingPayloads IsNot Nothing AndAlso lastExistingPayloads.Count > 0 Then runningCandlePayload = lastExistingPayloads.LastOrDefault.Value

                    If runningCandlePayload IsNot Nothing AndAlso runningCandlePayload.SnapshotDateTime >= tradeStartTime AndAlso runningCandlePayload.PreviousPayload IsNot Nothing Then
                        If runningCandlePayload.SnapshotDateTime >= tradeStartTime Then

                        End If
                    End If
                End If
            Next
        End If
        Return ret
    End Function
    Protected Overrides Function IsTriggerReceivedForModifyStoplossOrder() As List(Of Tuple(Of Boolean, String, Decimal))
        Dim ret As List(Of Tuple(Of Boolean, String, Decimal)) = Nothing
        Throw New NotImplementedException
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