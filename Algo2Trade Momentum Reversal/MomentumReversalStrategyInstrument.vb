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

            Dim retCtr As Integer = 0
            For Each runningInstrument In allInstruments
                'TO DO: Check if the instrument needs to be added or use custom code to extract instruments from DB or logic 
                'and then add to the rerturnable collection
                retCtr += 1
                If retCtr > 500 Then Exit For
                If ret Is Nothing Then ret = New List(Of MomentumReversalStrategyInstrument)
                ret.Add(New MomentumReversalStrategyInstrument(apiAdapter, runningInstrument, canceller))
            Next
        End If
        If ret IsNot Nothing AndAlso ret.Count > apiAdapter.MaxInstrumentPerTicker Then
            Throw New ApplicationException(String.Format("Max instruments per ticker exceeded, allowed:{0}, existing:{1}", apiAdapter.MaxInstrumentPerTicker, ret.Count))
        End If
        Return ret
    End Function

End Class
