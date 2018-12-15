Imports System.Threading
Imports Algo2TradeCore.Adapter
Imports Algo2TradeCore.Entity
Imports KiteConnect
Imports NLog

Namespace Subscriber

    Public Class ZerodhaInstrumentSubscriber
        Inherits APIInstrumentSubscriber

#Region "Logging and Status Progress"
        Public Shared logger As Logger = LogManager.GetCurrentClassLogger
#End Region
        Public Sub OnConnect()
            'OnHeartbeat("Connected Ticker")
        End Sub
        Public Sub OnClose()
            'OnHeartbeat("Closed Ticker")
        End Sub
        Public Sub OnError(message As String)
            'OnHeartbeat(String.Format("Error: {0}", message))
        End Sub
        Public Sub OnNoReconnect()
            'OnHeartbeat("Not Reconnecting")
        End Sub
        Public Sub OnReconnect()
            'OnHeartbeat("Reconnecting")
        End Sub
        Public Async Sub OnTickAsync(ByVal tickData As Tick)
            _cts.Token.ThrowIfCancellationRequested()
            Await Task.Delay(0).ConfigureAwait(False)
            If _subscribedStrategyInstruments IsNot Nothing AndAlso _subscribedStrategyInstruments.Count > 0 Then
                Dim runningTick As New ZerodhaTick() With {.WrappedTick = tickData}
                Parallel.ForEach(
                        _subscribedStrategyInstruments(tickData.InstrumentToken),
                        Sub(runningStrategyInstrument)
                            runningStrategyInstrument.ProcessTickAsync(runningTick)
                        End Sub
                    )
            End If
        End Sub
        Public Async Sub OnOrderUpdateAsync(orderData As Order)
            Await Task.Delay(0).ConfigureAwait(False)
            'If _todaysInstrumentsForOHLStrategy IsNot Nothing AndAlso _todaysInstrumentsForOHLStrategy.Count > 0 Then
            '    _todaysInstrumentsForOHLStrategy(orderData.InstrumentToken).StrategyWorker.ConsumedOrderUpdateAsync(orderData)
            'End If
            'OnHeartbeat(String.Format("OrderUpdate {0}", Utils.JsonSerialize(orderData)))
        End Sub
        Public Sub New(ByVal apiAdapter As APIAdapter, ByVal canceller As CancellationTokenSource)
            MyBase.New(apiAdapter, canceller)
        End Sub
    End Class
End Namespace