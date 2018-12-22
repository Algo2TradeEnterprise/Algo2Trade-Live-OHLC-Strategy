Imports System.Threading
Imports Algo2TradeCore.Adapter
Imports Algo2TradeCore.Entities
Imports NLog

Namespace Strategies
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
        Protected _apiConnection As IConnection
        Protected _parentStrategy As Strategy
        Public Property TradableInstrument As IInstrument

        Public Sub New(ByVal apiConnection As IConnection, ByVal associatedInstrument As IInstrument, ByVal parentStrategy As Strategy, ByVal canceller As CancellationTokenSource)
            _apiConnection = apiConnection
            TradableInstrument = associatedInstrument
            _parentStrategy = parentStrategy
            _cts = canceller
        End Sub
        Public MustOverride Overrides Function ToString() As String
        Public MustOverride Async Function RunDirectAsync() As Task
    End Class
End Namespace
