Imports System.ComponentModel
Imports System.ComponentModel.DataAnnotations
Imports System.Threading
Imports Algo2TradeCore
Imports Algo2TradeCore.Adapter
Imports Algo2TradeCore.Entities
Imports Algo2TradeCore.Strategies
Imports NLog
Imports Utilities.Numbers.NumberManipulation

Public Class OHLStrategyInstrument
    Inherits StrategyInstrument
    Implements IDisposable

#Region "Logging and Status Progress"
    Public Shared Shadows logger As Logger = LogManager.GetCurrentClassLogger
#End Region

    Private _OHLStrategyProtector As Integer = 0

    <Display(Name:="OHL", Order:=10)>
    Public ReadOnly Property OHL As String
        Get
            If Me.OpenPrice = Me.LowPrice Then
                Return "O=L"
            ElseIf Me.OpenPrice = Me.HighPrice Then
                Return "O=H"
            Else
                Return Nothing
            End If
        End Get
    End Property

    Public Sub New(ByVal associatedInstrument As IInstrument, ByVal associatedParentStrategy As Strategy, ByVal canceller As CancellationTokenSource)
        MyBase.New(associatedInstrument, associatedParentStrategy, canceller)
        _APIAdapter = New ZerodhaAdapter(ParentStrategy.ParentContoller, _cts)
        AddHandler _APIAdapter.Heartbeat, AddressOf OnHeartbeat
        AddHandler _APIAdapter.WaitingFor, AddressOf OnWaitingFor
        AddHandler _APIAdapter.DocumentRetryStatus, AddressOf OnDocumentRetryStatus
        AddHandler _APIAdapter.DocumentDownloadComplete, AddressOf OnDocumentDownloadComplete
    End Sub
    Public Overrides Function ToString() As String
        Return String.Format("{0}_{1}", ParentStrategy.ToString, TradableInstrument.ToString)
    End Function
    Public Overrides Async Function RunDirectAsync() As Task
        logger.Debug("{0}->RunDirectAsync, parameters:None", Me.ToString)
        _cts.Token.ThrowIfCancellationRequested()
        Try
            While Me.ParentStrategy.ParentContoller.APIConnection Is Nothing
                _cts.Token.ThrowIfCancellationRequested()
                logger.Debug("Waiting for fresh token:{0}", TradableInstrument.InstrumentIdentifier)
                Await Task.Delay(500).ConfigureAwait(False)
            End While
            Dim triggerRecevied As Tuple(Of Boolean, Trigger) = Await IsTriggerReachedAsync().ConfigureAwait(False)
            If triggerRecevied IsNot Nothing AndAlso triggerRecevied.Item1 = True Then
                _APIAdapter.SetAPIAccessToken(Me.ParentStrategy.ParentContoller.APIConnection.AccessToken)
                _cts.Token.ThrowIfCancellationRequested()
                Dim allTrades As IEnumerable(Of ITrade) = Await _APIAdapter.GetAllTradesAsync().ConfigureAwait(False)
                _cts.Token.ThrowIfCancellationRequested()
            End If
        Catch ex As Exception
            logger.Debug("^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^")
            logger.Error(ex.ToString)
        End Try
    End Function
    Public Overrides Async Function IsTriggerReachedAsync() As Task(Of Tuple(Of Boolean, Trigger))
        logger.Debug("{0}->IsTriggerReachedAsync, parameters:None", Me.ToString)
        _cts.Token.ThrowIfCancellationRequested()
        Await Task.Delay(0).ConfigureAwait(False)
        Dim ret As Tuple(Of Boolean, Trigger) = Nothing
        If _LastTick IsNot Nothing AndAlso _LastTick.Timestamp IsNot Nothing AndAlso _LastTick.Open = _LastTick.Low Then
            ret = New Tuple(Of Boolean, Trigger)(True, New Trigger() With {.Category = Trigger.TriggerType.PriceMatched, .Description = String.Format("O=L,({0})", _LastTick.Open)})
        ElseIf _LastTick IsNot Nothing AndAlso _LastTick.Timestamp IsNot Nothing AndAlso _LastTick.Open = _LastTick.High Then
            ret = New Tuple(Of Boolean, Trigger)(True, New Trigger() With {.Category = Trigger.TriggerType.PriceMatched, .Description = String.Format("O=H,({0})", _LastTick.Open)})
        End If
        'TO DO: Remove the below hard coding
        ret = New Tuple(Of Boolean, Trigger)(True, Nothing)
        Return ret
    End Function
    Public Overrides Async Function ProcessTickAsync(ByVal tickData As ITick) As Task
        'logger.Debug("ProcessTickAsync, tickData:{0}", Utilities.Strings.JsonSerialize(tickData))
        _cts.Token.ThrowIfCancellationRequested()
        _LastTick = tickData
        NotifyPropertyChanged("OHL")
        Await MyBase.ProcessTickAsync(tickData).ConfigureAwait(False)
        _cts.Token.ThrowIfCancellationRequested()
    End Function
    Public Overrides Async Function MonitorAsync() As Task
        Dim lastException As Exception = Nothing
        'Try

        While True
            If Me.ParentStrategy.ParentContoller.OrphanException IsNot Nothing Then
                Throw Me.ParentStrategy.ParentContoller.OrphanException
            End If
            'Dim r As Random = New Random()
            'Dim x = r.Next(0, 11)
            '_cts.Token.ThrowIfCancellationRequested()
            'If x = 7 Then
            '    'Zerodha call start - requires till end
            '    While Me.ParentStrategy.ParentContoller.APIConnection Is Nothing
            '        _cts.Token.ThrowIfCancellationRequested()
            '        logger.Debug("Waiting for fresh token:{0}", TradableInstrument.InstrumentIdentifier)
            '        Await Task.Delay(500).ConfigureAwait(False)
            '    End While

            '    _APIAdapter.SetAPIAccessToken(Me.ParentStrategy.ParentContoller.APIConnection.AccessToken)
            '    _cts.Token.ThrowIfCancellationRequested()

            '    Dim allTrades As IEnumerable(Of ITrade) = Await _APIAdapter.GetAllTradesAsync().ConfigureAwait(False)
            '    'Zerodha call end
            'End If
            Dim orderDetails As Dictionary(Of String, Object) = Await RunStrategyAsync().ConfigureAwait(False)
            _cts.Token.ThrowIfCancellationRequested()
            Await Task.Delay(1000)
        End While
        'Catch ex As Exception
        '    lastException = ex
        'End Try
        'If lastException IsNot Nothing Then
        '    Await Me.ParentStrategy.ParentContoller.CloseTickerIfConnectedAsync()
        '    Throw lastException
        'End If
        'Dim ctr As Integer
        'While True
        '    _cts.Token.ThrowIfCancellationRequested()
        '    Await Task.Delay(500)
        '    ctr += 500

        '    If ctr = 115000 Then
        '        'ret = New ApplicationException("Donno")
        '        ONJOYMA(New ApplicationException("Donno"))
        '        Exit While
        '    End If
        'End While
    End Function
    Private Async Function RunStrategyAsync() As Task(Of Dictionary(Of String, Object))
        Await Task.Delay(0).ConfigureAwait(False)
        Dim currentTime As Date = Now
        Dim ret As Dictionary(Of String, Object) = Nothing
        If _LastTick.Timestamp IsNot Nothing AndAlso _OHLStrategyProtector = 0 AndAlso
            currentTime.Hour = 12 AndAlso currentTime.Minute = 23 AndAlso currentTime.Second = 20 Then
            Dim trigger As Tuple(Of Boolean, APIAdapter.TransactionType, Decimal) = Await IsTriggerReceivedAsync().ConfigureAwait(False)
            If trigger IsNot Nothing AndAlso trigger.Item1 = True Then
                Dim OHLTradePrice As Decimal = trigger.Item3
                Dim buffer As Decimal = OHLTradePrice * 0.005
                Dim bufferModified As Decimal = Math.Round(ConvertFloorCeling(buffer, Convert.ToDouble(TradableInstrument.TickSize), RoundOfType.Floor), 2)
                Dim price As Decimal = Nothing
                Dim target As Decimal = Nothing
                Dim stoploss As Decimal = Nothing
                Select Case trigger.Item2
                    Case APIAdapter.TransactionType.Buy
                        price = OHLTradePrice + bufferModified
                        target = Math.Round(ConvertFloorCeling(OHLTradePrice * 0.005, Convert.ToDouble(TradableInstrument.TickSize), RoundOfType.Celing), 2)
                        stoploss = If(Math.Abs(OHLTradePrice - _LastTick.Low) = 0, Convert.ToDouble(TradableInstrument.TickSize) * 2, Math.Abs(OHLTradePrice - _LastTick.Low))
                    Case APIAdapter.TransactionType.Sell
                        price = OHLTradePrice - bufferModified
                        target = Math.Round(ConvertFloorCeling(OHLTradePrice * 0.005, Convert.ToDouble(TradableInstrument.TickSize), RoundOfType.Celing), 2)
                        stoploss = If(Math.Abs(_LastTick.High - OHLTradePrice) = 0, Convert.ToDouble(TradableInstrument.TickSize) * 2, Math.Abs(_LastTick.High - OHLTradePrice))
                End Select
                Interlocked.Increment(_OHLStrategyProtector)
                'Place Order
                While Me.ParentStrategy.ParentContoller.APIConnection Is Nothing
                    _cts.Token.ThrowIfCancellationRequested()
                    logger.Debug("Waiting for fresh token:{0}", TradableInstrument.InstrumentIdentifier)
                    Await Task.Delay(500).ConfigureAwait(False)
                End While
                _APIAdapter.SetAPIAccessToken(Me.ParentStrategy.ParentContoller.APIConnection.AccessToken)
                _cts.Token.ThrowIfCancellationRequested()
                Try
                    ret = Await _APIAdapter.PlaceBOLimitOrderAsync(tradeExchange:=APIAdapter.Exchange.NSE,
                                                                   tradingSymbol:=TradingSymbol,
                                                                   transaction:=trigger.Item2,
                                                                   quantity:=1,
                                                                   price:=price,
                                                                   squareOffValue:=target,
                                                                   stopLossValue:=stoploss,
                                                                   tag:=Me.ToString).ConfigureAwait(False)
                Catch ex As Exception
                    OnHeartbeat(String.Format("PlaceOrder execution failed. Will not retry. Error:{0}", ex.Message))
                End Try
            End If
        End If
        Return ret
    End Function
    Private Async Function IsTriggerReceivedAsync() As Task(Of Tuple(Of Boolean, APIAdapter.TransactionType, Decimal))
        Await Task.Delay(0).ConfigureAwait(False)
        Dim ret As Tuple(Of Boolean, APIAdapter.TransactionType, Decimal) = Nothing
        If _LastTick.Open = _LastTick.High AndAlso
                 _LastTick.Open = _LastTick.High Then
            ret = New Tuple(Of Boolean, APIAdapter.TransactionType, Decimal)(True, APIAdapter.TransactionType.Sell, _LastTick.LastPrice)
        ElseIf _LastTick.Open = _LastTick.Low AndAlso
            _LastTick.Open = _LastTick.Low Then
            ret = New Tuple(Of Boolean, APIAdapter.TransactionType, Decimal)(True, APIAdapter.TransactionType.Buy, _LastTick.LastPrice)
        End If
        Return ret
    End Function

