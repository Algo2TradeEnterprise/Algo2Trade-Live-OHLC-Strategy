﻿Imports System.Threading
Imports NLog
Imports Algo2TradeCore.Entities
Imports Algo2TradeCore.Controller
Imports Algo2TradeCore.Exceptions
Imports System.ComponentModel
Imports Syncfusion.WinForms.DataGrid
Imports Syncfusion.WinForms.DataGrid.Events
Imports Syncfusion.WinForms.Input.Enums
Imports Utilities.Time
Imports System.IO
Imports System.Runtime.Serialization.Formatters.Binary
Imports Algo2TradeCore.Entities.UserSettings
Imports Algo2TradeCore.Strategies

Public Class frmMainTabbed

#Region "Logging and Status Progress"
    Public Shared logger As Logger = LogManager.GetCurrentClassLogger
#End Region

#Region "Common Delegates"

    Delegate Sub SetSFGridDataBind_Delegate(ByVal [grd] As SfDataGrid, ByVal [value] As Object)
    Public Async Sub SetSFGridDataBind_ThreadSafe(ByVal [grd] As Syncfusion.WinForms.DataGrid.SfDataGrid, ByVal [value] As Object)
        ' InvokeRequired required compares the thread ID of the calling thread to the thread ID of the creating thread.  
        ' If these threads are different, it returns true.  
        If [grd].InvokeRequired Then
            Dim MyDelegate As New SetSFGridDataBind_Delegate(AddressOf SetSFGridDataBind_ThreadSafe)
            Me.Invoke(MyDelegate, New Object() {[grd], [value]})
        Else
            While True
                Try
                    [grd].DataSource = [value]
                    Exit While
                Catch sop As System.InvalidOperationException
                    logger.Error(sop)
                End Try
                Await Task.Delay(500, _cts.Token).ConfigureAwait(False)
            End While
        End If
    End Sub

    Delegate Sub BindingListAdd_Delegate(ByVal [src] As BindingList(Of ActivityDashboard), ByVal [value] As ActivityDashboard)
    Public Async Sub BindingListAdd_ThreadSafe(ByVal [src] As BindingList(Of ActivityDashboard), ByVal [value] As ActivityDashboard)
        ' InvokeRequired required compares the thread ID of the calling thread to the thread ID of the creating thread.  
        ' If these threads are different, it returns true.  
        If Me.InvokeRequired Then
            Dim MyDelegate As New BindingListAdd_Delegate(AddressOf BindingListAdd_ThreadSafe)
            Me.Invoke(MyDelegate, New Object() {[src], [value]})
        Else
            '[src].Add([value])
            While True
                Try
                    [src].Insert(0, [value])
                    Exit While
                Catch ex As Exception
                    logger.Error(ex)
                End Try
                Await Task.Delay(500, _cts.Token).ConfigureAwait(False)
            End While
        End If
    End Sub

    Delegate Sub SetSFGridFreezFirstColumn_Delegate(ByVal [grd] As SfDataGrid)
    Public Sub SetSFGridFreezFirstColumn_ThreadSafe(ByVal [grd] As Syncfusion.WinForms.DataGrid.SfDataGrid)
        ' InvokeRequired required compares the thread ID of the calling thread to the thread ID of the creating thread.  
        ' If these threads are different, it returns true.  
        If [grd].InvokeRequired Then
            Dim MyDelegate As New SetSFGridFreezFirstColumn_Delegate(AddressOf SetSFGridFreezFirstColumn_ThreadSafe)
            Me.Invoke(MyDelegate, New Object() {[grd]})
        Else
            [grd].FrozenColumnCount = 1
            'Await Task.Delay(500).ConfigureAwait(False)
        End If
    End Sub

    Delegate Sub SetGridDisplayIndex_Delegate(ByVal [grd] As DataGridView, ByVal [colName] As String, ByVal [value] As Integer)
    Public Sub SetGridDisplayIndex_ThreadSafe(ByVal [grd] As DataGridView, ByVal [colName] As String, ByVal [value] As Integer)
        ' InvokeRequired required compares the thread ID of the calling thread to the thread ID of the creating thread.  
        ' If these threads are different, it returns true.  
        If [grd].InvokeRequired Then
            Dim MyDelegate As New SetGridDisplayIndex_Delegate(AddressOf SetGridDisplayIndex_ThreadSafe)
            Me.Invoke(MyDelegate, New Object() {[grd], [colName], [value]})
        Else
            [grd].Columns([colName]).DisplayIndex = [value]
        End If
    End Sub
    Delegate Function GetGridColumnCount_Delegate(ByVal [grd] As DataGridView) As String
    Public Function GetGridColumnCount_ThreadSafe(ByVal [grd] As DataGridView) As String
        ' InvokeRequired required compares the thread ID of the calling thread to the thread ID of the creating thread.  
        ' If these threads are different, it returns true.  
        If [grd].InvokeRequired Then
            Dim MyDelegate As New GetGridColumnCount_Delegate(AddressOf GetGridColumnCount_ThreadSafe)
            Return Me.Invoke(MyDelegate, New Object() {[grd]})
        Else
            Return [grd].Columns.Count
        End If
    End Function

    Delegate Sub SetGridDataBind_Delegate(ByVal [grd] As DataGridView, ByVal [value] As Object)
    Public Sub SetGridDataBind_ThreadSafe(ByVal [grd] As DataGridView, ByVal [value] As Object)
        ' InvokeRequired required compares the thread ID of the calling thread to the thread ID of the creating thread.  
        ' If these threads are different, it returns true.  
        If [grd].InvokeRequired Then
            Dim MyDelegate As New SetGridDataBind_Delegate(AddressOf SetGridDataBind_ThreadSafe)
            Me.Invoke(MyDelegate, New Object() {[grd], [value]})
        Else
            [grd].DataSource = [value]
        End If
    End Sub

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

    Delegate Function GetObjectText_Delegate(ByVal [Object] As Object) As String
    Public Function GetObjectText_ThreadSafe(ByVal [Object] As Object) As String
        ' InvokeRequired required compares the thread ID of the calling thread to the thread ID of the creating thread.  
        ' If these threads are different, it returns true.  
        If [Object].InvokeRequired Then
            Dim MyDelegate As New GetObjectText_Delegate(AddressOf GetObjectText_ThreadSafe)
            Return Me.Invoke(MyDelegate, New Object() {[Object]})
        Else
            Return [Object].Text
        End If
    End Function

    Delegate Sub SetObjectText_Delegate(ByVal [Object] As Object, ByVal [text] As String)
    Public Sub SetObjectText_ThreadSafe(ByVal [Object] As Object, ByVal [text] As String)
        ' InvokeRequired required compares the thread ID of the calling thread to the thread ID of the creating thread.  
        ' If these threads are different, it returns true.  
        If [Object].InvokeRequired Then
            Dim MyDelegate As New SetObjectText_Delegate(AddressOf SetObjectText_ThreadSafe)
            Me.Invoke(MyDelegate, New Object() {[Object], [text]})
        Else
            [Object].Text = [text]
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

#Region "Standard Event Handlers"
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
        'ProgressStatus(String.Format("Try #{0}/{1}: Connecting", currentTry, totalTries))
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
    Private _commonController As APIStrategyController = Nothing
    Private _connection As IConnection = Nothing
    Private _commonControllerUserInput As ControllerUserInputs = Nothing
    Private _lastException As Exception = Nothing
#End Region

    Private Sub miUserDetails_Click(sender As Object, e As EventArgs) Handles miUserDetails.Click
        Dim newForm As New frmZerodhaUserDetails(_commonControllerUserInput)
        newForm.ShowDialog()
        If File.Exists(ControllerUserInputs.Filename) Then
            _commonControllerUserInput = Utilities.Strings.DeserializeToCollection(Of ControllerUserInputs)(ControllerUserInputs.Filename)
        End If
    End Sub

    Private Sub miAbout_Click(sender As Object, e As EventArgs) Handles miAbout.Click
        Dim newForm As New frmAbout
        newForm.ShowDialog()
    End Sub

    Private Sub miAdvanceOptions_Click(sender As Object, e As EventArgs) Handles miAdvancedOptions.Click
        Dim newForm As New frmAdvancedOptions(_commonControllerUserInput)
        newForm.ShowDialog()
        If File.Exists(ControllerUserInputs.Filename) Then
            _commonControllerUserInput = Utilities.Strings.DeserializeToCollection(Of ControllerUserInputs)(ControllerUserInputs.Filename)
        End If
    End Sub

