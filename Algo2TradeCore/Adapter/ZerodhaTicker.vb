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

            If _ticker IsNot Nothing Then
                RemoveHandler _ticker.OnTick, AddressOf currentZerodhaStrategyController.OnTickerTickAsync
                RemoveHandler _ticker.OnReconnect, AddressOf currentZerodhaStrategyController.OnTickerReconnect
                RemoveHandler _ticker.OnNoReconnect, AddressOf currentZerodhaStrategyController.OnTickerNoReconnect
                RemoveHandler _ticker.OnError, AddressOf currentZerodhaStrategyController.OnTickerError
                RemoveHandler _ticker.OnClose, AddressOf currentZerodhaStrategyController.OnTickerClose
                RemoveHandler _ticker.OnConnect, AddressOf currentZerodhaStrategyController.OnTickerConnect
                RemoveHandler _ticker.OnOrderUpdate, AddressOf currentZerodhaStrategyController.OnTickerOrderUpdateAsync

                If _ticker.IsConnected Then _ticker.Close()
            End If
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
            If subscriptionList Is Nothing OrElse UBound(subscriptionList) = 0 Then
                logger.Error("No tokens to subscribe")
            Else
                _ticker.Subscribe(Tokens:=subscriptionList)
                _ticker.SetMode(Tokens:=subscriptionList, Mode:=Constants.MODE_FULL)
                OnHeartbeat("Subscribed:" & subscriptionList.Count)
            End If
        End Function
        Public Overrides Function ToString() As String
            Return Me.GetType.ToString
        End Function
        Public Overrides Sub ClearLocalUniqueSubscriptionList()
            _subscribedInstruments = Nothing
        End Sub
        Public Overrides Function IsConnected() As Boolean
            If _ticker IsNot Nothing Then
                Return _ticker.IsConnected
            Else
                Return False
            End If
        End Function
        Public Overrides Async Function CloseTickerIfConnectedAsync() As Task
            Await Task.Delay(0)
            If _ticker IsNot Nothing AndAlso _ticker.IsConnected Then
                _ticker.Close()
            End If
        End Function

    End Class
End Namespace