Imports KiteConnect
Imports Algo2TradeCore.ChartHandler.ChartStyle
Imports Algo2TradeCore.Controller

Namespace Entities
    <Serializable>
    Public Class ZerodhaInstrument
        Implements IInstrument

        Private _RawTicks As Concurrent.ConcurrentDictionary(Of Date, ITick)
        Private _RawPayloads As Dictionary(Of Date, OHLCPayload)
        Private _candleStickCreator As Candlestick
        Private _ParentController As APIStrategyController
        Public Sub New(ByVal associatedParentController As APIStrategyController, ByVal associatedIdentifer As String)
            InstrumentIdentifier = associatedIdentifer
            _RawTicks = New Concurrent.ConcurrentDictionary(Of Date, ITick)
            _RawPayloads = New Dictionary(Of Date, OHLCPayload)
            _ParentController = associatedParentController
            _candleStickCreator = New Candlestick(Me.ParentController, Me, New Threading.CancellationTokenSource)
            'Intentionally created a new token since we dont want to cancel this process via the global cancellationtoken
            'No handlers necessary to be captured as we are not anticipating anyt internet hit or long running transaction
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
        Private _LastTick As ITick
        Public Property LastTick As ITick Implements IInstrument.LastTick
            Get
                Return _LastTick
            End Get
            Set(value As ITick)
                If value.InstrumentToken = Me.InstrumentIdentifier Then
                    _LastTick = value
                    If _LastTick IsNot Nothing AndAlso
                         _LastTick.Timestamp IsNot Nothing AndAlso
                        _RawTicks.TryAdd(_LastTick.Timestamp, _LastTick) Then
                        _candleStickCreator.FIFOProcessTickToCandleStickAsync()
                    Else
                        'Console.WriteLine(String.Format("Could not store:{0},{1},{2}", TradingSymbol, tickData.LastPrice, tickData.Timestamp.Value.ToLongTimeString))
                    End If
                End If
            End Set
        End Property
        Public ReadOnly Property RawTicks As Concurrent.ConcurrentDictionary(Of Date, ITick) Implements IInstrument.RawTicks
            Get
                Return _RawTicks
            End Get
        End Property

        Public ReadOnly Property RawPayloads As Dictionary(Of Date, OHLCPayload) Implements IInstrument.RawPayloads
            Get
                Return _RawPayloads
            End Get
        End Property

        Public ReadOnly Property ParentController As APIStrategyController Implements IInstrument.ParentController
            Get
                Return _ParentController
            End Get
        End Property
    End Class
End Namespace
