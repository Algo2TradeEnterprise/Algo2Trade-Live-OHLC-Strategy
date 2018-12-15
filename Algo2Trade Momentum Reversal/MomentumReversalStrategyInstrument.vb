Imports System.Threading
Imports Algo2TradeCore
Imports Algo2TradeCore.Adapter
Imports Algo2TradeCore.Entity
Imports Algo2TradeCore.Strategy
Imports NLog

Public Class MomentumReversalStrategyInstrument
    Inherits StrategyInstrument

#Region "Logging and Status Progress"
    Public Shared Shadows logger As Logger = LogManager.GetCurrentClassLogger
#End Region

    Public Sub New(ByVal apiAdapter As APIAdapter, ByVal associatedInstrument As IInstrument, ByVal canceller As CancellationTokenSource)
        MyBase.New(apiAdapter, associatedInstrument, canceller)
    End Sub
    Public Shared Async Function GetAllTradableInstrumentsAsync(ByVal allInstruments As IEnumerable(Of IInstrument),
                                                                ByVal apiAdapter As APIAdapter,
                                                                ByVal canceller As CancellationTokenSource) As Task(Of List(Of MomentumReversalStrategyInstrument))
        Dim ret As List(Of MomentumReversalStrategyInstrument) = Nothing
        Await Task.Delay(0).ConfigureAwait(False)
        If allInstruments IsNot Nothing AndAlso allInstruments.Count > 0 Then

            Dim retCtr As Integer = 0
            logger.Debug("Generating strategy instrument list and mapping to broker instrument list")
            For Each runningInstrument In allInstruments
                'TO DO: Check if the instrument needs to be added or use custom code to extract instruments from DB or logic 
                'and then add to the rerturnable collection
                retCtr += 1
                If retCtr > 500 Then Exit For
                If ret Is Nothing Then ret = New List(Of MomentumReversalStrategyInstrument)
                ret.Add(New MomentumReversalStrategyInstrument(apiAdapter, runningInstrument, canceller))
            Next
        End If
        If ret IsNot Nothing Then
            logger.Debug("Generated strategy instrument list mapped to broker instrument list, count:{0}", ret.Count)
            If ret.Count > apiAdapter.MaxInstrumentPerTicker Then
                Throw New ApplicationException(String.Format("Max instruments per ticker exceeded, allowed:{0}, existing:{1}", apiAdapter.MaxInstrumentPerTicker, ret.Count))
            End If
        End If
        Return ret
    End Function
    Public Overrides Function ToString() As String
        Return "Momentum Reversal"
    End Function
    Public Overrides Async Function RunDirectAsync() As Task
        Try
            Dim allTrades As IEnumerable(Of ITrade) = Await _apiAdapter.GetAllTradesAsync(New Dictionary(Of String, Object) From {{"xxx", TradableInstrument.InstrumentIdentifier}}, retryEnabled:=False).ConfigureAwait(False)
        Catch ex As Exception
            logger.Warn("For instrument:{0}, error:{1}", TradableInstrument.InstrumentIdentifier, ex.Message)
            'Throw ex
        End Try
    End Function
End Class
