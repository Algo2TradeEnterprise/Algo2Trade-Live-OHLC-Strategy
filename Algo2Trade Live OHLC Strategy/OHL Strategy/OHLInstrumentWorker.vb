Imports KiteConnect
Imports Utilities.Numbers
Imports Utilities.DAL
Imports System.Threading
Imports NLog
Imports Algo2TradeBLL

Public Class OHLInstrumentWorker
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

    Private _tickDetails As Concurrent.ConcurrentBag(Of Tick) = Nothing
    Private ReadOnly _instrumentToken As String
    Private ReadOnly _instrumentData As OHLStrategyTradableInstrument
    Private ReadOnly _zerodhaKite As ZerodhaKiteHelper
    Private ReadOnly _canceller As CancellationTokenSource
    Private _OHLStrategyProtector As Integer = 0
    Public Sub New(ByVal instrumentData As OHLStrategyTradableInstrument, ByVal zerodhaKite As ZerodhaKiteHelper, ByVal canceller As CancellationTokenSource)
        _instrumentToken = instrumentData.WrappedInstrument.InstrumentToken
        _instrumentData = instrumentData
        _zerodhaKite = zerodhaKite
        _canceller = canceller
    End Sub
    Public Async Function RunStrategyAsync() As Task
        Console.WriteLine(String.Format("Processing for:{0}", _instrumentData.WrappedInstrument.TradingSymbol))
        Await Task.Delay(100).ConfigureAwait(False)
        Dim currentTime As Date = Now
        Dim OHLStrategyTradeTime As TimeSpan = TimeSpan.Parse(My.Settings.OHLStrategyTradeTime)
        Dim lastTick As Tick = _instrumentData.WrappedTick
        If lastTick.Timestamp IsNot Nothing AndAlso _OHLStrategyProtector = 0 AndAlso
            currentTime.Hour = OHLStrategyTradeTime.Hours AndAlso currentTime.Minute = OHLStrategyTradeTime.Minutes AndAlso currentTime.Second >= OHLStrategyTradeTime.Seconds Then
            Dim OHLTradePrice As Decimal = lastTick.LastPrice
            Dim OHLQuantity As Integer = Common.CalculateQuantityFromInvestment(_instrumentData.InvestmentForStock, OHLTradePrice)
            If lastTick.Open = lastTick.High Then
                Interlocked.Increment(_OHLStrategyProtector)
                Dim tradeParameters As New Dictionary(Of String, Object)
                tradeParameters.Add("Exchange", Constants.EXCHANGE_NSE)
                tradeParameters.Add("TradingSymbol", _instrumentData.WrappedInstrument.TradingSymbol)
                tradeParameters.Add("TransactionType", Constants.TRANSACTION_TYPE_SELL)
                tradeParameters.Add("Quantity", OHLQuantity)
                tradeParameters.Add("Price", OHLTradePrice - 0.2)
                tradeParameters.Add("Product", Constants.PRODUCT_MIS)
                tradeParameters.Add("OrderType", Constants.ORDER_TYPE_LIMIT)
                tradeParameters.Add("Validity", Constants.VALIDITY_DAY)
                tradeParameters.Add("SquareOffValue", Math.Round(ConvertFloorCeling(OHLTradePrice * 0.005, Convert.ToDouble(_instrumentData.WrappedInstrument.TickSize), RoundOfType.Celing), 2))
                tradeParameters.Add("StoplossValue", Math.Abs(lastTick.High - OHLTradePrice))
                tradeParameters.Add("Variety", Constants.VARIETY_BO)
                tradeParameters.Add("Tag", "Algo2Trade_OHLStrategy")
                Await _zerodhaKite.ExecuteCommandAsync(ZerodhaKiteHelper.KiteCommands.PlaceOrder, tradeParameters).ConfigureAwait(False)
                'TODO: Process return of ExecuteCommand here
            ElseIf lastTick.Open = lastTick.Low Then
                Interlocked.Increment(_OHLStrategyProtector)
                Dim tradeParameters As New Dictionary(Of String, Object)
                tradeParameters.Add("Exchange", Constants.EXCHANGE_NSE)
                tradeParameters.Add("TradingSymbol", _instrumentData.WrappedInstrument.TradingSymbol)
                tradeParameters.Add("TransactionType", Constants.TRANSACTION_TYPE_BUY)
                tradeParameters.Add("Quantity", OHLQuantity)
                tradeParameters.Add("Price", OHLTradePrice + 0.2)
                tradeParameters.Add("Product", Constants.PRODUCT_MIS)
                tradeParameters.Add("OrderType", Constants.ORDER_TYPE_LIMIT)
                tradeParameters.Add("Validity", Constants.VALIDITY_DAY)
                tradeParameters.Add("SquareOffValue", Math.Round(ConvertFloorCeling(OHLTradePrice * 0.005, Convert.ToDouble(_instrumentData.WrappedInstrument.TickSize), RoundOfType.Celing), 2))
                tradeParameters.Add("StoplossValue", Math.Abs(OHLTradePrice - lastTick.Low))
                tradeParameters.Add("Variety", Constants.VARIETY_BO)
                tradeParameters.Add("Tag", "Algo2Trade_OHLStrategy")
                Await _zerodhaKite.ExecuteCommandAsync(ZerodhaKiteHelper.KiteCommands.PlaceOrder, tradeParameters).ConfigureAwait(False)
                'TODO: Process return of ExecuteCommand here
            End If
        Else
            logger.Debug("Time for taking trade has not come, current time:{0}", Now.ToString("yyyy-MM-dd HH:mm:ss"))
        End If
    End Function
    Public Async Function ConsumedTickDataAsync(ByVal tickData As Tick) As Task
        Await Task.Delay(1).ConfigureAwait(False)
        _canceller.Token.ThrowIfCancellationRequested()
        If _tickDetails Is Nothing Then _tickDetails = New Concurrent.ConcurrentBag(Of Tick)
        _tickDetails.Add(tickData)
        _instrumentData.WrappedTick = tickData
    End Function
    Public Async Function ConsumedOrderUpdateAsync(ByVal orderData As Order) As Task
        Await Task.Delay(1).ConfigureAwait(False)
        _canceller.Token.ThrowIfCancellationRequested()
        Dim ret As Dictionary(Of String, Object) = Await _zerodhaKite.ExecuteCommandAsync(ZerodhaKiteHelper.KiteCommands.GetOrderTrades, Nothing).ConfigureAwait(False)
        If ret IsNot Nothing AndAlso ret.ContainsKey(ZerodhaKiteHelper.KiteCommands.GetOrderTrades.ToString) Then
            Dim allTradeList As List(Of Trade) = ret(ZerodhaKiteHelper.KiteCommands.GetOrderTrades.ToString)
            If allTradeList IsNot Nothing AndAlso allTradeList.Count > 0 Then
                _instrumentData.TradeList = allTradeList.FindAll(Function(x)
                                                                     Return x.InstrumentToken = _instrumentToken
                                                                 End Function)
            End If
        End If
    End Function
End Class
