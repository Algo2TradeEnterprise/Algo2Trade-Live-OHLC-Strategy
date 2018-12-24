Namespace Strategies
    Public Class Trigger
        Public Enum TriggerType
            Timebased = 1
            PriceMatched
            HLBreached
            None
        End Enum
        Public Property Category As TriggerType
        Public Property Description As String
    End Class
End Namespace