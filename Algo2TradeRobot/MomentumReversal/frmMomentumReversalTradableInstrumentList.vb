
Public Class frmMomentumReversalTradableInstrumentList

    Private _TradableInstruments As IEnumerable(Of MomentumReversalStrategyInstrument)
    Public Sub New(ByVal associatedTradableInstruments As IEnumerable(Of MomentumReversalStrategyInstrument))
        Me._TradableInstruments = associatedTradableInstruments
        ' This call is required by the designer.
        InitializeComponent()
        ' Add any initialization after the InitializeComponent() call.
    End Sub

    Private Sub frmMomentumReversalTradableInstrumentList_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        If _TradableInstruments IsNot Nothing AndAlso _TradableInstruments.Count > 0 Then
            For Each instrument In _TradableInstruments
                If lstMomentumReversalTradableInstruments Is Nothing Then lstMomentumReversalTradableInstruments = New ListBox
                lstMomentumReversalTradableInstruments.Items.Add(instrument.TradableInstrument.TradingSymbol)
            Next
        End If
    End Sub
End Class