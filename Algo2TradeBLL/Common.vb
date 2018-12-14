Public Module Common
    Public Enum TypeOfStock
        Cash = 1
        Currency
        Commodity
        Futures
        None
    End Enum
    Public Function CalculateQuantityFromInvestment(ByVal lotSize As Integer, ByVal investment As Double, ByVal stockPrice As Double, ByVal typeOfStock As TypeOfStock) As Integer
        'TODO: Change Margin Multiplier
        Dim marginMultiplier As Decimal = 13
        Dim quantity As Integer = lotSize
        Dim quantityMultiplier As Double = investment / (quantity * stockPrice / marginMultiplier)
        Select Case typeOfStock
            Case TypeOfStock.Cash
                quantityMultiplier = quantityMultiplier
            Case TypeOfStock.Commodity
                quantityMultiplier = Math.Floor(quantityMultiplier)
            Case TypeOfStock.Currency
                Throw New ApplicationException("Not Implemented")
            Case TypeOfStock.Futures
                quantityMultiplier = Math.Floor(quantityMultiplier)
        End Select
        quantity = Math.Floor(quantity * quantityMultiplier)
        Return quantity
    End Function
    Public Function CalculateQuantityFromInvestment(ByVal investment As Double, ByVal stockPrice As Double) As Integer
        Return CalculateQuantityFromInvestment(1, investment, stockPrice, TypeOfStock.Cash)
    End Function
End Module
