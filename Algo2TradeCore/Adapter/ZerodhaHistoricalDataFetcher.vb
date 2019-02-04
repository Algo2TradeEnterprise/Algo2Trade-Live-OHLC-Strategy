Imports System.Net
Imports System.Threading
Imports Algo2TradeCore.Controller
Imports KiteConnect
Imports NLog
Imports Utilities.Network

Namespace Adapter
    Public Class ZerodhaHistoricalDataFetcher
        Inherits APIHistoricalDataFetcher
        Implements IDisposable

#Region "Logging and Status Progress"
        Public Shared Shadows logger As Logger = LogManager.GetCurrentClassLogger
#End Region

#Region "Events/Event handlers specific to the derived class"
        Public Event FetcherCandlesAsync(ByVal instrumentIdentifier As String, ByVal historicalCandlesJSONDict As Dictionary(Of String, Object))
        Public Event FetcherError(ByVal instrumentIdentifier As String, ByVal msg As String)
        'The below functions are needed to allow the derived classes to raise the above two events
        Protected Overridable Sub OnFetcherCandlesAsync(ByVal instrumentIdentifier As String, ByVal historicalCandlesJSONDict As Dictionary(Of String, Object))
            RaiseEvent FetcherCandlesAsync(instrumentIdentifier, historicalCandlesJSONDict)
        End Sub
        Protected Overridable Sub OnFetcherError(ByVal instrumentIdentifier As String, ByVal msg As String)
            RaiseEvent FetcherError(instrumentIdentifier, msg)
        End Sub
