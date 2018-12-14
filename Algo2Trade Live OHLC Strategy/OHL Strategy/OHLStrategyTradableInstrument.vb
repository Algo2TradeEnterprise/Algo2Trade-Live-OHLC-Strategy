Imports Algo2TradeBLL
Imports KiteConnect
Imports System.ComponentModel

Public Class OHLStrategyTradableInstrument
    Implements INotifyPropertyChanged
    Public Sub New(ByVal wrappedInstrument As Instrument)
        Me.WrappedInstrument = wrappedInstrument
    End Sub
    <Browsable(False)>
    Public ReadOnly Property InstrumentToken As String
        Get
            Return WrappedInstrument.InstrumentToken
        End Get
    End Property
    Public ReadOnly Property Exchange As String
        Get
            Return WrappedInstrument.Exchange
        End Get
    End Property
    Public ReadOnly Property TradingSymbol As String
        Get
            Return WrappedInstrument.TradingSymbol
        End Get
    End Property

    Private _LTP As Decimal
    Public ReadOnly Property LTP As Decimal
        Get
            Return _LTP
        End Get
    End Property

    Private _Open As Decimal
    Public ReadOnly Property Open As Decimal
        Get
            Return _Open
        End Get
    End Property

    Private _High As Decimal
    Public ReadOnly Property High As Decimal
        Get
            Return _High
        End Get
    End Property

    Private _Low As Decimal
    Public ReadOnly Property Low As Decimal
        Get
            Return _Low
        End Get
    End Property

    Private _Close As Decimal
    Public ReadOnly Property Close As Decimal
        Get
            Return _Close
        End Get
    End Property

    Private _Change As Decimal
    Public ReadOnly Property Change As Decimal
        Get
            Return _Change
        End Get
    End Property

    Private _PLD As Decimal
    Public ReadOnly Property PLD As Decimal
        Get
            Return _PLD
        End Get
    End Property

    Private _WrappedTick As Tick?
    <Browsable(False)>
    Public Property WrappedTick As Tick?
        Get
            Return _WrappedTick
        End Get
        Set(value As Tick?)
            Me._WrappedTick = value
            If Me._LTP <> Me._WrappedTick.Value.LastPrice Then
                Me._LTP = Me._WrappedTick.Value.LastPrice
                NotifyPropertyChanged("LTP")
            End If
            If _TradeList IsNot Nothing AndAlso _TradeList.Count > 0 Then
                Dim plOfDay As Decimal = 0
                Dim totalQuantity As Integer = 0
                For Each trades In Me._TradeList
                    If trades.TransactionType = "BUY" Then
                        plOfDay += trades.AveragePrice * trades.Quantity * -1
                        totalQuantity += trades.Quantity * -1
                    ElseIf trades.TransactionType = "SELL" Then
                        plOfDay += trades.AveragePrice * trades.Quantity
                        totalQuantity += trades.Quantity
                    End If
                Next
                If totalQuantity = 0 Then
                    Me._PLD = plOfDay
                Else
                    Me._PLD = plOfDay + (totalQuantity * -1 * Me._LTP)
                End If
            Else
                Me._PLD = 0
            End If
            NotifyPropertyChanged("PLD")
            If Me._Change <> Math.Round((WrappedTick.Value.LastPrice / WrappedTick.Value.Close) - 1, 4) Then
                Me._Change = Math.Round((WrappedTick.Value.LastPrice / WrappedTick.Value.Close) - 1, 4)
                NotifyPropertyChanged("Change")
            End If
            If Me._LTP <> Me._WrappedTick.Value.Open Then
                Me._Open = Me._WrappedTick.Value.Open
                NotifyPropertyChanged("Open")
            End If
            If Me._LTP <> Me._WrappedTick.Value.Low Then
                Me._Low = Me._WrappedTick.Value.Low
                NotifyPropertyChanged("Low")
            End If
            If Me._LTP <> Me._WrappedTick.Value.High Then
                Me._High = Me._WrappedTick.Value.High
                NotifyPropertyChanged("High")
            End If
            If Me._LTP <> Me._WrappedTick.Value.Close Then
                Me._Close = Me._WrappedTick.Value.Close
                NotifyPropertyChanged("Close")
            End If
        End Set
    End Property
    <Browsable(False)>
    Public Property StrategyWorker As OHLInstrumentWorker
    <Browsable(False)>
    Public ReadOnly Property WrappedInstrument As Instrument

    Private _TradeList As List(Of Trade)
    <Browsable(False)>
    Public Property TradeList As List(Of Trade)
        Get
            Return _TradeList
        End Get
        Set(value As List(Of Trade))
            Me._TradeList = value
        End Set
    End Property
    <Browsable(False)>
    Public Property OrderList As List(Of Order)
    <Browsable(False)>
    Public Property Positions As PositionResponse
    <Browsable(False)>
    Public Property TargetOfStock As Double = 0
    <Browsable(False)>
    Public Property StoplossOfStock As Double = 0
    <Browsable(False)>
    Public Property InvestmentForStock As Double = 0
    <Browsable(False)>
    Public Property IsTradable As Boolean = False

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged
    Private Sub NotifyPropertyChanged(ByVal p As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(p))
    End Sub
End Class
