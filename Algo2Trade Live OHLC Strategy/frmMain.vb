Imports System.IO
Imports System.Threading
Imports Utilities.DAL
Imports KiteConnect
Imports Algo2TradeBLL
Imports NLog
Imports System.ComponentModel

Public Class frmMain
#Region "Common Delegates"
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
        If Not [label].IsDisposed AndAlso Not Me.IsDisposed Then
            If [label].InvokeRequired Then
                Dim MyDelegate As New SetLabelText_Delegate(AddressOf SetLabelText_ThreadSafe)
                Me.Invoke(MyDelegate, New Object() {[label], [text]})
            Else
                [label].Text = [text]
            End If
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

    Delegate Sub SetDatagridBindDatatable_Delegate(ByVal [datagrid] As DataGridView, ByVal [table] As DataTable)
    Public Sub SetDatagridBindDatatable_ThreadSafe(ByVal [datagrid] As DataGridView, ByVal [table] As DataTable)
        ' InvokeRequired required compares the thread ID of the calling thread to the thread ID of the creating thread.  
        ' If these threads are different, it returns true.  
        If [datagrid].InvokeRequired Then
            Dim MyDelegate As New SetDatagridBindDatatable_Delegate(AddressOf SetDatagridBindDatatable_ThreadSafe)
            Me.Invoke(MyDelegate, New Object() {[datagrid], [table]})
        Else
            [datagrid].DataSource = [table]
        End If
    End Sub
    Delegate Function GetTextBoxText_Delegate(ByVal [Text] As TextBox) As String
    Public Function GetTextBoxText_ThreadSafe(ByVal [Text] As TextBox) As String
        ' InvokeRequired required compares the thread ID of the calling thread to the thread ID of the creating thread.  
        ' If these threads are different, it returns true.  
        If [Text].InvokeRequired Then
            Dim MyDelegate As New GetTextBoxText_Delegate(AddressOf GetTextBoxText_ThreadSafe)
            Return Me.Invoke(MyDelegate, New Object() {[Text]})
        Else
            Return [Text].Text
        End If
    End Function
    Delegate Sub SetDatagridBindClass_Delegate(ByVal [dg] As DataGridView, ByVal [value] As BindingList(Of OHLStrategyTradableInstrument))
    Private Sub SetDatagridBindClass_ThreadSafe(ByVal [dg] As DataGridView, ByVal [value] As BindingList(Of OHLStrategyTradableInstrument))
        ' InvokeRequired required compares the thread ID of the calling thread to the thread ID of the creating thread.  
        ' If these threads are different, it returns true.  
        If [dg].InvokeRequired Then
            Dim MyDelegate As New SetDatagridBindClass_Delegate(AddressOf SetDatagridBindClass_ThreadSafe)
            Me.Invoke(MyDelegate, New Object() {[dg], [value]})
        Else
            [dg].DataSource = [value]
            For Each dc As DataGridViewColumn In [dg].Columns
                If dc.ValueType = GetType(System.DateTime) Then
                    dc.DefaultCellStyle.Format = "dd/MM/yyyy h:mm:ss tt"
                End If
            Next
            [dg].Refresh()
        End If
    End Sub
#End Region

#Region "Logging and Status Progress"
    Public Shared logger As Logger = LogManager.GetCurrentClassLogger
#End Region

#Region "Variables"
    Private _myAPIKey As String = "hcwmefsivttbchla"
    Private _myAPISecret As String = "t9rd8wut44ija2vp15y87hln28h5oppb"
    Private _myAPIVersion As String = "3"
    Private _myUserId As String = "DK4056"
    Private _myPassword As String = "Zerodha@123a"
    Private _ticker As Ticker = Nothing
    Private _canceller As CancellationTokenSource = Nothing
    Private _zerodhaKite As ZerodhaKiteHelper = Nothing
    Private _allInstruments As List(Of Instrument) = Nothing
    Private _todaysInstrumentsForOHLStrategy As Dictionary(Of String, OHLStrategyTradableInstrument) = Nothing
    Private _ohlStrategy As OHLStrategyHelper = Nothing
    Private _mergedListOfAllInstruments As New BindingList(Of OHLStrategyTradableInstrument)
