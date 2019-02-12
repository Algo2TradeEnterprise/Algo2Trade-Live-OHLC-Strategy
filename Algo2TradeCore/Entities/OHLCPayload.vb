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
        Public Property NumberOfTicks As Integer Implements IPayload.NumberOfTicks
        Private _PayloadGeneratedBy As IPayload.PayloadSource
        Public ReadOnly Property PayloadGeneratedBy As IPayload.PayloadSource Implements IPayload.PayloadGeneratedBy
            Get
                Return _PayloadGeneratedBy
            End Get
        End Property

        Public Sub New(ByVal payloadGeneratedBy As IPayload.PayloadSource)
            Me._PayloadGeneratedBy = payloadGeneratedBy
        End Sub
        Public Overrides Function ToString() As String
            Return String.Format("TradingSymbol:{0}, Open:{1}, High:{2}, Low:{3}, Close:{4}, Volume:{5}, Timestamp:{6}",
                                 Me.TradingSymbol, Me.OpenPrice, Me.HighPrice, Me.LowPrice, Me.ClosePrice, Me.Volume, Me.SnapshotDateTime.ToString())
        End Function
    End Class
End Namespace