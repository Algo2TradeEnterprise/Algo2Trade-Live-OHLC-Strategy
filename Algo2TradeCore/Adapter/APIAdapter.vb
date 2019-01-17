Imports System.Threading
Imports Algo2TradeCore.Entities
Imports NLog
Imports Algo2TradeCore.Controller
Namespace Adapter
    Public MustInherit Class APIAdapter
        Protected _cts As CancellationTokenSource
        Public Property ParentController As APIStrategyController

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

        Public Sub New(ByVal associatedParentController As APIStrategyController,
                       ByVal canceller As CancellationTokenSource)
            Me.ParentController = associatedParentController
            _cts = canceller
        End Sub
        Public MustOverride Async Function GetAllInstrumentsAsync() As Task(Of IEnumerable(Of IInstrument))
        Public MustOverride Async Function GetAllTradesAsync() As Task(Of IEnumerable(Of ITrade))
        Public MustOverride Async Function GetAllQuotes(ByVal instruments As IEnumerable(Of IInstrument)) As Task(Of IEnumerable(Of IQuote))
        Public MustOverride Sub SetAPIAccessToken(ByVal apiAccessToken As String)
        Public Enum ExecutionCommands
            GetPositions = 1
            GetQuotes
            PlaceOrder
            ModifyOrderQuantity
            ModifyOrderPrice
            ModifyTargetOrderPrice
            ModifySLOrderPrice
            CancelOrder
            GetOrderHistory
            GetOrders
            GetOrderTrades
            GetInstruments '
            InvalidateAccessToken
            GenerateSession
            None
        End Enum
    End Class
End Namespace