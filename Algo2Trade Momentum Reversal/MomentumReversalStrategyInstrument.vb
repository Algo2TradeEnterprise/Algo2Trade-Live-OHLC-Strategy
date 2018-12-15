Imports System.Threading
Imports Algo2TradeCore
Imports Algo2TradeCore.Adapter
Imports Algo2TradeCore.Entity
Imports Algo2TradeCore.Strategy

Public Class MomentumReversalStrategyInstrument
    Inherits StrategyInstrument
    Public Sub New(ByVal apiAdapter As APIAdapter, ByVal associatedInstrument As IInstrument, ByVal canceller As CancellationTokenSource)
        MyBase.New(apiAdapter, associatedInstrument, canceller)
    End Sub
    Public Shared Async Function GetAllTradableInstrumentsAsync(ByVal allInstruments As IEnumerable(Of IInstrument),
                                                                ByVal apiAdapter As APIAdapter,
                                                                ByVal canceller As CancellationTokenSource) As Task(Of List(Of MomentumReversalStrategyInstrument))
        Dim ret As List(Of MomentumReversalStrategyInstrument) = Nothing
        Await Task.Delay(0).ConfigureAwait(False)
        If allInstruments IsNot Nothing AndAlso allInstruments.Count > 0 Then
            If ret Is Nothing Then ret = New List(Of MomentumReversalStrategyInstrument)
            Parallel.ForEach(
                allInstruments,
                Sub(runningInstrument)
                    ret.Add(New MomentumReversalStrategyInstrument(apiAdapter, runningInstrument, canceller))
                End Sub)
        End If
        Return ret
    End Function

End Class