#Region "Momentum Reversal"
    Private _MRUserInputs As MomentumReversalUserInputs = Nothing
    Private _MRdashboadList As BindingList(Of ActivityDashboard) = Nothing
    Private _MRTradableInstruments As IEnumerable(Of MomentumReversalStrategyInstrument) = Nothing
    Private Sub sfdgvMomentumReversalMainDashboard_FilterPopupShowing(sender As Object, e As FilterPopupShowingEventArgs) Handles sfdgvMomentumReversalMainDashboard.FilterPopupShowing
        ManipulateGridEx(GridMode.TouchupPopupFilter, e, GetType(MomentumReversalStrategy))
    End Sub
    Private Sub sfdgvMomentumReversalMainDashboard_AutoGeneratingColumn(sender As Object, e As AutoGeneratingColumnArgs) Handles sfdgvMomentumReversalMainDashboard.AutoGeneratingColumn
        ManipulateGridEx(GridMode.TouchupAutogeneratingColumn, e, GetType(MomentumReversalStrategy))
    End Sub
    Private Async Function MomentumReversalWorkerAsync() As Task
        If GetObjectText_ThreadSafe(btnMomentumReversalStart) = Common.LOGIN_PENDING Then
            MsgBox("Cannot start as another strategy is loggin in")
            Exit Function
        End If

        If _cts Is Nothing Then _cts = New CancellationTokenSource
        _cts.Token.ThrowIfCancellationRequested()
        _lastException = Nothing

        Try
            EnableDisableUIEx(UIMode.Active, GetType(MomentumReversalStrategy))
            EnableDisableUIEx(UIMode.BlockOther, GetType(MomentumReversalStrategy))

            OnHeartbeat("Validating MR user settings")
            If File.Exists("MomentumReversalSettings.Strategy.a2t") Then
                Dim fs As Stream = New FileStream("MomentumReversalSettings.Strategy.a2t", FileMode.Open)
                Dim bf As BinaryFormatter = New BinaryFormatter()
                _MRUserInputs = CType(bf.Deserialize(fs), MomentumReversalUserInputs)
                fs.Close()
                _MRUserInputs.InstrumentsData = Nothing
                _MRUserInputs.FillInstrumentDetails(_MRUserInputs.InstrumentDetailsFilePath, _cts)
            Else
                Throw New ApplicationException("Settings file not found. Please complete your settings properly.")
            End If

            If Not Common.IsZerodhaUserDetailsPopulated(_commonControllerUserInput) Then Throw New ApplicationException("Cannot proceed without API user details being entered")
            Dim currentUser As ZerodhaUser = Common.GetZerodhaCredentialsFromSettings(_commonControllerUserInput)

            If _commonController IsNot Nothing Then
                _commonController.RefreshCancellationToken(_cts)
            Else
                _commonController = New ZerodhaStrategyController(currentUser, _commonControllerUserInput, _cts)

                RemoveHandler _commonController.Heartbeat, AddressOf OnHeartbeat
                RemoveHandler _commonController.WaitingFor, AddressOf OnWaitingFor
                RemoveHandler _commonController.DocumentRetryStatus, AddressOf OnDocumentRetryStatus
                RemoveHandler _commonController.DocumentDownloadComplete, AddressOf OnDocumentDownloadComplete
                RemoveHandler _commonController.HeartbeatEx, AddressOf OnHeartbeatEx
                RemoveHandler _commonController.WaitingForEx, AddressOf OnWaitingForEx
                RemoveHandler _commonController.DocumentRetryStatusEx, AddressOf OnDocumentRetryStatusEx
                RemoveHandler _commonController.DocumentDownloadCompleteEx, AddressOf OnDocumentDownloadCompleteEx
                RemoveHandler _commonController.TickerClose, AddressOf OnTickerClose
                RemoveHandler _commonController.TickerConnect, AddressOf OnTickerConnect
                RemoveHandler _commonController.TickerError, AddressOf OnTickerError
                RemoveHandler _commonController.TickerErrorWithStatus, AddressOf OnTickerErrorWithStatus
                RemoveHandler _commonController.TickerNoReconnect, AddressOf OnTickerNoReconnect
                RemoveHandler _commonController.FetcherError, AddressOf OnFetcherError
                RemoveHandler _commonController.CollectorError, AddressOf OnCollectorError
                RemoveHandler _commonController.NewItemAdded, AddressOf OnNewItemAdded
                RemoveHandler _commonController.SessionExpiry, AddressOf OnSessionExpiry

                AddHandler _commonController.Heartbeat, AddressOf OnHeartbeat
                AddHandler _commonController.WaitingFor, AddressOf OnWaitingFor
                AddHandler _commonController.DocumentRetryStatus, AddressOf OnDocumentRetryStatus
                AddHandler _commonController.DocumentDownloadComplete, AddressOf OnDocumentDownloadComplete
                AddHandler _commonController.HeartbeatEx, AddressOf OnHeartbeatEx
                AddHandler _commonController.WaitingForEx, AddressOf OnWaitingForEx
                AddHandler _commonController.DocumentRetryStatusEx, AddressOf OnDocumentRetryStatusEx
                AddHandler _commonController.DocumentDownloadCompleteEx, AddressOf OnDocumentDownloadCompleteEx
                AddHandler _commonController.TickerClose, AddressOf OnTickerClose
                AddHandler _commonController.TickerConnect, AddressOf OnTickerConnect
                AddHandler _commonController.TickerError, AddressOf OnTickerError
                AddHandler _commonController.TickerErrorWithStatus, AddressOf OnTickerErrorWithStatus
                AddHandler _commonController.TickerNoReconnect, AddressOf OnTickerNoReconnect
                AddHandler _commonController.TickerReconnect, AddressOf OnTickerReconnect
                AddHandler _commonController.FetcherError, AddressOf OnFetcherError
                AddHandler _commonController.CollectorError, AddressOf OnCollectorError
                AddHandler _commonController.NewItemAdded, AddressOf OnNewItemAdded
                AddHandler _commonController.SessionExpiry, AddressOf OnSessionExpiry

#Region "Login"
                Dim loginMessage As String = Nothing
                While True
                    _cts.Token.ThrowIfCancellationRequested()
                    _connection = Nothing
                    loginMessage = Nothing
                    Try
                        OnHeartbeat("Attempting to get connection to Zerodha API")
                        _cts.Token.ThrowIfCancellationRequested()
                        _connection = Await _commonController.LoginAsync().ConfigureAwait(False)
                        _cts.Token.ThrowIfCancellationRequested()
                    Catch cx As OperationCanceledException
                        loginMessage = cx.Message
                        logger.Error(cx)
                        Exit While
                    Catch ex As Exception
                        loginMessage = ex.Message
                        logger.Error(ex)
                    End Try
                    If _connection Is Nothing Then
                        If loginMessage IsNot Nothing AndAlso (loginMessage.ToUpper.Contains("password".ToUpper) OrElse loginMessage.ToUpper.Contains("api_key".ToUpper) OrElse loginMessage.ToUpper.Contains("username".ToUpper)) Then
                            'No need to retry as its a password failure
                            OnHeartbeat(String.Format("Loging process failed:{0}", loginMessage))
                            Exit While
                        Else
                            OnHeartbeat(String.Format("Loging process failed:{0} | Waiting for 10 seconds before retrying connection", loginMessage))
                            _cts.Token.ThrowIfCancellationRequested()
                            Await Task.Delay(10000, _cts.Token).ConfigureAwait(False)
                            _cts.Token.ThrowIfCancellationRequested()
                        End If
                    Else
                        Exit While
                    End If
                End While
                If _connection Is Nothing Then
                    If loginMessage IsNot Nothing Then
                        Throw New ApplicationException(String.Format("No connection to Zerodha API could be established | Details:{0}", loginMessage))
                    Else
                        Throw New ApplicationException("No connection to Zerodha API could be established")
                    End If
                End If
#End Region

                OnHeartbeat("Completing all pre-automation requirements")
                _cts.Token.ThrowIfCancellationRequested()
                Dim isPreProcessingDone As Boolean = Await _commonController.PrepareToRunStrategyAsync().ConfigureAwait(False)
                _cts.Token.ThrowIfCancellationRequested()

                If Not isPreProcessingDone Then Throw New ApplicationException("PrepareToRunStrategyAsync did not succeed, cannot progress")
            End If 'Common controller
            EnableDisableUIEx(UIMode.ReleaseOther, GetType(MomentumReversalStrategy))

            Dim momentumReversalStrategyToExecute As New MomentumReversalStrategy(_commonController, 0, _MRUserInputs, 5, _cts)
            OnHeartbeatEx(String.Format("Running strategy:{0}", momentumReversalStrategyToExecute.ToString), New List(Of Object) From {momentumReversalStrategyToExecute})

            _cts.Token.ThrowIfCancellationRequested()
            Await _commonController.SubscribeStrategyAsync(momentumReversalStrategyToExecute).ConfigureAwait(False)
            _cts.Token.ThrowIfCancellationRequested()

            _MRTradableInstruments = momentumReversalStrategyToExecute.TradableStrategyInstruments
            SetObjectText_ThreadSafe(linklblMomentumReversalTradableInstrument, String.Format("Tradable Instruments: {0}", _MRTradableInstruments.Count))
            SetObjectEnableDisable_ThreadSafe(linklblMomentumReversalTradableInstrument, True)
            _cts.Token.ThrowIfCancellationRequested()

            _MRdashboadList = New BindingList(Of ActivityDashboard)(momentumReversalStrategyToExecute.SignalManager.ActivityDetails.Values.ToList)
            SetSFGridDataBind_ThreadSafe(sfdgvMomentumReversalMainDashboard, _MRdashboadList)
            SetSFGridFreezFirstColumn_ThreadSafe(sfdgvMomentumReversalMainDashboard)
            _cts.Token.ThrowIfCancellationRequested()

            Await momentumReversalStrategyToExecute.MonitorAsync().ConfigureAwait(False)
        Catch aex As AdapterBusinessException
            logger.Error(aex)
            If aex.ExceptionType = AdapterBusinessException.TypeOfException.PermissionException Then
                _lastException = aex
            Else
                MsgBox(String.Format("The following error occurred: {0}", aex.Message), MsgBoxStyle.Critical)
            End If
        Catch fex As ForceExitException
            logger.Error(fex)
            _lastException = fex
        Catch cx As OperationCanceledException
            logger.Error(cx)
            MsgBox(String.Format("The following error occurred: {0}", cx.Message), MsgBoxStyle.Critical)
        Catch ex As Exception
            logger.Error(ex)
            MsgBox(String.Format("The following error occurred: {0}", ex.Message), MsgBoxStyle.Critical)
        Finally
            ProgressStatus("No pending actions")
            EnableDisableUIEx(UIMode.ReleaseOther, GetType(MomentumReversalStrategy))
            EnableDisableUIEx(UIMode.Idle, GetType(MomentumReversalStrategy))
        End Try
        'If _cts Is Nothing OrElse _cts.IsCancellationRequested Then
        'Following portion need to be done for any kind of exception. Otherwise if we start again without closing the form then
        'it will not new object of controller. So orphan exception will throw exception again and information collector, historical data fetcher
        'and ticker will not work.
        If _commonController IsNot Nothing Then Await _commonController.CloseTickerIfConnectedAsync().ConfigureAwait(False)
        If _commonController IsNot Nothing Then Await _commonController.CloseFetcherIfConnectedAsync(True).ConfigureAwait(False)
        If _commonController IsNot Nothing Then Await _commonController.CloseCollectorIfConnectedAsync(True).ConfigureAwait(False)
        _commonController = Nothing
        _connection = Nothing
        _cts = Nothing
        'End If
    End Function
    Private Async Sub btnMomentumReversalStart_Click(sender As Object, e As EventArgs) Handles btnMomentumReversalStart.Click
        PreviousDayCleanup()
        Await Task.Run(AddressOf MomentumReversalWorkerAsync).ConfigureAwait(False)

        If _lastException IsNot Nothing Then
            If _lastException.GetType.BaseType Is GetType(AdapterBusinessException) AndAlso
                CType(_lastException, AdapterBusinessException).ExceptionType = AdapterBusinessException.TypeOfException.PermissionException Then
                Debug.WriteLine("Restart for permission")
                logger.Debug("Restarting the application again as there is premission issue")
                btnMomentumReversalStart_Click(sender, e)
            ElseIf _lastException.GetType Is GetType(ForceExitException) Then
                Debug.WriteLine("Restart for daily refresh")
                logger.Debug("Restarting the application again for daily refresh")
                btnMomentumReversalStart_Click(sender, e)
            End If
        End If
    End Sub
    Private Sub tmrMomentumReversalTickerStatus_Tick(sender As Object, e As EventArgs) Handles tmrMomentumReversalTickerStatus.Tick
        FlashTickerBulbEx(GetType(MomentumReversalStrategy))
    End Sub
    Private Async Sub btnMomentumReversalStop_Click(sender As Object, e As EventArgs) Handles btnMomentumReversalStop.Click
        If _commonController IsNot Nothing Then Await _commonController.CloseTickerIfConnectedAsync().ConfigureAwait(False)
        If _commonController IsNot Nothing Then Await _commonController.CloseFetcherIfConnectedAsync(True).ConfigureAwait(False)
        If _commonController IsNot Nothing Then Await _commonController.CloseCollectorIfConnectedAsync(True).ConfigureAwait(False)
        _cts.Cancel()
    End Sub
    Private Sub btnMomentumReversalSettings_Click(sender As Object, e As EventArgs) Handles btnMomentumReversalSettings.Click
        Dim newForm As New frmMomentumReversalSettings(_MRUserInputs)
        newForm.ShowDialog()
    End Sub
    Private Sub linklblMomentumReversalTradableInstrument_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles linklblMomentumReversalTradableInstrument.LinkClicked
        Dim newForm As New frmMomentumReversalTradableInstrumentList(_MRTradableInstruments)
        newForm.ShowDialog()
    End Sub
