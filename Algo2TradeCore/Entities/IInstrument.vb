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
        ReadOnly Property RawTicks As Concurrent.ConcurrentDictionary(Of Date, ITick)
        ReadOnly Property RawPayloads As Dictionary(Of Date, OHLCPayload)
    End Interface
End Namespace
