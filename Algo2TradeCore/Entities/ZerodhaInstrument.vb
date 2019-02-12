Imports KiteConnect
Imports Algo2TradeCore.ChartHandler.ChartStyle
Imports Algo2TradeCore.Controller
Imports Algo2TradeCore.Strategies

Namespace Entities
    <Serializable>
    Public Class ZerodhaInstrument
        Implements IInstrument

        Private _RawTicks As Concurrent.ConcurrentDictionary(Of Date, ITick)
        Private _RawPayloads As SortedDictionary(Of Date, OHLCPayload)
        Private _RawTickPayloads As Concurrent.ConcurrentDictionary(Of Date, OHLCPayload)
        Private _ParentController As APIStrategyController
        Private _CandleStickCreator As CandleStickChart
        Public Sub New(ByVal associatedParentController As APIStrategyController, ByVal associatedIdentifer As String)
            InstrumentIdentifier = associatedIdentifer
            _RawTicks = New Concurrent.ConcurrentDictionary(Of Date, ITick)
            _RawPayloads = New SortedDictionary(Of Date, OHLCPayload)
            _RawTickPayloads = New Concurrent.ConcurrentDictionary(Of Date, OHLCPayload)
            _ParentController = associatedParentController
            _CandleStickCreator = New CandleStickChart(Me.ParentController, Me, New Threading.CancellationTokenSource)
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
                        CandleStickCreator.FIFOProcessTickToCandleStickAsync()
                    Else
                        'Console.WriteLine(String.Format("Could not store:{0},{1},{2}", TradingSymbol, tickData.LastPrice, tickData.Timestamp.Value.ToLongTimeString))
                    End If
                End If
            End Set
        End Property
        Public Property RawTickPayloads As Concurrent.ConcurrentDictionary(Of Date, OHLCPayload) Implements IInstrument.RawTickPayloads
            Get
                Return _RawTickPayloads
            End Get
            Set(value As Concurrent.ConcurrentDictionary(Of Date, OHLCPayload))
                _RawTickPayloads = value
            End Set
        End Property

        Public Property RawPayloads As SortedDictionary(Of Date, OHLCPayload) Implements IInstrument.RawPayloads
            Get
                Return _RawPayloads
            End Get
            Set(value As SortedDictionary(Of Date, OHLCPayload))
                _RawPayloads = value
            End Set
        End Property

        Public Property RawTicks As Concurrent.ConcurrentDictionary(Of Date, ITick) Implements IInstrument.RawTicks
            Get
                Return _RawTicks
            End Get
            Set(value As Concurrent.ConcurrentDictionary(Of Date, ITick))
                _RawTicks = value
            End Set
        End Property

        Public ReadOnly Property ParentController As APIStrategyController Implements IInstrument.ParentController
            Get
                Return _ParentController
            End Get
        End Property

        Public ReadOnly Property CandleStickCreator As CandleStickChart Implements IInstrument.CandleStickCreator
            Get
                Return _CandleStickCreator
            End Get
        End Property
        Public Property FirstLevelConsumers As List(Of StrategyInstrument) Implements IInstrument.FirstLevelConsumers

    End Class
End Namespace