#End Region

#Region "OHL"
    Private _OHLUserInputs As OHLUserInputs = Nothing
    Private _OHLdashboadList As BindingList(Of ActivityDashboard) = Nothing
    Private _OHLTradableInstruments As IEnumerable(Of OHLStrategyInstrument) = Nothing
    Private Sub sfdgvOHLMainDashboard_FilterPopupShowing(sender As Object, e As FilterPopupShowingEventArgs) Handles sfdgvOHLMainDashboard.FilterPopupShowing
        ManipulateGridEx(GridMode.TouchupPopupFilter, e, GetType(OHLStrategy))
    End Sub
    Private Sub sfdgvOHLMainDashboard_AutoGeneratingColumn(sender As Object, e As AutoGeneratingColumnArgs) Handles sfdgvOHLMainDashboard.AutoGeneratingColumn
        ManipulateGridEx(GridMode.TouchupAutogeneratingColumn, e, GetType(OHLStrategy))
    End Sub
    Private Async Function OHLStartWorkerAsync() As Task
        If GetObjectText_ThreadSafe(btnOHLStart) = Common.LOGIN_PENDING Then
            MsgBox("Cannot start as another strategy is loggin in")
            Exit Function
        End If

        If _cts Is Nothing Then _cts = New CancellationTokenSource
        _cts.Token.ThrowIfCancellationRequested()
        _lastException = Nothing

        Try
            EnableDisableUIEx(UIMode.Active, GetType(OHLStrategy))
            EnableDisableUIEx(UIMode.BlockOther, GetType(OHLStrategy))

            OnHeartbeat("Validating OHL user settings")
            If File.Exists("OHLSettings.Strategy.a2t") Then
                Dim fs As Stream = New FileStream("OHLSettings.Strategy.a2t", FileMode.Open)
                Dim bf As BinaryFormatter = New BinaryFormatter()
                _OHLUserInputs = CType(bf.Deserialize(fs), OHLUserInputs)
                fs.Close()
                _OHLUserInputs.InstrumentsData = Nothing
                _OHLUserInputs.FillInstrumentDetails(_OHLUserInputs.InstrumentDetailsFilePath, _cts)
            Else
                Throw New ApplicationException("Settings file not found. Please complete your settings properly.")
            End If

            If Not Common.IsZerodhaUserDetailsPopulated(_commonControllerUserInput) Then Throw New ApplicationException("Cannot proceed without API user details being entered")
            Dim currentUser As ZerodhaUser = Common.GetZerodhaCredentialsFromSettings(_commonControllerUserInput)

            If _commonController IsNot Nothing Then
                _commonController.RefreshCancellationToken(_cts)
            Else
                _commonController = New ZerodhaStrategyController(currentUser, _commonControllerUserInput, _cts)

                RemoveHandler _commonController.Heartbeat, AddressOf OnHeartbeat
                RemoveHandler _commonController.WaitingFor, AddressOf OnWaitingFor
                RemoveHandler _commonController.DocumentRetryStatus, AddressOf OnDocumentRetryStatus
                RemoveHandler _commonController.DocumentDownloadComplete, AddressOf OnDocumentDownloadComplete
                RemoveHandler _commonController.HeartbeatEx, AddressOf OnHeartbeatEx
                RemoveHandler _commonController.WaitingForEx, AddressOf OnWaitingForEx
                RemoveHandler _commonController.DocumentRetryStatusEx, AddressOf OnDocumentRetryStatusEx
                RemoveHandler _commonController.DocumentDownloadCompleteEx, AddressOf OnDocumentDownloadCompleteEx
                RemoveHandler _commonController.TickerClose, AddressOf OnTickerClose
                RemoveHandler _commonController.TickerConnect, AddressOf OnTickerConnect
                RemoveHandler _commonController.TickerError, AddressOf OnTickerError
                RemoveHandler _commonController.TickerErrorWithStatus, AddressOf OnTickerErrorWithStatus
                RemoveHandler _commonController.TickerNoReconnect, AddressOf OnTickerNoReconnect
                RemoveHandler _commonController.FetcherError, AddressOf OnFetcherError
                RemoveHandler _commonController.CollectorError, AddressOf OnCollectorError
                RemoveHandler _commonController.NewItemAdded, AddressOf OnNewItemAdded
                RemoveHandler _commonController.SessionExpiry, AddressOf OnSessionExpiry

                AddHandler _commonController.Heartbeat, AddressOf OnHeartbeat
                AddHandler _commonController.WaitingFor, AddressOf OnWaitingFor
                AddHandler _commonController.DocumentRetryStatus, AddressOf OnDocumentRetryStatus
                AddHandler _commonController.DocumentDownloadComplete, AddressOf OnDocumentDownloadComplete
                AddHandler _commonController.HeartbeatEx, AddressOf OnHeartbeatEx
                AddHandler _commonController.WaitingForEx, AddressOf OnWaitingForEx
                AddHandler _commonController.DocumentRetryStatusEx, AddressOf OnDocumentRetryStatusEx
                AddHandler _commonController.DocumentDownloadCompleteEx, AddressOf OnDocumentDownloadCompleteEx
                AddHandler _commonController.TickerClose, AddressOf OnTickerClose
                AddHandler _commonController.TickerConnect, AddressOf OnTickerConnect
                AddHandler _commonController.TickerError, AddressOf OnTickerError
                AddHandler _commonController.TickerErrorWithStatus, AddressOf OnTickerErrorWithStatus
                AddHandler _commonController.TickerNoReconnect, AddressOf OnTickerNoReconnect
                AddHandler _commonController.TickerReconnect, AddressOf OnTickerReconnect
                AddHandler _commonController.FetcherError, AddressOf OnFetcherError
                AddHandler _commonController.CollectorError, AddressOf OnCollectorError
                AddHandler _commonController.NewItemAdded, AddressOf OnNewItemAdded
                AddHandler _commonController.SessionExpiry, AddressOf OnSessionExpiry

#Region "Login"
                Dim loginMessage As String = Nothing
                While True
                    _cts.Token.ThrowIfCancellationRequested()
                    _connection = Nothing
                    loginMessage = Nothing
                    Try
                        OnHeartbeat("Attempting to get connection to Zerodha API")
                        _cts.Token.ThrowIfCancellationRequested()
                        _connection = Await _commonController.LoginAsync().ConfigureAwait(False)
                        _cts.Token.ThrowIfCancellationRequested()
                    Catch cx As OperationCanceledException
                        loginMessage = cx.Message
                        logger.Error(cx)
                        Exit While
                    Catch ex As Exception
                        loginMessage = ex.Message
                        logger.Error(ex)
                    End Try
                    If _connection Is Nothing Then
                        If loginMessage IsNot Nothing AndAlso (loginMessage.ToUpper.Contains("password".ToUpper) OrElse loginMessage.ToUpper.Contains("api_key".ToUpper) OrElse loginMessage.ToUpper.Contains("username".ToUpper)) Then
                            'No need to retry as its a password failure
                            OnHeartbeat(String.Format("Loging process failed:{0}", loginMessage))
                            Exit While
                        Else
                            OnHeartbeat(String.Format("Loging process failed:{0} | Waiting for 10 seconds before retrying connection", loginMessage))
                            _cts.Token.ThrowIfCancellationRequested()
                            Await Task.Delay(10000, _cts.Token).ConfigureAwait(False)
                            _cts.Token.ThrowIfCancellationRequested()
                        End If
                    Else
                        Exit While
                    End If
                End While
                If _connection Is Nothing Then
                    If loginMessage IsNot Nothing Then
                        Throw New ApplicationException(String.Format("No connection to Zerodha API could be established | Details:{0}", loginMessage))
                    Else
                        Throw New ApplicationException("No connection to Zerodha API could be established")
                    End If
                End If
#End Region
                OnHeartbeat("Completing all pre-automation requirements")
                _cts.Token.ThrowIfCancellationRequested()
                Dim isPreProcessingDone As Boolean = Await _commonController.PrepareToRunStrategyAsync().ConfigureAwait(False)
                _cts.Token.ThrowIfCancellationRequested()

                If Not isPreProcessingDone Then Throw New ApplicationException("PrepareToRunStrategyAsync did not succeed, cannot progress")
            End If 'Common controller
            EnableDisableUIEx(UIMode.ReleaseOther, GetType(OHLStrategy))

            Dim ohlStrategyToExecute As New OHLStrategy(_commonController, 1, _OHLUserInputs, 0, _cts)
            OnHeartbeatEx(String.Format("Running strategy:{0}", ohlStrategyToExecute.ToString), New List(Of Object) From {ohlStrategyToExecute})

            _cts.Token.ThrowIfCancellationRequested()
            Await _commonController.SubscribeStrategyAsync(ohlStrategyToExecute).ConfigureAwait(False)
            _cts.Token.ThrowIfCancellationRequested()

            _OHLTradableInstruments = ohlStrategyToExecute.TradableStrategyInstruments
            SetObjectText_ThreadSafe(linklblOHLTradableInstruments, String.Format("Tradable Instruments: {0}", _OHLTradableInstruments.Count))
            SetObjectEnableDisable_ThreadSafe(linklblOHLTradableInstruments, True)
            _cts.Token.ThrowIfCancellationRequested()

            _OHLdashboadList = New BindingList(Of ActivityDashboard)(ohlStrategyToExecute.SignalManager.ActivityDetails.Values.ToList)
            SetSFGridDataBind_ThreadSafe(sfdgvOHLMainDashboard, _OHLdashboadList)
            SetSFGridFreezFirstColumn_ThreadSafe(sfdgvOHLMainDashboard)
            _cts.Token.ThrowIfCancellationRequested()

            Await ohlStrategyToExecute.MonitorAsync().ConfigureAwait(False)
        Catch aex As AdapterBusinessException
            logger.Error(aex)
            If aex.ExceptionType = AdapterBusinessException.TypeOfException.PermissionException Then
                _lastException = aex
            Else
                MsgBox(String.Format("The following error occurred: {0}", aex.Message), MsgBoxStyle.Critical)
            End If
        Catch fex As ForceExitException
            logger.Error(fex)
            _lastException = fex
        Catch cx As OperationCanceledException
            logger.Error(cx)
            MsgBox(String.Format("The following error occurred: {0}", cx.Message), MsgBoxStyle.Critical)
        Catch ex As Exception
            logger.Error(ex)
            MsgBox(String.Format("The following error occurred: {0}", ex.Message), MsgBoxStyle.Critical)
        Finally
            ProgressStatus("No pending actions")
            EnableDisableUIEx(UIMode.ReleaseOther, GetType(OHLStrategy))
            EnableDisableUIEx(UIMode.Idle, GetType(OHLStrategy))
        End Try
        'If _cts Is Nothing OrElse _cts.IsCancellationRequested Then
        'Following portion need to be done for any kind of exception. Otherwise if we start again without closing the form then
        'it will not new object of controller. So orphan exception will throw exception again and information collector, historical data fetcher
        'and ticker will not work.
        If _commonController IsNot Nothing Then Await _commonController.CloseTickerIfConnectedAsync().ConfigureAwait(False)
        If _commonController IsNot Nothing Then Await _commonController.CloseFetcherIfConnectedAsync(True).ConfigureAwait(False)
        If _commonController IsNot Nothing Then Await _commonController.CloseCollectorIfConnectedAsync(True).ConfigureAwait(False)
        _commonController = Nothing
        _connection = Nothing
        _cts = Nothing
        'End If
    End Function
    Private Async Sub btnOHLStart_Click(sender As Object, e As EventArgs) Handles btnOHLStart.Click
        PreviousDayCleanup()
        Await Task.Run(AddressOf OHLStartWorkerAsync).ConfigureAwait(False)

        If _lastException IsNot Nothing Then
            If _lastException.GetType.BaseType Is GetType(AdapterBusinessException) AndAlso
                CType(_lastException, AdapterBusinessException).ExceptionType = AdapterBusinessException.TypeOfException.PermissionException Then
                Debug.WriteLine("Restart for permission")
                logger.Debug("Restarting the application again as there is premission issue")
                btnOHLStart_Click(sender, e)
            ElseIf _lastException.GetType Is GetType(ForceExitException) Then
                Debug.WriteLine("Restart for daily refresh")
                logger.Debug("Restarting the application again for daily refresh")
                btnOHLStart_Click(sender, e)
            End If
        End If
    End Sub
    Private Sub tmrOHLTickerStatus_Tick(sender As Object, e As EventArgs) Handles tmrOHLTickerStatus.Tick
        FlashTickerBulbEx(GetType(OHLStrategy))
    End Sub
    Private Async Sub btnOHLStop_Click(sender As Object, e As EventArgs) Handles btnOHLStop.Click
        If _commonController IsNot Nothing Then Await _commonController.CloseTickerIfConnectedAsync().ConfigureAwait(False)
        If _commonController IsNot Nothing Then Await _commonController.CloseFetcherIfConnectedAsync(True).ConfigureAwait(False)
        If _commonController IsNot Nothing Then Await _commonController.CloseCollectorIfConnectedAsync(True).ConfigureAwait(False)
        _cts.Cancel()
    End Sub
    Private Sub btnOHLSettings_Click(sender As Object, e As EventArgs) Handles btnOHLSettings.Click
        Dim newForm As New frmOHLSettings(_OHLUserInputs)
        newForm.ShowDialog()
    End Sub
    Private Sub linklblOHLTradableInstruments_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles linklblOHLTradableInstruments.LinkClicked
        Dim newForm As New frmOHLTradableInstrumentList(_OHLTradableInstruments)
        newForm.ShowDialog()
    End Sub
