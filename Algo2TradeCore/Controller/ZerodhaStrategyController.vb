Imports System.Collections.Specialized
Imports System.Net
Imports Utilities.ErrorHandlers
Imports System.Threading
Imports System.Web
Imports Algo2TradeCore.Entity
Imports NLog
Imports Utilities.Network
Imports KiteConnect
Imports Algo2TradeCore.Adapter
Namespace Controller
    Public Class ZerodhaStrategyController
        Inherits APIStrategyController

#Region "Logging and Status Progress"
        Public Shared Shadows logger As Logger = LogManager.GetCurrentClassLogger
#End Region

        Private _Kite As Kite

        Public Sub New(ByVal currentUser As IUser,
                       ByVal canceller As CancellationTokenSource)
            MyBase.New(currentUser, canceller)
            _LoginURL = "https://kite.trade/connect/login"
        End Sub
        Protected Overrides Function GetLoginURL() As String
            logger.Debug("GetLoginURL, parameters:Nothing")
            Return String.Format("{0}?api_key={1}&v={2}", _LoginURL, _currentUser.APIKey, _currentUser.APIVersion)
        End Function
        Public Overrides Function GetErrorResponse(ByVal responseDict As Object) As String
            logger.Debug("GetErrorResponse, responseDict:{0}", Utils.JsonSerialize(responseDict))
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
            Return APIConnection
        End Function
        Private Async Function RequestAccessTokenAsync(ByVal requestToken As String) As Task(Of ZerodhaConnection)
            logger.Debug("RequestAccessTokenAsync, requestToken:{0}", requestToken)
            'Dont execute login process if _Kite is already connected
            _cts.Token.ThrowIfCancellationRequested()
            Dim ret As ZerodhaConnection = Nothing
            Await Task.Delay(0).ConfigureAwait(False)
            If _Kite Is Nothing Then
                _Kite = New Kite(_currentUser.APIKey, Debug:=True)
                ' For handling 403 errors
                _Kite.SetSessionExpiryHook(AddressOf OnSessionExpireAsync)
            End If
            OnHeartbeat("Generating session...")
            Dim user As User = _Kite.GenerateSession(requestToken, _currentUser.APISecret)
            Console.WriteLine(Utils.JsonSerialize(user))
            If user.AccessToken IsNot Nothing Then
                _Kite.SetAccessToken(user.AccessToken)
                logger.Debug("Acccess generation complete, user.AccessToken:{0}", user.AccessToken)

                ret = New ZerodhaConnection
                With ret
                    .ZerodhaAccessToken = user.AccessToken
                    .ZerodhaPublicToken = user.PublicToken
                    .ZerodhaRequestToken = requestToken
                    .ZerodhaUser = New ZerodhaUser() With {.UserId = _currentUser.UserId,
                                                            .Password = _currentUser.Password,
                                                            .APIKey = _currentUser.APIKey,
                                                            .API2FA = _currentUser.API2FA,
                                                            .APISecret = _currentUser.APISecret,
                                                            .APIVersion = _currentUser.APIVersion,
                                                            .WrappedUser = user}
                End With
            End If
            Return ret
        End Function
        Public Async Sub OnSessionExpireAsync()
            logger.Debug("RequestAccessTokenAsync, OnSessionExpireAsync:Nothing")
            'Wait for the lock and if locked, then exit immediately
            Await _LoginSemphore.WaitAsync(0).ConfigureAwait(False)
            OnHeartbeat("********** Need to login again **********")
            Try
                _cts.Token.ThrowIfCancellationRequested()
                _Kite = Nothing
                APIConnection = Nothing
                Await Task.Delay(2000).ConfigureAwait(False)
                Dim tempRet As ZerodhaConnection = Await LoginAsync().ConfigureAwait(False)
                If tempRet Is Nothing Then
                    Throw New ApplicationException("Login process failed after token expiry")
                End If
            Finally
                _LoginSemphore.Release()
            End Try
        End Sub
        Public Async Function TestAsync() As Task

            While True
                Dim prevAccessToken As String = CType(APIConnection, ZerodhaConnection).ZerodhaAccessToken
                Try
                    Dim adap As New ZerodhaAdapter(Me, _cts)
                    OnHeartbeat("***************** ##### Executing command againa")
                    Dim ret = Await adap.ExecuteCommandAsync(ZerodhaAdapter.KiteCommands.GetOrderTrades, Nothing).ConfigureAwait(False)
                    OnHeartbeat(Utils.JsonSerialize(ret))
                    Await Task.Delay(5000).ConfigureAwait(False)
                Catch tex As TokenException
                    Dim newAccessToken As String = prevAccessToken
                    While prevAccessToken = newAccessToken
                        If APIConnection IsNot Nothing Then
                            newAccessToken = CType(APIConnection, ZerodhaConnection).ZerodhaAccessToken
                        End If
                        Task.Delay(500).ConfigureAwait(False)
                    End While
                End Try
            End While
        End Function
    End Class
End Namespace