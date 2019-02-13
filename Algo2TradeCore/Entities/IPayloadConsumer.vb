Namespace Entities
    Public Interface IPayloadConsumer
        ReadOnly Property TypeOfConsumer As ConsumerType
        Enum ConsumerType
            Chart = 1
            Indicator
        End Enum
        Property OnwardLevelConsumers As List(Of IPayloadConsumer)
    End Interface
End Namespace