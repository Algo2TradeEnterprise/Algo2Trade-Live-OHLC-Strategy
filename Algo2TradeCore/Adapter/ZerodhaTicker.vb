Imports System.Threading
Imports Algo2TradeCore.Controller
Imports KiteConnect
Imports NLog

Namespace Adapter
    Public Class ZerodhaTicker
        Inherits APITicker
        Protected _ticker As Ticker

#Region "Logging and Status Progress"
        Public Shared Shadows logger As Logger = LogManager.GetCurrentClassLogger
#End Region

        Public Sub New(ByVal associatedParentController As APIStrategyController,
                       ByVal canceller As CancellationTokenSource)
            MyBase.New(associatedParentController, canceller)
        End Sub
        Public Overrides Async Function ConnectTickerAsync() As Task
            logger.Debug("{0}->ConnectTickerAsync, parameters:Nothing", Me.ToString)
            Await Task.Delay(0).ConfigureAwait(False)
            Dim currentZerodhaStrategyController As ZerodhaStrategyController = CType(ParentContoller, ZerodhaStrategyController)
            _ticker = New Ticker(ParentContoller.APIConnection.APIUser.APIKey, ParentContoller.APIConnection.AccessToken)
            AddHandler _ticker.OnTick, AddressOf currentZerodhaStrategyController.OnTickerTickAsync
            AddHandler _ticker.OnReconnect, AddressOf currentZerodhaStrategyController.OnTickerReconnect
            AddHandler _ticker.OnNoReconnect, AddressOf currentZerodhaStrategyController.OnTickerNoReconnect
            AddHandler _ticker.OnError, AddressOf currentZerodhaStrategyController.OnTickerError
            AddHandler _ticker.OnClose, AddressOf currentZerodhaStrategyController.OnTickerClose
            AddHandler _ticker.OnConnect, AddressOf currentZerodhaStrategyController.OnTickerConnect
            AddHandler _ticker.OnOrderUpdate, AddressOf currentZerodhaStrategyController.OnTickerOrderUpdateAsync

            _ticker.EnableReconnect(Interval:=5, Retries:=50)
            _ticker.Connect()
        End Function

        Public Overrides Async Function SubscribeAsync(ByVal instrumentIdentifiers As List(Of String)) As Task
            logger.Debug("{0}->SubscribeAsync, instrumentIdentifiers:{1}", Me.ToString, Utils.JsonSerialize(instrumentIdentifiers))
            Await Task.Delay(0).ConfigureAwait(False)
            If _subscribedInstruments Is Nothing Then _subscribedInstruments = New List(Of String)
            Dim subscriptionList() As UInt32 = Nothing

            Dim index As Integer = -1
            For Each runninInstrumentIdentifier In instrumentIdentifiers
                If _subscribedInstruments.Contains(runninInstrumentIdentifier) Then Continue For
                index += 1
                If index = 0 Then
                    ReDim subscriptionList(0)
                Else
                    ReDim Preserve subscriptionList(UBound(subscriptionList) + 1)
                End If
                subscriptionList(index) = runninInstrumentIdentifier
                _subscribedInstruments.Add(runninInstrumentIdentifier)
            Next
            _ticker.Subscribe(Tokens:=subscriptionList)
            _ticker.SetMode(Tokens:=subscriptionList, Mode:=Constants.MODE_FULL)
            OnHeartbeat("Subscribed")
        End Function
        Public Overrides Function ToString() As String
            Return Me.GetType.ToString
        End Function

    End Class
End Namespace