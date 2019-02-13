Namespace Entities
    Public Class PayloadToIndicatorConsumer
        Implements IPayloadConsumer
        Private _TypeOfConsumer As IPayloadConsumer.ConsumerType
        Public Sub New()
            _TypeOfConsumer = IPayloadConsumer.ConsumerType.Indicator
        End Sub
        Public ReadOnly Property TypeOfConsumer As IPayloadConsumer.ConsumerType Implements IPayloadConsumer.TypeOfConsumer
            Get
                Return _TypeOfConsumer
            End Get
        End Property
        Public Property OnwardLevelConsumers As List(Of IPayloadConsumer) Implements IPayloadConsumer.OnwardLevelConsumers
    End Class
End Namespace
