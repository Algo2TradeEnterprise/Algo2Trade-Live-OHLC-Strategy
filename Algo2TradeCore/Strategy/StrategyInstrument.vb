Imports System.Threading
Imports Algo2TradeCore.Adapter
Imports Algo2TradeCore.Entity

Namespace Strategy
    Public MustInherit Class StrategyInstrument
        Protected _cts As CancellationTokenSource
        Protected _apiAdapter As APIAdapter
        Public Property TradableInstrument As IInstrument
        Public Sub New(ByVal apiAdapter As APIAdapter, ByVal associatedInstrument As IInstrument, ByVal canceller As CancellationTokenSource)
            _apiAdapter = apiAdapter
            TradableInstrument = associatedInstrument
            _cts = canceller
        End Sub
        Public Overridable Async Function ProcessTickAsync(ByVal tickData As ITick) As Task

        End Function
    End Class
End Namespace
