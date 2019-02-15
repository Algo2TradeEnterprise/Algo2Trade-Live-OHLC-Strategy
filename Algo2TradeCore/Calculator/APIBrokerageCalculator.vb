Imports System.Threading

Namespace Calculator
    Public MustInherit Class APIBrokerageCalculator

        Protected _cts As CancellationTokenSource
        Public Sub New(canceller As CancellationTokenSource)
            _cts = canceller
        End Sub
        Public MustOverride Function GetIntradayEquityBrokerage(ByVal buy As Decimal, ByVal sell As Decimal, ByVal quantity As Integer) As IBrokerageAttributes
        Public MustOverride Function GetDeliveryEquityBrokerage(ByVal buy As Decimal, ByVal sell As Decimal, ByVal quantity As Integer) As IBrokerageAttributes
        Public MustOverride Function GetIntradayEquityFuturesBrokerage(ByVal buy As Decimal, ByVal sell As Decimal, ByVal quantity As Integer) As IBrokerageAttributes
        Public MustOverride Async Function GetIntradayCommodityFuturesBrokerageAsync(ByVal item As String, ByVal buy As Decimal, ByVal sell As Decimal, ByVal quantity As Integer) As Task(Of IBrokerageAttributes)
        Public MustOverride Function GetIntradayEquityOptionsBrokerage(ByVal buy As Decimal, ByVal sell As Decimal, ByVal quantity As Integer) As IBrokerageAttributes
        Public MustOverride Function GetIntradayCurrencyFuturesBrokerage(ByVal buy As Decimal, ByVal sell As Decimal, ByVal quantity As Integer) As IBrokerageAttributes
        Public MustOverride Function GetIntradayCurrencyOptionsBrokerage(ByVal StrikePrice As Decimal, ByVal buyPremium As Decimal, ByVal sellPremium As Decimal, ByVal quantity As Integer) As IBrokerageAttributes

    End Class
End Namespace
