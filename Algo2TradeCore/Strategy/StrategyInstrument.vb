Imports System.Threading
Imports Algo2TradeCore.Adapter
Imports Algo2TradeCore.Entity

Namespace Strategy
    Public MustInherit Class StrategyInstrument
        Protected _cts As CancellationTokenSource
        Protected _apiAdapter As APIAdapter
        Public Property TradableInstrument As IInstrument
        Protected _ticks As Utilities.Collections.LimitedStack(Of ITick)
        Public Sub New(ByVal apiAdapter As APIAdapter, ByVal associatedInstrument As IInstrument, ByVal canceller As CancellationTokenSource)
            _apiAdapter = apiAdapter
            TradableInstrument = associatedInstrument
            _cts = canceller
            _ticks = New Utilities.Collections.LimitedStack(Of ITick)(5500) 'Assuming 6 ticks per second for 15 minuters
        End Sub
        Public Overridable Async Function ProcessTickAsync(ByVal tickData As ITick) As Task
            Await Task.Delay(0).ConfigureAwait(False)
            _ticks.Push(tickData)
        End Function
    End Class
End Namespace
