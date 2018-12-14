Imports KiteConnect

Namespace Entity
    Public Class ZerodhaInstrument
        Implements IInstrument

        Public Sub New(ByVal identifier As String)
            InstrumentIdentifier = identifier
        End Sub
        Public Property InstrumentIdentifier As String Implements IInstrument.InstrumentIdentifier
        Public Property WrappedInstrument As Instrument
    End Class
End Namespace
