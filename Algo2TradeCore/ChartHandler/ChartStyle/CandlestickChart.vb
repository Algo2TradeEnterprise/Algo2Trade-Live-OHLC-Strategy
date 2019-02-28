﻿Imports System.Threading
Imports Algo2TradeCore.Entities
Imports Algo2TradeCore.Controller
Imports NLog
Imports Algo2TradeCore.Strategies
Namespace ChartHandler.ChartStyle

    Public Class CandleStickChart
        Inherits Chart
#Region "Logging and Status Progress"
        Public Shared Shadows logger As Logger = LogManager.GetCurrentClassLogger
#End Region


        Public Sub New(ByVal associatedParentController As APIStrategyController,
                       ByVal assoicatedParentInstrument As IInstrument,
                       ByVal associatedStrategyInstruments As IEnumerable(Of StrategyInstrument),
                       ByVal canceller As CancellationTokenSource)
            MyBase.New(associatedParentController, assoicatedParentInstrument, associatedStrategyInstruments, canceller)
        End Sub
        Public Overrides Async Function GetChartFromHistoricalAsync(ByVal historicalCandlesJSONDict As Dictionary(Of String, Object)) As Task
            'Exit Function
            'logger.Debug("{0}->GetChartFromHistoricalAsync, parameters:{1}", Me.ToString, Utilities.Strings.JsonSerialize(historicalCandlesJSONDict))
            Try
                While Interlocked.Read(_historicalLock) > 0
                    Await Task.Delay(10).ConfigureAwait(False)
                End While
                Interlocked.Increment(_historicalLock)
                'Debug.WriteLine(String.Format("Process Historical before. Time:{0}, Lock:{1}", Now, _lock))
                If historicalCandlesJSONDict.ContainsKey("data") Then
                    Dim historicalCandlesDict As Dictionary(Of String, Object) = historicalCandlesJSONDict("data")
                    If historicalCandlesDict.ContainsKey("candles") AndAlso historicalCandlesDict("candles").count > 0 Then
                        Dim historicalCandles As ArrayList = historicalCandlesDict("candles")
                        Dim previousCandlePayload As OHLCPayload = Nothing
                        If _parentInstrument.RawPayloads IsNot Nothing AndAlso _parentInstrument.RawPayloads.Count > 0 Then
                            Dim previousCandles As IEnumerable(Of KeyValuePair(Of Date, OHLCPayload)) =
                           _parentInstrument.RawPayloads.Where(Function(y)
                                                                   Return y.Key < Utilities.Time.GetDateTimeTillMinutes(historicalCandles(0)(0))
                                                               End Function)

                            If previousCandles IsNot Nothing AndAlso previousCandles.Count > 0 Then
                                previousCandlePayload = previousCandles.OrderByDescending(Function(x)
                                                                                              Return x.Key
                                                                                          End Function).FirstOrDefault.Value
                                'Print previous candle
                                'Debug.WriteLine(previousCandle.PreviousPayload.ToString)
                                'Debug.WriteLine(previousCandle.ToString)
                            End If
                        End If
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

                            If runningPayload IsNot Nothing Then
                                Dim existingOrAddedPayload As OHLCPayload = Nothing
                                Dim freshCandleAdded As Boolean = False
                                If _parentInstrument.RawPayloads IsNot Nothing Then
                                    If Not _parentInstrument.RawPayloads.ContainsKey(runningPayload.SnapshotDateTime) Then freshCandleAdded = True
                                    existingOrAddedPayload = _parentInstrument.RawPayloads.GetOrAdd(runningPayload.SnapshotDateTime, runningPayload)
                                End If
                                Dim candleNeedsUpdate As Boolean = False
                                If existingOrAddedPayload.PayloadGeneratedBy = IPayload.PayloadSource.Tick Then
                                    candleNeedsUpdate = Not runningPayload.Equals(existingOrAddedPayload)
                                    _parentInstrument.RawPayloads(runningPayload.SnapshotDateTime) = runningPayload
                                ElseIf Not runningPayload.Equals(existingOrAddedPayload) Then
                                    candleNeedsUpdate = True
                                End If
                                If candleNeedsUpdate OrElse freshCandleAdded Then
                                    'existingOrAddedPayload = runningPayload
                                    _parentInstrument.RawPayloads(runningPayload.SnapshotDateTime) = runningPayload
                                    If _subscribedStrategyInstruments IsNot Nothing AndAlso _subscribedStrategyInstruments.Count > 0 Then
                                        For Each runningSubscribedStrategyInstrument In _subscribedStrategyInstruments
                                            Await runningSubscribedStrategyInstrument.PopulateChartAndIndicatorsAsync(Me, runningPayload).ConfigureAwait(False)
                                        Next
                                    End If
                                End If
                            End If
                        Next
                        _parentInstrument.IsHistoricalCompleted = True
                        'TODO: Below loop is for checking purpose
                        'For Each payload In _parentInstrument.RawPayloads.OrderBy(Function(x)
                        '                                                              Return x.Key
                        '                                                          End Function)
                        '    Debug.WriteLine(payload.Value.ToString())
                        'Next
                    End If
                End If
            Catch ex As Exception
                logger.Error("GetChartFromHistoricalAsync:{0}, error:{1}", Me.ToString, ex.ToString)
                Me.ParentController.OrphanException = ex
            Finally
                Interlocked.Decrement(_historicalLock)
                'Debug.WriteLine(String.Format("Process Historical after. Time:{0}, Lock:{1}", Now, _lock))
            End Try
        End Function

        Public Overrides Async Function GetChartFromTickAsync(ByVal tickData As ITick) As Task
            'logger.Debug("{0}->GetChartFromTickAsync, parameters:{1}", Me.ToString, Utilities.Strings.JsonSerialize(tickData))
            If tickData Is Nothing OrElse tickData.Timestamp Is Nothing OrElse tickData.Timestamp.Value = Date.MinValue OrElse tickData.Timestamp.Value = New Date(1970, 1, 1, 5, 30, 0) Then
                Exit Function
            End If

            Try
                While Interlocked.Read(_tickLock) > 0
                    Await Task.Delay(10).ConfigureAwait(False)
                End While
                Interlocked.Increment(_tickLock)

                Dim lastExistingPayload As OHLCPayload = Nothing
                If _parentInstrument.RawPayloads IsNot Nothing AndAlso _parentInstrument.RawPayloads.Count > 0 Then
                    Dim lastExistingPayloads As IEnumerable(Of KeyValuePair(Of Date, OHLCPayload)) =
                        _parentInstrument.RawPayloads.Where(Function(y)
                                                                Return Utilities.Time.IsDateTimeEqualTillMinutes(y.Key, tickData.Timestamp.Value)
                                                            End Function)
                    If lastExistingPayloads IsNot Nothing AndAlso lastExistingPayloads.Count > 0 Then lastExistingPayload = lastExistingPayloads.LastOrDefault.Value
                End If
                Dim runningPayload As OHLCPayload = Nothing
                Dim tickWasProcessed As Boolean = False
                'Do not touch the payload if it was already processed by historical
                Dim freshCandle As Boolean = False
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
                        .PayloadGeneratedBy = IPayload.PayloadSource.Tick
                    End With
                    tickWasProcessed = True
                ElseIf lastExistingPayload Is Nothing Then
                    'Fresh candle needs to be created
                    freshCandle = True
                    Dim previousCandle As OHLCPayload = Nothing
                    Dim previousCandles As IEnumerable(Of KeyValuePair(Of Date, OHLCPayload)) =
                        _parentInstrument.RawPayloads.Where(Function(y)
                                                                Return y.Key < tickData.Timestamp.Value AndAlso
                                                                (y.Value.PayloadGeneratedBy = IPayload.PayloadSource.Tick OrElse
                                                                (_parentInstrument.IsHistoricalCompleted AndAlso
                                                                y.Value.PayloadGeneratedBy = IPayload.PayloadSource.Historical))
                                                            End Function)

                    If previousCandles IsNot Nothing AndAlso previousCandles.Count > 0 Then
                        previousCandle = previousCandles.OrderByDescending(Function(x)
                                                                               Return x.Key
                                                                           End Function).FirstOrDefault.Value
                        'Print previous candle
                        'Debug.WriteLine(previousCandle.PreviousPayload.ToString)
                        'Debug.WriteLine(previousCandle.ToString)
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
                    tickWasProcessed = True

                    ''TODO: Below loop is for checking purpose
                    'For Each payload In _parentInstrument.RawPayloads.OrderBy(Function(x)
                    '                                                              Return x.Key
                    '                                                          End Function)
                    '    If payload.Value.PreviousPayload IsNot Nothing Then
                    '        Debug.WriteLine(payload.Value.ToString())
                    '    End If
                    'Next
                End If
                If tickWasProcessed Then 'If not processed would mean that the tick was for a historical candle that was already processed and not for a live candle
                    If runningPayload IsNot Nothing Then
                        _parentInstrument.RawPayloads.GetOrAdd(runningPayload.SnapshotDateTime, runningPayload)
                    Else
                        runningPayload = lastExistingPayload
                    End If
                    If _subscribedStrategyInstruments IsNot Nothing AndAlso _subscribedStrategyInstruments.Count > 0 Then
                        For Each runningSubscribedStrategyInstrument In _subscribedStrategyInstruments
                            Await runningSubscribedStrategyInstrument.PopulateChartAndIndicatorsAsync(Me, runningPayload).ConfigureAwait(False)
                        Next
                    End If
                End If




                ''TODO: Below loop is for checking purpose
                'Try
                '    Dim outputConsumer As PayloadToChartConsumer = _subscribedStrategyInstruments.FirstOrDefault.RawPayloadConsumers.FirstOrDefault
                '    If freshCandle AndAlso outputConsumer.ChartPayloads IsNot Nothing AndAlso outputConsumer.ChartPayloads.Count > 0 Then
                '        For Each payload In outputConsumer.ChartPayloads.OrderBy(Function(x)
                '                                                                     Return x.Key
                '                                                                 End Function)
                '            If payload.Value.PreviousPayload IsNot Nothing Then
                '                Debug.WriteLine(payload.Value.ToString())
                '            End If
                '        Next
                '    End If
                'Catch ex As Exception
                '    Throw ex
                'End Try
            Catch ex As Exception
                logger.Error("GetChartFromTickAsync:{0}, error:{1}", Me.ToString, ex.ToString)
                Me.ParentController.OrphanException = ex
            Finally
                Interlocked.Decrement(_tickLock)
                If _tickLock <> 0 Then Throw New ApplicationException("Check why lock is not released")
                'Debug.WriteLine(String.Format("Process Historical after. Time:{0}, Lock:{1}", Now, _lock))
            End Try
        End Function

        Public Overrides Async Function ConvertTimeframeAsync(ByVal timeframe As Integer, ByVal currentPayload As OHLCPayload, ByVal outputConsumer As PayloadToChartConsumer) As Task
            'logger.Debug("{0}->ConvertTimeframeAsync, parameters:{1},{2},{3}", Me.ToString, timeframe, currentPayload.ToString, outputConsumer.ToString)
            Await Task.Delay(0).ConfigureAwait(False)
            Dim blockDateInThisTimeframe As New Date(currentPayload.SnapshotDateTime.Year,
                                                    currentPayload.SnapshotDateTime.Month,
                                                    currentPayload.SnapshotDateTime.Day,
                                                    currentPayload.SnapshotDateTime.Hour,
                                                    Math.Floor(currentPayload.SnapshotDateTime.Minute / timeframe) * timeframe, 0)

            Dim payloadSource As IPayload.PayloadSource = IPayload.PayloadSource.None
            If currentPayload.PayloadGeneratedBy = IPayload.PayloadSource.Tick Then
                payloadSource = IPayload.PayloadSource.CalculatedTick
            ElseIf currentPayload.PayloadGeneratedBy = IPayload.PayloadSource.Historical Then
                payloadSource = IPayload.PayloadSource.CalculatedHistorical
            End If

            If outputConsumer.ChartPayloads Is Nothing Then
                outputConsumer.ChartPayloads = New Concurrent.ConcurrentDictionary(Of Date, OHLCPayload)
                Dim runninPayload As New OHLCPayload(payloadSource)
                With runninPayload
                    .OpenPrice = currentPayload.OpenPrice
                    .HighPrice = currentPayload.HighPrice
                    .LowPrice = currentPayload.LowPrice
                    .ClosePrice = currentPayload.ClosePrice
                    .DailyVolume = currentPayload.DailyVolume
                    .NumberOfTicks = 0 ' Cannot caluclated as histrical will not have the value
                    .PreviousPayload = Nothing
                    .SnapshotDateTime = blockDateInThisTimeframe
                    .TradingSymbol = currentPayload.TradingSymbol
                    .Volume = currentPayload.Volume
                End With
                outputConsumer.ChartPayloads.GetOrAdd(blockDateInThisTimeframe, runninPayload)
            Else
                Dim lastExistingPayload As OHLCPayload = Nothing
                If outputConsumer.ChartPayloads IsNot Nothing AndAlso outputConsumer.ChartPayloads.Count > 0 Then
                    Dim lastExistingPayloads As IEnumerable(Of KeyValuePair(Of Date, OHLCPayload)) =
                    outputConsumer.ChartPayloads.Where(Function(x)
                                                           Return x.Key = blockDateInThisTimeframe
                                                       End Function)
                    If lastExistingPayloads IsNot Nothing AndAlso lastExistingPayloads.Count > 0 Then lastExistingPayload = lastExistingPayloads.LastOrDefault.Value
                End If

                If lastExistingPayload IsNot Nothing Then
                    Dim previousPayload As OHLCPayload = Nothing
                    Dim previousPayloads As IEnumerable(Of KeyValuePair(Of Date, OHLCPayload)) =
                        outputConsumer.ChartPayloads.Where(Function(y)
                                                               Return y.Key < blockDateInThisTimeframe AndAlso
                                                                (y.Value.PayloadGeneratedBy = IPayload.PayloadSource.CalculatedTick OrElse
                                                                Interlocked.Read(_historicalLock) = 0 OrElse
                                                                (Interlocked.Read(_historicalLock) = 1 AndAlso
                                                                currentPayload.PayloadGeneratedBy = IPayload.PayloadSource.Historical))
                                                           End Function)
                    If previousPayloads IsNot Nothing AndAlso previousPayloads.Count > 0 Then
                        previousPayload = previousPayloads.OrderByDescending(Function(x)
                                                                                 Return x.Key
                                                                             End Function).FirstOrDefault.Value
                    End If

                    If currentPayload.SnapshotDateTime = blockDateInThisTimeframe AndAlso currentPayload.PayloadGeneratedBy = IPayload.PayloadSource.Historical Then
                        lastExistingPayload.OpenPrice = currentPayload.OpenPrice
                    End If
                    With lastExistingPayload
                        .HighPrice = Math.Max(.HighPrice, currentPayload.HighPrice)
                        .LowPrice = Math.Min(.LowPrice, currentPayload.LowPrice)
                        .ClosePrice = currentPayload.ClosePrice
                        .PreviousPayload = previousPayload
                        If .PreviousPayload IsNot Nothing AndAlso .SnapshotDateTime.Date = .PreviousPayload.SnapshotDateTime.Date Then
                            .Volume = currentPayload.DailyVolume - .PreviousPayload.DailyVolume
                        Else
                            .Volume = currentPayload.DailyVolume
                        End If
                        .DailyVolume = currentPayload.DailyVolume
                        .PayloadGeneratedBy = payloadSource
                    End With

                    'If currentPayload.PayloadGeneratedBy = IPayload.PayloadSource.Tick Then
                    '    If lastExistingPayload.PreviousPayload IsNot Nothing Then Debug.WriteLine(lastExistingPayload)
                    'End If
                    'Debug.WriteLine(lastExistingPayload)
                Else
                    Dim runninPayload As New OHLCPayload(payloadSource)

                    Dim previousPayload As OHLCPayload = Nothing
                    Dim previousPayloads As IEnumerable(Of KeyValuePair(Of Date, OHLCPayload)) =
                        outputConsumer.ChartPayloads.Where(Function(y)
                                                               Return y.Key < blockDateInThisTimeframe AndAlso
                                                                (y.Value.PayloadGeneratedBy = IPayload.PayloadSource.CalculatedTick OrElse
                                                                Interlocked.Read(_historicalLock) = 0 OrElse
                                                                (Interlocked.Read(_historicalLock) = 1 AndAlso
                                                                currentPayload.PayloadGeneratedBy = IPayload.PayloadSource.Historical))
                                                           End Function)
                    If previousPayloads IsNot Nothing AndAlso previousPayloads.Count > 0 Then
                        previousPayload = previousPayloads.OrderByDescending(Function(x)
                                                                                 Return x.Key
                                                                             End Function).FirstOrDefault.Value
                    End If

                    With runninPayload
                        .OpenPrice = currentPayload.OpenPrice
                        .HighPrice = currentPayload.HighPrice
                        .LowPrice = currentPayload.LowPrice
                        .ClosePrice = currentPayload.ClosePrice
                        .DailyVolume = currentPayload.DailyVolume
                        .NumberOfTicks = 0 ' Cannot caluclated as histrical will not have the value
                        .PreviousPayload = previousPayload
                        .SnapshotDateTime = blockDateInThisTimeframe
                        .TradingSymbol = currentPayload.TradingSymbol
                        If .PreviousPayload IsNot Nothing AndAlso
                            .SnapshotDateTime.Date = .PreviousPayload.SnapshotDateTime.Date Then
                            .Volume = currentPayload.DailyVolume - .PreviousPayload.DailyVolume
                        Else
                            .Volume = currentPayload.DailyVolume
                        End If
                    End With
                    outputConsumer.ChartPayloads.GetOrAdd(runninPayload.SnapshotDateTime, runninPayload)

                    ''TODO: Below loop is for checking purpose
                    'For Each payload In outputConsumer.ChartPayloads.OrderBy(Function(x)
                    '                                                             Return x.Key
                    '                                                         End Function)
                    '    If payload.Value.PreviousPayload IsNot Nothing Then
                    '        Debug.WriteLine(payload.Value.ToString())
                    '    End If
                    'Next

                    'Debug.WriteLine(runninPayload)

                    'Debug.WriteLine(runninPayload.PreviousPayload.ToString)
                End If
            End If

        End Function

        Public Overrides Function ToString() As String
            Return Me.GetType.ToString
        End Function
    End Class
End Namespace
