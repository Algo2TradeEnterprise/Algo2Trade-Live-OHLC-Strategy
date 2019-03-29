﻿Imports NLog
Imports System.Threading
Imports Algo2TradeCore.Entities
Imports Algo2TradeCore.Controller
Imports Algo2TradeCore.Entities.Indicators
Imports Algo2TradeCore.ChartHandler.ChartStyle

Namespace ChartHandler.Indicator
    Public Class IndicatorManeger

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
        Private ReadOnly _parentChart As CandleStickChart
        Private ReadOnly _cts As New CancellationTokenSource
        Public Sub New(ByVal associatedParentController As APIStrategyController,
                      ByVal assoicatedParentChart As CandleStickChart,
                      ByVal canceller As CancellationTokenSource)
            Me.ParentController = associatedParentController
            _parentChart = assoicatedParentChart
            _cts = canceller
        End Sub

#Region "Public Functions"
        Public Async Function CalculateSMA(ByVal timeToCalculateFrom As Date, ByVal outputConsumer As SMAConsumer) As Task
            Await Task.Delay(0, _cts.Token).ConfigureAwait(False)
            If outputConsumer IsNot Nothing AndAlso outputConsumer.ParentConsumer IsNot Nothing AndAlso
                outputConsumer.ParentConsumer.ConsumerPayloads IsNot Nothing AndAlso outputConsumer.ParentConsumer.ConsumerPayloads.Count > 0 Then
                Dim requiredDataSet As IEnumerable(Of Date) =
                    outputConsumer.ParentConsumer.ConsumerPayloads.Keys.Where(Function(x)
                                                                                  Return x >= timeToCalculateFrom
                                                                              End Function)

                For Each runningInputDate In requiredDataSet.OrderBy(Function(x)
                                                                         Return x
                                                                     End Function)
                    If outputConsumer.ConsumerPayloads Is Nothing Then outputConsumer.ConsumerPayloads = New Concurrent.ConcurrentDictionary(Of Date, IPayload)
                    Dim previousNInputFieldPayloadDate As Date = GetSubPayloadStartDate(outputConsumer.ParentConsumer.ConsumerPayloads,
                                                                                        runningInputDate,
                                                                                        outputConsumer.SMAPeriod,
                                                                                        True).Item1
                    Dim smaValue As SMAConsumer.SMAPayload = Nothing
                    If Not outputConsumer.ConsumerPayloads.TryGetValue(runningInputDate, smaValue) Then
                        smaValue = New SMAConsumer.SMAPayload
                    End If
                    Dim requiredData As IEnumerable(Of KeyValuePair(Of Date, IPayload)) =
                        outputConsumer.ParentConsumer.ConsumerPayloads.Where(Function(x)
                                                                                 Return x.Key >= previousNInputFieldPayloadDate AndAlso
                                                                                        x.Key <= runningInputDate
                                                                             End Function)

                    If requiredData IsNot Nothing AndAlso requiredData.Count > 0 Then
                        Select Case outputConsumer.SMAField
                            Case TypeOfField.Close
                                smaValue.SMA.Value = requiredData.Sum(Function(s) CType(s.Value, OHLCPayload).ClosePrice.Value) / requiredData.Count
                        End Select
                    End If
                    outputConsumer.ConsumerPayloads.AddOrUpdate(runningInputDate, smaValue, Function(key, value) smaValue)
                Next
            End If
        End Function
        Public Async Function CalculateEMA(ByVal timeToCalculateFrom As Date, ByVal outputConsumer As EMAConsumer) As Task
            Await Task.Delay(0, _cts.Token).ConfigureAwait(False)
            If outputConsumer IsNot Nothing AndAlso outputConsumer.ParentConsumer IsNot Nothing AndAlso
                outputConsumer.ParentConsumer.ConsumerPayloads IsNot Nothing AndAlso outputConsumer.ParentConsumer.ConsumerPayloads.Count > 0 Then

                'Await CalculateSMA(timeToCalculateFrom, outputConsumer.SupportingSMAConsumer)

                Dim requiredDataSet As IEnumerable(Of Date) =
                    outputConsumer.ParentConsumer.ConsumerPayloads.Keys.Where(Function(x)
                                                                                  Return x >= timeToCalculateFrom
                                                                              End Function)

                For Each runningInputDate In requiredDataSet.OrderBy(Function(x)
                                                                         Return x
                                                                     End Function)
                    If outputConsumer.ConsumerPayloads Is Nothing Then outputConsumer.ConsumerPayloads = New Concurrent.ConcurrentDictionary(Of Date, IPayload)

                    Dim emaValue As EMAConsumer.EMAPayload = Nothing
                    If Not outputConsumer.ConsumerPayloads.TryGetValue(runningInputDate, emaValue) Then
                        emaValue = New EMAConsumer.EMAPayload
                    End If

                    Dim previousEMAValues As IEnumerable(Of KeyValuePair(Of Date, IPayload)) = Nothing
                    Dim previousEMAValue As EMAConsumer.EMAPayload = Nothing
                    If outputConsumer.ConsumerPayloads IsNot Nothing AndAlso outputConsumer.ConsumerPayloads.Count > 0 Then
                        previousEMAValues = outputConsumer.ConsumerPayloads.Where(Function(x)
                                                                                      Return x.Key < runningInputDate
                                                                                  End Function)
                        If previousEMAValues IsNot Nothing AndAlso previousEMAValues.Count > 0 Then
                            previousEMAValue = previousEMAValues.OrderBy(Function(y)
                                                                             Return y.Key
                                                                         End Function).LastOrDefault.Value
                        End If
                    End If
                    Select Case outputConsumer.EMAField
                        Case TypeOfField.Close
                            If previousEMAValues Is Nothing OrElse (previousEMAValues IsNot Nothing AndAlso previousEMAValues.Count < outputConsumer.EMAPeriod) Then
                                Await CalculateSMA(timeToCalculateFrom, outputConsumer.SupportingSMAConsumer)
                                emaValue.EMA.Value = CType(outputConsumer.SupportingSMAConsumer.ConsumerPayloads(runningInputDate), SMAConsumer.SMAPayload).SMA.Value
                            Else
                                emaValue.EMA.Value = (CType(outputConsumer.ParentConsumer.ConsumerPayloads(runningInputDate), OHLCPayload).ClosePrice.Value * (2 / (1 + outputConsumer.EMAPeriod))) + (previousEMAValue.EMA.Value * (1 - (2 / (1 + outputConsumer.EMAPeriod))))
                            End If
                    End Select
                    outputConsumer.ConsumerPayloads.AddOrUpdate(runningInputDate, emaValue, Function(key, value) emaValue)
                Next
            End If
        End Function
        Public Async Function CalculateATR(ByVal timeToCalculateFrom As Date, ByVal outputConsumer As ATRConsumer) As Task
            Await Task.Delay(0, _cts.Token).ConfigureAwait(False)
            If outputConsumer IsNot Nothing AndAlso outputConsumer.ParentConsumer IsNot Nothing AndAlso
                outputConsumer.ParentConsumer.ConsumerPayloads IsNot Nothing AndAlso outputConsumer.ParentConsumer.ConsumerPayloads.Count > 0 Then
                Dim requiredDataSet As IEnumerable(Of Date) =
                    outputConsumer.ParentConsumer.ConsumerPayloads.Keys.Where(Function(x)
                                                                                  Return x >= timeToCalculateFrom
                                                                              End Function)

                For Each runningInputDate In requiredDataSet.OrderBy(Function(x)
                                                                         Return x
                                                                     End Function)
                    If outputConsumer.ConsumerPayloads Is Nothing Then outputConsumer.ConsumerPayloads = New Concurrent.ConcurrentDictionary(Of Date, IPayload)

                    Dim atrValue As ATRConsumer.ATRPayload = Nothing
                    If Not outputConsumer.ConsumerPayloads.TryGetValue(runningInputDate, atrValue) Then
                        atrValue = New ATRConsumer.ATRPayload
                    End If

                    Dim previousATRValues As IEnumerable(Of KeyValuePair(Of Date, IPayload)) = Nothing
                    Dim previousATRValue As ATRConsumer.ATRPayload = Nothing
                    If outputConsumer.ConsumerPayloads IsNot Nothing AndAlso outputConsumer.ConsumerPayloads.Count > 0 Then
                        previousATRValues = outputConsumer.ConsumerPayloads.Where(Function(x)
                                                                                      Return x.Key < runningInputDate
                                                                                  End Function)
                        If previousATRValues IsNot Nothing AndAlso previousATRValues.Count > 0 Then
                            previousATRValue = previousATRValues.OrderBy(Function(y)
                                                                             Return y.Key
                                                                         End Function).LastOrDefault.Value
                        End If
                    End If

                    Dim currentPayload As OHLCPayload = outputConsumer.ParentConsumer.ConsumerPayloads(runningInputDate)
                    Dim highLow As Double = currentPayload.HighPrice.Value - currentPayload.LowPrice.Value
                    Dim highPClose As Double = 0
                    Dim lowPClose As Double = 0
                    Dim TR As Decimal = 0

                    If currentPayload.PreviousPayload Is Nothing Then
                        TR = highLow
                    Else
                        highPClose = Math.Abs(currentPayload.HighPrice.Value - currentPayload.PreviousPayload.ClosePrice.Value)
                        lowPClose = Math.Abs(currentPayload.LowPrice.Value - currentPayload.PreviousPayload.ClosePrice.Value)
                        TR = Math.Max(highLow, Math.Max(highPClose, lowPClose))
                    End If

                    If previousATRValues Is Nothing OrElse
                        (previousATRValues IsNot Nothing AndAlso previousATRValues.Count < outputConsumer.ATRPeriod) Then
                        If previousATRValues IsNot Nothing AndAlso previousATRValues.Count > 0 Then
                            atrValue.ATR.Value = (previousATRValues.Sum(Function(x)
                                                                            Return CType(x.Value, ATRConsumer.ATRPayload).ATR.Value
                                                                        End Function) + TR) / (previousATRValues.Count + 1)
                        Else
                            atrValue.ATR.Value = TR
                        End If
                    Else
                        atrValue.ATR.Value = (previousATRValue.ATR.Value * (outputConsumer.ATRPeriod - 1) + TR) / outputConsumer.ATRPeriod
                    End If

                    outputConsumer.ConsumerPayloads.AddOrUpdate(runningInputDate, atrValue, Function(key, value) atrValue)
                Next
            End If
        End Function
