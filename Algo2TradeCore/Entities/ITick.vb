Namespace Entities
    Public Interface ITick
        ReadOnly Property Close As Decimal
        ReadOnly Property High As Decimal
        ReadOnly Property InstrumentToken As String
        ReadOnly Property LastPrice As Decimal
        ReadOnly Property Low As Decimal
        ReadOnly Property Open As Decimal
        ReadOnly Property Timestamp As Date?
        ReadOnly Property Tradable As Boolean

        ReadOnly Property Broker As APISource
    End Interface
End Namespace