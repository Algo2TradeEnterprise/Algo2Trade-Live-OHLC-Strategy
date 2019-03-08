Imports System.ComponentModel
Imports System.Threading
Imports Algo2TradeCore.Controller
Imports Algo2TradeCore.Entities
Imports NLog

Namespace Strategies
    Public Class SignalStateManager

#Region "Logging and Status Progress"
        Public Shared logger As Logger = LogManager.GetCurrentClassLogger
#End Region

        Protected _cts As CancellationTokenSource
        Public Property ParentController As APIStrategyController
        Public Property ParentStrategy As Strategy
        Public Property ActivityDetails As Concurrent.ConcurrentDictionary(Of String, ActivityDashboard)

        Private _defaultDateValue As Date
        Private _defaultExceptionValue As Exception
        Public Sub New(ByVal associatedParentController As APIStrategyController,
                       ByVal associatedStrategy As Strategy,
                       ByVal canceller As CancellationTokenSource)
            Me.ParentController = associatedParentController
            Me.ParentStrategy = associatedStrategy
            Me._cts = canceller
            Me.ActivityDetails = New Concurrent.ConcurrentDictionary(Of String, ActivityDashboard)
            _defaultDateValue = New Date(2000, 1, 1)
            _defaultExceptionValue = New ApplicationException("Dummy")
        End Sub

#Region "Public Function"

#Region "Entry Activity"
        Public Async Function HandleEntryActivity(ByVal activityTag As String,
                                       ByVal associatedStrategyInstrument As StrategyInstrument,
                                       ByVal associatedOrderID As String,
                                       ByVal signalGeneratedTime As Date,
                                       ByVal requestTime As Date) As Task
            Await AddOrUpdateEntryActivity(activityTag:=activityTag,
                                     associatedStrategyInstrument:=associatedStrategyInstrument,
                                     associatedOrderID:=associatedOrderID,
                                     signalGeneratedTime:=signalGeneratedTime,
                                     requestTime:=requestTime,
                                     receivedTime:=_defaultDateValue,
                                     requestStatus:=ActivityDashboard.SignalStatusType.Handled,
                                     lastException:=_defaultExceptionValue).ConfigureAwait(False)
        End Function
        Public Async Function ActivateEntryActivity(ByVal activityTag As String,
                                         ByVal associatedStrategyInstrument As StrategyInstrument,
                                         ByVal associatedOrderID As String,
                                         ByVal receivedTime As Date) As Task
            Await AddOrUpdateEntryActivity(activityTag:=activityTag,
                                     associatedStrategyInstrument:=associatedStrategyInstrument,
                                     associatedOrderID:=associatedOrderID,
                                     receivedTime:=receivedTime,
                                     requestStatus:=ActivityDashboard.SignalStatusType.Activated).ConfigureAwait(False)
        End Function
        Public Async Function DiscardEntryActivity(ByVal activityTag As String,
                                        ByVal associatedStrategyInstrument As StrategyInstrument,
                                        ByVal associatedOrderID As String,
                                        ByVal receivedTime As Date,
                                        ByVal lastException As Exception) As Task
            Await AddOrUpdateEntryActivity(activityTag:=activityTag,
                                     associatedStrategyInstrument:=associatedStrategyInstrument,
                                     associatedOrderID:=associatedOrderID,
                                     receivedTime:=receivedTime,
                                     requestStatus:=ActivityDashboard.SignalStatusType.Discarded,
                                     lastException:=lastException).ConfigureAwait(False)
        End Function
        Public Async Function CancelEntryActivity(ByVal activityTag As String,
                                       ByVal associatedStrategyInstrument As StrategyInstrument,
                                       ByVal associatedOrderID As String,
                                       ByVal requestRemarks As String) As Task
            Await AddOrUpdateEntryActivity(activityTag:=activityTag,
                                     associatedStrategyInstrument:=associatedStrategyInstrument,
                                     associatedOrderID:=associatedOrderID,
                                     requestStatus:=ActivityDashboard.SignalStatusType.Cancelled,
                                     requestRemarks:=requestRemarks).ConfigureAwait(False)
        End Function
        Public Async Function RejectEntryActivity(ByVal activityTag As String,
                                       ByVal associatedStrategyInstrument As StrategyInstrument,
                                       ByVal associatedOrderID As String,
                                       ByVal requestRemarks As String) As Task
            Await AddOrUpdateEntryActivity(activityTag:=activityTag,
                                     associatedStrategyInstrument:=associatedStrategyInstrument,
                                     associatedOrderID:=associatedOrderID,
                                     requestStatus:=ActivityDashboard.SignalStatusType.Rejected,
                                     requestRemarks:=requestRemarks).ConfigureAwait(False)
        End Function
        Public Async Function CompleteEntryActivity(ByVal activityTag As String,
                                         ByVal associatedStrategyInstrument As StrategyInstrument,
                                         ByVal associatedOrderID As String,
                                         ByVal requestRemarks As String) As Task
            Await AddOrUpdateEntryActivity(activityTag:=activityTag,
                                     associatedStrategyInstrument:=associatedStrategyInstrument,
                                     associatedOrderID:=associatedOrderID,
                                     requestStatus:=ActivityDashboard.SignalStatusType.Completed,
                                     requestRemarks:=requestRemarks).ConfigureAwait(False)
        End Function
        Public Async Function RunningEntryActivity(ByVal activityTag As String,
                                        ByVal associatedStrategyInstrument As StrategyInstrument,
                                        ByVal associatedOrderID As String,
                                        ByVal requestRemarks As String) As Task
            Await AddOrUpdateEntryActivity(activityTag:=activityTag,
                                     associatedStrategyInstrument:=associatedStrategyInstrument,
                                     associatedOrderID:=associatedOrderID,
                                     requestStatus:=ActivityDashboard.SignalStatusType.Running,
                                     requestRemarks:=requestRemarks).ConfigureAwait(False)
        End Function
