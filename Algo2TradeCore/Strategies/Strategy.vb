Imports System.Threading
Imports Algo2TradeCore.Adapter
Imports Algo2TradeCore.Controller
Imports Algo2TradeCore.Entities
Imports NLog

Namespace Strategies
    Public MustInherit Class Strategy

#Region "Events/Event handlers"
        'This will launch the Ex events so that source is included, but will handle normal events from all the objects that it calls and convert into Ex events
        Public Event DocumentDownloadCompleteEx(ByVal source As List(Of Object))
        Public Event DocumentRetryStatusEx(ByVal currentTry As Integer, ByVal totalTries As Integer, ByVal source As List(Of Object))
        Public Event HeartbeatEx(ByVal msg As String, ByVal source As List(Of Object))
        Public Event WaitingForEx(ByVal elapsedSecs As Integer, ByVal totalSecs As Integer, ByVal msg As String, ByVal source As List(Of Object))
        'The below functions are needed to allow the derived classes to raise the above two events
        Protected Overridable Sub OnDocumentDownloadCompleteEx(ByVal source As List(Of Object))
            If source IsNot Nothing Then source = New List(Of Object)
            source.Add(Me)
            RaiseEvent DocumentDownloadCompleteEx(source)
        End Sub
        Protected Overridable Sub OnDocumentRetryStatusEx(ByVal currentTry As Integer, ByVal totalTries As Integer, ByVal source As List(Of Object))
            If source IsNot Nothing Then source = New List(Of Object)
            source.Add(Me)
            RaiseEvent DocumentRetryStatusEx(currentTry, totalTries, source)
        End Sub
        Protected Overridable Sub OnHeartbeatEx(ByVal msg As String, ByVal source As List(Of Object))
            If source IsNot Nothing Then source = New List(Of Object)
            source.Add(Me)
            RaiseEvent HeartbeatEx(msg, source)
        End Sub
        Protected Overridable Sub OnWaitingForEx(ByVal elapsedSecs As Integer, ByVal totalSecs As Integer, ByVal msg As String, ByVal source As List(Of Object))
            If source IsNot Nothing Then source = New List(Of Object)
            source.Add(Me)
            RaiseEvent WaitingForEx(elapsedSecs, totalSecs, msg, source)
        End Sub
        Protected Overridable Sub OnDocumentDownloadComplete()
            RaiseEvent DocumentDownloadCompleteEx(New List(Of Object) From {Me})
        End Sub
        Protected Overridable Sub OnDocumentRetryStatus(ByVal currentTry As Integer, ByVal totalTries As Integer)
            RaiseEvent DocumentRetryStatusEx(currentTry, totalTries, New List(Of Object) From {Me})
        End Sub
        Protected Overridable Sub OnHeartbeat(ByVal msg As String)
            RaiseEvent HeartbeatEx(msg, New List(Of Object) From {Me})
        End Sub
        Protected Overridable Sub OnWaitingFor(ByVal elapsedSecs As Integer, ByVal totalSecs As Integer, ByVal msg As String)
            RaiseEvent WaitingForEx(elapsedSecs, totalSecs, msg, New List(Of Object) From {Me})
        End Sub
#End Region

#Region "Logging and Status Progress"
        Public Shared Shadows logger As Logger = LogManager.GetCurrentClassLogger
