Imports System.Drawing

Namespace Entities
    Public Class OHLCPayload
        Implements IPayload
        Public Sub New(ByVal payloadGeneratedBy As IPayload.PayloadSource)
            Me._PayloadGeneratedBy = payloadGeneratedBy
        End Sub
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
        Public Property PayloadGeneratedBy As IPayload.PayloadSource Implements IPayload.PayloadGeneratedBy
            Get
                Return _PayloadGeneratedBy
            End Get
            Set(value As IPayload.PayloadSource)
                _PayloadGeneratedBy = value
            End Set
        End Property

        Private _CandleColor As Color
        Public ReadOnly Property CandleColor As Color Implements IPayload.CandleColor
            Get
                If Me.ClosePrice > Me.OpenPrice Then
                    _CandleColor = Color.Green
                ElseIf Me.ClosePrice < Me.OpenPrice Then
                    _CandleColor = Color.Red
                Else
                    _CandleColor = Color.White
                End If
                Return _CandleColor
            End Get
        End Property

        Private _CandleRange As Decimal
        Public ReadOnly Property CandleRange As Decimal Implements IPayload.CandleRange
            Get
                _CandleRange = Me.HighPrice - Me.LowPrice
                Return _CandleRange
            End Get
        End Property

        Private _CandleRangePercentage As Decimal
        Public ReadOnly Property CandleRangePercentage As Decimal Implements IPayload.CandleRangePercentage
            Get
                _CandleRangePercentage = Me.CandleRange * 100 / Me.ClosePrice
                Return _CandleRangePercentage
            End Get
        End Property

        Private _CandleWicks As IPayload.Wicks
        Public ReadOnly Property CandleWicks As IPayload.Wicks Implements IPayload.CandleWicks
            Get
                If _CandleWicks Is Nothing Then _CandleWicks = New IPayload.Wicks
                If Me.CandleColor = Color.Green Then
                    With _CandleWicks
                        .Top = Me.HighPrice - Me.ClosePrice
                        .Bottom = Me.OpenPrice - Me.LowPrice
                    End With
                ElseIf Me.CandleColor = Color.Red Then
                    With _CandleWicks
                        .Top = Me.HighPrice - Me.OpenPrice
                        .Bottom = Me.ClosePrice - Me.LowPrice
                    End With
                Else
                    With _CandleWicks
                        .Top = Me.HighPrice - Me.OpenPrice
                        .Bottom = Me.ClosePrice - Me.LowPrice
                    End With
                End If
                Return _CandleWicks
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return String.Format("TradingSymbol:{0}, Open:{1}, High:{2}, Low:{3}, Close:{4}, Volume:{5}, Timestamp:{6}, Source:{7}, DailyVolume:{8}",
                                 Me.TradingSymbol, Me.OpenPrice, Me.HighPrice, Me.LowPrice, Me.ClosePrice, Me.Volume,
                                 Me.SnapshotDateTime.ToString(), Me.PayloadGeneratedBy.ToString, Me.DailyVolume)
        End Function
        Public Overrides Function Equals(obj As Object) As Boolean
            Dim compareWith As OHLCPayload = obj
            With Me
                Return .OpenPrice = compareWith.OpenPrice And
                .HighPrice = compareWith.HighPrice And
                .LowPrice = compareWith.LowPrice And
                .ClosePrice = compareWith.ClosePrice And
                .Volume = compareWith.Volume And
                Utilities.Time.IsTimeEqualTillSeconds(.SnapshotDateTime, compareWith.SnapshotDateTime)
            End With
        End Function
    End Class
End Namespace