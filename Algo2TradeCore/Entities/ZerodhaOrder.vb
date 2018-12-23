Imports KiteConnect

Namespace Entities
    Public Class ZerodhaOrder
        Implements IOrder
        Public Property WrappedOrder As Order
        Public ReadOnly Property Broker As APISource Implements IOrder.Broker
            Get
                Return APISource.Zerodha
            End Get
        End Property

    End Class
End Namespace