#End Region

#Region "AmiSignal"
    Private _AmidashboadList As BindingList(Of ActivityDashboard) = Nothing
    Private Sub sfdgvAmiSignalMainDashboard_FilterPopupShowing(sender As Object, e As FilterPopupShowingEventArgs) Handles sfdgvAmiSignalMainDashboard.FilterPopupShowing
        ManipulateGridEx(GridMode.TouchupPopupFilter, e, GetType(AmiSignalStrategy))
    End Sub
    Private Sub sfdgvAmiSignalMainDashboard_AutoGeneratingColumn(sender As Object, e As AutoGeneratingColumnArgs) Handles sfdgvAmiSignalMainDashboard.AutoGeneratingColumn
        ManipulateGridEx(GridMode.TouchupAutogeneratingColumn, e, GetType(AmiSignalStrategy))
    End Sub
    Private Async Function AmiSignalStartWorkerAsync() As Task
        If GetObjectText_ThreadSafe(btnAmiSignalStart) = Common.LOGIN_PENDING Then
            MsgBox("Cannot start as another strategy is loggin in")
            Exit Function
        End If

        If _cts Is Nothing Then _cts = New CancellationTokenSource
        _cts.Token.ThrowIfCancellationRequested()

        Try
            OnHeartbeat("Validating Settings & instrument details")
            Dim amiSignalSettings As New AmiSignalUserInputs
            'amiSignalSettings.SignalTimeFrame = 1
            amiSignalSettings.FillSettingsDetails(IO.Path.Combine(My.Application.Info.DirectoryPath, "AmiIntegrationInputFilev1.0.csv"), _cts)

            EnableDisableUIEx(UIMode.Active, GetType(AmiSignalStrategy))
            EnableDisableUIEx(UIMode.BlockOther, GetType(AmiSignalStrategy))

            If Not Common.IsZerodhaUserDetailsPopulated(_commonControllerUserInput) Then Throw New ApplicationException("Cannot proceed without API user details being entered")
            Dim currentUser As ZerodhaUser = Common.GetZerodhaCredentialsFromSettings(_commonControllerUserInput)

            If _commonController IsNot Nothing Then
                _commonController.RefreshCancellationToken(_cts)
            Else
                _commonController = New ZerodhaStrategyController(currentUser, _commonControllerUserInput, _cts)

                RemoveHandler _commonController.Heartbeat, AddressOf OnHeartbeat
                RemoveHandler _commonController.WaitingFor, AddressOf OnWaitingFor
                RemoveHandler _commonController.DocumentRetryStatus, AddressOf OnDocumentRetryStatus
                RemoveHandler _commonController.DocumentDownloadComplete, AddressOf OnDocumentDownloadComplete
                RemoveHandler _commonController.HeartbeatEx, AddressOf OnHeartbeatEx
                RemoveHandler _commonController.WaitingForEx, AddressOf OnWaitingForEx
                RemoveHandler _commonController.DocumentRetryStatusEx, AddressOf OnDocumentRetryStatusEx
                RemoveHandler _commonController.DocumentDownloadCompleteEx, AddressOf OnDocumentDownloadCompleteEx
                RemoveHandler _commonController.TickerClose, AddressOf OnTickerClose
                RemoveHandler _commonController.TickerConnect, AddressOf OnTickerConnect
                RemoveHandler _commonController.TickerError, AddressOf OnTickerError
                RemoveHandler _commonController.TickerErrorWithStatus, AddressOf OnTickerErrorWithStatus
                RemoveHandler _commonController.TickerNoReconnect, AddressOf OnTickerNoReconnect
                RemoveHandler _commonController.FetcherError, AddressOf OnFetcherError
                RemoveHandler _commonController.CollectorError, AddressOf OnCollectorError
                RemoveHandler _commonController.NewItemAdded, AddressOf OnNewItemAdded
                RemoveHandler _commonController.SessionExpiry, AddressOf OnSessionExpiry

                AddHandler _commonController.Heartbeat, AddressOf OnHeartbeat
                AddHandler _commonController.WaitingFor, AddressOf OnWaitingFor
                AddHandler _commonController.DocumentRetryStatus, AddressOf OnDocumentRetryStatus
                AddHandler _commonController.DocumentDownloadComplete, AddressOf OnDocumentDownloadComplete
                AddHandler _commonController.HeartbeatEx, AddressOf OnHeartbeatEx
                AddHandler _commonController.WaitingForEx, AddressOf OnWaitingForEx
                AddHandler _commonController.DocumentRetryStatusEx, AddressOf OnDocumentRetryStatusEx
                AddHandler _commonController.DocumentDownloadCompleteEx, AddressOf OnDocumentDownloadCompleteEx
                AddHandler _commonController.TickerClose, AddressOf OnTickerClose
                AddHandler _commonController.TickerConnect, AddressOf OnTickerConnect
                AddHandler _commonController.TickerError, AddressOf OnTickerError
                AddHandler _commonController.TickerErrorWithStatus, AddressOf OnTickerErrorWithStatus
                AddHandler _commonController.TickerNoReconnect, AddressOf OnTickerNoReconnect
                AddHandler _commonController.TickerReconnect, AddressOf OnTickerReconnect
                AddHandler _commonController.FetcherError, AddressOf OnFetcherError
                AddHandler _commonController.CollectorError, AddressOf OnCollectorError
                AddHandler _commonController.NewItemAdded, AddressOf OnNewItemAdded
                AddHandler _commonController.SessionExpiry, AddressOf OnSessionExpiry

#Region "Login"
                Dim loginMessage As String = Nothing
                While True
                    _cts.Token.ThrowIfCancellationRequested()
                    _connection = Nothing
                    loginMessage = Nothing
                    Try
                        OnHeartbeat("Attempting to get connection to Zerodha API")
                        _cts.Token.ThrowIfCancellationRequested()
                        _connection = Await _commonController.LoginAsync().ConfigureAwait(False)
                        _cts.Token.ThrowIfCancellationRequested()
                    Catch cx As OperationCanceledException
                        loginMessage = cx.Message
                        logger.Error(cx)
                        Exit While
                    Catch ex As Exception
                        loginMessage = ex.Message
                        logger.Error(ex)
                    End Try
                    If _connection Is Nothing Then
                        If loginMessage IsNot Nothing AndAlso (loginMessage.ToUpper.Contains("password".ToUpper) OrElse loginMessage.ToUpper.Contains("api_key".ToUpper) OrElse loginMessage.ToUpper.Contains("username".ToUpper)) Then
                            'No need to retry as its a password failure
                            OnHeartbeat(String.Format("Loging process failed:{0}", loginMessage))
                            Exit While
                        Else
                            OnHeartbeat(String.Format("Loging process failed:{0} | Waiting for 10 seconds before retrying connection", loginMessage))
                            _cts.Token.ThrowIfCancellationRequested()
                            Await Task.Delay(10000, _cts.Token).ConfigureAwait(False)
                            _cts.Token.ThrowIfCancellationRequested()
                        End If
                    Else
                        Exit While
                    End If
                End While
                If _connection Is Nothing Then
                    If loginMessage IsNot Nothing Then
                        Throw New ApplicationException(String.Format("No connection to Zerodha API could be established | Details:{0}", loginMessage))
                    Else
                        Throw New ApplicationException("No connection to Zerodha API could be established")
                    End If
                End If
