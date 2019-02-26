Namespace Entities
    Public Interface IOrder
        ReadOnly Property OrderIdentifier As String
        ReadOnly Property TriggerPrice As Decimal
        ReadOnly Property TransactionType As String
        ReadOnly Property Tradingsymbol As String
        ReadOnly Property Status As String
        ReadOnly Property Quantity As Integer
        ReadOnly Property Price As Decimal
        ReadOnly Property PendingQuantity As Integer
        ReadOnly Property InstrumentIdentifier As String
        ReadOnly Property FilledQuantity As Integer
        ReadOnly Property AveragePrice As Decimal
        ReadOnly Property Tag As String
        ReadOnly Property ParentOrderIdentifier As String
        ReadOnly Property TimeStamp As Date
        ReadOnly Property Broker As APISource
    End Interface
End Namespace
