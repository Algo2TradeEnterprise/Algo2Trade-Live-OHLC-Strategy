Imports System.Threading
Imports Algo2TradeCore.Adapter
Imports Algo2TradeCore.Entities
Imports Algo2TradeCore.Strategies
Imports NLog


Namespace Controller
    Public MustInherit Class APIStrategyController

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

        Protected _currentUser As IUser
        Protected _cts As CancellationTokenSource
        Protected _MaxReTries As Integer = 20
        Protected _WaitDurationOnConnectionFailure As TimeSpan = TimeSpan.FromSeconds(5)
        Protected _WaitDurationOnServiceUnavailbleFailure As TimeSpan = TimeSpan.FromSeconds(30)
        Protected _WaitDurationOnAnyFailure As TimeSpan = TimeSpan.FromSeconds(10)
        Protected _LoginURL As String
        Protected _LoginThreads As Integer
        Public Property APIConnection As IConnection
        Protected _APIAdapter As APIAdapter
        Protected _AllInstruments As IEnumerable(Of IInstrument)
        Public Sub New(ByVal currentUser As IUser,
                       ByVal canceller As CancellationTokenSource)
            _currentUser = currentUser
            _cts = canceller
            _LoginThreads = 0
        End Sub
        Public MustOverride Function GetErrorResponse(ByVal responseDict As Object) As String

#Region "Login"
        Protected MustOverride Function GetLoginURL() As String
        Public MustOverride Async Function LoginAsync() As Task(Of IConnection)
        Public MustOverride Async Function ExecuteStrategyAsync(ByVal strategyToRun As Strategy) As Task
        Public MustOverride Async Function PrepareToRunStrategyAsync() As Task(Of Boolean)
#End Region

    End Class
End Namespace