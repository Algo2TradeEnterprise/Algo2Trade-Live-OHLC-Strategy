Imports System.ComponentModel
Imports System.ComponentModel.DataAnnotations
Imports System.Net.Http
Imports System.Threading
Imports Algo2TradeCore.Adapter
Imports Algo2TradeCore.Chart
Imports Algo2TradeCore.Entities
Imports NLog
Imports Utilities
Imports Utilities.ErrorHandlers

Namespace Strategies
    Public MustInherit Class StrategyInstrument
        Implements INotifyPropertyChanged
        Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged
        Protected Sub NotifyPropertyChanged(ByVal p As String)
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(p))
        End Sub

#Region "Events/Event handlers"
        Public Event DocumentDownloadCompleteEx(ByVal source As List(Of Object))
        Public Event DocumentRetryStatusEx(ByVal currentTry As Integer, ByVal totalTries As Integer, ByVal source As List(Of Object))
        Public Event HeartbeatEx(ByVal msg As String, ByVal source As List(Of Object))
        Public Event WaitingForEx(ByVal elapsedSecs As Integer, ByVal totalSecs As Integer, ByVal msg As String, ByVal source As List(Of Object))
        'The below functions are needed to allow the derived classes to raise the above two events
        Protected Overridable Sub OnDocumentDownloadCompleteEx(ByVal source As List(Of Object))
            If source IsNot Nothing Then source = New List(Of Object)
            source.Add(Me)
            RaiseEvent DocumentDownloadCompleteEx(source)
        End Sub
        Protected Overridable Sub OnDocumentRetryStatusEx(ByVal currentTry As Integer, ByVal totalTries As Integer, ByVal source As List(Of Object))
            If source IsNot Nothing Then source = New List(Of Object)
            source.Add(Me)
            RaiseEvent DocumentRetryStatusEx(currentTry, totalTries, source)
        End Sub
        Protected Overridable Sub OnHeartbeatEx(ByVal msg As String, ByVal source As List(Of Object))
            If source IsNot Nothing Then source = New List(Of Object)
            source.Add(Me)
            If TradableInstrument IsNot Nothing Then
                RaiseEvent HeartbeatEx(String.Format("{0}:{1}", TradableInstrument.InstrumentIdentifier, msg), source)
            Else
                RaiseEvent HeartbeatEx(String.Format("{0}:{1}", "No instrument", msg), source)
            End If
        End Sub
        Protected Overridable Sub OnWaitingForEx(ByVal elapsedSecs As Integer, ByVal totalSecs As Integer, ByVal msg As String, ByVal source As List(Of Object))
            If source IsNot Nothing Then source = New List(Of Object)
            source.Add(Me)
            If TradableInstrument IsNot Nothing Then
                RaiseEvent WaitingForEx(elapsedSecs, totalSecs, String.Format("{0}-{1}", TradableInstrument.InstrumentIdentifier, msg), source)
            Else
                RaiseEvent WaitingForEx(elapsedSecs, totalSecs, String.Format("{0}-{1}", "No instrument", msg), source)
            End If
        End Sub
        Protected Overridable Sub OnDocumentDownloadComplete()
            RaiseEvent DocumentDownloadCompleteEx(New List(Of Object) From {Me})
        End Sub
        Protected Overridable Sub OnDocumentRetryStatus(ByVal currentTry As Integer, ByVal totalTries As Integer)
            RaiseEvent DocumentRetryStatusEx(currentTry, totalTries, New List(Of Object) From {Me})
        End Sub
        Protected Overridable Sub OnHeartbeat(ByVal msg As String)
            If TradableInstrument IsNot Nothing Then
                RaiseEvent HeartbeatEx(String.Format("{0}:{1}", TradableInstrument.InstrumentIdentifier, msg), New List(Of Object) From {Me})
            Else
                RaiseEvent HeartbeatEx(String.Format("{0}:{1}", "No instrument", msg), New List(Of Object) From {Me})
            End If
        End Sub
        Protected Overridable Sub OnWaitingFor(ByVal elapsedSecs As Integer, ByVal totalSecs As Integer, ByVal msg As String)
            If TradableInstrument IsNot Nothing Then
                RaiseEvent WaitingForEx(elapsedSecs, totalSecs, String.Format("{0}-{1}", TradableInstrument.InstrumentIdentifier, msg), New List(Of Object) From {Me})
            Else
                RaiseEvent WaitingForEx(elapsedSecs, totalSecs, String.Format("{0}-{1}", "No instrument", msg), New List(Of Object) From {Me})
            End If
        End Sub