#End Region
        Public ReadOnly Property StrategyIdentifier As String
        Public Property TradableInstrumentsAsPerStrategy As IEnumerable(Of IInstrument)
        Public Property TradableStrategyInstruments As IEnumerable(Of StrategyInstrument)
        Public ReadOnly Property ActiveInstruments As Integer
            Get
                Dim instrumentCount As Integer = 0
                If TradableStrategyInstruments IsNot Nothing AndAlso TradableStrategyInstruments.Count > 0 Then
                    For Each runningStrategyInstrument In TradableStrategyInstruments
                        If runningStrategyInstrument.ActiveInstrument Then
                            instrumentCount += 1
                        End If
                    Next
                End If
                Return instrumentCount
            End Get
        End Property
        Public ReadOnly Property TotalPL As Decimal
            Get
                Dim plOfDay As Decimal = 0
                If TradableStrategyInstruments IsNot Nothing AndAlso TradableStrategyInstruments.Count > 0 Then
                    For Each runningStrategyInstrument In TradableStrategyInstruments
                        plOfDay += runningStrategyInstrument.PL
                    Next
                End If
                Return plOfDay
            End Get
        End Property
        Public Property ParentContoller As APIStrategyController
        Protected _cts As CancellationTokenSource
        Public Sub New(ByVal associatedParentController As APIStrategyController, ByVal canceller As CancellationTokenSource, ByVal associatedStrategyIdentifier As String)
            Me.ParentContoller = associatedParentController
            _cts = canceller
            Me.StrategyIdentifier = associatedStrategyIdentifier
        End Sub
        Public MustOverride Async Function CreateTradableStrategyInstrumentsAsync(ByVal allInstruments As IEnumerable(Of IInstrument)) As Task(Of Boolean)
        Public MustOverride Async Function SubscribeAsync(ByVal usableTicker As APITicker) As Task
        Public MustOverride Overrides Function ToString() As String
        'Public MustOverride Async Function ExecuteAsync() As Task
        Public MustOverride Async Function IsTriggerReachedAsync() As Task(Of Tuple(Of Boolean, Trigger))
        Public MustOverride Async Function MonitorAsync() As Task
        Public Overridable Async Function FillOrderDetailsAsync() As Task
            Try
                While True
                    If Me.ParentContoller.OrphanException IsNot Nothing Then
                        Throw Me.ParentContoller.OrphanException
                    End If
                    _cts.Token.ThrowIfCancellationRequested()
                    Await Me.ParentContoller.FillOrderDetailsAsyc(Me).ConfigureAwait(False)
                    Await Task.Delay(10000).ConfigureAwait(False)
                End While
            Catch ex As Exception
                logger.Error("Strategy:{0}, error:{1}", Me.ToString, ex.ToString)
            End Try
        End Function
        Public Overridable Async Function ExitAllTrades() As Task
            Try
                While True
                    If Me.ParentContoller.OrphanException IsNot Nothing Then
                        Throw Me.ParentContoller.OrphanException
                    End If
                    _cts.Token.ThrowIfCancellationRequested()
                    If IsTriggerReceivedForExitAllOrders() AndAlso TradableStrategyInstruments IsNot Nothing AndAlso TradableStrategyInstruments.Count > 0 Then
                        For Each runningStrategyInstrument In TradableStrategyInstruments
                            runningStrategyInstrument.ExitAllTrades()
                        Next
                    End If
                End While
            Catch ex As Exception
                logger.Error("Strategy:{0}, error:{1}", Me.ToString, ex.ToString)
            End Try
        End Function
        Protected MustOverride Function IsTriggerReceivedForExitAllOrders() As Boolean
        Public Overridable Function GetKiteExceptionResponse(ByVal ex As Exception) As Tuple(Of String, ExceptionResponse)
            Dim ret As Tuple(Of String, ExceptionResponse) = Nothing
            If ex.GetType = GetType(KiteConnect.GeneralException) Then
                ret = New Tuple(Of String, ExceptionResponse)(ex.Message, ExceptionResponse.Ignore)
            ElseIf ex.GetType = GetType(KiteConnect.GeneralException) Then
                ret = New Tuple(Of String, ExceptionResponse)(ex.Message, ExceptionResponse.Ignore)
            ElseIf ex.GetType = GetType(KiteConnect.PermissionException) Then
                ret = New Tuple(Of String, ExceptionResponse)(ex.Message, ExceptionResponse.Ignore)
            ElseIf ex.GetType = GetType(KiteConnect.OrderException) Then
                ret = New Tuple(Of String, ExceptionResponse)(ex.Message, ExceptionResponse.Ignore)
            ElseIf ex.GetType = GetType(KiteConnect.InputException) Then
                ret = New Tuple(Of String, ExceptionResponse)(ex.Message, ExceptionResponse.Ignore)
            ElseIf ex.GetType = GetType(KiteConnect.DataException) Then
                ret = New Tuple(Of String, ExceptionResponse)(ex.Message, ExceptionResponse.Ignore)
            ElseIf ex.GetType = GetType(KiteConnect.NetworkException) Then
                ret = New Tuple(Of String, ExceptionResponse)(ex.Message, ExceptionResponse.Ignore)
            Else
                ret = New Tuple(Of String, ExceptionResponse)(ex.Message, ExceptionResponse.NotKiteException)
            End If
            Return ret
        End Function
        Public Enum ExceptionResponse
            Retry = 1
            Ignore
            NotKiteException
        End Enum
    End Class
End Namespace
