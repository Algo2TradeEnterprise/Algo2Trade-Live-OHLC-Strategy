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

    Public Sub New(ByVal apiConnection As IConnection, ByVal associatedInstrument As IInstrument, ByVal parentStrategy As Strategy, ByVal canceller As CancellationTokenSource)
        MyBase.New(apiConnection, associatedInstrument, parentStrategy, canceller)
    End Sub
    Public Overrides Function ToString() As String
        Return Me.GetType().Name
    End Function
    Public Overrides Async Function RunDirectAsync() As Task
        'Try
        '    Dim allTrades As IEnumerable(Of ITrade) = Await _apiAdapter.GetAllTradesAsync(New Dictionary(Of String, Object) From {{"xxx", TradableInstrument.InstrumentIdentifier}}, retryEnabled:=True).ConfigureAwait(False)
        'Catch ex As Exception
        '    logger.Warn("For instrument:{0}, error:{1}", TradableInstrument.InstrumentIdentifier, ex.Message)
        '    'Throw ex
        'End Try
    End Function
End Class
