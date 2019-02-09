Imports Algo2TradeCore.ChartHandler.ChartStyle
Imports Algo2TradeCore.Controller
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
        Property RawTickPayloads As Concurrent.ConcurrentDictionary(Of Date, OHLCPayload)
        Property RawPayloads As SortedDictionary(Of Date, OHLCPayload)
        Property RawTicks As Concurrent.ConcurrentDictionary(Of Date, ITick)
        ReadOnly Property ParentController As APIStrategyController
        ReadOnly Property CandleStickCreator As CandleStickChart
    End Interface
End Namespace
