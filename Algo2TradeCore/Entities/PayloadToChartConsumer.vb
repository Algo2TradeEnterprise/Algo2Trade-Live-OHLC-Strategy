Namespace Entities
    Public Class PayloadToChartConsumer
        Implements IPayloadConsumer
        Public Sub New(ByVal timeframe As Integer)
            _TypeOfConsumer = IPayloadConsumer.ConsumerType.Chart
            Me.Timeframe = timeframe
        End Sub

        Private _TypeOfConsumer As IPayloadConsumer.ConsumerType
        Public ReadOnly Property TypeOfConsumer As IPayloadConsumer.ConsumerType Implements IPayloadConsumer.TypeOfConsumer
            Get
                Return _TypeOfConsumer
            End Get
        End Property
        Public ReadOnly Property Timeframe As Integer
        Public Property ChartPayloads As Concurrent.ConcurrentDictionary(Of Date, OHLCPayload)
        Public Property OnwardLevelConsumers As List(Of IPayloadConsumer) Implements IPayloadConsumer.OnwardLevelConsumers
    End Class
End Namespace
