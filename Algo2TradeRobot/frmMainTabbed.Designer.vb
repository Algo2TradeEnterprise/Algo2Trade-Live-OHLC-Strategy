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
        Me.btnMomentumReversalSettings = New System.Windows.Forms.Button()
        Me.pnlMomentumReversalBodyVerticalSplitter = New System.Windows.Forms.TableLayoutPanel()
        Me.PictureBox2 = New System.Windows.Forms.PictureBox()
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
        Me.PictureBox3 = New System.Windows.Forms.PictureBox()
        Me.pnlOHLBodyHorizontalSplitter = New System.Windows.Forms.TableLayoutPanel()
        Me.lstOHLLog = New System.Windows.Forms.ListBox()
        Me.sfdgvOHLMainDashboard = New Syncfusion.WinForms.DataGrid.SfDataGrid()
        Me.tabAmiSignal = New System.Windows.Forms.TabPage()
        Me.pnlAmiSignalMainPanelHorizontalSplitter = New System.Windows.Forms.TableLayoutPanel()
        Me.pnlAmiSignalTopHeaderVerticalSplitter = New System.Windows.Forms.TableLayoutPanel()
        Me.btnAmiSignalStop = New System.Windows.Forms.Button()
        Me.btnAmiSignalStart = New System.Windows.Forms.Button()
        Me.Panel3 = New System.Windows.Forms.Panel()
        Me.blbAmiSignalTickerStatus = New Bulb.LedBulb()
        Me.lblAmiSignalTickerStatus = New System.Windows.Forms.Label()
        Me.pnlAmiSignalBodyVerticalSplitter = New System.Windows.Forms.TableLayoutPanel()
        Me.pnlAmiSignalBodyHorizontalSplitter = New System.Windows.Forms.TableLayoutPanel()
        Me.lstAmiSignalLog = New System.Windows.Forms.ListBox()
        Me.sfdgvAmiSignalMainDashboard = New Syncfusion.WinForms.DataGrid.SfDataGrid()
        Me.PictureBox1 = New System.Windows.Forms.PictureBox()
        Me.tmrMomentumReversalTickerStatus = New System.Windows.Forms.Timer(Me.components)
        Me.tmrOHLTickerStatus = New System.Windows.Forms.Timer(Me.components)
        Me.tmrAmiSignalTickerStatus = New System.Windows.Forms.Timer(Me.components)
        Me.msMainMenuStrip.SuspendLayout()
        Me.tabMain.SuspendLayout()
        Me.tabMomentumReversal.SuspendLayout()
        Me.pnlMomentumReversalMainPanelHorizontalSplitter.SuspendLayout()
        Me.pnlMomentumReversalTopHeaderVerticalSplitter.SuspendLayout()
        Me.Panel1.SuspendLayout()
        Me.pnlMomentumReversalBodyVerticalSplitter.SuspendLayout()
        CType(Me.PictureBox2, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.pnlMomentumReversalBodyHorizontalSplitter.SuspendLayout()
        CType(Me.sfdgvMomentumReversalMainDashboard, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.tabOHL.SuspendLayout()
        Me.pnlOHLMainPanelHorizontalSplitter.SuspendLayout()
        Me.pnlOHLTopHeaderVerticalSplitter.SuspendLayout()
        Me.Panel2.SuspendLayout()
        Me.pnlOHLBodyVerticalSplitter.SuspendLayout()
        CType(Me.PictureBox3, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.pnlOHLBodyHorizontalSplitter.SuspendLayout()
        CType(Me.sfdgvOHLMainDashboard, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.tabAmiSignal.SuspendLayout()
        Me.pnlAmiSignalMainPanelHorizontalSplitter.SuspendLayout()
        Me.pnlAmiSignalTopHeaderVerticalSplitter.SuspendLayout()
        Me.Panel3.SuspendLayout()
        Me.pnlAmiSignalBodyVerticalSplitter.SuspendLayout()
        Me.pnlAmiSignalBodyHorizontalSplitter.SuspendLayout()
        CType(Me.sfdgvAmiSignalMainDashboard, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'msMainMenuStrip
        '
        Me.msMainMenuStrip.ImageScalingSize = New System.Drawing.Size(20, 20)
        Me.msMainMenuStrip.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.miSecurity, Me.miAbout})
        Me.msMainMenuStrip.Location = New System.Drawing.Point(0, 0)
        Me.msMainMenuStrip.Name = "msMainMenuStrip"
        Me.msMainMenuStrip.Padding = New System.Windows.Forms.Padding(8, 2, 0, 2)
        Me.msMainMenuStrip.Size = New System.Drawing.Size(1371, 28)
        Me.msMainMenuStrip.TabIndex = 0
        Me.msMainMenuStrip.Text = "MenuStrip1"
        '
        'miSecurity
        '
        Me.miSecurity.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.miUserDetails})
        Me.miSecurity.Name = "miSecurity"
        Me.miSecurity.Size = New System.Drawing.Size(73, 24)
        Me.miSecurity.Text = "Security"
        '
        'miUserDetails
        '
        Me.miUserDetails.Name = "miUserDetails"
        Me.miUserDetails.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.F2), System.Windows.Forms.Keys)
        Me.miUserDetails.Size = New System.Drawing.Size(220, 26)
        Me.miUserDetails.Text = "User Details"
        '
        'miAbout
        '
        Me.miAbout.Name = "miAbout"
        Me.miAbout.Size = New System.Drawing.Size(62, 24)
        Me.miAbout.Text = "About"
        '
        'tabMain
        '
        Me.tabMain.Controls.Add(Me.tabMomentumReversal)
        Me.tabMain.Controls.Add(Me.tabOHL)
        Me.tabMain.Controls.Add(Me.tabAmiSignal)
        Me.tabMain.Dock = System.Windows.Forms.DockStyle.Fill
        Me.tabMain.Location = New System.Drawing.Point(0, 28)
        Me.tabMain.Margin = New System.Windows.Forms.Padding(4)
        Me.tabMain.Name = "tabMain"
        Me.tabMain.SelectedIndex = 0
        Me.tabMain.Size = New System.Drawing.Size(1371, 722)
        Me.tabMain.TabIndex = 1
        '
        'tabMomentumReversal
        '
        Me.tabMomentumReversal.Controls.Add(Me.pnlMomentumReversalMainPanelHorizontalSplitter)
        Me.tabMomentumReversal.Location = New System.Drawing.Point(4, 25)
        Me.tabMomentumReversal.Margin = New System.Windows.Forms.Padding(4)
        Me.tabMomentumReversal.Name = "tabMomentumReversal"
        Me.tabMomentumReversal.Padding = New System.Windows.Forms.Padding(4)
        Me.tabMomentumReversal.Size = New System.Drawing.Size(1363, 693)
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
        Me.pnlMomentumReversalMainPanelHorizontalSplitter.Location = New System.Drawing.Point(4, 4)
        Me.pnlMomentumReversalMainPanelHorizontalSplitter.Margin = New System.Windows.Forms.Padding(4)
        Me.pnlMomentumReversalMainPanelHorizontalSplitter.Name = "pnlMomentumReversalMainPanelHorizontalSplitter"
        Me.pnlMomentumReversalMainPanelHorizontalSplitter.RowCount = 2
        Me.pnlMomentumReversalMainPanelHorizontalSplitter.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 7.0!))
        Me.pnlMomentumReversalMainPanelHorizontalSplitter.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 93.0!))
        Me.pnlMomentumReversalMainPanelHorizontalSplitter.Size = New System.Drawing.Size(1355, 685)
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
        Me.pnlMomentumReversalTopHeaderVerticalSplitter.Location = New System.Drawing.Point(4, 4)
        Me.pnlMomentumReversalTopHeaderVerticalSplitter.Margin = New System.Windows.Forms.Padding(4)
        Me.pnlMomentumReversalTopHeaderVerticalSplitter.Name = "pnlMomentumReversalTopHeaderVerticalSplitter"
        Me.pnlMomentumReversalTopHeaderVerticalSplitter.RowCount = 1
        Me.pnlMomentumReversalTopHeaderVerticalSplitter.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.pnlMomentumReversalTopHeaderVerticalSplitter.Size = New System.Drawing.Size(1347, 39)
        Me.pnlMomentumReversalTopHeaderVerticalSplitter.TabIndex = 0
        '
        'btnMomentumReversalStop
        '
        Me.btnMomentumReversalStop.Dock = System.Windows.Forms.DockStyle.Fill
        Me.btnMomentumReversalStop.Location = New System.Drawing.Point(93, 4)
        Me.btnMomentumReversalStop.Margin = New System.Windows.Forms.Padding(4)
        Me.btnMomentumReversalStop.Name = "btnMomentumReversalStop"
        Me.btnMomentumReversalStop.Size = New System.Drawing.Size(81, 31)
        Me.btnMomentumReversalStop.TabIndex = 10
        Me.btnMomentumReversalStop.Text = "Stop"
        Me.btnMomentumReversalStop.UseVisualStyleBackColor = True
        '
        'btnMomentumReversalStart
        '
        Me.btnMomentumReversalStart.Dock = System.Windows.Forms.DockStyle.Fill
        Me.btnMomentumReversalStart.Location = New System.Drawing.Point(4, 4)
        Me.btnMomentumReversalStart.Margin = New System.Windows.Forms.Padding(4)
        Me.btnMomentumReversalStart.Name = "btnMomentumReversalStart"
        Me.btnMomentumReversalStart.Size = New System.Drawing.Size(81, 31)
        Me.btnMomentumReversalStart.TabIndex = 2
        Me.btnMomentumReversalStart.Text = "Start"
        Me.btnMomentumReversalStart.UseVisualStyleBackColor = True
        '
        'Panel1
        '
        Me.Panel1.Controls.Add(Me.blbMomentumReversalTickerStatus)
        Me.Panel1.Controls.Add(Me.lblMomentumReversalTickerStatus)
        Me.Panel1.Location = New System.Drawing.Point(1197, 4)
        Me.Panel1.Margin = New System.Windows.Forms.Padding(4)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(146, 31)
        Me.Panel1.TabIndex = 9
        '
        'blbMomentumReversalTickerStatus
        '
        Me.blbMomentumReversalTickerStatus.Color = System.Drawing.Color.Pink
        Me.blbMomentumReversalTickerStatus.Dock = System.Windows.Forms.DockStyle.Right
        Me.blbMomentumReversalTickerStatus.Location = New System.Drawing.Point(99, 0)
        Me.blbMomentumReversalTickerStatus.Margin = New System.Windows.Forms.Padding(4)
        Me.blbMomentumReversalTickerStatus.Name = "blbMomentumReversalTickerStatus"
        Me.blbMomentumReversalTickerStatus.On = True
        Me.blbMomentumReversalTickerStatus.Size = New System.Drawing.Size(47, 31)
        Me.blbMomentumReversalTickerStatus.TabIndex = 7
        Me.blbMomentumReversalTickerStatus.Text = "LedBulb1"
        '
        'lblMomentumReversalTickerStatus
        '
        Me.lblMomentumReversalTickerStatus.AutoSize = True
        Me.lblMomentumReversalTickerStatus.Location = New System.Drawing.Point(9, 9)
        Me.lblMomentumReversalTickerStatus.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lblMomentumReversalTickerStatus.Name = "lblMomentumReversalTickerStatus"
        Me.lblMomentumReversalTickerStatus.Size = New System.Drawing.Size(91, 17)
        Me.lblMomentumReversalTickerStatus.TabIndex = 9
        Me.lblMomentumReversalTickerStatus.Text = "Ticker Status"
        '
        'btnMomentumReversalSettings
        '
        Me.btnMomentumReversalSettings.Dock = System.Windows.Forms.DockStyle.Fill
        Me.btnMomentumReversalSettings.Location = New System.Drawing.Point(805, 4)
        Me.btnMomentumReversalSettings.Margin = New System.Windows.Forms.Padding(4)
        Me.btnMomentumReversalSettings.Name = "btnMomentumReversalSettings"
        Me.btnMomentumReversalSettings.Size = New System.Drawing.Size(81, 31)
        Me.btnMomentumReversalSettings.TabIndex = 11
        Me.btnMomentumReversalSettings.Text = "Settings"
        Me.btnMomentumReversalSettings.UseVisualStyleBackColor = True
        '
        'pnlMomentumReversalBodyVerticalSplitter
        '
        Me.pnlMomentumReversalBodyVerticalSplitter.ColumnCount = 2
        Me.pnlMomentumReversalBodyVerticalSplitter.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 70.0!))
        Me.pnlMomentumReversalBodyVerticalSplitter.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30.0!))
        Me.pnlMomentumReversalBodyVerticalSplitter.Controls.Add(Me.PictureBox2, 0, 0)
        Me.pnlMomentumReversalBodyVerticalSplitter.Controls.Add(Me.pnlMomentumReversalBodyHorizontalSplitter, 0, 0)
        Me.pnlMomentumReversalBodyVerticalSplitter.Dock = System.Windows.Forms.DockStyle.Fill
        Me.pnlMomentumReversalBodyVerticalSplitter.Location = New System.Drawing.Point(4, 51)
        Me.pnlMomentumReversalBodyVerticalSplitter.Margin = New System.Windows.Forms.Padding(4)
        Me.pnlMomentumReversalBodyVerticalSplitter.Name = "pnlMomentumReversalBodyVerticalSplitter"
        Me.pnlMomentumReversalBodyVerticalSplitter.RowCount = 1
        Me.pnlMomentumReversalBodyVerticalSplitter.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.pnlMomentumReversalBodyVerticalSplitter.Size = New System.Drawing.Size(1347, 630)
        Me.pnlMomentumReversalBodyVerticalSplitter.TabIndex = 1
        '
        'PictureBox2
        '
        Me.PictureBox2.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.PictureBox2.Image = CType(resources.GetObject("PictureBox2.Image"), System.Drawing.Image)
        Me.PictureBox2.Location = New System.Drawing.Point(945, 3)
        Me.PictureBox2.Name = "PictureBox2"
        Me.PictureBox2.Size = New System.Drawing.Size(399, 624)
        Me.PictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.PictureBox2.TabIndex = 2
        Me.PictureBox2.TabStop = False
        '
        'pnlMomentumReversalBodyHorizontalSplitter
        '
        Me.pnlMomentumReversalBodyHorizontalSplitter.ColumnCount = 1
        Me.pnlMomentumReversalBodyHorizontalSplitter.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.pnlMomentumReversalBodyHorizontalSplitter.Controls.Add(Me.lstMomentumReversalLog, 0, 1)
        Me.pnlMomentumReversalBodyHorizontalSplitter.Controls.Add(Me.sfdgvMomentumReversalMainDashboard, 0, 0)
        Me.pnlMomentumReversalBodyHorizontalSplitter.Dock = System.Windows.Forms.DockStyle.Fill
        Me.pnlMomentumReversalBodyHorizontalSplitter.Location = New System.Drawing.Point(4, 4)
        Me.pnlMomentumReversalBodyHorizontalSplitter.Margin = New System.Windows.Forms.Padding(4)
        Me.pnlMomentumReversalBodyHorizontalSplitter.Name = "pnlMomentumReversalBodyHorizontalSplitter"
        Me.pnlMomentumReversalBodyHorizontalSplitter.RowCount = 2
        Me.pnlMomentumReversalBodyHorizontalSplitter.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 70.0!))
        Me.pnlMomentumReversalBodyHorizontalSplitter.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 30.0!))
        Me.pnlMomentumReversalBodyHorizontalSplitter.Size = New System.Drawing.Size(934, 622)
        Me.pnlMomentumReversalBodyHorizontalSplitter.TabIndex = 0
        '
        'lstMomentumReversalLog
        '
        Me.lstMomentumReversalLog.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lstMomentumReversalLog.ForeColor = System.Drawing.Color.FromArgb(CType(CType(29, Byte), Integer), CType(CType(29, Byte), Integer), CType(CType(29, Byte), Integer))
        Me.lstMomentumReversalLog.FormattingEnabled = True
        Me.lstMomentumReversalLog.ItemHeight = 16
        Me.lstMomentumReversalLog.Location = New System.Drawing.Point(4, 439)
        Me.lstMomentumReversalLog.Margin = New System.Windows.Forms.Padding(4)
        Me.lstMomentumReversalLog.Name = "lstMomentumReversalLog"
        Me.lstMomentumReversalLog.Size = New System.Drawing.Size(926, 179)
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
        Me.sfdgvMomentumReversalMainDashboard.Location = New System.Drawing.Point(4, 4)
        Me.sfdgvMomentumReversalMainDashboard.Margin = New System.Windows.Forms.Padding(4)
        Me.sfdgvMomentumReversalMainDashboard.Name = "sfdgvMomentumReversalMainDashboard"
        Me.sfdgvMomentumReversalMainDashboard.PasteOption = Syncfusion.WinForms.DataGrid.Enums.PasteOptions.None
        Me.sfdgvMomentumReversalMainDashboard.Size = New System.Drawing.Size(926, 427)
        Me.sfdgvMomentumReversalMainDashboard.TabIndex = 6
        Me.sfdgvMomentumReversalMainDashboard.Text = "SfDataGrid1"
        '
        'tabOHL
        '
        Me.tabOHL.Controls.Add(Me.pnlOHLMainPanelHorizontalSplitter)
        Me.tabOHL.Location = New System.Drawing.Point(4, 25)
        Me.tabOHL.Margin = New System.Windows.Forms.Padding(4)
        Me.tabOHL.Name = "tabOHL"
        Me.tabOHL.Padding = New System.Windows.Forms.Padding(4)
        Me.tabOHL.Size = New System.Drawing.Size(1363, 693)
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
        Me.pnlOHLMainPanelHorizontalSplitter.Location = New System.Drawing.Point(4, 4)
        Me.pnlOHLMainPanelHorizontalSplitter.Margin = New System.Windows.Forms.Padding(4)
        Me.pnlOHLMainPanelHorizontalSplitter.Name = "pnlOHLMainPanelHorizontalSplitter"
        Me.pnlOHLMainPanelHorizontalSplitter.RowCount = 2
        Me.pnlOHLMainPanelHorizontalSplitter.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 7.0!))
        Me.pnlOHLMainPanelHorizontalSplitter.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 93.0!))
        Me.pnlOHLMainPanelHorizontalSplitter.Size = New System.Drawing.Size(1355, 685)
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
        Me.pnlOHLTopHeaderVerticalSplitter.Location = New System.Drawing.Point(4, 4)
        Me.pnlOHLTopHeaderVerticalSplitter.Margin = New System.Windows.Forms.Padding(4)
        Me.pnlOHLTopHeaderVerticalSplitter.Name = "pnlOHLTopHeaderVerticalSplitter"
        Me.pnlOHLTopHeaderVerticalSplitter.RowCount = 1
        Me.pnlOHLTopHeaderVerticalSplitter.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.pnlOHLTopHeaderVerticalSplitter.Size = New System.Drawing.Size(1347, 39)
        Me.pnlOHLTopHeaderVerticalSplitter.TabIndex = 0
        '
        'btnOHLStop
        '
        Me.btnOHLStop.Dock = System.Windows.Forms.DockStyle.Fill
        Me.btnOHLStop.Location = New System.Drawing.Point(93, 4)
        Me.btnOHLStop.Margin = New System.Windows.Forms.Padding(4)
        Me.btnOHLStop.Name = "btnOHLStop"
        Me.btnOHLStop.Size = New System.Drawing.Size(81, 31)
        Me.btnOHLStop.TabIndex = 11
        Me.btnOHLStop.Text = "Stop"
        Me.btnOHLStop.UseVisualStyleBackColor = True
        '
        'btnOHLStart
        '
        Me.btnOHLStart.Dock = System.Windows.Forms.DockStyle.Fill
        Me.btnOHLStart.Location = New System.Drawing.Point(4, 4)
        Me.btnOHLStart.Margin = New System.Windows.Forms.Padding(4)
        Me.btnOHLStart.Name = "btnOHLStart"
        Me.btnOHLStart.Size = New System.Drawing.Size(81, 31)
        Me.btnOHLStart.TabIndex = 2
        Me.btnOHLStart.Text = "Start"
        Me.btnOHLStart.UseVisualStyleBackColor = True
        '
        'Panel2
        '
        Me.Panel2.Controls.Add(Me.blbOHLTickerStatus)
        Me.Panel2.Controls.Add(Me.lblOHLTickerStatus)
        Me.Panel2.Location = New System.Drawing.Point(1197, 4)
        Me.Panel2.Margin = New System.Windows.Forms.Padding(4)
        Me.Panel2.Name = "Panel2"
        Me.Panel2.Size = New System.Drawing.Size(146, 31)
        Me.Panel2.TabIndex = 9
        '
        'blbOHLTickerStatus
        '
        Me.blbOHLTickerStatus.Color = System.Drawing.Color.Pink
        Me.blbOHLTickerStatus.Dock = System.Windows.Forms.DockStyle.Right
        Me.blbOHLTickerStatus.Location = New System.Drawing.Point(99, 0)
        Me.blbOHLTickerStatus.Margin = New System.Windows.Forms.Padding(4)
        Me.blbOHLTickerStatus.Name = "blbOHLTickerStatus"
        Me.blbOHLTickerStatus.On = True
        Me.blbOHLTickerStatus.Size = New System.Drawing.Size(47, 31)
        Me.blbOHLTickerStatus.TabIndex = 7
        Me.blbOHLTickerStatus.Text = "LedBulb1"
        '
        'lblOHLTickerStatus
        '
        Me.lblOHLTickerStatus.AutoSize = True
        Me.lblOHLTickerStatus.Location = New System.Drawing.Point(11, 9)
        Me.lblOHLTickerStatus.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lblOHLTickerStatus.Name = "lblOHLTickerStatus"
        Me.lblOHLTickerStatus.Size = New System.Drawing.Size(91, 17)
        Me.lblOHLTickerStatus.TabIndex = 9
        Me.lblOHLTickerStatus.Text = "Ticker Status"
        '
        'pnlOHLBodyVerticalSplitter
        '
        Me.pnlOHLBodyVerticalSplitter.ColumnCount = 2
        Me.pnlOHLBodyVerticalSplitter.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 70.0!))
        Me.pnlOHLBodyVerticalSplitter.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30.0!))
        Me.pnlOHLBodyVerticalSplitter.Controls.Add(Me.PictureBox3, 0, 0)
        Me.pnlOHLBodyVerticalSplitter.Controls.Add(Me.pnlOHLBodyHorizontalSplitter, 0, 0)
        Me.pnlOHLBodyVerticalSplitter.Dock = System.Windows.Forms.DockStyle.Fill
        Me.pnlOHLBodyVerticalSplitter.Location = New System.Drawing.Point(4, 51)
        Me.pnlOHLBodyVerticalSplitter.Margin = New System.Windows.Forms.Padding(4)
        Me.pnlOHLBodyVerticalSplitter.Name = "pnlOHLBodyVerticalSplitter"
        Me.pnlOHLBodyVerticalSplitter.RowCount = 1
        Me.pnlOHLBodyVerticalSplitter.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.pnlOHLBodyVerticalSplitter.Size = New System.Drawing.Size(1347, 630)
        Me.pnlOHLBodyVerticalSplitter.TabIndex = 1
        '
        'PictureBox3
        '
        Me.PictureBox3.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PictureBox3.Image = CType(resources.GetObject("PictureBox3.Image"), System.Drawing.Image)
        Me.PictureBox3.Location = New System.Drawing.Point(945, 3)
        Me.PictureBox3.Name = "PictureBox3"
        Me.PictureBox3.Size = New System.Drawing.Size(399, 624)
        Me.PictureBox3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.PictureBox3.TabIndex = 2
        Me.PictureBox3.TabStop = False
        '
        'pnlOHLBodyHorizontalSplitter
        '
        Me.pnlOHLBodyHorizontalSplitter.ColumnCount = 1
        Me.pnlOHLBodyHorizontalSplitter.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.pnlOHLBodyHorizontalSplitter.Controls.Add(Me.lstOHLLog, 0, 1)
        Me.pnlOHLBodyHorizontalSplitter.Controls.Add(Me.sfdgvOHLMainDashboard, 0, 0)
        Me.pnlOHLBodyHorizontalSplitter.Dock = System.Windows.Forms.DockStyle.Fill
        Me.pnlOHLBodyHorizontalSplitter.Location = New System.Drawing.Point(4, 4)
        Me.pnlOHLBodyHorizontalSplitter.Margin = New System.Windows.Forms.Padding(4)
        Me.pnlOHLBodyHorizontalSplitter.Name = "pnlOHLBodyHorizontalSplitter"
        Me.pnlOHLBodyHorizontalSplitter.RowCount = 2
        Me.pnlOHLBodyHorizontalSplitter.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 70.0!))
        Me.pnlOHLBodyHorizontalSplitter.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 30.0!))
        Me.pnlOHLBodyHorizontalSplitter.Size = New System.Drawing.Size(934, 622)
        Me.pnlOHLBodyHorizontalSplitter.TabIndex = 0
        '
        'lstOHLLog
        '
        Me.lstOHLLog.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lstOHLLog.ForeColor = System.Drawing.Color.FromArgb(CType(CType(29, Byte), Integer), CType(CType(29, Byte), Integer), CType(CType(29, Byte), Integer))
        Me.lstOHLLog.FormattingEnabled = True
        Me.lstOHLLog.ItemHeight = 16
        Me.lstOHLLog.Location = New System.Drawing.Point(4, 439)
        Me.lstOHLLog.Margin = New System.Windows.Forms.Padding(4)
        Me.lstOHLLog.Name = "lstOHLLog"
        Me.lstOHLLog.Size = New System.Drawing.Size(926, 179)
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
        Me.sfdgvOHLMainDashboard.Location = New System.Drawing.Point(4, 4)
        Me.sfdgvOHLMainDashboard.Margin = New System.Windows.Forms.Padding(4)
        Me.sfdgvOHLMainDashboard.Name = "sfdgvOHLMainDashboard"
        Me.sfdgvOHLMainDashboard.PasteOption = Syncfusion.WinForms.DataGrid.Enums.PasteOptions.None
        Me.sfdgvOHLMainDashboard.Size = New System.Drawing.Size(926, 427)
        Me.sfdgvOHLMainDashboard.TabIndex = 6
        Me.sfdgvOHLMainDashboard.Text = "SfDataGrid1"
        '
        'tabAmiSignal
        '
        Me.tabAmiSignal.Controls.Add(Me.pnlAmiSignalMainPanelHorizontalSplitter)
        Me.tabAmiSignal.Location = New System.Drawing.Point(4, 25)
        Me.tabAmiSignal.Margin = New System.Windows.Forms.Padding(4)
        Me.tabAmiSignal.Name = "tabAmiSignal"
        Me.tabAmiSignal.Size = New System.Drawing.Size(1363, 693)
        Me.tabAmiSignal.TabIndex = 2
        Me.tabAmiSignal.Text = "AmiBroker Signal"
        Me.tabAmiSignal.UseVisualStyleBackColor = True
        '
        'pnlAmiSignalMainPanelHorizontalSplitter
        '
        Me.pnlAmiSignalMainPanelHorizontalSplitter.ColumnCount = 1
        Me.pnlAmiSignalMainPanelHorizontalSplitter.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.pnlAmiSignalMainPanelHorizontalSplitter.Controls.Add(Me.pnlAmiSignalTopHeaderVerticalSplitter, 0, 0)
        Me.pnlAmiSignalMainPanelHorizontalSplitter.Controls.Add(Me.pnlAmiSignalBodyVerticalSplitter, 0, 1)
        Me.pnlAmiSignalMainPanelHorizontalSplitter.Dock = System.Windows.Forms.DockStyle.Fill
        Me.pnlAmiSignalMainPanelHorizontalSplitter.Location = New System.Drawing.Point(0, 0)
        Me.pnlAmiSignalMainPanelHorizontalSplitter.Margin = New System.Windows.Forms.Padding(4)
        Me.pnlAmiSignalMainPanelHorizontalSplitter.Name = "pnlAmiSignalMainPanelHorizontalSplitter"
        Me.pnlAmiSignalMainPanelHorizontalSplitter.RowCount = 2
        Me.pnlAmiSignalMainPanelHorizontalSplitter.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 7.0!))
        Me.pnlAmiSignalMainPanelHorizontalSplitter.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 93.0!))
        Me.pnlAmiSignalMainPanelHorizontalSplitter.Size = New System.Drawing.Size(1363, 693)
        Me.pnlAmiSignalMainPanelHorizontalSplitter.TabIndex = 1
        '
        'pnlAmiSignalTopHeaderVerticalSplitter
        '
        Me.pnlAmiSignalTopHeaderVerticalSplitter.ColumnCount = 15
        Me.pnlAmiSignalTopHeaderVerticalSplitter.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 6.666668!))
        Me.pnlAmiSignalTopHeaderVerticalSplitter.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 6.666668!))
        Me.pnlAmiSignalTopHeaderVerticalSplitter.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 6.666668!))
        Me.pnlAmiSignalTopHeaderVerticalSplitter.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 6.666668!))
        Me.pnlAmiSignalTopHeaderVerticalSplitter.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 6.666668!))
        Me.pnlAmiSignalTopHeaderVerticalSplitter.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 6.666668!))
        Me.pnlAmiSignalTopHeaderVerticalSplitter.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 6.666668!))
        Me.pnlAmiSignalTopHeaderVerticalSplitter.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 6.666668!))
        Me.pnlAmiSignalTopHeaderVerticalSplitter.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 6.666668!))
        Me.pnlAmiSignalTopHeaderVerticalSplitter.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 6.666668!))
        Me.pnlAmiSignalTopHeaderVerticalSplitter.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 6.666668!))
        Me.pnlAmiSignalTopHeaderVerticalSplitter.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 6.666668!))
        Me.pnlAmiSignalTopHeaderVerticalSplitter.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 6.666668!))
        Me.pnlAmiSignalTopHeaderVerticalSplitter.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 2.741935!))
        Me.pnlAmiSignalTopHeaderVerticalSplitter.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 11.29032!))
        Me.pnlAmiSignalTopHeaderVerticalSplitter.Controls.Add(Me.btnAmiSignalStop, 0, 0)
        Me.pnlAmiSignalTopHeaderVerticalSplitter.Controls.Add(Me.btnAmiSignalStart, 0, 0)
        Me.pnlAmiSignalTopHeaderVerticalSplitter.Controls.Add(Me.Panel3, 14, 0)
        Me.pnlAmiSignalTopHeaderVerticalSplitter.Dock = System.Windows.Forms.DockStyle.Fill
        Me.pnlAmiSignalTopHeaderVerticalSplitter.Location = New System.Drawing.Point(4, 4)
        Me.pnlAmiSignalTopHeaderVerticalSplitter.Margin = New System.Windows.Forms.Padding(4)
        Me.pnlAmiSignalTopHeaderVerticalSplitter.Name = "pnlAmiSignalTopHeaderVerticalSplitter"
        Me.pnlAmiSignalTopHeaderVerticalSplitter.RowCount = 1
        Me.pnlAmiSignalTopHeaderVerticalSplitter.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.pnlAmiSignalTopHeaderVerticalSplitter.Size = New System.Drawing.Size(1355, 40)
        Me.pnlAmiSignalTopHeaderVerticalSplitter.TabIndex = 0
        '
        'btnAmiSignalStop
        '
        Me.btnAmiSignalStop.Dock = System.Windows.Forms.DockStyle.Fill
        Me.btnAmiSignalStop.Location = New System.Drawing.Point(93, 4)
        Me.btnAmiSignalStop.Margin = New System.Windows.Forms.Padding(4)
        Me.btnAmiSignalStop.Name = "btnAmiSignalStop"
        Me.btnAmiSignalStop.Size = New System.Drawing.Size(81, 32)
        Me.btnAmiSignalStop.TabIndex = 10
        Me.btnAmiSignalStop.Text = "Stop"
        Me.btnAmiSignalStop.UseVisualStyleBackColor = True
        '
        'btnAmiSignalStart
        '
        Me.btnAmiSignalStart.Dock = System.Windows.Forms.DockStyle.Fill
        Me.btnAmiSignalStart.Location = New System.Drawing.Point(4, 4)
        Me.btnAmiSignalStart.Margin = New System.Windows.Forms.Padding(4)
        Me.btnAmiSignalStart.Name = "btnAmiSignalStart"
        Me.btnAmiSignalStart.Size = New System.Drawing.Size(81, 32)
        Me.btnAmiSignalStart.TabIndex = 2
        Me.btnAmiSignalStart.Text = "Start"
        Me.btnAmiSignalStart.UseVisualStyleBackColor = True
        '
        'Panel3
        '
        Me.Panel3.Controls.Add(Me.blbAmiSignalTickerStatus)
        Me.Panel3.Controls.Add(Me.lblAmiSignalTickerStatus)
        Me.Panel3.Location = New System.Drawing.Point(1197, 4)
        Me.Panel3.Margin = New System.Windows.Forms.Padding(4)
        Me.Panel3.Name = "Panel3"
        Me.Panel3.Size = New System.Drawing.Size(147, 32)
        Me.Panel3.TabIndex = 9
        '
        'blbAmiSignalTickerStatus
        '
        Me.blbAmiSignalTickerStatus.Color = System.Drawing.Color.Pink
        Me.blbAmiSignalTickerStatus.Dock = System.Windows.Forms.DockStyle.Right
        Me.blbAmiSignalTickerStatus.Location = New System.Drawing.Point(100, 0)
        Me.blbAmiSignalTickerStatus.Margin = New System.Windows.Forms.Padding(4)
        Me.blbAmiSignalTickerStatus.Name = "blbAmiSignalTickerStatus"
        Me.blbAmiSignalTickerStatus.On = True
        Me.blbAmiSignalTickerStatus.Size = New System.Drawing.Size(47, 32)
        Me.blbAmiSignalTickerStatus.TabIndex = 7
        Me.blbAmiSignalTickerStatus.Text = "LedBulb1"
        '
        'lblAmiSignalTickerStatus
        '
        Me.lblAmiSignalTickerStatus.AutoSize = True
        Me.lblAmiSignalTickerStatus.Location = New System.Drawing.Point(9, 9)
        Me.lblAmiSignalTickerStatus.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lblAmiSignalTickerStatus.Name = "lblAmiSignalTickerStatus"
        Me.lblAmiSignalTickerStatus.Size = New System.Drawing.Size(91, 17)
        Me.lblAmiSignalTickerStatus.TabIndex = 9
        Me.lblAmiSignalTickerStatus.Text = "Ticker Status"
        '
        'pnlAmiSignalBodyVerticalSplitter
        '
        Me.pnlAmiSignalBodyVerticalSplitter.ColumnCount = 2
        Me.pnlAmiSignalBodyVerticalSplitter.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 70.0!))
        Me.pnlAmiSignalBodyVerticalSplitter.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30.0!))
        Me.pnlAmiSignalBodyVerticalSplitter.Controls.Add(Me.pnlAmiSignalBodyHorizontalSplitter, 0, 0)
        Me.pnlAmiSignalBodyVerticalSplitter.Controls.Add(Me.PictureBox1, 1, 0)
        Me.pnlAmiSignalBodyVerticalSplitter.Dock = System.Windows.Forms.DockStyle.Fill
        Me.pnlAmiSignalBodyVerticalSplitter.Location = New System.Drawing.Point(4, 52)
        Me.pnlAmiSignalBodyVerticalSplitter.Margin = New System.Windows.Forms.Padding(4)
        Me.pnlAmiSignalBodyVerticalSplitter.Name = "pnlAmiSignalBodyVerticalSplitter"
        Me.pnlAmiSignalBodyVerticalSplitter.RowCount = 1
        Me.pnlAmiSignalBodyVerticalSplitter.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.pnlAmiSignalBodyVerticalSplitter.Size = New System.Drawing.Size(1355, 637)
        Me.pnlAmiSignalBodyVerticalSplitter.TabIndex = 1
        '
        'pnlAmiSignalBodyHorizontalSplitter
        '
        Me.pnlAmiSignalBodyHorizontalSplitter.ColumnCount = 1
        Me.pnlAmiSignalBodyHorizontalSplitter.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.pnlAmiSignalBodyHorizontalSplitter.Controls.Add(Me.lstAmiSignalLog, 0, 1)
        Me.pnlAmiSignalBodyHorizontalSplitter.Controls.Add(Me.sfdgvAmiSignalMainDashboard, 0, 0)
        Me.pnlAmiSignalBodyHorizontalSplitter.Dock = System.Windows.Forms.DockStyle.Fill
        Me.pnlAmiSignalBodyHorizontalSplitter.Location = New System.Drawing.Point(4, 4)
        Me.pnlAmiSignalBodyHorizontalSplitter.Margin = New System.Windows.Forms.Padding(4)
        Me.pnlAmiSignalBodyHorizontalSplitter.Name = "pnlAmiSignalBodyHorizontalSplitter"
        Me.pnlAmiSignalBodyHorizontalSplitter.RowCount = 2
        Me.pnlAmiSignalBodyHorizontalSplitter.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 70.0!))
        Me.pnlAmiSignalBodyHorizontalSplitter.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 30.0!))
        Me.pnlAmiSignalBodyHorizontalSplitter.Size = New System.Drawing.Size(940, 629)
        Me.pnlAmiSignalBodyHorizontalSplitter.TabIndex = 0
        '
        'lstAmiSignalLog
        '
        Me.lstAmiSignalLog.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lstAmiSignalLog.ForeColor = System.Drawing.Color.FromArgb(CType(CType(29, Byte), Integer), CType(CType(29, Byte), Integer), CType(CType(29, Byte), Integer))
        Me.lstAmiSignalLog.FormattingEnabled = True
        Me.lstAmiSignalLog.ItemHeight = 16
        Me.lstAmiSignalLog.Location = New System.Drawing.Point(4, 444)
        Me.lstAmiSignalLog.Margin = New System.Windows.Forms.Padding(4)
        Me.lstAmiSignalLog.Name = "lstAmiSignalLog"
        Me.lstAmiSignalLog.Size = New System.Drawing.Size(932, 181)
        Me.lstAmiSignalLog.TabIndex = 9
        '
        'sfdgvAmiSignalMainDashboard
        '
        Me.sfdgvAmiSignalMainDashboard.AccessibleName = "Table"
        Me.sfdgvAmiSignalMainDashboard.AllowDraggingColumns = True
        Me.sfdgvAmiSignalMainDashboard.AllowEditing = False
        Me.sfdgvAmiSignalMainDashboard.AllowFiltering = True
        Me.sfdgvAmiSignalMainDashboard.AllowResizingColumns = True
        Me.sfdgvAmiSignalMainDashboard.AutoGenerateColumnsMode = Syncfusion.WinForms.DataGrid.Enums.AutoGenerateColumnsMode.SmartReset
        Me.sfdgvAmiSignalMainDashboard.AutoSizeColumnsMode = Syncfusion.WinForms.DataGrid.Enums.AutoSizeColumnsMode.AllCells
        Me.sfdgvAmiSignalMainDashboard.Dock = System.Windows.Forms.DockStyle.Fill
        Me.sfdgvAmiSignalMainDashboard.Location = New System.Drawing.Point(4, 4)
        Me.sfdgvAmiSignalMainDashboard.Margin = New System.Windows.Forms.Padding(4)
        Me.sfdgvAmiSignalMainDashboard.Name = "sfdgvAmiSignalMainDashboard"
        Me.sfdgvAmiSignalMainDashboard.PasteOption = Syncfusion.WinForms.DataGrid.Enums.PasteOptions.None
        Me.sfdgvAmiSignalMainDashboard.Size = New System.Drawing.Size(932, 432)
        Me.sfdgvAmiSignalMainDashboard.TabIndex = 6
        Me.sfdgvAmiSignalMainDashboard.Text = "SfDataGrid1"
        '
        'PictureBox1
        '
        Me.PictureBox1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PictureBox1.Image = CType(resources.GetObject("PictureBox1.Image"), System.Drawing.Image)
        Me.PictureBox1.Location = New System.Drawing.Point(951, 3)
        Me.PictureBox1.Name = "PictureBox1"
        Me.PictureBox1.Size = New System.Drawing.Size(401, 631)
        Me.PictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.PictureBox1.TabIndex = 1
        Me.PictureBox1.TabStop = False
        '
        'tmrMomentumReversalTickerStatus
        '
        Me.tmrMomentumReversalTickerStatus.Enabled = True
        '
        'tmrOHLTickerStatus
        '
        Me.tmrOHLTickerStatus.Enabled = True
        '
        'tmrAmiSignalTickerStatus
        '
        Me.tmrAmiSignalTickerStatus.Enabled = True
        '
        'frmMainTabbed
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 16.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1371, 750)
        Me.Controls.Add(Me.tabMain)
        Me.Controls.Add(Me.msMainMenuStrip)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MainMenuStrip = Me.msMainMenuStrip
        Me.Margin = New System.Windows.Forms.Padding(4)
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
        CType(Me.PictureBox2, System.ComponentModel.ISupportInitialize).EndInit()
        Me.pnlMomentumReversalBodyHorizontalSplitter.ResumeLayout(False)
        CType(Me.sfdgvMomentumReversalMainDashboard, System.ComponentModel.ISupportInitialize).EndInit()
        Me.tabOHL.ResumeLayout(False)
        Me.pnlOHLMainPanelHorizontalSplitter.ResumeLayout(False)
        Me.pnlOHLTopHeaderVerticalSplitter.ResumeLayout(False)
        Me.Panel2.ResumeLayout(False)
        Me.Panel2.PerformLayout()
        Me.pnlOHLBodyVerticalSplitter.ResumeLayout(False)
        CType(Me.PictureBox3, System.ComponentModel.ISupportInitialize).EndInit()
        Me.pnlOHLBodyHorizontalSplitter.ResumeLayout(False)
        CType(Me.sfdgvOHLMainDashboard, System.ComponentModel.ISupportInitialize).EndInit()
        Me.tabAmiSignal.ResumeLayout(False)
        Me.pnlAmiSignalMainPanelHorizontalSplitter.ResumeLayout(False)
        Me.pnlAmiSignalTopHeaderVerticalSplitter.ResumeLayout(False)
        Me.Panel3.ResumeLayout(False)
        Me.Panel3.PerformLayout()
        Me.pnlAmiSignalBodyVerticalSplitter.ResumeLayout(False)
        Me.pnlAmiSignalBodyHorizontalSplitter.ResumeLayout(False)
        CType(Me.sfdgvAmiSignalMainDashboard, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
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
    Friend WithEvents tabAmiSignal As TabPage
    Friend WithEvents pnlAmiSignalMainPanelHorizontalSplitter As TableLayoutPanel
    Friend WithEvents pnlAmiSignalTopHeaderVerticalSplitter As TableLayoutPanel
    Friend WithEvents btnAmiSignalStop As Button
    Friend WithEvents btnAmiSignalStart As Button
    Friend WithEvents Panel3 As Panel
    Friend WithEvents blbAmiSignalTickerStatus As Bulb.LedBulb
    Friend WithEvents lblAmiSignalTickerStatus As Label
    Friend WithEvents pnlAmiSignalBodyVerticalSplitter As TableLayoutPanel
    Friend WithEvents pnlAmiSignalBodyHorizontalSplitter As TableLayoutPanel
    Friend WithEvents lstAmiSignalLog As ListBox
    Friend WithEvents sfdgvAmiSignalMainDashboard As Syncfusion.WinForms.DataGrid.SfDataGrid
    Friend WithEvents tmrAmiSignalTickerStatus As Timer
    Friend WithEvents PictureBox1 As PictureBox
    Friend WithEvents PictureBox2 As PictureBox
    Friend WithEvents PictureBox3 As PictureBox
End Class