#End Region

#Region "Target Modify Activity"
        Public Async Function HandleTargetModifyActivity(ByVal activityTag As String,
                                              ByVal associatedStrategyInstrument As StrategyInstrument,
                                              ByVal associatedOrderID As String,
                                              ByVal requestTime As Date,
                                              ByVal price As Decimal) As Task
            Await AddOrUpdateTargetModifyActivity(activityTag:=activityTag,
                                            associatedStrategyInstrument:=associatedStrategyInstrument,
                                            associatedOrderID:=associatedOrderID,
                                            requestTime:=requestTime,
                                            receivedTime:=_defaultDateValue,
                                            requestStatus:=ActivityDashboard.SignalStatusType.Handled,
                                            lastException:=_defaultExceptionValue,
                                            price:=price).ConfigureAwait(False)
        End Function
        Public Async Function ActivateTargetModifyActivity(ByVal activityTag As String,
                                                ByVal associatedStrategyInstrument As StrategyInstrument,
                                                ByVal associatedOrderID As String,
                                                ByVal receivedTime As Date) As Task
            Await AddOrUpdateTargetModifyActivity(activityTag:=activityTag,
                                            associatedStrategyInstrument:=associatedStrategyInstrument,
                                            associatedOrderID:=associatedOrderID,
                                            receivedTime:=receivedTime,
                                            requestStatus:=ActivityDashboard.SignalStatusType.Activated).ConfigureAwait(False)
        End Function
        Public Async Function RejectTargetModifyActivity(ByVal activityTag As String,
                                              ByVal associatedStrategyInstrument As StrategyInstrument,
                                              ByVal associatedOrderID As String,
                                              ByVal requestRemarks As String) As Task
            Await AddOrUpdateTargetModifyActivity(activityTag:=activityTag,
                                            associatedStrategyInstrument:=associatedStrategyInstrument,
                                            associatedOrderID:=associatedOrderID,
                                            requestStatus:=ActivityDashboard.SignalStatusType.Rejected,
                                            requestRemarks:=requestRemarks).ConfigureAwait(False)
        End Function
        Public Async Function CompleteTargetModifyActivity(ByVal activityTag As String,
                                                ByVal associatedStrategyInstrument As StrategyInstrument,
                                                ByVal associatedOrderID As String,
                                                ByVal requestRemarks As String) As Task
            Await AddOrUpdateTargetModifyActivity(activityTag:=activityTag,
                                            associatedStrategyInstrument:=associatedStrategyInstrument,
                                            associatedOrderID:=associatedOrderID,
                                            requestStatus:=ActivityDashboard.SignalStatusType.Completed,
                                            requestRemarks:=requestRemarks).ConfigureAwait(False)
        End Function
#End Region

