Namespace Entities
    Public Class OHLCPayload
        Implements IPayload

        Public Property TradingSymbol As String Implements IPayload.TradingSymbol
        Public Property OpenPrice As Decimal Implements IPayload.OpenPrice
        Public Property HighPrice As Decimal Implements IPayload.HighPrice
        Public Property LowPrice As Decimal Implements IPayload.LowPrice
        Public Property ClosePrice As Decimal Implements IPayload.ClosePrice
        Public Property Volume As Long Implements IPayload.Volume
        Public Property DailyVolume As Long Implements IPayload.DailyVolume
        Public Property SnapshotDateTime As Date Implements IPayload.SnapshotDateTime
        Public Property PreviousPayload As IPayload Implements IPayload.PreviousPayload
    End Class
End Namespace