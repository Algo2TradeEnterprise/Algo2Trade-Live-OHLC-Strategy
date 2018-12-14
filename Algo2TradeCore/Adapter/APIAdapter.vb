Imports System.Threading
Imports Algo2TradeCore.Entity
Imports Algo2TradeCore.Subscriber

Namespace Adapter
    Public MustInherit Class APIAdapter
        Protected _userId As String
        Protected _password As String
        Protected _cts As CancellationTokenSource

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
        Public Sub New(ByVal userId As String,
                       ByVal password As String,
                       ByVal canceller As CancellationTokenSource)
            _userId = userId
            _password = password
            _cts = canceller
        End Sub
        Public MustOverride Async Function LoginAsync() As Task(Of IConnection)
        Public MustOverride Async Function ConnectTickerAsync(ByVal subscriber As APIInstrumentSubscriber) As Task
    End Class
End Namespace