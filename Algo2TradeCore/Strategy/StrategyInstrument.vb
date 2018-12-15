Imports System.Threading
Imports Algo2TradeCore.Adapter
Imports Algo2TradeCore.Entity
Imports NLog

Namespace Strategy
    Public MustInherit Class StrategyInstrument

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

        Protected _cts As CancellationTokenSource
        Protected _apiAdapter As APIAdapter
        Public Property TradableInstrument As IInstrument
        Protected _ticks As Utilities.Collections.LimitedStack(Of ITick)
        Public Sub New(ByVal apiAdapter As APIAdapter, ByVal associatedInstrument As IInstrument, ByVal canceller As CancellationTokenSource)
            _apiAdapter = apiAdapter
            TradableInstrument = associatedInstrument
            _cts = canceller
            _ticks = New Utilities.Collections.LimitedStack(Of ITick)(5500) 'Assuming 6 ticks per second for 15 minuters
        End Sub
        Public Overridable Async Function ProcessTickAsync(ByVal tickData As ITick) As Task
            Await Task.Delay(0).ConfigureAwait(False)
            _ticks.Push(tickData)
        End Function
    End Class
End Namespace
