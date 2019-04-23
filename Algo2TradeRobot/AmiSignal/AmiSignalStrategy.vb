Imports System.Threading
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

    Public Sub New(ByVal associatedParentController As APIStrategyController,
                   ByVal strategyIdentifier As String,
                   ByVal userSettings As AmiSignalUserInputs,
                   ByVal maxNumberOfDaysForHistoricalFetch As Integer,
                   ByVal canceller As CancellationTokenSource)
        MyBase.New(associatedParentController, strategyIdentifier, False, userSettings, maxNumberOfDaysForHistoricalFetch, canceller)
        Me.ExitAllTrades = False
        'Though the TradableStrategyInstruments is being populated from inside by newing it,
        'lets also initiatilize here so that after creation of the strategy and before populating strategy instruments,
        'the fron end grid can bind to this created TradableStrategyInstruments which will be empty
        'TradableStrategyInstruments = New List(Of StrategyInstrument)
    End Sub
    Public Overrides Async Function CreateTradableStrategyInstrumentsAsync(ByVal allInstruments As IEnumerable(Of IInstrument)) As Task(Of Boolean)
        If allInstruments IsNot Nothing AndAlso allInstruments.Count > 0 Then
            logger.Debug("CreateTradableStrategyInstrumentsAsync, allInstruments.Count:{0}", allInstruments.Count)
        Else
            logger.Debug("CreateTradableStrategyInstrumentsAsync, allInstruments.Count:Nothing or 0")
        End If
        _cts.Token.ThrowIfCancellationRequested()
        Dim ret As Boolean = False
        Dim retTradableInstrumentsAsPerStrategy As List(Of IInstrument) = Nothing
        Await Task.Delay(0, _cts.Token).ConfigureAwait(False)
        logger.Debug("Starting to fill strategy specific instruments, strategy:{0}", Me.ToString)
        If allInstruments IsNot Nothing AndAlso allInstruments.Count > 0 Then
            'Get AmiSignal Strategy Instruments
            Dim amiUserInputs As AmiSignalUserInputs = CType(UserSettings, AmiSignalUserInputs)
            If amiUserInputs.InstrumentsData IsNot Nothing AndAlso amiUserInputs.InstrumentsData.Count > 0 Then
                Dim dummyAllInstruments As List(Of IInstrument) = allInstruments.ToList
                For Each instrument In amiUserInputs.InstrumentsData
                    _cts.Token.ThrowIfCancellationRequested()
                    Dim runningTradableInstrument As IInstrument = dummyAllInstruments.Find(Function(x)
                                                                                                Return x.TradingSymbol.ToUpper = instrument.Value.InstrumentName.ToUpper
                                                                                            End Function)
                    _cts.Token.ThrowIfCancellationRequested()
                    ret = True
                    If retTradableInstrumentsAsPerStrategy Is Nothing Then retTradableInstrumentsAsPerStrategy = New List(Of IInstrument)
                    If runningTradableInstrument IsNot Nothing Then retTradableInstrumentsAsPerStrategy.Add(runningTradableInstrument)
                Next
                TradableInstrumentsAsPerStrategy = retTradableInstrumentsAsPerStrategy
            End If
        End If
        If retTradableInstrumentsAsPerStrategy IsNot Nothing AndAlso retTradableInstrumentsAsPerStrategy.Count > 0 Then
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
                Dim runningTradableStrategyInstrument As New AmiSignalStrategyInstrument(runningTradableInstrument, Me, False, _cts)
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

    Public Overrides Async Function MonitorAsync() As Task
        Dim lastException As Exception = Nothing

        Try
            _cts.Token.ThrowIfCancellationRequested()
            Dim tasks As New List(Of Task)()
            For Each tradableStrategyInstrument As AmiSignalStrategyInstrument In TradableStrategyInstruments
                _cts.Token.ThrowIfCancellationRequested()
                tasks.Add(Task.Run(AddressOf tradableStrategyInstrument.MonitorAsync, _cts.Token))
            Next
            tasks.Add(Task.Run(AddressOf MonitorAmiBrokerAsync, _cts.Token))
            tasks.Add(Task.Run(AddressOf ForceExitAllTradesAsync, _cts.Token))
            Await Task.WhenAll(tasks).ConfigureAwait(False)
        Catch ex As Exception
            lastException = ex
            logger.Error(ex)
        End Try
        If lastException IsNot Nothing Then
            Await ParentController.CloseTickerIfConnectedAsync().ConfigureAwait(False)
            Await ParentController.CloseFetcherIfConnectedAsync(False).ConfigureAwait(False)
            Await ParentController.CloseCollectorIfConnectedAsync(False).ConfigureAwait(False)
            Throw lastException
        End If
    End Function

    Public Async Function MonitorAmiBrokerAsync() As Task
        'logger.Debug("MonitorAmiBrokerAsync, parameters:Nothing")
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
                Try
                    If Me.ParentController.OrphanException IsNot Nothing Then
                        Throw Me.ParentController.OrphanException
                    End If
                    _cts.Token.ThrowIfCancellationRequested()
                    If server.Pending Then
                        client = server.AcceptTcpClient
                        clientData = New IO.StreamReader(client.GetStream)
                        PopulateExternalSignalAsync(clientData.ReadLine())
                    End If
                Catch cex As OperationCanceledException
                    logger.Error(cex)
                    Me.ParentController.OrphanException = cex
                    Exit While
                Catch iex As Exception
                    logger.Error("Strategy:{0}, error:{1}", Me.ToString, iex.ToString)
                    If server IsNot Nothing Then server.Stop()
                    server = Nothing
                    server = New Net.Sockets.TcpListener(serverIP, serverPort)
                    server.Start()
                End Try
                Await Task.Delay(100, _cts.Token).ConfigureAwait(False)
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

    Protected Overrides Function IsTriggerReceivedForExitAllOrders() As Tuple(Of Boolean, String)
        Dim ret As Tuple(Of Boolean, String) = Nothing
        Dim currentTime As Date = Now
        If currentTime >= Me.UserSettings.EODExitTime Then
            ret = New Tuple(Of Boolean, String)(True, "EOD Exit")
        End If
        Return ret
    End Function

    Private Async Function PopulateExternalSignalAsync(ByVal signal As String) As Task
        logger.Debug("PopulateExternalSignalAsync, parameters:{0}", signal)
        Await Task.Delay(0, _cts.Token).ConfigureAwait(False)
        If Me.TradableStrategyInstruments IsNot Nothing AndAlso Me.TradableStrategyInstruments.Count > 0 Then
            Dim signalarr() As String = signal.Trim.Split(" ")

            'Signal format
            'Entry ADANIPORTS-I Buy MKT 0 200
            'StopLoss ADANIPORTS-I Sell SL-M 390 200
            'Target ADANIPORTS-I Sell LIMIT 392 200
            'Entry ADANIPORTS-I Short MKT 0 200
            'StopLoss ADANIPORTS-I Cover SL-M 393 200
            'Target ADANIPORTS-I Cover LIMIT 391 200

            If signalarr.Count > 6 Then
                logger.Error(New ApplicationException(String.Format("Invalid Signal Details. {0}", signal)))
                Exit Function
            End If
            Dim amiUserInputs As AmiSignalUserInputs = CType(UserSettings, AmiSignalUserInputs)
            If amiUserInputs.InstrumentsData.ContainsKey(signalarr(1).ToUpper) Then
                Dim runningStrategyInstruments As IEnumerable(Of StrategyInstrument) = Me.TradableStrategyInstruments.Where(Function(x)
                                                                                                                                Return x.TradableInstrument.TradingSymbol.ToUpper = amiUserInputs.InstrumentsData(signalarr(1).ToUpper).InstrumentName.ToUpper
                                                                                                                            End Function)
                If runningStrategyInstruments IsNot Nothing AndAlso runningStrategyInstruments.Count > 0 Then
                    CType(runningStrategyInstruments.FirstOrDefault, AmiSignalStrategyInstrument).PopulateExternalSignalAsync(signal)
                End If
            Else
                logger.Error(New ApplicationException(String.Format("Instrument is not available in the given list. {0}", signal)))
            End If
        End If
    End Function
End Class
