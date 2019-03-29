Namespace Entities.Indicators
    Public Class SMAConsumer
        Inherits PayloadToIndicatorConsumer
        Public Sub New(ByVal associatedParentConsumer As IPayloadConsumer, ByVal smaPeriod As Integer, ByVal smaField As Enums.TypeOfField)
            'MyBase.New(Indicator.SMA)
            MyBase.New(associatedParentConsumer)
            Me.SMAPeriod = smaPeriod
            Me.SMAField = smaField
        End Sub
        Public ReadOnly Property SMAPeriod As Integer
        Public ReadOnly Property SMAField As Enums.TypeOfField
        Class SMAPayload
            Implements IPayload
            Public Sub New()
                Me.SMA = New Field(TypeOfField.SMA)
            End Sub
            Public Property TradingSymbol As String Implements IPayload.TradingSymbol
            Public Property SMA As Field
        End Class
    End Class
End Namespace