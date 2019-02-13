Imports System.Threading
Imports Algo2TradeCore.Entities
Imports Algo2TradeCore.Controller
Imports NLog
Namespace ChartHandler.ChartStyle
    Public Class CandleStickChart
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
            RaiseEvent HeartbeatEx(msg, source)
        End Sub
        Protected Overridable Sub OnWaitingForEx(ByVal elapsedSecs As Integer, ByVal totalSecs As Integer, ByVal msg As String, ByVal source As List(Of Object))
            If source IsNot Nothing Then source = New List(Of Object)
            source.Add(Me)
            RaiseEvent WaitingForEx(elapsedSecs, totalSecs, msg, source)
        End Sub
        Protected Overridable Sub OnDocumentDownloadComplete()
            RaiseEvent DocumentDownloadCompleteEx(New List(Of Object) From {Me})
        End Sub
        Protected Overridable Sub OnDocumentRetryStatus(ByVal currentTry As Integer, ByVal totalTries As Integer)
            RaiseEvent DocumentRetryStatusEx(currentTry, totalTries, New List(Of Object) From {Me})
        End Sub
        Protected Overridable Sub OnHeartbeat(ByVal msg As String)
            RaiseEvent HeartbeatEx(msg, New List(Of Object) From {Me})
        End Sub
        Protected Overridable Sub OnWaitingFor(ByVal elapsedSecs As Integer, ByVal totalSecs As Integer, ByVal msg As String)
            RaiseEvent WaitingForEx(elapsedSecs, totalSecs, msg, New List(Of Object) From {Me})
        End Sub
#End Region

#Region "Logging and Status Progress"
        Public Shared logger As Logger = LogManager.GetCurrentClassLogger