#Region "Stoploss Modify Activity"
        Public Async Function HandleStoplossModifyActivity(ByVal activityTag As String,
                                                ByVal associatedStrategyInstrument As StrategyInstrument,
                                                ByVal associatedOrderID As String,
                                                ByVal requestTime As Date,
                                                ByVal triggerprice As Decimal) As Task
            Await AddOrUpdateStoplossModifyActivity(activityTag:=activityTag,
                                              associatedStrategyInstrument:=associatedStrategyInstrument,
                                              associatedOrderID:=associatedOrderID,
                                              requestTime:=requestTime,
                                              receivedTime:=_defaultDateValue,
                                              requestStatus:=ActivityDashboard.SignalStatusType.Handled,
                                              lastException:=_defaultExceptionValue,
                                              triggerPrice:=triggerprice).ConfigureAwait(False)
        End Function
        Public Async Function ActivateStoplossModifyActivity(ByVal activityTag As String,
                                                  ByVal associatedStrategyInstrument As StrategyInstrument,
                                                  ByVal associatedOrderID As String,
                                                  ByVal receivedTime As Date) As Task
            Await AddOrUpdateStoplossModifyActivity(activityTag:=activityTag,
                                              associatedStrategyInstrument:=associatedStrategyInstrument,
                                              associatedOrderID:=associatedOrderID,
                                              receivedTime:=receivedTime,
                                              requestStatus:=ActivityDashboard.SignalStatusType.Activated).ConfigureAwait(False)
        End Function
        Public Async Function RejectStoplossModifyActivity(ByVal activityTag As String,
                                                ByVal associatedStrategyInstrument As StrategyInstrument,
                                                ByVal associatedOrderID As String,
                                                ByVal requestRemarks As String) As Task
            Await AddOrUpdateStoplossModifyActivity(activityTag:=activityTag,
                                              associatedStrategyInstrument:=associatedStrategyInstrument,
                                              associatedOrderID:=associatedOrderID,
                                              requestStatus:=ActivityDashboard.SignalStatusType.Rejected,
                                              requestRemarks:=requestRemarks).ConfigureAwait(False)
        End Function
        Public Async Function CompleteStoplossModifyActivity(ByVal activityTag As String,
                                                  ByVal associatedStrategyInstrument As StrategyInstrument,
                                                  ByVal associatedOrderID As String,
                                                  ByVal requestRemarks As String) As Task
            Await AddOrUpdateStoplossModifyActivity(activityTag:=activityTag,
                                              associatedStrategyInstrument:=associatedStrategyInstrument,
                                              associatedOrderID:=associatedOrderID,
                                              requestStatus:=ActivityDashboard.SignalStatusType.Completed,
                                              requestRemarks:=requestRemarks).ConfigureAwait(False)
        End Function
#End Region

#Region "Cancel Activity"
        Public Async Function HandleCancelActivity(ByVal activityTag As String,
                                        ByVal associatedStrategyInstrument As StrategyInstrument,
                                        ByVal associatedOrderID As String,
                                        ByVal requestTime As Date) As Task
            Await AddOrUpdateCancelActivity(activityTag:=activityTag,
                                      associatedStrategyInstrument:=associatedStrategyInstrument,
                                      associatedOrderID:=associatedOrderID,
                                      requestTime:=requestTime,
                                      receivedTime:=_defaultDateValue,
                                      requestStatus:=ActivityDashboard.SignalStatusType.Handled,
                                      lastException:=_defaultExceptionValue).ConfigureAwait(False)
        End Function
        Public Async Function ActivateCancelActivity(ByVal activityTag As String,
                                          ByVal associatedStrategyInstrument As StrategyInstrument,
                                          ByVal associatedOrderID As String,
                                          ByVal receivedTime As Date) As Task
            Await AddOrUpdateCancelActivity(activityTag:=activityTag,
                                      associatedStrategyInstrument:=associatedStrategyInstrument,
                                      associatedOrderID:=associatedOrderID,
                                      receivedTime:=receivedTime,
                                      requestStatus:=ActivityDashboard.SignalStatusType.Activated).ConfigureAwait(False)
        End Function
        Public Async Function RejectCancelActivity(ByVal activityTag As String,
                                        ByVal associatedStrategyInstrument As StrategyInstrument,
                                        ByVal associatedOrderID As String,
                                        ByVal requestRemarks As String) As Task
            Await AddOrUpdateCancelActivity(activityTag:=activityTag,
                                      associatedStrategyInstrument:=associatedStrategyInstrument,
                                      associatedOrderID:=associatedOrderID,
                                      requestStatus:=ActivityDashboard.SignalStatusType.Rejected,
                                      requestRemarks:=requestRemarks).ConfigureAwait(False)
        End Function
        Public Async Function CompleteCancelActivity(ByVal activityTag As String,
                                          ByVal associatedStrategyInstrument As StrategyInstrument,
                                          ByVal associatedOrderID As String,
                                          ByVal requestRemarks As String) As Task
            Await AddOrUpdateCancelActivity(activityTag:=activityTag,
                                      associatedStrategyInstrument:=associatedStrategyInstrument,
                                      associatedOrderID:=associatedOrderID,
                                      requestStatus:=ActivityDashboard.SignalStatusType.Completed,
                                      requestRemarks:=requestRemarks).ConfigureAwait(False)
        End Function
