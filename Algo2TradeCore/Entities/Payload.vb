Public Class Payload
    Public Property TradingSymbol As String
    Public Property OpenPrice As Decimal
    Public Property HighPrice As Decimal
    Public Property LowPrice As Decimal
    Public Property ClosePrice As Decimal
    Public Property Volume As Long
    Public Property DailyVolume As Long
    Public Property SnapshotDateTime As Date
    Public Property PreviousPayload As Payload
End Class
