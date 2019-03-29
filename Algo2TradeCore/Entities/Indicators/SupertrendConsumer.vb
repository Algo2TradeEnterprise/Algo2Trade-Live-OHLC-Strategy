Imports System.Drawing

Namespace Entities.Indicators
    Public Class SupertrendConsumer
        Inherits PayloadToIndicatorConsumer
        Public Sub New(ByVal associatedParentConsumer As IPayloadConsumer, ByVal supertrendPeriod As Integer, ByVal supertrendMultiplier As Integer)
            MyBase.New(associatedParentConsumer)
            Me.SupertrendPeriod = supertrendPeriod
            Me.SupertrendMultiplier = supertrendMultiplier
            Me.SupportingATRConsumer = New ATRConsumer(associatedParentConsumer, supertrendPeriod)
        End Sub
        Public ReadOnly Property SupertrendPeriod As Integer
        Public ReadOnly Property SupertrendMultiplier As Integer
        Public ReadOnly Property SupportingATRConsumer As ATRConsumer
        Class SupertrendPayload
            Implements IPayload
            Public Sub New()
                Me.Supertrend = New Field(TypeOfField.Supertrend)
            End Sub
            Public Property TradingSymbol As String Implements IPayload.TradingSymbol
            Public Property FinalUpperBand As Decimal
            Public Property FinalLowerBand As Decimal
            Public Property Supertrend As Field
            Public Property SupertrendColor As Color
        End Class
    End Class
End Namespace