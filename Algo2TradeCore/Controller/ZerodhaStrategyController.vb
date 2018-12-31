Imports System.Collections.Specialized
Imports System.Net
Imports Utilities.ErrorHandlers
Imports System.Threading
Imports System.Web
Imports Algo2TradeCore.Entities
Imports NLog
Imports Utilities.Network
Imports KiteConnect
Imports Algo2TradeCore.Adapter
Imports Utilities
Imports System.Net.Http
Imports Algo2TradeCore.Strategies

Namespace Controller
    Public Class ZerodhaStrategyController
        Inherits APIStrategyController

#Region "Logging and Status Progress"
        Public Shared Shadows logger As Logger = LogManager.GetCurrentClassLogger
#End Region

        Public Sub New(ByVal validatedUser As ZerodhaUser,
                       ByVal canceller As CancellationTokenSource)
            MyBase.New(validatedUser, canceller)
            _LoginURL = "https://kite.trade/connect/login"
        End Sub

#Region "Login"
        Protected Overrides Function GetLoginURL() As String
            logger.Debug("GetLoginURL, parameters:Nothing")
            Return String.Format("{0}?api_key={1}&v={2}", _LoginURL, _currentUser.APIKey, _currentUser.APIVersion)
        End Function
        Public Overrides Function GetErrorResponse(ByVal response As Object) As String
            logger.Debug("GetErrorResponse, response:{0}", Utils.JsonSerialize(response))
            _cts.Token.ThrowIfCancellationRequested()
            Dim ret As String = Nothing

            If response IsNot Nothing AndAlso
               response.GetType = GetType(Dictionary(Of String, Object)) AndAlso
               CType(response, Dictionary(Of String, Object)).ContainsKey("status") AndAlso
               CType(response, Dictionary(Of String, Object))("status") = "error" AndAlso
               CType(response, Dictionary(Of String, Object)).ContainsKey("message") Then
                ret = String.Format("Zerodha reported error, message:{0}", CType(response, Dictionary(Of String, Object))("message"))
            End If
            Return ret
        End Function
        Public Overrides Async Function LoginAsync() As Task(Of IConnection)
            logger.Debug("LoginAsync, parameters:Nothing")
            While True
                _cts.Token.ThrowIfCancellationRequested()
                Try
                    Dim requestToken As String = Nothing

                    Dim postContent As New Dictionary(Of String, String)
                    postContent.Add("user_id", _currentUser.UserId)
                    postContent.Add("password", _currentUser.Password)
                    postContent.Add("login", "")

                    HttpBrowser.KillCookies()
                    _cts.Token.ThrowIfCancellationRequested()

                    Using browser As New HttpBrowser(Nothing, DecompressionMethods.GZip, TimeSpan.FromMinutes(1), _cts)
                        AddHandler browser.DocumentDownloadComplete, AddressOf OnDocumentDownloadComplete
                        AddHandler browser.DocumentRetryStatus, AddressOf OnDocumentRetryStatus
                        AddHandler browser.Heartbeat, AddressOf OnHeartbeat
                        AddHandler browser.WaitingFor, AddressOf OnWaitingFor

                        'Keep the below headers constant for all login browser operations
                        browser.UserAgent = GetRandomUserAgent()
                        browser.KeepAlive = True

                        Dim redirectedURI As Uri = Nothing

                        'Now launch the authentication page
                        Dim headers As New Dictionary(Of String, String)
                        headers.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8")
                        'headers.Add("Accept-Encoding", "gzip, deflate, br")
                        headers.Add("Accept-Encoding", "*")
                        headers.Add("Accept-Language", "en-US,en;q=0.8")
                        headers.Add("Host", "kite.trade")
                        headers.Add("X-Kite-version", _currentUser.APIVersion)

                        OnHeartbeat("Opening login page")
                        logger.Debug("Opening login page, GetLoginURL:{0}, headers:{1}", GetLoginURL, Utils.JsonSerialize(headers))

                        _cts.Token.ThrowIfCancellationRequested()
                        Dim tempRet As Tuple(Of Uri, Object) = Await browser.NonPOSTRequestAsync(GetLoginURL,
                                                                                              Http.HttpMethod.Get,
                                                                                              Nothing,
                                                                                              False,
                                                                                              headers,
                                                                                              False).ConfigureAwait(False)

                        _cts.Token.ThrowIfCancellationRequested()
                        'Should be getting back the redirected URL in Item1 and the htmldocument response in Item2
                        Dim finalURLToCall As Uri = Nothing
                        If tempRet IsNot Nothing AndAlso tempRet.Item1 IsNot Nothing AndAlso tempRet.Item1.ToString.Contains("sess_id") Then
                            logger.Debug("Login page returned, sess_id string:{0}", tempRet.Item1.ToString)
                            finalURLToCall = tempRet.Item1
                            redirectedURI = tempRet.Item1

                            postContent = New Dictionary(Of String, String)
                            postContent.Add("user_id", _currentUser.UserId)
                            postContent.Add("password", _currentUser.Password)
                            postContent.Add("login", "")

                            'Now prepare the step 1 authentication
                            headers = New Dictionary(Of String, String)
                            headers.Add("Accept", "application/json, text/plain, */*")
                            headers.Add("Accept-Language", "en-US")
                            'headers.Add("Accept-Encoding", "gzip, deflate, br")
                            headers.Add("Content-Type", "application/x-www-form-urlencoded")
                            headers.Add("Host", "kite.zerodha.com")
                            headers.Add("Origin", "https://kite.zerodha.com")
                            headers.Add("X-Kite-version", _currentUser.APIVersion)

                            tempRet = Nothing
                            OnHeartbeat("Submitting Id/pass")
                            logger.Debug("Submitting Id/pass, redirectedURI:{0}, postContent:{1}, headers:{2}", redirectedURI.ToString, Utils.JsonSerialize(postContent), Utils.JsonSerialize(headers))
                            _cts.Token.ThrowIfCancellationRequested()
                            tempRet = Await browser.POSTRequestAsync("https://kite.zerodha.com/api/login",
                                                     redirectedURI.ToString,
                                                     postContent,
                                                     False,
                                                     headers,
                                                     False).ConfigureAwait(False)
                            _cts.Token.ThrowIfCancellationRequested()
                            'Should come back as redirected url in Item1 and htmldocument in Item2
                            Dim q1 As String = Nothing
                            Dim q2 As String = Nothing

                            If tempRet IsNot Nothing AndAlso tempRet.Item2 IsNot Nothing AndAlso tempRet.Item2.GetType Is GetType(Dictionary(Of String, Object)) AndAlso
                    tempRet.Item2.containskey("status") AndAlso tempRet.Item2("status") = "success" AndAlso
                    tempRet.Item2.containskey("data") AndAlso tempRet.Item2("data").containskey("question_ids") AndAlso tempRet.Item2("data")("question_ids").count >= 2 Then
                                q1 = tempRet.Item2("data")("question_ids")(0)
                                q2 = tempRet.Item2("data")("question_ids")(1)
                                If q1 IsNot Nothing AndAlso q2 IsNot Nothing Then
                                    logger.Debug("Id/pass submission returned, q1:{0}, q2:{1}", q1, q2)
                                    'Now preprate the 2 step authentication
                                    Dim stringPostContent As New Http.StringContent(String.Format("user_id={0}&question_id={1}&question_id={2}&answer={3}&answer={4}",
                                                                                      Uri.EscapeDataString(_currentUser.UserId),
                                                                                      Uri.EscapeDataString(q1),
                                                                                      Uri.EscapeDataString(q2),
                                                                                      Uri.EscapeDataString("a"),
                                                                                      Uri.EscapeDataString("a")),
                                                                        Text.Encoding.UTF8, "application/x-www-form-urlencoded")

                                    headers = New Dictionary(Of String, String)
                                    headers.Add("Accept", "application/json, text/plain, */*")
                                    headers.Add("Accept-Language", "en-US,en;q=0.5")
                                    'headers.Add("Accept-Encoding", "gzip, deflate, br")
                                    headers.Add("Content-Type", "application/x-www-form-urlencoded")
                                    headers.Add("Host", "kite.zerodha.com")
                                    headers.Add("Origin", "https://kite.zerodha.com")
                                    headers.Add("X-Kite-version", _currentUser.APIVersion)

                                    tempRet = Nothing
                                    OnHeartbeat("Submitting 2FA")
                                    logger.Debug("Submitting 2FA, redirectedURI:{0}, stringPostContent:{1}, headers:{2}", redirectedURI.ToString, Await stringPostContent.ReadAsStringAsync().ConfigureAwait(False), Utils.JsonSerialize(headers))
                                    _cts.Token.ThrowIfCancellationRequested()
                                    tempRet = Await browser.POSTRequestAsync("https://kite.zerodha.com/api/twofa",
                                                                 redirectedURI.ToString,
                                                                 stringPostContent,
                                                                 False,
                                                                 headers,
                                                                 False).ConfigureAwait(False)
                                    _cts.Token.ThrowIfCancellationRequested()

                                    'Should come back as redirect url in Item1 and htmldocument response in Item2
                                    If tempRet IsNot Nothing AndAlso tempRet.Item1 IsNot Nothing AndAlso tempRet.Item1.ToString.Contains("request_token") Then
                                        redirectedURI = tempRet.Item1
                                        Dim queryStrings As NameValueCollection = HttpUtility.ParseQueryString(redirectedURI.Query)
                                        requestToken = queryStrings("request_token")
                                        logger.Debug("2FA submission returned, requestToken:{0}", requestToken)
                                        logger.Debug("Authentication complete, requestToken:{0}", requestToken)
                                    ElseIf tempRet IsNot Nothing AndAlso tempRet.Item2 IsNot Nothing AndAlso tempRet.Item2.GetType Is GetType(Dictionary(Of String, Object)) AndAlso
                                tempRet.Item2.containskey("status") AndAlso tempRet.Item2("status") = "success" Then
                                        logger.Debug("2FA submission returned, redirection:true")

                                        headers = New Dictionary(Of String, String)
                                        headers.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8")
                                        headers.Add("Accept-Encoding", "gzip, deflate, br")
                                        headers.Add("Accept-Language", "en-US,en;q=0.5")
                                        headers.Add("Host", "kite.zerodha.com")
                                        headers.Add("X-Kite-version", _currentUser.APIVersion)
                                        tempRet = Nothing

                                        OnHeartbeat("Addressing redirection")
                                        logger.Debug("Redirecting, finalURLToCall:{0}, headers:{1}", finalURLToCall.ToString, Utils.JsonSerialize(headers))
                                        _cts.Token.ThrowIfCancellationRequested()
                                        tempRet = Await browser.NonPOSTRequestAsync(String.Format("{0}&skip_session=true", finalURLToCall.ToString),
                                                                        Http.HttpMethod.Get,
                                                                        finalURLToCall.ToString,
                                                                        False,
                                                                        headers,
                                                                        True).ConfigureAwait(False)
                                        _cts.Token.ThrowIfCancellationRequested()
                                        If tempRet IsNot Nothing AndAlso tempRet.Item1 IsNot Nothing AndAlso tempRet.Item1.ToString.Contains("request_token") Then
                                            redirectedURI = tempRet.Item1
                                            Dim queryStrings As NameValueCollection = HttpUtility.ParseQueryString(redirectedURI.Query)
                                            requestToken = queryStrings("request_token")
                                            logger.Debug("Redirection returned, requestToken:{0}", requestToken)
                                            logger.Debug("Authentication complete, requestToken:{0}", requestToken)
                                        Else
                                            If tempRet IsNot Nothing AndAlso tempRet.Item2 IsNot Nothing AndAlso tempRet.Item2.GetType Is GetType(Dictionary(Of String, Object)) Then
                                                Throw New AuthenticationException(GetErrorResponse(tempRet.Item2),
                                                                   AuthenticationException.TypeOfException.SecondLevelFailure)
                                            Else
                                                Throw New AuthenticationException("Step 2 authentication did not produce any request_token after redirection",
                                                                   AuthenticationException.TypeOfException.SecondLevelFailure)
                                            End If
                                        End If
                                    Else
                                        If tempRet IsNot Nothing AndAlso tempRet.Item2 IsNot Nothing AndAlso tempRet.Item2.GetType Is GetType(Dictionary(Of String, Object)) Then
                                            Throw New AuthenticationException(GetErrorResponse(tempRet.Item2),
                                                                   AuthenticationException.TypeOfException.SecondLevelFailure)
                                        Else
                                            Throw New AuthenticationException("Step 2 authentication did not produce any request_token",
                                                                   AuthenticationException.TypeOfException.SecondLevelFailure)
                                        End If
                                    End If
                                Else
                                    If tempRet IsNot Nothing AndAlso tempRet.Item2 IsNot Nothing AndAlso tempRet.Item2.GetType Is GetType(Dictionary(Of String, Object)) Then
                                        Throw New AuthenticationException(GetErrorResponse(tempRet.Item2),
                                                                   AuthenticationException.TypeOfException.SecondLevelFailure)
                                    Else
                                        Throw New AuthenticationException("Step 2 authentication did not produce first or second questions",
                                                                   AuthenticationException.TypeOfException.SecondLevelFailure)
                                    End If
                                End If
                            Else
                                If tempRet IsNot Nothing AndAlso tempRet.Item2 IsNot Nothing AndAlso tempRet.Item2.GetType Is GetType(Dictionary(Of String, Object)) Then
                                    Throw New AuthenticationException(GetErrorResponse(tempRet.Item2),
                                                                   AuthenticationException.TypeOfException.FirstLevelFaulure)
                                Else
                                    Throw New AuthenticationException("Step 1 authentication did not produce any questions in the response", AuthenticationException.TypeOfException.FirstLevelFaulure)
                                End If
                            End If
                        Else
                            If tempRet IsNot Nothing AndAlso tempRet.Item2 IsNot Nothing AndAlso tempRet.Item2.GetType Is GetType(Dictionary(Of String, Object)) Then
                                Throw New AuthenticationException(GetErrorResponse(tempRet.Item2),
                                                                   AuthenticationException.TypeOfException.FirstLevelFaulure)
                            Else
                                Throw New AuthenticationException("Step 1 authentication prepration to get to the login page failed",
                                                                   AuthenticationException.TypeOfException.FirstLevelFaulure)
                            End If
                        End If
                        RemoveHandler browser.DocumentDownloadComplete, AddressOf OnDocumentDownloadComplete
                        RemoveHandler browser.DocumentRetryStatus, AddressOf OnDocumentRetryStatus
                        RemoveHandler browser.Heartbeat, AddressOf OnHeartbeat
                        RemoveHandler browser.WaitingFor, AddressOf OnWaitingFor
                    End Using
                    If requestToken IsNot Nothing Then
                        _cts.Token.ThrowIfCancellationRequested()
                        APIConnection = Await RequestAccessTokenAsync(requestToken).ConfigureAwait(False)
                        _cts.Token.ThrowIfCancellationRequested()

                        'Now open the ticker
                        If _APITicker IsNot Nothing Then
                            _APITicker.ClearLocalUniqueSubscriptionList()
                            _cts.Token.ThrowIfCancellationRequested()
                            Await _APITicker.CloseTickerIfConnectedAsync().ConfigureAwait(False)
                            _cts.Token.ThrowIfCancellationRequested()
                            RemoveHandler _APITicker.Heartbeat, AddressOf OnHeartbeat
                            RemoveHandler _APITicker.WaitingFor, AddressOf OnWaitingFor
                            RemoveHandler _APITicker.DocumentRetryStatus, AddressOf OnDocumentRetryStatus
                            RemoveHandler _APITicker.DocumentDownloadComplete, AddressOf OnDocumentDownloadComplete
                        Else
                            _APITicker = New ZerodhaTicker(Me, _cts)
                        End If

                        AddHandler _APITicker.Heartbeat, AddressOf OnHeartbeat
                        AddHandler _APITicker.WaitingFor, AddressOf OnWaitingFor
                        AddHandler _APITicker.DocumentRetryStatus, AddressOf OnDocumentRetryStatus
                        AddHandler _APITicker.DocumentDownloadComplete, AddressOf OnDocumentDownloadComplete

                        _cts.Token.ThrowIfCancellationRequested()
                        Await _APITicker.ConnectTickerAsync().ConfigureAwait(False)
                        _cts.Token.ThrowIfCancellationRequested()
                    End If
                Catch tex As KiteConnect.TokenException
                    logger.Error(tex)
                    OnHeartbeat("Possible error while generating session, token may be invalid, retrying the whole login process")
                    Continue While
                End Try
                _cts.Token.ThrowIfCancellationRequested()
                Exit While
            End While
            Return APIConnection
        End Function
        Private Async Function RequestAccessTokenAsync(ByVal requestToken As String) As Task(Of ZerodhaConnection)
            logger.Debug("RequestAccessTokenAsync, requestToken:{0}", requestToken)

            _cts.Token.ThrowIfCancellationRequested()
            Dim ret As ZerodhaConnection = Nothing
            Await Task.Delay(0).ConfigureAwait(False)
            Dim kiteConnector As New Kite(_currentUser.APIKey, Debug:=True)

            Dim lastException As Exception = Nothing
            Dim allOKWithoutException As Boolean = False

            Using Waiter As New Waiter(_cts)
                AddHandler Waiter.Heartbeat, AddressOf OnHeartbeat
                AddHandler Waiter.WaitingFor, AddressOf OnWaitingFor

                For retryCtr = 1 To _MaxReTries
                    _cts.Token.ThrowIfCancellationRequested()
                    lastException = Nothing
                    OnHeartbeat("Generating session...")
                    logger.Debug("Generating session, command:{0}, requestToken:{1}, _currentUser.APISecret:{2}",
                                                    "GenerateSession", requestToken, _currentUser.APISecret)
                    OnDocumentRetryStatus(retryCtr, _MaxReTries)
                    Try
                        _cts.Token.ThrowIfCancellationRequested()
                        Dim user As User = kiteConnector.GenerateSession(requestToken, _currentUser.APISecret)
                        _cts.Token.ThrowIfCancellationRequested()
                        Console.WriteLine(Utils.JsonSerialize(user))
                        logger.Debug("Processing response")

                        If user.AccessToken IsNot Nothing Then
                            kiteConnector.SetAccessToken(user.AccessToken)
                            logger.Debug("Session generated, user.AccessToken:{0}", user.AccessToken)

                            ret = New ZerodhaConnection
                            With ret
                                .ZerodhaUser = New ZerodhaUser() With {.UserId = _currentUser.UserId,
                                                                        .Password = _currentUser.Password,
                                                                        .APIKey = _currentUser.APIKey,
                                                                        .API2FA = _currentUser.API2FA,
                                                                        .APISecret = _currentUser.APISecret,
                                                                        .APIVersion = _currentUser.APIVersion,
                                                                        .WrappedUser = user}
                                .RequestToken = requestToken
                            End With
                            lastException = Nothing
                            allOKWithoutException = True
                            Exit For
                        Else
                            Throw New ApplicationException(String.Format("Generating session did not succeed, command:{0}", "GenerateSession"))
                        End If
                        _cts.Token.ThrowIfCancellationRequested()
                    Catch tex As KiteConnect.TokenException
                        logger.Error(tex)
                        lastException = tex
                        Exit For
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
            ' For handling 403 errors
            '_Kite.SetSessionExpiryHook(AddressOf OnSessionExpireAsync)
            Return ret
        End Function
        Public Async Sub OnSessionExpireAsync()
            logger.Debug("OnSessionExpireAsync, parameters:Nothing")
            _cts.Token.ThrowIfCancellationRequested()
            'Wait for the lock and if locked, then exit immediately
            If _LoginThreads = 0 Then
                Interlocked.Increment(_LoginThreads)
                APIConnection = Nothing
            Else
                Exit Sub
            End If
            OnHeartbeat("********** Need to login again **********")
            Dim tempConn As ZerodhaConnection = Nothing
            Try
                _cts.Token.ThrowIfCancellationRequested()
                Await Task.Delay(2000).ConfigureAwait(False)
                Dim loginMessage As String = Nothing
                While True
                    _cts.Token.ThrowIfCancellationRequested()
                    loginMessage = Nothing
                    tempConn = Nothing
                    Try
                        OnHeartbeat("Attempting to get connection to Zerodha API")
                        _cts.Token.ThrowIfCancellationRequested()
                        tempConn = Await LoginAsync().ConfigureAwait(False)
                        _cts.Token.ThrowIfCancellationRequested()
                    Catch ex As Exception
                        loginMessage = ex.Message
                        logger.Error(ex)
                    End Try
                    If tempConn Is Nothing Then
                        If loginMessage IsNot Nothing AndAlso loginMessage.ToUpper.Contains("password".ToUpper) Then
                            'No need to retry as its a password failure
                            OnHeartbeat(String.Format("Loging process failed:{0}", loginMessage))
                            Exit While
                        Else
                            OnHeartbeat(String.Format("Login process failed after token expiry:{0} | Waiting for 10 seconds before retrying connection", loginMessage))
                            _cts.Token.ThrowIfCancellationRequested()
                            Await Task.Delay(10000)
                            _cts.Token.ThrowIfCancellationRequested()
                        End If
                    Else
                        OnHeartbeat("Relogin completed, checking to see if strategy instruments need to be resubscribed")

                        If _AllStrategies IsNot Nothing AndAlso _AllStrategies.Count > 0 Then
                            For Each runningStratgey In _AllStrategies
                                _cts.Token.ThrowIfCancellationRequested()
                                OnHeartbeatEx(String.Format("Resubscribing strategy instruments, strategy:{0}", runningStratgey.ToString), New List(Of Object) From {runningStratgey})
                                _cts.Token.ThrowIfCancellationRequested()
                                Await runningStratgey.SubscribeAsync(_APITicker).ConfigureAwait(False)
                                _cts.Token.ThrowIfCancellationRequested()
                            Next
                        End If
                        OnHeartbeat("Relogin completed with resubscriptions")
                        Exit While
                    End If
                End While
                If tempConn Is Nothing Then
                    If loginMessage IsNot Nothing Then
                        Throw New ApplicationException(String.Format("No connection to Zerodha API could be established | Details:{0}", loginMessage))
                    Else
                        Throw New ApplicationException("No connection to Zerodha API could be established")
                    End If
                End If
            Finally
                Interlocked.Decrement(_LoginThreads)
            End Try
            _cts.Token.ThrowIfCancellationRequested()
        End Sub
