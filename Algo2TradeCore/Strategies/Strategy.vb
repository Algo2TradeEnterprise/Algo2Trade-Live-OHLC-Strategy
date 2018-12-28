Imports System.Threading
Imports Algo2TradeCore.Adapter
Imports Algo2TradeCore.Controller
Imports Algo2TradeCore.Entities
Imports NLog

Namespace Strategies
    Public MustInherit Class Strategy

#Region "Events/Event handlers"
        Public Event DocumentDownloadComplete()
        Public Event DocumentRetryStatus(ByVal currentTry As Integer, ByVal totalTries As Integer)
        Public Event Heartbeat(ByVal msg As String)
        Public Event WaitingFor(ByVal elapsedSecs As Integer, ByVal totalSecs As Integer, ByVal msg As String)
        'The below functions are needed to allow the derived classes to raise the above two events
        Protected Overridable Sub OnDocumentDownloadComplete()
            RaiseEvent DocumentDownloadComplete()
        End Sub
        Protected Overridable Sub OnDocumentRetryStatus(ByVal currentTry As Integer, ByVal totalTries As Integer)
            RaiseEvent DocumentRetryStatus(currentTry, totalTries)
        End Sub
        Protected Overridable Sub OnHeartbeat(ByVal msg As String)
            RaiseEvent Heartbeat(msg)
        End Sub
        Protected Overridable Sub OnWaitingFor(ByVal elapsedSecs As Integer, ByVal totalSecs As Integer, ByVal msg As String)
            RaiseEvent WaitingFor(elapsedSecs, totalSecs, msg)
        End Sub
#End Region

#Region "Logging and Status Progress"
        Public Shared Shadows logger As Logger = LogManager.GetCurrentClassLogger
#End Region

        Public Property TradableStrategyInstruments As IEnumerable(Of StrategyInstrument)
        Public Property ParentContoller As APIStrategyController
        Protected _cts As CancellationTokenSource
        Public Sub New(ByVal associatedParentController As APIStrategyController, ByVal canceller As CancellationTokenSource)
            Me.ParentContoller = associatedParentController
            _cts = canceller
        End Sub
        Public MustOverride Async Function CreateTradableStrategyInstrumentsAsync(ByVal allInstruments As IEnumerable(Of IInstrument)) As Task(Of Boolean)
        Public MustOverride Overrides Function ToString() As String
        Public MustOverride Async Function ExecuteAsync() As Task
        Public MustOverride Async Function SubscribeAsync(ByVal usableTicker As APITicker) As Task
        Public MustOverride Async Function IsTriggerReachedAsync() As Task(Of Tuple(Of Boolean, Trigger))
    End Class
End Namespace
