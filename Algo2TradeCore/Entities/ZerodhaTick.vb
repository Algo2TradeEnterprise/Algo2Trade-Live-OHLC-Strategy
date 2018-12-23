Imports KiteConnect

Namespace Entities
    Public Class ZerodhaTick
        Implements ITick

        Public Property WrappedTick As Tick
        Public ReadOnly Property Broker As APISource Implements ITick.Broker
            Get
                Return APISource.Zerodha
            End Get
        End Property
    End Class
End Namespace
