Imports System.Threading
Imports Algo2TradeCore
Imports Algo2TradeCore.Adapter
Imports Algo2TradeCore.Entities
Imports Algo2TradeCore.Strategies
Imports NLog

Public Class OHLStrategyInstrument
    Inherits StrategyInstrument

#Region "Logging and Status Progress"
    Public Shared Shadows logger As Logger = LogManager.GetCurrentClassLogger
#End Region

    Public Sub New(ByVal associatedInstrument As IInstrument, ByVal parentStrategy As Strategy, ByVal canceller As CancellationTokenSource)
        MyBase.New(associatedInstrument, parentStrategy, canceller)
        _APIAdapter = New ZerodhaAdapter(parentStrategy.ParentContoller, _cts)
        AddHandler _APIAdapter.Heartbeat, AddressOf OnHeartbeat
        AddHandler _APIAdapter.WaitingFor, AddressOf OnWaitingFor
        AddHandler _APIAdapter.DocumentRetryStatus, AddressOf OnDocumentRetryStatus
        AddHandler _APIAdapter.DocumentDownloadComplete, AddressOf OnDocumentDownloadComplete
    End Sub
    Public Overrides Function ToString() As String
        Return Me.GetType().Name
    End Function
    Public Overrides Async Function RunDirectAsync() As Task
        While True
            Try
                While Me.ParentStrategy.ParentContoller.APIConnection Is Nothing
                    logger.Debug("Waiting for fresh token:{0}", TradableInstrument.InstrumentIdentifier)
                    Await Task.Delay(500).ConfigureAwait(False)
                End While
                _APIAdapter.SetAPIAccessToken(Me.ParentStrategy.ParentContoller.APIConnection.AccessToken)
                Dim allTrades As IEnumerable(Of ITrade) = Await _APIAdapter.GetAllTradesAsync().ConfigureAwait(False)
            Catch ex As Exception
                logger.Debug("^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^")
                logger.Error(ex.ToString)
            End Try
            Await Task.Delay(10000).ConfigureAwait(False)
        End While
    End Function
End Class
