Imports Algo2TradeCore.Adapter

Namespace Entities
    Public Class PlaceOrderParameters
        Public Property EntryDirection As APIAdapter.TransactionType = APIAdapter.TransactionType.None
        Public Property Quantity As Integer = 0
        Public Property Price As Decimal = Nothing
        Public Property TriggerPrice As Decimal = Nothing
        Public Property SquareOffValue As Decimal = Nothing
        Public Property StoplossValue As Decimal = Nothing
        'Public Property Tag As String = Nothing
        Public Property SignalCandle As IPayload = Nothing
        Public Overrides Function ToString() As String
            Return String.Format("{0}{1}{2}{3}{4}{5}", EntryDirection.ToString(), Price, TriggerPrice, SquareOffValue, StoplossValue, If(SignalCandle Is Nothing, "Nothing", SignalCandle.SnapshotDateTime.ToString()))
        End Function
    End Class
End Namespace