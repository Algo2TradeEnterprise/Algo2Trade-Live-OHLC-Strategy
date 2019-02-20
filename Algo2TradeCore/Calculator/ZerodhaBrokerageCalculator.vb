Imports System.Net.Http
Imports System.Threading
Imports Utilities.Network

Namespace Calculator
    Public Class ZerodhaBrokerageCalculator
        Inherits APIBrokerageCalculator

        Private _jsonDictionary As Dictionary(Of String, Object)
        Public Sub New(canceller As CancellationTokenSource)
            MyBase.New(canceller)
        End Sub
        Public Overrides Function GetIntradayEquityBrokerage(buy As Decimal, sell As Decimal, quantity As Integer) As IBrokerageAttributes
            logger.Debug("{0}->GetIntradayEquityBrokerage, parameters:{1},{2},{3}", Me.ToString, buy, sell, quantity)
            Dim ret As New ZerodhaBrokerageAttributes
            Dim m = buy
            Dim g = sell
            Dim v = quantity
            ret.Buy = buy
            ret.Sell = sell
            ret.Quantity = quantity
            ret.Multiplier = 1
            ret.CTT = 0

            Dim o = If((m * v * 0.0001) > 20, 20, Math.Round((m * v * 0.0001), 2))
            Dim i = If((g * v * 0.0001) > 20, 20, Math.Round((g * v * 0.0001), 2))
            Dim n = Math.Round((o + i), 2)
            Dim a = Math.Round(((m + g) * v), 2)
            Dim r = Convert.ToInt32(g * v * 0.00025)
            Dim s = Math.Round((0.0000325 * a), 2)
            Dim l = 0
            Dim c = Math.Round((0.18 * (n + s)), (2))

            ret.Turnover = a
            ret.Brokerage = n
            ret.STT = r
            ret.ExchangeFees = s
            ret.Clearing = l
            ret.GST = c
            ret.TotalTax = ret.Brokerage + ret.STT + ret.ExchangeFees + ret.Clearing + ret.GST + ret.SEBI
            Return ret
        End Function

        Public Overrides Function GetDeliveryEquityBrokerage(buy As Decimal, sell As Decimal, quantity As Integer) As IBrokerageAttributes
            logger.Debug("{0}->GetDeliveryEquityBrokerage, parameters:{1},{2},{3}", Me.ToString, buy, sell, quantity)
            Dim ret As New ZerodhaBrokerageAttributes
            Dim m = buy
            Dim g = sell
            Dim v = quantity
            ret.Buy = buy
            ret.Sell = sell
            ret.Quantity = quantity
            ret.Multiplier = 1
            ret.CTT = 0

            'Dim t = 1.5
            'Dim e = 1.5

            Dim o = Math.Round(((m + g) * v), 2)
            Dim i = 0
            Dim n = Convert.ToInt32(0.001 * o)
            Dim a = Math.Round((0.0000325 * o), 2)
            Dim r = 0
            Dim s = Math.Round((0.18 * (i + a)), (2))

            ret.Turnover = o
            ret.Brokerage = i
            ret.STT = n
            ret.ExchangeFees = a
            ret.Clearing = r
            ret.GST = s
            ret.TotalTax = ret.Brokerage + ret.STT + ret.ExchangeFees + ret.Clearing + ret.GST + ret.SEBI
            Return ret
        End Function

        Public Overrides Function GetIntradayEquityFuturesBrokerage(buy As Decimal, sell As Decimal, quantity As Integer) As IBrokerageAttributes
            logger.Debug("{0}->GetIntradayEquityFuturesBrokerage, parameters:{1},{2},{3}", Me.ToString, buy, sell, quantity)
            Dim ret As New ZerodhaBrokerageAttributes
            Dim m = buy
            Dim g = sell
            Dim v = quantity
            ret.Buy = buy
            ret.Sell = sell
            ret.Quantity = quantity
            ret.Multiplier = 1
            ret.CTT = 0

            'Dim t = 1.5
            'Dim e = 1.5

            Dim o = Math.Round(((m + g) * v), (2))
            Dim i = If((m * v * 0.0001) > 20, 20, Math.Round((m * v * 0.0001), (2)))
            Dim n = If((g * v * 0.0001) > 20, 20, Math.Round((g * v * 0.0001), (2)))
            Dim a = Math.Round((i + n), (2))
            Dim r = Convert.ToInt32(g * v * 0.0001)
            Dim s = Math.Round((0.000021 * o), (2))
            Dim l = Math.Round((0.000019 * o), (2))
            Dim c = Math.Round((0.000002 * o), (2))
            Dim p = Math.Round((0.18 * (a + s)), (2))

            ret.Turnover = o
            ret.Brokerage = a
            ret.STT = r
            ret.ExchangeFees = l
            ret.Clearing = c
            ret.GST = p
            ret.TotalTax = ret.Brokerage + ret.STT + ret.ExchangeFees + ret.Clearing + ret.GST + ret.SEBI
            Return ret
        End Function

        Public Overrides Async Function GetIntradayCommodityFuturesBrokerageAsync(item As String, buy As Decimal, sell As Decimal, quantity As Integer) As Task(Of IBrokerageAttributes)
            logger.Debug("{0}->GetIntradayCommodityFuturesBrokerageAsync, parameters:{1},{2},{3},{4}", Me.ToString, item, buy, sell, quantity)
            Dim ret As New ZerodhaBrokerageAttributes
            Dim m = buy
            Dim g = sell
            Dim v = quantity
            ret.Buy = buy
            ret.Sell = sell
            ret.Quantity = quantity

            If _jsonDictionary Is Nothing OrElse _jsonDictionary.Count = 0 Then
                _jsonDictionary = Await GetJsonForCommodityAsync(_cts).ConfigureAwait(False)
            End If

            Dim t = _jsonDictionary(item).ToString.Substring(0, _jsonDictionary(item).ToString.Length - 1)
            Dim e = _jsonDictionary(item).ToString.Substring(_jsonDictionary(item).ToString.Length - 1)
            ret.Multiplier = t
            Dim o = Math.Round(((m + g) * t * v), 2)
            Dim i = Nothing
            If m * t * v > 200000.0 Then
                i = 20
            Else
                i = If(m * t * v * 0.0001 > 20, 20, Math.Round((m * t * v * 0.0001), 2))
            End If
            Dim n = Nothing
            If g * t * v > 200000.0 Then
                n = 20
            Else
                n = If(g * t * v * 0.0001 > 20, 20, Math.Round((g * t * v * 0.0001), 2))
            End If
            Dim a = i + n
            Dim r = 0.00
            If e = "a" Then
                r = Math.Round((0.0001 * g * v * t), 2)
            End If
            Dim s = 0.00
            Dim l = 0.00
            Dim c = 0.00
            s = If(e = "a", Math.Round((0.000036 * o), 2), Math.Round((0.0000105 * o), 2))
            l = If(e = "a", Math.Round((0.000026 * o), 2), Math.Round((0.0000005 * o), 2))
            c = Math.Round((0.00001 * o), 2)
            If item = "RBDPMOLEIN" And o >= 100000.0 Then
                Dim p = Convert.ToInt32(Math.Round((o / 100000.0), 2))
                s = p
            End If
            If item = "CASTORSEED" Then
                l = Math.Round((0.000005 * o), 2)
                c = Math.Round((0.00001 * o), 2)
            ElseIf item = "RBDPMOLEIN" Then
                l = Math.Round((0.00001 * o), 2)
                c = Math.Round((0.00001 * o), 2)
            ElseIf item = "PEPPER" Then
                l = Math.Round((0.0000005 * o), 2)
                c = Math.Round((0.00001 * o), 2)
            End If
            Dim d = Math.Round((0.18 * (a + s)), 2)

            ret.Turnover = o
            ret.CTT = r
            ret.Brokerage = a
            ret.ExchangeFees = l
            ret.Clearing = c
            ret.GST = d
            ret.TotalTax = a + r + s + d + ret.SEBI
            Return ret
        End Function

        Public Overrides Function GetIntradayEquityOptionsBrokerage(buy As Decimal, sell As Decimal, quantity As Integer) As IBrokerageAttributes
            logger.Debug("{0}->GetIntradayEquityOptionsBrokerage, parameters:{1},{2},{3}", Me.ToString, buy, sell, quantity)
            Throw New NotImplementedException()
        End Function

        Public Overrides Function GetIntradayCurrencyFuturesBrokerage(buy As Decimal, sell As Decimal, quantity As Integer) As IBrokerageAttributes
            logger.Debug("{0}->GetIntradayCurrencyFuturesBrokerage, parameters:{1},{2},{3}", Me.ToString, buy, sell, quantity)
            Throw New NotImplementedException()
        End Function

        Public Overrides Function GetIntradayCurrencyOptionsBrokerage(StrikePrice As Decimal, buyPremium As Decimal, sellPremium As Decimal, quantity As Integer) As IBrokerageAttributes
            logger.Debug("{0}->GetIntradayCurrencyOptionsBrokerage, parameters:{1},{2},{3},{4}", Me.ToString, StrikePrice, buyPremium, sellPremium, quantity)
            Throw New NotImplementedException()
        End Function

