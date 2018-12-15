Imports System.Threading
Imports Algo2TradeBLL
Imports KiteConnect
Imports NLog

Public Class OHLStrategyHelper
#Region "Events/Event handlers"
    Public Event DocumentDownloadComplete()
    Public Event DocumentRetryStatus(ByVal currentTry As Integer, ByVal totalTries As Integer)
    Public Event Heartbeat(ByVal msg As String)
    Public Event WaitingFor(ByVal elapsedSecs As Integer, ByVal totalSecs As Integer, ByVal msg As String)
    'The below functions are needed to allow the derived classes to raise the above two events
    Protected Overridable Sub OnDocumentDownloadComplete()
        RaiseEvent DocumentDownloadComplete()
    End Sub
    Protected Overridable Sub OnDocumentRetryStatus(ByVal currentTry As Integer, ByVal totalTries As Integer)
        RaiseEvent DocumentRetryStatus(currentTry, totalTries)
    End Sub
    Protected Overridable Sub OnHeartbeat(ByVal msg As String)
        RaiseEvent Heartbeat(msg)
    End Sub
    Protected Overridable Sub OnWaitingFor(ByVal elapsedSecs As Integer, ByVal totalSecs As Integer, ByVal msg As String)
        RaiseEvent WaitingFor(elapsedSecs, totalSecs, msg)
    End Sub
#End Region

#Region "Logging and Status Progress"
    Public Shared logger As Logger = LogManager.GetCurrentClassLogger
#End Region

#Region "Property"
    Private _canceller As CancellationTokenSource = Nothing
    Private _zerodhaKite As ZerodhaKiteHelper
#End Region

#Region "Constructor"
    Public Sub New(ByVal zerodhaKite As ZerodhaKiteHelper, ByVal canceller As CancellationTokenSource)
        _zerodhaKite = zerodhaKite
        _canceller = canceller
    End Sub
#End Region

    Public Function GetTodaysInstruments(ByVal inputDT As DataTable, ByVal allInstruments As List(Of Instrument)) As Dictionary(Of String, OHLStrategyTradableInstrument)
        Dim ret As Dictionary(Of String, OHLStrategyTradableInstrument) = Nothing
        If inputDT IsNot Nothing AndAlso inputDT.Rows.Count > 0 AndAlso inputDT.Columns.Count = 2 Then
            logger.Debug("Checking {0} input instruments for match with active instruments", inputDT.Rows.Count)
            For i As Integer = 0 To inputDT.Rows.Count - 1
                logger.Debug("Row: {0}/{1}", i + 1, inputDT.Rows.Count)
                _canceller.Token.ThrowIfCancellationRequested()
                Dim tradingSymbol As String = inputDT.Rows(i).Item(0)
                Dim brokerInstrument As Instrument = allInstruments.Find(Function(x)
                                                                             Return x.TradingSymbol = tradingSymbol
                                                                         End Function)
                If brokerInstrument.TradingSymbol = tradingSymbol Then
                    Dim runningInstrument As New OHLStrategyTradableInstrument(brokerInstrument)
                    runningInstrument.IsTradable = True
                    runningInstrument.TargetOfStock = inputDT.Rows(i).Item(1)

                    Dim eachInstrumentWorker As New OHLInstrumentWorker(runningInstrument, _zerodhaKite, _canceller)
                    AddHandler eachInstrumentWorker.Heartbeat, AddressOf OnHeartbeat
                    AddHandler eachInstrumentWorker.DocumentDownloadComplete, AddressOf OnDocumentDownloadComplete
                    AddHandler eachInstrumentWorker.DocumentRetryStatus, AddressOf OnDocumentRetryStatus
                    AddHandler eachInstrumentWorker.WaitingFor, AddressOf OnWaitingFor
                    runningInstrument.StrategyWorker = eachInstrumentWorker

                    If ret Is Nothing Then ret = New Dictionary(Of String, OHLStrategyTradableInstrument)
                    ret.Add(runningInstrument.WrappedInstrument.InstrumentToken, runningInstrument)
                Else
                    logger.Error("Not found instrument:{0} in the active instruments", tradingSymbol)
                End If
            Next
        Else
            Throw New ApplicationException("Invalid datatable to get tradable instrument data")
        End If
        Dim count As Integer = If(ret Is Nothing, 0, ret.Count)
        logger.Debug("Total tradable instruments created:{0}", count)
        Return ret
    End Function
    Public Async Function RunAsync(ByVal tradableIstruments As Dictionary(Of String, OHLStrategyTradableInstrument)) As Task
        While True
            If tradableIstruments IsNot Nothing AndAlso tradableIstruments.Count > 0 Then
                Dim currentTime As Date = Now
                Dim OHLStrategyTradeTime As TimeSpan = TimeSpan.Parse(My.Settings.OHLStrategyTradeTime)
                If currentTime.Hour = OHLStrategyTradeTime.Hours AndAlso currentTime.Minute = OHLStrategyTradeTime.Minutes AndAlso currentTime.Second >= OHLStrategyTradeTime.Seconds Then
                    Dim instrumentStrategyTasks As IEnumerable(Of Task) = From tradableInstrument In tradableIstruments
                                                                          Select tradableInstrument.Value.StrategyWorker.RunStrategyAsync()
                    Await Task.WhenAll(instrumentStrategyTasks).ConfigureAwait(False)
                Else
                    Await Task.Delay(500).ConfigureAwait(False)
                    Continue While
                End If
            Else
                Throw New ApplicationException("Cannot run any strategies as no instrument available")
            End If
            Exit While
        End While
    End Function
End Class
