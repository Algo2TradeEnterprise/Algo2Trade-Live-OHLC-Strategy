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
Imports Algo2TradeCore.Controller
Namespace Adapter
    Public Class ZerodhaAdapter
        Inherits APIAdapter

#Region "Logging and Status Progress"
        Public Shared Shadows logger As Logger = LogManager.GetCurrentClassLogger
#End Region

        Protected _Kite As Kite
        Protected _Ticker As Ticker
        Public Sub New(ByVal controller As ZerodhaStrategyController,
               ByVal canceller As CancellationTokenSource)
            MyBase.New(controller, canceller)
            _Kite = New Kite(APIKey:=CType(controller.APIConnection, ZerodhaConnection).ZerodhaUser.APIKey,
                             AccessToken:=CType(controller.APIConnection, ZerodhaConnection).ZerodhaAccessToken,
                             Debug:=True)
            _Kite.SetSessionExpiryHook(AddressOf controller.OnSessionExpireAsync)
        End Sub

        'Public Overrides Async Function GetAllInstrumentsAsync(Optional ByVal isRetryEnabled As Boolean = True) As Task(Of IEnumerable(Of IInstrument))
        '    Dim ret As List(Of ZerodhaInstrument) = Nothing
        '    Dim command As KiteCommands = KiteCommands.GetInstruments
        '    OnHeartbeat("Executing Zerodha command to fetch all instruments")

        '    Dim tempAllRet As Dictionary(Of String, Object) = Await ExecuteCommandAsync(command, Nothing, isRetryEnabled).ConfigureAwait(False)

        '    Dim tempRet As Object = Nothing
        '    If tempAllRet.ContainsKey(command.ToString) Then
        '        tempRet = tempAllRet(command.ToString)
        '        If tempRet IsNot Nothing Then
        '            Dim errorMessage As String = GetErrorResponse(tempRet)
        '            If errorMessage IsNot Nothing Then
        '                Throw New ApplicationException(errorMessage)
        '            End If
        '        Else
        '            Throw New ApplicationException(String.Format("No return fetched after executing command:{0}", command.ToString))
        '        End If
        '    Else
        '        Throw New ApplicationException(String.Format("Relevant command fired was not detected in the response:{0}", command.ToString))
        '    End If

        '    If tempRet.GetType = GetType(List(Of Instrument)) Then
        '        OnHeartbeat(String.Format("Creating Zerodha instrument collection from API instruments, count:{0}", tempRet.count))
        '        For Each runningInstrument As Instrument In CType(tempRet, List(Of Instrument))
        '            If ret Is Nothing Then ret = New List(Of ZerodhaInstrument)
        '            ret.Add(New ZerodhaInstrument(runningInstrument.InstrumentToken) With {.WrappedInstrument = runningInstrument})
        '        Next
        '    Else
        '        Throw New ApplicationException(String.Format("List of instruments not returned from command:{0}", command.ToString))
        '    End If
        '    Return ret
        'End Function
        'Public Function GetRandom(ByVal Min As Integer, ByVal Max As Integer) As Integer
        ' by making Generator static, we preserve the same instance '
        ' (i.e., do not create new instances with the same seed over and over) '
        ' between calls '
        'Static Generator As System.Random = New System.Random()
        '    Return Generator.Next(Min, Max)
        'End Function
        'Public Overrides Async Function TestAsync(ByVal str As String) As Task
        '    logger.Warn("Starting:{0}", str)
        '    While True
        '        Await Task.Delay(GetRandom(5000, 50000)).ConfigureAwait(False)
        '        Exit While
        '    End While
        '    logger.Warn("Ending:{0}", str)
        'End Function
        'Public Overrides Async Function GetAllTradesAsync(Optional ByVal tradeData As Dictionary(Of String, Object) = Nothing, Optional ByVal isRetryEnabled As Boolean = True) As Task(Of IEnumerable(Of ITrade))
        '    Dim ret As List(Of ZerodhaTrade) = Nothing
        '    Dim command As KiteCommands = KiteCommands.GetOrderTrades
        '    OnHeartbeat(String.Format("Executing Zerodha command to fetch all trades, xxx:{0}", tradeData("xxx")))
        '    Dim tempAllRet As Dictionary(Of String, Object) = Await ExecuteCommandAsync(command, Nothing, isRetryEnabled).ConfigureAwait(False)
        '    OnHeartbeat(String.Format("Executed Zerodha command to fetch all trades, xxx:{0}", tradeData("xxx")))

        '    Dim tempRet As Object = Nothing
        '    If tempAllRet IsNot Nothing AndAlso tempAllRet.ContainsKey(command.ToString) Then
        '        tempRet = tempAllRet(command.ToString)
        '        If tempRet IsNot Nothing Then
        '            Dim errorMessage As String = GetErrorResponse(tempRet)
        '            If errorMessage IsNot Nothing Then
        '                Throw New ApplicationException(errorMessage)
        '            End If
        '        Else
        '            Throw New ApplicationException(String.Format("No return fetched after executing command:{0}", command.ToString))
        '        End If
        '    Else
        '        Throw New ApplicationException(String.Format("Relevant command fired was not detected in the response:{0}", command.ToString))
        '    End If

        '    If tempRet.GetType = GetType(List(Of Trade)) Then
        '        OnHeartbeat(String.Format("Creating Zerodha trade collection from API trades, count:{0}", tempRet.count))
        '        For Each runningTrade As Trade In CType(tempRet, List(Of Trade))
        '            If ret Is Nothing Then ret = New List(Of ZerodhaTrade)
        '            ret.Add(New ZerodhaTrade With {.WrappedTrade = runningTrade})
        '        Next
        '    Else
        '        Throw New ApplicationException(String.Format("List of trades not returned from command:{0}", command.ToString))
        '    End If
        '    Return ret
        'End Function

        Public Async Function ExecuteCommandAsync(ByVal command As KiteCommands, ByVal stockData As Dictionary(Of String, Object), Optional ByVal isRetryEnabled As Boolean = True) As Task(Of Dictionary(Of String, Object))
            Dim ret As Dictionary(Of String, Object) = Nothing

            Dim lastException As Exception = Nothing
            Dim allOKWithoutException As Boolean = False


            Using Waiter As New Waiter(_cts)
                AddHandler Waiter.Heartbeat, AddressOf OnHeartbeat
                AddHandler Waiter.WaitingFor, AddressOf OnWaitingFor
                Dim totalTries As Integer = 1

                If isRetryEnabled Then totalTries = _MaxReTries
                For retryCtr = 1 To totalTries
                    _cts.Token.ThrowIfCancellationRequested()
                    While _controller.APIConnection Is Nothing
                        OnHeartbeat(String.Format("Waiting for new access token before executing:{0}...", command))
                        Await Task.Delay(1000).ConfigureAwait(False)
                    End While
                    lastException = Nothing
                    OnDocumentRetryStatus(retryCtr, totalTries)
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
                                    tradeList = Await Task.Factory.StartNew(Function()
                                                                                Try
                                                                                    _Kite.GetOrderTrades()
                                                                                Catch ex As Exception
                                                                                    lastException = ex
                                                                                End Try
                                                                            End Function).ConfigureAwait(False)
                                    ''tradeList = Await Task.Factory.StartNew(Function() _Kite.GetOrderTrades()).ContinueWith(Function(t)
                                    ''                                                                                            If t.Exception IsNot Nothing Then
                                    ''                                                                                                Throw t.Exception
                                    ''                                                                                            Else
                                    ''                                                                                                Return Nothing
                                    ''                                                                                            End If
                                    ''                                                                                        End Function, CancellationToken.None, TaskContinuationOptions.None, TaskScheduler.Current).ConfigureAwait(False)
                                    ''tradeList = Await Task.Run(Function() _Kite.GetOrderTrades(OrderId:=CType(stockData("OrderId"), String))).ConfigureAwait(False)
                                Else
                                    tradeList = Await Task.Factory.StartNew(Function()
                                                                                Try
                                                                                    Return _Kite.GetOrderTrades()
                                                                                Catch ex As Exception
                                                                                    lastException = ex
                                                                                End Try
                                                                            End Function).ConfigureAwait(False)
                                    'tradeList = Await Task.Run(Function() _Kite.GetOrderTrades()).ConfigureAwait(False)
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
                                Dim invalidateToken = _Kite.InvalidateAccessToken(CType(_controller.APIConnection, ZerodhaConnection).ZerodhaAccessToken)
                                lastException = Nothing
                                allOKWithoutException = True
                                Exit For
                            Case Else
                                Throw New ApplicationException("No Command Triggered")
                        End Select
                        If lastException IsNot Nothing Then
                            Throw lastException
                        End If
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

                        If isRetryEnabled Then retryCtr -= 1 : OnHeartbeat(String.Format("Error while executing Kite command, error:{0} command:{1}", lastException.Message, command.ToString))
                        'We will allow it to continue normal flow as the expiry is handled asynchronously via a seperate channel 
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
                                If isRetryEnabled Then retryCtr -= 1 : OnHeartbeat(String.Format("Error while executing Kite command, error:{0} command:{1}", lastException.Message, command.ToString))
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
                                If isRetryEnabled Then retryCtr -= 1 : OnHeartbeat(String.Format("Error while executing Kite command, error:{0} command:{1}", lastException.Message, command.ToString))
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
                                    If isRetryEnabled Then retryCtr -= 1 : OnHeartbeat(String.Format("Error while executing Kite command, error:{0} command:{1}", hex.Message, command.ToString))
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
                            If isRetryEnabled Then retryCtr -= 1 : OnHeartbeat(String.Format("Error while executing Kite command, error:{0} command:{1}", lastException.Message, command.ToString))
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
                                If isRetryEnabled Then retryCtr -= 1 : OnHeartbeat(String.Format("Error while executing Kite command, error:{0} command:{1}", lastException.Message, command.ToString))
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
                            If isRetryEnabled Then retryCtr -= 1 : OnHeartbeat(String.Format("Error while executing Kite command, error:{0} command:{1}", lastException.Message, command.ToString))
                        End If
                    Finally
                        OnDocumentDownloadComplete()
                    End Try
                    _cts.Token.ThrowIfCancellationRequested()
                    GC.Collect()
                    Exit For
                Next
                RemoveHandler Waiter.Heartbeat, AddressOf OnHeartbeat
                RemoveHandler Waiter.WaitingFor, AddressOf OnWaitingFor
            End Using
            _cts.Token.ThrowIfCancellationRequested()
            If Not allOKWithoutException AndAlso isRetryEnabled Then Throw lastException
            Return ret
        End Function

        'Public Overrides Async Function ConnectTickerAsync(ByVal subscriber As APIInstrumentSubscriber) As Task
        '    Await Task.Delay(0).ConfigureAwait(False)
        '    _Ticker = New Ticker(_ZerodhaConnection.APIKey, _ZerodhaConnection.APIUser.WrappedUser.AccessToken)
        '    Dim zerodhaSubscriber As ZerodhaInstrumentSubscriber = CType(subscriber, ZerodhaInstrumentSubscriber)
        '    AddHandler _Ticker.OnTick, AddressOf zerodhaSubscriber.OnTickerTickAsync
        '    AddHandler _Ticker.OnReconnect, AddressOf zerodhaSubscriber.OnTickerReconnect
        '    AddHandler _Ticker.OnNoReconnect, AddressOf zerodhaSubscriber.OnTickerNoReconnect
        '    AddHandler _Ticker.OnError, AddressOf zerodhaSubscriber.OnTickerError
        '    AddHandler _Ticker.OnClose, AddressOf zerodhaSubscriber.OnTickerClose
        '    AddHandler _Ticker.OnConnect, AddressOf zerodhaSubscriber.OnTickerConnect
        '    AddHandler _Ticker.OnOrderUpdate, AddressOf zerodhaSubscriber.OnTickerOrderUpdateAsync

        '    _Ticker.EnableReconnect(Interval:=5, Retries:=50)
        '    _Ticker.Connect()
        '    If zerodhaSubscriber.SubcribedInstruments IsNot Nothing AndAlso zerodhaSubscriber.SubcribedInstruments.Count > 0 Then
        '        For Each runningInstrumentIdentifier In zerodhaSubscriber.SubcribedInstruments
        '            logger.Debug("Subscribing instrument identfier:{0}", runningInstrumentIdentifier)
        '            _Ticker.Subscribe(Tokens:=New UInt32() {runningInstrumentIdentifier})
        '            _Ticker.SetMode(Tokens:=New UInt32() {runningInstrumentIdentifier}, Mode:=Constants.MODE_FULL)
        '        Next
        '    End If
        'End Function
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
            GetInstruments '
            InvalidateAccessToken
            None
        End Enum
    End Class
End Namespace
