Imports System.ComponentModel
Imports System.Threading
Imports Algo2TradeCore.Controller
Imports Algo2TradeCore.Entities
Imports NLog

Namespace Strategies
    Public Class SignalStateManager
        Implements INotifyPropertyChanged
        Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

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
        Public Sub HandleEntryActivity(ByVal activityTag As String,
                                       ByVal tradingSymbol As String,
                                       ByVal signalGeneratedTime As Date,
                                       ByVal requestTime As Date)
            AddOrUpdateEntryActivity(activityTag:=activityTag,
                                     tradingSymbol:=tradingSymbol,
                                     signalGeneratedTime:=signalGeneratedTime,
                                     requestTime:=requestTime,
                                     receivedTime:=_defaultDateValue,
                                     requestStatus:=ActivityDashboard.SignalStatusType.Handled,
                                     lastException:=_defaultExceptionValue)
        End Sub
        Public Sub ActivateEntryActivity(ByVal activityTag As String,
                                         ByVal receivedTime As Date)
            AddOrUpdateEntryActivity(activityTag:=activityTag,
                                     receivedTime:=receivedTime,
                                     requestStatus:=ActivityDashboard.SignalStatusType.Activated)
        End Sub
        Public Sub DiscardEntryActivity(ByVal activityTag As String,
                                        ByVal receivedTime As Date,
                                        ByVal lastException As Exception)
            AddOrUpdateEntryActivity(activityTag:=activityTag,
                                     receivedTime:=receivedTime,
                                     requestStatus:=ActivityDashboard.SignalStatusType.Discarded,
                                     lastException:=lastException)
        End Sub
        Public Sub CancelEntryActivity(ByVal activityTag As String,
                                       ByVal requestRemarks As String)
            AddOrUpdateEntryActivity(activityTag:=activityTag,
                                     requestStatus:=ActivityDashboard.SignalStatusType.Cancelled,
                                     requestRemarks:=requestRemarks)
        End Sub
        Public Sub RejectEntryActivity(ByVal activityTag As String,
                                       ByVal requestRemarks As String)
            AddOrUpdateEntryActivity(activityTag:=activityTag,
                                     requestStatus:=ActivityDashboard.SignalStatusType.Rejected,
                                     requestRemarks:=requestRemarks)
        End Sub
        Public Sub CompleteEntryActivity(ByVal activityTag As String,
                                         ByVal requestRemarks As String)
            AddOrUpdateEntryActivity(activityTag:=activityTag,
                                     requestStatus:=ActivityDashboard.SignalStatusType.Completed,
                                     requestRemarks:=requestRemarks)
        End Sub
        Public Sub RunningEntryActivity(ByVal activityTag As String,
                                        ByVal requestRemarks As String)
            AddOrUpdateEntryActivity(activityTag:=activityTag,
                                     requestStatus:=ActivityDashboard.SignalStatusType.Running,
                                     requestRemarks:=requestRemarks)
        End Sub
#End Region

#Region "Target Modify Activity"
        Public Sub HandleTargetModifyActivity(ByVal activityTag As String,
                                              ByVal requestTime As Date,
                                              ByVal price As Decimal)
            AddOrUpdateTargetModifyActivity(activityTag:=activityTag,
                                            requestTime:=requestTime,
                                            receivedTime:=_defaultDateValue,
                                            requestStatus:=ActivityDashboard.SignalStatusType.Handled,
                                            lastException:=_defaultExceptionValue,
                                            price:=price)
        End Sub
        Public Sub ActivateTargetModifyActivity(ByVal activityTag As String,
                                                ByVal receivedTime As Date)
            AddOrUpdateTargetModifyActivity(activityTag:=activityTag,
                                            receivedTime:=receivedTime,
                                            requestStatus:=ActivityDashboard.SignalStatusType.Activated)
        End Sub
        Public Sub RejectTargetModifyActivity(ByVal activityTag As String,
                                              ByVal requestRemarks As String)
            AddOrUpdateTargetModifyActivity(activityTag:=activityTag,
                                            requestStatus:=ActivityDashboard.SignalStatusType.Rejected,
                                            requestRemarks:=requestRemarks)
        End Sub
        Public Sub CompleteTargetModifyActivity(ByVal activityTag As String,
                                                ByVal requestRemarks As String)
            AddOrUpdateTargetModifyActivity(activityTag:=activityTag,
                                            requestStatus:=ActivityDashboard.SignalStatusType.Completed,
                                            requestRemarks:=requestRemarks)
        End Sub
#End Region