#End Region

    Private Sub InitUI(ByVal enabled As Boolean)
        SetObjectEnableDisable_ThreadSafe(btnSelectedStocksStartAlgo, enabled)
        SetObjectEnableDisable_ThreadSafe(btnSelectedStocksStopAlgo, Not enabled)
        SetLabelText_ThreadSafe(lblSelectedStocksProgress, "Progess Status .....")
    End Sub
    Private Sub frmMain_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        GlobalDiagnosticsContext.Set("appname", My.Application.Info.AssemblyName)
        GlobalDiagnosticsContext.Set("version", My.Application.Info.Version.ToString)
        logger.Trace("*************************** Logging started ***************************")
        Me.Text = String.Format("{0} - v{1}", My.Application.Info.Title, My.Application.Info.Version)
        InitUI(True)
    End Sub
    Private Sub btnBrowse_Click(sender As Object, e As EventArgs) Handles btnSelectedStocksFilePathBrowse.Click
        opnFileDialog.ShowDialog()
    End Sub
    Private Sub opnFileDialog_FileOk(sender As Object, e As System.ComponentModel.CancelEventArgs) Handles opnFileDialog.FileOk
        txtSelectedStocksFilePath.Text = opnFileDialog.FileName
    End Sub
    Private Sub btnStop_Click(sender As Object, e As EventArgs) Handles btnSelectedStocksStopAlgo.Click
        _canceller.Cancel()
    End Sub
    Private Async Sub btnStart_Click(sender As Object, e As EventArgs) Handles btnSelectedStocksStartAlgo.Click
        _canceller = New CancellationTokenSource
        InitUI(False)
        Try
            Dim workableInputsAfterVaidation As Tuple(Of DataTable) = Nothing
            OnHeartbeat("Validating inputs...")
            workableInputsAfterVaidation = ValidateInputs()

            OnHeartbeat("Attempting login process...")
            _zerodhaKite = New ZerodhaKiteHelper(_myUserId, _myPassword, _myAPIKey, _myAPISecret, _myAPIVersion, _canceller)
            AddHandler _zerodhaKite.Heartbeat, AddressOf OnHeartbeat
            AddHandler _zerodhaKite.DocumentDownloadComplete, AddressOf OnDocumentDownloadComplete
            AddHandler _zerodhaKite.DocumentRetryStatus, AddressOf OnDocumentRetryStatus
            AddHandler _zerodhaKite.WaitingFor, AddressOf OnWaitingFor

            OnHeartbeat("Attempting login authentication...")
            Dim tempRet As Boolean = Await _zerodhaKite.LoginAsync().ConfigureAwait(False)
            Dim accessObtained As Boolean = False
            If tempRet Then
                OnHeartbeat("Attempting login access...")
                accessObtained = Await _zerodhaKite.RequestAccessTokenAsync().ConfigureAwait(False)
            End If

            If accessObtained Then
                'Get active instruments for the day
                OnHeartbeat("Getting instrument list for the day...")
                Dim ret As Dictionary(Of String, Object) = Await _zerodhaKite.ExecuteCommandAsync(ZerodhaKiteHelper.KiteCommands.GetInstruments, Nothing).ConfigureAwait(False)
                If ret IsNot Nothing Then
                    _allInstruments = ret(ZerodhaKiteHelper.KiteCommands.GetInstruments.ToString)
                Else
                    Throw New ApplicationException("Instrument list for the day could not be obtained from Zerodha, cannot proceed")
                End If
                OnHeartbeat("Creating tradable instruments for this stratgey...")
                _ohlStrategy = New OHLStrategyHelper(_zerodhaKite, _canceller)
                AddHandler _ohlStrategy.Heartbeat, AddressOf OnHeartbeat
                AddHandler _ohlStrategy.DocumentDownloadComplete, AddressOf OnDocumentDownloadComplete
                AddHandler _ohlStrategy.DocumentRetryStatus, AddressOf OnDocumentRetryStatus
                AddHandler _ohlStrategy.WaitingFor, AddressOf OnWaitingFor
                _todaysInstrumentsForOHLStrategy = _ohlStrategy.GetTodaysInstruments(workableInputsAfterVaidation.Item1, _allInstruments)
                'Open ticker and subscribe to active instruments
                OnHeartbeat("Creating ticker and subscribe to instruments...")
                initTicker()

                Dim totalPL As Decimal = 0
                For Each instruments In _todaysInstrumentsForOHLStrategy.Values
                    _mergedListOfAllInstruments.Add(instruments)
                    totalPL += instruments.PLD
                Next
                SetDatagridBindClass_ThreadSafe(dgrvSelectedStocksMainView, _mergedListOfAllInstruments)
                SetLabelText_ThreadSafe(lblSelectedStocksTotalPL, String.Format("Total PL: {0}", totalPL))
                Await _ohlStrategy.RunAsync(_todaysInstrumentsForOHLStrategy).ConfigureAwait(False)
                'Await TestCheck().ConfigureAwait(False)
            Else
                Throw New ApplicationException("Login process failed while obtaining access token")
            End If
        Catch oex As OperationCanceledException
            logger.Error(oex)
            MsgBox(String.Format("The following error occurred: {0}", oex.Message), MsgBoxStyle.Critical)
        Catch ex As Exception
            logger.Error(ex)
            MsgBox(String.Format("The following error occurred: {0}", ex.Message), MsgBoxStyle.Critical)
        Finally
            InitUI(True)
        End Try
    End Sub
    Private Sub frmMain_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        If _ticker IsNot Nothing Then
            _ticker.Close()
        End If
    End Sub
    Private Function ValidateInputs() As Tuple(Of DataTable)
        Dim ret As Tuple(Of DataTable)
        OnHeartbeat("Validating selected stocks file input...")
        Dim dt As DataTable = ValidateSelectedStocksFile()
        ret = New Tuple(Of DataTable)(dt)
        Return ret
    End Function
    Private Function ValidateSelectedStocksFile() As DataTable
        Dim ret As DataTable = Nothing
        Dim filePath As String = GetTextBoxText_ThreadSafe(txtSelectedStocksFilePath)
        Try
            If File.Exists(filePath) Then
                Dim dt As DataTable = Nothing
                If Path.GetExtension(filePath) <> ".csv" Then Throw New ApplicationException(String.Format("Invalid File Type. Only (.csv) file supported. File:{0}", filePath))
                Using csvConverter As New CSVHelper(filePath, ",", _canceller)
                    AddHandler csvConverter.Heartbeat, AddressOf OnHeartbeat
                    AddHandler csvConverter.WaitingFor, AddressOf OnWaitingFor
                    dt = csvConverter.GetDataTableFromCSV(1)
                End Using

                If dt IsNot Nothing AndAlso dt.Rows.Count > 0 AndAlso dt.Columns.Count = 2 Then
                    logger.Debug("Total count of records in CSV file:{0}", dt.Rows.Count)
                    For i As Integer = 0 To dt.Rows.Count - 1
                        OnHeartbeat(String.Format("Validating data from input CSV file {0}/{1}", i + 1, dt.Rows.Count))
                        If dt.Rows(i).Item(0).trim = "" AndAlso dt.Rows(i).Item(1).trim = "" Then
                            Throw New ApplicationException(String.Format("Blank row found in CSV File. Row Number:{0}, File:{1}", i + 1, filePath))
                        ElseIf dt.Rows(i).Item(0).trim = "" Then
                            Throw New ApplicationException(String.Format("First column null found in CSV File. Row Number:{0}, File:{1}", i + 1, filePath))
                        ElseIf dt.Rows(i).Item(1).trim = "" Then
                            Throw New ApplicationException(String.Format("Second column null found in CSV File. Row Number:{0}, File:{1}", i + 1, filePath))
                        End If
                        If Not IsNumeric(dt.Rows(i).Item(1).trim) Then Throw New ApplicationException(String.Format("Second column should be a number. Row Number:{0}, File:{1}", i + 1, filePath))
                    Next
                    ret = dt
                Else
                    If dt.Columns.Count <> 2 Then
                        Throw New ApplicationException(String.Format("Both the columns in the CSV file were not found, File:{0}", filePath))
                    Else
                        Throw New ApplicationException(String.Format("Blank CSV input file detected. File:{0}", filePath))
                    End If
                End If
            Else
                Throw New ApplicationException(String.Format("CSV input file does not exist. File:{0}", filePath))
            End If
        Catch ax As ApplicationException
            Throw ax
        Catch ex As Exception
            If ex.Message.Contains(filePath) Then
                Throw ex
            Else
                Throw New ApplicationException(String.Format("Unknown exception:{0}, File:{1}", ex.Message, filePath), ex)
            End If
        End Try

        Return ret
    End Function
    Private Sub frmMain_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        If _ticker IsNot Nothing AndAlso _ticker.IsConnected Then
            _ticker.Close()
        End If
        End
    End Sub
    Private Sub initTicker()
        _ticker = New Ticker(_myAPIKey, _zerodhaKite.AccessToken)

        AddHandler _ticker.OnTick, AddressOf OnTick
        AddHandler _ticker.OnReconnect, AddressOf OnReconnect
        AddHandler _ticker.OnNoReconnect, AddressOf OnNoReconnect
        AddHandler _ticker.OnError, AddressOf OnError
        AddHandler _ticker.OnClose, AddressOf OnClose
        AddHandler _ticker.OnConnect, AddressOf OnConnect
        AddHandler _ticker.OnOrderUpdate, AddressOf OnOrderUpdate

        _ticker.EnableReconnect(Interval:=5, Retries:=50)
        _ticker.Connect()

        If _todaysInstrumentsForOHLStrategy IsNot Nothing AndAlso _todaysInstrumentsForOHLStrategy.Count > 0 Then
            For Each token In _todaysInstrumentsForOHLStrategy.Keys
                _todaysInstrumentsForOHLStrategy(token).StrategyWorker.ConsumedOrderUpdateAsync(Nothing)
                _ticker.Subscribe(Tokens:=New UInt32() {token})
                _ticker.SetMode(Tokens:=New UInt32() {token}, Mode:=Constants.MODE_FULL)
            Next
        Else
            Throw New ApplicationException("No instrument available that could be subscribed for ticks")
        End If
    End Sub
    Private Async Function TestCheck() As Task
        Dim ret As Dictionary(Of String, Object) = Nothing

        'Dim tradeParameters As New Dictionary(Of String, Object)
        'tradeParameters.Add("Exchange", Constants.EXCHANGE_NSE)
        'tradeParameters.Add("TradingSymbol", "BPCL")
        'tradeParameters.Add("TransactionType", Constants.TRANSACTION_TYPE_BUY)
        'tradeParameters.Add("Quantity", 1)
        'tradeParameters.Add("Price", 329)
        'tradeParameters.Add("Product", Constants.PRODUCT_MIS)
        'tradeParameters.Add("OrderType", Constants.ORDER_TYPE_LIMIT)
        'tradeParameters.Add("Validity", Constants.VALIDITY_DAY)
        'tradeParameters.Add("SquareOffValue", 3)
        'tradeParameters.Add("StoplossValue", 3)
        'tradeParameters.Add("Variety", Constants.VARIETY_BO)
        'tradeParameters.Add("Tag", "Algo2Trade_OHLStrategy")
        'ret = Await _zerodhaKite.ExecuteCommandAsync(ZerodhaKiteHelper.KiteCommands.PlaceOrder, tradeParameters).ConfigureAwait(False)
        'OnHeartbeat(Utils.JsonSerialize(ret(ZerodhaKiteHelper.KiteCommands.PlaceOrder.ToString)))

        Dim checkParameters As New Dictionary(Of String, Object)
        checkParameters.Add("OrderId", "181212002417114")
        checkParameters.Add("ParentOrderId", "181212002231823")
        checkParameters.Add("Price", 284.6)
        checkParameters.Add("TriggerPrice", 326.45)
        checkParameters.Add("Variety", Constants.VARIETY_BO)

        'ret = Await _zerodhaKite.ExecuteCommandAsync(ZerodhaKiteHelper.KiteCommands.ModifySLOrderPrice, checkParameters).ConfigureAwait(False)
        'OnHeartbeat(Utils.JsonSerialize(ret(ZerodhaKiteHelper.KiteCommands.ModifySLOrderPrice.ToString)))
        'ret = Await _zerodhaKite.ExecuteCommandAsync(ZerodhaKiteHelper.KiteCommands.CancelOrder, checkParameters).ConfigureAwait(False)
        'OnHeartbeat(Utils.JsonSerialize(ret(ZerodhaKiteHelper.KiteCommands.CancelOrder.ToString)))

        'ret = Await _zerodhaKite.ExecuteCommandAsync(ZerodhaKiteHelper.KiteCommands.GetOrderHistory, checkParameters).ConfigureAwait(False)
        'OnHeartbeat(Utils.JsonSerialize(ret(ZerodhaKiteHelper.KiteCommands.GetOrderHistory.ToString)))
        ret = Await _zerodhaKite.ExecuteCommandAsync(ZerodhaKiteHelper.KiteCommands.GetOrders, Nothing).ConfigureAwait(False)
        OnHeartbeat(Utils.JsonSerialize(ret(ZerodhaKiteHelper.KiteCommands.GetOrders.ToString)))
        ret = Await _zerodhaKite.ExecuteCommandAsync(ZerodhaKiteHelper.KiteCommands.GetOrderTrades, Nothing).ConfigureAwait(False)
        OnHeartbeat(Utils.JsonSerialize(ret(ZerodhaKiteHelper.KiteCommands.GetOrderTrades.ToString)))
    End Function

