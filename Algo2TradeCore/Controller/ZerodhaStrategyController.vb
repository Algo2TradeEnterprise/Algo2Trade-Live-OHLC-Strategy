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

        Public Sub New(ByVal currentUser As ZerodhaUser,
                       ByVal canceller As CancellationTokenSource)
            MyBase.New(currentUser, canceller)
            _LoginURL = "https://kite.trade/connect/login"
        End Sub

#Region "Login"
        Protected Overrides Function GetLoginURL() As String
            logger.Debug("GetLoginURL, parameters:Nothing")
            Return String.Format("{0}?api_key={1}&v={2}", _LoginURL, _currentUser.APIKey, _currentUser.APIVersion)
        End Function
        Public Overrides Function GetErrorResponse(ByVal responseDict As Object) As String
            If responseDict IsNot Nothing AndAlso
                responseDict.GetType = GetType(Dictionary(Of String, Object)) AndAlso
                CType(responseDict, Dictionary(Of String, Object)).Count < 50 Then
                logger.Debug("GetErrorResponse, responseDict:{0}", Utils.JsonSerialize(responseDict))
            ElseIf responseDict IsNot Nothing AndAlso
                responseDict.GetType = GetType(Dictionary(Of String, Object)) AndAlso
                CType(responseDict, Dictionary(Of String, Object)).Count > 50 Then
                logger.Debug("GetErrorResponse, responseDict:Too large")
            Else
                logger.Debug("GetErrorResponse, responseDict:Nothing or not Dictionary of String,Object")
            End If
            Dim ret As String = Nothing
            If responseDict IsNot Nothing AndAlso
               responseDict.GetType = GetType(Dictionary(Of String, Object)) AndAlso
               CType(responseDict, Dictionary(Of String, Object)).ContainsKey("status") AndAlso
               CType(responseDict, Dictionary(Of String, Object))("status") = "error" AndAlso
               CType(responseDict, Dictionary(Of String, Object)).ContainsKey("message") Then
                ret = String.Format("Zerodha reported error, message:{0}", CType(responseDict, Dictionary(Of String, Object))("message"))
            End If
            Return ret
        End Function
        Public Overrides Async Function LoginAsync() As Task(Of IConnection)
            logger.Debug("LoginAsync, parameters:Nothing")
            While True
                Try
                    Dim requestToken As String = Nothing

                    Dim postContent As New Dictionary(Of String, String)
                    postContent.Add("user_id", _currentUser.UserId)
                    postContent.Add("password", _currentUser.Password)
                    postContent.Add("login", "")

                    HttpBrowser.KillCookies()

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

                        OnHeartbeat("Getting login page")

                        Dim tempRet As Tuple(Of Uri, Object) = Await browser.NonPOSTRequestAsync(GetLoginURL,
                                                                                              Http.HttpMethod.Get,
                                                                                              Nothing,
                                                                                              False,
                                                                                              headers,
                                                                                              False).ConfigureAwait(False)

                        'Should be getting back the redirected URL in Item1 and the htmldocument response in Item2
                        Dim finalURLToCall As Uri = Nothing
                        If tempRet IsNot Nothing AndAlso tempRet.Item1 IsNot Nothing AndAlso tempRet.Item1.ToString.Contains("sess_id") Then
                            logger.Debug("Getting login page, sess_id:{0}", tempRet.Item1.ToString.Contains("sess_id"))
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
                            tempRet = Await browser.POSTRequestAsync("https://kite.zerodha.com/api/login",
                                                     redirectedURI.ToString,
                                                     postContent,
                                                     False,
                                                     headers,
                                                     False).ConfigureAwait(False)
                            'Should come back as redirected url in Item1 and htmldocument in Item2
                            Dim q1 As String = Nothing
                            Dim q2 As String = Nothing

                            If tempRet IsNot Nothing AndAlso tempRet.Item2 IsNot Nothing AndAlso tempRet.Item2.GetType Is GetType(Dictionary(Of String, Object)) AndAlso
                    tempRet.Item2.containskey("status") AndAlso tempRet.Item2("status") = "success" AndAlso
                    tempRet.Item2.containskey("data") AndAlso tempRet.Item2("data").containskey("question_ids") AndAlso tempRet.Item2("data")("question_ids").count >= 2 Then
                                q1 = tempRet.Item2("data")("question_ids")(0)
                                q2 = tempRet.Item2("data")("question_ids")(1)
                                If q1 IsNot Nothing AndAlso q2 IsNot Nothing Then
                                    'Now preprate the 2 step authentication
                                    Dim stringPostContent As New Http.StringContent(String.Format("user_id={0}&question_id={1}&question_id={2}&answer={3}&answer={4}",
                                                                                      Uri.EscapeDataString(_currentUser.UserId),
                                                                                      Uri.EscapeDataString(q1),
                                                                                      Uri.EscapeDataString(q2),
                                                                                      Uri.EscapeDataString("a"),
                                                                                      Uri.EscapeDataString("a")),
                                                                        Text.Encoding.UTF8, "application/x-www-form-urlencoded")

                                    logger.Debug("Submitting 2FA, stringPostContent:{0}", Await stringPostContent.ReadAsStringAsync().ConfigureAwait(False))
                                    headers = New Dictionary(Of String, String)
                                    headers.Add("Accept", "application/json, text/plain, */*")
                                    headers.Add("Accept-Language", "en-US,en;q=0.5")
                                    'headers.Add("Accept-Encoding", "gzip, deflate, br")
                                    headers.Add("Content-Type", "application/x-www-form-urlencoded")
                                    headers.Add("Host", "kite.zerodha.com")
                                    headers.Add("Origin", "https://kite.zerodha.com")
                                    headers.Add("X-Kite-version", _currentUser.APIVersion)

                                    _cts.Token.ThrowIfCancellationRequested()
                                    tempRet = Nothing
                                    OnHeartbeat("Submitting 2FA")
                                    tempRet = Await browser.POSTRequestAsync("https://kite.zerodha.com/api/twofa",
                                                                 redirectedURI.ToString,
                                                                 stringPostContent,
                                                                 False,
                                                                 headers,
                                                                 False).ConfigureAwait(False)

                                    'Should come back as redirect url in Item1 and htmldocument response in Item2
                                    If tempRet IsNot Nothing AndAlso tempRet.Item1 IsNot Nothing AndAlso tempRet.Item1.ToString.Contains("request_token") Then
                                        redirectedURI = tempRet.Item1
                                        Dim queryStrings As NameValueCollection = HttpUtility.ParseQueryString(redirectedURI.Query)
                                        requestToken = queryStrings("request_token")
                                        logger.Debug("Authentication complete, requestToken:{0}", requestToken)
                                    ElseIf tempRet IsNot Nothing AndAlso tempRet.Item2 IsNot Nothing AndAlso tempRet.Item2.GetType Is GetType(Dictionary(Of String, Object)) AndAlso
                                tempRet.Item2.containskey("status") AndAlso tempRet.Item2("status") = "success" Then

                                        headers = New Dictionary(Of String, String)
                                        headers.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8")
                                        headers.Add("Accept-Encoding", "gzip, deflate, br")
                                        headers.Add("Accept-Language", "en-US,en;q=0.5")
                                        headers.Add("Host", "kite.zerodha.com")
                                        headers.Add("X-Kite-version", _currentUser.APIVersion)
                                        tempRet = Nothing

                                        OnHeartbeat("Addressing redirection")
                                        tempRet = Await browser.NonPOSTRequestAsync(String.Format("{0}&skip_session=true", finalURLToCall.ToString),
                                                                        Http.HttpMethod.Get,
                                                                        finalURLToCall.ToString,
                                                                        False,
                                                                        headers,
                                                                        True).ConfigureAwait(False)
                                        If tempRet IsNot Nothing AndAlso tempRet.Item1 IsNot Nothing AndAlso tempRet.Item1.ToString.Contains("request_token") Then
                                            redirectedURI = tempRet.Item1
                                            Dim queryStrings As NameValueCollection = HttpUtility.ParseQueryString(redirectedURI.Query)
                                            requestToken = queryStrings("request_token")
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
                        APIConnection = Await RequestAccessTokenAsync(requestToken).ConfigureAwait(False)
                    End If
                Catch tex As KiteConnect.TokenException
                    logger.Error(tex)
                    OnHeartbeat("Error possibiliy while generating session, token may be invalid, retrying the whole login process")
                    Continue While
                End Try
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
                    OnDocumentRetryStatus(retryCtr, _MaxReTries)
                    Try
                        _cts.Token.ThrowIfCancellationRequested()
                        OnHeartbeat(String.Format("Firing Zerodha command to generate session, command:{0}, requestToken:{1}, _currentUser.APISecret:{2}",
                                                    "GenerateSession", requestToken, _currentUser.APISecret))
                        Dim user As User = kiteConnector.GenerateSession(requestToken, _currentUser.APISecret)
                        Console.WriteLine(Utils.JsonSerialize(user))
                        _cts.Token.ThrowIfCancellationRequested()
                        logger.Debug("Processing response")

                        If user.AccessToken IsNot Nothing Then
                            kiteConnector.SetAccessToken(user.AccessToken)
                            logger.Debug("Acccess generation complete, user.AccessToken:{0}", user.AccessToken)

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
            logger.Debug("RequestAccessTokenAsync, OnSessionExpireAsync:Nothing")
            'Wait for the lock and if locked, then exit immediately
            If _LoginThreads = 0 Then
                Interlocked.Increment(_LoginThreads)
                APIConnection = Nothing
            Else
                Exit Sub
            End If
            OnHeartbeat("********** Need to login again **********")
            Try
                _cts.Token.ThrowIfCancellationRequested()
                Await Task.Delay(2000).ConfigureAwait(False)
                Dim tempRet As ZerodhaConnection = Await LoginAsync().ConfigureAwait(False)
                If tempRet Is Nothing Then
                    Throw New ApplicationException("Login process failed after token expiry")
                End If
            Finally
                Interlocked.Decrement(_LoginThreads)
            End Try
        End Sub
