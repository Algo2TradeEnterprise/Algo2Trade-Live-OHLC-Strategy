
Imports System.Threading
Imports NLog
Imports Algo2TradeCore.Adapter
Imports Algo2TradeCore.Entity
Imports Algo2TradeCore.Subscriber
Imports Algo2TradeCore.Strategy

Public Class frmMain
#Region "Logging and Status Progress"
    Public Shared logger As Logger = LogManager.GetCurrentClassLogger
#End Region

#Region "Common Delegates"
    Delegate Sub SetListAddItem_Delegate(ByVal [lst] As ListBox, ByVal [value] As Object)
    Public Sub SetListAddItem_ThreadSafe(ByVal [lst] As ListBox, ByVal [value] As Object)
        ' InvokeRequired required compares the thread ID of the calling thread to the thread ID of the creating thread.  
        ' If these threads are different, it returns true.  
        If [lst].InvokeRequired Then
            Dim MyDelegate As New SetListAddItem_Delegate(AddressOf SetListAddItem_ThreadSafe)
            Me.Invoke(MyDelegate, New Object() {lst, [value]})
        Else
            [lst].Items.Insert(0, [value])
        End If
    End Sub
    Delegate Sub SetObjectEnableDisable_Delegate(ByVal [obj] As Object, ByVal [value] As Boolean)
    Public Sub SetObjectEnableDisable_ThreadSafe(ByVal [obj] As Object, ByVal [value] As Boolean)
        ' InvokeRequired required compares the thread ID of the calling thread to the thread ID of the creating thread.  
        ' If these threads are different, it returns true.  
        If [obj].InvokeRequired Then
            Dim MyDelegate As New SetObjectEnableDisable_Delegate(AddressOf SetObjectEnableDisable_ThreadSafe)
            Me.Invoke(MyDelegate, New Object() {[obj], [value]})
        Else
            [obj].Enabled = [value]
        End If
    End Sub

    Delegate Sub SetLabelText_Delegate(ByVal [label] As Label, ByVal [text] As String)
    Public Sub SetLabelText_ThreadSafe(ByVal [label] As Label, ByVal [text] As String)
        ' InvokeRequired required compares the thread ID of the calling thread to the thread ID of the creating thread.  
        ' If these threads are different, it returns true.  
        If [label].InvokeRequired Then
            Dim MyDelegate As New SetLabelText_Delegate(AddressOf SetLabelText_ThreadSafe)
            Me.Invoke(MyDelegate, New Object() {[label], [text]})
        Else
            [label].Text = [text]
        End If
    End Sub
    Delegate Function GetLabelText_Delegate(ByVal [label] As Label) As String

    Public Function GetLabelText_ThreadSafe(ByVal [label] As Label) As String
        ' InvokeRequired required compares the thread ID of the calling thread to the thread ID of the creating thread.  
        ' If these threads are different, it returns true.  
        If [label].InvokeRequired Then
            Dim MyDelegate As New GetLabelText_Delegate(AddressOf GetLabelText_ThreadSafe)
            Return Me.Invoke(MyDelegate, New Object() {[label]})
        Else
            Return [label].Text
        End If
    End Function

    Delegate Sub SetLabelTag_Delegate(ByVal [label] As Label, ByVal [tag] As String)
    Public Sub SetLabelTag_ThreadSafe(ByVal [label] As Label, ByVal [tag] As String)
        ' InvokeRequired required compares the thread ID of the calling thread to the thread ID of the creating thread.  
        ' If these threads are different, it returns true.  
        If [label].InvokeRequired Then
            Dim MyDelegate As New SetLabelTag_Delegate(AddressOf SetLabelTag_ThreadSafe)
            Me.Invoke(MyDelegate, New Object() {[label], [tag]})
        Else
            [label].Tag = [tag]
        End If
    End Sub
    Delegate Function GetLabelTag_Delegate(ByVal [label] As Label) As String

    Public Function GetLabelTag_ThreadSafe(ByVal [label] As Label) As String
        ' InvokeRequired required compares the thread ID of the calling thread to the thread ID of the creating thread.  
        ' If these threads are different, it returns true.  
        If [label].InvokeRequired Then
            Dim MyDelegate As New GetLabelTag_Delegate(AddressOf GetLabelTag_ThreadSafe)
            Return Me.Invoke(MyDelegate, New Object() {[label]})
        Else
            Return [label].Tag
        End If
    End Function
    Delegate Sub SetToolStripLabel_Delegate(ByVal [toolStrip] As StatusStrip, ByVal [label] As ToolStripStatusLabel, ByVal [text] As String)
    Public Sub SetToolStripLabel_ThreadSafe(ByVal [toolStrip] As StatusStrip, ByVal [label] As ToolStripStatusLabel, ByVal [text] As String)
        ' InvokeRequired required compares the thread ID of the calling thread to the thread ID of the creating thread.  
        ' If these threads are different, it returns true.  
        If [toolStrip].InvokeRequired Then
            Dim MyDelegate As New SetToolStripLabel_Delegate(AddressOf SetToolStripLabel_ThreadSafe)
            Me.Invoke(MyDelegate, New Object() {[toolStrip], [label], [text]})
        Else
            [label].Text = [text]
        End If
    End Sub

    Delegate Function GetToolStripLabel_Delegate(ByVal [toolStrip] As StatusStrip, ByVal [label] As ToolStripLabel) As String
    Public Function GetToolStripLabel_ThreadSafe(ByVal [toolStrip] As StatusStrip, ByVal [label] As ToolStripLabel) As String
        ' InvokeRequired required compares the thread ID of the calling thread to the thread ID of the creating thread.  
        ' If these threads are different, it returns true.  
        If [toolStrip].InvokeRequired Then
            Dim MyDelegate As New GetToolStripLabel_Delegate(AddressOf GetToolStripLabel_ThreadSafe)
            Return Me.Invoke(MyDelegate, New Object() {[toolStrip], [label]})
        Else
            Return [label].Text
        End If
    End Function