#End Region
                OnHeartbeat("Completing all pre-automation requirements")
                _cts.Token.ThrowIfCancellationRequested()
                Dim isPreProcessingDone As Boolean = Await _commonController.PrepareToRunStrategyAsync().ConfigureAwait(False)
                _cts.Token.ThrowIfCancellationRequested()

                If Not isPreProcessingDone Then Throw New ApplicationException("PrepareToRunStrategyAsync did not succeed, cannot progress")
            End If 'Common controller
            EnableDisableUIEx(UIMode.ReleaseOther, GetType(AmiSignalStrategy))

            Dim AmiSignalStrategyToExecute As New AmiSignalStrategy(_commonController, 2, amiSignalSettings, 0, _cts)
            OnHeartbeatEx(String.Format("Running strategy:{0}", AmiSignalStrategyToExecute.ToString), New List(Of Object) From {AmiSignalStrategyToExecute})

            _cts.Token.ThrowIfCancellationRequested()
            Await _commonController.SubscribeStrategyAsync(AmiSignalStrategyToExecute).ConfigureAwait(False)
            _cts.Token.ThrowIfCancellationRequested()

            _AmidashboadList = New BindingList(Of ActivityDashboard)(AmiSignalStrategyToExecute.SignalManager.ActivityDetails.Values.ToList)
            SetSFGridDataBind_ThreadSafe(sfdgvAmiSignalMainDashboard, _AmidashboadList)
            SetSFGridFreezFirstColumn_ThreadSafe(sfdgvAmiSignalMainDashboard)

            Await AmiSignalStrategyToExecute.MonitorAsync().ConfigureAwait(False)
        Catch cx As OperationCanceledException
            logger.Error(cx)
            MsgBox(String.Format("The following error occurred: {0}", cx.Message), MsgBoxStyle.Critical)
        Catch ex As Exception
            logger.Error(ex)
            MsgBox(String.Format("The following error occurred: {0}", ex.Message), MsgBoxStyle.Critical)
        Finally
            ProgressStatus("No pending actions")
            EnableDisableUIEx(UIMode.ReleaseOther, GetType(AmiSignalStrategy))
            EnableDisableUIEx(UIMode.Idle, GetType(AmiSignalStrategy))
        End Try
        If _cts Is Nothing OrElse _cts.IsCancellationRequested Then
            If _commonController IsNot Nothing Then Await _commonController.CloseTickerIfConnectedAsync().ConfigureAwait(False)
            If _commonController IsNot Nothing Then Await _commonController.CloseFetcherIfConnectedAsync(True).ConfigureAwait(False)
            If _commonController IsNot Nothing Then Await _commonController.CloseCollectorIfConnectedAsync(True).ConfigureAwait(False)
            _commonController = Nothing
            _connection = Nothing
            _cts = Nothing
        End If
    End Function
    Private Async Sub btnAmiSignalStart_Click(sender As Object, e As EventArgs) Handles btnAmiSignalStart.Click
        PreviousDayCleanup()
        Await Task.Run(AddressOf AmiSignalStartWorkerAsync).ConfigureAwait(False)
    End Sub
    Private Sub tmrAmiSignalTickerStatus_Tick(sender As Object, e As EventArgs) Handles tmrAmiSignalTickerStatus.Tick
        FlashTickerBulbEx(GetType(AmiSignalStrategy))
    End Sub
    Private Async Sub btnAmiSignalStop_Click(sender As Object, e As EventArgs) Handles btnAmiSignalStop.Click
        If _commonController IsNot Nothing Then Await _commonController.CloseTickerIfConnectedAsync().ConfigureAwait(False)
        If _commonController IsNot Nothing Then Await _commonController.CloseFetcherIfConnectedAsync(True).ConfigureAwait(False)
        If _commonController IsNot Nothing Then Await _commonController.CloseCollectorIfConnectedAsync(True).ConfigureAwait(False)
        _cts.Cancel()
    End Sub
#End Region

#Region "EMA & Supertrend Strategy"
    Private _EMA_SupertrendUserInputs As EMA_SupertrendStrategyUserInputs = Nothing
    Private _EMA_SupertrendDashboadList As BindingList(Of ActivityDashboard) = Nothing
    Private _EMA_SupertrendTradableInstruments As IEnumerable(Of EMA_SupertrendStrategyInstrument) = Nothing
    Private Sub sfdgvEMA_SupertrendMainDashboard_FilterPopupShowing(sender As Object, e As FilterPopupShowingEventArgs) Handles sfdgvEMA_SupertrendMainDashboard.FilterPopupShowing
        ManipulateGridEx(GridMode.TouchupPopupFilter, e, GetType(EMA_SupertrendStrategy))
    End Sub
    Private Sub sfdgvEMA_SupertrendMainDashboard_AutoGeneratingColumn(sender As Object, e As AutoGeneratingColumnArgs) Handles sfdgvEMA_SupertrendMainDashboard.AutoGeneratingColumn
        ManipulateGridEx(GridMode.TouchupAutogeneratingColumn, e, GetType(EMA_SupertrendStrategy))
    End Sub
    Private Async Function EMA_SupertrendWorkerAsync() As Task
        If GetObjectText_ThreadSafe(btnEMA_SupertrendStart) = Common.LOGIN_PENDING Then
            MsgBox("Cannot start as another strategy is loggin in")
            Exit Function
        End If

        If _cts Is Nothing Then _cts = New CancellationTokenSource
        _cts.Token.ThrowIfCancellationRequested()
        _lastException = Nothing

        Try
            EnableDisableUIEx(UIMode.Active, GetType(EMA_SupertrendStrategy))
            EnableDisableUIEx(UIMode.BlockOther, GetType(EMA_SupertrendStrategy))

            OnHeartbeat("Validating Strategy user settings")
            If File.Exists("EMA_SupertrendSettings.Strategy.a2t") Then
                Dim fs As Stream = New FileStream("EMA_SupertrendSettings.Strategy.a2t", FileMode.Open)
                Dim bf As BinaryFormatter = New BinaryFormatter()
                _EMA_SupertrendUserInputs = CType(bf.Deserialize(fs), EMA_SupertrendStrategyUserInputs)
                fs.Close()
                _EMA_SupertrendUserInputs.InstrumentsData = Nothing
                _EMA_SupertrendUserInputs.FillInstrumentDetails(_EMA_SupertrendUserInputs.InstrumentDetailsFilePath, _cts)
            Else
                Throw New ApplicationException("Settings file not found. Please complete your settings properly.")
            End If

            If Not Common.IsZerodhaUserDetailsPopulated(_commonControllerUserInput) Then Throw New ApplicationException("Cannot proceed without API user details being entered")
            Dim currentUser As ZerodhaUser = Common.GetZerodhaCredentialsFromSettings(_commonControllerUserInput)

            If _commonController IsNot Nothing Then
                _commonController.RefreshCancellationToken(_cts)
            Else
                _commonController = New ZerodhaStrategyController(currentUser, _commonControllerUserInput, _cts)

                RemoveHandler _commonController.Heartbeat, AddressOf OnHeartbeat
                RemoveHandler _commonController.WaitingFor, AddressOf OnWaitingFor
                RemoveHandler _commonController.DocumentRetryStatus, AddressOf OnDocumentRetryStatus
                RemoveHandler _commonController.DocumentDownloadComplete, AddressOf OnDocumentDownloadComplete
                RemoveHandler _commonController.HeartbeatEx, AddressOf OnHeartbeatEx
                RemoveHandler _commonController.WaitingForEx, AddressOf OnWaitingForEx
                RemoveHandler _commonController.DocumentRetryStatusEx, AddressOf OnDocumentRetryStatusEx
                RemoveHandler _commonController.DocumentDownloadCompleteEx, AddressOf OnDocumentDownloadCompleteEx
                RemoveHandler _commonController.TickerClose, AddressOf OnTickerClose
                RemoveHandler _commonController.TickerConnect, AddressOf OnTickerConnect
                RemoveHandler _commonController.TickerError, AddressOf OnTickerError
                RemoveHandler _commonController.TickerErrorWithStatus, AddressOf OnTickerErrorWithStatus
                RemoveHandler _commonController.TickerNoReconnect, AddressOf OnTickerNoReconnect
                RemoveHandler _commonController.FetcherError, AddressOf OnFetcherError
                RemoveHandler _commonController.CollectorError, AddressOf OnCollectorError
                RemoveHandler _commonController.NewItemAdded, AddressOf OnNewItemAdded
                RemoveHandler _commonController.SessionExpiry, AddressOf OnSessionExpiry

                AddHandler _commonController.Heartbeat, AddressOf OnHeartbeat
                AddHandler _commonController.WaitingFor, AddressOf OnWaitingFor
                AddHandler _commonController.DocumentRetryStatus, AddressOf OnDocumentRetryStatus
                AddHandler _commonController.DocumentDownloadComplete, AddressOf OnDocumentDownloadComplete
                AddHandler _commonController.HeartbeatEx, AddressOf OnHeartbeatEx
                AddHandler _commonController.WaitingForEx, AddressOf OnWaitingForEx
                AddHandler _commonController.DocumentRetryStatusEx, AddressOf OnDocumentRetryStatusEx
                AddHandler _commonController.DocumentDownloadCompleteEx, AddressOf OnDocumentDownloadCompleteEx
                AddHandler _commonController.TickerClose, AddressOf OnTickerClose
                AddHandler _commonController.TickerConnect, AddressOf OnTickerConnect
                AddHandler _commonController.TickerError, AddressOf OnTickerError
                AddHandler _commonController.TickerErrorWithStatus, AddressOf OnTickerErrorWithStatus
                AddHandler _commonController.TickerNoReconnect, AddressOf OnTickerNoReconnect
                AddHandler _commonController.TickerReconnect, AddressOf OnTickerReconnect
                AddHandler _commonController.FetcherError, AddressOf OnFetcherError
                AddHandler _commonController.CollectorError, AddressOf OnCollectorError
                AddHandler _commonController.NewItemAdded, AddressOf OnNewItemAdded
                AddHandler _commonController.SessionExpiry, AddressOf OnSessionExpiry

#Region "Login"
                Dim loginMessage As String = Nothing
                While True
                    _cts.Token.ThrowIfCancellationRequested()
                    _connection = Nothing
                    loginMessage = Nothing
                    Try
                        OnHeartbeat("Attempting to get connection to Zerodha API")
                        _cts.Token.ThrowIfCancellationRequested()
                        _connection = Await _commonController.LoginAsync().ConfigureAwait(False)
                        _cts.Token.ThrowIfCancellationRequested()
                    Catch cx As OperationCanceledException
                        loginMessage = cx.Message
                        logger.Error(cx)
                        Exit While
                    Catch ex As Exception
                        loginMessage = ex.Message
                        logger.Error(ex)
                    End Try
                    If _connection Is Nothing Then
                        If loginMessage IsNot Nothing AndAlso (loginMessage.ToUpper.Contains("password".ToUpper) OrElse loginMessage.ToUpper.Contains("api_key".ToUpper) OrElse loginMessage.ToUpper.Contains("username".ToUpper)) Then
                            'No need to retry as its a password failure
                            OnHeartbeat(String.Format("Loging process failed:{0}", loginMessage))
                            Exit While
                        Else
                            OnHeartbeat(String.Format("Loging process failed:{0} | Waiting for 10 seconds before retrying connection", loginMessage))
                            _cts.Token.ThrowIfCancellationRequested()
                            Await Task.Delay(10000, _cts.Token).ConfigureAwait(False)
                            _cts.Token.ThrowIfCancellationRequested()
                        End If
                    Else
                        Exit While
                    End If
                End While
                If _connection Is Nothing Then
                    If loginMessage IsNot Nothing Then
                        Throw New ApplicationException(String.Format("No connection to Zerodha API could be established | Details:{0}", loginMessage))
                    Else
                        Throw New ApplicationException("No connection to Zerodha API could be established")
                    End If
                End If