#Region "BrowseHTTP"
        Private Async Function GetJsonForCommodityAsync(ByVal canceller As CancellationTokenSource) As Task(Of Dictionary(Of String, Object))
            Dim proxyToBeUsed As HttpProxy = Nothing
            Dim ret As Dictionary(Of String, Object) = Nothing

            Using browser As New HttpBrowser(proxyToBeUsed, Net.DecompressionMethods.GZip, New TimeSpan(0, 1, 0), canceller)
                Dim l As Tuple(Of Uri, Object) = Await browser.NonPOSTRequestAsync("https://zerodha.com/static/app.js",
                                                                                     HttpMethod.Get,
                                                                                     Nothing,
                                                                                     True,
                                                                                     Nothing,
                                                                                     False,
                                                                                     Nothing).ConfigureAwait(False)
                If l Is Nothing OrElse l.Item2 Is Nothing Then
                    Throw New ApplicationException(String.Format("No response in the additional site's historical race results landing page: {0}", "https://zerodha.com/static/app.js"))
                End If
                If l IsNot Nothing AndAlso l.Item2 IsNot Nothing Then
                    Dim jString As String = l.Item2
                    If jString IsNot Nothing Then
                        Dim map As String = Utilities.Strings.GetTextBetween("COMMODITY_MULTIPLIER_MAP=", "},", jString)
                        If map IsNot Nothing Then
                            map = map & "}"
                            ret = Utilities.Strings.JsonDeserialize(map)
                        End If
                    End If
                End If
            End Using
            Return ret
        End Function
#End Region
    End Class
End Namespace
