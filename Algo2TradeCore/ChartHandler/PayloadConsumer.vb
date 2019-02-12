Imports System.Threading
Imports Algo2TradeCore.Entities

Namespace ChartHandler
    Public MustInherit Class PayloadConsumer
        Protected _cts As CancellationTokenSource
        Public Sub New(ByVal canceller As CancellationTokenSource)
            _cts = canceller
        End Sub
        Public MustOverride Async Function PopulateFromOHLCAsync(ByVal payload As OHLCPayload) As Task
        Public MustOverride Async Function PopulateFromValuesAsync(ByVal ParamArray values() As Tuple(Of Date, Decimal)) As Task
    End Class
End Namespace