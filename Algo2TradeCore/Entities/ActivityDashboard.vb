Imports System.ComponentModel.DataAnnotations

Namespace Entities
    Public Class ActivityDashboard
        Public Sub New()
            EntryActivity = New Activity(ActivityType.Entry)
            TargetModifyActivity = New Activity(ActivityType.TargetModify)
            StoplossModifyActivity = New Activity(ActivityType.StoplossModify)
            CancelActivity = New Activity(ActivityType.Cancel)
        End Sub

        <Display(Name:="Symbol", Order:=0)>
        Public Property TradingSymbol As String

        <Display(Name:="Signal Generated Time", Order:=1)>
        Public Property SignalGeneratedTime As Date

        <Display(Name:="Signal Direction", Order:=2)>
        Public ReadOnly Property SignalDirection As String

        <Display(Name:="Signal PL", Order:=3)>
        Public Property ProfitLossOfSignal As Decimal

        <Display(Name:="Active Instrument", Order:=4)>
        Public Property ActiveInstrument As Boolean

        <Display(Name:="Total Executed Orders", Order:=5)>
        Public Property TotalExecutedOrders As Integer

        <Display(Name:="Overall PL", Order:=6)>
        Public Property OverallProfitLoss As Decimal

        <System.ComponentModel.Browsable(False)>
        Public Property EntryActivity As Activity
        <Display(Name:="Entry Request Time", Order:=7)>
        Public ReadOnly Property EntryRequestTime As Date
        <Display(Name:="Entry Request Status", Order:=8)>
        Public ReadOnly Property EntryRequestStatus As SignalStatusType

        <System.ComponentModel.Browsable(False)>
        Public Property TargetModifyActivity As Activity
        <Display(Name:="Target Modify Request Time", Order:=9)>
        Public ReadOnly Property TargetModifyRequestTime As Date
        <Display(Name:="Target Modify Request Status", Order:=10)>
        Public ReadOnly Property TargetModifyRequestStatus As SignalStatusType

        <System.ComponentModel.Browsable(False)>
        Public Property StoplossModifyActivity As Activity
        <Display(Name:="Stoploss Modify Request Time", Order:=11)>
        Public ReadOnly Property StoplossModifyRequestTime As Date
        <Display(Name:="Stoploss Modify Request Status", Order:=12)>
        Public ReadOnly Property StoplossModifyRequestStatus As SignalStatusType

        <System.ComponentModel.Browsable(False)>
        Public Property CancelActivity As Activity
        <Display(Name:="Cancel Request Time", Order:=13)>
        Public ReadOnly Property CancelRequestTime As Date
        <Display(Name:="Cancel Request Status", Order:=14)>
        Public ReadOnly Property CancelRequestStatus As SignalStatusType

        <Display(Name:="Last Price", Order:=15)>
        Public Property LastPrice As Decimal

        <Display(Name:="Timestamp", Order:=16)>
        Public Property Timestamp As Date?

        <Display(Name:="Last Candle Time", Order:=17)>
        Public Property LastCandleTime As Date

        <System.ComponentModel.Browsable(False)>
        Public Property ParentOrderID As String


        Public Class Activity
            Public Sub New(ByVal typeOfActivity As ActivityType)
                Me.TypeOfActivity = typeOfActivity
                PreviousActivityAttributes = New Activity(Me.TypeOfActivity)
            End Sub
            Public ReadOnly Property TypeOfActivity As ActivityType

            Private _RequestTime As Date
            Public Property RequestTime As Date
                Get
                    Return _RequestTime
                End Get
                Set(value As Date)
                    PreviousActivityAttributes.RequestTime = _RequestTime
                    _RequestTime = value
                End Set
            End Property

            Private _ReceivedTime As Date
            Public Property ReceivedTime As Date
                Get
                    Return _ReceivedTime
                End Get
                Set(value As Date)
                    PreviousActivityAttributes.ReceivedTime = _ReceivedTime
                    _ReceivedTime = value
                End Set
            End Property

            Private _RequestStatus As SignalStatusType
            Public Property RequestStatus As SignalStatusType
                Get
                    Return _RequestStatus
                End Get
                Set(value As SignalStatusType)
                    PreviousActivityAttributes.RequestStatus = _RequestStatus
                    _RequestStatus = value
                End Set
            End Property

            Private _RequestRemarks As String
            Public Property RequestRemarks As String
                Get
                    Return _RequestRemarks
                End Get
                Set(value As String)
                    PreviousActivityAttributes.RequestRemarks = _RequestRemarks
                    _RequestRemarks = value
                End Set
            End Property

            Private _LastException As Exception
            Public Property LastException As Exception
                Get
                    Return _LastException
                End Get
                Set(value As Exception)
                    PreviousActivityAttributes.LastException = _LastException
                    _LastException = value
                End Set
            End Property

            Private _Supporting As String
            Public Property Supporting As String
                Get
                    Return _Supporting
                End Get
                Set(value As String)
                    PreviousActivityAttributes.Supporting = _Supporting
                    _Supporting = value
                End Set
            End Property
            Public Property PreviousActivityAttributes As Activity
        End Class
        Public Enum ActivityType
            Entry = 1
            TargetModify
            StoplossModify
            Cancel
            None
        End Enum
        Public Enum SignalStatusType
            Handled = 1
            Activated
            Running
            Completed
            Cancelled
            Rejected
            Discarded
            None
        End Enum

        'Signal Status flow diagram
        'Entry Activity: Handled->Activated->Running->Complete/Cancelled/Rejected/Discarded
        'Modify/Cancel Activity: Handled->Activated->Complete/Rejected

    End Class
End Namespace
