Imports System.Threading
Imports Algo2TradeCore.Entities
Namespace ChartHandler.ChartTimeframe
    Public Class ChartTimeframeConsumer
        Inherits PayloadConsumer
        Private _timeframe As Integer
        Public Sub New(ByVal timeframe As Integer, ByVal canceller As CancellationTokenSource)
            MyBase.New(canceller)
            _timeframe = timeframe
        End Sub
        Public Property ChartPayloads As SortedDictionary(Of Date, OHLCPayload)
        Public OnwardLevelConsumers As List(Of PayloadConsumer)

        Public Overrides Async Function PopulateFromOHLCAsync(ByVal payload As OHLCPayload) As Task
            Await Task.Delay(0).ConfigureAwait(False)
            Dim blockDateInThisTimeframe As New Date(payload.SnapshotDateTime.Year,
                                                         payload.SnapshotDateTime.Month,
                                                         payload.SnapshotDateTime.Day,
                                                         payload.SnapshotDateTime.Hour,
                                                         Math.Floor(payload.SnapshotDateTime.Minute / _timeframe) * _timeframe, 0)

            If ChartPayloads Is Nothing Then
                ChartPayloads = New SortedDictionary(Of Date, OHLCPayload) From {{blockDateInThisTimeframe, payload}}
            Else
                Dim lastAvailablePayload As IEnumerable(Of KeyValuePair(Of Date, OHLCPayload)) =
                    ChartPayloads.Where(Function(x)
                                            Return x.Key = blockDateInThisTimeframe
                                        End Function)
                If lastAvailablePayload IsNot Nothing AndAlso lastAvailablePayload.Count > 0 Then
                    With lastAvailablePayload.LastOrDefault.Value
                        .HighPrice = Math.Max(.HighPrice, payload.HighPrice)
                        .LowPrice = Math.Min(.LowPrice, payload.LowPrice)
                        .ClosePrice = payload.ClosePrice
                        If .PreviousPayload Is Nothing Then
                            .Volume = payload.DailyVolume
                        Else
                            .Volume = payload.DailyVolume - .PreviousPayload.DailyVolume
                        End If
                        .DailyVolume = payload.DailyVolume
                    End With
                Else
                    Dim runninPayload As New OHLCPayload(IPayload.PayloadSource.Calculated)
                    Dim previousPayload As OHLCPayload = Nothing
                    If ChartPayloads.ContainsKey(GetPreviousXMinuteCandleTime(blockDateInThisTimeframe, ChartPayloads, _timeframe)) Then
                        previousPayload = ChartPayloads(GetPreviousXMinuteCandleTime(blockDateInThisTimeframe, ChartPayloads, _timeframe))
                    End If
                    With runninPayload
                        .OpenPrice = payload.OpenPrice
                        .HighPrice = payload.HighPrice
                        .LowPrice = payload.LowPrice
                        .ClosePrice = payload.ClosePrice
                        .DailyVolume = payload.DailyVolume
                        .NumberOfTicks = 0 ' Cannot caluclated as histrical will not have the value
                        .PreviousPayload = previousPayload
                        .SnapshotDateTime = blockDateInThisTimeframe
                        .TradingSymbol = payload.TradingSymbol
                        If .PreviousPayload IsNot Nothing Then
                            .Volume = payload.DailyVolume - .PreviousPayload.DailyVolume
                        Else
                            .Volume = payload.DailyVolume
                        End If
                    End With
                    ChartPayloads.Add(runninPayload.SnapshotDateTime, runninPayload)
                    Debug.WriteLine(runninPayload.PreviousPayload.ToString)
                End If
            End If
        End Function
        Private Function GetPreviousXMinuteCandleTime(ByVal lowerTFTime As Date, ByVal higherTFPayload As SortedDictionary(Of Date, OHLCPayload), ByVal higherTF As Integer) As Date
            Dim ret As Date = Nothing

            If higherTFPayload IsNot Nothing AndAlso higherTFPayload.Count > 0 Then
                ret = higherTFPayload.Keys.LastOrDefault(Function(x)
                                                             Return x <= lowerTFTime.AddMinutes(-higherTF)
                                                         End Function)
            End If
            Return ret
        End Function

        Public Overrides Async Function PopulateFromValuesAsync(ByVal ParamArray values() As Tuple(Of Date, Decimal)) As Task
            Await Task.Delay(0).ConfigureAwait(False)
            Throw New NotImplementedException
        End Function
    End Class
End Namespace