#Region "Event Handler"
    Private Sub OnConnect()
        OnHeartbeat("Connected Ticker")
    End Sub
    Private Sub OnClose()
        OnHeartbeat("Closed Ticker")
    End Sub
    Private Sub OnError(message As String)
        OnHeartbeat(String.Format("Error: {0}", message))
    End Sub
    Private Sub OnNoReconnect()
        OnHeartbeat("Not Reconnecting")
    End Sub
    Private Sub OnReconnect()
        OnHeartbeat("Reconnecting")
    End Sub
    Private Async Sub OnTick(tickData As Tick)
        Try
            _canceller.Token.ThrowIfCancellationRequested()
        Catch oex As OperationCanceledException
            _ticker.Close()
            logger.Error(oex)
            MsgBox(String.Format("The following error occurred: {0}", oex.Message), MsgBoxStyle.Critical)
            InitUI(True)
        End Try

        If _todaysInstrumentsForOHLStrategy IsNot Nothing AndAlso _todaysInstrumentsForOHLStrategy.Count > 0 Then
            _todaysInstrumentsForOHLStrategy(tickData.InstrumentToken).StrategyWorker.ConsumedTickDataAsync(tickData)
        End If

        Console.WriteLine(Utils.JsonSerialize(tickData))
        OnHeartbeat(String.Format("Fetching Tick Data: Instrument Name: {0}, Time: {1}",
                                  _todaysInstrumentsForOHLStrategy(tickData.InstrumentToken).WrappedInstrument.TradingSymbol,
                                  If(tickData.Mode = "full", tickData.Timestamp.Value.ToString, Now)))
    End Sub
    Private Async Sub OnOrderUpdate(orderData As Order)
        If _todaysInstrumentsForOHLStrategy IsNot Nothing AndAlso _todaysInstrumentsForOHLStrategy.Count > 0 Then
            _todaysInstrumentsForOHLStrategy(orderData.InstrumentToken).StrategyWorker.ConsumedOrderUpdateAsync(orderData)
        End If
        OnHeartbeat(String.Format("OrderUpdate {0}", Utils.JsonSerialize(orderData)))
    End Sub
    Private Sub OnHeartbeat(message As String)
        SetLabelText_ThreadSafe(lblSelectedStocksProgress, String.Format("{0}", message))
        Console.WriteLine(String.Format("{0}{1}System Time:{2}", message, vbNewLine, Now))
        logger.Debug(message)
    End Sub
    Private Sub OnDocumentDownloadComplete()
        OnHeartbeat("Document download compelete")
    End Sub
    Private Sub OnDocumentRetryStatus(currentTry As Integer, totalTries As Integer)
        OnHeartbeat(String.Format("Try #{0}/{1}: Connecting...", currentTry, totalTries))
    End Sub
    Public Sub OnWaitingFor(ByVal elapsedSecs As Integer, ByVal totalSecs As Integer, ByVal msg As String)
        OnHeartbeat(String.Format("{0}, waiting {1}/{2} secs", msg, elapsedSecs, totalSecs))
    End Sub
#End Region
End Class