#Region "Stoploss Modify Activity"
        Public Sub HandleStoplossModifyActivity(ByVal activityTag As String,
                                                ByVal requestTime As Date,
                                                ByVal triggerprice As Decimal)
            AddOrUpdateStoplossModifyActivity(activityTag:=activityTag,
                                              requestTime:=requestTime,
                                              receivedTime:=_defaultDateValue,
                                              requestStatus:=ActivityDashboard.SignalStatusType.Handled,
                                              lastException:=_defaultExceptionValue,
                                              triggerPrice:=triggerprice)
        End Sub
        Public Sub ActivateStoplossModifyActivity(ByVal activityTag As String,
                                                  ByVal receivedTime As Date)
            AddOrUpdateStoplossModifyActivity(activityTag:=activityTag,
                                              receivedTime:=receivedTime,
                                              requestStatus:=ActivityDashboard.SignalStatusType.Activated)
        End Sub
        Public Sub RejectStoplossModifyActivity(ByVal activityTag As String,
                                                ByVal requestRemarks As String)
            AddOrUpdateStoplossModifyActivity(activityTag:=activityTag,
                                              requestStatus:=ActivityDashboard.SignalStatusType.Rejected,
                                              requestRemarks:=requestRemarks)
        End Sub
        Public Sub CompleteStoplossModifyActivity(ByVal activityTag As String,
                                                  ByVal requestRemarks As String)
            AddOrUpdateStoplossModifyActivity(activityTag:=activityTag,
                                              requestStatus:=ActivityDashboard.SignalStatusType.Completed,
                                              requestRemarks:=requestRemarks)
        End Sub
#End Region

