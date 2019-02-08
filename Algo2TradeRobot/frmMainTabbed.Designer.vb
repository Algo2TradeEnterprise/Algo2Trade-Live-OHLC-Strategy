<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmMainTabbed
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmMainTabbed))
        Me.msMainMenuStrip = New System.Windows.Forms.MenuStrip()
        Me.miSecurity = New System.Windows.Forms.ToolStripMenuItem()
        Me.miUserDetails = New System.Windows.Forms.ToolStripMenuItem()
        Me.miAbout = New System.Windows.Forms.ToolStripMenuItem()
        Me.tabMain = New System.Windows.Forms.TabControl()
        Me.tabMomentumReversal = New System.Windows.Forms.TabPage()
        Me.pnlMomentumReversalMainPanelHorizontalSplitter = New System.Windows.Forms.TableLayoutPanel()
        Me.pnlMomentumReversalTopHeaderVerticalSplitter = New System.Windows.Forms.TableLayoutPanel()
        Me.btnMomentumReversalStop = New System.Windows.Forms.Button()
        Me.btnMomentumReversalStart = New System.Windows.Forms.Button()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.blbMomentumReversalTickerStatus = New Bulb.LedBulb()
        Me.lblMomentumReversalTickerStatus = New System.Windows.Forms.Label()
        Me.pnlMomentumReversalBodyVerticalSplitter = New System.Windows.Forms.TableLayoutPanel()
        Me.pnlMomentumReversalBodyHorizontalSplitter = New System.Windows.Forms.TableLayoutPanel()
        Me.lstMomentumReversalLog = New System.Windows.Forms.ListBox()
        Me.sfdgvMomentumReversalMainDashboard = New Syncfusion.WinForms.DataGrid.SfDataGrid()
        Me.tabOHL = New System.Windows.Forms.TabPage()
        Me.pnlOHLMainPanelHorizontalSplitter = New System.Windows.Forms.TableLayoutPanel()
        Me.pnlOHLTopHeaderVerticalSplitter = New System.Windows.Forms.TableLayoutPanel()
        Me.btnOHLStop = New System.Windows.Forms.Button()
        Me.btnOHLStart = New System.Windows.Forms.Button()
        Me.Panel2 = New System.Windows.Forms.Panel()
        Me.blbOHLTickerStatus = New Bulb.LedBulb()
        Me.lblOHLTickerStatus = New System.Windows.Forms.Label()
        Me.pnlOHLBodyVerticalSplitter = New System.Windows.Forms.TableLayoutPanel()
        Me.pnlOHLBodyHorizontalSplitter = New System.Windows.Forms.TableLayoutPanel()
        Me.lstOHLLog = New System.Windows.Forms.ListBox()
        Me.sfdgvOHLMainDashboard = New Syncfusion.WinForms.DataGrid.SfDataGrid()
        Me.tmrMomentumReversalTickerStatus = New System.Windows.Forms.Timer(Me.components)
        Me.tmrOHLTickerStatus = New System.Windows.Forms.Timer(Me.components)
        Me.btnMomentumReversalSettings = New System.Windows.Forms.Button()
        Me.msMainMenuStrip.SuspendLayout()
        Me.tabMain.SuspendLayout()
        Me.tabMomentumReversal.SuspendLayout()
        Me.pnlMomentumReversalMainPanelHorizontalSplitter.SuspendLayout()
        Me.pnlMomentumReversalTopHeaderVerticalSplitter.SuspendLayout()
        Me.Panel1.SuspendLayout()
        Me.pnlMomentumReversalBodyVerticalSplitter.SuspendLayout()
        Me.pnlMomentumReversalBodyHorizontalSplitter.SuspendLayout()
        CType(Me.sfdgvMomentumReversalMainDashboard, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.tabOHL.SuspendLayout()
        Me.pnlOHLMainPanelHorizontalSplitter.SuspendLayout()
        Me.pnlOHLTopHeaderVerticalSplitter.SuspendLayout()
        Me.Panel2.SuspendLayout()
        Me.pnlOHLBodyVerticalSplitter.SuspendLayout()
        Me.pnlOHLBodyHorizontalSplitter.SuspendLayout()
        CType(Me.sfdgvOHLMainDashboard, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'msMainMenuStrip
        '
        Me.msMainMenuStrip.ImageScalingSize = New System.Drawing.Size(20, 20)
        Me.msMainMenuStrip.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.miSecurity, Me.miAbout})
        Me.msMainMenuStrip.Location = New System.Drawing.Point(0, 0)
        Me.msMainMenuStrip.Name = "msMainMenuStrip"
        Me.msMainMenuStrip.Size = New System.Drawing.Size(1028, 24)
        Me.msMainMenuStrip.TabIndex = 0
        Me.msMainMenuStrip.Text = "MenuStrip1"
        '
        'miSecurity
        '
        Me.miSecurity.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.miUserDetails})
        Me.miSecurity.Name = "miSecurity"
        Me.miSecurity.Size = New System.Drawing.Size(61, 20)
        Me.miSecurity.Text = "Security"
        '
        'miUserDetails
        '
        Me.miUserDetails.Name = "miUserDetails"
        Me.miUserDetails.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.F2), System.Windows.Forms.Keys)
        Me.miUserDetails.Size = New System.Drawing.Size(181, 22)
        Me.miUserDetails.Text = "User Details"
        '
        'miAbout
        '
        Me.miAbout.Name = "miAbout"
        Me.miAbout.Size = New System.Drawing.Size(52, 20)
        Me.miAbout.Text = "About"
        '
        'tabMain
        '
        Me.tabMain.Controls.Add(Me.tabMomentumReversal)
        Me.tabMain.Controls.Add(Me.tabOHL)
        Me.tabMain.Dock = System.Windows.Forms.DockStyle.Fill
        Me.tabMain.Location = New System.Drawing.Point(0, 24)
        Me.tabMain.Name = "tabMain"
        Me.tabMain.SelectedIndex = 0
        Me.tabMain.Size = New System.Drawing.Size(1028, 585)
        Me.tabMain.TabIndex = 1
        '
        'tabMomentumReversal
        '
        Me.tabMomentumReversal.Controls.Add(Me.pnlMomentumReversalMainPanelHorizontalSplitter)
        Me.tabMomentumReversal.Location = New System.Drawing.Point(4, 22)
        Me.tabMomentumReversal.Name = "tabMomentumReversal"
        Me.tabMomentumReversal.Padding = New System.Windows.Forms.Padding(3, 3, 3, 3)
        Me.tabMomentumReversal.Size = New System.Drawing.Size(1020, 559)
        Me.tabMomentumReversal.TabIndex = 0
        Me.tabMomentumReversal.Text = "Momentum Reversal"
        Me.tabMomentumReversal.UseVisualStyleBackColor = True
        '
        'pnlMomentumReversalMainPanelHorizontalSplitter
        '
        Me.pnlMomentumReversalMainPanelHorizontalSplitter.ColumnCount = 1
        Me.pnlMomentumReversalMainPanelHorizontalSplitter.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.pnlMomentumReversalMainPanelHorizontalSplitter.Controls.Add(Me.pnlMomentumReversalTopHeaderVerticalSplitter, 0, 0)
        Me.pnlMomentumReversalMainPanelHorizontalSplitter.Controls.Add(Me.pnlMomentumReversalBodyVerticalSplitter, 0, 1)
        Me.pnlMomentumReversalMainPanelHorizontalSplitter.Dock = System.Windows.Forms.DockStyle.Fill
        Me.pnlMomentumReversalMainPanelHorizontalSplitter.Location = New System.Drawing.Point(3, 3)
        Me.pnlMomentumReversalMainPanelHorizontalSplitter.Name = "pnlMomentumReversalMainPanelHorizontalSplitter"
        Me.pnlMomentumReversalMainPanelHorizontalSplitter.RowCount = 2
        Me.pnlMomentumReversalMainPanelHorizontalSplitter.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 7.0!))
        Me.pnlMomentumReversalMainPanelHorizontalSplitter.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 93.0!))
        Me.pnlMomentumReversalMainPanelHorizontalSplitter.Size = New System.Drawing.Size(1014, 553)
        Me.pnlMomentumReversalMainPanelHorizontalSplitter.TabIndex = 0
        '
        'pnlMomentumReversalTopHeaderVerticalSplitter
        '
        Me.pnlMomentumReversalTopHeaderVerticalSplitter.ColumnCount = 15
        Me.pnlMomentumReversalTopHeaderVerticalSplitter.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 6.666668!))
        Me.pnlMomentumReversalTopHeaderVerticalSplitter.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 6.666668!))
        Me.pnlMomentumReversalTopHeaderVerticalSplitter.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 6.666668!))
        Me.pnlMomentumReversalTopHeaderVerticalSplitter.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 6.666668!))
        Me.pnlMomentumReversalTopHeaderVerticalSplitter.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 6.666668!))
        Me.pnlMomentumReversalTopHeaderVerticalSplitter.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 6.666668!))
        Me.pnlMomentumReversalTopHeaderVerticalSplitter.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 6.666668!))
        Me.pnlMomentumReversalTopHeaderVerticalSplitter.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 6.666668!))
        Me.pnlMomentumReversalTopHeaderVerticalSplitter.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 6.666668!))
        Me.pnlMomentumReversalTopHeaderVerticalSplitter.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 6.666668!))
        Me.pnlMomentumReversalTopHeaderVerticalSplitter.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 6.666668!))
        Me.pnlMomentumReversalTopHeaderVerticalSplitter.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 6.666668!))
        Me.pnlMomentumReversalTopHeaderVerticalSplitter.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 6.666668!))
        Me.pnlMomentumReversalTopHeaderVerticalSplitter.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 2.741935!))
        Me.pnlMomentumReversalTopHeaderVerticalSplitter.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 11.29032!))
        Me.pnlMomentumReversalTopHeaderVerticalSplitter.Controls.Add(Me.btnMomentumReversalStop, 0, 0)
        Me.pnlMomentumReversalTopHeaderVerticalSplitter.Controls.Add(Me.btnMomentumReversalStart, 0, 0)
        Me.pnlMomentumReversalTopHeaderVerticalSplitter.Controls.Add(Me.Panel1, 14, 0)
        Me.pnlMomentumReversalTopHeaderVerticalSplitter.Controls.Add(Me.btnMomentumReversalSettings, 9, 0)
        Me.pnlMomentumReversalTopHeaderVerticalSplitter.Dock = System.Windows.Forms.DockStyle.Fill
        Me.pnlMomentumReversalTopHeaderVerticalSplitter.Location = New System.Drawing.Point(3, 3)
        Me.pnlMomentumReversalTopHeaderVerticalSplitter.Name = "pnlMomentumReversalTopHeaderVerticalSplitter"
        Me.pnlMomentumReversalTopHeaderVerticalSplitter.RowCount = 1
        Me.pnlMomentumReversalTopHeaderVerticalSplitter.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.pnlMomentumReversalTopHeaderVerticalSplitter.Size = New System.Drawing.Size(1008, 32)
        Me.pnlMomentumReversalTopHeaderVerticalSplitter.TabIndex = 0
        '
        'btnMomentumReversalStop
        '
        Me.btnMomentumReversalStop.Dock = System.Windows.Forms.DockStyle.Fill
        Me.btnMomentumReversalStop.Location = New System.Drawing.Point(69, 3)
        Me.btnMomentumReversalStop.Name = "btnMomentumReversalStop"
        Me.btnMomentumReversalStop.Size = New System.Drawing.Size(60, 26)
        Me.btnMomentumReversalStop.TabIndex = 10
        Me.btnMomentumReversalStop.Text = "Stop"
        Me.btnMomentumReversalStop.UseVisualStyleBackColor = True
        '
        'btnMomentumReversalStart
        '
        Me.btnMomentumReversalStart.Dock = System.Windows.Forms.DockStyle.Fill
        Me.btnMomentumReversalStart.Location = New System.Drawing.Point(3, 3)
        Me.btnMomentumReversalStart.Name = "btnMomentumReversalStart"
        Me.btnMomentumReversalStart.Size = New System.Drawing.Size(60, 26)
        Me.btnMomentumReversalStart.TabIndex = 2
        Me.btnMomentumReversalStart.Text = "Start"
        Me.btnMomentumReversalStart.UseVisualStyleBackColor = True
        '
        'Panel1
        '
        Me.Panel1.Controls.Add(Me.blbMomentumReversalTickerStatus)
        Me.Panel1.Controls.Add(Me.lblMomentumReversalTickerStatus)
        Me.Panel1.Location = New System.Drawing.Point(888, 3)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(117, 26)
        Me.Panel1.TabIndex = 9
        '
        'blbMomentumReversalTickerStatus
        '
        Me.blbMomentumReversalTickerStatus.Color = System.Drawing.Color.Pink
        Me.blbMomentumReversalTickerStatus.Dock = System.Windows.Forms.DockStyle.Right
        Me.blbMomentumReversalTickerStatus.Location = New System.Drawing.Point(82, 0)
        Me.blbMomentumReversalTickerStatus.Name = "blbMomentumReversalTickerStatus"
        Me.blbMomentumReversalTickerStatus.On = True
        Me.blbMomentumReversalTickerStatus.Size = New System.Drawing.Size(35, 26)
        Me.blbMomentumReversalTickerStatus.TabIndex = 7
        Me.blbMomentumReversalTickerStatus.Text = "LedBulb1"
        '
        'lblMomentumReversalTickerStatus
        '
        Me.lblMomentumReversalTickerStatus.AutoSize = True
        Me.lblMomentumReversalTickerStatus.Location = New System.Drawing.Point(24, 10)
        Me.lblMomentumReversalTickerStatus.Name = "lblMomentumReversalTickerStatus"
        Me.lblMomentumReversalTickerStatus.Size = New System.Drawing.Size(70, 13)
        Me.lblMomentumReversalTickerStatus.TabIndex = 9
        Me.lblMomentumReversalTickerStatus.Text = "Ticker Status"
        '
        'pnlMomentumReversalBodyVerticalSplitter
        '
        Me.pnlMomentumReversalBodyVerticalSplitter.ColumnCount = 2
        Me.pnlMomentumReversalBodyVerticalSplitter.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 70.0!))
        Me.pnlMomentumReversalBodyVerticalSplitter.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30.0!))
        Me.pnlMomentumReversalBodyVerticalSplitter.Controls.Add(Me.pnlMomentumReversalBodyHorizontalSplitter, 0, 0)
        Me.pnlMomentumReversalBodyVerticalSplitter.Dock = System.Windows.Forms.DockStyle.Fill
        Me.pnlMomentumReversalBodyVerticalSplitter.Location = New System.Drawing.Point(3, 41)
        Me.pnlMomentumReversalBodyVerticalSplitter.Name = "pnlMomentumReversalBodyVerticalSplitter"
        Me.pnlMomentumReversalBodyVerticalSplitter.RowCount = 1
        Me.pnlMomentumReversalBodyVerticalSplitter.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.pnlMomentumReversalBodyVerticalSplitter.Size = New System.Drawing.Size(1008, 509)
        Me.pnlMomentumReversalBodyVerticalSplitter.TabIndex = 1
        '
        'pnlMomentumReversalBodyHorizontalSplitter
        '
        Me.pnlMomentumReversalBodyHorizontalSplitter.ColumnCount = 1
        Me.pnlMomentumReversalBodyHorizontalSplitter.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.pnlMomentumReversalBodyHorizontalSplitter.Controls.Add(Me.lstMomentumReversalLog, 0, 1)
        Me.pnlMomentumReversalBodyHorizontalSplitter.Controls.Add(Me.sfdgvMomentumReversalMainDashboard, 0, 0)
        Me.pnlMomentumReversalBodyHorizontalSplitter.Dock = System.Windows.Forms.DockStyle.Fill
        Me.pnlMomentumReversalBodyHorizontalSplitter.Location = New System.Drawing.Point(3, 3)
        Me.pnlMomentumReversalBodyHorizontalSplitter.Name = "pnlMomentumReversalBodyHorizontalSplitter"
        Me.pnlMomentumReversalBodyHorizontalSplitter.RowCount = 2
        Me.pnlMomentumReversalBodyHorizontalSplitter.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 70.0!))
        Me.pnlMomentumReversalBodyHorizontalSplitter.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 30.0!))
        Me.pnlMomentumReversalBodyHorizontalSplitter.Size = New System.Drawing.Size(699, 503)
        Me.pnlMomentumReversalBodyHorizontalSplitter.TabIndex = 0
        '
        'lstMomentumReversalLog
        '
        Me.lstMomentumReversalLog.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lstMomentumReversalLog.ForeColor = System.Drawing.Color.FromArgb(CType(CType(29, Byte), Integer), CType(CType(29, Byte), Integer), CType(CType(29, Byte), Integer))
        Me.lstMomentumReversalLog.FormattingEnabled = True
        Me.lstMomentumReversalLog.Location = New System.Drawing.Point(3, 355)
        Me.lstMomentumReversalLog.Name = "lstMomentumReversalLog"
        Me.lstMomentumReversalLog.Size = New System.Drawing.Size(693, 145)
        Me.lstMomentumReversalLog.TabIndex = 9
        '
        'sfdgvMomentumReversalMainDashboard
        '
        Me.sfdgvMomentumReversalMainDashboard.AccessibleName = "Table"
        Me.sfdgvMomentumReversalMainDashboard.AllowDraggingColumns = True
        Me.sfdgvMomentumReversalMainDashboard.AllowEditing = False
        Me.sfdgvMomentumReversalMainDashboard.AllowFiltering = True
        Me.sfdgvMomentumReversalMainDashboard.AllowResizingColumns = True
        Me.sfdgvMomentumReversalMainDashboard.AutoGenerateColumnsMode = Syncfusion.WinForms.DataGrid.Enums.AutoGenerateColumnsMode.SmartReset
        Me.sfdgvMomentumReversalMainDashboard.AutoSizeColumnsMode = Syncfusion.WinForms.DataGrid.Enums.AutoSizeColumnsMode.AllCells
        Me.sfdgvMomentumReversalMainDashboard.Dock = System.Windows.Forms.DockStyle.Fill
        Me.sfdgvMomentumReversalMainDashboard.Location = New System.Drawing.Point(3, 3)
        Me.sfdgvMomentumReversalMainDashboard.Name = "sfdgvMomentumReversalMainDashboard"
        Me.sfdgvMomentumReversalMainDashboard.PasteOption = Syncfusion.WinForms.DataGrid.Enums.PasteOptions.None
        Me.sfdgvMomentumReversalMainDashboard.Size = New System.Drawing.Size(693, 346)
        Me.sfdgvMomentumReversalMainDashboard.TabIndex = 6
        Me.sfdgvMomentumReversalMainDashboard.Text = "SfDataGrid1"
        '
        'tabOHL
        '
        Me.tabOHL.Controls.Add(Me.pnlOHLMainPanelHorizontalSplitter)
        Me.tabOHL.Location = New System.Drawing.Point(4, 22)
        Me.tabOHL.Name = "tabOHL"
        Me.tabOHL.Padding = New System.Windows.Forms.Padding(3, 3, 3, 3)
        Me.tabOHL.Size = New System.Drawing.Size(1252, 636)
        Me.tabOHL.TabIndex = 1
        Me.tabOHL.Text = "OHL"
        Me.tabOHL.UseVisualStyleBackColor = True
        '
        'pnlOHLMainPanelHorizontalSplitter
        '
        Me.pnlOHLMainPanelHorizontalSplitter.ColumnCount = 1
        Me.pnlOHLMainPanelHorizontalSplitter.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.pnlOHLMainPanelHorizontalSplitter.Controls.Add(Me.pnlOHLTopHeaderVerticalSplitter, 0, 0)
        Me.pnlOHLMainPanelHorizontalSplitter.Controls.Add(Me.pnlOHLBodyVerticalSplitter, 0, 1)
        Me.pnlOHLMainPanelHorizontalSplitter.Dock = System.Windows.Forms.DockStyle.Fill
        Me.pnlOHLMainPanelHorizontalSplitter.Location = New System.Drawing.Point(3, 3)
        Me.pnlOHLMainPanelHorizontalSplitter.Name = "pnlOHLMainPanelHorizontalSplitter"
        Me.pnlOHLMainPanelHorizontalSplitter.RowCount = 2
        Me.pnlOHLMainPanelHorizontalSplitter.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 7.0!))
        Me.pnlOHLMainPanelHorizontalSplitter.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 93.0!))
        Me.pnlOHLMainPanelHorizontalSplitter.Size = New System.Drawing.Size(1246, 630)
        Me.pnlOHLMainPanelHorizontalSplitter.TabIndex = 1
        '
        'pnlOHLTopHeaderVerticalSplitter
        '
        Me.pnlOHLTopHeaderVerticalSplitter.ColumnCount = 15
        Me.pnlOHLTopHeaderVerticalSplitter.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 6.666668!))
        Me.pnlOHLTopHeaderVerticalSplitter.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 6.666668!))
        Me.pnlOHLTopHeaderVerticalSplitter.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 6.666668!))
        Me.pnlOHLTopHeaderVerticalSplitter.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 6.666668!))
        Me.pnlOHLTopHeaderVerticalSplitter.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 6.666668!))
        Me.pnlOHLTopHeaderVerticalSplitter.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 6.666668!))
        Me.pnlOHLTopHeaderVerticalSplitter.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 6.666668!))
        Me.pnlOHLTopHeaderVerticalSplitter.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 6.666668!))
        Me.pnlOHLTopHeaderVerticalSplitter.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 6.666668!))
        Me.pnlOHLTopHeaderVerticalSplitter.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 6.666668!))
        Me.pnlOHLTopHeaderVerticalSplitter.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 6.666668!))
        Me.pnlOHLTopHeaderVerticalSplitter.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 6.666668!))
        Me.pnlOHLTopHeaderVerticalSplitter.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 6.666668!))
        Me.pnlOHLTopHeaderVerticalSplitter.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 2.741935!))
        Me.pnlOHLTopHeaderVerticalSplitter.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 11.29032!))
        Me.pnlOHLTopHeaderVerticalSplitter.Controls.Add(Me.btnOHLStop, 0, 0)
        Me.pnlOHLTopHeaderVerticalSplitter.Controls.Add(Me.btnOHLStart, 0, 0)
        Me.pnlOHLTopHeaderVerticalSplitter.Controls.Add(Me.Panel2, 14, 0)
        Me.pnlOHLTopHeaderVerticalSplitter.Dock = System.Windows.Forms.DockStyle.Fill
        Me.pnlOHLTopHeaderVerticalSplitter.Location = New System.Drawing.Point(3, 3)
        Me.pnlOHLTopHeaderVerticalSplitter.Name = "pnlOHLTopHeaderVerticalSplitter"
        Me.pnlOHLTopHeaderVerticalSplitter.RowCount = 1
        Me.pnlOHLTopHeaderVerticalSplitter.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.pnlOHLTopHeaderVerticalSplitter.Size = New System.Drawing.Size(1240, 38)
        Me.pnlOHLTopHeaderVerticalSplitter.TabIndex = 0
        '
        'btnOHLStop
        '
        Me.btnOHLStop.Dock = System.Windows.Forms.DockStyle.Fill
        Me.btnOHLStop.Location = New System.Drawing.Point(85, 3)
        Me.btnOHLStop.Name = "btnOHLStop"
        Me.btnOHLStop.Size = New System.Drawing.Size(76, 32)
        Me.btnOHLStop.TabIndex = 11
        Me.btnOHLStop.Text = "Stop"
        Me.btnOHLStop.UseVisualStyleBackColor = True
        '
        'btnOHLStart
        '
        Me.btnOHLStart.Dock = System.Windows.Forms.DockStyle.Fill
        Me.btnOHLStart.Location = New System.Drawing.Point(3, 3)
        Me.btnOHLStart.Name = "btnOHLStart"
        Me.btnOHLStart.Size = New System.Drawing.Size(76, 32)
        Me.btnOHLStart.TabIndex = 2
        Me.btnOHLStart.Text = "Start"
        Me.btnOHLStart.UseVisualStyleBackColor = True
        '
        'Panel2
        '
        Me.Panel2.Controls.Add(Me.blbOHLTickerStatus)
        Me.Panel2.Controls.Add(Me.lblOHLTickerStatus)
        Me.Panel2.Location = New System.Drawing.Point(1102, 3)
        Me.Panel2.Name = "Panel2"
        Me.Panel2.Size = New System.Drawing.Size(135, 31)
        Me.Panel2.TabIndex = 9
        '
        'blbOHLTickerStatus
        '
        Me.blbOHLTickerStatus.Color = System.Drawing.Color.Pink
        Me.blbOHLTickerStatus.Dock = System.Windows.Forms.DockStyle.Right
        Me.blbOHLTickerStatus.Location = New System.Drawing.Point(100, 0)
        Me.blbOHLTickerStatus.Name = "blbOHLTickerStatus"
        Me.blbOHLTickerStatus.On = True
        Me.blbOHLTickerStatus.Size = New System.Drawing.Size(35, 31)
        Me.blbOHLTickerStatus.TabIndex = 7
        Me.blbOHLTickerStatus.Text = "LedBulb1"
        '
        'lblOHLTickerStatus
        '
        Me.lblOHLTickerStatus.AutoSize = True
        Me.lblOHLTickerStatus.Location = New System.Drawing.Point(24, 10)
        Me.lblOHLTickerStatus.Name = "lblOHLTickerStatus"
        Me.lblOHLTickerStatus.Size = New System.Drawing.Size(70, 13)
        Me.lblOHLTickerStatus.TabIndex = 9
        Me.lblOHLTickerStatus.Text = "Ticker Status"
        '
        'pnlOHLBodyVerticalSplitter
        '
        Me.pnlOHLBodyVerticalSplitter.ColumnCount = 2
        Me.pnlOHLBodyVerticalSplitter.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 70.0!))
        Me.pnlOHLBodyVerticalSplitter.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30.0!))
        Me.pnlOHLBodyVerticalSplitter.Controls.Add(Me.pnlOHLBodyHorizontalSplitter, 0, 0)
        Me.pnlOHLBodyVerticalSplitter.Dock = System.Windows.Forms.DockStyle.Fill
        Me.pnlOHLBodyVerticalSplitter.Location = New System.Drawing.Point(3, 47)
        Me.pnlOHLBodyVerticalSplitter.Name = "pnlOHLBodyVerticalSplitter"
        Me.pnlOHLBodyVerticalSplitter.RowCount = 1
        Me.pnlOHLBodyVerticalSplitter.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.pnlOHLBodyVerticalSplitter.Size = New System.Drawing.Size(1240, 580)
        Me.pnlOHLBodyVerticalSplitter.TabIndex = 1
        '
        'pnlOHLBodyHorizontalSplitter
        '
        Me.pnlOHLBodyHorizontalSplitter.ColumnCount = 1
        Me.pnlOHLBodyHorizontalSplitter.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.pnlOHLBodyHorizontalSplitter.Controls.Add(Me.lstOHLLog, 0, 1)
        Me.pnlOHLBodyHorizontalSplitter.Controls.Add(Me.sfdgvOHLMainDashboard, 0, 0)
        Me.pnlOHLBodyHorizontalSplitter.Dock = System.Windows.Forms.DockStyle.Fill
        Me.pnlOHLBodyHorizontalSplitter.Location = New System.Drawing.Point(3, 3)
        Me.pnlOHLBodyHorizontalSplitter.Name = "pnlOHLBodyHorizontalSplitter"
        Me.pnlOHLBodyHorizontalSplitter.RowCount = 2
        Me.pnlOHLBodyHorizontalSplitter.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 70.0!))
        Me.pnlOHLBodyHorizontalSplitter.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 30.0!))
        Me.pnlOHLBodyHorizontalSplitter.Size = New System.Drawing.Size(862, 574)
        Me.pnlOHLBodyHorizontalSplitter.TabIndex = 0
        '
        'lstOHLLog
        '
        Me.lstOHLLog.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lstOHLLog.ForeColor = System.Drawing.Color.FromArgb(CType(CType(29, Byte), Integer), CType(CType(29, Byte), Integer), CType(CType(29, Byte), Integer))
        Me.lstOHLLog.FormattingEnabled = True
        Me.lstOHLLog.Location = New System.Drawing.Point(3, 404)
        Me.lstOHLLog.Name = "lstOHLLog"
        Me.lstOHLLog.Size = New System.Drawing.Size(856, 167)
        Me.lstOHLLog.TabIndex = 9
        '
        'sfdgvOHLMainDashboard
        '
        Me.sfdgvOHLMainDashboard.AccessibleName = "Table"
        Me.sfdgvOHLMainDashboard.AllowDraggingColumns = True
        Me.sfdgvOHLMainDashboard.AllowEditing = False
        Me.sfdgvOHLMainDashboard.AllowFiltering = True
        Me.sfdgvOHLMainDashboard.AllowResizingColumns = True
        Me.sfdgvOHLMainDashboard.AutoGenerateColumnsMode = Syncfusion.WinForms.DataGrid.Enums.AutoGenerateColumnsMode.SmartReset
        Me.sfdgvOHLMainDashboard.AutoSizeColumnsMode = Syncfusion.WinForms.DataGrid.Enums.AutoSizeColumnsMode.AllCells
        Me.sfdgvOHLMainDashboard.Dock = System.Windows.Forms.DockStyle.Fill
        Me.sfdgvOHLMainDashboard.Location = New System.Drawing.Point(3, 3)
        Me.sfdgvOHLMainDashboard.Name = "sfdgvOHLMainDashboard"
        Me.sfdgvOHLMainDashboard.PasteOption = Syncfusion.WinForms.DataGrid.Enums.PasteOptions.None
        Me.sfdgvOHLMainDashboard.Size = New System.Drawing.Size(856, 395)
        Me.sfdgvOHLMainDashboard.TabIndex = 6
        Me.sfdgvOHLMainDashboard.Text = "SfDataGrid1"
        '
        'tmrMomentumReversalTickerStatus
        '
        Me.tmrMomentumReversalTickerStatus.Enabled = True
        '
        'tmrOHLTickerStatus
        '
        Me.tmrOHLTickerStatus.Enabled = True
        '
        'btnMomentumReversalSettings
        '
        Me.btnMomentumReversalSettings.Location = New System.Drawing.Point(597, 3)
        Me.btnMomentumReversalSettings.Name = "btnMomentumReversalSettings"
        Me.btnMomentumReversalSettings.Size = New System.Drawing.Size(60, 26)
        Me.btnMomentumReversalSettings.TabIndex = 11
        Me.btnMomentumReversalSettings.Text = "Settings"
        Me.btnMomentumReversalSettings.UseVisualStyleBackColor = True
        '
        'frmMainTabbed
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1028, 609)
        Me.Controls.Add(Me.tabMain)
        Me.Controls.Add(Me.msMainMenuStrip)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MainMenuStrip = Me.msMainMenuStrip
        Me.Name = "frmMainTabbed"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Algo2Trade Robot"
        Me.msMainMenuStrip.ResumeLayout(False)
        Me.msMainMenuStrip.PerformLayout()
        Me.tabMain.ResumeLayout(False)
        Me.tabMomentumReversal.ResumeLayout(False)
        Me.pnlMomentumReversalMainPanelHorizontalSplitter.ResumeLayout(False)
        Me.pnlMomentumReversalTopHeaderVerticalSplitter.ResumeLayout(False)
        Me.Panel1.ResumeLayout(False)
        Me.Panel1.PerformLayout()
        Me.pnlMomentumReversalBodyVerticalSplitter.ResumeLayout(False)
        Me.pnlMomentumReversalBodyHorizontalSplitter.ResumeLayout(False)
        CType(Me.sfdgvMomentumReversalMainDashboard, System.ComponentModel.ISupportInitialize).EndInit()
        Me.tabOHL.ResumeLayout(False)
        Me.pnlOHLMainPanelHorizontalSplitter.ResumeLayout(False)
        Me.pnlOHLTopHeaderVerticalSplitter.ResumeLayout(False)
        Me.Panel2.ResumeLayout(False)
        Me.Panel2.PerformLayout()
        Me.pnlOHLBodyVerticalSplitter.ResumeLayout(False)
        Me.pnlOHLBodyHorizontalSplitter.ResumeLayout(False)
        CType(Me.sfdgvOHLMainDashboard, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents msMainMenuStrip As MenuStrip
    Friend WithEvents miSecurity As ToolStripMenuItem
    Friend WithEvents miUserDetails As ToolStripMenuItem
    Friend WithEvents miAbout As ToolStripMenuItem
    Friend WithEvents tabMain As TabControl
    Friend WithEvents tabMomentumReversal As TabPage
    Friend WithEvents tabOHL As TabPage
    Friend WithEvents pnlMomentumReversalMainPanelHorizontalSplitter As TableLayoutPanel
    Friend WithEvents pnlMomentumReversalTopHeaderVerticalSplitter As TableLayoutPanel
    Friend WithEvents btnMomentumReversalStart As Button
    Friend WithEvents Panel1 As Panel
    Friend WithEvents lblMomentumReversalTickerStatus As Label
    Friend WithEvents blbMomentumReversalTickerStatus As Bulb.LedBulb
    Friend WithEvents pnlMomentumReversalBodyVerticalSplitter As TableLayoutPanel
    Friend WithEvents pnlMomentumReversalBodyHorizontalSplitter As TableLayoutPanel
    Friend WithEvents sfdgvMomentumReversalMainDashboard As Syncfusion.WinForms.DataGrid.SfDataGrid
    Friend WithEvents lstMomentumReversalLog As ListBox
    Friend WithEvents tmrMomentumReversalTickerStatus As Timer
    Friend WithEvents pnlOHLMainPanelHorizontalSplitter As TableLayoutPanel
    Friend WithEvents pnlOHLTopHeaderVerticalSplitter As TableLayoutPanel
    Friend WithEvents btnOHLStart As Button
    Friend WithEvents Panel2 As Panel
    Friend WithEvents lblOHLTickerStatus As Label
    Friend WithEvents blbOHLTickerStatus As Bulb.LedBulb
    Friend WithEvents pnlOHLBodyVerticalSplitter As TableLayoutPanel
    Friend WithEvents pnlOHLBodyHorizontalSplitter As TableLayoutPanel
    Friend WithEvents lstOHLLog As ListBox
    Friend WithEvents sfdgvOHLMainDashboard As Syncfusion.WinForms.DataGrid.SfDataGrid
    Friend WithEvents tmrOHLTickerStatus As Timer
    Friend WithEvents btnMomentumReversalStop As Button
    Friend WithEvents btnOHLStop As Button
    Friend WithEvents btnMomentumReversalSettings As Button
End Class