#End Region

#Region "Private Function"
        Private Function GetSubPayloadStartDate(ByVal inputPayload As Concurrent.ConcurrentDictionary(Of Date, IPayload),
                                                ByVal beforeThisTime As Date,
                                                ByVal numberOfItemsToRetrive As Integer,
                                                ByVal includeTimePassedAsOneOftheItems As Boolean) As Tuple(Of Date, Integer)
            Dim ret As Tuple(Of Date, Integer) = Nothing
            If inputPayload IsNot Nothing Then
                Dim requiredData As IEnumerable(Of KeyValuePair(Of Date, IPayload)) = Nothing
                If includeTimePassedAsOneOftheItems Then
                    requiredData = inputPayload.Where(Function(y)
                                                          Return y.Key <= beforeThisTime
                                                      End Function)
                Else
                    requiredData = inputPayload.Where(Function(y)
                                                          Return y.Key < beforeThisTime
                                                      End Function)
                End If
                If requiredData IsNot Nothing AndAlso requiredData.Count > 0 Then
                    ret = New Tuple(Of Date, Integer)(requiredData.OrderByDescending(Function(x)
                                                                                         Return x.Key
                                                                                     End Function).Take(numberOfItemsToRetrive).LastOrDefault.Key, requiredData.Count)
                End If
            End If
            Return ret
        End Function
#End Region

    End Class
End Namespace