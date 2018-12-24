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
        Try
            While Me.ParentStrategy.ParentContoller.APIConnection Is Nothing
                logger.Debug("Waiting for fresh token:{0}", TradableInstrument.InstrumentIdentifier)
                Await Task.Delay(500).ConfigureAwait(False)
            End While
            Dim triggerRecevied As Tuple(Of Boolean, Trigger) = Await IsTriggerReachedAsync().ConfigureAwait(False)
            If triggerRecevied IsNot Nothing AndAlso triggerRecevied.Item1 = True Then
                _APIAdapter.SetAPIAccessToken(Me.ParentStrategy.ParentContoller.APIConnection.AccessToken)
                Dim allTrades As IEnumerable(Of ITrade) = Await _APIAdapter.GetAllTradesAsync().ConfigureAwait(False)
            End If
        Catch ex As Exception
            logger.Debug("^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^")
            logger.Error(ex.ToString)
        End Try
    End Function
    Public Overrides Async Function IsTriggerReachedAsync() As Task(Of Tuple(Of Boolean, Trigger))
        Dim ret As Tuple(Of Boolean, Trigger) = Nothing
        If _LastTick IsNot Nothing AndAlso _LastTick.Timestamp IsNot Nothing AndAlso _LastTick.Open = _LastTick.Low Then
            ret = New Tuple(Of Boolean, Trigger)(True, New Trigger() With {.Category = Trigger.TriggerType.PriceMatched, .Description = String.Format("O=L,({0})", _LastTick.Open)})
        ElseIf _LastTick IsNot Nothing AndAlso _LastTick.Timestamp IsNot Nothing AndAlso _LastTick.Open = _LastTick.High Then
            ret = New Tuple(Of Boolean, Trigger)(True, New Trigger() With {.Category = Trigger.TriggerType.PriceMatched, .Description = String.Format("O=H,({0})", _LastTick.Open)})
        End If
        Return ret
    End Function

End Class
