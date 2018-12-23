Imports System.Threading
Imports Algo2TradeCore.Entities
Imports KiteConnect
Imports NLog
Imports Algo2TradeCore.Controller
Imports System.Text.RegularExpressions

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
                             AccessToken:=CType(controller.APIConnection, ZerodhaConnection).AccessToken,
                             Debug:=False)
            _Kite.SetSessionExpiryHook(AddressOf controller.OnSessionExpireAsync)
        End Sub
        Public Overrides Sub SetAPIAccessToken(ByVal apiAccessToken As String)
            _Kite.SetAccessToken(apiAccessToken)
        End Sub
        Public Overrides Async Function GetAllInstrumentsAsync() As Task(Of IEnumerable(Of IInstrument))
            logger.Debug("GetAllInstrumentsAsync, Parameters:Nothing")
            Dim ret As List(Of ZerodhaInstrument) = Nothing
            Dim command As KiteCommands = KiteCommands.GetInstruments

            Dim tempAllRet As Dictionary(Of String, Object) = Await ExecuteCommandAsync(command, Nothing).ConfigureAwait(False)

            Dim tempRet As Object = Nothing
            If tempAllRet IsNot Nothing AndAlso tempAllRet.ContainsKey(command.ToString) Then
                tempRet = tempAllRet(command.ToString)
                If tempRet IsNot Nothing Then
                    Dim errorMessage As String = _controller.GetErrorResponse(tempRet)
                    If errorMessage IsNot Nothing Then
                        Throw New ApplicationException(errorMessage)
                    End If
                Else
                    Throw New ApplicationException(String.Format("Zerodha command execution did not return anything, command:{0}", command.ToString))
                End If
            Else
                Throw New ApplicationException(String.Format("Relevant command was fired but not detected in the response, command:{0}", command.ToString))
            End If

            If tempRet.GetType = GetType(List(Of Instrument)) Then
                OnHeartbeat(String.Format("Creating Zerodha instrument collection from API instruments, count:{0}", tempRet.count))
                Dim zerodhaReturedInstruments As List(Of Instrument) = CType(tempRet, List(Of Instrument))
                For Each runningInstrument As Instrument In zerodhaReturedInstruments
                    If ret Is Nothing Then ret = New List(Of ZerodhaInstrument)
                    ret.Add(New ZerodhaInstrument(runningInstrument.InstrumentToken) With {.WrappedInstrument = runningInstrument})
                    'If runningInstrument.InstrumentType = "FUT" AndAlso runningInstrument.Exchange = "NFO" Then
                    '    Dim coreInstrumentName As String = Regex.Replace(runningInstrument.TradingSymbol, "[0-9]+[A-Z]+FUT", "")
                    '    If coreInstrumentName IsNot Nothing Then
                    '        Dim cashInstrumentToAdd = zerodhaReturedInstruments.Where(Function(x)
                    '                                                                      Return x.TradingSymbol = coreInstrumentName
                    '                                                                  End Function).FirstOrDefault
                    '        If cashInstrumentToAdd.TradingSymbol IsNot Nothing AndAlso ret.Find(Function(x)
                    '                                                                                Return x.InstrumentIdentifier = cashInstrumentToAdd.InstrumentToken
                    '                                                                            End Function) Is Nothing Then
                    '            ret.Add(New ZerodhaInstrument(cashInstrumentToAdd.InstrumentToken) With {.WrappedInstrument = cashInstrumentToAdd})
                    '        End If
                    '    End If
                    'End If
                Next
            Else
                Throw New ApplicationException(String.Format("Zerodha command execution did not return any list of instrument, command:{0}", command.ToString))
            End If
            Return ret
        End Function
        Public Overrides Async Function GetAllTradesAsync() As Task(Of IEnumerable(Of ITrade))
            logger.Debug("GetAllTradesAsync, Parameters:Nothing")
            Dim ret As List(Of ZerodhaTrade) = Nothing
            Dim command As KiteCommands = KiteCommands.GetOrderTrades
            Dim tempAllRet As Dictionary(Of String, Object) = Await ExecuteCommandAsync(command, Nothing).ConfigureAwait(False)

            Dim tempRet As Object = Nothing
            If tempAllRet IsNot Nothing AndAlso tempAllRet.ContainsKey(command.ToString) Then
                tempRet = tempAllRet(command.ToString)
                If tempRet IsNot Nothing Then
                    Dim errorMessage As String = _controller.GetErrorResponse(tempRet)
                    If errorMessage IsNot Nothing Then
                        Throw New ApplicationException(errorMessage)
                    End If
                Else
                    Throw New ApplicationException(String.Format("Zerodha command execution did not return anything, command:{0}", command.ToString))
                End If
            Else
                Throw New ApplicationException(String.Format("Relevant command was fired but not detected in the response, command:{0}", command.ToString))
            End If

            If tempRet.GetType = GetType(List(Of Trade)) Then
                OnHeartbeat(String.Format("Creating Zerodha trade collection from API trades, count:{0}", tempRet.count))
                Dim zerodhaReturedTrades As List(Of Trade) = CType(tempRet, List(Of Trade))
                For Each runningTrade As Trade In zerodhaReturedTrades
                    If ret Is Nothing Then ret = New List(Of ZerodhaTrade)
                    ret.Add(New ZerodhaTrade With {.WrappedTrade = runningTrade})
                Next
            Else
                Throw New ApplicationException(String.Format("Zerodha command execution did not return any list of trade, command:{0}", command.ToString))
            End If
            Return ret
        End Function

        Private Async Function ExecuteCommandAsync(ByVal command As KiteCommands, ByVal stockData As Dictionary(Of String, Object)) As Task(Of Dictionary(Of String, Object))
            logger.Debug("ExecuteCommandAsync, command:{0}, stockData:{1}", command.ToString, Utils.JsonSerialize(stockData))
            Dim ret As Dictionary(Of String, Object) = Nothing

            Dim lastException As Exception = Nothing
            OnHeartbeat(String.Format("Firing Zerodha command to complete desired action, command:{0}", command.ToString))
            Select Case command
                Case KiteCommands.GetPositions
                    Dim positions As PositionResponse = Nothing
                    positions = Await Task.Factory.StartNew(Function()
                                                                Try
                                                                    Return _Kite.GetPositions()
                                                                Catch ex As Exception
                                                                    logger.Error(ex)
                                                                    lastException = ex
                                                                End Try
                                                            End Function).ConfigureAwait(False)
                    ret = New Dictionary(Of String, Object) From {{command.ToString, positions}}
                Case KiteCommands.PlaceOrder
                    Dim placedOrders As Dictionary(Of String, Object) = Nothing
                    If stockData IsNot Nothing AndAlso stockData.Count > 0 Then
                        placedOrders = Await Task.Factory.StartNew(Function()
                                                                       Try
                                                                           Return _Kite.PlaceOrder(Exchange:=CType(stockData("Exchange"), String),
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
                                                                       Catch ex As Exception
                                                                           logger.Error(ex)
                                                                           lastException = ex
                                                                       End Try
                                                                   End Function).ConfigureAwait(False)
                    End If
                    ret = New Dictionary(Of String, Object) From {{command.ToString, placedOrders}}
                Case KiteCommands.ModifyOrderQuantity
                    Dim modifiedOrdersQuantity As Dictionary(Of String, Object) = Nothing
                    If stockData IsNot Nothing AndAlso stockData.Count > 0 Then
                        modifiedOrdersQuantity = Await Task.Factory.StartNew(Function()
                                                                                 Try
                                                                                     Return _Kite.ModifyOrder(OrderId:=CType(stockData("OrderId"), String),
                                                                                                              Quantity:=CType(stockData("Quantity"), String))
                                                                                 Catch ex As Exception
                                                                                     logger.Error(ex)
                                                                                     lastException = ex
                                                                                 End Try
                                                                             End Function).ConfigureAwait(False)
                    End If
                    ret = New Dictionary(Of String, Object) From {{command.ToString, modifiedOrdersQuantity}}
                Case KiteCommands.ModifyTargetOrderPrice, KiteCommands.ModifyOrderPrice
                    Dim modifiedOrdersPrice As Dictionary(Of String, Object) = Nothing
                    If stockData IsNot Nothing AndAlso stockData.Count > 0 Then
                        modifiedOrdersPrice = Await Task.Factory.StartNew(Function()
                                                                              Try
                                                                                  Return _Kite.ModifyOrder(OrderId:=CType(stockData("OrderId"), String),
                                                                                                           Price:=CType(stockData("Price"), Decimal))
                                                                              Catch ex As Exception
                                                                                  logger.Error(ex)
                                                                                  lastException = ex
                                                                              End Try
                                                                          End Function).ConfigureAwait(False)
                    End If
                    ret = New Dictionary(Of String, Object) From {{command.ToString, modifiedOrdersPrice}}
                Case KiteCommands.ModifySLOrderPrice
                    Dim modifiedOrdersPrice As Dictionary(Of String, Object) = Nothing
                    If stockData IsNot Nothing AndAlso stockData.Count > 0 Then
                        modifiedOrdersPrice = Await Task.Factory.StartNew(Function()
                                                                              Try
                                                                                  Return _Kite.ModifyOrder(OrderId:=CType(stockData("OrderId"), String),
                                                                                                           TriggerPrice:=CType(stockData("TriggerPrice"), Decimal))
                                                                              Catch ex As Exception
                                                                                  logger.Error(ex)
                                                                                  lastException = ex
                                                                              End Try
                                                                          End Function).ConfigureAwait(False)
                    End If
                    ret = New Dictionary(Of String, Object) From {{command.ToString, modifiedOrdersPrice}}
                Case KiteCommands.CancelOrder
                    Dim cancelledOrder As Dictionary(Of String, Object) = Nothing
                    If stockData IsNot Nothing AndAlso stockData.Count > 0 Then
                        cancelledOrder = Await Task.Factory.StartNew(Function()
                                                                         Try
                                                                             Return _Kite.CancelOrder(OrderId:=CType(stockData("OrderId"), String),
                                                                                               ParentOrderId:=CType(stockData("ParentOrderId"), String),
                                                                                               Variety:=CType(stockData("Variety"), String))
                                                                         Catch ex As Exception
                                                                             logger.Error(ex)
                                                                             lastException = ex
                                                                         End Try
                                                                     End Function).ConfigureAwait(False)
                    End If
                    ret = New Dictionary(Of String, Object) From {{command.ToString, cancelledOrder}}
                Case KiteCommands.GetOrderHistory
                    Dim orderList As List(Of Order) = Nothing
                    If stockData IsNot Nothing AndAlso stockData.Count > 0 Then
                        orderList = Await Task.Factory.StartNew(Function()
                                                                    Try
                                                                        Return _Kite.GetOrderHistory(OrderId:=CType(stockData("OrderId"), String))
                                                                    Catch ex As Exception
                                                                        logger.Error(ex)
                                                                        lastException = ex
                                                                    End Try
                                                                End Function).ConfigureAwait(False)
                    End If
                    ret = New Dictionary(Of String, Object) From {{command.ToString, orderList}}
                Case KiteCommands.GetOrders
                    Dim orderList As List(Of Order) = Nothing
                    orderList = Await Task.Factory.StartNew(Function()
                                                                Try
                                                                    Return _Kite.GetOrders()
                                                                Catch ex As Exception
                                                                    logger.Error(ex)
                                                                    lastException = ex
                                                                End Try
                                                            End Function).ConfigureAwait(False)
                    ret = New Dictionary(Of String, Object) From {{command.ToString, orderList}}
                Case KiteCommands.GetOrderTrades
                    Dim tradeList As List(Of Trade) = Nothing
                    If stockData IsNot Nothing AndAlso stockData.Count > 0 Then
                        tradeList = Await Task.Factory.StartNew(Function()
                                                                    Try
                                                                        Return _Kite.GetOrderTrades(OrderId:=CType(stockData("OrderId"), String))
                                                                    Catch ex As Exception
                                                                        logger.Error(ex)
                                                                        lastException = ex
                                                                    End Try
                                                                End Function).ConfigureAwait(False)
                    Else
                        tradeList = Await Task.Factory.StartNew(Function()
                                                                    Try
                                                                        Return _Kite.GetOrderTrades()
                                                                    Catch ex As Exception
                                                                        logger.Error(ex)
                                                                        lastException = ex
                                                                    End Try
                                                                End Function).ConfigureAwait(False)
                    End If
                    ret = New Dictionary(Of String, Object) From {{command.ToString, tradeList}}
                Case KiteCommands.GetInstruments
                    Dim instruments As List(Of Instrument) = Nothing
                    instruments = Await Task.Factory.StartNew(Function()
                                                                  Try
                                                                      Return _Kite.GetInstruments()
                                                                  Catch ex As Exception
                                                                      logger.Error(ex)
                                                                      lastException = ex
                                                                  End Try
                                                              End Function).ConfigureAwait(False)
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
                    Dim invalidateToken = _Kite.InvalidateAccessToken(CType(_controller.APIConnection, ZerodhaConnection).AccessToken)
                    lastException = Nothing
                Case Else
                    Throw New ApplicationException("No Command Triggered")
            End Select
            If lastException IsNot Nothing Then
                Throw lastException
            End If
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
            GenerateSession
            None
        End Enum
    End Class
End Namespace
