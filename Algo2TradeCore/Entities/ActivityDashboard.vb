Namespace Entities
    <Serializable>
    Public Class ActivityDashboard
        Public Sub New()
            EntryActivity = New Activity
            TargetModifyActivity = New Activity
            StoplossModifyActivity = New Activity
            CancelActivity = New Activity
        End Sub

        Public Property TradingSymbol As String
        Public Property SignalGeneratedTime As Date
        Public Property EntryActivity As Activity
        Public Property TargetModifyActivity As Activity
        Public Property StoplossModifyActivity As Activity
        Public Property CancelActivity As Activity
        'Public Property SignalStatus As SignalStatusType
        Public Property ProfitLossOfSignal As Decimal
        Public Property ActiveInstrument As Boolean
        Public Property TotalExecutedOrders As Integer
        Public Property OverallProfitLoss As Decimal

        Public Class Activity
            Public Property RequestTime As Date
            Public Property ReceivedTime As Date
            Public Property RequestStatus As SignalStatusType
            Public Property RequestRemarks As String
            Public Property LastException As Exception
            Public Property Supporting As String
        End Class
        Public Enum SignalStatusType
            Handled = 1
            Activated
            Running
            Complete
            Cancelled
            Rejected
            Discarded
            None
        End Enum
    End Class
End Namespace
