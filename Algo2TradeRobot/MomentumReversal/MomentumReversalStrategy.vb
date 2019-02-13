Imports System.Text.RegularExpressions
Imports System.Threading
Imports Algo2TradeCore.Adapter
Imports Algo2TradeCore.Controller
Imports Algo2TradeCore.Entities
Imports Algo2TradeCore.Strategies
Imports Algo2TradeCore.UserSettings
Imports NLog

Public Class MomentumReversalStrategy
    Inherits Strategy
#Region "Logging and Status Progress"
    Public Shared Shadows logger As Logger = LogManager.GetCurrentClassLogger
#End Region

    Public Sub New(ByVal associatedParentController As APIStrategyController,
                   ByVal canceller As CancellationTokenSource,
                   ByVal strategyIdentifier As String,
                   ByVal userSettings As MomentumReversalUserInputs)
        MyBase.New(associatedParentController, canceller, strategyIdentifier, userSettings)
        'Though the TradableStrategyInstruments is being populated from inside by newing it,
        'lets also initiatilize here so that after creation of the strategy and before populating strategy instruments,
        'the fron end grid can bind to this created TradableStrategyInstruments which will be empty
        'TradableStrategyInstruments = New List(Of StrategyInstrument)
    End Sub
    Public Sub New(ByVal associatedParentController As APIStrategyController,
                   ByVal canceller As CancellationTokenSource,
                   ByVal strategyIdentifier As String)
        Me.New(associatedParentController, canceller, strategyIdentifier, Nothing)
        'Though the TradableStrategyInstruments is being populated from inside by newing it,
        'lets also initiatilize here so that after creation of the strategy and before populating strategy instruments,
        'the fron end grid can bind to this created TradableStrategyInstruments which will be empty
        'TradableStrategyInstruments = New List(Of StrategyInstrument)
    End Sub
    ''' <summary>
    ''' This function will fill the instruments based on the stratgey used and also create the workers
    ''' </summary>
    ''' <param name="allInstruments"></param>
    ''' <returns></returns>
    Public Overrides Async Function CreateTradableStrategyInstrumentsAsync(ByVal allInstruments As IEnumerable(Of IInstrument)) As Task(Of Boolean)
        If allInstruments IsNot Nothing AndAlso allInstruments.Count > 0 Then
            logger.Debug("CreateTradableStrategyInstrumentsAsync, allInstruments.Count:{0}", allInstruments.Count)
        Else
            logger.Debug("CreateTradableStrategyInstrumentsAsync, allInstruments.Count:Nothing or 0")
        End If
        _cts.Token.ThrowIfCancellationRequested()
        Dim ret As Boolean = False
        Dim retTradableInstrumentsAsPerStrategy As List(Of IInstrument) = Nothing
        Await Task.Delay(0).ConfigureAwait(False)
        logger.Debug("Starting to fill strategy specific instruments, strategy:{0}", Me.ToString)
        If allInstruments IsNot Nothing AndAlso allInstruments.Count > 0 Then
            'Get all the futures instruments
            'Dim futureAllInstruments = allInstruments.Where(Function(x)
            '                                                    Return x.InstrumentType = "FUT" AndAlso x.Exchange = "NFO"
            '                                                End Function)
            '_cts.Token.ThrowIfCancellationRequested()
            'If futureAllInstruments IsNot Nothing AndAlso futureAllInstruments.Count > 0 Then
            '    For Each runningFutureAllInstrument In futureAllInstruments
            '        _cts.Token.ThrowIfCancellationRequested()
            '        Dim coreInstrumentName As String = Regex.Replace(runningFutureAllInstrument.TradingSymbol, "[0-9]+[A-Z]+FUT", "")
            '        If coreInstrumentName IsNot Nothing Then
            '            Dim cashInstrumentToAdd = allInstruments.Where(Function(x)
            '                                                               Return x.TradingSymbol = coreInstrumentName
            '                                                           End Function).FirstOrDefault
            '            _cts.Token.ThrowIfCancellationRequested()
            '            If cashInstrumentToAdd IsNot Nothing AndAlso cashInstrumentToAdd.TradingSymbol IsNot Nothing AndAlso
            '                (retTradableInstrumentsAsPerStrategy Is Nothing OrElse (retTradableInstrumentsAsPerStrategy IsNot Nothing AndAlso
            '                retTradableInstrumentsAsPerStrategy.Find(Function(x)
            '                                                             Return x.InstrumentIdentifier = cashInstrumentToAdd.InstrumentIdentifier
            '                                                         End Function) Is Nothing)) Then
            '                ret = True
            '                If retTradableInstrumentsAsPerStrategy Is Nothing Then retTradableInstrumentsAsPerStrategy = New List(Of IInstrument)
            '                retTradableInstrumentsAsPerStrategy.Add(cashInstrumentToAdd)
            '            End If
            '        End If
            '    Next
            '    TradableInstrumentsAsPerStrategy = retTradableInstrumentsAsPerStrategy
            'End If

            Dim futureAllInstruments = allInstruments.Where(Function(x)
                                                                Return x.InstrumentType = "FUT" AndAlso x.Exchange = "MCX" 'AndAlso x.InstrumentIdentifier = "54177543"
                                                            End Function)
            _cts.Token.ThrowIfCancellationRequested()
            If futureAllInstruments IsNot Nothing AndAlso futureAllInstruments.Count > 0 Then
                For Each runningFutureAllInstrument In futureAllInstruments.Take(50)
                    _cts.Token.ThrowIfCancellationRequested()
                    ret = True
                    If retTradableInstrumentsAsPerStrategy Is Nothing Then retTradableInstrumentsAsPerStrategy = New List(Of IInstrument)
                    retTradableInstrumentsAsPerStrategy.Add(runningFutureAllInstrument)
                Next
                TradableInstrumentsAsPerStrategy = retTradableInstrumentsAsPerStrategy
            End If

            'Get MR Strategy Instruments
            'Dim mrUserInputs As MomentumReversalUserInputs = CType(UserSettings, MomentumReversalUserInputs)
            'If mrUserInputs.InstrumentsData IsNot Nothing AndAlso mrUserInputs.InstrumentsData.Count > 0 Then
            '    Dim dummyAllInstruments As List(Of IInstrument) = allInstruments.ToList
            '    Dim cashInstrumentList As IEnumerable(Of KeyValuePair(Of String, MomentumReversalUserInputs.InstrumentDetails)) =
            '        mrUserInputs.InstrumentsData.Where(Function(x)
            '                                               Return x.Value.MarketType = MomentumReversalUserInputs.InstrumentType.Cash OrElse
            '                                                                     x.Value.MarketType = MomentumReversalUserInputs.InstrumentType.Both
            '                                           End Function)
            '    Dim futureInstrumentList As IEnumerable(Of KeyValuePair(Of String, MomentumReversalUserInputs.InstrumentDetails)) =
            '        mrUserInputs.InstrumentsData.Where(Function(x)
            '                                               Return x.Value.MarketType = MomentumReversalUserInputs.InstrumentType.Futures OrElse
            '                                                                     x.Value.MarketType = MomentumReversalUserInputs.InstrumentType.Both
            '                                           End Function)
            '    For Each instrument In cashInstrumentList.ToList
            '        _cts.Token.ThrowIfCancellationRequested()
            '        Dim runningTradableInstrument As IInstrument = dummyAllInstruments.Find(Function(x)
            '                                                                                    Return x.TradingSymbol = instrument.Key
            '                                                                                End Function)
            '        _cts.Token.ThrowIfCancellationRequested()
            '        ret = True
            '        If retTradableInstrumentsAsPerStrategy Is Nothing Then retTradableInstrumentsAsPerStrategy = New List(Of IInstrument)
            '        If runningTradableInstrument IsNot Nothing Then retTradableInstrumentsAsPerStrategy.Add(runningTradableInstrument)
            '    Next
            '    For Each instrument In futureInstrumentList.ToList
            '        _cts.Token.ThrowIfCancellationRequested()
            '        Dim runningTradableInstrument As IInstrument = Nothing
            '        'Dim allTradableInstruments As List(Of IInstrument) = dummyAllInstruments.FindAll(Function(x)
            '        '                                                                                     Return Regex.Replace(x.TradingSymbol, "[0-9]+[A-Z]+FUT", "") = instrument.Key AndAlso
            '        '                                                                                         x.InstrumentType = "FUT" AndAlso x.Exchange = "NFO"
            '        '                                                                                 End Function)

            '        Dim allTradableInstruments As List(Of IInstrument) = dummyAllInstruments.FindAll(Function(x)
            '                                                                                             Return Regex.Replace(x.TradingSymbol, "[0-9]+[A-Z]+FUT", "") = instrument.Key AndAlso
            '                                                                                                 x.InstrumentType = "FUT" AndAlso x.Exchange = "MCX"
            '                                                                                         End Function)

            '        Dim minExpiry As Date = allTradableInstruments.Min(Function(x)
            '                                                               If Not x.Expiry.Value.Date = Now.Date Then
            '                                                                   Return x.Expiry.Value
            '                                                               Else
            '                                                                   Return Date.MaxValue
            '                                                               End If
            '                                                           End Function)
            '        runningTradableInstrument = allTradableInstruments.Find(Function(x)
            '                                                                    Return x.Expiry = minExpiry
            '                                                                End Function)
            '        _cts.Token.ThrowIfCancellationRequested()
            '        ret = True
            '        If retTradableInstrumentsAsPerStrategy Is Nothing Then retTradableInstrumentsAsPerStrategy = New List(Of IInstrument)
            '        If runningTradableInstrument IsNot Nothing Then retTradableInstrumentsAsPerStrategy.Add(runningTradableInstrument)
            '    Next
            '    TradableInstrumentsAsPerStrategy = retTradableInstrumentsAsPerStrategy
            'End If
        End If

        If retTradableInstrumentsAsPerStrategy IsNot Nothing AndAlso retTradableInstrumentsAsPerStrategy.Count > 0 Then
            'tradableInstrumentsAsPerStrategy = tradableInstrumentsAsPerStrategy.Take(5).ToList
            'Now create the strategy tradable instruments
            Dim retTradableStrategyInstruments As List(Of MomentumReversalStrategyInstrument) = Nothing
            logger.Debug("Creating strategy tradable instruments, _tradableInstruments.count:{0}", retTradableInstrumentsAsPerStrategy.Count)
            'Remove the old handlers from the previous strategyinstruments collection
            If TradableStrategyInstruments IsNot Nothing AndAlso TradableStrategyInstruments.Count > 0 Then
                For Each runningTradableStrategyInstruments In TradableStrategyInstruments
                    RemoveHandler runningTradableStrategyInstruments.HeartbeatEx, AddressOf OnHeartbeatEx
                    RemoveHandler runningTradableStrategyInstruments.WaitingForEx, AddressOf OnWaitingForEx
                    RemoveHandler runningTradableStrategyInstruments.DocumentRetryStatusEx, AddressOf OnDocumentRetryStatusEx
                    RemoveHandler runningTradableStrategyInstruments.DocumentDownloadCompleteEx, AddressOf OnDocumentDownloadCompleteEx
                Next
                TradableStrategyInstruments = Nothing
            End If

            'Now create the fresh handlers
            For Each runningTradableInstrument In retTradableInstrumentsAsPerStrategy
                _cts.Token.ThrowIfCancellationRequested()
                If retTradableStrategyInstruments Is Nothing Then retTradableStrategyInstruments = New List(Of MomentumReversalStrategyInstrument)
                Dim runningTradableStrategyInstrument As New MomentumReversalStrategyInstrument(runningTradableInstrument, Me, _cts)
                AddHandler runningTradableStrategyInstrument.HeartbeatEx, AddressOf OnHeartbeatEx
                AddHandler runningTradableStrategyInstrument.WaitingForEx, AddressOf OnWaitingForEx
                AddHandler runningTradableStrategyInstrument.DocumentRetryStatusEx, AddressOf OnDocumentRetryStatusEx
                AddHandler runningTradableStrategyInstrument.DocumentDownloadCompleteEx, AddressOf OnDocumentDownloadCompleteEx

                retTradableStrategyInstruments.Add(runningTradableStrategyInstrument)
                'If runningTradableInstrument.FirstLevelConsumers Is Nothing Then runningTradableInstrument.FirstLevelConsumers = New List(Of StrategyInstrument)
                'runningTradableInstrument.FirstLevelConsumers.Add(runningTradableStrategyInstrument)
            Next
            TradableStrategyInstruments = retTradableStrategyInstruments
        Else
            Throw New ApplicationException(String.Format("Cannot run this strategy as no strategy instruments could be created from the tradable instruments, stratgey:{0}", Me.ToString))
        End If

        Return ret
    End Function
    ''' <summary>
    ''' This will create the required number of instrument workers based on the already filled tradable instruments.
    ''' It will also trigger the RunDirect method if the common condition for trigger for all instruments as per this strategy is satisfied
    ''' </summary>
    ''' <returns></returns>
    'Public Overrides Async Function ExecuteAsync() As Task
    '    logger.Debug("ExecuteAsync, parameters:Nothing")
    '    _cts.Token.ThrowIfCancellationRequested()

    '    'To fire any time based common calls to the strategy instruments
    '    While True
    '        _cts.Token.ThrowIfCancellationRequested()
    '        Dim triggerRecevied As Tuple(Of Boolean, Trigger) = Await IsTriggerReachedAsync().ConfigureAwait(False)
    '        If triggerRecevied IsNot Nothing AndAlso triggerRecevied.Item1 = True Then
    '            If TradableStrategyInstruments IsNot Nothing AndAlso TradableStrategyInstruments.Count > 0 Then
    '                For Each runningTradableStrategyInstrument In TradableStrategyInstruments
    '                    _cts.Token.ThrowIfCancellationRequested()
    '                    runningTradableStrategyInstrument.RunDirectAsync()
    '                Next
    '            End If
    '        End If
    '        Await Task.Delay(1001).ConfigureAwait(False)
    '    End While
    'End Function
    Public Overrides Async Function SubscribeAsync(ByVal usableTicker As APITicker, ByVal usableFetcher As APIHistoricalDataFetcher) As Task
        logger.Debug("SubscribeAsync, usableTicker:{0}", usableTicker.ToString)
        _cts.Token.ThrowIfCancellationRequested()
        If TradableStrategyInstruments IsNot Nothing AndAlso TradableStrategyInstruments.Count > 0 Then
            Dim runningInstrumentIdentifiers As List(Of String) = Nothing
            For Each runningTradableStrategyInstruments In TradableStrategyInstruments
                _cts.Token.ThrowIfCancellationRequested()
                If runningInstrumentIdentifiers Is Nothing Then runningInstrumentIdentifiers = New List(Of String)
                runningInstrumentIdentifiers.Add(runningTradableStrategyInstruments.TradableInstrument.InstrumentIdentifier)
            Next
            _cts.Token.ThrowIfCancellationRequested()
            Await usableTicker.SubscribeAsync(runningInstrumentIdentifiers).ConfigureAwait(False)
            Await usableFetcher.SubscribeAsync(runningInstrumentIdentifiers).ConfigureAwait(False)
            _cts.Token.ThrowIfCancellationRequested()
        End If
    End Function
    Public Overrides Async Function IsTriggerReachedAsync() As Task(Of Tuple(Of Boolean, Trigger))
        logger.Debug("IsTriggerReachedAsync, parameters:Nothing")
        _cts.Token.ThrowIfCancellationRequested()
        Await Task.Delay(0).ConfigureAwait(False)
        Dim ret As Tuple(Of Boolean, Trigger) = Nothing
        Dim currentTime As Date = Now
        Dim compareTime As TimeSpan = Nothing
        TimeSpan.TryParse("15:32:30", compareTime)
        If Utilities.Time.IsTimeEqualTillSeconds(currentTime, compareTime) Then
            ret = New Tuple(Of Boolean, Trigger)(True,
                                                 New Trigger() With
                                                 {.Category = Trigger.TriggerType.Timebased,
                                                 .Description = String.Format("Time reached:{0}", currentTime.ToString("HH:mm:ss"))})
        End If
        'TO DO: remove the below hard coding
        'ret = New Tuple(Of Boolean, Trigger)(True, Nothing)
        Return ret
    End Function
    Public Overrides Async Function MonitorAsync() As Task
        'Dim ctr As Integer
        'While True
        '    _cts.Token.ThrowIfCancellationRequested()
        '    Await Task.Delay(500)
        '    ctr += 500

        '    If ctr = 10000 Then
        '        _cts.Cancel()
        '    End If
        '    Await Task.Delay(1000)
        'End While
        Dim lastException As Exception = Nothing

        Try
            _cts.Token.ThrowIfCancellationRequested()
            Dim tasks As New List(Of Task)()
            For Each tradableStrategyInstrument As MomentumReversalStrategyInstrument In TradableStrategyInstruments
                _cts.Token.ThrowIfCancellationRequested()
                tasks.Add(Task.Run(AddressOf tradableStrategyInstrument.MonitorAsync, _cts.Token))
            Next
            'Task to run order update periodically
            tasks.Add(Task.Run(AddressOf FillOrderDetailsAsync, _cts.Token))
            'tasks.Add(Task.Run(AddressOf ExitAllTrades))
            Await Task.WhenAll(tasks).ConfigureAwait(False)
        Catch ex As Exception
            lastException = ex
            logger.Error(ex)
        End Try
        If lastException IsNot Nothing Then
            Await ParentController.CloseTickerIfConnectedAsync()
            Await ParentController.CloseFetcherIfConnectedAsync()
            Throw lastException
        End If
    End Function
    Public Overrides Function ToString() As String
        Return Me.GetType().Name
    End Function
    Protected Overrides Function IsTriggerReceivedForExitAllOrders() As Boolean
        Dim currentTime As Date = Now
        If currentTime.Hour = 14 AndAlso currentTime.Minute = 30 AndAlso currentTime.Second >= 0 Then
            Return True
        Else
            Return False
        End If
    End Function
End Class
