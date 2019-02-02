Imports System.Net
Imports System.Threading
Imports Algo2TradeCore.Controller
Imports KiteConnect
Imports NLog
Imports Utilities.Network

Namespace Adapter
    Public Class ZerodhaHistoricalDataFetcher
        Inherits APIHistoricalDataFetcher

#Region "Logging and Status Progress"
        Public Shared Shadows logger As Logger = LogManager.GetCurrentClassLogger
#End Region
#Region "Events/Event handlers specific to the derived class"
        Public Event FetcherCandlesAsync(ByVal historicalCandlesJSONDict As Dictionary(Of String, Object))
        Public Event FetcherError(ByVal msg As String)
        'The below functions are needed to allow the derived classes to raise the above two events
        Protected Overridable Sub OnFetcherCandlesAsync(ByVal historicalCandlesJSONDict As Dictionary(Of String, Object))
            RaiseEvent FetcherCandlesAsync(historicalCandlesJSONDict)
        End Sub
        Protected Overridable Sub OnFetcherError(ByVal msg As String)
            RaiseEvent FetcherError(msg)
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
        End Sub
        Public Overrides Async Function ConnectFetcherAsync() As Task
            logger.Debug("{0}->ConnectTickerAsync, parameters:Nothing", Me.ToString)
            _cts.Token.ThrowIfCancellationRequested()
            Await Task.Delay(0).ConfigureAwait(False)
            Dim currentZerodhaStrategyController As ZerodhaStrategyController = CType(ParentController, ZerodhaStrategyController)

            RemoveHandler Me.FetcherCandlesAsync, AddressOf currentZerodhaStrategyController.OnFetcherCandlesAsync
            RemoveHandler Me.FetcherError, AddressOf currentZerodhaStrategyController.OnFetcherError
            _cts.Token.ThrowIfCancellationRequested()
            AddHandler Me.FetcherCandlesAsync, AddressOf currentZerodhaStrategyController.OnFetcherCandlesAsync
            AddHandler Me.FetcherError, AddressOf currentZerodhaStrategyController.OnFetcherError
        End Function

        Public Overrides Async Function StartPollingAsync() As Task
            Try
                _stopPollRunning = False
                _isPollRunning = False
                ServicePointManager.DefaultConnectionLimit = 10

                While True
                    If _stopPollRunning Then
                        _isPollRunning = False
                        Exit While
                    End If
                    _isPollRunning = True
                    _cts.Token.ThrowIfCancellationRequested()
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
                            tasks.Add(Task.Run(AddressOf specificInstrumentHistoricalDataFetcher.GetHistoricalCandleStick))
                        Next
                        OnHeartbeat("Polling historical candles")
                        Await Task.WhenAll(tasks).ConfigureAwait(False)
                    End If
                    Await Task.Delay(6000)
                End While
            Catch ex As Exception
                Throw ex
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

        Public Async Function GetHistoricalCandleStick() As Task(Of String)
            If _instrumentIdentifer Is Nothing Then Exit Function
            _cts.Token.ThrowIfCancellationRequested()

            HttpBrowser.KillCookies()
            _cts.Token.ThrowIfCancellationRequested()

            Using browser As New HttpBrowser(Nothing, DecompressionMethods.GZip, TimeSpan.FromSeconds(45), _cts)
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
                logger.Debug("Opening historical candle page, GetLoginURL:{0}, headers:{1}", historicalDataURL, Utils.JsonSerialize(headers))

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
                        OnFetcherError(errorMessage)
                    Else
                        OnFetcherCandlesAsync(tempRet.Item2)
                    End If
                Else
                    Throw New ApplicationException("Fetching of historical data failed as no return detected")
                End If
                RemoveHandler browser.DocumentDownloadComplete, AddressOf OnDocumentDownloadComplete
                RemoveHandler browser.DocumentRetryStatus, AddressOf OnDocumentRetryStatus
                RemoveHandler browser.Heartbeat, AddressOf OnHeartbeat
                RemoveHandler browser.WaitingFor, AddressOf OnWaitingFor
            End Using
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
            'Intentionally no _cts.Token.ThrowIfCancellationRequested() since we need to close the ticker when cancellation is done
            While IsConnected()
                _stopPollRunning = True
                Await Task.Delay(100).ConfigureAwait(False)
            End While
        End Function

    End Class
End Namespace