Namespace Entities.Indicators
    Public Class SMAConsumer
        Inherits PayloadToIndicatorConsumer
        Public Sub New()
            MyBase.New(Indicator.SMA)
        End Sub
        Public ReadOnly Property SMAPeriod As Integer
        Public Property SMAPayloads As Concurrent.ConcurrentDictionary(Of Date, Object)
    End Class
End Namespace