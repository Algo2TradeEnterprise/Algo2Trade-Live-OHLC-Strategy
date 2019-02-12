Imports Algo2TradeCore.ChartHandler.ChartStyle
Imports Algo2TradeCore.Controller
Imports Algo2TradeCore.Strategies

Namespace Entities
    Public Interface IInstrument
        Property InstrumentIdentifier As String

        ReadOnly Property Exchange As String
        ReadOnly Property Expiry As Date?
        ReadOnly Property InstrumentType As String
        ReadOnly Property LotSize As UInteger
        ReadOnly Property Segment As String
        ReadOnly Property TickSize As Decimal
        ReadOnly Property TradingSymbol As String
        ReadOnly Property Broker As APISource
        Property LastTick As ITick
        Property RawTicks As Concurrent.ConcurrentDictionary(Of Date, ITick)
        Property RawTickPayloads As Concurrent.ConcurrentDictionary(Of Date, OHLCPayload)
        Property RawPayloads As SortedDictionary(Of Date, OHLCPayload)
        ReadOnly Property ParentController As APIStrategyController
        ReadOnly Property CandleStickCreator As CandleStickChart
        Property FirstLevelConsumers As List(Of StrategyInstrument)
    End Interface
End Namespace
