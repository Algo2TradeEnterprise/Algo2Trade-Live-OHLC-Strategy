Imports System.Collections.Specialized
Imports System.Net
Imports System.Web
Imports System.Threading
Imports Utilities.Network
Imports Utilities.ErrorHandlers
Imports KiteConnect
Imports Utilities
Imports System.Net.Http
Imports NLog

Public Class ZerodhaKiteHelper
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

#Region "Property"
    Private _myAPIKey As String = Nothing
    Private _myAPISecret As String = Nothing
    Private _myAPIVersion As String = Nothing
    Private _myUserId As String = Nothing
    Private _myPassword As String = Nothing
    Private _myRequestToken As String = Nothing
    Private _Kite As Kite = Nothing
    Private _canceller As CancellationTokenSource = Nothing
    Private ReadOnly _LoginURL As String = "https://kite.trade/connect/login"
    Private _user As User = Nothing
    Public Property PublicToken As String = Nothing
    Public Property AccessToken As String = Nothing
    Public Property MaxReTries As Integer = 20
    Public Property WaitDurationOnConnectionFailure As TimeSpan = TimeSpan.FromSeconds(5)
    Public Property WaitDurationOnServiceUnavailbleFailure As TimeSpan = TimeSpan.FromSeconds(30)
    Public Property WaitDurationOnAnyFailure As TimeSpan = TimeSpan.FromSeconds(10)

#End Region

#Region "Constructor"
    Public Sub New(ByVal userID As String, ByVal password As String, ByVal apiKey As String, ByVal apiSecret As String, ByVal apiVersion As String, ByVal canceller As CancellationTokenSource)
        _myUserId = userID
        _myPassword = password
        _myAPIKey = apiKey
        _myAPISecret = apiSecret
        _myAPIVersion = apiVersion
        _canceller = canceller
    End Sub
