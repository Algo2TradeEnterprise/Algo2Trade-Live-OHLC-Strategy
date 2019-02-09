Namespace Entities
    Public Interface IPayload
        Property TradingSymbol As String
        Property OpenPrice As Decimal
        Property HighPrice As Decimal
        Property LowPrice As Decimal
        Property ClosePrice As Decimal
        Property Volume As Long
        Property DailyVolume As Long
        Property SnapshotDateTime As Date
        Property PreviousPayload As IPayload
        Property NumberOfTicks As Integer
        ReadOnly Property PayloadGeneratedBy As PayloadSource
        Enum PayloadSource
            Tick
            Historical
        End Enum
    End Interface
End Namespace