#End Region

#Region "Common tasks for all strategies"
        Public Overrides Async Function PrepareToRunStrategyAsync() As Task(Of Boolean)
            logger.Debug("PrepareToRunStrategyAsync, parameter:Nothing")
            _cts.Token.ThrowIfCancellationRequested()

            Dim ret As Boolean = False
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
                    OnHeartbeat("Getting all instruments for the day...")
                    OnDocumentRetryStatus(retryCtr, _MaxReTries)
                    Try
                        _cts.Token.ThrowIfCancellationRequested()
                        While APIConnection Is Nothing
                            logger.Debug("Waiting for fresh token in controller before calling GetAllInstrumentsAsync")
                            Await Task.Delay(500).ConfigureAwait(False)
                        End While
                        _APIAdapter.SetAPIAccessToken(APIConnection.AccessToken)

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
                AddHandler strategyToRun.Heartbeat, AddressOf OnHeartbeat
                AddHandler strategyToRun.WaitingFor, AddressOf OnWaitingFor
                AddHandler strategyToRun.DocumentRetryStatus, AddressOf OnDocumentRetryStatus
                AddHandler strategyToRun.DocumentDownloadComplete, AddressOf OnDocumentDownloadComplete
                OnHeartbeat(String.Format("As per the strategy logic, tradable instruments being fetched, strategy:{0}", strategyToRun.ToString))
                Dim ret As Boolean = Await strategyToRun.FillTradableInstrumentsAsync(_AllInstruments).ConfigureAwait(False)
                _cts.Token.ThrowIfCancellationRequested()
                If Not ret Then Throw New ApplicationException(String.Format("No instruments fetched that can be traded, strategy:{0}", strategyToRun.ToString))
                OnHeartbeat(String.Format("Executing the strategy by creating relevant instrument workers, strategy:{0}", strategyToRun.ToString))
                Await strategyToRun.ExecuteAsync().ConfigureAwait(False)
                _cts.Token.ThrowIfCancellationRequested()
            End If
        End Function
#End Region
        Public Async Function TestAsync() As Task
            While True
                Dim prevAccessToken As String = CType(APIConnection, ZerodhaConnection).AccessToken
                Try
                    Dim adap As New ZerodhaAdapter(Me, _cts)
                    OnHeartbeat("***************** ##### Executing command againa")
                    'Dim ret = Await adap.ExecuteCommandAsync(ZerodhaAdapter.KiteCommands.GetOrderTrades, Nothing).ConfigureAwait(False)
                    Dim ret = Await adap.GetAllInstrumentsAsync().ConfigureAwait(False)
                    OnHeartbeat(Utils.JsonSerialize(ret))
                    Await Task.Delay(5000).ConfigureAwait(False)
                Catch tex As TokenException
                    Dim newAccessToken As String = prevAccessToken
                    While prevAccessToken = newAccessToken
                        If APIConnection IsNot Nothing Then
                            newAccessToken = CType(APIConnection, ZerodhaConnection).AccessToken
                        End If
                        Task.Delay(500).ConfigureAwait(False)
                    End While
                Catch ex As Exception
                    Console.WriteLine("Exzception")
                End Try
            End While
        End Function
    End Class
End Namespace