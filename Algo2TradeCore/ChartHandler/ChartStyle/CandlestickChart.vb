Imports System.Threading
Imports Algo2TradeCore.Entities
Imports Algo2TradeCore.Strategies
Imports NLog
Namespace ChartHandler.ChartStyle
    Public Class Candlestick
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

        Private _parentInstrument As IInstrument
        Private _cts As New CancellationTokenSource
        Public Sub New(ByVal assoicatedParentInstrument As IInstrument, ByVal canceller As CancellationTokenSource)
            _parentInstrument = assoicatedParentInstrument
            _cts = canceller
        End Sub

        Public Async Function FIFOProcessTickToCandleStickAsync() As Task
            Await Task.Delay(0).ConfigureAwait(False)
            If _parentInstrument.RawTicks IsNot Nothing AndAlso _parentInstrument.RawTicks.Count > 0 Then
                Dim FIFOTicks As IEnumerable(Of KeyValuePair(Of Date, ITick)) = _parentInstrument.RawTicks.Where(Function(y)
                                                                                                                     Return y.Key = _parentInstrument.RawTicks.Min(Function(x)
                                                                                                                                                                       Return x.Key
                                                                                                                                                                   End Function)
                                                                                                                 End Function)
                Dim removedTick As ITick
                For Each runningFIFOTick In FIFOTicks
                    CalculateCandleStickFromTick(_parentInstrument.RawPayloads, runningFIFOTick.Value)
                    'Console.WriteLine(Utilities.Strings.JsonSerialize(z1))
                    _parentInstrument.RawTicks.TryRemove(runningFIFOTick.Key, removedTick)
                Next
            End If
        End Function

        Private Sub CalculateCandleStickFromTick(ByVal existingPayloads As Dictionary(Of DateTime, OHLCPayload), ByVal tickData As ITick)
            SyncLock Me
                If tickData Is Nothing OrElse tickData.Timestamp Is Nothing OrElse tickData.Timestamp.Value = Date.MinValue OrElse tickData.Timestamp.Value = New Date(1970, 1, 1, 5, 30, 0) Then
                    Exit Sub
                End If

                Dim lastExistingPayload As OHLCPayload = Nothing
                If existingPayloads IsNot Nothing AndAlso existingPayloads.Count > 0 Then
                    lastExistingPayload = existingPayloads.LastOrDefault.Value
                End If
                '3 condition exist
                Dim runningPayload As OHLCPayload = Nothing
                If lastExistingPayload IsNot Nothing Then
                    If Not Utilities.Time.IsDateTimeEqualTillMinutes(lastExistingPayload.SnapshotDateTime, tickData.Timestamp.Value) Then
                        'Fresh candle needs to be created
                        'Print previous candle
                        'Console.WriteLine(Utilities.Strings.JsonSerialize(lastExistingPayload))
                        runningPayload = New OHLCPayload
                        With runningPayload
                            .TradingSymbol = _parentInstrument.TradingSymbol
                            .OpenPrice = tickData.LastPrice
                            .HighPrice = tickData.LastPrice
                            .LowPrice = tickData.LastPrice
                            .ClosePrice = tickData.LastPrice
                            If lastExistingPayload.SnapshotDateTime.Date = tickData.Timestamp.Value.Date Then
                                .Volume = tickData.Volume - lastExistingPayload.DailyVolume
                            Else
                                .Volume = tickData.Volume
                            End If
                            .DailyVolume = tickData.Volume
                            .SnapshotDateTime = Utilities.Time.GetDateTimeTillMinutes(tickData.Timestamp.Value)
                            .PreviousPayload = lastExistingPayload
                        End With
                    ElseIf Utilities.Time.IsDateTimeEqualTillMinutes(lastExistingPayload.SnapshotDateTime, tickData.Timestamp.Value) Then
                        'Existing candle needs to be altered
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
                        End With
                    Else
                        'SHould not come here
                        Throw New NotImplementedException("Why have you come here")
                    End If
                Else
                    runningPayload = New OHLCPayload
                    With runningPayload
                        .TradingSymbol = _parentInstrument.TradingSymbol
                        .OpenPrice = tickData.LastPrice
                        .HighPrice = tickData.LastPrice
                        .LowPrice = tickData.LastPrice
                        .ClosePrice = tickData.LastPrice
                        .Volume = tickData.Volume
                        .DailyVolume = tickData.Volume
                        .SnapshotDateTime = Utilities.Time.GetDateTimeTillMinutes(tickData.Timestamp.Value)
                        .PreviousPayload = lastExistingPayload
                    End With
                End If
                If runningPayload IsNot Nothing Then existingPayloads.Add(runningPayload.SnapshotDateTime, runningPayload)
            End SyncLock
        End Sub
        Public Overrides Function ToString() As String
            Return Me.GetType.ToString
        End Function
    End Class
End Namespace
