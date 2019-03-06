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
        Public Sub HandleEntrySignal(ByVal activityTag As String,
                                ByVal tradingSymbol As String,
                                ByVal signalGeneratedTime As Date,
                                ByVal entryRequestTime As Date)
            AddOrUpdateEntryActivity(activityTag:=activityTag,
                                tradingSymbol:=tradingSymbol,
                                signalGeneratedTime:=signalGeneratedTime,
                                entryRequestTime:=entryRequestTime,
                                entryReceivedTime:=_defaultDateValue,
                                signalStatus:=ActivityDashboard.SignalStatusType.Handled,
                                lastException:=_defaultExceptionValue)
        End Sub
        Public Sub ActivateEntrySignal(ByVal activityTag As String,
                                  ByVal entryReceivedTime As Date)
            AddOrUpdateEntryActivity(activityTag:=activityTag,
                                entryReceivedTime:=entryReceivedTime,
                                signalStatus:=ActivityDashboard.SignalStatusType.Activated)
        End Sub
        Public Sub DiscardEntrySignal(ByVal activityTag As String,
                                 ByVal entryReceivedTime As Date,
                                 ByVal lastException As Exception)
            AddOrUpdateEntryActivity(activityTag:=activityTag,
                                entryReceivedTime:=entryReceivedTime,
                                signalStatus:=ActivityDashboard.SignalStatusType.Discarded,
                                lastException:=lastException)
        End Sub
        Public Sub HandleTargetModifySignal(ByVal activityTag As String,
                                            ByVal entryRequestTime As Date,
                                            ByVal price As Decimal)
            AddOrUpdateTargetModifyActivity(activityTag:=activityTag,
                                            requestTime:=entryRequestTime,
                                            receivedTime:=_defaultDateValue,
                                            requestStatus:=ActivityDashboard.SignalStatusType.Handled,
                                            lastException:=_defaultExceptionValue,
                                            price:=price)
        End Sub
        Public Sub ActivateTargetModifySignal(ByVal activityTag As String,
                                             ByVal entryReceivedTime As Date)
            AddOrUpdateTargetModifyActivity(activityTag:=activityTag,
                                            receivedTime:=entryReceivedTime,
                                            requestStatus:=ActivityDashboard.SignalStatusType.Activated)
        End Sub
        Public Sub HandleStoplossModifySignal(ByVal activityTag As String,
                                            ByVal entryRequestTime As Date,
                                            ByVal triggerprice As Decimal)
            AddOrUpdateStoplossModifyActivity(activityTag:=activityTag,
                                            requestTime:=entryRequestTime,
                                            receivedTime:=_defaultDateValue,
                                            requestStatus:=ActivityDashboard.SignalStatusType.Handled,
                                            lastException:=_defaultExceptionValue,
                                            triggerPrice:=triggerprice)
        End Sub
        Public Sub ActivateStoplossModifySignal(ByVal activityTag As String,
                                             ByVal entryReceivedTime As Date)
            AddOrUpdateStoplossModifyActivity(activityTag:=activityTag,
                                            receivedTime:=entryReceivedTime,
                                            requestStatus:=ActivityDashboard.SignalStatusType.Activated)
        End Sub
        Private Sub AddOrUpdateEntryActivity(ByVal activityTag As String,
                                        Optional ByVal tradingSymbol As String = Nothing,
                                        Optional ByVal signalGeneratedTime As Date = Nothing,
                                        Optional ByVal entryRequestTime As Date = Nothing,
                                        Optional ByVal entryReceivedTime As Date = Nothing,
                                        Optional ByVal signalStatus As ActivityDashboard.SignalStatusType = ActivityDashboard.SignalStatusType.None,
                                        Optional ByVal signalRemarks As String = Nothing,
                                        Optional ByVal profitLossOfSignal As Decimal = Decimal.MinValue,
                                        Optional ByVal activeInstrument As Boolean? = Nothing,
                                        Optional ByVal totalExecutedOrders As Integer = Integer.MinValue,
                                        Optional ByVal overallProfitLoss As Decimal = Decimal.MinValue,
                                        Optional ByVal lastException As Exception = Nothing)

            Dim currentActivity As New ActivityDashboard
            Dim existingActivities As ActivityDashboard = Me.ActivityDetails.GetOrAdd(activityTag, currentActivity)

            If tradingSymbol IsNot Nothing Then existingActivities.TradingSymbol = tradingSymbol
            If signalGeneratedTime <> Nothing OrElse signalGeneratedTime <> Date.MinValue Then existingActivities.SignalGeneratedTime = signalGeneratedTime
            If entryRequestTime <> Nothing OrElse entryRequestTime <> Date.MinValue Then existingActivities.EntryActivity.RequestTime = entryRequestTime
            If entryReceivedTime <> Nothing OrElse entryReceivedTime <> Date.MinValue Then existingActivities.EntryActivity.ReceivedTime = If(entryReceivedTime.Equals(_defaultDateValue), Date.MinValue, entryReceivedTime)
            If signalStatus <> ActivityDashboard.SignalStatusType.None Then existingActivities.EntryActivity.RequestStatus = signalStatus
            If signalRemarks IsNot Nothing Then existingActivities.EntryActivity.RequestRemarks = signalRemarks
            If profitLossOfSignal <> Decimal.MinValue Then existingActivities.ProfitLossOfSignal = profitLossOfSignal
            If activeInstrument IsNot Nothing Then existingActivities.ActiveInstrument = activeInstrument.Value
            If totalExecutedOrders <> Integer.MinValue Then existingActivities.TotalExecutedOrders = totalExecutedOrders
            If overallProfitLoss <> Decimal.MinValue Then existingActivities.OverallProfitLoss = overallProfitLoss
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
    End Class
End Namespace