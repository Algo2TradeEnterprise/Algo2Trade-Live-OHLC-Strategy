﻿Imports System.Threading
Imports Algo2TradeCore.Entities
Imports KiteConnect
Imports NLog
Imports Algo2TradeCore.Controller

Namespace Adapter
    Public Class ZerodhaAdapter
        Inherits APIAdapter

#Region "Logging and Status Progress"
        Public Shared Shadows logger As Logger = LogManager.GetCurrentClassLogger
#End Region

        Protected _Kite As Kite
        Public Sub New(ByVal associatedParentController As ZerodhaStrategyController,
               ByVal canceller As CancellationTokenSource)
            MyBase.New(associatedParentController, canceller)
            _Kite = New Kite(APIKey:=CType(associatedParentController.APIConnection, ZerodhaConnection).ZerodhaUser.APIKey,
                             AccessToken:=CType(associatedParentController.APIConnection, ZerodhaConnection).AccessToken,
                             Debug:=False)
            _Kite.SetSessionExpiryHook(AddressOf associatedParentController.OnSessionExpireAsync)
        End Sub
        Public Overrides Sub SetAPIAccessToken(ByVal apiAccessToken As String)
            logger.Debug("SetAPIAccessToken, apiAccessToken:{0}", apiAccessToken)
            _Kite.SetAccessToken(apiAccessToken)
        End Sub
        Public Overrides Async Function GetAllInstrumentsAsync() As Task(Of IEnumerable(Of IInstrument))
            logger.Debug("GetAllInstrumentsAsync, parameters:Nothing")
            Dim ret As List(Of ZerodhaInstrument) = Nothing
            Dim execCommand As ExecutionCommands = ExecutionCommands.GetInstruments

            _cts.Token.ThrowIfCancellationRequested()
            Dim tempAllRet As Dictionary(Of String, Object) = Await ExecuteCommandAsync(execCommand, Nothing).ConfigureAwait(False)
            _cts.Token.ThrowIfCancellationRequested()

            Dim tempRet As Object = Nothing
            If tempAllRet IsNot Nothing AndAlso tempAllRet.ContainsKey(execCommand.ToString) Then
                tempRet = tempAllRet(execCommand.ToString)
                If tempRet IsNot Nothing Then
                    Dim errorMessage As String = ParentController.GetErrorResponse(tempRet)
                    If errorMessage IsNot Nothing Then
                        Throw New ApplicationException(errorMessage)
                    End If
                Else
                    Throw New ApplicationException(String.Format("Zerodha command execution did not return anything, command:{0}", execCommand.ToString))
                End If
            Else
                Throw New ApplicationException(String.Format("Relevant command was fired but not detected in the response, command:{0}", execCommand.ToString))
            End If

            If tempRet.GetType = GetType(List(Of Instrument)) Then
                OnHeartbeat(String.Format("Creating Zerodha instrument collection from API instruments, count:{0}", tempRet.count))
                Dim zerodhaReturedInstruments As List(Of Instrument) = CType(tempRet, List(Of Instrument))
                For Each runningInstrument As Instrument In zerodhaReturedInstruments
                    _cts.Token.ThrowIfCancellationRequested()
                    If ret Is Nothing Then ret = New List(Of ZerodhaInstrument)
                    ret.Add(New ZerodhaInstrument(runningInstrument.InstrumentToken) With {.WrappedInstrument = runningInstrument})
                Next
            Else
                Throw New ApplicationException(String.Format("Zerodha command execution did not return any list of instrument, command:{0}", execCommand.ToString))
            End If
            Return ret
        End Function
        Public Overrides Async Function GetAllQuotes(ByVal instruments As IEnumerable(Of IInstrument)) As Task(Of IEnumerable(Of IQuote))
            logger.Debug("GetAllQuotes, instruments:{0}", Utils.JsonSerialize(instruments))
            Dim ret As List(Of ZerodhaQuote) = Nothing
            Dim execCommand As ExecutionCommands = ExecutionCommands.GetQuotes

            _cts.Token.ThrowIfCancellationRequested()
            Dim tempAllRet As Dictionary(Of String, Object) = Await ExecuteCommandAsync(execCommand, New Dictionary(Of String, Object) From {{"instruments", instruments}}).ConfigureAwait(False)
            _cts.Token.ThrowIfCancellationRequested()

            Dim tempRet As Object = Nothing
            If tempAllRet IsNot Nothing AndAlso tempAllRet.ContainsKey(execCommand.ToString) Then
                tempRet = tempAllRet(execCommand.ToString)
                If tempRet IsNot Nothing Then
                    Dim errorMessage As String = ParentController.GetErrorResponse(tempRet)
                    If errorMessage IsNot Nothing Then
                        Throw New ApplicationException(errorMessage)
                    End If
                Else
                    Throw New ApplicationException(String.Format("Zerodha command execution did not return anything, command:{0}", execCommand.ToString))
                End If
            Else
                Throw New ApplicationException(String.Format("Relevant command was fired but not detected in the response, command:{0}", execCommand.ToString))
            End If

            If tempRet.GetType = GetType(Dictionary(Of String, Quote)) Then
                OnHeartbeat(String.Format("Creating Zerodha quote collection from API quotes, count:{0}", tempRet.count))
                Dim zerodhaReturedQuotes As Dictionary(Of String, Quote) = CType(tempRet, Dictionary(Of String, Quote))
                For Each runningQuote In zerodhaReturedQuotes
                    _cts.Token.ThrowIfCancellationRequested()
                    If ret Is Nothing Then ret = New List(Of ZerodhaQuote)
                    ret.Add(New ZerodhaQuote() With {.WrappedQuote = runningQuote.Value})
                Next
            Else
                Throw New ApplicationException(String.Format("Zerodha command execution did not return any list of quotes, command:{0}", execCommand.ToString))
            End If
            Return ret
        End Function
        Public Overrides Async Function GetAllTradesAsync() As Task(Of IEnumerable(Of ITrade))
            logger.Debug("GetAllTradesAsync, parameters:Nothing")
            Dim ret As List(Of ZerodhaTrade) = Nothing
            Dim execCommand As ExecutionCommands = ExecutionCommands.GetOrderTrades
            _cts.Token.ThrowIfCancellationRequested()
            Dim tempAllRet As Dictionary(Of String, Object) = Await ExecuteCommandAsync(execCommand, Nothing).ConfigureAwait(False)
            _cts.Token.ThrowIfCancellationRequested()

            Dim tempRet As Object = Nothing
            If tempAllRet IsNot Nothing AndAlso tempAllRet.ContainsKey(execCommand.ToString) Then
                tempRet = tempAllRet(execCommand.ToString)
                If tempRet IsNot Nothing Then
                    Dim errorMessage As String = ParentController.GetErrorResponse(tempRet)
                    If errorMessage IsNot Nothing Then
                        Throw New ApplicationException(errorMessage)
                    End If
                Else
                    Throw New ApplicationException(String.Format("Zerodha command execution did not return anything, command:{0}", execCommand.ToString))
                End If
            Else
                Throw New ApplicationException(String.Format("Relevant command was fired but not detected in the response, command:{0}", execCommand.ToString))
            End If

            If tempRet.GetType = GetType(List(Of Trade)) Then
                OnHeartbeat(String.Format("Creating Zerodha trade collection from API trades, count:{0}", tempRet.count))
                Dim zerodhaReturedTrades As List(Of Trade) = CType(tempRet, List(Of Trade))
                For Each runningTrade As Trade In zerodhaReturedTrades
                    _cts.Token.ThrowIfCancellationRequested()
                    If ret Is Nothing Then ret = New List(Of ZerodhaTrade)
                    ret.Add(New ZerodhaTrade With {.WrappedTrade = runningTrade})
                Next
            Else
                Throw New ApplicationException(String.Format("Zerodha command execution did not return any list of trade, command:{0}", execCommand.ToString))
            End If
            Return ret
        End Function

        Private Async Function ExecuteCommandAsync(ByVal command As ExecutionCommands, ByVal stockData As Dictionary(Of String, Object)) As Task(Of Dictionary(Of String, Object))
            logger.Debug("ExecuteCommandAsync, command:{0}, stockData:{1}", command.ToString, Utils.JsonSerialize(stockData))
            _cts.Token.ThrowIfCancellationRequested()
            Dim ret As Dictionary(Of String, Object) = Nothing

            Dim lastException As Exception = Nothing
            OnHeartbeat(String.Format("Firing Zerodha command to complete desired action, command:{0}", command.ToString))
            Select Case command
                Case ExecutionCommands.GetQuotes
                    Dim getQuotesResponse As Dictionary(Of String, Quote) = Nothing
                    If stockData IsNot Nothing AndAlso stockData.ContainsKey("instruments") Then
                        Dim index As Integer = -1
                        Dim subscriptionList() As String = Nothing
                        For Each runningInstruments As IInstrument In stockData("instruments")
                            _cts.Token.ThrowIfCancellationRequested()
                            index += 1
                            If index = 0 Then
                                ReDim subscriptionList(0)
                            Else
                                ReDim Preserve subscriptionList(UBound(subscriptionList) + 1)
                            End If
                            subscriptionList(index) = runningInstruments.InstrumentIdentifier
                        Next
                        If subscriptionList IsNot Nothing AndAlso subscriptionList.Length > 0 Then

                            getQuotesResponse = Await Task.Factory.StartNew(Function()
                                                                                Try
                                                                                    Return _Kite.GetQuote(subscriptionList)
                                                                                Catch ex As Exception
                                                                                    logger.Error(ex)
                                                                                    lastException = ex
                                                                                    Return Nothing
                                                                                End Try
                                                                            End Function).ConfigureAwait(False)
                            _cts.Token.ThrowIfCancellationRequested()
                            ret = New Dictionary(Of String, Object) From {{command.ToString, getQuotesResponse}}
                        End If
                    End If
                Case ExecutionCommands.GetPositions
                    Dim positions As PositionResponse = Nothing
                    positions = Await Task.Factory.StartNew(Function()
                                                                Try
                                                                    Return _Kite.GetPositions()
                                                                Catch ex As Exception
                                                                    logger.Error(ex)
                                                                    lastException = ex
                                                                    Return Nothing
                                                                End Try
                                                            End Function).ConfigureAwait(False)
                    _cts.Token.ThrowIfCancellationRequested()
                    ret = New Dictionary(Of String, Object) From {{command.ToString, positions}}
                Case ExecutionCommands.PlaceOrder
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
                                                                           Return Nothing
                                                                       End Try
                                                                   End Function).ConfigureAwait(False)
                        _cts.Token.ThrowIfCancellationRequested()
                    End If
                    ret = New Dictionary(Of String, Object) From {{command.ToString, placedOrders}}
                Case ExecutionCommands.ModifyOrderQuantity
                    Dim modifiedOrdersQuantity As Dictionary(Of String, Object) = Nothing
                    If stockData IsNot Nothing AndAlso stockData.Count > 0 Then
                        modifiedOrdersQuantity = Await Task.Factory.StartNew(Function()
                                                                                 Try
                                                                                     Return _Kite.ModifyOrder(OrderId:=CType(stockData("OrderId"), String),
                                                                                                              Quantity:=CType(stockData("Quantity"), String))
                                                                                 Catch ex As Exception
                                                                                     logger.Error(ex)
                                                                                     lastException = ex
                                                                                     Return Nothing
                                                                                 End Try
                                                                             End Function).ConfigureAwait(False)
                        _cts.Token.ThrowIfCancellationRequested()
                    End If
                    ret = New Dictionary(Of String, Object) From {{command.ToString, modifiedOrdersQuantity}}
                Case ExecutionCommands.ModifyTargetOrderPrice, ExecutionCommands.ModifyOrderPrice
                    Dim modifiedOrdersPrice As Dictionary(Of String, Object) = Nothing
                    If stockData IsNot Nothing AndAlso stockData.Count > 0 Then
                        modifiedOrdersPrice = Await Task.Factory.StartNew(Function()
                                                                              Try
                                                                                  Return _Kite.ModifyOrder(OrderId:=CType(stockData("OrderId"), String),
                                                                                                           Price:=CType(stockData("Price"), Decimal))
                                                                              Catch ex As Exception
                                                                                  logger.Error(ex)
                                                                                  lastException = ex
                                                                                  Return Nothing
                                                                              End Try
                                                                          End Function).ConfigureAwait(False)
                        _cts.Token.ThrowIfCancellationRequested()
                    End If
                    ret = New Dictionary(Of String, Object) From {{command.ToString, modifiedOrdersPrice}}
                Case ExecutionCommands.ModifySLOrderPrice
                    Dim modifiedOrdersPrice As Dictionary(Of String, Object) = Nothing
                    If stockData IsNot Nothing AndAlso stockData.Count > 0 Then
                        modifiedOrdersPrice = Await Task.Factory.StartNew(Function()
                                                                              Try
                                                                                  Return _Kite.ModifyOrder(OrderId:=CType(stockData("OrderId"), String),
                                                                                                           TriggerPrice:=CType(stockData("TriggerPrice"), Decimal))
                                                                              Catch ex As Exception
                                                                                  logger.Error(ex)
                                                                                  lastException = ex
                                                                                  Return Nothing
                                                                              End Try
                                                                          End Function).ConfigureAwait(False)
                        _cts.Token.ThrowIfCancellationRequested()
                    End If
                    ret = New Dictionary(Of String, Object) From {{command.ToString, modifiedOrdersPrice}}
                Case ExecutionCommands.CancelOrder
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
                                                                             Return Nothing
                                                                         End Try
                                                                     End Function).ConfigureAwait(False)
                        _cts.Token.ThrowIfCancellationRequested()
                    End If
                    ret = New Dictionary(Of String, Object) From {{command.ToString, cancelledOrder}}
                Case ExecutionCommands.GetOrderHistory
                    Dim orderList As List(Of Order) = Nothing
                    If stockData IsNot Nothing AndAlso stockData.Count > 0 Then
                        orderList = Await Task.Factory.StartNew(Function()
                                                                    Try
                                                                        Return _Kite.GetOrderHistory(OrderId:=CType(stockData("OrderId"), String))
                                                                    Catch ex As Exception
                                                                        logger.Error(ex)
                                                                        lastException = ex
                                                                        Return Nothing
                                                                    End Try
                                                                End Function).ConfigureAwait(False)
                        _cts.Token.ThrowIfCancellationRequested()
                    End If
                    ret = New Dictionary(Of String, Object) From {{command.ToString, orderList}}
                Case ExecutionCommands.GetOrders
                    Dim orderList As List(Of Order) = Nothing
                    orderList = Await Task.Factory.StartNew(Function()
                                                                Try
                                                                    Return _Kite.GetOrders()
                                                                Catch ex As Exception
                                                                    logger.Error(ex)
                                                                    lastException = ex
                                                                    Return Nothing
                                                                End Try
                                                            End Function).ConfigureAwait(False)
                    _cts.Token.ThrowIfCancellationRequested()
                    ret = New Dictionary(Of String, Object) From {{command.ToString, orderList}}
                Case ExecutionCommands.GetOrderTrades
                    Dim tradeList As List(Of Trade) = Nothing
                    If stockData IsNot Nothing AndAlso stockData.Count > 0 Then
                        tradeList = Await Task.Factory.StartNew(Function()
                                                                    Try
                                                                        Return _Kite.GetOrderTrades(OrderId:=CType(stockData("OrderId"), String))
                                                                    Catch ex As Exception
                                                                        logger.Error(ex)
                                                                        lastException = ex
                                                                        Return Nothing
                                                                    End Try
                                                                End Function).ConfigureAwait(False)
                        _cts.Token.ThrowIfCancellationRequested()
                    Else
                        tradeList = Await Task.Factory.StartNew(Function()
                                                                    Try
                                                                        Return _Kite.GetOrderTrades()
                                                                    Catch ex As Exception
                                                                        logger.Error(ex)
                                                                        lastException = ex
                                                                        Return Nothing
                                                                    End Try
                                                                End Function).ConfigureAwait(False)
                        _cts.Token.ThrowIfCancellationRequested()
                    End If
                    ret = New Dictionary(Of String, Object) From {{command.ToString, tradeList}}
                Case ExecutionCommands.GetInstruments
                    Dim instruments As List(Of Instrument) = Nothing
                    instruments = Await Task.Factory.StartNew(Function()
                                                                  Try
                                                                      Return _Kite.GetInstruments()
                                                                  Catch ex As Exception
                                                                      logger.Error(ex)
                                                                      lastException = ex
                                                                      Return Nothing
                                                                  End Try
                                                              End Function).ConfigureAwait(False)
                    _cts.Token.ThrowIfCancellationRequested()
                    Dim count As Integer = If(instruments Is Nothing, 0, instruments.Count)
                    logger.Debug(String.Format("Fetched {0} instruments from Zerodha", count))
                    If instruments IsNot Nothing AndAlso instruments.Count > 0 Then
                        instruments.RemoveAll(Function(x) x.Exchange = "BFO" Or x.Exchange = "BSE")
                        instruments.RemoveAll(Function(x) x.Segment.EndsWith("OPT"))
                        instruments.RemoveAll(Function(x) x.TradingSymbol.Length > 3 AndAlso x.TradingSymbol.Substring(x.TradingSymbol.Length - 3).StartsWith("-"))
                        count = If(instruments Is Nothing, 0, instruments.Count)
                        logger.Debug(String.Format("After cleanup, fetched {0} instruments from Zerodha", count))
                    End If
                    _cts.Token.ThrowIfCancellationRequested()
                    ret = New Dictionary(Of String, Object) From {{command.ToString, instruments}}
                Case ExecutionCommands.InvalidateAccessToken
                    Dim invalidateToken = _Kite.InvalidateAccessToken(CType(ParentController.APIConnection, ZerodhaConnection).AccessToken)
                    lastException = Nothing
                    _cts.Token.ThrowIfCancellationRequested()
                Case Else
                    Throw New ApplicationException("No Command Triggered")
            End Select
            If lastException IsNot Nothing Then
                Throw lastException
            End If
            Return ret
        End Function
    End Class
End Namespace
