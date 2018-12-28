Imports KiteConnect

Namespace Entities
    <Serializable>
    Public Class ZerodhaInstrument
        Implements IInstrument

        Public Sub New(ByVal associatedIdentifer As String)
            InstrumentIdentifier = associatedIdentifer
        End Sub
        Public Property InstrumentIdentifier As String Implements IInstrument.InstrumentIdentifier
        Public ReadOnly Property Exchange As String Implements IInstrument.Exchange
            Get
                Return WrappedInstrument.Exchange
            End Get
        End Property

        Public ReadOnly Property Expiry As Date? Implements IInstrument.Expiry
            Get
                Return WrappedInstrument.Expiry
            End Get
        End Property

        Public ReadOnly Property InstrumentType As String Implements IInstrument.InstrumentType
            Get
                Return WrappedInstrument.InstrumentType
            End Get
        End Property

        Public ReadOnly Property LotSize As UInteger Implements IInstrument.LotSize
            Get
                Return WrappedInstrument.LotSize
            End Get
        End Property

        Public ReadOnly Property Segment As String Implements IInstrument.Segment
            Get
                Return WrappedInstrument.Segment
            End Get
        End Property

        Public ReadOnly Property TickSize As Decimal Implements IInstrument.TickSize
            Get
                Return WrappedInstrument.TickSize
            End Get
        End Property

        Public ReadOnly Property TradingSymbol As String Implements IInstrument.TradingSymbol
            Get
                Return WrappedInstrument.TradingSymbol
            End Get
        End Property
        Public Property WrappedInstrument As Instrument
        Public ReadOnly Property Broker As APISource Implements IInstrument.Broker
            Get
                Return APISource.Zerodha
            End Get
        End Property
        Public Overrides Function ToString() As String
            Return InstrumentIdentifier
        End Function
        Public Property WrappedLastTick As Tick
    End Class
End Namespace