#End Region

#Region "Get Signal"
        Public Function GetSignalActivities(ByVal signalCandleTime As Date, ByVal instrumentIdentifier As String) As IEnumerable(Of ActivityDashboard)
            Dim ret As IEnumerable(Of ActivityDashboard) = Nothing
            Try
                Dim tempRet As List(Of ActivityDashboard) = Nothing
                If Me.ActivityDetails IsNot Nothing AndAlso Me.ActivityDetails.Count > 0 Then
                    Dim runningStrategyInstrumentActivities As IEnumerable(Of KeyValuePair(Of String, ActivityDashboard)) =
                    Me.ActivityDetails.Where(Function(x)
                                                 Dim key As String = Convert.ToInt64(x.Key, 16)
                                                 Return key.Substring(0, 1).Equals(Me.ParentStrategy.StrategyIdentifier) AndAlso
                                                 Val(key.Substring(1, 3)) = Val(Me.ParentController.InstrumentMappingTable(instrumentIdentifier))
                                             End Function)

                    If runningStrategyInstrumentActivities IsNot Nothing AndAlso runningStrategyInstrumentActivities.Count > 0 Then
                        Dim currentSignalActivities As IEnumerable(Of KeyValuePair(Of String, ActivityDashboard)) =
                        runningStrategyInstrumentActivities.Where(Function(y)
                                                                      Return Utilities.Time.IsDateTimeEqualTillMinutes(y.Value.SignalGeneratedTime, signalCandleTime)
                                                                  End Function)

                        If currentSignalActivities IsNot Nothing AndAlso currentSignalActivities.Count > 0 Then
                            For Each currentSignalActivity In currentSignalActivities
                                If tempRet Is Nothing Then tempRet = New List(Of ActivityDashboard)
                                tempRet.Add(currentSignalActivity.Value)
                            Next
                        End If
                    End If
                End If
                ret = tempRet
            Catch ex As Exception
                logger.Error(ex)
                Throw ex
            End Try
            Return ret
        End Function
        Public Function GetSignalActivities(ByVal activityTag As String) As ActivityDashboard
            Dim ret As ActivityDashboard = Nothing
            If Me.ActivityDetails IsNot Nothing AndAlso Me.ActivityDetails.Count > 0 AndAlso Me.ActivityDetails.ContainsKey(activityTag) Then
                ret = Me.ActivityDetails(activityTag)
            End If
            Return ret
        End Function
#End Region