#Region "IDisposable Support"
    Private disposedValue As Boolean ' To detect redundant calls

    ' IDisposable
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                ' TODO: dispose managed state (managed objects).
                If _APIAdapter IsNot Nothing Then
                    RemoveHandler _APIAdapter.Heartbeat, AddressOf OnHeartbeat
                    RemoveHandler _APIAdapter.WaitingFor, AddressOf OnWaitingFor
                    RemoveHandler _APIAdapter.DocumentRetryStatus, AddressOf OnDocumentRetryStatus
                    RemoveHandler _APIAdapter.DocumentDownloadComplete, AddressOf OnDocumentDownloadComplete
                End If
            End If

            ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
            ' TODO: set large fields to null.
        End If
        disposedValue = True
    End Sub

    ' TODO: override Finalize() only if Dispose(disposing As Boolean) above has code to free unmanaged resources.
    'Protected Overrides Sub Finalize()
    '    ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
    '    Dispose(False)
    '    MyBase.Finalize()
    'End Sub

    ' This code added by Visual Basic to correctly implement the disposable pattern.
    Public Sub Dispose() Implements IDisposable.Dispose
        ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
        Dispose(True)
        ' TODO: uncomment the following line if Finalize() is overridden above.
        ' GC.SuppressFinalize(Me)
    End Sub
#End Region
End Class