#End Region

                OnHeartbeat("Completing all pre-automation requirements")
                _cts.Token.ThrowIfCancellationRequested()
                Dim isPreProcessingDone As Boolean = Await _commonController.PrepareToRunStrategyAsync().ConfigureAwait(False)
                _cts.Token.ThrowIfCancellationRequested()

                If Not isPreProcessingDone Then Throw New ApplicationException("PrepareToRunStrategyAsync did not succeed, cannot progress")
            End If 'Common controller
            EnableDisableUIEx(UIMode.ReleaseOther, GetType(EMA_SupertrendStrategy))

            Dim emaSupertrendStrategyToExecute As New EMA_SupertrendStrategy(_commonController, 4, _EMA_SupertrendUserInputs, 5, _cts)
            OnHeartbeatEx(String.Format("Running strategy:{0}", emaSupertrendStrategyToExecute.ToString), New List(Of Object) From {emaSupertrendStrategyToExecute})

            _cts.Token.ThrowIfCancellationRequested()
            Await _commonController.SubscribeStrategyAsync(emaSupertrendStrategyToExecute).ConfigureAwait(False)
            _cts.Token.ThrowIfCancellationRequested()

            _EMA_SupertrendTradableInstruments = emaSupertrendStrategyToExecute.TradableStrategyInstruments
            SetObjectText_ThreadSafe(linklblEMA_SupertrendTradableInstrument, String.Format("Tradable Instruments: {0}", _EMA_SupertrendTradableInstruments.Count))
            SetObjectEnableDisable_ThreadSafe(linklblEMA_SupertrendTradableInstrument, True)
            _cts.Token.ThrowIfCancellationRequested()

            _EMA_SupertrendDashboadList = New BindingList(Of ActivityDashboard)(emaSupertrendStrategyToExecute.SignalManager.ActivityDetails.Values.ToList)
            SetSFGridDataBind_ThreadSafe(sfdgvEMA_SupertrendMainDashboard, _EMA_SupertrendDashboadList)
            SetSFGridFreezFirstColumn_ThreadSafe(sfdgvEMA_SupertrendMainDashboard)
            _cts.Token.ThrowIfCancellationRequested()

            Await emaSupertrendStrategyToExecute.MonitorAsync().ConfigureAwait(False)
        Catch aex As AdapterBusinessException
            logger.Error(aex)
            If aex.ExceptionType = AdapterBusinessException.TypeOfException.PermissionException Then
                _lastException = aex
            Else
                MsgBox(String.Format("The following error occurred: {0}", aex.Message), MsgBoxStyle.Critical)
            End If
        Catch fex As ForceExitException
            logger.Error(fex)
            _lastException = fex
        Catch cx As OperationCanceledException
            logger.Error(cx)
            MsgBox(String.Format("The following error occurred: {0}", cx.Message), MsgBoxStyle.Critical)
        Catch ex As Exception
            logger.Error(ex)
            MsgBox(String.Format("The following error occurred: {0}", ex.Message), MsgBoxStyle.Critical)
        Finally
            ProgressStatus("No pending actions")
            EnableDisableUIEx(UIMode.ReleaseOther, GetType(EMA_SupertrendStrategy))
            EnableDisableUIEx(UIMode.Idle, GetType(EMA_SupertrendStrategy))
        End Try
        'If _cts Is Nothing OrElse _cts.IsCancellationRequested Then
        'Following portion need to be done for any kind of exception. Otherwise if we start again without closing the form then
        'it will not new object of controller. So orphan exception will throw exception again and information collector, historical data fetcher
        'and ticker will not work.
        If _commonController IsNot Nothing Then Await _commonController.CloseTickerIfConnectedAsync().ConfigureAwait(False)
        If _commonController IsNot Nothing Then Await _commonController.CloseFetcherIfConnectedAsync(True).ConfigureAwait(False)
        If _commonController IsNot Nothing Then Await _commonController.CloseCollectorIfConnectedAsync(True).ConfigureAwait(False)
        _commonController = Nothing
        _connection = Nothing
        _cts = Nothing
        'End If
    End Function
    Private Async Sub btnEMA_SupertrendStart_Click(sender As Object, e As EventArgs) Handles btnEMA_SupertrendStart.Click
        PreviousDayCleanup()
        Await Task.Run(AddressOf EMA_SupertrendWorkerAsync).ConfigureAwait(False)

        If _lastException IsNot Nothing Then
            If _lastException.GetType.BaseType Is GetType(AdapterBusinessException) AndAlso
                CType(_lastException, AdapterBusinessException).ExceptionType = AdapterBusinessException.TypeOfException.PermissionException Then
                Debug.WriteLine("Restart for permission")
                logger.Debug("Restarting the application again as there is premission issue")
                btnEMA_SupertrendStart_Click(sender, e)
            ElseIf _lastException.GetType Is GetType(ForceExitException) Then
                Debug.WriteLine("Restart for daily refresh")
                logger.Debug("Restarting the application again for daily refresh")
                btnEMA_SupertrendStart_Click(sender, e)
            End If
        End If
    End Sub
    Private Sub tmrEMA_SupertrendTickerStatus_Tick(sender As Object, e As EventArgs) Handles tmrEMA_SupertrendTickerStatus.Tick
        FlashTickerBulbEx(GetType(EMA_SupertrendStrategy))
    End Sub
    Private Async Sub btnEMA_SupertrendStop_Click(sender As Object, e As EventArgs) Handles btnEMA_SupertrendStop.Click
        If _commonController IsNot Nothing Then Await _commonController.CloseTickerIfConnectedAsync().ConfigureAwait(False)
        If _commonController IsNot Nothing Then Await _commonController.CloseFetcherIfConnectedAsync(True).ConfigureAwait(False)
        If _commonController IsNot Nothing Then Await _commonController.CloseCollectorIfConnectedAsync(True).ConfigureAwait(False)
        _cts.Cancel()
    End Sub
    Private Sub btnEMA_SupertrendSettings_Click(sender As Object, e As EventArgs) Handles btnEMA_SupertrendSettings.Click
        Dim newForm As New frmEMA_SupertrendSettings(_EMA_SupertrendUserInputs)
        newForm.ShowDialog()
    End Sub
    Private Sub linklblEMA_SupertrendTradableInstrument_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles linklblEMA_SupertrendTradableInstrument.LinkClicked
        Dim newForm As New frmEMA_SupertrendTradableInstrumentList(_EMA_SupertrendTradableInstruments)
        newForm.ShowDialog()
    End Sub
#End Region

#Region "Common to all stratgeies"

