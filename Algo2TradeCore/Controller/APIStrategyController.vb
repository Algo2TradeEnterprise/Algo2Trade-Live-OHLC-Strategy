Imports System.Net.Http
Imports System.Threading
Imports Algo2TradeCore.Adapter
Imports Algo2TradeCore.Adapter.APIAdapter
Imports Algo2TradeCore.Entities
Imports Algo2TradeCore.Strategies
Imports Algo2TradeCore.Exceptions
Imports NLog
Imports Utilities
Imports Utilities.ErrorHandlers
Imports Algo2TradeCore.ChartHandler.ChartStyle

Namespace Controller
    Public MustInherit Class APIStrategyController

#Region "Events/Event handlers"
        Public Event DocumentDownloadComplete()
        Public Event DocumentRetryStatus(ByVal currentTry As Integer, ByVal totalTries As Integer)
        Public Event Heartbeat(ByVal msg As String)
        Public Event WaitingFor(ByVal elapsedSecs As Integer, ByVal totalSecs As Integer, ByVal msg As String)
        Public Event DocumentDownloadCompleteEx(ByVal source As List(Of Object))
        Public Event DocumentRetryStatusEx(ByVal currentTry As Integer, ByVal totalTries As Integer, ByVal source As List(Of Object))
        Public Event HeartbeatEx(ByVal msg As String, ByVal source As List(Of Object))
        Public Event WaitingForEx(ByVal elapsedSecs As Integer, ByVal totalSecs As Integer, ByVal msg As String, ByVal source As List(Of Object))
        'Create the events for UI to handle the way it needs to show the ticker
        Public Event TickerConnect()
        Public Event TickerClose()
        Public Event TickerErrorWithStatus(ByVal isConnected As Boolean, ByVal errorMessage As String)
        Public Event TickerError(ByVal errorMessage As String)
        Public Event TickerNoReconnect()
        Public Event TickerReconnect()
        Public Event FetcherError(ByVal instrumentIdentifier As String, ByVal errorMessage As String)

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
        'The below functions are needed to allow the derived classes to raise the above two events
        Protected Overridable Sub OnDocumentDownloadCompleteEx(ByVal source As List(Of Object))
            RaiseEvent DocumentDownloadCompleteEx(source)
        End Sub
        Protected Overridable Sub OnDocumentRetryStatusEx(ByVal currentTry As Integer, ByVal totalTries As Integer, ByVal source As List(Of Object))
            RaiseEvent DocumentRetryStatusEx(currentTry, totalTries, source)
        End Sub
        Protected Overridable Sub OnHeartbeatEx(ByVal msg As String, ByVal source As List(Of Object))
            RaiseEvent HeartbeatEx(msg, source)
        End Sub
        Protected Overridable Sub OnWaitingForEx(ByVal elapsedSecs As Integer, ByVal totalSecs As Integer, ByVal msg As String, ByVal source As List(Of Object))
            RaiseEvent WaitingForEx(elapsedSecs, totalSecs, msg, source)
        End Sub
        Public Overridable Sub OnTickerConnect()
            RaiseEvent TickerConnect()
        End Sub
        Public Overridable Sub OnTickerClose()
            RaiseEvent TickerClose()
        End Sub
        Public Overridable Sub OnTickerError(ByVal errorMessage As String)
            RaiseEvent TickerError(errorMessage)
        End Sub
        Public Overridable Sub OnTickerErrorWithStatus(ByVal isConnected As Boolean, ByVal errorMessage As String)
            RaiseEvent TickerErrorWithStatus(isConnected, errorMessage)
        End Sub
        Public Overridable Sub OnTickerNoReconnect()
            RaiseEvent TickerNoReconnect()
        End Sub
        Public Overridable Sub OnTickerReconnect()
            RaiseEvent TickerReconnect()
        End Sub
        Public Overridable Sub OnFetcherError(ByVal instrumentIdentifier As String, ByVal errorMessage As String)
            RaiseEvent FetcherError(instrumentIdentifier, errorMessage)
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
        Public ReadOnly Property BrokerSource As APISource
        Public Property OrphanException As Exception
        Protected _APIAdapter As APIAdapter
        Protected _APITicker As APITicker
        Protected _APIHistoricalDataFetcher As APIHistoricalDataFetcher
        Protected _AllInstruments As IEnumerable(Of IInstrument)
        Protected _AllStrategyUniqueInstruments As IEnumerable(Of IInstrument)
        Protected _AllStrategies As List(Of Strategy)
        Protected _subscribedStrategyInstruments As Dictionary(Of String, List(Of StrategyInstrument))
        Protected _rawPayloadCreators As Dictionary(Of String, CandleStickChart)
        Public Sub New(ByVal validatedUser As IUser,
                       ByVal associatedBrokerSource As APISource,
                       ByVal canceller As CancellationTokenSource)
            _currentUser = validatedUser
            Me.BrokerSource = associatedBrokerSource
            _cts = canceller
            _LoginThreads = 0
        End Sub
        Public MustOverride Function GetErrorResponse(ByVal response As Object) As String
        Public MustOverride Async Function CloseTickerIfConnectedAsync() As Task
        Public MustOverride Async Function CloseFetcherIfConnectedAsync() As Task

        Public Sub RefreshCancellationToken(ByVal canceller As CancellationTokenSource)
            _cts = canceller
            If _APITicker IsNot Nothing Then _APITicker.RefreshCancellationToken(canceller)
            If _APIHistoricalDataFetcher IsNot Nothing Then _APIHistoricalDataFetcher.RefreshCancellationToken(canceller)
        End Sub
        Protected Async Function ExecuteCommandAsync(ByVal command As APIAdapter.ExecutionCommands, ByVal data As Object) As Task(Of Object)
            logger.Debug("ExecuteCommandAsync, parameters:{0},{1}", command, Utilities.Strings.JsonSerialize(data))
            Dim ret As Object = Nothing
            Dim lastException As Exception = Nothing
            Dim allOKWithoutException As Boolean = False

            Using Waiter As New Waiter(_cts)
                AddHandler Waiter.Heartbeat, AddressOf OnHeartbeat
                AddHandler Waiter.WaitingFor, AddressOf OnWaitingFor
                Dim apiConnectionBeingUsed As IConnection = Me.APIConnection
                For retryCtr = 1 To _MaxReTries
                    _cts.Token.ThrowIfCancellationRequested()
                    lastException = Nothing
                    While Me.APIConnection Is Nothing OrElse apiConnectionBeingUsed Is Nothing OrElse
                       (Me.APIConnection IsNot Nothing AndAlso apiConnectionBeingUsed IsNot Nothing AndAlso
                       Not Me.APIConnection.Equals(apiConnectionBeingUsed))
                        apiConnectionBeingUsed = Me.APIConnection
                        _cts.Token.ThrowIfCancellationRequested()
                        logger.Debug("Waiting for fresh token before running command:{0}", command.ToString)
                        Await Task.Delay(500).ConfigureAwait(False)
                        _cts.Token.ThrowIfCancellationRequested()
                    End While
                    _APIAdapter.SetAPIAccessToken(APIConnection.AccessToken)

                    logger.Debug("Firing command:{0}", command.ToString)
                    OnDocumentRetryStatus(retryCtr, _MaxReTries)
                    Try
                        _cts.Token.ThrowIfCancellationRequested()
                        Select Case command
                            Case ExecutionCommands.GetInstruments
                                Dim allInstrumentsResponse As IEnumerable(Of IInstrument) = Nothing
                                allInstrumentsResponse = Await _APIAdapter.GetAllInstrumentsAsync().ConfigureAwait(False)
                                If allInstrumentsResponse IsNot Nothing Then
                                    logger.Debug("Getting instruments is complete, allInstrumentsResponse.count:{0}", allInstrumentsResponse.Count)
                                    lastException = Nothing
                                    allOKWithoutException = True
                                    _cts.Token.ThrowIfCancellationRequested()
                                    ret = allInstrumentsResponse
                                    _cts.Token.ThrowIfCancellationRequested()
                                    Exit For
                                Else
                                    Throw New ApplicationException(String.Format("Getting all instruments for the day did not succeed"))
                                End If
                            Case ExecutionCommands.GetQuotes
                                Dim allQuotesResponse As IEnumerable(Of IQuote) = Nothing
                                allQuotesResponse = Await _APIAdapter.GetAllQuotesAsync(data).ConfigureAwait(False)
                                If allQuotesResponse IsNot Nothing Then
                                    logger.Debug("Getting all quotes is complete, allQuotesResponse.count:{0}", allQuotesResponse.Count)
                                    lastException = Nothing
                                    allOKWithoutException = True
                                    _cts.Token.ThrowIfCancellationRequested()
                                    ret = allQuotesResponse
                                    _cts.Token.ThrowIfCancellationRequested()
                                    Exit For
                                Else
                                    Throw New ApplicationException(String.Format("Getting all quotes did not succeed"))
                                End If
                            Case ExecutionCommands.GetOrders
                                Dim allOrderResponse As IEnumerable(Of IOrder) = Nothing
                                allOrderResponse = Await _APIAdapter.GetAllOrdersAsync().ConfigureAwait(False)
                                If allOrderResponse IsNot Nothing Then
                                    logger.Debug("Getting all orders is complete, allOrdersResponse.count:{0}", allOrderResponse.Count)
                                Else
                                    logger.Debug("Getting all orders is complete, allOrdersResponse.count:{0}", 0)
                                End If
                                lastException = Nothing
                                allOKWithoutException = True
                                _cts.Token.ThrowIfCancellationRequested()
                                ret = allOrderResponse
                                _cts.Token.ThrowIfCancellationRequested()
                                Exit For
                        End Select
                    Catch aex As AdapterBusinessException
                        logger.Error(aex)
                        lastException = aex
                        Select Case aex.ExceptionType
                            Case AdapterBusinessException.TypeOfException.TokenException
                                Continue For
                            Case AdapterBusinessException.TypeOfException.DataException
                                Continue For
                            Case AdapterBusinessException.TypeOfException.NetworkException
                                Continue For
                            Case Else
                                Exit For
                        End Select
                    Catch opx As OperationCanceledException
                        logger.Error(opx)
                        lastException = opx
                        If Not _cts.Token.IsCancellationRequested Then
                            _cts.Token.ThrowIfCancellationRequested()
                            If Not Waiter.WaitOnInternetFailure(_WaitDurationOnConnectionFailure) Then
                                'Provide required wait in case internet was already up
                                logger.Debug("HTTP->Task was cancelled without internet problem:{0}",
                                             opx.Message)
                                _cts.Token.ThrowIfCancellationRequested()
                                Waiter.SleepRequiredDuration(_WaitDurationOnAnyFailure.TotalSeconds, "Non-explicit cancellation")
                                _cts.Token.ThrowIfCancellationRequested()
                            Else
                                logger.Debug("HTTP->Task was cancelled due to internet problem:{0}, waited prescribed seconds, will now retry",
                                             opx.Message)
                                'Since internet was down, no need to consume retries
                                retryCtr -= 1
                            End If
                        End If
                    Catch hex As HttpRequestException
                        logger.Error(hex)
                        lastException = hex
                        If ExceptionExtensions.GetExceptionMessages(hex).Contains("trust relationship") Then
                            Throw New ForbiddenException(hex.Message, hex, ForbiddenException.TypeOfException.PossibleReloginRequired)
                        End If
                        _cts.Token.ThrowIfCancellationRequested()
                        If Not Waiter.WaitOnInternetFailure(_WaitDurationOnConnectionFailure) Then
                            If hex.Message.Contains("429") Or hex.Message.Contains("503") Then
                                logger.Debug("HTTP->429/503 error without internet problem:{0}",
                                             hex.Message)
                                _cts.Token.ThrowIfCancellationRequested()
                                Waiter.SleepRequiredDuration(_WaitDurationOnServiceUnavailbleFailure.TotalSeconds, "Service unavailable(429/503)")
                                _cts.Token.ThrowIfCancellationRequested()
                                'Since site service is blocked, no need to consume retries
                                retryCtr -= 1
                            ElseIf hex.Message.Contains("404") Then
                                logger.Debug("HTTP->404 error without internet problem:{0}",
                                             hex.Message)
                                _cts.Token.ThrowIfCancellationRequested()
                                'No point retrying, exit for
                                Exit For
                            Else
                                If ExceptionExtensions.IsExceptionConnectionRelated(hex) Then
                                    logger.Debug("HTTP->HttpRequestException without internet problem but of type internet related detected:{0}",
                                                 hex.Message)
                                    _cts.Token.ThrowIfCancellationRequested()
                                    Waiter.SleepRequiredDuration(_WaitDurationOnConnectionFailure.TotalSeconds, "Connection HttpRequestException")
                                    _cts.Token.ThrowIfCancellationRequested()
                                    'Since exception was internet related, no need to consume retries
                                    retryCtr -= 1
                                Else
                                    'Provide required wait in case internet was already up
                                    logger.Debug("HTTP->HttpRequestException without internet problem:{0}",
                                                 hex.Message)
                                    _cts.Token.ThrowIfCancellationRequested()
                                    Waiter.SleepRequiredDuration(_WaitDurationOnAnyFailure.TotalSeconds, "Unknown HttpRequestException:" & hex.Message)
                                    _cts.Token.ThrowIfCancellationRequested()
                                End If
                            End If
                        Else
                            logger.Debug("HTTP->HttpRequestException with internet problem:{0}, waited prescribed seconds, will now retry",
                                         hex.Message)
                            'Since internet was down, no need to consume retries
                            retryCtr -= 1
                        End If
                    Catch ex As Exception
                        logger.Error(ex)
                        lastException = ex
                        'Exit if it is a network failure check and stop retry to avoid stack overflow
                        'Need to relogin, no point retrying
                        If ExceptionExtensions.GetExceptionMessages(ex).Contains("disposed") Then
                            Throw New ForbiddenException(ex.Message, ex, ForbiddenException.TypeOfException.ExceptionInBetweenLoginProcess)
                        End If
                        _cts.Token.ThrowIfCancellationRequested()
                        If Not Waiter.WaitOnInternetFailure(_WaitDurationOnConnectionFailure) Then
                            'Provide required wait in case internet was already up
                            _cts.Token.ThrowIfCancellationRequested()
                            If ExceptionExtensions.IsExceptionConnectionRelated(ex) Then
                                logger.Debug("HTTP->Exception without internet problem but of type internet related detected:{0}",
                                             ex.Message)
                                _cts.Token.ThrowIfCancellationRequested()
                                Waiter.SleepRequiredDuration(_WaitDurationOnConnectionFailure.TotalSeconds, "Connection Exception")
                                _cts.Token.ThrowIfCancellationRequested()
                                'Since exception was internet related, no need to consume retries
                                retryCtr -= 1
                            Else
                                logger.Debug("HTTP->Exception without internet problem of unknown type detected:{0}",
                                             ex.Message)
                                _cts.Token.ThrowIfCancellationRequested()
                                Waiter.SleepRequiredDuration(_WaitDurationOnAnyFailure.TotalSeconds, "Unknown Exception")
                                _cts.Token.ThrowIfCancellationRequested()
                            End If
                        Else
                            logger.Debug("HTTP->Exception with internet problem:{0}, waited prescribed seconds, will now retry",
                                         ex.Message)
                            'Since internet was down, no need to consume retries
                            retryCtr -= 1
                        End If
                    Finally
                        OnDocumentDownloadComplete()
                    End Try
                    _cts.Token.ThrowIfCancellationRequested()
                    If ret IsNot Nothing Then
                        Exit For
                    End If
                    GC.Collect()
                Next
                RemoveHandler Waiter.Heartbeat, AddressOf OnHeartbeat
                RemoveHandler Waiter.WaitingFor, AddressOf OnWaitingFor
            End Using
            _cts.Token.ThrowIfCancellationRequested()
            If Not allOKWithoutException Then Throw lastException
            Return ret
        End Function


        Protected MustOverride Function GetLoginURL() As String
        Public MustOverride Async Function LoginAsync() As Task(Of IConnection)
        Public MustOverride Async Function PrepareToRunStrategyAsync() As Task(Of Boolean)
        Public MustOverride Async Function SubscribeStrategyAsync(ByVal strategyToRun As Strategy) As Task
        Public MustOverride Async Function FillOrderDetailsAsync(ByVal strategyToRun As Strategy) As Task
        Public Sub FillCandlestickCreator()
            If _AllStrategyUniqueInstruments IsNot Nothing AndAlso _AllStrategyUniqueInstruments.Count > 0 Then
                For Each runningStrategyUniqueInstruments In _AllStrategyUniqueInstruments
                    If _rawPayloadCreators IsNot Nothing AndAlso _rawPayloadCreators.ContainsKey(runningStrategyUniqueInstruments.InstrumentIdentifier) Then
                        Continue For
                    End If
                    If _rawPayloadCreators Is Nothing Then _rawPayloadCreators = New Dictionary(Of String, CandleStickChart)
                    _rawPayloadCreators.Add(runningStrategyUniqueInstruments.InstrumentIdentifier,
                                            New CandleStickChart(Me,
                                                                 runningStrategyUniqueInstruments,
                                                                 _subscribedStrategyInstruments(runningStrategyUniqueInstruments.InstrumentIdentifier),
                                                                 _cts))
                    If runningStrategyUniqueInstruments.RawPayloads Is Nothing Then runningStrategyUniqueInstruments.RawPayloads = New Concurrent.ConcurrentDictionary(Of Date, OHLCPayload)
                Next
            End If
        End Sub

    End Class
End Namespace