#Region "Cancel Activity"
        Public Sub HandleCancelActivity(ByVal activityTag As String,
                                        ByVal requestTime As Date)
            AddOrUpdateCancelActivity(activityTag:=activityTag,
                                      requestTime:=requestTime,
                                      receivedTime:=_defaultDateValue,
                                      requestStatus:=ActivityDashboard.SignalStatusType.Handled,
                                      lastException:=_defaultExceptionValue)
        End Sub
        Public Sub ActivateCancelActivity(ByVal activityTag As String,
                                          ByVal receivedTime As Date)
            AddOrUpdateCancelActivity(activityTag:=activityTag,
                                      receivedTime:=receivedTime,
                                      requestStatus:=ActivityDashboard.SignalStatusType.Activated)
        End Sub
        Public Sub RejectCancelActivity(ByVal activityTag As String,
                                        ByVal requestRemarks As String)
            AddOrUpdateCancelActivity(activityTag:=activityTag,
                                      requestStatus:=ActivityDashboard.SignalStatusType.Rejected,
                                      requestRemarks:=requestRemarks)
        End Sub
        Public Sub CompleteCancelActivity(ByVal activityTag As String,
                                          ByVal requestRemarks As String)
            AddOrUpdateCancelActivity(activityTag:=activityTag,
                                      requestStatus:=ActivityDashboard.SignalStatusType.Completed,
                                      requestRemarks:=requestRemarks)
        End Sub
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
        Public Async Function UIRefresh(ByVal instrument As IInstrument) As Task
            Try
                Await Task.Delay(0, _cts.Token).ConfigureAwait(False)
                If ActivityDetails IsNot Nothing AndAlso ActivityDetails.Count > 0 Then
                    Dim currentInstrumentActivities As IEnumerable(Of KeyValuePair(Of String, ActivityDashboard)) =
                        Me.ActivityDetails.Where(Function(x)
                                                     Dim key As String = Convert.ToInt64(x.Key, 16)
                                                     Return key.Substring(0, 4).Equals(String.Format("{0}{1:D3}", Me.ParentStrategy.StrategyIdentifier, Me.ParentController.InstrumentMappingTable(instrument.InstrumentIdentifier)))
                                                 End Function)
                    If currentInstrumentActivities IsNot Nothing AndAlso currentInstrumentActivities.Count > 0 Then
                        currentInstrumentActivities.Select(Function(x)
                                                               If instrument.LastTick IsNot Nothing AndAlso instrument.LastTick.LastPrice <> x.Value.LastPrice Then NotifyPropertyChanged("LastPrice")
                                                               If instrument.LastTick IsNot Nothing AndAlso instrument.LastTick.Timestamp <> x.Value.Timestamp Then NotifyPropertyChanged("Timestamp")
                                                               If instrument.LastTick IsNot Nothing AndAlso instrument.LastTick.Timestamp IsNot Nothing AndAlso instrument.LastTick.Timestamp.HasValue AndAlso Utilities.Time.IsDateTimeEqualTillMinutes(instrument.LastTick.Timestamp.Value, x.Value.LastCandleTime) Then NotifyPropertyChanged("LastCandleTime")
                                                               If instrument.LastTick IsNot Nothing AndAlso instrument.LastTick.LastPrice <> x.Value.LastPrice Then NotifyPropertyChanged("ProfitLossOfSignal")
                                                               If instrument.LastTick IsNot Nothing AndAlso instrument.LastTick.LastPrice <> x.Value.LastPrice Then NotifyPropertyChanged("OverallProfitLoss")
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
                                NotifyPropertyChanged("EntryRequestTime")
                            Case ActivityDashboard.ActivityType.TargetModify
                                NotifyPropertyChanged("TargetModifyRequestTime")
                            Case ActivityDashboard.ActivityType.StoplossModify
                                NotifyPropertyChanged("StoplossModifyRequestTime")
                            Case ActivityDashboard.ActivityType.Cancel
                                NotifyPropertyChanged("CancelRequestTime")
                            Case Else
                                Throw New NotImplementedException
                        End Select
                    End If
                    If activityToChange.RequestStatus <> activityToChange.PreviousActivityAttributes.RequestStatus Then
                        Select Case activityToChange.TypeOfActivity
                            Case ActivityDashboard.ActivityType.Entry
                                NotifyPropertyChanged("EntryRequestStatus")
                            Case ActivityDashboard.ActivityType.TargetModify
                                NotifyPropertyChanged("TargetModifyRequestStatus")
                            Case ActivityDashboard.ActivityType.StoplossModify
                                NotifyPropertyChanged("StoplossModifyRequestStatus")
                            Case ActivityDashboard.ActivityType.Cancel
                                NotifyPropertyChanged("CancelRequestStatus")
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
        Private Sub AddOrUpdateEntryActivity(ByVal activityTag As String,
                                            Optional ByVal tradingSymbol As String = Nothing,
                                            Optional ByVal signalGeneratedTime As Date = Nothing,
                                            Optional ByVal requestTime As Date = Nothing,
                                            Optional ByVal receivedTime As Date = Nothing,
                                            Optional ByVal requestStatus As ActivityDashboard.SignalStatusType = ActivityDashboard.SignalStatusType.None,
                                            Optional ByVal requestRemarks As String = Nothing,
                                            Optional ByVal lastException As Exception = Nothing)

            Dim currentActivity As New ActivityDashboard
            Dim existingActivities As ActivityDashboard = Me.ActivityDetails.GetOrAdd(activityTag, currentActivity)

            If tradingSymbol IsNot Nothing Then existingActivities.TradingSymbol = tradingSymbol
            If signalGeneratedTime <> Nothing OrElse signalGeneratedTime <> Date.MinValue Then existingActivities.SignalGeneratedTime = signalGeneratedTime
            If requestTime <> Nothing OrElse requestTime <> Date.MinValue Then existingActivities.EntryActivity.RequestTime = requestTime
            If receivedTime <> Nothing OrElse receivedTime <> Date.MinValue Then existingActivities.EntryActivity.ReceivedTime = If(receivedTime.Equals(_defaultDateValue), Date.MinValue, receivedTime)
            If requestStatus <> ActivityDashboard.SignalStatusType.None Then existingActivities.EntryActivity.RequestStatus = requestStatus
            If requestRemarks IsNot Nothing Then existingActivities.EntryActivity.RequestRemarks = requestRemarks
            If lastException IsNot Nothing Then existingActivities.EntryActivity.LastException = If(lastException.Equals(_defaultExceptionValue), Nothing, lastException)

            Me.ActivityDetails.AddOrUpdate(activityTag, existingActivities, Function(key, value) existingActivities)
        End Sub
        Private Sub AddOrUpdateTargetModifyActivity(ByVal activityTag As String,
                                                    Optional ByVal requestTime As Date = Nothing,
                                                    Optional ByVal receivedTime As Date = Nothing,
                                                    Optional ByVal requestStatus As ActivityDashboard.SignalStatusType = ActivityDashboard.SignalStatusType.None,
                                                    Optional ByVal requestRemarks As String = Nothing,
                                                    Optional ByVal lastException As Exception = Nothing,
                                                    Optional ByVal price As Decimal = Decimal.MinValue)

            Dim currentActivity As New ActivityDashboard
            Dim existingActivities As ActivityDashboard = Me.ActivityDetails.GetOrAdd(activityTag, currentActivity)

            If requestTime <> Nothing OrElse requestTime <> Date.MinValue Then existingActivities.TargetModifyActivity.RequestTime = requestTime
            If receivedTime <> Nothing OrElse receivedTime <> Date.MinValue Then existingActivities.TargetModifyActivity.ReceivedTime = If(receivedTime.Equals(_defaultDateValue), Date.MinValue, receivedTime)
            If requestStatus <> ActivityDashboard.SignalStatusType.None Then existingActivities.TargetModifyActivity.RequestStatus = requestStatus
            If requestRemarks IsNot Nothing Then existingActivities.TargetModifyActivity.RequestRemarks = requestRemarks
            If lastException IsNot Nothing Then existingActivities.TargetModifyActivity.LastException = If(lastException.Equals(_defaultExceptionValue), Nothing, lastException)

            Me.ActivityDetails.AddOrUpdate(activityTag, existingActivities, Function(key, value) existingActivities)
        End Sub
        Private Sub AddOrUpdateStoplossModifyActivity(ByVal activityTag As String,
                                                      Optional ByVal requestTime As Date = Nothing,
                                                      Optional ByVal receivedTime As Date = Nothing,
                                                      Optional ByVal requestStatus As ActivityDashboard.SignalStatusType = ActivityDashboard.SignalStatusType.None,
                                                      Optional ByVal requestRemarks As String = Nothing,
                                                      Optional ByVal lastException As Exception = Nothing,
                                                      Optional ByVal triggerPrice As Decimal = Decimal.MinValue)

            Dim currentActivity As New ActivityDashboard
            Dim existingActivities As ActivityDashboard = Me.ActivityDetails.GetOrAdd(activityTag, currentActivity)

            If requestTime <> Nothing OrElse requestTime <> Date.MinValue Then existingActivities.StoplossModifyActivity.RequestTime = requestTime
            If receivedTime <> Nothing OrElse receivedTime <> Date.MinValue Then existingActivities.StoplossModifyActivity.ReceivedTime = If(receivedTime.Equals(_defaultDateValue), Date.MinValue, receivedTime)
            If requestStatus <> ActivityDashboard.SignalStatusType.None Then existingActivities.StoplossModifyActivity.RequestStatus = requestStatus
            If requestRemarks IsNot Nothing Then existingActivities.StoplossModifyActivity.RequestRemarks = requestRemarks
            If lastException IsNot Nothing Then existingActivities.StoplossModifyActivity.LastException = If(lastException.Equals(_defaultExceptionValue), Nothing, lastException)
            If triggerPrice <> Decimal.MinValue Then existingActivities.StoplossModifyActivity.Supporting = triggerPrice

            Me.ActivityDetails.AddOrUpdate(activityTag, existingActivities, Function(key, value) existingActivities)
        End Sub
        Private Sub AddOrUpdateCancelActivity(ByVal activityTag As String,
                                              Optional ByVal requestTime As Date = Nothing,
                                              Optional ByVal receivedTime As Date = Nothing,
                                              Optional ByVal requestStatus As ActivityDashboard.SignalStatusType = ActivityDashboard.SignalStatusType.None,
                                              Optional ByVal requestRemarks As String = Nothing,
                                              Optional ByVal lastException As Exception = Nothing)

            Dim currentActivity As New ActivityDashboard
            Dim existingActivities As ActivityDashboard = Me.ActivityDetails.GetOrAdd(activityTag, currentActivity)

            If requestTime <> Nothing OrElse requestTime <> Date.MinValue Then existingActivities.CancelActivity.RequestTime = requestTime
            If receivedTime <> Nothing OrElse receivedTime <> Date.MinValue Then existingActivities.CancelActivity.ReceivedTime = If(receivedTime.Equals(_defaultDateValue), Date.MinValue, receivedTime)
            If requestStatus <> ActivityDashboard.SignalStatusType.None Then existingActivities.CancelActivity.RequestStatus = requestStatus
            If requestRemarks IsNot Nothing Then existingActivities.CancelActivity.RequestRemarks = requestRemarks
            If lastException IsNot Nothing Then existingActivities.CancelActivity.LastException = If(lastException.Equals(_defaultExceptionValue), Nothing, lastException)

            Me.ActivityDetails.AddOrUpdate(activityTag, existingActivities, Function(key, value) existingActivities)
        End Sub
        Protected Sub NotifyPropertyChanged(ByVal p As String)
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(p))
        End Sub
#End Region

    End Class
End Namespace