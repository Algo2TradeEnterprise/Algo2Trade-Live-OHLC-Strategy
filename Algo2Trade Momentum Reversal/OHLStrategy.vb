Imports System.Threading
Imports Algo2TradeCore.Controller
Imports Algo2TradeCore.Entities
Imports Algo2TradeCore.Strategies
Imports NLog

Public Class OHLStrategy
    Inherits Strategy
#Region "Logging and Status Progress"
    Public Shared Shadows logger As Logger = LogManager.GetCurrentClassLogger
#End Region

    Public Sub New(ByVal parentContoller As APIStrategyController, ByVal canceller As CancellationTokenSource)
        MyBase.New(parentContoller, canceller)
    End Sub
    Public Overrides Async Function FillTradableInstrumentsAsync(ByVal allInstruments As IEnumerable(Of IInstrument)) As Task(Of Boolean)
        If allInstruments IsNot Nothing AndAlso allInstruments.Count > 0 Then
            logger.Debug("FillTradableInstrumentsAsync, allInstruments.Count:{0}", allInstruments.Count)
        Else
            logger.Debug("FillTradableInstrumentsAsync, allInstruments.Count:0")
        End If
        Dim ret As Boolean = False
        Dim retInstrument As List(Of IInstrument) = Nothing
        Await Task.Delay(0).ConfigureAwait(False)
        logger.Debug("Starting to fill strategy specific instruments, strategy:{0}", Me.ToString)
        If allInstruments IsNot Nothing AndAlso allInstruments.Count > 0 Then
            'TO DO: change to actual code here
            For i As Integer = 0 To 10
                ret = True
                If retInstrument Is Nothing Then retInstrument = New List(Of IInstrument)
                retInstrument.Add(allInstruments(i))
            Next
        End If

        If ret Then
            _tradableInstruments = retInstrument
            logger.Warn("Strategy specific instruments obtained, _tradableInstruments.Count:{0}", _tradableInstruments.Count)
        Else
            logger.Warn("Strategy specific instruments obtained, _tradableInstruments.Count:{0}", 0)
        End If
        Return ret
    End Function
    Public Overrides Async Function ExecuteAsync() As Task
        logger.Debug("ExecuteAsync, parameters:Nothing")
        Await Task.Delay(0).ConfigureAwait(False)
        If _tradableInstruments IsNot Nothing AndAlso _tradableInstruments.Count > 0 Then
            Dim retTradableStrategyInstruments As List(Of OHLStrategyInstrument) = Nothing
            logger.Debug("Creating strategy tradable instruments, _tradableInstruments.count:{0}", _tradableInstruments.Count)
            For Each runningTradableInstrument In _tradableInstruments
                If retTradableStrategyInstruments Is Nothing Then retTradableStrategyInstruments = New List(Of OHLStrategyInstrument)
                Dim runningTradableStrategyInstrument As New OHLStrategyInstrument(runningTradableInstrument, Me, _cts)
                AddHandler runningTradableStrategyInstrument.Heartbeat, AddressOf OnHeartbeat
                AddHandler runningTradableStrategyInstrument.WaitingFor, AddressOf OnWaitingFor
                AddHandler runningTradableStrategyInstrument.DocumentRetryStatus, AddressOf OnDocumentRetryStatus
                AddHandler runningTradableStrategyInstrument.DocumentDownloadComplete, AddressOf OnDocumentDownloadComplete

                retTradableStrategyInstruments.Add(runningTradableStrategyInstrument)
            Next
            _tradableStrategyInstruments = retTradableStrategyInstruments
        Else
            Throw New ApplicationException(String.Format("Cannot run this strategy as no strategy instruments could be created from the tradable instruments, stratgey:{0}", Me.ToString))
        End If
        'To fire any time based common calls to the strategy instruments
        If _tradableStrategyInstruments IsNot Nothing AndAlso _tradableStrategyInstruments.Count > 0 Then
            For Each runningTradableStrategyInstrument In _tradableStrategyInstruments
                runningTradableStrategyInstrument.RunDirectAsync()
            Next
        End If
    End Function
    Public Overrides Function ToString() As String
        Return Me.GetType().Name
    End Function
End Class