#End Region

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
        Return String.Format("{0}?api_key={1}&v={2}", _LoginURL, _myAPIKey, -_myAPIVersion)
    End Function
    Public Async Function LoginAsync() As Task(Of Boolean)
        Dim ret As Boolean = False

        Dim postContent As New Dictionary(Of String, String)
        postContent.Add("user_id", _myUserId)
        postContent.Add("password", _myPassword)
        postContent.Add("login", "")

        HttpBrowser.KillCookies()

        Using browser As New HttpBrowser(Nothing, DecompressionMethods.GZip, TimeSpan.FromMinutes(1), _canceller)
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
            headers.Add("X-Kite-version", _myAPIVersion)

            OnHeartbeat("Getting login page")

            Dim tempRet As Tuple(Of Uri, Object) =
            Await browser.NonPOSTRequestAsync(GetLoginURL,
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
                postContent.Add("user_id", _myUserId)
                postContent.Add("password", _myPassword)
                postContent.Add("login", "")

                'Now prepare the step 1 authentication
                headers = New Dictionary(Of String, String)
                headers.Add("Accept", "application/json, text/plain, */*")
                headers.Add("Accept-Language", "en-US")
                'headers.Add("Accept-Encoding", "gzip, deflate, br")
                headers.Add("Content-Type", "application/x-www-form-urlencoded")
                headers.Add("Host", "kite.zerodha.com")
                headers.Add("Origin", "https://kite.zerodha.com")
                headers.Add("X-Kite-version", _myAPIVersion)

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
                                                                                  Uri.EscapeDataString(_myUserId),
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
                        headers.Add("X-Kite-version", _myAPIVersion)

                        _canceller.Token.ThrowIfCancellationRequested()
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
                            _myRequestToken = queryStrings("request_token")
                            logger.Debug("Login complete, request_token:{0}", _myRequestToken)
                            ret = True
                        ElseIf tempRet IsNot Nothing AndAlso tempRet.Item2 IsNot Nothing AndAlso tempRet.Item2.GetType Is GetType(Dictionary(Of String, Object)) AndAlso
                            tempRet.Item2.containskey("status") AndAlso tempRet.Item2("status") = "success" Then

                            headers = New Dictionary(Of String, String)
                            headers.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8")
                            headers.Add("Accept-Encoding", "gzip, deflate, br")
                            headers.Add("Accept-Language", "en-US,en;q=0.5")
                            headers.Add("Host", "kite.zerodha.com")
                            headers.Add("X-Kite-version", _myAPIVersion)
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
                                _myRequestToken = queryStrings("request_token")
                                logger.Debug("Login complete, request_token:{0}", _myRequestToken)
                                ret = True
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
        Return ret
    End Function
    Public Async Function RequestAccessTokenAsync() As Task(Of Boolean)
        'Dont execute login process if _Kite is already connected
        _canceller.Token.ThrowIfCancellationRequested()
        Dim ret As Boolean = False
        Await Task.Delay(1).ConfigureAwait(False)
        If _Kite Is Nothing Then
            _Kite = New Kite(_myAPIKey, Debug:=False)
            ' For handling 403 errors
            _Kite.SetSessionExpiryHook(AddressOf OnTokenExpireAsync)
        End If
        OnHeartbeat("Generating session...")
        Dim user As User = _Kite.GenerateSession(_myRequestToken, _myAPISecret)
        Console.WriteLine(Utils.JsonSerialize(user))
        If user.AccessToken IsNot Nothing Then
            _user = user
            PublicToken = user.PublicToken
            AccessToken = user.AccessToken
            _Kite.SetAccessToken(AccessToken)
            logger.Debug("Acccess generation complete, AccessToken:{0}", AccessToken)
            ret = True
        End If
        Return ret
    End Function
    Private Async Sub OnTokenExpireAsync()
        AccessToken = Nothing
        OnHeartbeat("********************Need To Login Again********************")
        _canceller.Token.ThrowIfCancellationRequested()
        _Kite = Nothing
        Await Task.Delay(2000).ConfigureAwait(False)
        Dim tempRet As Boolean = Await LoginAsync().ConfigureAwait(False)
        If tempRet Then
            'Do the authorization
            Dim accessObtained As Boolean = Await RequestAccessTokenAsync().ConfigureAwait(False)
            If Not accessObtained Then
                Throw New ApplicationException("Login process failed while obtaining access token")
            End If
        End If
    End Sub
#End Region

    Public Async Function ExecuteCommandAsync(ByVal command As KiteCommands, ByVal stockData As Dictionary(Of String, Object)) As Task(Of Dictionary(Of String, Object))
        Dim ret As Dictionary(Of String, Object) = Nothing
        Await Task.Delay(1).ConfigureAwait(False)

        Dim lastException As Exception = Nothing
        Dim allOKWithoutException As Boolean = False

        Using Waiter As New Waiter(_canceller)
            AddHandler Waiter.Heartbeat, AddressOf OnHeartbeat
            AddHandler Waiter.WaitingFor, AddressOf OnWaitingFor

            For retryCtr = 1 To MaxReTries
                _canceller.Token.ThrowIfCancellationRequested()
                While AccessToken <> _user.AccessToken
                    OnHeartbeat(String.Format("Waiting for new access token before executing:{0}...", command))
                    Await Task.Delay(1000).ConfigureAwait(False)
                End While
                lastException = Nothing
                RaiseEvent DocumentRetryStatus(retryCtr, MaxReTries)
                Try
                    OnHeartbeat(String.Format("Executing Kite command:{0}...", command))
                    _canceller.Token.ThrowIfCancellationRequested()
                    Select Case command
                        Case KiteCommands.GetPositions
                            Dim positions As PositionResponse = _Kite.GetPositions()
                            ret = New Dictionary(Of String, Object) From {{command.ToString, positions}}
                            _canceller.Token.ThrowIfCancellationRequested()
                        Case KiteCommands.PlaceOrder
                            If stockData IsNot Nothing AndAlso stockData.Count > 0 Then
                                Dim s As Stopwatch = New Stopwatch
                                s.Start()
                                Dim placedOrders = _Kite.PlaceOrder(Exchange:=CType(stockData("Exchange"), String),
                                                                    TradingSymbol:=CType(stockData("TradingSymbol"), String),
                                                                    TransactionType:=CType(stockData("TransactionType"), String),
                                                                    Quantity:=CType(stockData("Quantity"), Integer),
                                                                    Price:=CType(stockData("Price"), Decimal),
                                                                    Product:=CType(stockData("Product"), String),
                                                                    OrderType:=CType(stockData("OrderType"), String),
                                                                    Validity:=CType(stockData("Validity"), String),
                                                                    SquareOffValue:=CType(stockData("SquareOffValue"), Decimal),
                                                                    StoplossValue:=CType(stockData("StoplossValue"), Decimal),
                                                                    Variety:=CType(stockData("Variety"), String),
                                                                    Tag:=CType(stockData("Tag"), String))
                                s.Stop()

                                Console.WriteLine(String.Format("Placed Order Time:{0}", s.ElapsedMilliseconds))
                                ret = New Dictionary(Of String, Object) From {{command.ToString, placedOrders}}
                            End If
                        Case KiteCommands.ModifyOrderQuantity
                            If stockData IsNot Nothing AndAlso stockData.Count > 0 Then
                                Dim modifiedOrdersQuantity = _Kite.ModifyOrder(OrderId:=CType(stockData("OrderId"), String),
                                                                               Quantity:=CType(stockData("Quantity"), String))

                                ret = New Dictionary(Of String, Object) From {{command.ToString, modifiedOrdersQuantity}}
                            End If
                        Case KiteCommands.ModifyTargetOrderPrice, KiteCommands.ModifyOrderPrice
                            If stockData IsNot Nothing AndAlso stockData.Count > 0 Then
                                Dim modifiedOrdersPrice = _Kite.ModifyOrder(OrderId:=CType(stockData("OrderId"), String),
                                                                            Price:=CType(stockData("Price"), Decimal))

                                ret = New Dictionary(Of String, Object) From {{command.ToString, modifiedOrdersPrice}}
                            End If
                        Case KiteCommands.ModifySLOrderPrice
                            If stockData IsNot Nothing AndAlso stockData.Count > 0 Then
                                Dim modifiedOrdersPrice = _Kite.ModifyOrder(OrderId:=CType(stockData("OrderId"), String),
                                                                            TriggerPrice:=CType(stockData("TriggerPrice"), Decimal))

                                ret = New Dictionary(Of String, Object) From {{command.ToString, modifiedOrdersPrice}}
                            End If
                        Case KiteCommands.CancelOrder
                            If stockData IsNot Nothing AndAlso stockData.Count > 0 Then
                                Dim cancelledOrder = _Kite.CancelOrder(OrderId:=CType(stockData("OrderId"), String),
                                                                   ParentOrderId:=CType(stockData("ParentOrderId"), String),
                                                                   Variety:=CType(stockData("Variety"), String))

                                ret = New Dictionary(Of String, Object) From {{command.ToString, cancelledOrder}}
                            End If
                        Case KiteCommands.GetOrderHistory
                            If stockData IsNot Nothing AndAlso stockData.Count > 0 Then
                                Dim orderList As List(Of Order) = _Kite.GetOrderHistory(OrderId:=CType(stockData("OrderId"), String))
                                ret = New Dictionary(Of String, Object) From {{command.ToString, orderList}}
                            End If
                        Case KiteCommands.GetOrders
                            Dim orderList As List(Of Order) = _Kite.GetOrders()
                            ret = New Dictionary(Of String, Object) From {{command.ToString, orderList}}
                        Case KiteCommands.GetOrderTrades
                            Dim tradeList As List(Of Trade) = Nothing
                            If stockData IsNot Nothing AndAlso stockData.Count > 0 Then
                                tradeList = _Kite.GetOrderTrades(OrderId:=CType(stockData("OrderId"), String))
                            Else
                                tradeList = _Kite.GetOrderTrades()
                            End If
                            ret = New Dictionary(Of String, Object) From {{command.ToString, tradeList}}
                        Case KiteCommands.GetInstruments
                            Dim instruments As List(Of Instrument) = _Kite.GetInstruments()
                            Dim count As Integer = If(instruments Is Nothing, 0, instruments.Count)
                            logger.Debug(String.Format("Fetched {0} instruments from Zerodha", count))
                            If instruments IsNot Nothing AndAlso instruments.Count > 0 Then
                                instruments.RemoveAll(Function(x) x.Exchange = "BFO" Or x.Exchange = "BSE")
                                instruments.RemoveAll(Function(x) x.Segment.EndsWith("OPT"))
                                instruments.RemoveAll(Function(x) x.TradingSymbol.Length > 3 AndAlso x.TradingSymbol.Substring(x.TradingSymbol.Length - 3).StartsWith("-"))
                                count = If(instruments Is Nothing, 0, instruments.Count)
                                logger.Debug(String.Format("After cleanup, fetched {0} instruments from Zerodha", count))
                            End If
                            ret = New Dictionary(Of String, Object) From {{command.ToString, instruments}}
                        Case KiteCommands.InvalidateAccessToken
                            Dim invalidateToken = _Kite.InvalidateAccessToken(AccessToken)
                            lastException = Nothing
                            allOKWithoutException = True
                            Exit For
                        Case Else
                            Throw New ApplicationException("No Command Triggered")
                    End Select
                    If ret IsNot Nothing AndAlso ret.ContainsKey(command.ToString) Then
                        lastException = Nothing
                        allOKWithoutException = True
                        Exit For
                    Else
                        Throw New ApplicationException(String.Format("{0} did not succeed", command.ToString))
                    End If
                    _canceller.Token.ThrowIfCancellationRequested()
                Catch iatex As KiteConnect.TokenException
                    logger.Error(iatex)
                    lastException = iatex
                    _canceller.Token.ThrowIfCancellationRequested()
                    logger.Debug("KITE->Token expired possibility:{0},{1}",
                                             iatex.Message, command)
                    retryCtr -= 1
                    'We will allow it to continue normal flow as the expiry is handled asynchnously via a seperate channel 
                    'and hence no actions necessary here
                Catch opx As OperationCanceledException
                    logger.Error(opx)
                    lastException = opx

                    If Not _canceller.Token.IsCancellationRequested Then
                        _canceller.Token.ThrowIfCancellationRequested()
                        If Not Waiter.WaitOnInternetFailure(WaitDurationOnConnectionFailure) Then
                            'Provide required wait in case internet was already up
                            logger.Debug("HTTP->Task was cancelled without internet problem:{0},{1}",
                                             opx.Message, command)
                            _canceller.Token.ThrowIfCancellationRequested()
                            Waiter.SleepRequiredDuration(WaitDurationOnAnyFailure.TotalSeconds, "Non-explicit cancellation")
                            _canceller.Token.ThrowIfCancellationRequested()
                        Else
                            logger.Debug("HTTP->Task was cancelled due to internet problem:{0},{1}",
                                             opx.Message, command)
                            'Since internet was down, no need to consume retries
                            retryCtr -= 1
                        End If
                    End If
                Catch hex As HttpRequestException
                    logger.Error(hex)
                    lastException = hex

                    'Need to relogin, no point retrying
                    If ExceptionExtensions.GetExceptionMessages(hex).Contains("trust relationship") Then
                        Throw New ForbiddenException(hex.Message, hex, ForbiddenException.TypeOfException.PossibleReloginRequired)
                    End If

                    _canceller.Token.ThrowIfCancellationRequested()
                    If Not Waiter.WaitOnInternetFailure(Me.WaitDurationOnConnectionFailure) Then
                        If hex.Message.Contains("429") Or hex.Message.Contains("503") Then
                            logger.Debug("HTTP->429/503 error without internet problem:{0},{1}",
                                             hex.Message, command)
                            _canceller.Token.ThrowIfCancellationRequested()
                            Waiter.SleepRequiredDuration(WaitDurationOnServiceUnavailbleFailure.TotalSeconds, "Service unavailable(429/503)")
                            _canceller.Token.ThrowIfCancellationRequested()
                            'Since site service is blocked, no need to consume retries
                            retryCtr -= 1
                        ElseIf hex.Message.Contains("404") Then
                            logger.Debug("HTTP->404 error without internet problem:{0},{1}",
                                             hex.Message, command)
                            _canceller.Token.ThrowIfCancellationRequested()
                            'No point retrying, exit for
                            Exit For
                        Else
                            If ExceptionExtensions.IsExceptionConnectionRelated(hex) Then
                                logger.Debug("HTTP->HttpRequestException without internet problem but of type internet related detected:{0},{1}",
                                                 hex.Message, command)
                                _canceller.Token.ThrowIfCancellationRequested()
                                Waiter.SleepRequiredDuration(WaitDurationOnConnectionFailure.TotalSeconds, "Connection HttpRequestException")
                                _canceller.Token.ThrowIfCancellationRequested()
                                'Since exception was internet related, no need to consume retries
                                retryCtr -= 1
                            Else
                                'Provide required wait in case internet was already up
                                logger.Debug("HTTP->HttpRequestException without internet problem:{0},{1}",
                                                 hex.Message, command)
                                _canceller.Token.ThrowIfCancellationRequested()
                                Waiter.SleepRequiredDuration(WaitDurationOnAnyFailure.TotalSeconds, "Unknown HttpRequestException")
                                _canceller.Token.ThrowIfCancellationRequested()
                            End If
                        End If
                    Else
                        logger.Debug("HTTP->HttpRequestException with internet problem:{0},{1}",
                                         hex.Message, command)
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

                    _canceller.Token.ThrowIfCancellationRequested()
                    If Not Waiter.WaitOnInternetFailure(Me.WaitDurationOnConnectionFailure) Then
                        'Provide required wait in case internet was already up
                        _canceller.Token.ThrowIfCancellationRequested()
                        If ExceptionExtensions.IsExceptionConnectionRelated(ex) Then
                            logger.Debug("HTTP->Exception without internet problem but of type internet related detected:{0},{1}",
                                             ex.Message, command)
                            _canceller.Token.ThrowIfCancellationRequested()
                            Waiter.SleepRequiredDuration(WaitDurationOnConnectionFailure.TotalSeconds, "Connection Exception")
                            _canceller.Token.ThrowIfCancellationRequested()
                            'Since exception was internet related, no need to consume retries
                            retryCtr -= 1
                        Else
                            logger.Debug("HTTP->Exception without internet problem of unknown type detected:{0},{1}",
                                             ex.Message, command)
                            _canceller.Token.ThrowIfCancellationRequested()
                            Waiter.SleepRequiredDuration(WaitDurationOnAnyFailure.TotalSeconds, "Unknown Exception")
                            _canceller.Token.ThrowIfCancellationRequested()
                        End If
                    Else
                        logger.Debug("HTTP->Exception with internet problem:{0},{1}",
                                         ex.Message, command)
                        'Since internet was down, no need to consume retries
                        retryCtr -= 1
                    End If
                Finally
                    OnDocumentDownloadComplete()
                End Try
                _canceller.Token.ThrowIfCancellationRequested()
                GC.Collect()
            Next
            RemoveHandler Waiter.Heartbeat, AddressOf OnHeartbeat
            RemoveHandler Waiter.WaitingFor, AddressOf OnWaitingFor
        End Using
        _canceller.Token.ThrowIfCancellationRequested()
        If Not allOKWithoutException Then Throw lastException
        Return ret
    End Function
    Public Enum KiteCommands
        GetPositions = 1
        PlaceOrder
        ModifyOrderQuantity
        ModifyOrderPrice
        ModifyTargetOrderPrice
        ModifySLOrderPrice
        CancelOrder
        GetOrderHistory
        GetOrders
        GetOrderTrades
        GetInstruments
        InvalidateAccessToken
        None
    End Enum
End Class