#End Region

        Public Property ParentController As APIStrategyController
        Private _parentInstrument As IInstrument
        Private _cts As New CancellationTokenSource
        Private _lock As Integer
        Private _tickLock As Integer
        Public Sub New(ByVal associatedParentController As APIStrategyController, ByVal assoicatedParentInstrument As IInstrument, ByVal canceller As CancellationTokenSource)
            Me.ParentController = associatedParentController
            _parentInstrument = assoicatedParentInstrument
            _cts = canceller
        End Sub
        Public Async Function ProcessHistoricalJSONToCandleStickAsync(ByVal historicalCandlesJSONDict As Dictionary(Of String, Object)) As Task
            'Exit Function
            Try
                While _lock > 0
                    Await Task.Delay(10).ConfigureAwait(False)
                End While
                Interlocked.Increment(_lock)
                'Debug.WriteLine(String.Format("Process Historical before. Time:{0}, Lock:{1}", Now, _lock))
                If historicalCandlesJSONDict.ContainsKey("data") Then
                    Dim historicalCandlesDict As Dictionary(Of String, Object) = historicalCandlesJSONDict("data")
                    If historicalCandlesDict.ContainsKey("candles") Then
                        Dim historicalCandles As ArrayList = historicalCandlesDict("candles")
                        Dim previousCandlePayload As OHLCPayload = Nothing
                        For Each historicalCandle In historicalCandles
                            Dim runningPayload As OHLCPayload = New OHLCPayload(IPayload.PayloadSource.Historical)
                            With runningPayload
                                .SnapshotDateTime = Utilities.Time.GetDateTimeTillMinutes(historicalCandle(0))
                                .TradingSymbol = _parentInstrument.TradingSymbol
                                .OpenPrice = historicalCandle(1)
                                .HighPrice = historicalCandle(2)
                                .LowPrice = historicalCandle(3)
                                .ClosePrice = historicalCandle(4)
                                .Volume = historicalCandle(5)
                                If previousCandlePayload IsNot Nothing AndAlso
                                    .SnapshotDateTime.Date = previousCandlePayload.SnapshotDateTime.Date Then
                                    .DailyVolume = .Volume + previousCandlePayload.DailyVolume
                                Else
                                    .DailyVolume = .Volume
                                End If
                                .PreviousPayload = previousCandlePayload
                            End With
                            previousCandlePayload = runningPayload
                            'The below will add or update items inside the dictionary
                            '_parentInstrument.RawPayloads(runningPayload.SnapshotDateTime) = runningPayload
                        Next
                    End If
                End If
            Catch ex As Exception
                logger.Error("Strategy Instrument:{0}, error:{1}", Me.ToString, ex.ToString)
                Me.ParentController.OrphanException = ex
            Finally
                Interlocked.Decrement(_lock)
                'Debug.WriteLine(String.Format("Process Historical after. Time:{0}, Lock:{1}", Now, _lock))
            End Try
        End Function
        'Public Async Function FIFOProcessTickToCandleStickAsync() As Task
        '    If _lock = 0 Then
        '        Try
        '            Interlocked.Increment(_lock)
        '            'Debug.WriteLine(String.Format("Process Tick before. Time:{0}, Lock:{1}", Now, _lock))
        '            Await Task.Delay(0).ConfigureAwait(False)
        '            If _parentInstrument.RawTicks IsNot Nothing AndAlso _parentInstrument.RawTicks.Count > 0 Then
        '                Dim FIFOTicks As IEnumerable(Of KeyValuePair(Of Date, ITick)) = _parentInstrument.RawTicks.Where(Function(y)
        '                                                                                                                     Return y.Key = _parentInstrument.RawTicks.Min(Function(x)
        '                                                                                                                                                                       Return x.Key
        '                                                                                                                                                                   End Function)
        '                                                                                                                 End Function)
        '                Dim removedTick As ITick = Nothing
        '                For Each runningFIFOTick In FIFOTicks
        '                    CalculateCandleStickFromTick(_parentInstrument.RawTickPayloads, runningFIFOTick.Value)
        '                    'Console.WriteLine(Utilities.Strings.JsonSerialize(z1))
        '                    _parentInstrument.RawTicks.TryRemove(runningFIFOTick.Key, removedTick)
        '                Next

        '                'Now insert this candle into the main payload
        '                If _parentInstrument.RawTickPayloads IsNot Nothing AndAlso _parentInstrument.RawTickPayloads.Count > 0 Then
        '                    For Each runningRawTickPayload In _parentInstrument.RawTickPayloads
        '                        If _parentInstrument.RawPayloads.ContainsKey(runningRawTickPayload.Key) Then
        '                            If _parentInstrument.RawPayloads(runningRawTickPayload.Key).PayloadGeneratedBy = IPayload.PayloadSource.Tick Then
        '                                _parentInstrument.RawPayloads(runningRawTickPayload.Key) = runningRawTickPayload.Value
        '                            End If
        '                        Else
        '                            _parentInstrument.RawPayloads.Add(runningRawTickPayload.Key, runningRawTickPayload.Value)
        '                        End If
        '                    Next
        '                    'Push to all consumers
        '                    If _parentInstrument.FirstLevelConsumers IsNot Nothing AndAlso _parentInstrument.FirstLevelConsumers.Count > 0 Then
        '                        For Each runningFirstLevelConsumer In _parentInstrument.FirstLevelConsumers
        '                            Await runningFirstLevelConsumer.StrategyXMinutePayloadsSource.PopulateFromOHLCAsync(_parentInstrument.RawPayloads(_parentInstrument.RawTickPayloads.LastOrDefault.Key)).ConfigureAwait(False)
        '                        Next
        '                    End If

        '                    'Remove all items except the last one
        '                    Dim removableRawTickPayloads As IEnumerable(Of KeyValuePair(Of Date, OHLCPayload)) =
        '                        _parentInstrument.RawTickPayloads.Where(Function(y)
        '                                                                    Return y.Key < _parentInstrument.RawTickPayloads.Max(Function(x)
        '                                                                                                                             Return x.Key
        '                                                                                                                         End Function)
        '                                                                End Function)
        '                    Dim removedRawTickPayload As OHLCPayload = Nothing
        '                    For Each removableRawTickPayload In removableRawTickPayloads
        '                        _parentInstrument.RawTickPayloads.TryRemove(removableRawTickPayload.Key, removedRawTickPayload)
        '                    Next
        '                End If
        '            End If
        '        Catch ex As Exception
        '            logger.Error("Strategy Instrument:{0}, error:{1}", Me.ToString, ex.ToString)
        '            Me.ParentController.OrphanException = ex
        '        Finally
        '            Interlocked.Decrement(_lock)
        '            'Debug.WriteLine(String.Format("Process Tick after. Time:{0}, Lock:{1}", Now, _lock))
        '        End Try
        '    End If
        'End Function

        Public Async Function CalculateCandleStickFromTickAsync(ByVal existingPayloads As SortedDictionary(Of DateTime, OHLCPayload), ByVal tickData As ITick) As Task
            If tickData Is Nothing OrElse tickData.Timestamp Is Nothing OrElse tickData.Timestamp.Value = Date.MinValue OrElse tickData.Timestamp.Value = New Date(1970, 1, 1, 5, 30, 0) Then
                Exit Function
            End If

            Try
                While Interlocked.Read(_tickLock) > 0
                    Await Task.Delay(10).ConfigureAwait(False)
                End While
                Interlocked.Increment(_tickLock)

                Dim lastExistingPayload As OHLCPayload = Nothing
                If existingPayloads IsNot Nothing AndAlso existingPayloads.Count > 0 Then
                    Dim lastExistingPayloads As IEnumerable(Of KeyValuePair(Of Date, OHLCPayload)) =
                        _parentInstrument.RawPayloads.Where(Function(y)
                                                                Return Utilities.Time.IsDateTimeEqualTillMinutes(y.Key, tickData.Timestamp.Value)
                                                            End Function)
                    If lastExistingPayloads IsNot Nothing AndAlso lastExistingPayloads.Count > 0 Then lastExistingPayload = lastExistingPayloads.LastOrDefault.Value
                End If
                Dim runningPayload As OHLCPayload = Nothing
                If lastExistingPayload IsNot Nothing Then
                    With lastExistingPayload
                        .HighPrice = Math.Max(lastExistingPayload.HighPrice, tickData.LastPrice)
                        .LowPrice = Math.Min(lastExistingPayload.LowPrice, tickData.LastPrice)
                        .ClosePrice = tickData.LastPrice
                        If .PreviousPayload IsNot Nothing Then
                            If .PreviousPayload.SnapshotDateTime.Date = tickData.Timestamp.Value.Date Then
                                .Volume = tickData.Volume - .PreviousPayload.DailyVolume
                            Else
                                .Volume = tickData.Volume
                            End If
                        Else
                            .Volume = tickData.Volume
                        End If
                        .DailyVolume = tickData.Volume
                        .NumberOfTicks += 1
                    End With
                Else
                    'Fresh candle needs to be created
                    Dim previousCandle As OHLCPayload = Nothing
                    Dim previousCandles As IEnumerable(Of KeyValuePair(Of Date, OHLCPayload)) =
                        _parentInstrument.RawPayloads.Where(Function(y)
                                                                Return y.Key < tickData.Timestamp.Value
                                                            End Function)
                    If previousCandles IsNot Nothing AndAlso previousCandles.Count > 0 Then
                        previousCandle = previousCandles.LastOrDefault.Value
                        'Print previous candle
                        Debug.WriteLine(previousCandle)
                    End If
                    runningPayload = New OHLCPayload(IPayload.PayloadSource.Tick)
                    With runningPayload
                        .TradingSymbol = _parentInstrument.TradingSymbol
                        .OpenPrice = tickData.LastPrice
                        .HighPrice = tickData.LastPrice
                        .LowPrice = tickData.LastPrice
                        .ClosePrice = tickData.LastPrice
                        .Volume = tickData.Volume
                        .DailyVolume = tickData.Volume
                        .SnapshotDateTime = Utilities.Time.GetDateTimeTillMinutes(tickData.Timestamp.Value)
                        .PreviousPayload = previousCandle
                        .NumberOfTicks = 1
                    End With
                End If
                If runningPayload IsNot Nothing Then existingPayloads.Add(runningPayload.SnapshotDateTime, runningPayload)
            Catch ex As Exception
                logger.Error("Strategy Instrument:{0}, error:{1}", Me.ToString, ex.ToString)
                Me.ParentController.OrphanException = ex
            Finally
                Interlocked.Decrement(_tickLock)
                If _tickLock <> 0 Then Throw New ApplicationException("Check why lock is not released")
                'Debug.WriteLine(String.Format("Process Historical after. Time:{0}, Lock:{1}", Now, _lock))
            End Try
        End Function
        Public Overrides Function ToString() As String
            Return Me.GetType.ToString
        End Function
    End Class
End Namespace