#End Region

#Region "Logging and Status Progress"
        Public Shared logger As Logger = LogManager.GetCurrentClassLogger
#End Region

#Region "Enum"
        Public Enum ExecuteCommands
            PlaceBOLimtMISOrder = 1
        End Enum
#End Region


        <System.ComponentModel.Browsable(False)>
        Public Property ParentStrategy As Strategy
        <System.ComponentModel.Browsable(False)>
        Public Property TradableInstrument As IInstrument
        Public Property RawPayloads As Dictionary(Of DateTime, Payload)
        Public Property RawTicks As Concurrent.ConcurrentDictionary(Of DateTime, ITick)
        Public Property OrderDetails As Concurrent.ConcurrentDictionary(Of String, IBusinessOrder)

        Protected _cts As CancellationTokenSource
        Protected _LastTick As ITick
        Protected _APIAdapter As APIAdapter
        Protected _MaxReTries As Integer = 20
        Protected _WaitDurationOnConnectionFailure As TimeSpan = TimeSpan.FromSeconds(5)
        Protected _WaitDurationOnServiceUnavailbleFailure As TimeSpan = TimeSpan.FromSeconds(30)
        Protected _WaitDurationOnAnyFailure As TimeSpan = TimeSpan.FromSeconds(10)
        'UI Properties
        <Display(Name:="Symbol", Order:=0)>
        Public Overridable ReadOnly Property TradingSymbol As String
            Get
                If TradableInstrument IsNot Nothing Then
                    Return TradableInstrument.TradingSymbol
                Else
                    Return Nothing
                End If
            End Get
        End Property
        Private _Tradabale As Boolean
        <Display(Name:="Tradable", Order:=1)>
        Public Overridable ReadOnly Property Tradabale As Boolean
            Get
                If _LastTick IsNot Nothing Then
                    _Tradabale = _LastTick.Tradable
                    Return _Tradabale
                Else
                    Return Nothing
                End If
            End Get
        End Property
        Private _OpenPrice As Decimal
        <Display(Name:="Open", Order:=2)>
        Public Overridable ReadOnly Property OpenPrice As Decimal
            Get
                If _LastTick IsNot Nothing Then
                    _OpenPrice = _LastTick.Open
                    Return _OpenPrice
                Else
                    Return Nothing
                End If
            End Get
        End Property
        Private _HighPrice As Decimal
        <Display(Name:="High", Order:=3)>
        Public Overridable ReadOnly Property HighPrice As Decimal
            Get
                If _LastTick IsNot Nothing Then
                    _HighPrice = _LastTick.High
                    Return _HighPrice
                Else
                    Return Nothing
                End If
            End Get
        End Property
        Private _LowPrice As Decimal
        <Display(Name:="Low", Order:=4)>
        Public Overridable ReadOnly Property LowPrice As Decimal
            Get
                If _LastTick IsNot Nothing Then
                    _LowPrice = _LastTick.Low
                    Return _LowPrice
                Else
                    Return Nothing
                End If
            End Get
        End Property
        Private _ClosePrice As Decimal
        <Display(Name:="Previous Close", Order:=5)>
        Public Overridable ReadOnly Property ClosePrice As Decimal
            Get
                If _LastTick IsNot Nothing Then
                    _ClosePrice = _LastTick.Close
                    Return _ClosePrice
                Else
                    Return Nothing
                End If
            End Get
        End Property
        Private _Volume As Long
        <Display(Name:="Volume", Order:=6)>
        Public Overridable ReadOnly Property Volume As Long
            Get
                If _LastTick IsNot Nothing Then
                    _Volume = _LastTick.Volume
                    Return _Volume
                Else
                    Return Nothing
                End If
            End Get
        End Property
        Private _AveragePrice As Decimal
        <Display(Name:="Average Price", Order:=7)>
        Public Overridable ReadOnly Property AveragePrice As Decimal
            Get
                If _LastTick IsNot Nothing Then
                    _AveragePrice = _LastTick.AveragePrice
                    Return _AveragePrice
                Else
                    Return Nothing
                End If
            End Get
        End Property
        Private _LastPrice As Decimal
        <Display(Name:="Last Price", Order:=8)>
        Public Overridable ReadOnly Property LastPrice As Decimal
            Get
                If _LastTick IsNot Nothing Then
                    _LastPrice = _LastTick.LastPrice
                    Return _LastPrice
                Else
                    Return Nothing
                End If
            End Get
        End Property
        Private _Timestamp As Date?
        <Display(Name:="Timestamp", Order:=9)>
        Public Overridable ReadOnly Property Timestamp As Date?
            Get
                If _LastTick IsNot Nothing Then
                    _Timestamp = _LastTick.Timestamp
                    Return _Timestamp
                Else
                    Return Nothing
                End If
            End Get
        End Property


        Public Sub New(ByVal associatedInstrument As IInstrument, ByVal associatedParentStrategy As Strategy, ByVal canceller As CancellationTokenSource)
            TradableInstrument = associatedInstrument
            Me.ParentStrategy = associatedParentStrategy
            _cts = canceller
            RawTicks = New Concurrent.ConcurrentDictionary(Of Date, ITick)
            OrderDetails = New Concurrent.ConcurrentDictionary(Of String, IBusinessOrder)
            RawPayloads = New Dictionary(Of Date, Payload)
        End Sub
        Public MustOverride Overrides Function ToString() As String
        Public MustOverride Async Function RunDirectAsync() As Task
        Public MustOverride Async Function IsTriggerReachedAsync() As Task(Of Tuple(Of Boolean, Trigger))
        Public Overridable Async Function ProcessTickAsync(ByVal tickData As ITick) As Task
            Await Task.Delay(0).ConfigureAwait(False)
            If tickData IsNot Nothing AndAlso tickData.LastPrice <> _LastPrice Then NotifyPropertyChanged("LastPrice")
            If tickData IsNot Nothing AndAlso tickData.Tradable <> _Tradabale Then NotifyPropertyChanged("Tradable")
            If tickData IsNot Nothing AndAlso tickData.Open <> _OpenPrice Then NotifyPropertyChanged("OpenPrice")
            If tickData IsNot Nothing AndAlso tickData.High <> _HighPrice Then NotifyPropertyChanged("HighPrice")
            If tickData IsNot Nothing AndAlso tickData.Low <> _LowPrice Then NotifyPropertyChanged("LowPrice")
            If tickData IsNot Nothing AndAlso tickData.Close <> _ClosePrice Then NotifyPropertyChanged("ClosePrice")
            If tickData IsNot Nothing AndAlso tickData.Volume <> _Volume Then NotifyPropertyChanged("Volume")
            If tickData IsNot Nothing AndAlso tickData.AveragePrice <> _AveragePrice Then NotifyPropertyChanged("AveragePrice")
            If tickData IsNot Nothing AndAlso tickData.Timestamp <> _Timestamp Then NotifyPropertyChanged("Timestamp")
            If Not RawTicks.TryAdd(tickData.Timestamp, tickData) Then
                'Console.WriteLine(String.Format("Could not store:{0},{1},{2}", TradingSymbol, tickData.LastPrice, tickData.Timestamp.Value.ToLongTimeString))
            End If
        End Function
        Public Overridable Async Function ProcessOrderAsync(ByVal orderData As IBusinessOrder) As Task
            Await Task.Delay(0).ConfigureAwait(False)
            OrderDetails.AddOrUpdate(orderData.ParentOrderIdentifier, orderData, Function(key, value) orderData)
        End Function
        Public MustOverride Async Function MonitorAsync() As Task
        Protected MustOverride Function IsTriggerReceivedForPlaceOrder() As Tuple(Of Boolean, APIAdapter.TransactionType, Integer, Decimal, Decimal, Decimal)

        Protected Async Function ExecuteCommandAsync(ByVal command As ExecuteCommands, ByVal data As Object) As Task(Of Object)
            Dim ret As Object = Nothing
            Dim lastException As Exception = Nothing
            Dim allOKWithoutException As Boolean = False

            Using Waiter As New Waiter(_cts)
                AddHandler Waiter.Heartbeat, AddressOf OnHeartbeat
                AddHandler Waiter.WaitingFor, AddressOf OnWaitingFor

                For retryCtr = 1 To _MaxReTries
                    _cts.Token.ThrowIfCancellationRequested()
                    lastException = Nothing
                    While Me.ParentStrategy.ParentContoller.APIConnection Is Nothing
                        _cts.Token.ThrowIfCancellationRequested()
                        logger.Debug("Waiting for fresh token before running command:{0}", command.ToString)
                        Await Task.Delay(500).ConfigureAwait(False)
                        _cts.Token.ThrowIfCancellationRequested()
                    End While
                    _APIAdapter.SetAPIAccessToken(Me.ParentStrategy.ParentContoller.APIConnection.AccessToken)

                    logger.Debug("Firing command:{0}", command.ToString)
                    OnDocumentRetryStatus(retryCtr, _MaxReTries)
                    Try
                        _cts.Token.ThrowIfCancellationRequested()
                        Select Case command
                            Case ExecuteCommands.PlaceBOLimtMISOrder
                                Dim trigger As Tuple(Of Boolean, APIAdapter.TransactionType, Integer, Decimal, Decimal, Decimal) = IsTriggerReceivedForPlaceOrder()
                                If trigger IsNot Nothing AndAlso trigger.Item1 = True Then
                                    Dim placeOrderResponse As Dictionary(Of String, Object) = Nothing
                                    placeOrderResponse = Await _APIAdapter.PlaceBOLimitMISOrderAsync(tradeExchange:=APIAdapter.Exchange.NSE,
                                                                                                       tradingSymbol:=TradingSymbol,
                                                                                                       transaction:=trigger.Item2,
                                                                                                       quantity:=trigger.Item3,
                                                                                                       price:=trigger.Item4,
                                                                                                       squareOffValue:=trigger.Item5,
                                                                                                       stopLossValue:=trigger.Item6,
                                                                                                       tag:=Me.ToString).ConfigureAwait(False)
                                    If placeOrderResponse IsNot Nothing Then
                                        logger.Debug("Place order is completed, placeOrderResponse:{0}", Strings.JsonSerialize(placeOrderResponse))
                                        lastException = Nothing
                                        allOKWithoutException = True
                                        _cts.Token.ThrowIfCancellationRequested()
                                        ret = placeOrderResponse
                                        _cts.Token.ThrowIfCancellationRequested()
                                        Exit For
                                    Else
                                        Throw New ApplicationException(String.Format("Place order did not succeed"))
                                    End If
                                Else
                                    lastException = Nothing
                                    allOKWithoutException = True
                                    _cts.Token.ThrowIfCancellationRequested()
                                    Exit For
                                End If
                        End Select
                    Catch tex As KiteConnect.TokenException
                        logger.Error(tex)
                        lastException = tex
                        Continue For
                    Catch opx As OperationCanceledException
                        logger.Error(opx)
                        lastException = opx
                        If Not _cts.Token.IsCancellationRequested Then
                            _cts.Token.ThrowIfCancellationRequested()
                            If Not Waiter.WaitOnInternetFailure(_WaitDurationOnConnectionFailure) Then
                                'Provide required wait in case internet was already up
                                logger.Debug("HTTP->Task was cancelled without internet problem:{0}",
                                             opx.Message)
                                _cts.Token.ThrowIfCancellationRequested()
                                Waiter.SleepRequiredDuration(_WaitDurationOnAnyFailure.TotalSeconds, "Non-explicit cancellation")
                                _cts.Token.ThrowIfCancellationRequested()
                            Else
                                logger.Debug("HTTP->Task was cancelled due to internet problem:{0}, waited prescribed seconds, will now retry",
                                             opx.Message)
                                'Since internet was down, no need to consume retries
                                retryCtr -= 1
                            End If
                        End If
                    Catch hex As HttpRequestException
                        logger.Error(hex)
                        lastException = hex
                        If ExceptionExtensions.GetExceptionMessages(hex).Contains("trust relationship") Then
                            Throw New ForbiddenException(hex.Message, hex, ForbiddenException.TypeOfException.PossibleReloginRequired)
                        End If
                        _cts.Token.ThrowIfCancellationRequested()
                        If Not Waiter.WaitOnInternetFailure(_WaitDurationOnConnectionFailure) Then
                            If hex.Message.Contains("429") Or hex.Message.Contains("503") Then
                                logger.Debug("HTTP->429/503 error without internet problem:{0}",
                                             hex.Message)
                                _cts.Token.ThrowIfCancellationRequested()
                                Waiter.SleepRequiredDuration(_WaitDurationOnServiceUnavailbleFailure.TotalSeconds, "Service unavailable(429/503)")
                                _cts.Token.ThrowIfCancellationRequested()
                                'Since site service is blocked, no need to consume retries
                                retryCtr -= 1
                            ElseIf hex.Message.Contains("404") Then
                                logger.Debug("HTTP->404 error without internet problem:{0}",
                                             hex.Message)
                                _cts.Token.ThrowIfCancellationRequested()
                                'No point retrying, exit for
                                Exit For
                            Else
                                If ExceptionExtensions.IsExceptionConnectionRelated(hex) Then
                                    logger.Debug("HTTP->HttpRequestException without internet problem but of type internet related detected:{0}",
                                                 hex.Message)
                                    _cts.Token.ThrowIfCancellationRequested()
                                    Waiter.SleepRequiredDuration(_WaitDurationOnConnectionFailure.TotalSeconds, "Connection HttpRequestException")
                                    _cts.Token.ThrowIfCancellationRequested()
                                    'Since exception was internet related, no need to consume retries
                                    retryCtr -= 1
                                Else
                                    'Provide required wait in case internet was already up
                                    logger.Debug("HTTP->HttpRequestException without internet problem:{0}",
                                                 hex.Message)
                                    _cts.Token.ThrowIfCancellationRequested()
                                    Waiter.SleepRequiredDuration(_WaitDurationOnAnyFailure.TotalSeconds, "Unknown HttpRequestException:" & hex.Message)
                                    _cts.Token.ThrowIfCancellationRequested()
                                End If
                            End If
                        Else
                            logger.Debug("HTTP->HttpRequestException with internet problem:{0}, waited prescribed seconds, will now retry",
                                         hex.Message)
                            'Since internet was down, no need to consume retries
                            retryCtr -= 1
                        End If
                    Catch ex As Exception
                        logger.Error(ex)
                        lastException = ex
                        'Exit if it is a network failure check and stop retry to avoid stack overflow
                        'Need to relogin, no point retrying
                        If ExceptionExtensions.GetExceptionMessages(ex).Contains("disposed") Then
                            Throw New ForbiddenException(ex.Message, ex, ForbiddenException.TypeOfException.ExceptionInBetweenLoginProcess)
                        End If
                        _cts.Token.ThrowIfCancellationRequested()
                        If Not Waiter.WaitOnInternetFailure(_WaitDurationOnConnectionFailure) Then
                            'Provide required wait in case internet was already up
                            _cts.Token.ThrowIfCancellationRequested()
                            If ExceptionExtensions.IsExceptionConnectionRelated(ex) Then
                                logger.Debug("HTTP->Exception without internet problem but of type internet related detected:{0}",
                                             ex.Message)
                                _cts.Token.ThrowIfCancellationRequested()
                                Waiter.SleepRequiredDuration(_WaitDurationOnConnectionFailure.TotalSeconds, "Connection Exception")
                                _cts.Token.ThrowIfCancellationRequested()
                                'Since exception was internet related, no need to consume retries
                                retryCtr -= 1
                            Else
                                logger.Debug("HTTP->Exception without internet problem of unknown type detected:{0}",
                                             ex.Message)
                                _cts.Token.ThrowIfCancellationRequested()
                                Waiter.SleepRequiredDuration(_WaitDurationOnAnyFailure.TotalSeconds, "Unknown Exception")
                                _cts.Token.ThrowIfCancellationRequested()
                            End If
                        Else
                            logger.Debug("HTTP->Exception with internet problem:{0}, waited prescribed seconds, will now retry",
                                         ex.Message)
                            'Since internet was down, no need to consume retries
                            retryCtr -= 1
                        End If
                    Finally
                        OnDocumentDownloadComplete()
                    End Try
                    _cts.Token.ThrowIfCancellationRequested()
                    If ret IsNot Nothing Then
                        Exit For
                    End If
                    GC.Collect()
                Next
                RemoveHandler Waiter.Heartbeat, AddressOf OnHeartbeat
                RemoveHandler Waiter.WaitingFor, AddressOf OnWaitingFor
            End Using
            _cts.Token.ThrowIfCancellationRequested()
            If Not allOKWithoutException Then Throw lastException
            Return ret
        End Function
    End Class
End Namespace
