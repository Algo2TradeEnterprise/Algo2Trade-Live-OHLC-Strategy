Imports KiteConnect

Namespace Entities
    Public Class ZerodhaOrder
        Implements IOrder

        Public Property WrappedOrder As Order

        Public ReadOnly Property OrderIdentifier As String Implements IOrder.OrderIdentifier
            Get
                Return WrappedOrder.OrderId
            End Get
        End Property

        Public ReadOnly Property Broker As APISource Implements IOrder.Broker
            Get
                Return APISource.Zerodha
            End Get
        End Property

        Public ReadOnly Property TriggerPrice As Decimal Implements IOrder.TriggerPrice
            Get
                Return WrappedOrder.TriggerPrice
            End Get
        End Property

        Public ReadOnly Property TransactionType As String Implements IOrder.TransactionType
            Get
                Return WrappedOrder.TransactionType
            End Get
        End Property

        Public ReadOnly Property Tradingsymbol As String Implements IOrder.Tradingsymbol
            Get
                Return WrappedOrder.Tradingsymbol
            End Get
        End Property

        Public ReadOnly Property Status As String Implements IOrder.Status
            Get
                Return WrappedOrder.Status
            End Get
        End Property

        Public ReadOnly Property Quantity As Integer Implements IOrder.Quantity
            Get
                Return WrappedOrder.Quantity
            End Get
        End Property

        Public ReadOnly Property Price As Decimal Implements IOrder.Price
            Get
                Return WrappedOrder.Price
            End Get
        End Property

        Public ReadOnly Property PendingQuantity As Integer Implements IOrder.PendingQuantity
            Get
                Return WrappedOrder.PendingQuantity
            End Get
        End Property

        Public ReadOnly Property InstrumentIdentifier As String Implements IOrder.InstrumentIdentifier
            Get
                Return WrappedOrder.InstrumentToken
            End Get
        End Property

        Public ReadOnly Property FilledQuantity As Integer Implements IOrder.FilledQuantity
            Get
                Return WrappedOrder.FilledQuantity
            End Get
        End Property

        Public ReadOnly Property AveragePrice As Decimal Implements IOrder.AveragePrice
            Get
                Return WrappedOrder.AveragePrice
            End Get
        End Property

        Public ReadOnly Property ParentOrderIdentifier As String Implements IOrder.ParentOrderIdentifier
            Get
                Return WrappedOrder.ParentOrderId
            End Get
        End Property

        Public ReadOnly Property Tag As String Implements IOrder.Tag
            Get
                Return WrappedOrder.Tag
            End Get
        End Property

        Public ReadOnly Property TimeStamp As Date Implements IOrder.TimeStamp
            Get
                Return WrappedOrder.OrderTimestamp
            End Get
        End Property

    End Class
End Namespace
