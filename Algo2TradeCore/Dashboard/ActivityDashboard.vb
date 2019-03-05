Namespace Dashboard
    <Serializable>
    Public Class ActivityDashboard
        Public Property TradingSymbol As String
        Public Property SignalGeneratedTime As Date
        Public Property EntryRequestTime As Date
        Public Property EntryReceivedTime As Date
        Public Property SignalStatus As String
        Public Property ProfitLossOfSignal As String
        Public Property ActiveInstrument As Boolean
        Public Property TotalExecutedOrders As Integer
        Public Property OverallProfitLoss As Integer
    End Class
End Namespace
