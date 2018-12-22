Imports System.Threading
Imports Algo2TradeCore.Entities
Imports NLog
Imports Algo2TradeCore.Controller
Namespace Adapter
    Public MustInherit Class APIAdapter
        Protected _cts As CancellationTokenSource
        Protected _controller As APIStrategyController
        Protected _MaxInstrumentPerTicker As Integer

        Public ReadOnly Property MaxInstrumentPerTicker As Integer
            Get
                Return _MaxInstrumentPerTicker
            End Get
        End Property
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
        Public Shared logger As Logger = LogManager.GetCurrentClassLogger
#End Region

        Public Sub New(ByVal controller As APIStrategyController,
                       ByVal canceller As CancellationTokenSource)
            _controller = controller
            _cts = canceller
        End Sub
        'Public MustOverride Async Function ConnectTickerAsync(ByVal subscriber As APIInstrumentSubscriber) As Task
        Public MustOverride Async Function GetAllInstrumentsAsync() As Task(Of IEnumerable(Of IInstrument))
        'Public MustOverride Async Function GetAllTradesAsync(Optional ByVal tradeData As Dictionary(Of String, Object) = Nothing, Optional ByVal retryEnabled As Boolean = True) As Task(Of IEnumerable(Of ITrade))
    End Class
End Namespace