#Region "UI Refresh"
        Public Async Function UIRefresh(ByVal associatedStrategyInstrument As StrategyInstrument) As Task
            Try
                Await Task.Delay(0, _cts.Token).ConfigureAwait(False)
                If associatedStrategyInstrument IsNot Nothing AndAlso
                    ActivityDetails IsNot Nothing AndAlso ActivityDetails.Count > 0 Then
                    Dim currentInstrumentActivities As IEnumerable(Of KeyValuePair(Of String, ActivityDashboard)) =
                        Me.ActivityDetails.Where(Function(x)
                                                     Dim key As String = Convert.ToInt64(x.Key, 16)
                                                     Return key.Substring(0, 4).Equals(String.Format("{0}{1:D3}", Me.ParentStrategy.StrategyIdentifier, Me.ParentStrategy.ParentController.InstrumentMappingTable(associatedStrategyInstrument.TradableInstrument.InstrumentIdentifier)))
                                                 End Function)
                    If currentInstrumentActivities IsNot Nothing AndAlso currentInstrumentActivities.Count > 0 Then
                        currentInstrumentActivities.Select(Function(x)
                                                               If associatedStrategyInstrument.TradableInstrument.LastTick IsNot Nothing AndAlso
                                                               associatedStrategyInstrument.TradableInstrument.LastTick.LastPrice <> x.Value.GetDirtyLastPrice Then
                                                                   x.Value.NotifyPropertyChanged("LastPrice")
                                                                   x.Value.NotifyPropertyChanged("SignalPL")
                                                                   x.Value.NotifyPropertyChanged("OverallPL")
                                                                   x.Value.NotifyPropertyChanged("TotalExecutedOrders")
                                                                   x.Value.NotifyPropertyChanged("ActiveInstrument")
                                                               End If
                                                               If associatedStrategyInstrument.TradableInstrument.LastTick IsNot Nothing AndAlso associatedStrategyInstrument.TradableInstrument.LastTick.Timestamp <> x.Value.GetDirtyTimestamp Then x.Value.NotifyPropertyChanged("Timestamp")
                                                               If associatedStrategyInstrument.TradableInstrument.LastTick IsNot Nothing AndAlso associatedStrategyInstrument.TradableInstrument.LastTick.Timestamp IsNot Nothing AndAlso associatedStrategyInstrument.TradableInstrument.LastTick.Timestamp.HasValue AndAlso Utilities.Time.IsDateTimeEqualTillMinutes(associatedStrategyInstrument.TradableInstrument.LastTick.Timestamp.Value, x.Value.GetDirtyLastCandleTime) Then x.Value.NotifyPropertyChanged("LastCandleTime")
                                                               If associatedStrategyInstrument.TradableInstrument.TradingSymbol IsNot Nothing AndAlso associatedStrategyInstrument.TradableInstrument.TradingSymbol <> x.Value.GetDirtyTradingSymbol Then x.Value.NotifyPropertyChanged("TradingSymbol")
                                                               Return True
                                                           End Function)
                    End If
                End If
            Catch cex As OperationCanceledException
                logger.Error(cex)
                Me.ParentStrategy.ParentController.OrphanException = cex
            Catch ex As Exception
                logger.Error("Strategy Instrument:{0}, error:{1}", Me.ToString, ex.ToString)
                Throw ex
            End Try
        End Function
        Public Async Function UIRefresh(ByVal activityToChange As ActivityDashboard.Activity) As Task
            Try
                Await Task.Delay(0, _cts.Token).ConfigureAwait(False)
                If activityToChange IsNot Nothing Then
                    If Not Utilities.Time.IsDateTimeEqualTillMinutes(activityToChange.RequestTime, activityToChange.PreviousActivityAttributes.RequestTime) Then
                        Select Case activityToChange.TypeOfActivity
                            Case ActivityDashboard.ActivityType.Entry
                                activityToChange.ParentActivityDashboard.NotifyPropertyChanged("EntryRequestTime")
                            Case ActivityDashboard.ActivityType.TargetModify
                                activityToChange.ParentActivityDashboard.NotifyPropertyChanged("TargetModifyRequestTime")
                            Case ActivityDashboard.ActivityType.StoplossModify
                                activityToChange.ParentActivityDashboard.NotifyPropertyChanged("StoplossModifyRequestTime")
                            Case ActivityDashboard.ActivityType.Cancel
                                activityToChange.ParentActivityDashboard.NotifyPropertyChanged("CancelRequestTime")
                            Case Else
                                Throw New NotImplementedException
                        End Select
                    End If
                    If activityToChange.RequestStatus <> activityToChange.PreviousActivityAttributes.RequestStatus Then
                        Select Case activityToChange.TypeOfActivity
                            Case ActivityDashboard.ActivityType.Entry
                                activityToChange.ParentActivityDashboard.NotifyPropertyChanged("EntryRequestStatus")
                            Case ActivityDashboard.ActivityType.TargetModify
                                activityToChange.ParentActivityDashboard.NotifyPropertyChanged("TargetModifyRequestStatus")
                            Case ActivityDashboard.ActivityType.StoplossModify
                                activityToChange.ParentActivityDashboard.NotifyPropertyChanged("StoplossModifyRequestStatus")
                            Case ActivityDashboard.ActivityType.Cancel
                                activityToChange.ParentActivityDashboard.NotifyPropertyChanged("CancelRequestStatus")
                            Case Else
                                Throw New NotImplementedException
                        End Select
                    End If
                End If
            Catch cex As OperationCanceledException
                logger.Error(cex)
                Me.ParentStrategy.ParentController.OrphanException = cex
            Catch ex As Exception
                logger.Error("Strategy Instrument:{0}, error:{1}", Me.ToString, ex.ToString)
                Throw ex
            End Try
        End Function
