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
            If TradableInstrument IsNot Nothing Then
                RaiseEvent Heartbeat(String.Format("{0}:{1}", TradableInstrument.InstrumentIdentifier, msg))
            Else
                RaiseEvent Heartbeat(String.Format("{0}:{1}", "No instrument", msg))
            End If
        End Sub
        Protected Overridable Sub OnWaitingFor(ByVal elapsedSecs As Integer, ByVal totalSecs As Integer, ByVal msg As String)
            If TradableInstrument IsNot Nothing Then
                RaiseEvent WaitingFor(elapsedSecs, totalSecs, String.Format("{0}-{1}", TradableInstrument.InstrumentIdentifier, msg))
            Else
                RaiseEvent WaitingFor(elapsedSecs, totalSecs, String.Format("{0}-{1}", "No instrument", msg))
            End If
        End Sub
#End Region

#Region "Logging and Status Progress"
        Public Shared logger As Logger = LogManager.GetCurrentClassLogger
#End Region

        Protected _cts As CancellationTokenSource
        Public Property ParentStrategy As Strategy
        Public Property TradableInstrument As IInstrument
        Protected _LastTick As ITick
        Protected _APIAdapter As APIAdapter
        Public Sub New(ByVal associatedInstrument As IInstrument, ByVal associatedParentStrategy As Strategy, ByVal canceller As CancellationTokenSource)
            TradableInstrument = associatedInstrument
            Me.ParentStrategy = associatedParentStrategy
            _cts = canceller
        End Sub
        Public MustOverride Overrides Function ToString() As String
        Public MustOverride Async Function RunDirectAsync() As Task
        Public MustOverride Async Function IsTriggerReachedAsync() As Task(Of Tuple(Of Boolean, Trigger))
        Public MustOverride Async Function ProcessTickAsync(ByVal tickData As ITick) As Task
    End Class
End Namespace
