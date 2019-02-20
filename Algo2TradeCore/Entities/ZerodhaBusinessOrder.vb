Namespace Entities
    Public Class ZerodhaBusinessOrder
        Implements IBusinessOrder

        Public ReadOnly Property Broker As APISource Implements IBusinessOrder.Broker
            Get
                Return APISource.Zerodha
            End Get
        End Property

        Public Property ParentOrder As IOrder Implements IBusinessOrder.ParentOrder

        Public Property ParentOrderIdentifier As String Implements IBusinessOrder.ParentOrderIdentifier

        Public Property SLOrder As IEnumerable(Of IOrder) Implements IBusinessOrder.SLOrder

        Public Property TargetOrder As IEnumerable(Of IOrder) Implements IBusinessOrder.TargetOrder

        Public Property AllOrder As IEnumerable(Of IOrder) Implements IBusinessOrder.AllOrder

    End Class
End Namespace