#End Region

#End Region

#Region "Private Functions"
        Private Async Function AddOrUpdateEntryActivity(ByVal activityTag As String,
                                             ByVal associatedStrategyInstrument As StrategyInstrument,
                                             ByVal associatedOrderID As String,
                                             Optional ByVal signalGeneratedTime As Date = Nothing,
                                             Optional ByVal requestTime As Date = Nothing,
                                             Optional ByVal receivedTime As Date = Nothing,
                                             Optional ByVal requestStatus As ActivityDashboard.SignalStatusType = ActivityDashboard.SignalStatusType.None,
                                             Optional ByVal requestRemarks As String = Nothing,
                                             Optional ByVal lastException As Exception = Nothing) As Task

            Dim currentActivity As New ActivityDashboard(associatedStrategyInstrument)

            Dim existingActivities As ActivityDashboard = Me.ActivityDetails.GetOrAdd(activityTag, currentActivity)

            If associatedOrderID IsNot Nothing Then existingActivities.ParentOrderID = associatedOrderID
            If signalGeneratedTime <> Nothing OrElse signalGeneratedTime <> Date.MinValue Then existingActivities.SignalGeneratedTime = signalGeneratedTime
            If requestTime <> Nothing OrElse requestTime <> Date.MinValue Then existingActivities.EntryActivity.RequestTime = requestTime
            If receivedTime <> Nothing OrElse receivedTime <> Date.MinValue Then existingActivities.EntryActivity.ReceivedTime = If(receivedTime.Equals(_defaultDateValue), Date.MinValue, receivedTime)
            If requestStatus <> ActivityDashboard.SignalStatusType.None Then existingActivities.EntryActivity.RequestStatus = requestStatus
            If requestRemarks IsNot Nothing Then existingActivities.EntryActivity.RequestRemarks = requestRemarks
            If lastException IsNot Nothing Then existingActivities.EntryActivity.LastException = If(lastException.Equals(_defaultExceptionValue), Nothing, lastException)

            Me.ActivityDetails.AddOrUpdate(activityTag, existingActivities, Function(key, value) existingActivities)
            Await UIRefresh(Me.ActivityDetails(activityTag).EntryActivity).ConfigureAwait(False)
        End Function
        Private Async Function AddOrUpdateTargetModifyActivity(ByVal activityTag As String,
                                                    ByVal associatedStrategyInstrument As StrategyInstrument,
                                                    ByVal associatedOrderID As String,
                                                    Optional ByVal requestTime As Date = Nothing,
                                                    Optional ByVal receivedTime As Date = Nothing,
                                                    Optional ByVal requestStatus As ActivityDashboard.SignalStatusType = ActivityDashboard.SignalStatusType.None,
                                                    Optional ByVal requestRemarks As String = Nothing,
                                                    Optional ByVal lastException As Exception = Nothing,
                                                    Optional ByVal price As Decimal = Decimal.MinValue) As Task

            Dim currentActivity As New ActivityDashboard(associatedStrategyInstrument)
            Dim existingActivities As ActivityDashboard = Me.ActivityDetails.GetOrAdd(activityTag, currentActivity)

            If associatedOrderID IsNot Nothing Then existingActivities.ParentOrderID = associatedOrderID
            If requestTime <> Nothing OrElse requestTime <> Date.MinValue Then existingActivities.TargetModifyActivity.RequestTime = requestTime
            If receivedTime <> Nothing OrElse receivedTime <> Date.MinValue Then existingActivities.TargetModifyActivity.ReceivedTime = If(receivedTime.Equals(_defaultDateValue), Date.MinValue, receivedTime)
            If requestStatus <> ActivityDashboard.SignalStatusType.None Then existingActivities.TargetModifyActivity.RequestStatus = requestStatus
            If requestRemarks IsNot Nothing Then existingActivities.TargetModifyActivity.RequestRemarks = requestRemarks
            If lastException IsNot Nothing Then existingActivities.TargetModifyActivity.LastException = If(lastException.Equals(_defaultExceptionValue), Nothing, lastException)

            Me.ActivityDetails.AddOrUpdate(activityTag, existingActivities, Function(key, value) existingActivities)
            Await UIRefresh(Me.ActivityDetails(activityTag).TargetModifyActivity).ConfigureAwait(False)
        End Function
        Private Async Function AddOrUpdateStoplossModifyActivity(ByVal activityTag As String,
                                                      ByVal associatedStrategyInstrument As StrategyInstrument,
                                                      ByVal associatedOrderID As String,
                                                      Optional ByVal requestTime As Date = Nothing,
                                                      Optional ByVal receivedTime As Date = Nothing,
                                                      Optional ByVal requestStatus As ActivityDashboard.SignalStatusType = ActivityDashboard.SignalStatusType.None,
                                                      Optional ByVal requestRemarks As String = Nothing,
                                                      Optional ByVal lastException As Exception = Nothing,
                                                      Optional ByVal triggerPrice As Decimal = Decimal.MinValue) As Task

            Dim currentActivity As New ActivityDashboard(associatedStrategyInstrument)
            Dim existingActivities As ActivityDashboard = Me.ActivityDetails.GetOrAdd(activityTag, currentActivity)

            If associatedOrderID IsNot Nothing Then existingActivities.ParentOrderID = associatedOrderID
            If requestTime <> Nothing OrElse requestTime <> Date.MinValue Then existingActivities.StoplossModifyActivity.RequestTime = requestTime
            If receivedTime <> Nothing OrElse receivedTime <> Date.MinValue Then existingActivities.StoplossModifyActivity.ReceivedTime = If(receivedTime.Equals(_defaultDateValue), Date.MinValue, receivedTime)
            If requestStatus <> ActivityDashboard.SignalStatusType.None Then existingActivities.StoplossModifyActivity.RequestStatus = requestStatus
            If requestRemarks IsNot Nothing Then existingActivities.StoplossModifyActivity.RequestRemarks = requestRemarks
            If lastException IsNot Nothing Then existingActivities.StoplossModifyActivity.LastException = If(lastException.Equals(_defaultExceptionValue), Nothing, lastException)
            If triggerPrice <> Decimal.MinValue Then existingActivities.StoplossModifyActivity.Supporting = triggerPrice

            Me.ActivityDetails.AddOrUpdate(activityTag, existingActivities, Function(key, value) existingActivities)
            Await UIRefresh(Me.ActivityDetails(activityTag).StoplossModifyActivity).ConfigureAwait(False)
        End Function
        Private Async Function AddOrUpdateCancelActivity(ByVal activityTag As String,
                                              ByVal associatedStrategyInstrument As StrategyInstrument,
                                              ByVal associatedOrderID As String,
                                              Optional ByVal requestTime As Date = Nothing,
                                              Optional ByVal receivedTime As Date = Nothing,
                                              Optional ByVal requestStatus As ActivityDashboard.SignalStatusType = ActivityDashboard.SignalStatusType.None,
                                              Optional ByVal requestRemarks As String = Nothing,
                                              Optional ByVal lastException As Exception = Nothing) As Task

            Dim currentActivity As New ActivityDashboard(associatedStrategyInstrument)
            Dim existingActivities As ActivityDashboard = Me.ActivityDetails.GetOrAdd(activityTag, currentActivity)

            If associatedOrderID IsNot Nothing Then existingActivities.ParentOrderID = associatedOrderID
            If requestTime <> Nothing OrElse requestTime <> Date.MinValue Then existingActivities.CancelActivity.RequestTime = requestTime
            If receivedTime <> Nothing OrElse receivedTime <> Date.MinValue Then existingActivities.CancelActivity.ReceivedTime = If(receivedTime.Equals(_defaultDateValue), Date.MinValue, receivedTime)
            If requestStatus <> ActivityDashboard.SignalStatusType.None Then existingActivities.CancelActivity.RequestStatus = requestStatus
            If requestRemarks IsNot Nothing Then existingActivities.CancelActivity.RequestRemarks = requestRemarks
            If lastException IsNot Nothing Then existingActivities.CancelActivity.LastException = If(lastException.Equals(_defaultExceptionValue), Nothing, lastException)

            Me.ActivityDetails.AddOrUpdate(activityTag, existingActivities, Function(key, value) existingActivities)
            Await UIRefresh(Me.ActivityDetails(activityTag).CancelActivity).ConfigureAwait(False)
        End Function
#End Region

    End Class
End Namespace