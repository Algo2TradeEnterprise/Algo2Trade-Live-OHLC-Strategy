Namespace Entities
    Public Interface IBusinessOrder
        Property ParentOrderIdentifier As String
        ReadOnly Property Broker As APISource
        Property ParentOrder As IOrder
        Property TargetOrder As IEnumerable(Of IOrder)
        Property SLOrder As IEnumerable(Of IOrder)
    End Interface
End Namespace
