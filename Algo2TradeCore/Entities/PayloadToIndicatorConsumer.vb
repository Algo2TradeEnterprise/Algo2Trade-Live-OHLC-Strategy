Namespace Entities
    Public Class PayloadToIndicatorConsumer
        Implements IPayloadConsumer
        Public Sub New(ByVal typeOfIndicator As Enums.Indicator)
            _TypeOfConsumer = IPayloadConsumer.ConsumerType.Indicator
            Me.TypeOfIndicator = typeOfIndicator
        End Sub

        Private _TypeOfConsumer As IPayloadConsumer.ConsumerType
        Public ReadOnly Property TypeOfConsumer As IPayloadConsumer.ConsumerType Implements IPayloadConsumer.TypeOfConsumer
            Get
                Return _TypeOfConsumer
            End Get
        End Property
        Public ReadOnly Property TypeOfIndicator As Enums.Indicator
        Public Property OnwardLevelConsumers As List(Of IPayloadConsumer) Implements IPayloadConsumer.OnwardLevelConsumers
    End Class
End Namespace