#End Region

        Private ZERODHA_HISTORICAL_URL = "https://kitecharts-aws.zerodha.com/api/chart/{0}/minute?api_key=kitefront&access_token=K&from={1}&to={2}"

        Public Sub New(ByVal associatedParentController As APIStrategyController,
                       ByVal canceller As CancellationTokenSource)
            MyBase.New(associatedParentController, canceller)
        End Sub
        Public Sub New(ByVal associatedParentController As APIStrategyController,
                       ByVal instrumentIdentifier As String,
                       ByVal canceller As CancellationTokenSource)
            MyBase.New(associatedParentController, instrumentIdentifier, canceller)
            Dim currentZerodhaStrategyController As ZerodhaStrategyController = CType(ParentController, ZerodhaStrategyController)
            AddHandler Me.FetcherCandlesAsync, AddressOf currentZerodhaStrategyController.OnFetcherCandlesAsync
            AddHandler Me.FetcherError, AddressOf currentZerodhaStrategyController.OnFetcherError
        End Sub
        Public Overrides Async Function ConnectFetcherAsync() As Task
            logger.Debug("{0}->ConnectTickerAsync, parameters:Nothing", Me.ToString)
            _cts.Token.ThrowIfCancellationRequested()
            Await Task.Delay(0).ConfigureAwait(False)
            'Dim currentZerodhaStrategyController As ZerodhaStrategyController = CType(ParentController, ZerodhaStrategyController)

            'RemoveHandler Me.FetcherCandlesAsync, AddressOf currentZerodhaStrategyController.OnFetcherCandlesAsync
            'RemoveHandler Me.FetcherError, AddressOf currentZerodhaStrategyController.OnFetcherError
            '_cts.Token.ThrowIfCancellationRequested()
            'AddHandler Me.FetcherCandlesAsync, AddressOf currentZerodhaStrategyController.OnFetcherCandlesAsync
            'AddHandler Me.FetcherError, AddressOf currentZerodhaStrategyController.OnFetcherError
        End Function

        Public Overrides Async Function StartPollingAsync() As Task
            Try
                _stopPollRunning = False
                _isPollRunning = False
                ServicePointManager.DefaultConnectionLimit = 10
                Dim lastTimeWhenDone As Date = Date.MinValue
                Dim nextTimeToDo As Date = Date.MinValue
                While True
                    If _stopPollRunning Then
                        _isPollRunning = False
                        Exit While
                    End If
                    _isPollRunning = True
                    _cts.Token.ThrowIfCancellationRequested()
                    lastTimeWhenDone = Now
                    If _subscribedInstruments IsNot Nothing AndAlso _subscribedInstruments.Count > 0 Then
                        Dim tasks As New List(Of Task)()
                        Dim specificInstrumentsHistoricalDataFetcher As List(Of ZerodhaHistoricalDataFetcher) = Nothing
                        For Each subscribedInstrument In _subscribedInstruments
                            _cts.Token.ThrowIfCancellationRequested()
                            If specificInstrumentsHistoricalDataFetcher Is Nothing Then specificInstrumentsHistoricalDataFetcher = New List(Of ZerodhaHistoricalDataFetcher)
                            specificInstrumentsHistoricalDataFetcher.Add(New ZerodhaHistoricalDataFetcher(Me.ParentController, subscribedInstrument, Me._cts))
                        Next
                        For Each specificInstrumentHistoricalDataFetcher In specificInstrumentsHistoricalDataFetcher
                            _cts.Token.ThrowIfCancellationRequested()
                            tasks.Add(Task.Run(AddressOf specificInstrumentHistoricalDataFetcher.GetHistoricalCandleStick, _cts.Token))
                        Next
                        OnHeartbeat("Polling historical candles")
                        Await Task.WhenAll(tasks).ConfigureAwait(False)
                        'Cleanup
                        If Me.ParentController.APIConnection Is Nothing Then Exit While

                        For Each subscribedInstrument In _subscribedInstruments
                            _cts.Token.ThrowIfCancellationRequested()
                            subscribedInstrument = Nothing
                        Next
                        For Each specificInstrumentHistoricalDataFetcher In specificInstrumentsHistoricalDataFetcher
                            _cts.Token.ThrowIfCancellationRequested()
                            specificInstrumentHistoricalDataFetcher.Dispose()
                            specificInstrumentHistoricalDataFetcher = Nothing
                        Next
                        For Each task In tasks
                            _cts.Token.ThrowIfCancellationRequested()
                            task.Dispose()
                            task = Nothing
                        Next
                        System.GC.AddMemoryPressure(1024 * 1024)
                        System.GC.Collect()
                        specificInstrumentsHistoricalDataFetcher.Clear()
                        specificInstrumentsHistoricalDataFetcher.TrimExcess()
                        specificInstrumentsHistoricalDataFetcher = Nothing
                        tasks.Clear()
                        tasks.TrimExcess()
                        tasks = Nothing
                    End If
                    _cts.Token.ThrowIfCancellationRequested()

                    If Utilities.Time.IsDateTimeEqualTillMinutes(lastTimeWhenDone, nextTimeToDo) Then
                        'Already done for this minute
                        lastTimeWhenDone = lastTimeWhenDone.AddMinutes(1)
                        nextTimeToDo = New Date(lastTimeWhenDone.Year, lastTimeWhenDone.Month, lastTimeWhenDone.Day, lastTimeWhenDone.Hour, lastTimeWhenDone.Minute, 5)
                    Else
                        nextTimeToDo = New Date(lastTimeWhenDone.Year, lastTimeWhenDone.Month, lastTimeWhenDone.Day, lastTimeWhenDone.Hour, lastTimeWhenDone.Minute, 5)
                    End If
                    Console.WriteLine(nextTimeToDo.ToLongTimeString)

                    While Now < nextTimeToDo
                        _cts.Token.ThrowIfCancellationRequested()
                        If _stopPollRunning Then
                            _isPollRunning = False
                            Exit While
                        End If
                        Await Task.Delay(1000)
                    End While
                End While
            Catch ex As Exception
                logger.Error("Instrument Identifier:{0}, error:{1}", _instrumentIdentifer, ex.ToString)
                Me.ParentController.OrphanException = ex
            Finally
                _isPollRunning = False
            End Try
        End Function
        Public Overrides Async Function SubscribeAsync(ByVal instrumentIdentifiers As List(Of String)) As Task
            logger.Debug("{0}->SubscribeAsync, instrumentIdentifiers:{1}", Me.ToString, Utils.JsonSerialize(instrumentIdentifiers))
            _cts.Token.ThrowIfCancellationRequested()
            Await Task.Delay(0).ConfigureAwait(False)
            If _subscribedInstruments Is Nothing Then _subscribedInstruments = New List(Of String)
            For Each runningInstrumentIdentifier In instrumentIdentifiers
                _cts.Token.ThrowIfCancellationRequested()
                If _subscribedInstruments.Contains(runningInstrumentIdentifier) Then Continue For
                _subscribedInstruments.Add(runningInstrumentIdentifier)
            Next
            If _subscribedInstruments Is Nothing OrElse _subscribedInstruments.Count = 0 Then
                OnHeartbeat("No instruments were subscribed for historical as they may be already subscribed")
                logger.Error("No tokens to subscribe for historical")
            Else
                OnHeartbeat(String.Format("Subscribed:{0} instruments for historical", _subscribedInstruments.Count))
                StartPollingAsync()
            End If
        End Function

        Protected Overrides Async Function GetHistoricalCandleStick() As Task
            Try
                If _instrumentIdentifer Is Nothing Then Exit Function
                _cts.Token.ThrowIfCancellationRequested()

                HttpBrowser.KillCookies()
                _cts.Token.ThrowIfCancellationRequested()

                Using browser As New HttpBrowser(Nothing, DecompressionMethods.GZip, TimeSpan.FromSeconds(30), _cts)
                    AddHandler browser.DocumentDownloadComplete, AddressOf OnDocumentDownloadComplete
                    AddHandler browser.DocumentRetryStatus, AddressOf OnDocumentRetryStatus
                    AddHandler browser.Heartbeat, AddressOf OnHeartbeat
                    AddHandler browser.WaitingFor, AddressOf OnWaitingFor

                    'Keep the below headers constant for all login browser operations
                    browser.UserAgent = GetRandomUserAgent()
                    browser.KeepAlive = True

                    Dim headers As New Dictionary(Of String, String)
                    headers.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8")
                    'headers.Add("Accept-Encoding", "gzip, deflate, br")
                    headers.Add("Accept-Encoding", "*")
                    headers.Add("Accept-Language", "en-US,en;q=0.8")
                    headers.Add("Cookie", "session=c")

                    Dim historicalDataURL As String = String.Format(ZERODHA_HISTORICAL_URL,
                                                                    _instrumentIdentifer,
                                                                    Now.AddDays(-27).ToString("yyyy-MM-dd"),
                                                                    Now.ToString("yyyy-MM-dd"))
                    logger.Debug("Opening historical candle page, GetHistoricalDataURL:{0}, headers:{1}", historicalDataURL, Utils.JsonSerialize(headers))
                    _cts.Token.ThrowIfCancellationRequested()
                    Dim tempRet As Tuple(Of Uri, Object) = Await browser.NonPOSTRequestAsync(historicalDataURL,
                                                                                          Http.HttpMethod.Get,
                                                                                          Nothing,
                                                                                          True,
                                                                                          headers,
                                                                                          False).ConfigureAwait(False)

                    _cts.Token.ThrowIfCancellationRequested()
                    If tempRet IsNot Nothing AndAlso tempRet.Item2 IsNot Nothing AndAlso tempRet.Item2.GetType Is GetType(Dictionary(Of String, Object)) Then
                        Dim errorMessage As String = ParentController.GetErrorResponse(tempRet.Item2)
                        If errorMessage IsNot Nothing Then
                            OnFetcherError(_instrumentIdentifer, errorMessage)
                        Else
                            OnFetcherCandlesAsync(_instrumentIdentifer, tempRet.Item2)
                        End If
                    Else
                        Throw New ApplicationException("Fetching of historical data failed as no return detected")
                    End If
                    RemoveHandler browser.DocumentDownloadComplete, AddressOf OnDocumentDownloadComplete
                    RemoveHandler browser.DocumentRetryStatus, AddressOf OnDocumentRetryStatus
                    RemoveHandler browser.Heartbeat, AddressOf OnHeartbeat
                    RemoveHandler browser.WaitingFor, AddressOf OnWaitingFor
                End Using
            Catch ex As Exception
                logger.Error("Instrument Identifier:{0}, error:{1}", _instrumentIdentifer, ex.ToString)
                Throw ex
            End Try
        End Function

        Public Overrides Function ToString() As String
            Return Me.GetType.ToString
        End Function
        Public Overrides Sub ClearLocalUniqueSubscriptionList()
            _subscribedInstruments = Nothing
        End Sub
        Public Overrides Function IsConnected() As Boolean
            Return _isPollRunning
        End Function
        Public Overrides Async Function CloseFetcherIfConnectedAsync() As Task
            'Intentionally no _cts.Token.ThrowIfCancellationRequested() since we need to close the fetcher when cancellation is done
            While IsConnected()
                _stopPollRunning = True
                Await Task.Delay(100).ConfigureAwait(False)
            End While
        End Function

#Region "IDisposable Support"
        Private disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not disposedValue Then
                If disposing Then
                    ' TODO: dispose managed state (managed objects).
                    Dim currentZerodhaStrategyController As ZerodhaStrategyController = CType(ParentController, ZerodhaStrategyController)

                    RemoveHandler Me.FetcherCandlesAsync, AddressOf currentZerodhaStrategyController.OnFetcherCandlesAsync
                    RemoveHandler Me.FetcherError, AddressOf currentZerodhaStrategyController.OnFetcherError
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
End Namespace