Imports System.Collections.Specialized
Imports System.Net
Imports System.Threading
Imports System.Web
Imports Algo2TradeCore.Entity
Imports Algo2TradeCore.Subscriber
Imports KiteConnect
Imports NLog
Imports Utilities.ErrorHandlers
Imports Utilities.Network

Namespace Adapter
    Public Class ZerodhaAdapter
        Inherits APIAdapter



#Region "Logging and Status Progress"
        Public Shared logger As Logger = LogManager.GetCurrentClassLogger
#End Region

        Protected _APISecret As String
        Protected _APIKey As String
        Protected _APIVersion As String
        Protected _API2FA As Dictionary(Of String, String)
        Protected _Kite As Kite
        Protected _Ticker As Ticker
        Protected _ZerodhaConnection As ZerodhaConnection

        Private ReadOnly _LoginURL As String = "https://kite.trade/connect/login"
        Private _loginSemphore As New SemaphoreSlim(1, 1)
        Public Sub New(ByVal userId As String,
                       ByVal password As String,
                       ByVal APISecret As String,
                       ByVal APIKey As String,
                       ByVal APIVersion As String,
                       ByVal API2FA As Dictionary(Of String, String),
                       ByVal canceller As CancellationTokenSource)
            MyBase.New(userId, password, canceller)
            _APISecret = APISecret
            _APIKey = APIKey
            _APIVersion = APIVersion
            _API2FA = API2FA
        End Sub

