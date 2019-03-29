Namespace Entities.Indicators
    Public Class ATRConsumer
        Inherits PayloadToIndicatorConsumer
        Public Sub New(ByVal associatedParentConsumer As IPayloadConsumer, ByVal atrPeriod As Integer)
            MyBase.New(associatedParentConsumer)
            Me.ATRPeriod = atrPeriod
        End Sub
        Public ReadOnly Property ATRPeriod As Integer
        Class ATRPayload
            Implements IPayload
            Public Sub New()
                Me.ATR = New Field(TypeOfField.ATR)
            End Sub
            Public Property TradingSymbol As String Implements IPayload.TradingSymbol
            Public Property ATR As Field
        End Class
    End Class
End Namespace