#End Region

#Region "Event Handlers"
    Public Sub ProgressStatus(ByVal msg As String)
        SyncLock Me
            If Not msg.EndsWith("...") Then msg = String.Format("{0}...", msg)

            SetListAddItem_ThreadSafe(lstLog, msg)
            ' If StringManipulation.GetStringSimilarityPercentage(msg, _lastLoggedMessage) < 90 Then
            logger.Info(msg)
            'End If
            '_lastLoggedMessage = msg
        End SyncLock
    End Sub
    Private Sub OnHeartbeat(msg As String)
        'Update detailed status on the first part, dont append if the text starts with <
        If msg.Contains("<<<") Then
            msg = Replace(msg, "<<<", Nothing)
            ProgressStatus(msg)
        Else
            ProgressStatus(msg)
        End If
        msg = Nothing
    End Sub
    Private Sub OnWaitingFor(elapsedSecs As Integer, totalSecs As Integer, msg As String)
        If msg.Contains("...") Then msg = msg.Replace("...", "")
        ProgressStatus(String.Format("{0}, waiting {1}/{2} secs", msg, elapsedSecs, totalSecs))
    End Sub
    Private Sub OnDocumentRetryStatus(currentTry As Integer, totalTries As Integer)
        ProgressStatus(String.Format("Try #{0}/{1}: Connecting", currentTry, totalTries))
    End Sub
    Private Sub OnDocumentDownloadComplete()
    End Sub
    Private Sub OnErrorOcurred(ByVal msg As String, ByVal ex As Exception)
        MsgBox(ex.Message)
        End
    End Sub
#End Region

#Region "Private Attributes"
    Private _cts As CancellationTokenSource
    Private _lastLoggedMessage As String = Nothing
    Private _adapter As APIAdapter = Nothing
    Private _connection As IConnection = Nothing
    Private _subscriber As APIInstrumentSubscriber = Nothing
#End Region

    Private Sub frmMain_Load(sender As Object, e As EventArgs) Handles Me.Load
        GlobalDiagnosticsContext.Set("appname", My.Application.Info.AssemblyName)
        GlobalDiagnosticsContext.Set("version", My.Application.Info.Version.ToString)
        logger.Trace("*************************** Logging started ***************************")
    End Sub

    Private Async Sub btnStart_Click(sender As Object, e As EventArgs) Handles btnStart.Click
        _cts = New CancellationTokenSource()
        _cts.Token.ThrowIfCancellationRequested()
        Try
            _adapter = New ZerodhaAdapter("DK4056",
                                            "Zerodha@123a",
                                            "3",
                                            "hcwmefsivttbchla",
                                            "t9rd8wut44ija2vp15y87hln28h5oppb",
                                            Nothing,
                                            _cts)
            AddHandler _adapter.Heartbeat, AddressOf OnHeartbeat
            AddHandler _adapter.WaitingFor, AddressOf OnWaitingFor
            AddHandler _adapter.DocumentRetryStatus, AddressOf OnDocumentRetryStatus
            AddHandler _adapter.DocumentDownloadComplete, AddressOf OnDocumentDownloadComplete
            OnHeartbeat("Attempting to get connection to Zerodha server")
            _connection = Await _adapter.LoginAsync().ConfigureAwait(False)
            OnHeartbeat("Getting all instruments for the day")
            Dim allInstruments As IEnumerable(Of IInstrument) = Await _adapter.GetAllInstrumentsAsync().ConfigureAwait(False)
            OnHeartbeat("Getting tradable instruments as per strategy")
            Dim mrStrategyInstruments As List(Of MomentumReversalStrategyInstrument) =
                Await MomentumReversalStrategyInstrument.GetAllTradableInstrumentsAsync(allInstruments,
                                                                                    _adapter,
                                                                                    _cts).ConfigureAwait(False)
            OnHeartbeat("Opening subcriber and connecting to ticker")
            _subscriber = New ZerodhaInstrumentSubscriber(_adapter, _cts)
            For Each runningStrategyInstrument As MomentumReversalStrategyInstrument In mrStrategyInstruments
                _subscriber.SubscribeStrategy(runningStrategyInstrument)
            Next
            Await _adapter.ConnectTickerAsync(_subscriber).ConfigureAwait(False)
        Catch cx As OperationCanceledException
            logger.Error(cx)
            MsgBox(String.Format("The following error occurred: {0}", cx.Message), MsgBoxStyle.Critical)
        Catch ex As Exception
            logger.Error(ex)
            MsgBox(String.Format("The following error occurred: {0}", ex.Message), MsgBoxStyle.Critical)
        Finally
            ProgressStatus("No pending actions")
        End Try
    End Sub

End Class