#End Region

#Region "Common tasks for all strategies"
        Public Overrides Async Function PrepareToRunStrategyAsync() As Task(Of Boolean)
            logger.Debug("PrepareToRunStrategyAsync, parameters:Nothing")
            _cts.Token.ThrowIfCancellationRequested()

            Dim ret As Boolean = False
            _AllStrategies = Nothing
            _AllInstruments = Nothing
            If _APIAdapter IsNot Nothing Then
                RemoveHandler _APIAdapter.Heartbeat, AddressOf OnHeartbeat
                RemoveHandler _APIAdapter.WaitingFor, AddressOf OnWaitingFor
                RemoveHandler _APIAdapter.DocumentRetryStatus, AddressOf OnDocumentRetryStatus
                RemoveHandler _APIAdapter.DocumentDownloadComplete, AddressOf OnDocumentDownloadComplete
            End If
            _APIAdapter = New ZerodhaAdapter(Me, _cts)
            AddHandler _APIAdapter.Heartbeat, AddressOf OnHeartbeat
            AddHandler _APIAdapter.WaitingFor, AddressOf OnWaitingFor
            AddHandler _APIAdapter.DocumentRetryStatus, AddressOf OnDocumentRetryStatus
            AddHandler _APIAdapter.DocumentDownloadComplete, AddressOf OnDocumentDownloadComplete

            Dim lastException As Exception = Nothing
            Dim allOKWithoutException As Boolean = False

            Using Waiter As New Waiter(_cts)
                AddHandler Waiter.Heartbeat, AddressOf OnHeartbeat
                AddHandler Waiter.WaitingFor, AddressOf OnWaitingFor

                For retryCtr = 1 To _MaxReTries
                    _cts.Token.ThrowIfCancellationRequested()
                    lastException = Nothing
                    While Me.APIConnection Is Nothing
                        _cts.Token.ThrowIfCancellationRequested()
                        logger.Debug("Waiting for fresh token before running GetAllInstrumentsAsync")
                        Await Task.Delay(500).ConfigureAwait(False)
                        _cts.Token.ThrowIfCancellationRequested()
                    End While
                    _APIAdapter.SetAPIAccessToken(APIConnection.AccessToken)

                    OnHeartbeat("Getting all instruments for the day...")
                    OnDocumentRetryStatus(retryCtr, _MaxReTries)
                    Try
                        _cts.Token.ThrowIfCancellationRequested()
                        _AllInstruments = Await _APIAdapter.GetAllInstrumentsAsync().ConfigureAwait(False)
                        _cts.Token.ThrowIfCancellationRequested()

                        logger.Debug("Processing response")

                        If _AllInstruments IsNot Nothing Then
                            logger.Debug("Getting instruments is complete, _AllInstruments.count:{0}", _AllInstruments.Count)
                            lastException = Nothing
                            allOKWithoutException = True
                            ret = True
                            Exit For
                        Else
                            Throw New ApplicationException(String.Format("Getting all instruments for the day did not succeed"))
                        End If
                        _cts.Token.ThrowIfCancellationRequested()
                    Catch tex As KiteConnect.TokenException
                        logger.Error(tex)
                        lastException = tex
                        Continue For
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
                    If _AllInstruments IsNot Nothing Then
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
        ''' <summary>
        ''' This will help find all tradable instruments as per the passed strategy and then create the strategy workers for each of these instruments
        ''' </summary>
        ''' <param name="strategyToRun"></param>
        ''' <returns></returns>
        Public Overrides Async Function ExecuteStrategyAsync(ByVal strategyToRun As Strategy) As Task
            logger.Debug("ExecuteStrategyAsync, strategyToRun:{0}", strategyToRun.ToString)
            _cts.Token.ThrowIfCancellationRequested()

            If strategyToRun IsNot Nothing Then
                If _AllStrategies Is Nothing Then _AllStrategies = New List(Of Strategy)
                'Find if this strategy already exists
                Dim found = _AllStrategies.Where(Function(x)
                                                     Return x.GetType Is strategyToRun.GetType
                                                 End Function)
                If found Is Nothing OrElse found.Count = 0 Then
                    _AllStrategies.Add(strategyToRun)
                End If
                'Remove and add fresh handlers to be cautious
                RemoveHandler strategyToRun.HeartbeatEx, AddressOf OnHeartbeatEx
                RemoveHandler strategyToRun.WaitingForEx, AddressOf OnWaitingForEx
                RemoveHandler strategyToRun.DocumentRetryStatusEx, AddressOf OnDocumentRetryStatusEx
                RemoveHandler strategyToRun.DocumentDownloadCompleteEx, AddressOf OnDocumentDownloadCompleteEx

                AddHandler strategyToRun.HeartbeatEx, AddressOf OnHeartbeatEx
                AddHandler strategyToRun.WaitingForEx, AddressOf OnWaitingForEx
                AddHandler strategyToRun.DocumentRetryStatusEx, AddressOf OnDocumentRetryStatusEx
                AddHandler strategyToRun.DocumentDownloadCompleteEx, AddressOf OnDocumentDownloadCompleteEx
                OnHeartbeatEx(String.Format("As per the strategy logic, tradable instruments being fetched, strategy:{0}", strategyToRun.ToString), New List(Of Object) From {strategyToRun})
                _cts.Token.ThrowIfCancellationRequested()
                Dim ret As Boolean = Await strategyToRun.CreateTradableStrategyInstrumentsAsync(_AllInstruments).ConfigureAwait(False)
                _cts.Token.ThrowIfCancellationRequested()
                'Now we know what are the instruments as per the strategy and their corresponding workers
                If Not ret Then Throw New ApplicationException(String.Format("No instruments fetched that can be traded, strategy:{0}", strategyToRun.ToString))

                'Now create a local collection inside the controller for the strategy instruments that need to be subscribed as a dictionary for easy picking during tick receive
                If _subscribedStrategyInstruments Is Nothing Then _subscribedStrategyInstruments = New Dictionary(Of String, List(Of StrategyInstrument))
                For Each runningTradableStrategyInstrument In strategyToRun.TradableStrategyInstruments
                    _cts.Token.ThrowIfCancellationRequested()
                    Dim instrumentKey As String = runningTradableStrategyInstrument.TradableInstrument.InstrumentIdentifier
                    Dim strategiesToBeSubscribedForThisInstrument As List(Of StrategyInstrument) = Nothing
                    If _subscribedStrategyInstruments.ContainsKey(instrumentKey) Then
                        strategiesToBeSubscribedForThisInstrument = _subscribedStrategyInstruments(instrumentKey)
                    Else
                        strategiesToBeSubscribedForThisInstrument = New List(Of StrategyInstrument)
                        _subscribedStrategyInstruments.Add(instrumentKey, strategiesToBeSubscribedForThisInstrument)
                    End If
                    'Remove the current strategy if present already
                    strategiesToBeSubscribedForThisInstrument.RemoveAll(Function(X)
                                                                            Return X.GetType Is runningTradableStrategyInstrument.GetType
                                                                        End Function)
                    strategiesToBeSubscribedForThisInstrument.Add(runningTradableStrategyInstrument)
                Next

                _cts.Token.ThrowIfCancellationRequested()
                Await strategyToRun.SubscribeAsync(_APITicker).ConfigureAwait(False)
                _cts.Token.ThrowIfCancellationRequested()

                'OnHeartbeat(String.Format("Executing the strategy by creating relevant instrument workers, strategy:{0}", strategyToRun.ToString))
                'Await strategyToRun.ExecuteAsync().ConfigureAwait(False)
                _cts.Token.ThrowIfCancellationRequested()
            End If
        End Function
#End Region
        Public Overrides Async Function CloseTickerIfConnectedAsync() As Task
            If _APITicker IsNot Nothing Then Await _APITicker.CloseTickerIfConnectedAsync().ConfigureAwait(False)
        End Function

        Public Overrides Sub OnTickerConnect()
            logger.Debug("OnTickerConnect, parameters:Nothing", Me.ToString)
            MyBase.OnTickerConnect()
        End Sub
        Public Overrides Sub OnTickerClose()
            logger.Debug("OnTickerClose, parameters:Nothing", Me.ToString)
            MyBase.OnTickerClose()
        End Sub
        Public Overrides Sub OnTickerError(ByVal errorMessage As String)
            logger.Debug("OnTickerError, errorMessage:{0}", errorMessage)
            If _APITicker IsNot Nothing Then
                OnTickerErrorWithStatus(_APITicker.IsConnected, errorMessage)
            Else
                OnTickerErrorWithStatus(False, errorMessage)
            End If
            MyBase.OnTickerError(errorMessage)
            If errorMessage.Contains("403") Then OnSessionExpireAsync()
        End Sub
        Public Overrides Sub OnTickerErrorWithStatus(ByVal isConnected As Boolean, ByVal errorMessage As String)
            logger.Debug("OnTickerErrorWithStatus, isConnected:{0}, errorMessage:{1}", isConnected, errorMessage)
            MyBase.OnTickerErrorWithStatus(isConnected, errorMessage)
        End Sub
        Public Overrides Sub OnTickerNoReconnect()
            logger.Debug("OnTickerNoReconnect, parameters:Nothing", Me.ToString)
            'OnHeartbeat("Ticker, not Reconnecting")
            MyBase.OnTickerNoReconnect()
        End Sub
        Public Overrides Sub OnTickerReconnect()
            logger.Debug("OnTickerReconnect, parameters:Nothing", Me.ToString)
            MyBase.OnTickerReconnect()
        End Sub
        Public Async Sub OnTickerTickAsync(ByVal tickData As Tick)
            'logger.Debug("OnTickerTickAsync, tickData:{0}", Utils.JsonSerialize(tickData))
            Await Task.Delay(0).ConfigureAwait(False)
            If _subscribedStrategyInstruments IsNot Nothing AndAlso _subscribedStrategyInstruments.Count > 0 Then
                Dim runningTick As New ZerodhaTick() With {.WrappedTick = tickData}
                For Each runningStrategyInstrument In _subscribedStrategyInstruments(tickData.InstrumentToken)
                    runningStrategyInstrument.ProcessTickAsync(runningTick)
                Next
            End If
        End Sub
        Public Async Sub OnTickerOrderUpdateAsync(ByVal orderData As Order)
            logger.Debug("OnTickerOrderUpdateAsync, orderData:{0}", Utils.JsonSerialize(orderData))
            Await Task.Delay(0).ConfigureAwait(False)
            'If _todaysInstrumentsForOHLStrategy IsNot Nothing AndAlso _todaysInstrumentsForOHLStrategy.Count > 0 Then
            '    _todaysInstrumentsForOHLStrategy(orderData.InstrumentToken).StrategyWorker.ConsumedOrderUpdateAsync(orderData)
            'End If
            'OnHeartbeat(String.Format("OrderUpdate {0}", Utils.JsonSerialize(orderData)))
        End Sub
    End Class
End Namespace