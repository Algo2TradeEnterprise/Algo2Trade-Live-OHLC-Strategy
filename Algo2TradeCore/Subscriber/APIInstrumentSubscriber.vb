﻿Imports System.Threading
Imports Algo2TradeCore.Adapter
Imports Algo2TradeCore.Strategy

Namespace Subscriber
    Public MustInherit Class APIInstrumentSubscriber
        Protected _apiAdapter As APIAdapter
        Protected _cts As CancellationTokenSource
        Protected _subscribedStrategyInstruments As Dictionary(Of String, StrategyInstrument)
        Public SubcribedInstruments As List(Of String)
        Public Sub New(ByVal apiAdapter As APIAdapter, ByVal canceller As CancellationTokenSource)
            _apiAdapter = apiAdapter
            _cts = canceller
        End Sub
        Public Overridable Sub SubscribeStrategy(ByVal strategy As StrategyInstrument)
            If _subscribedStrategyInstruments Is Nothing Then _subscribedStrategyInstruments = New Dictionary(Of String, StrategyInstrument)
            _subscribedStrategyInstruments.Add(strategy.TradableInstrument.InstrumentIdentifier, strategy)
            If SubcribedInstruments IsNot Nothing Then SubcribedInstruments = New List(Of String)
            If Not SubcribedInstruments.Contains(strategy.TradableInstrument.InstrumentIdentifier) Then
                SubcribedInstruments.Add(strategy.TradableInstrument.InstrumentIdentifier)
            End If
        End Sub
    End Class
End Namespace