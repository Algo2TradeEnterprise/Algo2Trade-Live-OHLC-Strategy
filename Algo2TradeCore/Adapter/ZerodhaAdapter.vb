Imports System.Collections.Specialized
Imports System.Net
Imports System.Net.Http
Imports System.Threading
Imports System.Web
Imports Algo2TradeCore.Entity
Imports Algo2TradeCore.Subscriber
Imports KiteConnect
Imports NLog
Imports Utilities
Imports Utilities.ErrorHandlers
Imports Utilities.Network

Namespace Adapter
    Public Class ZerodhaAdapter
        Inherits APIAdapter

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
            _MaxInstrumentPerTicker = 900
        End Sub

        Public Overrides Async Function GetAllInstrumentsAsync(Optional ByVal isRetryEnabled As Boolean = True) As Task(Of IEnumerable(Of IInstrument))
            Dim ret As List(Of ZerodhaInstrument) = Nothing
            Dim command As KiteCommands = KiteCommands.GetInstruments
            Dim tempAllRet As Dictionary(Of String, Object) = Await ExecuteCommandAsync(command, Nothing, isRetryEnabled).ConfigureAwait(False)

            Dim tempRet As Object = Nothing
            If tempAllRet.ContainsKey(command.ToString) Then
                tempRet = tempAllRet(command.ToString)
                If tempRet IsNot Nothing Then
                    Dim errorMessage As String = GetErrorResponse(tempRet)
                    If errorMessage IsNot Nothing Then
                        Throw New ApplicationException(errorMessage)
                    End If
                Else
                    Throw New ApplicationException(String.Format("No return fetched after executing command:{0}", command.ToString))
                End If
            Else
                Throw New ApplicationException(String.Format("Relevant command fired was not detected in the response:{0}", command.ToString))
            End If

            If tempRet.GetType = GetType(List(Of Instrument)) Then
                For Each runningInstrument As Instrument In CType(tempRet, List(Of Instrument))
                    If ret Is Nothing Then ret = New List(Of ZerodhaInstrument)
                    ret.Add(New ZerodhaInstrument(runningInstrument.InstrumentToken) With {.WrappedInstrument = runningInstrument})
                Next
            Else
                Throw New ApplicationException(String.Format("List of instruments not returned from command:{0}", command.ToString))
            End If
            Return ret
        End Function

        Private Async Function ExecuteCommandAsync(ByVal command As KiteCommands, ByVal stockData As Dictionary(Of String, Object), Optional ByVal isRetryEnabled As Boolean = True) As Task(Of Dictionary(Of String, Object))
            Dim ret As Dictionary(Of String, Object) = Nothing

            Dim lastException As Exception = Nothing
            Dim allOKWithoutException As Boolean = False

            Using Waiter As New Waiter(_cts)
                AddHandler Waiter.Heartbeat, AddressOf OnHeartbeat
                AddHandler Waiter.WaitingFor, AddressOf OnWaitingFor

                For retryCtr = 1 To _MaxReTries
                    _cts.Token.ThrowIfCancellationRequested()
                    While _Kite Is Nothing
                        OnHeartbeat(String.Format("Waiting for new access token before executing:{0}...", command))
                        Await Task.Delay(1000).ConfigureAwait(False)
                    End While
                    lastException = Nothing
                    OnDocumentRetryStatus(retryCtr, _MaxReTries)
                    Try
                        OnHeartbeat(String.Format("Executing Kite command:{0}...", command))
                        _cts.Token.ThrowIfCancellationRequested()

                        Select Case command
                            Case KiteCommands.GetPositions
                                Dim positions As PositionResponse = _Kite.GetPositions()
                                ret = New Dictionary(Of String, Object) From {{command.ToString, positions}}
                                _cts.Token.ThrowIfCancellationRequested()
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
                                Dim invalidateToken = _Kite.InvalidateAccessToken(_ZerodhaConnection.APIUser.WrappedUser.AccessToken)
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
                        _cts.Token.ThrowIfCancellationRequested()
                    Catch iatex As KiteConnect.TokenException
                        logger.Error(iatex)
                        lastException = iatex
                        _cts.Token.ThrowIfCancellationRequested()
                        logger.Debug("KITE->Token expired possibility:{0},{1}",
                                             iatex.Message, command)
                        retryCtr -= 1
                        'We will allow it to continue normal flow as the expiry is handled asynchnously via a seperate channel 
                        'and hence no actions necessary here
                    Catch opx As OperationCanceledException
                        logger.Error(opx)
                        lastException = opx

                        If Not _cts.Token.IsCancellationRequested Then
                            _cts.Token.ThrowIfCancellationRequested()
                            If Not Waiter.WaitOnInternetFailure(_WaitDurationOnConnectionFailure) Then
                                'Provide required wait in case internet was already up
                                logger.Debug("HTTP->Task was cancelled without internet problem:{0},{1}",
                                             opx.Message, command)
                                _cts.Token.ThrowIfCancellationRequested()
                                Waiter.SleepRequiredDuration(_WaitDurationOnAnyFailure.TotalSeconds, "Non-explicit cancellation")
                                _cts.Token.ThrowIfCancellationRequested()
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

                        _cts.Token.ThrowIfCancellationRequested()
                        If Not Waiter.WaitOnInternetFailure(_WaitDurationOnConnectionFailure) Then
                            If hex.Message.Contains("429") Or hex.Message.Contains("503") Then
                                logger.Debug("HTTP->429/503 error without internet problem:{0},{1}",
                                             hex.Message, command)
                                _cts.Token.ThrowIfCancellationRequested()
                                Waiter.SleepRequiredDuration(_WaitDurationOnServiceUnavailbleFailure.TotalSeconds, "Service unavailable(429/503)")
                                _cts.Token.ThrowIfCancellationRequested()
                                'Since site service is blocked, no need to consume retries
                                retryCtr -= 1
                            ElseIf hex.Message.Contains("404") Then
                                logger.Debug("HTTP->404 error without internet problem:{0},{1}",
                                             hex.Message, command)
                                _cts.Token.ThrowIfCancellationRequested()
                                'No point retrying, exit for
                                Exit For
                            Else
                                If ExceptionExtensions.IsExceptionConnectionRelated(hex) Then
                                    logger.Debug("HTTP->HttpRequestException without internet problem but of type internet related detected:{0},{1}",
                                                 hex.Message, command)
                                    _cts.Token.ThrowIfCancellationRequested()
                                    Waiter.SleepRequiredDuration(_WaitDurationOnConnectionFailure.TotalSeconds, "Connection HttpRequestException")
                                    _cts.Token.ThrowIfCancellationRequested()
                                    'Since exception was internet related, no need to consume retries
                                    retryCtr -= 1
                                Else
                                    'Provide required wait in case internet was already up
                                    logger.Debug("HTTP->HttpRequestException without internet problem:{0},{1}",
                                                 hex.Message, command)
                                    _cts.Token.ThrowIfCancellationRequested()
                                    Waiter.SleepRequiredDuration(_WaitDurationOnAnyFailure.TotalSeconds, "Unknown HttpRequestException")
                                    _cts.Token.ThrowIfCancellationRequested()
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

                        _cts.Token.ThrowIfCancellationRequested()
                        If Not Waiter.WaitOnInternetFailure(_WaitDurationOnConnectionFailure) Then
                            'Provide required wait in case internet was already up
                            _cts.Token.ThrowIfCancellationRequested()
                            If ExceptionExtensions.IsExceptionConnectionRelated(ex) Then
                                logger.Debug("HTTP->Exception without internet problem but of type internet related detected:{0},{1}",
                                             ex.Message, command)
                                _cts.Token.ThrowIfCancellationRequested()
                                Waiter.SleepRequiredDuration(_WaitDurationOnConnectionFailure.TotalSeconds, "Connection Exception")
                                _cts.Token.ThrowIfCancellationRequested()
                                'Since exception was internet related, no need to consume retries
                                retryCtr -= 1
                            Else
                                logger.Debug("HTTP->Exception without internet problem of unknown type detected:{0},{1}",
                                             ex.Message, command)
                                _cts.Token.ThrowIfCancellationRequested()
                                Waiter.SleepRequiredDuration(_WaitDurationOnAnyFailure.TotalSeconds, "Unknown Exception")
                                _cts.Token.ThrowIfCancellationRequested()
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
                    _cts.Token.ThrowIfCancellationRequested()
                    GC.Collect()
                Next
                RemoveHandler Waiter.Heartbeat, AddressOf OnHeartbeat
                RemoveHandler Waiter.WaitingFor, AddressOf OnWaitingFor
            End Using
            _cts.Token.ThrowIfCancellationRequested()
            If Not allOKWithoutException Then Throw lastException
            Return ret
        End Function