#Region "Login"
        Private Function GetErrorResponse(ByVal responseDict As Dictionary(Of String, Object)) As String
            Dim ret As String = Nothing
            If responseDict IsNot Nothing AndAlso
            responseDict.ContainsKey("status") AndAlso responseDict("status") = "error" AndAlso responseDict.ContainsKey("message") Then
                ret = String.Format("Zerodha reported error:{0}", responseDict("message"))
            End If
            Return ret
        End Function
        Private Function GetLoginURL() As String
            Return String.Format("{0}?api_key={1}&v={2}", _LoginURL, _APIKey, _APIVersion)
        End Function
        Public Overrides Async Function LoginAsync() As Task(Of IConnection)
            Dim requestToken As String = Nothing

            Dim postContent As New Dictionary(Of String, String)
            postContent.Add("user_id", _userId)
            postContent.Add("password", _password)
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
                headers.Add("X-Kite-version", _APIVersion)

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
                    logger.Debug("Getting login page: sess_id retrieved is:{0}", tempRet.Item1.ToString.Contains("sess_id"))
                    finalURLToCall = tempRet.Item1
                    redirectedURI = tempRet.Item1

                    postContent = New Dictionary(Of String, String)
                    postContent.Add("user_id", _userId)
                    postContent.Add("password", _password)
                    postContent.Add("login", "")

                    'Now prepare the step 1 authentication
                    headers = New Dictionary(Of String, String)
                    headers.Add("Accept", "application/json, text/plain, */*")
                    headers.Add("Accept-Language", "en-US")
                    'headers.Add("Accept-Encoding", "gzip, deflate, br")
                    headers.Add("Content-Type", "application/x-www-form-urlencoded")
                    headers.Add("Host", "kite.zerodha.com")
                    headers.Add("Origin", "https://kite.zerodha.com")
                    headers.Add("X-Kite-version", _APIVersion)

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
                                                                                  Uri.EscapeDataString(_userId),
                                                                                  Uri.EscapeDataString(q1),
                                                                                  Uri.EscapeDataString(q2),
                                                                                  Uri.EscapeDataString("a"),
                                                                                  Uri.EscapeDataString("a")),
                                                                    Text.Encoding.UTF8, "application/x-www-form-urlencoded")

                            logger.Debug("Submitting 2FA: Post content being used {0}", Await stringPostContent.ReadAsStringAsync().ConfigureAwait(False))
                            headers = New Dictionary(Of String, String)
                            headers.Add("Accept", "application/json, text/plain, */*")
                            headers.Add("Accept-Language", "en-US,en;q=0.5")
                            'headers.Add("Accept-Encoding", "gzip, deflate, br")
                            headers.Add("Content-Type", "application/x-www-form-urlencoded")
                            headers.Add("Host", "kite.zerodha.com")
                            headers.Add("Origin", "https://kite.zerodha.com")
                            headers.Add("X-Kite-version", _APIVersion)

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
                                headers.Add("X-Kite-version", _APIVersion)
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
                _ZerodhaConnection = Await RequestAccessTokenAsync(requestToken).ConfigureAwait(False)
            End If
            Return _ZerodhaConnection
        End Function
        Private Async Function RequestAccessTokenAsync(ByVal requestToken As String) As Task(Of ZerodhaConnection)
            'Dont execute login process if _Kite is already connected
            _cts.Token.ThrowIfCancellationRequested()
            Dim ret As ZerodhaConnection = Nothing
            Await Task.Delay(1).ConfigureAwait(False)
            If _Kite Is Nothing Then
                _Kite = New Kite(_APIKey, Debug:=False)
                ' For handling 403 errors
                _Kite.SetSessionExpiryHook(AddressOf OnTokenExpireAsync)
            End If
            OnHeartbeat("Generating session...")
            Dim user As User = _Kite.GenerateSession(requestToken, _APIVersion)
            Console.WriteLine(Utils.JsonSerialize(user))
            If user.AccessToken IsNot Nothing Then
                _Kite.SetAccessToken(user.AccessToken)
                logger.Debug("Acccess generation complete, AccessToken:{0}", user.AccessToken)

                ret = New ZerodhaConnection
                With ret
                    .UserId = _userId
                    .Password = _password
                    .APIKey = _APIKey
                    .APISecret = _APISecret
                    .APIVersion = _APIVersion
                    .API2FA = _API2FA
                    .APIRequestToken = requestToken
                    .APIUser = New ZerodhaUser() With {.WrappedUser = user}
                End With
            End If
            Return ret
        End Function
        Private Async Sub OnTokenExpireAsync()
            'Wait for the lock and if locked, then exit immediately
            Await _loginSemphore.WaitAsync(0).ConfigureAwait(False)
            OnHeartbeat("********** Need to login again **********")
            Try
                _cts.Token.ThrowIfCancellationRequested()
                _Kite = Nothing
                _ZerodhaConnection = Nothing
                Await Task.Delay(2000).ConfigureAwait(False)
                Dim tempRet As ZerodhaConnection = Await LoginAsync().ConfigureAwait(False)
                If tempRet Is Nothing Then
                    Throw New ApplicationException("Login process failed after token expiry")
                End If
            Finally
                _loginSemphore.Release()
            End Try
        End Sub
        Public Overrides Async Function ConnectTickerAsync(ByVal subscriber As APIInstrumentSubscriber) As Task
            Await Task.Delay(0).ConfigureAwait(False)
            _Ticker = New Ticker(_ZerodhaConnection.APIKey, _ZerodhaConnection.APIUser.WrappedUser.AccessToken)
            Dim zerodhaSubscriber As ZerodhaInstrumentSubscriber = CType(subscriber, ZerodhaInstrumentSubscriber)
            AddHandler _Ticker.OnTick, AddressOf zerodhaSubscriber.OnTickAsync
            AddHandler _Ticker.OnReconnect, AddressOf zerodhaSubscriber.OnReconnect
            AddHandler _Ticker.OnNoReconnect, AddressOf zerodhaSubscriber.OnNoReconnect
            AddHandler _Ticker.OnError, AddressOf zerodhaSubscriber.OnError
            AddHandler _Ticker.OnClose, AddressOf zerodhaSubscriber.OnClose
            AddHandler _Ticker.OnConnect, AddressOf zerodhaSubscriber.OnConnect
            AddHandler _Ticker.OnOrderUpdate, AddressOf zerodhaSubscriber.OnOrderUpdateAsync

            _Ticker.EnableReconnect(Interval:=5, Retries:=50)
            _Ticker.Connect()
            If zerodhaSubscriber.SubcribedInstruments IsNot Nothing AndAlso zerodhaSubscriber.SubcribedInstruments.Count > 0 Then
                For Each runningSubcribedInstrument In zerodhaSubscriber.SubcribedInstruments
                    _Ticker.Subscribe(Tokens:=New UInt32() {runningSubcribedInstrument})
                    _Ticker.SetMode(Tokens:=New UInt32() {runningSubcribedInstrument}, Mode:=Constants.MODE_FULL)
                Next
            End If
        End Function
#End Region
    End Class
End Namespace
