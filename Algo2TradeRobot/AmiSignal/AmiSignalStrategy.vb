﻿Imports System.Threading
Imports Algo2TradeCore.Adapter
Imports Algo2TradeCore.Controller
Imports Algo2TradeCore.Entities
Imports Algo2TradeCore.Strategies
Imports NLog
Imports Utilities.Time
Imports Utilities.DAL

Public Class AmiSignalStrategy
    Inherits Strategy
#Region "Logging and Status Progress"
    Public Shared Shadows logger As Logger = LogManager.GetCurrentClassLogger
#End Region

    Public Property EntrySignals As Concurrent.ConcurrentDictionary(Of String, AmiSignal)
    Public Property ExitSignals As Concurrent.ConcurrentDictionary(Of String, AmiSignal)
    Public Sub New(ByVal associatedParentController As APIStrategyController,
                   ByVal strategyIdentifier As String,
                   ByVal userSettings As AmiSignalUserInputs,
                   ByVal canceller As CancellationTokenSource)
        MyBase.New(associatedParentController, strategyIdentifier, userSettings, canceller)
        'Though the TradableStrategyInstruments is being populated from inside by newing it,
        'lets also initiatilize here so that after creation of the strategy and before populating strategy instruments,
        'the fron end grid can bind to this created TradableStrategyInstruments which will be empty
        'TradableStrategyInstruments = New List(Of StrategyInstrument)
    End Sub
    Public Sub New(ByVal associatedParentController As APIStrategyController,
                   ByVal strategyIdentifier As String,
                   ByVal canceller As CancellationTokenSource)
        MyBase.New(associatedParentController, strategyIdentifier, Nothing, canceller)
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
            Dim futureAllInstruments = allInstruments.Where(Function(x)
                                                                Return x.InstrumentType = "FUT" AndAlso x.Exchange = "MCX" 'AndAlso x.InstrumentIdentifier = "54177543"
                                                            End Function)
            _cts.Token.ThrowIfCancellationRequested()
            If futureAllInstruments IsNot Nothing AndAlso futureAllInstruments.Count > 0 Then
                For Each runningFutureAllInstrument In futureAllInstruments.Take(1)
                    _cts.Token.ThrowIfCancellationRequested()
                    ret = True
                    If retTradableInstrumentsAsPerStrategy Is Nothing Then retTradableInstrumentsAsPerStrategy = New List(Of IInstrument)
                    retTradableInstrumentsAsPerStrategy.Add(runningFutureAllInstrument)
                Next
                TradableInstrumentsAsPerStrategy = retTradableInstrumentsAsPerStrategy
            End If

            ''Get AmiSignal Strategy Instruments
            'Dim filePath As String = "G:\algo2trade\GitHub\Algo2Trade Live\AmiSignal Tradable Instruments.csv"
            ''Dim filePath As String = "D:\algo2trade\Code\Algo2Trade Live\AmiSignal Tradable Instruments - Copy.csv"
            'Dim dt As DataTable = Nothing
            'Using readCSV As New CSVHelper(filePath, ",", _cts)
            '    dt = readCSV.GetDataTableFromCSV(0)
            'End Using
            'If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
            '    Dim dummyAllInstruments As List(Of IInstrument) = allInstruments.ToList
            '    For i As Integer = 0 To dt.Rows.Count - 1
            '        _cts.Token.ThrowIfCancellationRequested()
            '        Dim rowNumber As Integer = i
            '        Dim runningTradableInstrument As IInstrument = dummyAllInstruments.Find(Function(x)
            '                                                                                    Return x.TradingSymbol = dt.Rows(rowNumber).Item(0)
            '                                                                                End Function)
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
            Dim retTradableStrategyInstruments As List(Of AmiSignalStrategyInstrument) = Nothing
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
                If retTradableStrategyInstruments Is Nothing Then retTradableStrategyInstruments = New List(Of AmiSignalStrategyInstrument)
                Dim runningTradableStrategyInstrument As New AmiSignalStrategyInstrument(runningTradableInstrument, Me, _cts)
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
        Dim currentTime As Date = ISTNow()
        Dim compareTime As TimeSpan = Nothing
        TimeSpan.TryParse("15:32:30", compareTime)
        If IsTimeEqualTillSeconds(currentTime, compareTime) Then
            ret = New Tuple(Of Boolean, Trigger)(True,
                                                 New Trigger() With
                                                 {.Category = Trigger.TriggerType.Timebased,
                                                 .Description = String.Format("Time reached:{0}", currentTime.ToString("HH:mm:ss"))})
        End If
        'TO DO: remove the below hard coding
        ret = New Tuple(Of Boolean, Trigger)(True, Nothing)
        Return ret
    End Function
    Public Overrides Async Function MonitorAsync() As Task
        'Dim ctr As Integer = 0
        'While True
        '    ctr += 1000
        '    'If ctr = 10000 Then Throw New ApplicationException("DOnno")
        '    _cts.Token.ThrowIfCancellationRequested()
        '    Await Task.Delay(1000)
        'End While
        Dim lastException As Exception = Nothing

        Try
            _cts.Token.ThrowIfCancellationRequested()
            Dim tasks As New List(Of Task)()
            For Each tradableStrategyInstrument As AmiSignalStrategyInstrument In TradableStrategyInstruments
                _cts.Token.ThrowIfCancellationRequested()
                tasks.Add(Task.Run(AddressOf tradableStrategyInstrument.MonitorAsync, _cts.Token))
            Next
            'Task to run order update periodically
            tasks.Add(Task.Run(AddressOf FillOrderDetailsAsync, _cts.Token))
            tasks.Add(Task.Run(AddressOf MonitorAmiBrokerAsync, _cts.Token))
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
    Public Async Function MonitorAmiBrokerAsync() As Task
        Try
            If Me.ParentController.OrphanException IsNot Nothing Then
                Throw Me.ParentController.OrphanException
            End If
            _cts.Token.ThrowIfCancellationRequested()
            Dim serverIP As Net.IPAddress = Net.IPAddress.Loopback
            Dim serverPort As Integer = 64555
            Dim server As Net.Sockets.TcpListener = New Net.Sockets.TcpListener(serverIP, serverPort)
            Dim client As Net.Sockets.TcpClient = Nothing
            Dim clientData As IO.StreamReader = Nothing
            server.Start()
            While True
                If Me.ParentController.OrphanException IsNot Nothing Then
                    Throw Me.ParentController.OrphanException
                End If
                _cts.Token.ThrowIfCancellationRequested()
                If server.Pending Then
                    client = server.AcceptTcpClient
                    clientData = New IO.StreamReader(client.GetStream)
                    'Console.WriteLine(clientData.ReadLine())
                    PopulateSignalAsync(clientData.ReadLine())
                End If
                Await Task.Delay(100).ConfigureAwait(False)
            End While
        Catch ex As Exception
            'To log exceptions getting created from this function as the bubble up of the exception
            'will anyways happen to Strategy.MonitorAsync but it will not be shown until all tasks exit
            logger.Error("Strategy:{0}, error:{1}", Me.ToString, ex.ToString)
            Throw ex
        End Try
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
    Private Async Function PopulateSignalAsync(ByVal signal As String) As Task
        Dim signalarr() As String = signal.Trim.Split(" ")
        'TODO: Validation of 2 items, first item buy,sell,cover,short. second should present in tradable instrument list.
        Dim currentSignal As AmiSignal = Nothing
        If EntrySignals Is Nothing Then EntrySignals = New Concurrent.ConcurrentDictionary(Of String, AmiSignal)
        If ExitSignals Is Nothing Then ExitSignals = New Concurrent.ConcurrentDictionary(Of String, AmiSignal)

        Select Case signalarr(0).ToUpper()
            Case "BUY"
                currentSignal = New AmiSignal
                With currentSignal
                    .Direction = APIAdapter.TransactionType.Buy
                    .InstrumentIdentifier = signalarr(1)
                    .SignalType = TypeOfSignal.Entry
                    .Timestamp = Now()
                End With
                EntrySignals.GetOrAdd(currentSignal.InstrumentIdentifier, currentSignal)
            Case "SELL"
                currentSignal = New AmiSignal
                With currentSignal
                    .Direction = APIAdapter.TransactionType.Sell
                    .InstrumentIdentifier = signalarr(1)
                    .SignalType = TypeOfSignal.Exit
                    .Timestamp = Now()
                End With
                ExitSignals.GetOrAdd(currentSignal.InstrumentIdentifier, currentSignal)
            Case "SHORT"
                currentSignal = New AmiSignal
                With currentSignal
                    .Direction = APIAdapter.TransactionType.Sell
                    .InstrumentIdentifier = signalarr(1)
                    .SignalType = TypeOfSignal.Entry
                    .Timestamp = Now()
                End With
                EntrySignals.GetOrAdd(currentSignal.InstrumentIdentifier, currentSignal)
            Case "COVER"
                currentSignal = New AmiSignal
                With currentSignal
                    .Direction = APIAdapter.TransactionType.Buy
                    .InstrumentIdentifier = signalarr(1)
                    .SignalType = TypeOfSignal.Exit
                    .Timestamp = Now()
                End With
                ExitSignals.GetOrAdd(currentSignal.InstrumentIdentifier, currentSignal)
        End Select
        'TODO: Need to validate get or add return to check unique entry
    End Function

    <Serializable>
    Public Class AmiSignal
        Public InstrumentIdentifier As String
        Public Direction As APIAdapter.TransactionType
        Public SignalType As TypeOfSignal
        Public Timestamp As Date
    End Class
    Public Enum TypeOfSignal
        Entry = 1
        [Exit]
        None
    End Enum
End Class