#Region "EX function"
    Private Enum UIMode
        Active = 1
        Idle
        BlockOther
        ReleaseOther
        None
    End Enum
    Private Sub EnableDisableUIEx(ByVal mode As UIMode, ByVal source As Object)
        If source Is GetType(OHLStrategy) Then
            Select Case mode
                Case UIMode.Active
                    SetObjectEnableDisable_ThreadSafe(btnOHLStart, False)
                    SetObjectEnableDisable_ThreadSafe(btnOHLStop, True)
                Case UIMode.BlockOther
                    If GetObjectText_ThreadSafe(btnMomentumReversalStart) = "Start" Then
                        SetObjectText_ThreadSafe(btnMomentumReversalStart, Common.LOGIN_PENDING)
                        SetObjectText_ThreadSafe(btnMomentumReversalStop, Common.LOGIN_PENDING)
                    End If
                    If GetObjectText_ThreadSafe(btnAmiSignalStart) = "Start" Then
                        SetObjectText_ThreadSafe(btnAmiSignalStart, Common.LOGIN_PENDING)
                        SetObjectText_ThreadSafe(btnAmiSignalStop, Common.LOGIN_PENDING)
                    End If
                Case UIMode.ReleaseOther
                    If GetObjectText_ThreadSafe(btnMomentumReversalStart) = Common.LOGIN_PENDING Then
                        SetObjectText_ThreadSafe(btnMomentumReversalStart, "Start")
                        SetObjectText_ThreadSafe(btnMomentumReversalStop, "Stop")
                    End If
                    If GetObjectText_ThreadSafe(btnAmiSignalStart) = Common.LOGIN_PENDING Then
                        SetObjectText_ThreadSafe(btnAmiSignalStart, "Start")
                        SetObjectText_ThreadSafe(btnAmiSignalStop, "Stop")
                    End If
                Case UIMode.Idle
                    SetObjectEnableDisable_ThreadSafe(btnOHLStart, True)
                    SetObjectEnableDisable_ThreadSafe(btnOHLStop, False)
                    SetObjectEnableDisable_ThreadSafe(linklblOHLTradableInstruments, False)
                    SetSFGridDataBind_ThreadSafe(sfdgvOHLMainDashboard, Nothing)
            End Select
        ElseIf source Is GetType(MomentumReversalStrategy) Then
            Select Case mode
                Case UIMode.Active
                    SetObjectEnableDisable_ThreadSafe(btnMomentumReversalStart, False)
                    SetObjectEnableDisable_ThreadSafe(btnMomentumReversalStop, True)
                    'SetObjectEnableDisable_ThreadSafe(btnMomentumReversalSettings, False)
                Case UIMode.BlockOther
                    If GetObjectText_ThreadSafe(btnOHLStart) = "Start" Then
                        SetObjectText_ThreadSafe(btnOHLStart, Common.LOGIN_PENDING)
                        SetObjectText_ThreadSafe(btnOHLStop, Common.LOGIN_PENDING)
                    End If
                    If GetObjectText_ThreadSafe(btnAmiSignalStart) = "Start" Then
                        SetObjectText_ThreadSafe(btnAmiSignalStart, Common.LOGIN_PENDING)
                        SetObjectText_ThreadSafe(btnAmiSignalStop, Common.LOGIN_PENDING)
                    End If
                Case UIMode.ReleaseOther
                    If GetObjectText_ThreadSafe(btnOHLStart) = Common.LOGIN_PENDING Then
                        SetObjectText_ThreadSafe(btnOHLStart, "Start")
                        SetObjectText_ThreadSafe(btnOHLStop, "Stop")
                    End If
                    If GetObjectText_ThreadSafe(btnAmiSignalStart) = Common.LOGIN_PENDING Then
                        SetObjectText_ThreadSafe(btnAmiSignalStart, "Start")
                        SetObjectText_ThreadSafe(btnAmiSignalStop, "Stop")
                    End If
                Case UIMode.Idle
                    SetObjectEnableDisable_ThreadSafe(btnMomentumReversalStart, True)
                    SetObjectEnableDisable_ThreadSafe(btnMomentumReversalStop, False)
                    'SetObjectEnableDisable_ThreadSafe(btnMomentumReversalSettings, True)
                    SetObjectEnableDisable_ThreadSafe(linklblMomentumReversalTradableInstrument, False)
                    SetSFGridDataBind_ThreadSafe(sfdgvMomentumReversalMainDashboard, Nothing)
            End Select
        ElseIf source Is GetType(AmiSignalStrategy) Then
            Select Case mode
                Case UIMode.Active
                    SetObjectEnableDisable_ThreadSafe(btnAmiSignalStart, False)
                    SetObjectEnableDisable_ThreadSafe(btnAmiSignalStop, True)
                Case UIMode.BlockOther
                    If GetObjectText_ThreadSafe(btnOHLStart) = "Start" Then
                        SetObjectText_ThreadSafe(btnOHLStart, Common.LOGIN_PENDING)
                        SetObjectText_ThreadSafe(btnOHLStop, Common.LOGIN_PENDING)
                    End If
                    If GetObjectText_ThreadSafe(btnMomentumReversalStart) = "Start" Then
                        SetObjectText_ThreadSafe(btnMomentumReversalStart, Common.LOGIN_PENDING)
                        SetObjectText_ThreadSafe(btnMomentumReversalStop, Common.LOGIN_PENDING)
                    End If
                Case UIMode.ReleaseOther
                    If GetObjectText_ThreadSafe(btnOHLStart) = Common.LOGIN_PENDING Then
                        SetObjectText_ThreadSafe(btnOHLStart, "Start")
                        SetObjectText_ThreadSafe(btnOHLStop, "Stop")
                    End If
                    If GetObjectText_ThreadSafe(btnMomentumReversalStart) = Common.LOGIN_PENDING Then
                        SetObjectText_ThreadSafe(btnMomentumReversalStart, "Start")
                        SetObjectText_ThreadSafe(btnMomentumReversalStop, "Stop")
                    End If
                Case UIMode.Idle
                    SetObjectEnableDisable_ThreadSafe(btnAmiSignalStart, True)
                    SetObjectEnableDisable_ThreadSafe(btnAmiSignalStop, False)
                    SetSFGridDataBind_ThreadSafe(sfdgvAmiSignalMainDashboard, Nothing)
            End Select
        ElseIf source Is GetType(EMA_SupertrendStrategy) Then
            Select Case mode
                Case UIMode.Active
                    SetObjectEnableDisable_ThreadSafe(btnEMA_SupertrendStart, False)
                    SetObjectEnableDisable_ThreadSafe(btnEMA_SupertrendStop, True)
                Case UIMode.BlockOther
                    If GetObjectText_ThreadSafe(btnOHLStart) = "Start" Then
                        SetObjectText_ThreadSafe(btnOHLStart, Common.LOGIN_PENDING)
                        SetObjectText_ThreadSafe(btnOHLStop, Common.LOGIN_PENDING)
                    End If
                    If GetObjectText_ThreadSafe(btnMomentumReversalStart) = "Start" Then
                        SetObjectText_ThreadSafe(btnMomentumReversalStart, Common.LOGIN_PENDING)
                        SetObjectText_ThreadSafe(btnMomentumReversalStop, Common.LOGIN_PENDING)
                    End If
                    If GetObjectText_ThreadSafe(btnAmiSignalStart) = "Start" Then
                        SetObjectText_ThreadSafe(btnAmiSignalStart, Common.LOGIN_PENDING)
                        SetObjectText_ThreadSafe(btnAmiSignalStop, Common.LOGIN_PENDING)
                    End If
                Case UIMode.ReleaseOther
                    If GetObjectText_ThreadSafe(btnOHLStart) = Common.LOGIN_PENDING Then
                        SetObjectText_ThreadSafe(btnOHLStart, "Start")
                        SetObjectText_ThreadSafe(btnOHLStop, "Stop")
                    End If
                    If GetObjectText_ThreadSafe(btnMomentumReversalStart) = Common.LOGIN_PENDING Then
                        SetObjectText_ThreadSafe(btnMomentumReversalStart, "Start")
                        SetObjectText_ThreadSafe(btnMomentumReversalStop, "Stop")
                    End If
                    If GetObjectText_ThreadSafe(btnAmiSignalStart) = "Start" Then
                        SetObjectText_ThreadSafe(btnAmiSignalStart, Common.LOGIN_PENDING)
                        SetObjectText_ThreadSafe(btnAmiSignalStop, Common.LOGIN_PENDING)
                    End If
                Case UIMode.Idle
                    SetObjectEnableDisable_ThreadSafe(btnEMA_SupertrendStart, True)
                    SetObjectEnableDisable_ThreadSafe(btnEMA_SupertrendStop, False)
                    SetSFGridDataBind_ThreadSafe(sfdgvEMA_SupertrendMainDashboard, Nothing)
            End Select
        End If
    End Sub
    Private Sub FlashTickerBulbEx(ByVal source As Object)
        Dim blbTickerStatusCommon As Bulb.LedBulb = Nothing
        Dim tmrTickerStatusCommon As System.Windows.Forms.Timer = Nothing
        If source Is GetType(OHLStrategy) Then
            blbTickerStatusCommon = blbOHLTickerStatus
            tmrTickerStatusCommon = tmrOHLTickerStatus
        ElseIf source Is GetType(MomentumReversalStrategy) Then
            blbTickerStatusCommon = blbMomentumReversalTickerStatus
            tmrTickerStatusCommon = tmrMomentumReversalTickerStatus
        ElseIf source Is GetType(AmiSignalStrategy) Then
            blbTickerStatusCommon = blbAmiSignalTickerStatus
            tmrTickerStatusCommon = tmrAmiSignalTickerStatus
        ElseIf source Is GetType(EMA_SupertrendStrategy) Then
            blbTickerStatusCommon = blbEMA_SupertrendTickerStatus
            tmrTickerStatusCommon = tmrEMA_SupertrendTickerStatus
        End If

        tmrTickerStatusCommon.Enabled = False

        'Dim trialEndDate As Date = New Date(2019, 3, 16, 0, 0, 0)
        'If Now() >= trialEndDate Then
        '    MsgBox("You Trial Period is over. Kindly contact Algo2Trade", MsgBoxStyle.Critical)
        '    End
        'End If

        If tmrTickerStatusCommon.Interval = 700 Then
            tmrTickerStatusCommon.Interval = 2000
            blbTickerStatusCommon.Visible = True
        Else
            tmrTickerStatusCommon.Interval = 700
            blbTickerStatusCommon.Visible = False
        End If
        tmrTickerStatusCommon.Enabled = True
    End Sub
    Private Sub ColorTickerBulbEx(ByVal source As Object, ByVal color As Color)
        Dim blbTickerStatusCommon As Bulb.LedBulb = Nothing
        If source Is GetType(OHLStrategy) Then
            blbTickerStatusCommon = blbOHLTickerStatus
        ElseIf source Is GetType(MomentumReversalStrategy) Then
            blbTickerStatusCommon = blbMomentumReversalTickerStatus
        ElseIf source Is GetType(AmiSignalStrategy) Then
            blbTickerStatusCommon = blbAmiSignalTickerStatus
        ElseIf source Is GetType(EMA_SupertrendStrategy) Then
            blbTickerStatusCommon = blbEMA_SupertrendTickerStatus
        End If
        blbTickerStatusCommon.Color = color
    End Sub
    Private Enum GridMode
        TouchupPopupFilter
        TouchupAutogeneratingColumn
        FrozenGridColumn
        None
    End Enum
    Private Sub ManipulateGridEx(ByVal mode As GridMode, ByVal parameter As Object, ByVal source As Object)
        Dim sfdgvCommon As SfDataGrid = Nothing
        If source Is GetType(OHLStrategy) Then
            sfdgvCommon = sfdgvOHLMainDashboard
        ElseIf source Is GetType(MomentumReversalStrategy) Then
            sfdgvCommon = sfdgvMomentumReversalMainDashboard
        ElseIf source Is GetType(AmiSignalStrategy) Then
            sfdgvCommon = sfdgvAmiSignalMainDashboard
        ElseIf source Is GetType(EMA_SupertrendStrategy) Then
            sfdgvCommon = sfdgvEMA_SupertrendMainDashboard
        End If

        Dim eFilterPopupShowingEventArgsCommon As FilterPopupShowingEventArgs = Nothing
        Dim eAutoGeneratingColumnArgsCommon As AutoGeneratingColumnArgs = Nothing
        Dim colorToUseCommon As Color = Nothing
        Select Case mode
            Case GridMode.TouchupPopupFilter
                eFilterPopupShowingEventArgsCommon = parameter
            Case GridMode.TouchupAutogeneratingColumn
                eAutoGeneratingColumnArgsCommon = parameter
        End Select

        If eFilterPopupShowingEventArgsCommon IsNot Nothing Then

            eFilterPopupShowingEventArgsCommon.Control.BackColor = ColorTranslator.FromHtml("#EDF3F3")

            'Customize the appearance of the CheckedListBox

            sfdgvCommon.Style.CheckBoxStyle.CheckedBackColor = Color.White
            sfdgvCommon.Style.CheckBoxStyle.CheckedTickColor = Color.LightSkyBlue
            eFilterPopupShowingEventArgsCommon.Control.CheckListBox.Style.CheckBoxStyle.CheckedBackColor = Color.White
            eFilterPopupShowingEventArgsCommon.Control.CheckListBox.Style.CheckBoxStyle.CheckedTickColor = Color.LightSkyBlue

            'Customize the appearance of the Ok and Cancel buttons
            eFilterPopupShowingEventArgsCommon.Control.CancelButton.BackColor = Color.DeepSkyBlue
            eFilterPopupShowingEventArgsCommon.Control.OkButton.BackColor = eFilterPopupShowingEventArgsCommon.Control.CancelButton.BackColor
            eFilterPopupShowingEventArgsCommon.Control.CancelButton.ForeColor = Color.White
            eFilterPopupShowingEventArgsCommon.Control.OkButton.ForeColor = eFilterPopupShowingEventArgsCommon.Control.CancelButton.ForeColor
        ElseIf eAutoGeneratingColumnArgsCommon IsNot Nothing Then
            sfdgvCommon.Style.HeaderStyle.BackColor = Color.DeepSkyBlue
            sfdgvCommon.Style.HeaderStyle.TextColor = Color.White

            sfdgvCommon.Style.CheckBoxStyle.CheckedBackColor = Color.White
            sfdgvCommon.Style.CheckBoxStyle.CheckedTickColor = Color.LightSkyBlue
            If eAutoGeneratingColumnArgsCommon.Column.CellType = "DateTime" Then
                CType(eAutoGeneratingColumnArgsCommon.Column, GridDateTimeColumn).Pattern = DateTimePattern.SortableDateTime
            End If
        End If
    End Sub
    Private Enum LogMode
        All
        One
        None
    End Enum
    Private Sub WriteLogEx(ByVal mode As LogMode, ByVal msg As String, ByVal source As Object)
        Dim lstLogCommon As ListBox = Nothing
        If source IsNot Nothing AndAlso source.GetType Is GetType(OHLStrategy) Then
            Select Case mode
                Case LogMode.One
                    SetListAddItem_ThreadSafe(lstOHLLog, String.Format("{0}-{1}", Format(ISTNow, "yyyy-MM-dd HH:mm:ss"), msg))
            End Select
        ElseIf source IsNot Nothing AndAlso source.GetType Is GetType(MomentumReversalStrategy) Then
            Select Case mode
                Case LogMode.One
                    SetListAddItem_ThreadSafe(lstMomentumReversalLog, String.Format("{0}-{1}", Format(ISTNow, "yyyy-MM-dd HH:mm:ss"), msg))
            End Select
        ElseIf source IsNot Nothing AndAlso source.GetType Is GetType(AmiSignalStrategy) Then
            Select Case mode
                Case LogMode.One
                    SetListAddItem_ThreadSafe(lstAmiSignalLog, String.Format("{0}-{1}", Format(ISTNow, "yyyy-MM-dd HH:mm:ss"), msg))
            End Select
        ElseIf source IsNot Nothing AndAlso source.GetType Is GetType(EMA_SupertrendStrategy) Then
            Select Case mode
                Case LogMode.One
                    SetListAddItem_ThreadSafe(lstEMA_SupertrendLog, String.Format("{0}-{1}", Format(ISTNow, "yyyy-MM-dd HH:mm:ss"), msg))
            End Select
        ElseIf source Is Nothing Then
            Select Case mode
                Case LogMode.All
                    SetListAddItem_ThreadSafe(lstOHLLog, String.Format("{0}-{1}", Format(ISTNow, "yyyy-MM-dd HH:mm:ss"), msg))
                    SetListAddItem_ThreadSafe(lstMomentumReversalLog, String.Format("{0}-{1}", Format(ISTNow, "yyyy-MM-dd HH:mm:ss"), msg))
                    SetListAddItem_ThreadSafe(lstAmiSignalLog, String.Format("{0}-{1}", Format(ISTNow, "yyyy-MM-dd HH:mm:ss"), msg))
                    SetListAddItem_ThreadSafe(lstEMA_SupertrendLog, String.Format("{0}-{1}", Format(ISTNow, "yyyy-MM-dd HH:mm:ss"), msg))
            End Select
        End If
    End Sub
    Private Sub PreviousDayCleanup()
        Try
            Dim todayDate As String = Now.ToString("yy_MM_dd")
            For Each runningFile In Directory.GetFiles(My.Application.Info.DirectoryPath, "*.Instruments.a2t")
                If Not runningFile.Contains(todayDate) Then File.Delete(runningFile)
            Next
            For Each runningFile In Directory.GetFiles(My.Application.Info.DirectoryPath, "*.ActivityDashboard.a2t")
                If Not runningFile.Contains(todayDate) Then File.Delete(runningFile)
            Next
            For Each runningFile In Directory.GetFiles(My.Application.Info.DirectoryPath, "*.Margin.a2t")
                If Not runningFile.Contains(todayDate) Then File.Delete(runningFile)
            Next
        Catch ex As Exception
            logger.Error(ex)
            MsgBox(String.Format("The following error occurred: {0}", ex.Message), MsgBoxStyle.Critical)
        End Try
    End Sub
