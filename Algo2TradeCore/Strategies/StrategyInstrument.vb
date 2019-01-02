Imports System.ComponentModel
Imports System.ComponentModel.DataAnnotations
Imports System.Threading
Imports Algo2TradeCore.Adapter
Imports Algo2TradeCore.Chart
Imports Algo2TradeCore.Entities
Imports NLog

Namespace Strategies
    Public MustInherit Class StrategyInstrument
        Implements INotifyPropertyChanged
        Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged
        Protected Sub NotifyPropertyChanged(ByVal p As String)
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(p))
        End Sub

#Region "Events/Event handlers"
        Public Event DocumentDownloadCompleteEx(ByVal source As List(Of Object))
        Public Event DocumentRetryStatusEx(ByVal currentTry As Integer, ByVal totalTries As Integer, ByVal source As List(Of Object))
        Public Event HeartbeatEx(ByVal msg As String, ByVal source As List(Of Object))
        Public Event WaitingForEx(ByVal elapsedSecs As Integer, ByVal totalSecs As Integer, ByVal msg As String, ByVal source As List(Of Object))
        'The below functions are needed to allow the derived classes to raise the above two events
        Protected Overridable Sub OnDocumentDownloadCompleteEx(ByVal source As List(Of Object))
            If source IsNot Nothing Then source = New List(Of Object)
            source.Add(Me)
            RaiseEvent DocumentDownloadCompleteEx(source)
        End Sub
        Protected Overridable Sub OnDocumentRetryStatusEx(ByVal currentTry As Integer, ByVal totalTries As Integer, ByVal source As List(Of Object))
            If source IsNot Nothing Then source = New List(Of Object)
            source.Add(Me)
            RaiseEvent DocumentRetryStatusEx(currentTry, totalTries, source)
        End Sub
        Protected Overridable Sub OnHeartbeatEx(ByVal msg As String, ByVal source As List(Of Object))
            If source IsNot Nothing Then source = New List(Of Object)
            source.Add(Me)
            If TradableInstrument IsNot Nothing Then
                RaiseEvent HeartbeatEx(String.Format("{0}:{1}", TradableInstrument.InstrumentIdentifier, msg), source)
            Else
                RaiseEvent HeartbeatEx(String.Format("{0}:{1}", "No instrument", msg), source)
            End If
        End Sub
        Protected Overridable Sub OnWaitingForEx(ByVal elapsedSecs As Integer, ByVal totalSecs As Integer, ByVal msg As String, ByVal source As List(Of Object))
            If source IsNot Nothing Then source = New List(Of Object)
            source.Add(Me)
            If TradableInstrument IsNot Nothing Then
                RaiseEvent WaitingForEx(elapsedSecs, totalSecs, String.Format("{0}-{1}", TradableInstrument.InstrumentIdentifier, msg), source)
            Else
                RaiseEvent WaitingForEx(elapsedSecs, totalSecs, String.Format("{0}-{1}", "No instrument", msg), source)
            End If
        End Sub
        Protected Overridable Sub OnDocumentDownloadComplete()
            RaiseEvent DocumentDownloadCompleteEx(New List(Of Object) From {Me})
        End Sub
        Protected Overridable Sub OnDocumentRetryStatus(ByVal currentTry As Integer, ByVal totalTries As Integer)
            RaiseEvent DocumentRetryStatusEx(currentTry, totalTries, New List(Of Object) From {Me})
        End Sub
        Protected Overridable Sub OnHeartbeat(ByVal msg As String)
            If TradableInstrument IsNot Nothing Then
                RaiseEvent HeartbeatEx(String.Format("{0}:{1}", TradableInstrument.InstrumentIdentifier, msg), New List(Of Object) From {Me})
            Else
                RaiseEvent HeartbeatEx(String.Format("{0}:{1}", "No instrument", msg), New List(Of Object) From {Me})
            End If
        End Sub
        Protected Overridable Sub OnWaitingFor(ByVal elapsedSecs As Integer, ByVal totalSecs As Integer, ByVal msg As String)
            If TradableInstrument IsNot Nothing Then
                RaiseEvent WaitingForEx(elapsedSecs, totalSecs, String.Format("{0}-{1}", TradableInstrument.InstrumentIdentifier, msg), New List(Of Object) From {Me})
            Else
                RaiseEvent WaitingForEx(elapsedSecs, totalSecs, String.Format("{0}-{1}", "No instrument", msg), New List(Of Object) From {Me})
            End If
        End Sub
#End Region

#Region "Logging and Status Progress"
        Public Shared logger As Logger = LogManager.GetCurrentClassLogger
