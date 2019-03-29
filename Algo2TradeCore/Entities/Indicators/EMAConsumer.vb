Namespace Entities.Indicators
    Public Class EMAConsumer
        Inherits PayloadToIndicatorConsumer
        Public Sub New(ByVal associatedParentConsumer As IPayloadConsumer, ByVal emaPeriod As Integer, ByVal emaField As Enums.TypeOfField)
            MyBase.New(associatedParentConsumer)
            Me.EMAPeriod = emaPeriod
            Me.EMAField = emaField
            Me.SupportingSMAConsumer = New SMAConsumer(associatedParentConsumer, emaPeriod, emaField)
        End Sub
        Public ReadOnly Property EMAPeriod As Integer
        Public ReadOnly Property EMAField As Enums.TypeOfField
        Public ReadOnly Property SupportingSMAConsumer As SMAConsumer
        Class EMAPayload
            Implements IPayload
            Public Sub New()
                Me.EMA = New Field(TypeOfField.EMA)
            End Sub
            Public Property TradingSymbol As String Implements IPayload.TradingSymbol
            Public Property EMA As Field
        End Class
    End Class
End Namespace