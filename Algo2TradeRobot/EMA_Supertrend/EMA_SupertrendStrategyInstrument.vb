Imports System.Threading
Imports Algo2TradeCore.Adapter
Imports Algo2TradeCore.Entities
Imports Algo2TradeCore.Strategies
Imports Utilities.Numbers
Imports NLog
Imports Algo2TradeCore.Entities.Indicators

Public Class EMA_SupertrendStrategyInstrument
    Inherits StrategyInstrument
    Implements IDisposable

#Region "Logging and Status Progress"
    Public Shared Shadows logger As Logger = LogManager.GetCurrentClassLogger
#End Region

    Private lastPrevPayloadPlaceOrder As String = ""
    Private lastPrevPayloadExitOrder As String = ""
    Private ReadOnly _dummyFastEMAConsumer As EMAConsumer
    Private ReadOnly _dummySlowEMAConsumer As EMAConsumer
    Private ReadOnly _dummySupertrendConsumer As SupertrendConsumer
    Private ReadOnly _useST As Boolean = True
    Public Sub New(ByVal associatedInstrument As IInstrument, ByVal associatedParentStrategy As Strategy, ByVal canceller As CancellationTokenSource)
        MyBase.New(associatedInstrument, associatedParentStrategy, canceller)
        Select Case Me.ParentStrategy.ParentController.BrokerSource
            Case APISource.Zerodha
                _APIAdapter = New ZerodhaAdapter(ParentStrategy.ParentController, _cts)
            Case APISource.Upstox
                Throw New NotImplementedException
            Case APISource.None
                Throw New NotImplementedException
        End Select
        AddHandler _APIAdapter.Heartbeat, AddressOf OnHeartbeat
        AddHandler _APIAdapter.WaitingFor, AddressOf OnWaitingFor
        AddHandler _APIAdapter.DocumentRetryStatus, AddressOf OnDocumentRetryStatus
        AddHandler _APIAdapter.DocumentDownloadComplete, AddressOf OnDocumentDownloadComplete
        RawPayloadDependentConsumers = New List(Of IPayloadConsumer)
        If Me.ParentStrategy.IsStrategyCandleStickBased Then
            If Me.ParentStrategy.UserSettings.SignalTimeFrame > 0 Then
                Dim chartConsumer As PayloadToChartConsumer = New PayloadToChartConsumer(Me.ParentStrategy.UserSettings.SignalTimeFrame)
                chartConsumer.OnwardLevelConsumers = New List(Of IPayloadConsumer) From
                {New EMAConsumer(chartConsumer, CType(Me.ParentStrategy.UserSettings, EMA_SupertrendStrategyUserInputs).FastEMAPeriod, TypeOfField.Close),
                 New EMAConsumer(chartConsumer, CType(Me.ParentStrategy.UserSettings, EMA_SupertrendStrategyUserInputs).SlowEMAPeriod, TypeOfField.Close),
                 New SupertrendConsumer(chartConsumer, CType(Me.ParentStrategy.UserSettings, EMA_SupertrendStrategyUserInputs).SupertrendPeriod, CType(Me.ParentStrategy.UserSettings, EMA_SupertrendStrategyUserInputs).SupertrendMultiplier)}
                RawPayloadDependentConsumers.Add(chartConsumer)
                _dummyFastEMAConsumer = New EMAConsumer(chartConsumer, CType(Me.ParentStrategy.UserSettings, EMA_SupertrendStrategyUserInputs).FastEMAPeriod, TypeOfField.Close)
                _dummySlowEMAConsumer = New EMAConsumer(chartConsumer, CType(Me.ParentStrategy.UserSettings, EMA_SupertrendStrategyUserInputs).SlowEMAPeriod, TypeOfField.Close)
                _dummySupertrendConsumer = New SupertrendConsumer(chartConsumer, CType(Me.ParentStrategy.UserSettings, EMA_SupertrendStrategyUserInputs).SupertrendPeriod, CType(Me.ParentStrategy.UserSettings, EMA_SupertrendStrategyUserInputs).SupertrendMultiplier)
            Else
                Throw New ApplicationException(String.Format("Signal Timeframe is 0 or Nothing, does not adhere to the strategy:{0}", Me.ParentStrategy.ToString))
            End If
        End If
    End Sub
    Public Overrides Async Function MonitorAsync() As Task
        Try
            While True
                If Me.ParentStrategy.ParentController.OrphanException IsNot Nothing Then
                    Throw Me.ParentStrategy.ParentController.OrphanException
                End If

                _cts.Token.ThrowIfCancellationRequested()
                Dim placeOrderTrigger As Tuple(Of ExecuteCommandAction, PlaceOrderParameters) = Await IsTriggerReceivedForPlaceOrderAsync(False).ConfigureAwait(False)
                If placeOrderTrigger IsNot Nothing AndAlso placeOrderTrigger.Item1 = ExecuteCommandAction.Take Then
                    Await ExecuteCommandAsync(ExecuteCommands.PlaceCOMarketMISOrder, Nothing).ConfigureAwait(False)
                End If
                _cts.Token.ThrowIfCancellationRequested()
                Dim modifyStoplossOrderTrigger As List(Of Tuple(Of ExecuteCommandAction, IOrder, Decimal)) = Await IsTriggerReceivedForModifyStoplossOrderAsync(False).ConfigureAwait(False)
                If modifyStoplossOrderTrigger IsNot Nothing AndAlso modifyStoplossOrderTrigger.Count > 0 Then
                    Await ExecuteCommandAsync(ExecuteCommands.ModifyStoplossOrder, Nothing).ConfigureAwait(False)
                End If
                _cts.Token.ThrowIfCancellationRequested()
                Dim exitOrderTrigger As List(Of Tuple(Of ExecuteCommandAction, IOrder)) = Await IsTriggerReceivedForExitOrderAsync(False).ConfigureAwait(False)
                If exitOrderTrigger IsNot Nothing AndAlso exitOrderTrigger.Count > 0 Then
                    Await ExecuteCommandAsync(ExecuteCommands.CancelCOOrder, Nothing).ConfigureAwait(False)
                End If
                _cts.Token.ThrowIfCancellationRequested()
                Await Task.Delay(1000, _cts.Token).ConfigureAwait(False)
            End While
        Catch ex As Exception
            'To log exceptions getting created from this function as the bubble up of the exception
            'will anyways happen to Strategy.MonitorAsync but it will not be shown until all tasks exit
            logger.Error("Strategy Instrument:{0}, error:{1}", Me.ToString, ex.ToString)
            Throw ex
        End Try
    End Function

    Protected Overrides Async Function IsTriggerReceivedForPlaceOrderAsync(ByVal forcePrint As Boolean) As Task(Of Tuple(Of ExecuteCommandAction, PlaceOrderParameters))
        Dim ret As Tuple(Of ExecuteCommandAction, PlaceOrderParameters) = Nothing
        Await Task.Delay(0, _cts.Token).ConfigureAwait(False)
        Dim emaStUserSettings As EMA_SupertrendStrategyUserInputs = Me.ParentStrategy.UserSettings
        Dim runningCandlePayload As OHLCPayload = GetXMinuteCurrentCandle(emaStUserSettings.SignalTimeFrame)
        Dim capitalAtDayStart As Decimal = Me.ParentStrategy.ParentController.GetUserMargin(Me.TradableInstrument.ExchangeDetails.ExchangeType)
        Dim supertrendConsumer As SupertrendConsumer = GetConsumer(Me.RawPayloadDependentConsumers, _dummySupertrendConsumer)

        Try
            If runningCandlePayload IsNot Nothing AndAlso runningCandlePayload.PreviousPayload IsNot Nothing Then
                If forcePrint OrElse Not runningCandlePayload.PreviousPayload.ToString = lastPrevPayloadPlaceOrder Then
                    lastPrevPayloadPlaceOrder = runningCandlePayload.PreviousPayload.ToString
                    logger.Warn("Potential Signal Candle is:{0}. Will check rest parameters.", runningCandlePayload.PreviousPayload.ToString)
                    logger.Debug("Rest all parameters: Trade Start Time:{0}, LastTradeEntryTime:{1}, RunningCandlePayloadSnapshotDateTime:{2},
                    PayloadGeneratedBy:{3}, IsActiveInstrument:{4}, IsHistoricalCompleted:{5}, MTM Loss: {6}, MTM Profit: {7}, 
                    TotalStrategyPL:{8}, IsCrossover(above):{9}, IsCrossover(below):{10}, SupertrendColor:{11}, Quantity:{12},
                    Stoploss%:{13}, IT%:{14}, T%:{15}, LVT%:{16}, LVStartTime:{17}, LVEndYime:{18}, TradingSymbol:{19}",
                    emaStUserSettings.TradeStartTime,
                    emaStUserSettings.LastTradeEntryTime,
                    runningCandlePayload.SnapshotDateTime.ToString,
                    runningCandlePayload.PayloadGeneratedBy.ToString,
                    IsActiveInstrument(),
                    Me.TradableInstrument.IsHistoricalCompleted,
                    capitalAtDayStart * Math.Abs(emaStUserSettings.MaxLossPercentagePerDay) * -1 / 100,
                    capitalAtDayStart * Math.Abs(emaStUserSettings.MaxProfitPercentagePerDay) / 100,
                    Me.ParentStrategy.GetTotalPL,
                    IsCrossover(_dummyFastEMAConsumer, _dummySlowEMAConsumer, TypeOfField.EMA, TypeOfField.EMA, runningCandlePayload, CrossDirection.Above),
                    IsCrossover(_dummyFastEMAConsumer, _dummySlowEMAConsumer, TypeOfField.EMA, TypeOfField.EMA, runningCandlePayload, CrossDirection.Below),
                    CType(supertrendConsumer.ConsumerPayloads(runningCandlePayload.PreviousPayload.SnapshotDateTime), SupertrendConsumer.SupertrendPayload).SupertrendColor.ToString,
                    Me.TradableInstrument.LotSize * emaStUserSettings.InstrumentsData(Me.TradableInstrument.RawInstrumentName).Quantity,
                    emaStUserSettings.InstrumentsData(Me.TradableInstrument.RawInstrumentName).StoplossPercentage,
                    emaStUserSettings.InstrumentsData(Me.TradableInstrument.RawInstrumentName).IntemediateTargetPercentage,
                    emaStUserSettings.InstrumentsData(Me.TradableInstrument.RawInstrumentName).TargetPercentage,
                    emaStUserSettings.InstrumentsData(Me.TradableInstrument.RawInstrumentName).LowVolatilityTargetPercentage,
                    emaStUserSettings.InstrumentsData(Me.TradableInstrument.RawInstrumentName).LowVolatilityStartTime.ToString,
                    emaStUserSettings.InstrumentsData(Me.TradableInstrument.RawInstrumentName).LowVolatilityExitTime.ToString,
                    Me.TradableInstrument.TradingSymbol)
                End If
            End If
        Catch ex As Exception
            logger.Error(ex)
        End Try

        Dim parameters As PlaceOrderParameters = Nothing
        If Now >= emaStUserSettings.TradeStartTime AndAlso Now <= emaStUserSettings.LastTradeEntryTime AndAlso
            runningCandlePayload IsNot Nothing AndAlso runningCandlePayload.SnapshotDateTime >= emaStUserSettings.TradeStartTime AndAlso
            runningCandlePayload.PayloadGeneratedBy = OHLCPayload.PayloadSource.CalculatedTick AndAlso
            runningCandlePayload.PreviousPayload IsNot Nothing AndAlso Not IsActiveInstrument() AndAlso Me.TradableInstrument.IsHistoricalCompleted AndAlso
            Me.ParentStrategy.GetTotalPL() > capitalAtDayStart * Math.Abs(emaStUserSettings.MaxLossPercentagePerDay) * -1 / 100 AndAlso
            Me.ParentStrategy.GetTotalPL() < capitalAtDayStart * Math.Abs(emaStUserSettings.MaxProfitPercentagePerDay) / 100 Then

            Dim marketPrice As Decimal = Me.TradableInstrument.LastTick.LastPrice
            Dim triggerPrice As Decimal = Nothing
            Dim quantity As Integer = Me.TradableInstrument.LotSize * emaStUserSettings.InstrumentsData(Me.TradableInstrument.RawInstrumentName).Quantity

            If IsCrossover(_dummyFastEMAConsumer, _dummySlowEMAConsumer, TypeOfField.EMA, TypeOfField.EMA, runningCandlePayload, CrossDirection.Above) AndAlso
                supertrendConsumer.ConsumerPayloads.ContainsKey(runningCandlePayload.PreviousPayload.SnapshotDateTime) AndAlso
                (Not _useST Or CType(supertrendConsumer.ConsumerPayloads(runningCandlePayload.PreviousPayload.SnapshotDateTime), SupertrendConsumer.SupertrendPayload).SupertrendColor = Color.Green) Then

                triggerPrice = marketPrice - ConvertFloorCeling((marketPrice * emaStUserSettings.InstrumentsData(Me.TradableInstrument.RawInstrumentName).StoplossPercentage / 100), Me.TradableInstrument.TickSize, RoundOfType.Floor)

                parameters = New PlaceOrderParameters With
                                   {.EntryDirection = APIAdapter.TransactionType.Buy,
                                   .Quantity = quantity,
                                   .TriggerPrice = triggerPrice,
                                   .SignalCandle = runningCandlePayload.PreviousPayload}
            ElseIf IsCrossover(_dummyFastEMAConsumer, _dummySlowEMAConsumer, TypeOfField.EMA, TypeOfField.EMA, runningCandlePayload, CrossDirection.Below) AndAlso
                supertrendConsumer.ConsumerPayloads.ContainsKey(runningCandlePayload.PreviousPayload.SnapshotDateTime) AndAlso
                (Not _useST Or CType(supertrendConsumer.ConsumerPayloads(runningCandlePayload.PreviousPayload.SnapshotDateTime), SupertrendConsumer.SupertrendPayload).SupertrendColor = Color.Red) Then

                triggerPrice = marketPrice + ConvertFloorCeling((marketPrice * emaStUserSettings.InstrumentsData(Me.TradableInstrument.RawInstrumentName).StoplossPercentage / 100), Me.TradableInstrument.TickSize, RoundOfType.Floor)

                parameters = New PlaceOrderParameters With
                                   {.EntryDirection = APIAdapter.TransactionType.Sell,
                                   .Quantity = quantity,
                                   .TriggerPrice = triggerPrice,
                                   .SignalCandle = runningCandlePayload.PreviousPayload}
            End If
        End If

        'Below portion have to be done in every place order trigger
        If parameters IsNot Nothing Then
            Dim currentSignalActivities As IEnumerable(Of ActivityDashboard) = Me.ParentStrategy.SignalManager.GetSignalActivities(parameters.SignalCandle.SnapshotDateTime, Me.TradableInstrument.InstrumentIdentifier)
            If currentSignalActivities IsNot Nothing AndAlso currentSignalActivities.Count > 0 Then
                If currentSignalActivities.FirstOrDefault.EntryActivity.RequestStatus = ActivityDashboard.SignalStatusType.Discarded AndAlso
                    currentSignalActivities.FirstOrDefault.EntryActivity.LastException IsNot Nothing AndAlso
                    currentSignalActivities.FirstOrDefault.EntryActivity.LastException.Message.ToUpper.Contains("TIME") Then
                    ret = New Tuple(Of ExecuteCommandAction, PlaceOrderParameters)(ExecuteCommandAction.WaitAndTake, parameters)
                ElseIf currentSignalActivities.FirstOrDefault.EntryActivity.RequestStatus = ActivityDashboard.SignalStatusType.Discarded Then
                    ret = New Tuple(Of ExecuteCommandAction, PlaceOrderParameters)(ExecuteCommandAction.Take, parameters)
                    'ElseIf currentSignalActivities.FirstOrDefault.EntryActivity.RequestStatus = ActivityDashboard.SignalStatusType.Rejected Then
                    '    ret = New Tuple(Of ExecuteCommandAction, PlaceOrderParameters)(ExecuteCommandAction.Take, parameters)
                Else
                    ret = New Tuple(Of ExecuteCommandAction, PlaceOrderParameters)(ExecuteCommandAction.DonotTake, Nothing)
                End If
            Else
                ret = New Tuple(Of ExecuteCommandAction, PlaceOrderParameters)(ExecuteCommandAction.Take, parameters)
            End If
        Else
            ret = New Tuple(Of ExecuteCommandAction, PlaceOrderParameters)(ExecuteCommandAction.DonotTake, Nothing)
        End If
        Return ret
    End Function

    Protected Overrides Async Function IsTriggerReceivedForModifyStoplossOrderAsync(ByVal forcePrint As Boolean) As Task(Of List(Of Tuple(Of ExecuteCommandAction, IOrder, Decimal)))
        Dim ret As List(Of Tuple(Of ExecuteCommandAction, IOrder, Decimal)) = Nothing
        Await Task.Delay(0, _cts.Token).ConfigureAwait(False)
        Dim emaStUserSettings As EMA_SupertrendStrategyUserInputs = Me.ParentStrategy.UserSettings
        If OrderDetails IsNot Nothing AndAlso OrderDetails.Count > 0 Then
            For Each parentOrderId In OrderDetails.Keys
                Dim parentBusinessOrder As IBusinessOrder = OrderDetails(parentOrderId)
                If parentBusinessOrder.ParentOrder IsNot Nothing AndAlso
                    parentBusinessOrder.ParentOrder.Status = "COMPLETE" AndAlso
                    parentBusinessOrder.SLOrder IsNot Nothing AndAlso parentBusinessOrder.SLOrder.Count > 0 Then
                    Dim runningCandlePayload As OHLCPayload = Nothing
                    Dim checkIT As Boolean = False
                    If emaStUserSettings.InstrumentsData(Me.TradableInstrument.RawInstrumentName).IntemediateTargetPercentage <> Decimal.MinValue Then
                        runningCandlePayload = GetXMinuteCurrentCandle(emaStUserSettings.SignalTimeFrame)
                        If runningCandlePayload IsNot Nothing AndAlso runningCandlePayload.PreviousPayload IsNot Nothing Then
                            checkIT = True
                        End If
                    End If
                    Dim intermediateTargetReached As Boolean = False
                    Dim triggerPrice As Decimal = Decimal.MinValue
                    If parentBusinessOrder.ParentOrder.TransactionType = "BUY" Then
                        triggerPrice = parentBusinessOrder.ParentOrder.AveragePrice - ConvertFloorCeling((parentBusinessOrder.ParentOrder.AveragePrice * emaStUserSettings.InstrumentsData(Me.TradableInstrument.RawInstrumentName).StoplossPercentage / 100), Me.TradableInstrument.TickSize, RoundOfType.Floor)
                        If checkIT Then
                            Dim it As Decimal = parentBusinessOrder.ParentOrder.AveragePrice + (parentBusinessOrder.ParentOrder.AveragePrice * emaStUserSettings.InstrumentsData(Me.TradableInstrument.RawInstrumentName).IntemediateTargetPercentage / 100)
                            intermediateTargetReached = runningCandlePayload.PreviousPayload.HighPrice.Value >= ConvertFloorCeling(it, Me.TradableInstrument.TickSize, RoundOfType.Floor)
                        End If
                    ElseIf parentBusinessOrder.ParentOrder.TransactionType = "SELL" Then
                        triggerPrice = parentBusinessOrder.ParentOrder.AveragePrice + ConvertFloorCeling((parentBusinessOrder.ParentOrder.AveragePrice * emaStUserSettings.InstrumentsData(Me.TradableInstrument.RawInstrumentName).StoplossPercentage / 100), Me.TradableInstrument.TickSize, RoundOfType.Floor)
                        If checkIT Then
                            Dim it As Decimal = parentBusinessOrder.ParentOrder.AveragePrice - (parentBusinessOrder.ParentOrder.AveragePrice * emaStUserSettings.InstrumentsData(Me.TradableInstrument.RawInstrumentName).IntemediateTargetPercentage / 100)
                            intermediateTargetReached = runningCandlePayload.PreviousPayload.LowPrice.Value <= ConvertFloorCeling(it, Me.TradableInstrument.TickSize, RoundOfType.Floor)
                        End If
                    End If
                    For Each slOrder In parentBusinessOrder.SLOrder
                        If Not slOrder.Status = "COMPLETE" AndAlso Not slOrder.Status = "CANCELLED" AndAlso Not slOrder.Status = "REJECTED" Then
                            If intermediateTargetReached Then
                                logger.Debug("IT reached")
                                triggerPrice = runningCandlePayload.OpenPrice.Value
                            End If
                            If slOrder.TriggerPrice <> triggerPrice Then
                                'Below portion have to be done in every modify stoploss order trigger
                                Dim currentSignalActivities As ActivityDashboard = Me.ParentStrategy.SignalManager.GetSignalActivities(slOrder.Tag)
                                If currentSignalActivities IsNot Nothing AndAlso currentSignalActivities.StoplossModifyActivity.Supporting = triggerPrice Then
                                    If currentSignalActivities.StoplossModifyActivity.RequestStatus = ActivityDashboard.SignalStatusType.Handled OrElse
                                        currentSignalActivities.StoplossModifyActivity.RequestStatus = ActivityDashboard.SignalStatusType.Activated OrElse
                                        currentSignalActivities.StoplossModifyActivity.RequestStatus = ActivityDashboard.SignalStatusType.Completed Then
                                        Continue For
                                    End If
                                End If
                                If ret Is Nothing Then ret = New List(Of Tuple(Of ExecuteCommandAction, IOrder, Decimal))
                                ret.Add(New Tuple(Of ExecuteCommandAction, IOrder, Decimal)(ExecuteCommandAction.Take, slOrder, triggerPrice))
                            End If
                        End If
                    Next
                End If
            Next
        End If
        Return ret
    End Function

    Protected Overrides Async Function IsTriggerReceivedForExitOrderAsync(ByVal forcePrint As Boolean) As Task(Of List(Of Tuple(Of ExecuteCommandAction, IOrder)))
        Dim ret As List(Of Tuple(Of ExecuteCommandAction, IOrder)) = Nothing
        Dim emaStUserSettings As EMA_SupertrendStrategyUserInputs = Me.ParentStrategy.UserSettings
        Dim allActiveOrders As List(Of IOrder) = GetAllActiveOrders(APIAdapter.TransactionType.None)
        If allActiveOrders IsNot Nothing AndAlso allActiveOrders.Count > 0 Then
            Dim parentOrders As List(Of IOrder) = allActiveOrders.FindAll(Function(x)
                                                                              Return x.ParentOrderIdentifier Is Nothing
                                                                          End Function)
            If parentOrders IsNot Nothing AndAlso parentOrders.Count > 0 Then
                For Each parentOrder In parentOrders
                    Dim parentBusinessOrder As IBusinessOrder = OrderDetails(parentOrder.OrderIdentifier)
                    If parentBusinessOrder.SLOrder IsNot Nothing AndAlso parentBusinessOrder.SLOrder.Count > 0 Then
                        For Each slOrder In parentBusinessOrder.SLOrder
                            Dim tradeWillExit As Boolean = False
                            If Not slOrder.Status = "COMPLETE" AndAlso Not slOrder.Status = "CANCELLED" AndAlso Not slOrder.Status = "REJECTED" Then
                                Dim runningCandle As OHLCPayload = GetXMinuteCurrentCandle(Me.ParentStrategy.UserSettings.SignalTimeFrame)
                                If runningCandle IsNot Nothing AndAlso runningCandle.PreviousPayload IsNot Nothing Then
                                    If parentBusinessOrder.ParentOrder.TransactionType = "BUY" Then
                                        'Exit for T % reach or LT% reach
                                        Dim target As Decimal = parentBusinessOrder.ParentOrder.AveragePrice + ConvertFloorCeling((parentBusinessOrder.ParentOrder.AveragePrice * emaStUserSettings.InstrumentsData(Me.TradableInstrument.RawInstrumentName).TargetPercentage / 100), Me.TradableInstrument.TickSize, RoundOfType.Floor)
                                        If emaStUserSettings.InstrumentsData(Me.TradableInstrument.RawInstrumentName).LowVolatilityStartTime <> Now.Date AndAlso
                                            emaStUserSettings.InstrumentsData(Me.TradableInstrument.RawInstrumentName).LowVolatilityExitTime <> Now.Date AndAlso
                                            emaStUserSettings.InstrumentsData(Me.TradableInstrument.RawInstrumentName).LowVolatilityTargetPercentage <> Decimal.MinValue Then
                                            If Now >= emaStUserSettings.InstrumentsData(Me.TradableInstrument.RawInstrumentName).LowVolatilityStartTime AndAlso
                                                Now <= emaStUserSettings.InstrumentsData(Me.TradableInstrument.RawInstrumentName).LowVolatilityExitTime Then
                                                target = parentBusinessOrder.ParentOrder.AveragePrice + ConvertFloorCeling((parentBusinessOrder.ParentOrder.AveragePrice * emaStUserSettings.InstrumentsData(Me.TradableInstrument.RawInstrumentName).LowVolatilityTargetPercentage / 100), Me.TradableInstrument.TickSize, RoundOfType.Floor)
                                            End If
                                        End If
                                        If Me.TradableInstrument.LastTick.LastPrice >= target Then
                                            logger.Warn("T% Or LT% reached. OrderId:{0}, LastPrice:{1}, TargetPrice:{2}", slOrder.ParentOrderIdentifier, Me.TradableInstrument.LastTick.LastPrice, target)
                                            Await ForceExitSpecificTradeAsync(slOrder).ConfigureAwait(False)
                                        End If
                                        'Exit for Opposite direction EMA crossover
                                        If IsCrossover(_dummyFastEMAConsumer, _dummySlowEMAConsumer, TypeOfField.EMA, TypeOfField.EMA, runningCandle, CrossDirection.Below) Then
                                            logger.Warn("Ema crossover below")
                                            tradeWillExit = True
                                        End If
                                        'Exit for Candle close beyond Supertrend
                                        Dim supertrendConsumer As SupertrendConsumer = GetConsumer(Me.RawPayloadDependentConsumers, _dummySupertrendConsumer)
                                        If _useST AndAlso supertrendConsumer IsNot Nothing AndAlso supertrendConsumer.ConsumerPayloads.Count > 0 AndAlso
                                            supertrendConsumer.ConsumerPayloads.ContainsKey(runningCandle.PreviousPayload.SnapshotDateTime) AndAlso
                                            runningCandle.PreviousPayload.ClosePrice.Value < CType(supertrendConsumer.ConsumerPayloads(runningCandle.PreviousPayload.SnapshotDateTime), SupertrendConsumer.SupertrendPayload).Supertrend.Value Then
                                            logger.Warn("Beyond ST")
                                            tradeWillExit = True
                                        End If
                                    ElseIf parentBusinessOrder.ParentOrder.TransactionType = "SELL" Then
                                        'Exit for T % reach or LT% reach
                                        Dim target As Decimal = parentBusinessOrder.ParentOrder.AveragePrice - ConvertFloorCeling((parentBusinessOrder.ParentOrder.AveragePrice * emaStUserSettings.InstrumentsData(Me.TradableInstrument.RawInstrumentName).TargetPercentage / 100), Me.TradableInstrument.TickSize, RoundOfType.Floor)
                                        If emaStUserSettings.InstrumentsData(Me.TradableInstrument.RawInstrumentName).LowVolatilityStartTime <> Now.Date AndAlso
                                            emaStUserSettings.InstrumentsData(Me.TradableInstrument.RawInstrumentName).LowVolatilityExitTime <> Now.Date AndAlso
                                            emaStUserSettings.InstrumentsData(Me.TradableInstrument.RawInstrumentName).LowVolatilityTargetPercentage <> Decimal.MinValue Then
                                            If Now >= emaStUserSettings.InstrumentsData(Me.TradableInstrument.RawInstrumentName).LowVolatilityStartTime AndAlso
                                                Now <= emaStUserSettings.InstrumentsData(Me.TradableInstrument.RawInstrumentName).LowVolatilityExitTime Then
                                                target = parentBusinessOrder.ParentOrder.AveragePrice - ConvertFloorCeling((parentBusinessOrder.ParentOrder.AveragePrice * emaStUserSettings.InstrumentsData(Me.TradableInstrument.RawInstrumentName).LowVolatilityTargetPercentage / 100), Me.TradableInstrument.TickSize, RoundOfType.Floor)
                                            End If
                                        End If
                                        If Me.TradableInstrument.LastTick.LastPrice <= target Then
                                            logger.Warn("T% Or LT% reached. OrderId:{0}, LastPrice:{1}, TargetPrice:{2}", slOrder.ParentOrderIdentifier, Me.TradableInstrument.LastTick.LastPrice, target)
                                            Await ForceExitSpecificTradeAsync(slOrder).ConfigureAwait(False)
                                        End If
                                        'Exit for Opposite direction EMA crossover
                                        If IsCrossover(_dummyFastEMAConsumer, _dummySlowEMAConsumer, TypeOfField.EMA, TypeOfField.EMA, runningCandle, CrossDirection.Above) Then
                                            logger.Warn("Ema crossover above")
                                            tradeWillExit = True
                                        End If
                                        'Exit for Candle close beyond Supertrend
                                        Dim supertrendConsumer As SupertrendConsumer = GetConsumer(Me.RawPayloadDependentConsumers, _dummySupertrendConsumer)
                                        If _useST AndAlso supertrendConsumer IsNot Nothing AndAlso supertrendConsumer.ConsumerPayloads.Count > 0 AndAlso
                                            supertrendConsumer.ConsumerPayloads.ContainsKey(runningCandle.PreviousPayload.SnapshotDateTime) AndAlso
                                            runningCandle.PreviousPayload.ClosePrice.Value > CType(supertrendConsumer.ConsumerPayloads(runningCandle.PreviousPayload.SnapshotDateTime), SupertrendConsumer.SupertrendPayload).Supertrend.Value Then
                                            logger.Warn("Beyond ST")
                                            tradeWillExit = True
                                        End If
                                    End If
                                End If
                            End If
                            If tradeWillExit Then
                                'Below portion have to be done in every cancel order trigger
                                Dim currentSignalActivities As ActivityDashboard = Me.ParentStrategy.SignalManager.GetSignalActivities(parentOrder.Tag)
                                If currentSignalActivities IsNot Nothing Then
                                    If currentSignalActivities.CancelActivity.RequestStatus = ActivityDashboard.SignalStatusType.Handled OrElse
                                        currentSignalActivities.CancelActivity.RequestStatus = ActivityDashboard.SignalStatusType.Activated OrElse
                                        currentSignalActivities.CancelActivity.RequestStatus = ActivityDashboard.SignalStatusType.Completed Then
                                        Continue For
                                    End If
                                End If
                                If ret Is Nothing Then ret = New List(Of Tuple(Of ExecuteCommandAction, IOrder))
                                ret.Add(New Tuple(Of ExecuteCommandAction, IOrder)(ExecuteCommandAction.Take, slOrder))
                            End If
                        Next
                    End If
                Next
            End If
        End If
        Return ret
    End Function

    Protected Overrides Async Function ForceExitSpecificTradeAsync(order As IOrder) As Task
        If order IsNot Nothing AndAlso Not order.Status = "COMPLETE" AndAlso Not order.Status = "CANCELLED" AndAlso Not order.Status = "REJECTED" Then
            Dim cancellableOrder As New List(Of Tuple(Of ExecuteCommandAction, IOrder)) From
        {
            New Tuple(Of ExecuteCommandAction, IOrder)(ExecuteCommandAction.Take, order)
        }
            Await ExecuteCommandAsync(ExecuteCommands.ForceCancelCOOrder, cancellableOrder).ConfigureAwait(False)
        End If
    End Function

#Region "IDisposable Support"
    Private disposedValue As Boolean ' To detect redundant calls

    ' IDisposable
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                ' TODO: dispose managed state (managed objects).
            End If

            ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
            ' TODO: set large fields to null.
        End If
        disposedValue = True
    End Sub

    ' TODO: override Finalize() only if Dispose(disposing As Boolean) above has code to free unmanaged resources.
    'Protected Overrides Sub Finalize()
    '    ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
    '    Dispose(False)
    '    MyBase.Finalize()
    'End Sub

    ' This code added by Visual Basic to correctly implement the disposable pattern.
    Public Sub Dispose() Implements IDisposable.Dispose
        ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
        Dispose(True)
        ' TODO: uncomment the following line if Finalize() is overridden above.
        ' GC.SuppressFinalize(Me)
    End Sub
#End Region
End Class