#End Region

#Region "EX Users"
    Private Sub frmMainTabbed_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        GlobalDiagnosticsContext.Set("appname", My.Application.Info.AssemblyName)
        GlobalDiagnosticsContext.Set("version", My.Application.Info.Version.ToString)
        logger.Trace("*************************** Logging started ***************************")

        If File.Exists(ControllerUserInputs.Filename) Then
            _commonControllerUserInput = Utilities.Strings.DeserializeToCollection(Of ControllerUserInputs)(ControllerUserInputs.Filename)
        End If
        If Not Common.IsZerodhaUserDetailsPopulated(_commonControllerUserInput) Then
            miUserDetails_Click(sender, e)
        End If
        Me.Text = String.Format("Algo2Trade Robot v{0}", My.Application.Info.Version)
        EnableDisableUIEx(UIMode.Idle, GetType(OHLStrategy))
        EnableDisableUIEx(UIMode.Idle, GetType(MomentumReversalStrategy))
        EnableDisableUIEx(UIMode.Idle, GetType(AmiSignalStrategy))
        EnableDisableUIEx(UIMode.Idle, GetType(EMA_SupertrendStrategy))
        'tabMain.TabPages.Remove(tabOHL)
        'tabMain.TabPages.Remove(tabMomentumReversal)
        'tabMain.TabPages.Remove(tabAmiSignal)
    End Sub
    Private Sub OnTickerClose()
        ColorTickerBulbEx(GetType(OHLStrategy), Color.Pink)
        ColorTickerBulbEx(GetType(MomentumReversalStrategy), Color.Pink)
        ColorTickerBulbEx(GetType(AmiSignalStrategy), Color.Pink)
        ColorTickerBulbEx(GetType(EMA_SupertrendStrategy), Color.Pink)
        OnHeartbeat("Ticker:Closed")
    End Sub
    Private Sub OnTickerConnect()
        ColorTickerBulbEx(GetType(OHLStrategy), Color.Lime)
        ColorTickerBulbEx(GetType(MomentumReversalStrategy), Color.Lime)
        ColorTickerBulbEx(GetType(AmiSignalStrategy), Color.Lime)
        ColorTickerBulbEx(GetType(EMA_SupertrendStrategy), Color.Lime)
        OnHeartbeat("Ticker:Connected")
    End Sub
    Private Sub OnTickerErrorWithStatus(ByVal isConnected As Boolean, ByVal errorMsg As String)
        If Not isConnected Then
            ColorTickerBulbEx(GetType(OHLStrategy), Color.Pink)
            ColorTickerBulbEx(GetType(MomentumReversalStrategy), Color.Pink)
            ColorTickerBulbEx(GetType(AmiSignalStrategy), Color.Pink)
            ColorTickerBulbEx(GetType(EMA_SupertrendStrategy), Color.Pink)
        End If
    End Sub
    Private Sub OnTickerError(ByVal errorMsg As String)
        OnHeartbeat(String.Format("Ticker:Error:{0}", errorMsg))
    End Sub
    Private Sub OnTickerNoReconnect()
        'Nothing to do
    End Sub
    Private Sub OnTickerReconnect()
        ColorTickerBulbEx(GetType(OHLStrategy), Color.Yellow)
        ColorTickerBulbEx(GetType(MomentumReversalStrategy), Color.Yellow)
        ColorTickerBulbEx(GetType(AmiSignalStrategy), Color.Yellow)
        ColorTickerBulbEx(GetType(EMA_SupertrendStrategy), Color.Yellow)
        OnHeartbeat("Ticker:Reconnecting")
    End Sub
    Private Sub OnFetcherError(ByVal instrumentIdentifier As String, ByVal errorMsg As String)
        OnHeartbeat(String.Format("Historical Data Fetcher: Error:{0}, InstrumentIdentifier:{1}", errorMsg, instrumentIdentifier))
    End Sub
    Private Sub OnCollectorError(ByVal errorMsg As String)
        OnHeartbeat(String.Format("Information Collector: Error:{0}", errorMsg))
    End Sub
    Public Sub ProgressStatus(ByVal msg As String)
        If Not msg.EndsWith("...") Then msg = String.Format("{0}...", msg)
        WriteLogEx(LogMode.All, msg, Nothing)
        logger.Info(msg)
    End Sub
    Public Sub ProgressStatusEx(ByVal msg As String, ByVal source As List(Of Object))
        If Not msg.EndsWith("...") Then msg = String.Format("{0}...", msg)
        If source Is Nothing Then
            WriteLogEx(LogMode.All, msg, Nothing)
        ElseIf source IsNot Nothing AndAlso source.Count > 0 Then
            For Each runningSource In source
                WriteLogEx(LogMode.One, msg, runningSource)
            Next
        End If
        logger.Info(msg)
    End Sub
    Private Sub OnHeartbeatEx(msg As String, ByVal source As List(Of Object))
        'Update detailed status on the first part, dont append if the text starts with <
        If msg.Contains("<<<") Then
            msg = Replace(msg, "<<<", Nothing)
            ProgressStatusEx(msg, source)
        Else
            ProgressStatusEx(msg, source)
        End If
        msg = Nothing
    End Sub
    Private Sub OnWaitingForEx(elapsedSecs As Integer, totalSecs As Integer, msg As String, ByVal source As List(Of Object))
        If msg.Contains("...") Then msg = msg.Replace("...", "")
        ProgressStatusEx(String.Format("{0}, waiting {1}/{2} secs", msg, elapsedSecs, totalSecs), source)
    End Sub
    Private Sub OnDocumentRetryStatusEx(currentTry As Integer, totalTries As Integer, ByVal source As List(Of Object))
        'ProgressStatusEx(String.Format("Try #{0}/{1}: Connecting", currentTry, totalTries), source)
    End Sub
    Private Sub OnDocumentDownloadCompleteEx(ByVal source As List(Of Object))
    End Sub
    Protected Overridable Sub OnNewItemAdded(ByVal item As ActivityDashboard)
        If item IsNot Nothing Then
            Select Case item.ParentStrategyInstrument.ParentStrategy.GetType
                Case GetType(MomentumReversalStrategy)
                    BindingListAdd_ThreadSafe(_MRdashboadList, item)
                Case GetType(OHLStrategy)
                    BindingListAdd_ThreadSafe(_OHLdashboadList, item)
                Case GetType(AmiSignalStrategy)
                    BindingListAdd_ThreadSafe(_AmidashboadList, item)
                Case GetType(EMA_SupertrendStrategy)
                    BindingListAdd_ThreadSafe(_EMA_SupertrendDashboadList, item)
                Case Else
                    Throw New NotImplementedException
            End Select
        End If
    End Sub
    Protected Sub OnSessionExpiry(ByVal runningStrategy As Strategy)
        SetSFGridDataBind_ThreadSafe(sfdgvMomentumReversalMainDashboard, Nothing)
        Select Case runningStrategy.GetType
            Case GetType(MomentumReversalStrategy)
                _MRdashboadList = Nothing
                _MRdashboadList = New BindingList(Of ActivityDashboard)(runningStrategy.SignalManager.ActivityDetails.Values.ToList)
                SetSFGridDataBind_ThreadSafe(sfdgvMomentumReversalMainDashboard, _MRdashboadList)
            Case GetType(OHLStrategy)
                _OHLdashboadList = Nothing
                _OHLdashboadList = New BindingList(Of ActivityDashboard)(runningStrategy.SignalManager.ActivityDetails.Values.ToList)
                SetSFGridDataBind_ThreadSafe(sfdgvMomentumReversalMainDashboard, _OHLdashboadList)
            Case GetType(AmiSignalStrategy)
                _AmidashboadList = Nothing
                _AmidashboadList = New BindingList(Of ActivityDashboard)(runningStrategy.SignalManager.ActivityDetails.Values.ToList)
                SetSFGridDataBind_ThreadSafe(sfdgvMomentumReversalMainDashboard, _AmidashboadList)
            Case GetType(EMA_SupertrendStrategy)
                _EMA_SupertrendDashboadList = Nothing
                _EMA_SupertrendDashboadList = New BindingList(Of ActivityDashboard)(runningStrategy.SignalManager.ActivityDetails.Values.ToList)
                SetSFGridDataBind_ThreadSafe(sfdgvEMA_SupertrendMainDashboard, _AmidashboadList)
            Case Else
                Throw New NotImplementedException
        End Select
        SetSFGridFreezFirstColumn_ThreadSafe(sfdgvMomentumReversalMainDashboard)
        SetSFGridFreezFirstColumn_ThreadSafe(sfdgvEMA_SupertrendMainDashboard)
    End Sub
#End Region

#End Region

End Class