#End Region


        <System.ComponentModel.Browsable(False)>
        Public Property ParentStrategy As Strategy
        <System.ComponentModel.Browsable(False)>
        Public Property TradableInstrument As IInstrument
        Public Property RawPayloads As Dictionary(Of DateTime, Payload)
        Public Property RawTicks As Concurrent.ConcurrentDictionary(Of DateTime, ITick)
        Protected _cts As CancellationTokenSource
        Protected _LastTick As ITick
        Protected _APIAdapter As APIAdapter
        Protected _candlestickHelper As Candlestick
        'UI Properties
        <Display(Name:="Symbol", Order:=0)>
        Public Overridable ReadOnly Property TradingSymbol As String
            Get
                If TradableInstrument IsNot Nothing Then
                    Return TradableInstrument.TradingSymbol
                Else
                    Return Nothing
                End If
            End Get
        End Property
        Private _Tradabale As Boolean
        <Display(Name:="Tradable", Order:=1)>
        Public Overridable ReadOnly Property Tradabale As Boolean
            Get
                If _LastTick IsNot Nothing Then
                    _Tradabale = _LastTick.Tradable
                    Return _Tradabale
                Else
                    Return Nothing
                End If
            End Get
        End Property
        Private _OpenPrice As Decimal
        <Display(Name:="Open", Order:=2)>
        Public Overridable ReadOnly Property OpenPrice As Decimal
            Get
                If _LastTick IsNot Nothing Then
                    _OpenPrice = _LastTick.Open
                    Return _OpenPrice
                Else
                    Return Nothing
                End If
            End Get
        End Property
        Private _HighPrice As Decimal
        <Display(Name:="High", Order:=3)>
        Public Overridable ReadOnly Property HighPrice As Decimal
            Get
                If _LastTick IsNot Nothing Then
                    _HighPrice = _LastTick.High
                    Return _HighPrice
                Else
                    Return Nothing
                End If
            End Get
        End Property
        Private _LowPrice As Decimal
        <Display(Name:="Low", Order:=4)>
        Public Overridable ReadOnly Property LowPrice As Decimal
            Get
                If _LastTick IsNot Nothing Then
                    _LowPrice = _LastTick.Low
                    Return _LowPrice
                Else
                    Return Nothing
                End If
            End Get
        End Property
        Private _ClosePrice As Decimal
        <Display(Name:="Previous Close", Order:=5)>
        Public Overridable ReadOnly Property ClosePrice As Decimal
            Get
                If _LastTick IsNot Nothing Then
                    _ClosePrice = _LastTick.Close
                    Return _ClosePrice
                Else
                    Return Nothing
                End If
            End Get
        End Property
        Private _Volume As Long
        <Display(Name:="Volume", Order:=6)>
        Public Overridable ReadOnly Property Volume As Long
            Get
                If _LastTick IsNot Nothing Then
                    _Volume = _LastTick.Volume
                    Return _Volume
                Else
                    Return Nothing
                End If
            End Get
        End Property
        Private _AveragePrice As Decimal
        <Display(Name:="Average Price", Order:=7)>
        Public Overridable ReadOnly Property AveragePrice As Decimal
            Get
                If _LastTick IsNot Nothing Then
                    _AveragePrice = _LastTick.AveragePrice
                    Return _AveragePrice
                Else
                    Return Nothing
                End If
            End Get
        End Property
        Private _LastPrice As Decimal
        <Display(Name:="Last Price", Order:=8)>
        Public Overridable ReadOnly Property LastPrice As Decimal
            Get
                If _LastTick IsNot Nothing Then
                    _LastPrice = _LastTick.LastPrice
                    Return _LastPrice
                Else
                    Return Nothing
                End If
            End Get
        End Property
        Private _Timestamp As Date?
        <Display(Name:="Timestamp", Order:=9)>
        Public Overridable ReadOnly Property Timestamp As Date?
            Get
                If _LastTick IsNot Nothing Then
                    _Timestamp = _LastTick.Timestamp
                    Return _Timestamp
                Else
                    Return Nothing
                End If
            End Get
        End Property


        Public Sub New(ByVal associatedInstrument As IInstrument, ByVal associatedParentStrategy As Strategy, ByVal canceller As CancellationTokenSource)
            TradableInstrument = associatedInstrument
            Me.ParentStrategy = associatedParentStrategy
            _cts = canceller
            RawTicks = New Concurrent.ConcurrentDictionary(Of Date, ITick)
            RawPayloads = New Dictionary(Of Date, Payload)
            _candlestickHelper = New Candlestick(Me, _cts)
            '_candlestickHelper.ConsumeTicks().ConfigureAwait(False)
        End Sub
        Public MustOverride Overrides Function ToString() As String
        Public MustOverride Async Function RunDirectAsync() As Task
        Public MustOverride Async Function IsTriggerReachedAsync() As Task(Of Tuple(Of Boolean, Trigger))
        Public Overridable Async Function ProcessTickAsync(ByVal tickData As ITick) As Task
            Await Task.Delay(0).ConfigureAwait(False)
            If tickData IsNot Nothing AndAlso tickData.LastPrice <> _LastPrice Then NotifyPropertyChanged("LastPrice")
            If tickData IsNot Nothing AndAlso tickData.Tradable <> _Tradabale Then NotifyPropertyChanged("Tradable")
            If tickData IsNot Nothing AndAlso tickData.Open <> _OpenPrice Then NotifyPropertyChanged("OpenPrice")
            If tickData IsNot Nothing AndAlso tickData.High <> _HighPrice Then NotifyPropertyChanged("HighPrice")
            If tickData IsNot Nothing AndAlso tickData.Low <> _LowPrice Then NotifyPropertyChanged("LowPrice")
            If tickData IsNot Nothing AndAlso tickData.Close <> _ClosePrice Then NotifyPropertyChanged("ClosePrice")
            If tickData IsNot Nothing AndAlso tickData.Volume <> _Volume Then NotifyPropertyChanged("Volume")
            If tickData IsNot Nothing AndAlso tickData.AveragePrice <> _AveragePrice Then NotifyPropertyChanged("AveragePrice")
            If tickData IsNot Nothing AndAlso tickData.Timestamp <> _Timestamp Then NotifyPropertyChanged("Timestamp")
            If Not RawTicks.TryAdd(tickData.Timestamp, tickData) Then
                'Console.WriteLine(String.Format("Could not store:{0},{1},{2}", TradingSymbol, tickData.LastPrice, tickData.Timestamp.Value.ToLongTimeString))
            End If
        End Function
    End Class
End Namespace