#Region "Login"
        Private Function GetErrorResponse(ByVal responseDict As Object) As String
            Dim ret As String = Nothing
            If responseDict IsNot Nothing AndAlso
               responseDict.GetType = GetType(Dictionary(Of String, Object)) AndAlso
               CType(responseDict, Dictionary(Of String, Object)).ContainsKey("status") AndAlso
               CType(responseDict, Dictionary(Of String, Object))("status") = "error" AndAlso
               CType(responseDict, Dictionary(Of String, Object)).ContainsKey("message") Then
                ret = String.Format("Zerodha reported error:{0}", CType(responseDict, Dictionary(Of String, Object))("message"))
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
            Await Task.Delay(0).ConfigureAwait(False)
            If _Kite Is Nothing Then
                _Kite = New Kite(_APIKey, Debug:=True)
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
                For Each runningInstrumentIdentifier In zerodhaSubscriber.SubcribedInstruments
                    _Ticker.Subscribe(Tokens:=New UInt32() {runningInstrumentIdentifier})
                    _Ticker.SetMode(Tokens:=New UInt32() {runningInstrumentIdentifier}, Mode:=Constants.MODE_FULL)
                Next
            End If
        End Function
#End Region

        Private Enum KiteCommands
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
            GetInstruments '
            InvalidateAccessToken
            None
        End Enum
    End Class
End Namespace
