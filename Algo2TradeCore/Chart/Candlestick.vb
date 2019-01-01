Imports System.Threading
Imports Algo2TradeCore.Entities
Imports Algo2TradeCore.Strategies
Imports NLog

Namespace Chart
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

        Private _parentStrategyInstrument As StrategyInstrument
        Private _cts As New CancellationTokenSource
        Private _tmrRawPayloadGenerator As Timer
        Public Sub New(ByVal associatedParentStrategyInstrument As StrategyInstrument, ByVal canceller As CancellationTokenSource)
            _parentStrategyInstrument = associatedParentStrategyInstrument
            _cts = canceller
        End Sub
        Public Sub CalculateCandleFromTick(ByVal existingPayloads As Dictionary(Of DateTime, Payload), ByVal tickData As ITick)
            SyncLock Me
                If tickData Is Nothing OrElse tickData.Timestamp Is Nothing OrElse tickData.Timestamp.Value = Date.MinValue OrElse tickData.Timestamp.Value = New Date(1970, 1, 1, 5, 30, 0) Then
                    Exit Sub
                End If

                Dim lastExistingPayload As Payload = Nothing
                If existingPayloads IsNot Nothing AndAlso existingPayloads.Count > 0 Then
                    lastExistingPayload = existingPayloads.LastOrDefault.Value
                End If
                '3 condition exist
                Dim runningPayload As Payload = Nothing
                If lastExistingPayload IsNot Nothing Then
                    'Fill up any blank payload between lastPayload and current timestamp
                    Dim runningPayloadDateTime As DateTime = lastExistingPayload.SnapshotDateTime.AddMinutes(1)
                    Dim prevPayload As Payload = lastExistingPayload
                    While Utilities.Time.IsDateTimeLessTillMinutes(runningPayloadDateTime, tickData.Timestamp.Value)
                        runningPayload = New Payload
                        With runningPayload
                            .TradingSymbol = _parentStrategyInstrument.TradingSymbol
                            .OpenPrice = prevPayload.ClosePrice
                            .LowPrice = prevPayload.ClosePrice
                            .HighPrice = prevPayload.ClosePrice
                            .ClosePrice = prevPayload.ClosePrice
                            .Volume = 0
                            .DailyVolume = prevPayload.DailyVolume
                            .SnapshotDateTime = runningPayloadDateTime
                            .PreviousPayload = prevPayload
                            existingPayloads.Add(runningPayload.SnapshotDateTime, runningPayload)
                            runningPayloadDateTime = runningPayloadDateTime.AddMinutes(1)
                        End With
                        prevPayload = runningPayload
                    End While
                    'Refresh last existing paylos from the list created above
                    lastExistingPayload = existingPayloads.LastOrDefault.Value
                    'Means no gap filling in the candles was done
                    If Utilities.Time.IsDateTimeEqualTillMinutes(lastExistingPayload.SnapshotDateTime, tickData.Timestamp.Value.AddMinutes(-1)) Then
                        'Fresh candle needs to be created
                        'Print previous candle
                        'Console.WriteLine(Utilities.Strings.JsonSerialize(lastExistingPayload))
                        runningPayload = New Payload
                        With runningPayload
                            .TradingSymbol = _parentStrategyInstrument.TradingSymbol
                            .OpenPrice = tickData.LastPrice
                            .LowPrice = tickData.LastPrice
                            .HighPrice = tickData.LastPrice
                            .ClosePrice = tickData.LastPrice
                            .Volume = tickData.Volume - lastExistingPayload.DailyVolume
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
                            .Volume = tickData.Volume - .PreviousPayload.DailyVolume
                            .DailyVolume = tickData.Volume
                        End With
                    Else
                        'SHould not come here
                        Throw New NotImplementedException
                    End If
                Else
                    runningPayload = New Payload
                    With runningPayload
                        .TradingSymbol = _parentStrategyInstrument.TradingSymbol
                        .OpenPrice = tickData.LastPrice
                        .LowPrice = tickData.LastPrice
                        .HighPrice = tickData.LastPrice
                        .ClosePrice = tickData.LastPrice
                        .Volume = tickData.Volume
                        .DailyVolume = tickData.Volume
                        .SnapshotDateTime = Utilities.Time.GetDateTimeTillMinutes(tickData.Timestamp.Value)
                        .PreviousPayload = Nothing
                    End With
                End If
                existingPayloads.Add(runningPayload.SnapshotDateTime, runningPayload)
            End SyncLock
        End Sub
        Public Overrides Function ToString() As String
            Return Me.GetType.ToString
        End Function
    End Class
End Namespace
