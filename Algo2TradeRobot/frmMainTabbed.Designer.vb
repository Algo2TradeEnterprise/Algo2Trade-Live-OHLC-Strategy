﻿<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
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
        Me.miOptions = New System.Windows.Forms.ToolStripMenuItem()
        Me.miUserDetails = New System.Windows.Forms.ToolStripMenuItem()
        Me.miAdvancedOptions = New System.Windows.Forms.ToolStripMenuItem()
        Me.miAbout = New System.Windows.Forms.ToolStripMenuItem()
        Me.tabMain = New System.Windows.Forms.TabControl()
        Me.tabNearFarHedging = New System.Windows.Forms.TabPage()
        Me.pnlNearFarHedgingMainPanelHorizontalSplitter = New System.Windows.Forms.TableLayoutPanel()
        Me.pnlNearFarHedgingTopHeaderVerticalSplitter = New System.Windows.Forms.TableLayoutPanel()
        Me.btnNearFarHedgingStop = New System.Windows.Forms.Button()
        Me.btnNearFarHedgingStart = New System.Windows.Forms.Button()
        Me.Panel5 = New System.Windows.Forms.Panel()
        Me.blbNearFarHedgingTickerStatus = New Bulb.LedBulb()
        Me.lblNearFarHedgingTickerStatus = New System.Windows.Forms.Label()
        Me.btnNearFarHedgingSettings = New System.Windows.Forms.Button()
        Me.linklblNearFarHedgingTradableInstrument = New System.Windows.Forms.LinkLabel()
        Me.pnlNearFarHedgingBodyVerticalSplitter = New System.Windows.Forms.TableLayoutPanel()
        Me.pnlNearFarHedgingBodyHorizontalSplitter = New System.Windows.Forms.TableLayoutPanel()
        Me.lstNearFarHedgingLog = New System.Windows.Forms.ListBox()
        Me.sfdgvNearFarHedgingMainDashboard = New Syncfusion.WinForms.DataGrid.SfDataGrid()
        Me.tabEMA_Supertrend = New System.Windows.Forms.TabPage()
        Me.pnlEMA5_20STMainPanelHorizontalSplitter = New System.Windows.Forms.TableLayoutPanel()
        Me.pnlEMA5_20STTopHeaderVerticalSplitter = New System.Windows.Forms.TableLayoutPanel()
        Me.btnEMA_SupertrendExitAll = New System.Windows.Forms.Button()
        Me.btnEMA_SupertrendStop = New System.Windows.Forms.Button()
        Me.btnEMA_SupertrendStart = New System.Windows.Forms.Button()
        Me.Panel4 = New System.Windows.Forms.Panel()
        Me.blbEMA_SupertrendTickerStatus = New Bulb.LedBulb()
        Me.lblEMA_SupertrendTickerStatus = New System.Windows.Forms.Label()
        Me.btnEMA_SupertrendSettings = New System.Windows.Forms.Button()
        Me.linklblEMA_SupertrendTradableInstrument = New System.Windows.Forms.LinkLabel()
        Me.pnlEMA5_20STBodyVerticalSplitter = New System.Windows.Forms.TableLayoutPanel()
        Me.pnlEMA5_20STBodyHorizontalSplitter = New System.Windows.Forms.TableLayoutPanel()
        Me.lstEMA_SupertrendLog = New System.Windows.Forms.ListBox()
        Me.sfdgvEMA_SupertrendMainDashboard = New Syncfusion.WinForms.DataGrid.SfDataGrid()
        Me.tabMomentumReversal = New System.Windows.Forms.TabPage()
        Me.pnlMomentumReversalMainPanelHorizontalSplitter = New System.Windows.Forms.TableLayoutPanel()
        Me.pnlMomentumReversalTopHeaderVerticalSplitter = New System.Windows.Forms.TableLayoutPanel()
        Me.btnMomentumReversalStop = New System.Windows.Forms.Button()
        Me.btnMomentumReversalStart = New System.Windows.Forms.Button()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.blbMomentumReversalTickerStatus = New Bulb.LedBulb()
        Me.lblMomentumReversalTickerStatus = New System.Windows.Forms.Label()
        Me.btnMomentumReversalSettings = New System.Windows.Forms.Button()
        Me.linklblMomentumReversalTradableInstrument = New System.Windows.Forms.LinkLabel()
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
        Me.btnOHLSettings = New System.Windows.Forms.Button()
        Me.linklblOHLTradableInstruments = New System.Windows.Forms.LinkLabel()
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
        Me.btnAmiSignalSettings = New System.Windows.Forms.Button()
        Me.linklblAmiSignalTradableInstrument = New System.Windows.Forms.LinkLabel()
        Me.pnlAmiSignalBodyVerticalSplitter = New System.Windows.Forms.TableLayoutPanel()
        Me.pnlAmiSignalBodyHorizontalSplitter = New System.Windows.Forms.TableLayoutPanel()
        Me.lstAmiSignalLog = New System.Windows.Forms.ListBox()
        Me.sfdgvAmiSignalMainDashboard = New Syncfusion.WinForms.DataGrid.SfDataGrid()
        Me.PictureBox1 = New System.Windows.Forms.PictureBox()
        Me.tmrMomentumReversalTickerStatus = New System.Windows.Forms.Timer(Me.components)
        Me.tmrOHLTickerStatus = New System.Windows.Forms.Timer(Me.components)
        Me.tmrAmiSignalTickerStatus = New System.Windows.Forms.Timer(Me.components)
        Me.tmrEMA_SupertrendTickerStatus = New System.Windows.Forms.Timer(Me.components)
        Me.tmrNearFarHedgingTickerStatus = New System.Windows.Forms.Timer(Me.components)
        Me.msMainMenuStrip.SuspendLayout()
        Me.tabMain.SuspendLayout()
        Me.tabNearFarHedging.SuspendLayout()
        Me.pnlNearFarHedgingMainPanelHorizontalSplitter.SuspendLayout()
        Me.pnlNearFarHedgingTopHeaderVerticalSplitter.SuspendLayout()
        Me.Panel5.SuspendLayout()
        Me.pnlNearFarHedgingBodyVerticalSplitter.SuspendLayout()
        Me.pnlNearFarHedgingBodyHorizontalSplitter.SuspendLayout()
        CType(Me.sfdgvNearFarHedgingMainDashboard, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.tabEMA_Supertrend.SuspendLayout()
        Me.pnlEMA5_20STMainPanelHorizontalSplitter.SuspendLayout()
        Me.pnlEMA5_20STTopHeaderVerticalSplitter.SuspendLayout()
        Me.Panel4.SuspendLayout()
        Me.pnlEMA5_20STBodyVerticalSplitter.SuspendLayout()
        Me.pnlEMA5_20STBodyHorizontalSplitter.SuspendLayout()
        CType(Me.sfdgvEMA_SupertrendMainDashboard, System.ComponentModel.ISupportInitialize).BeginInit()
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
        Me.msMainMenuStrip.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.miOptions, Me.miAbout})
        Me.msMainMenuStrip.Location = New System.Drawing.Point(0, 0)
        Me.msMainMenuStrip.Name = "msMainMenuStrip"
        Me.msMainMenuStrip.Padding = New System.Windows.Forms.Padding(8, 2, 0, 2)
        Me.msMainMenuStrip.Size = New System.Drawing.Size(1371, 28)
        Me.msMainMenuStrip.TabIndex = 0
        Me.msMainMenuStrip.Text = "MenuStrip1"
        '
        'miOptions
        '
        Me.miOptions.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.miUserDetails, Me.miAdvancedOptions})
        Me.miOptions.Name = "miOptions"
        Me.miOptions.Size = New System.Drawing.Size(73, 24)
        Me.miOptions.Text = "&Options"
        '
        'miUserDetails
        '
        Me.miUserDetails.Name = "miUserDetails"
        Me.miUserDetails.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.F2), System.Windows.Forms.Keys)
        Me.miUserDetails.Size = New System.Drawing.Size(263, 26)
        Me.miUserDetails.Text = "&User Details"
        '
        'miAdvancedOptions
        '
        Me.miAdvancedOptions.Name = "miAdvancedOptions"
        Me.miAdvancedOptions.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.F7), System.Windows.Forms.Keys)
        Me.miAdvancedOptions.Size = New System.Drawing.Size(263, 26)
        Me.miAdvancedOptions.Text = "A&dvanced Options"
        '
        'miAbout
        '
        Me.miAbout.Name = "miAbout"
        Me.miAbout.Size = New System.Drawing.Size(62, 24)
        Me.miAbout.Text = "&About"
        '
        'tabMain
        '
        Me.tabMain.Controls.Add(Me.tabNearFarHedging)
        Me.tabMain.Controls.Add(Me.tabEMA_Supertrend)
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
        'tabNearFarHedging
        '
        Me.tabNearFarHedging.Controls.Add(Me.pnlNearFarHedgingMainPanelHorizontalSplitter)
        Me.tabNearFarHedging.Location = New System.Drawing.Point(4, 25)
        Me.tabNearFarHedging.Name = "tabNearFarHedging"
        Me.tabNearFarHedging.Size = New System.Drawing.Size(1363, 693)
        Me.tabNearFarHedging.TabIndex = 4
        Me.tabNearFarHedging.Text = "Near Far Hedging"
        Me.tabNearFarHedging.UseVisualStyleBackColor = True
        '
        'pnlNearFarHedgingMainPanelHorizontalSplitter
        '
        Me.pnlNearFarHedgingMainPanelHorizontalSplitter.ColumnCount = 1
        Me.pnlNearFarHedgingMainPanelHorizontalSplitter.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.pnlNearFarHedgingMainPanelHorizontalSplitter.Controls.Add(Me.pnlNearFarHedgingTopHeaderVerticalSplitter, 0, 0)
        Me.pnlNearFarHedgingMainPanelHorizontalSplitter.Controls.Add(Me.pnlNearFarHedgingBodyVerticalSplitter, 0, 1)
        Me.pnlNearFarHedgingMainPanelHorizontalSplitter.Dock = System.Windows.Forms.DockStyle.Fill
        Me.pnlNearFarHedgingMainPanelHorizontalSplitter.Location = New System.Drawing.Point(0, 0)
        Me.pnlNearFarHedgingMainPanelHorizontalSplitter.Margin = New System.Windows.Forms.Padding(4)
        Me.pnlNearFarHedgingMainPanelHorizontalSplitter.Name = "pnlNearFarHedgingMainPanelHorizontalSplitter"
        Me.pnlNearFarHedgingMainPanelHorizontalSplitter.RowCount = 2
        Me.pnlNearFarHedgingMainPanelHorizontalSplitter.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 7.0!))
        Me.pnlNearFarHedgingMainPanelHorizontalSplitter.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 93.0!))
        Me.pnlNearFarHedgingMainPanelHorizontalSplitter.Size = New System.Drawing.Size(1363, 693)
        Me.pnlNearFarHedgingMainPanelHorizontalSplitter.TabIndex = 2
        '
        'pnlNearFarHedgingTopHeaderVerticalSplitter
        '
        Me.pnlNearFarHedgingTopHeaderVerticalSplitter.ColumnCount = 15
        Me.pnlNearFarHedgingTopHeaderVerticalSplitter.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 6.666668!))
        Me.pnlNearFarHedgingTopHeaderVerticalSplitter.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 6.666668!))
        Me.pnlNearFarHedgingTopHeaderVerticalSplitter.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 6.666668!))
        Me.pnlNearFarHedgingTopHeaderVerticalSplitter.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 6.666668!))
        Me.pnlNearFarHedgingTopHeaderVerticalSplitter.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 6.666668!))
        Me.pnlNearFarHedgingTopHeaderVerticalSplitter.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 6.666668!))
        Me.pnlNearFarHedgingTopHeaderVerticalSplitter.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 6.666668!))
        Me.pnlNearFarHedgingTopHeaderVerticalSplitter.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 6.666668!))
        Me.pnlNearFarHedgingTopHeaderVerticalSplitter.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 6.666668!))
        Me.pnlNearFarHedgingTopHeaderVerticalSplitter.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 6.666668!))
        Me.pnlNearFarHedgingTopHeaderVerticalSplitter.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16.70379!))
        Me.pnlNearFarHedgingTopHeaderVerticalSplitter.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 0.8166295!))
        Me.pnlNearFarHedgingTopHeaderVerticalSplitter.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 1.707498!))
        Me.pnlNearFarHedgingTopHeaderVerticalSplitter.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 2.741935!))
        Me.pnlNearFarHedgingTopHeaderVerticalSplitter.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 11.29032!))
        Me.pnlNearFarHedgingTopHeaderVerticalSplitter.Controls.Add(Me.btnNearFarHedgingStop, 0, 0)
        Me.pnlNearFarHedgingTopHeaderVerticalSplitter.Controls.Add(Me.btnNearFarHedgingStart, 0, 0)
        Me.pnlNearFarHedgingTopHeaderVerticalSplitter.Controls.Add(Me.Panel5, 14, 0)
        Me.pnlNearFarHedgingTopHeaderVerticalSplitter.Controls.Add(Me.btnNearFarHedgingSettings, 9, 0)
        Me.pnlNearFarHedgingTopHeaderVerticalSplitter.Controls.Add(Me.linklblNearFarHedgingTradableInstrument, 10, 0)
        Me.pnlNearFarHedgingTopHeaderVerticalSplitter.Dock = System.Windows.Forms.DockStyle.Fill
        Me.pnlNearFarHedgingTopHeaderVerticalSplitter.Location = New System.Drawing.Point(4, 4)
        Me.pnlNearFarHedgingTopHeaderVerticalSplitter.Margin = New System.Windows.Forms.Padding(4)
        Me.pnlNearFarHedgingTopHeaderVerticalSplitter.Name = "pnlNearFarHedgingTopHeaderVerticalSplitter"
        Me.pnlNearFarHedgingTopHeaderVerticalSplitter.RowCount = 1
        Me.pnlNearFarHedgingTopHeaderVerticalSplitter.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.pnlNearFarHedgingTopHeaderVerticalSplitter.Size = New System.Drawing.Size(1355, 40)
        Me.pnlNearFarHedgingTopHeaderVerticalSplitter.TabIndex = 0
        '
        'btnNearFarHedgingStop
        '
        Me.btnNearFarHedgingStop.Dock = System.Windows.Forms.DockStyle.Fill
        Me.btnNearFarHedgingStop.Location = New System.Drawing.Point(94, 4)
        Me.btnNearFarHedgingStop.Margin = New System.Windows.Forms.Padding(4)
        Me.btnNearFarHedgingStop.Name = "btnNearFarHedgingStop"
        Me.btnNearFarHedgingStop.Size = New System.Drawing.Size(82, 32)
        Me.btnNearFarHedgingStop.TabIndex = 10
        Me.btnNearFarHedgingStop.Text = "Stop"
        Me.btnNearFarHedgingStop.UseVisualStyleBackColor = True
        '
        'btnNearFarHedgingStart
        '
        Me.btnNearFarHedgingStart.Dock = System.Windows.Forms.DockStyle.Fill
        Me.btnNearFarHedgingStart.Location = New System.Drawing.Point(4, 4)
        Me.btnNearFarHedgingStart.Margin = New System.Windows.Forms.Padding(4)
        Me.btnNearFarHedgingStart.Name = "btnNearFarHedgingStart"
        Me.btnNearFarHedgingStart.Size = New System.Drawing.Size(82, 32)
        Me.btnNearFarHedgingStart.TabIndex = 2
        Me.btnNearFarHedgingStart.Text = "Start"
        Me.btnNearFarHedgingStart.UseVisualStyleBackColor = True
        '
        'Panel5
        '
        Me.Panel5.Controls.Add(Me.blbNearFarHedgingTickerStatus)
        Me.Panel5.Controls.Add(Me.lblNearFarHedgingTickerStatus)
        Me.Panel5.Location = New System.Drawing.Point(1201, 4)
        Me.Panel5.Margin = New System.Windows.Forms.Padding(4)
        Me.Panel5.Name = "Panel5"
        Me.Panel5.Size = New System.Drawing.Size(146, 31)
        Me.Panel5.TabIndex = 9
        '
        'blbNearFarHedgingTickerStatus
        '
        Me.blbNearFarHedgingTickerStatus.Color = System.Drawing.Color.Pink
        Me.blbNearFarHedgingTickerStatus.Dock = System.Windows.Forms.DockStyle.Right
        Me.blbNearFarHedgingTickerStatus.Location = New System.Drawing.Point(99, 0)
        Me.blbNearFarHedgingTickerStatus.Margin = New System.Windows.Forms.Padding(4)
        Me.blbNearFarHedgingTickerStatus.Name = "blbNearFarHedgingTickerStatus"
        Me.blbNearFarHedgingTickerStatus.On = True
        Me.blbNearFarHedgingTickerStatus.Size = New System.Drawing.Size(47, 31)
        Me.blbNearFarHedgingTickerStatus.TabIndex = 7
        Me.blbNearFarHedgingTickerStatus.Text = "LedBulb1"
        '
        'lblNearFarHedgingTickerStatus
        '
        Me.lblNearFarHedgingTickerStatus.AutoSize = True
        Me.lblNearFarHedgingTickerStatus.Location = New System.Drawing.Point(9, 9)
        Me.lblNearFarHedgingTickerStatus.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lblNearFarHedgingTickerStatus.Name = "lblNearFarHedgingTickerStatus"
        Me.lblNearFarHedgingTickerStatus.Size = New System.Drawing.Size(91, 17)
        Me.lblNearFarHedgingTickerStatus.TabIndex = 9
        Me.lblNearFarHedgingTickerStatus.Text = "Ticker Status"
        '
        'btnNearFarHedgingSettings
        '
        Me.btnNearFarHedgingSettings.Dock = System.Windows.Forms.DockStyle.Fill
        Me.btnNearFarHedgingSettings.Location = New System.Drawing.Point(814, 4)
        Me.btnNearFarHedgingSettings.Margin = New System.Windows.Forms.Padding(4)
        Me.btnNearFarHedgingSettings.Name = "btnNearFarHedgingSettings"
        Me.btnNearFarHedgingSettings.Size = New System.Drawing.Size(82, 32)
        Me.btnNearFarHedgingSettings.TabIndex = 11
        Me.btnNearFarHedgingSettings.Text = "Settings"
        Me.btnNearFarHedgingSettings.UseVisualStyleBackColor = True
        '
        'linklblNearFarHedgingTradableInstrument
        '
        Me.linklblNearFarHedgingTradableInstrument.AutoSize = True
        Me.linklblNearFarHedgingTradableInstrument.Dock = System.Windows.Forms.DockStyle.Fill
        Me.linklblNearFarHedgingTradableInstrument.Enabled = False
        Me.linklblNearFarHedgingTradableInstrument.Location = New System.Drawing.Point(903, 0)
        Me.linklblNearFarHedgingTradableInstrument.Name = "linklblNearFarHedgingTradableInstrument"
        Me.linklblNearFarHedgingTradableInstrument.Size = New System.Drawing.Size(220, 40)
        Me.linklblNearFarHedgingTradableInstrument.TabIndex = 12
        Me.linklblNearFarHedgingTradableInstrument.TabStop = True
        Me.linklblNearFarHedgingTradableInstrument.Text = "Tradable Instruments: 0"
        Me.linklblNearFarHedgingTradableInstrument.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'pnlNearFarHedgingBodyVerticalSplitter
        '
        Me.pnlNearFarHedgingBodyVerticalSplitter.ColumnCount = 2
        Me.pnlNearFarHedgingBodyVerticalSplitter.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 70.0!))
        Me.pnlNearFarHedgingBodyVerticalSplitter.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30.0!))
        Me.pnlNearFarHedgingBodyVerticalSplitter.Controls.Add(Me.pnlNearFarHedgingBodyHorizontalSplitter, 0, 0)
        Me.pnlNearFarHedgingBodyVerticalSplitter.Dock = System.Windows.Forms.DockStyle.Fill
        Me.pnlNearFarHedgingBodyVerticalSplitter.Location = New System.Drawing.Point(4, 52)
        Me.pnlNearFarHedgingBodyVerticalSplitter.Margin = New System.Windows.Forms.Padding(4)
        Me.pnlNearFarHedgingBodyVerticalSplitter.Name = "pnlNearFarHedgingBodyVerticalSplitter"
        Me.pnlNearFarHedgingBodyVerticalSplitter.RowCount = 1
        Me.pnlNearFarHedgingBodyVerticalSplitter.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.pnlNearFarHedgingBodyVerticalSplitter.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 637.0!))
        Me.pnlNearFarHedgingBodyVerticalSplitter.Size = New System.Drawing.Size(1355, 637)
        Me.pnlNearFarHedgingBodyVerticalSplitter.TabIndex = 1
        '
        'pnlNearFarHedgingBodyHorizontalSplitter
        '
        Me.pnlNearFarHedgingBodyHorizontalSplitter.ColumnCount = 1
        Me.pnlNearFarHedgingBodyHorizontalSplitter.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.pnlNearFarHedgingBodyHorizontalSplitter.Controls.Add(Me.lstNearFarHedgingLog, 0, 1)
        Me.pnlNearFarHedgingBodyHorizontalSplitter.Controls.Add(Me.sfdgvNearFarHedgingMainDashboard, 0, 0)
        Me.pnlNearFarHedgingBodyHorizontalSplitter.Dock = System.Windows.Forms.DockStyle.Fill
        Me.pnlNearFarHedgingBodyHorizontalSplitter.Location = New System.Drawing.Point(4, 4)
        Me.pnlNearFarHedgingBodyHorizontalSplitter.Margin = New System.Windows.Forms.Padding(4)
        Me.pnlNearFarHedgingBodyHorizontalSplitter.Name = "pnlNearFarHedgingBodyHorizontalSplitter"
        Me.pnlNearFarHedgingBodyHorizontalSplitter.RowCount = 2
        Me.pnlNearFarHedgingBodyHorizontalSplitter.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 70.0!))
        Me.pnlNearFarHedgingBodyHorizontalSplitter.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 30.0!))
        Me.pnlNearFarHedgingBodyHorizontalSplitter.Size = New System.Drawing.Size(940, 629)
        Me.pnlNearFarHedgingBodyHorizontalSplitter.TabIndex = 0
        '
        'lstNearFarHedgingLog
        '
        Me.lstNearFarHedgingLog.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lstNearFarHedgingLog.ForeColor = System.Drawing.Color.FromArgb(CType(CType(29, Byte), Integer), CType(CType(29, Byte), Integer), CType(CType(29, Byte), Integer))
        Me.lstNearFarHedgingLog.FormattingEnabled = True
        Me.lstNearFarHedgingLog.ItemHeight = 16
        Me.lstNearFarHedgingLog.Location = New System.Drawing.Point(4, 444)
        Me.lstNearFarHedgingLog.Margin = New System.Windows.Forms.Padding(4)
        Me.lstNearFarHedgingLog.Name = "lstNearFarHedgingLog"
        Me.lstNearFarHedgingLog.Size = New System.Drawing.Size(932, 181)
        Me.lstNearFarHedgingLog.TabIndex = 9
        '
        'sfdgvNearFarHedgingMainDashboard
        '
        Me.sfdgvNearFarHedgingMainDashboard.AccessibleName = "Table"
        Me.sfdgvNearFarHedgingMainDashboard.AllowDraggingColumns = True
        Me.sfdgvNearFarHedgingMainDashboard.AllowEditing = False
        Me.sfdgvNearFarHedgingMainDashboard.AllowFiltering = True
        Me.sfdgvNearFarHedgingMainDashboard.AllowResizingColumns = True
        Me.sfdgvNearFarHedgingMainDashboard.AutoGenerateColumnsMode = Syncfusion.WinForms.DataGrid.Enums.AutoGenerateColumnsMode.SmartReset
        Me.sfdgvNearFarHedgingMainDashboard.AutoSizeColumnsMode = Syncfusion.WinForms.DataGrid.Enums.AutoSizeColumnsMode.AllCells
        Me.sfdgvNearFarHedgingMainDashboard.Dock = System.Windows.Forms.DockStyle.Fill
        Me.sfdgvNearFarHedgingMainDashboard.Location = New System.Drawing.Point(4, 4)
        Me.sfdgvNearFarHedgingMainDashboard.Margin = New System.Windows.Forms.Padding(4)
        Me.sfdgvNearFarHedgingMainDashboard.Name = "sfdgvNearFarHedgingMainDashboard"
        Me.sfdgvNearFarHedgingMainDashboard.PasteOption = Syncfusion.WinForms.DataGrid.Enums.PasteOptions.None
        Me.sfdgvNearFarHedgingMainDashboard.Size = New System.Drawing.Size(932, 432)
        Me.sfdgvNearFarHedgingMainDashboard.TabIndex = 6
        Me.sfdgvNearFarHedgingMainDashboard.Text = "SfDataGrid1"
        '
        'tabEMA_Supertrend
        '
        Me.tabEMA_Supertrend.Controls.Add(Me.pnlEMA5_20STMainPanelHorizontalSplitter)
        Me.tabEMA_Supertrend.Location = New System.Drawing.Point(4, 25)
        Me.tabEMA_Supertrend.Name = "tabEMA_Supertrend"
        Me.tabEMA_Supertrend.Size = New System.Drawing.Size(1363, 693)
        Me.tabEMA_Supertrend.TabIndex = 3
        Me.tabEMA_Supertrend.Text = "EMA & Supertrend Strategy"
        Me.tabEMA_Supertrend.UseVisualStyleBackColor = True
        '
        'pnlEMA5_20STMainPanelHorizontalSplitter
        '
        Me.pnlEMA5_20STMainPanelHorizontalSplitter.ColumnCount = 1
        Me.pnlEMA5_20STMainPanelHorizontalSplitter.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.pnlEMA5_20STMainPanelHorizontalSplitter.Controls.Add(Me.pnlEMA5_20STTopHeaderVerticalSplitter, 0, 0)
        Me.pnlEMA5_20STMainPanelHorizontalSplitter.Controls.Add(Me.pnlEMA5_20STBodyVerticalSplitter, 0, 1)
        Me.pnlEMA5_20STMainPanelHorizontalSplitter.Dock = System.Windows.Forms.DockStyle.Fill
        Me.pnlEMA5_20STMainPanelHorizontalSplitter.Location = New System.Drawing.Point(0, 0)
        Me.pnlEMA5_20STMainPanelHorizontalSplitter.Margin = New System.Windows.Forms.Padding(4)
        Me.pnlEMA5_20STMainPanelHorizontalSplitter.Name = "pnlEMA5_20STMainPanelHorizontalSplitter"
        Me.pnlEMA5_20STMainPanelHorizontalSplitter.RowCount = 2
        Me.pnlEMA5_20STMainPanelHorizontalSplitter.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 7.0!))
        Me.pnlEMA5_20STMainPanelHorizontalSplitter.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 93.0!))
        Me.pnlEMA5_20STMainPanelHorizontalSplitter.Size = New System.Drawing.Size(1363, 693)
        Me.pnlEMA5_20STMainPanelHorizontalSplitter.TabIndex = 1
        '
        'pnlEMA5_20STTopHeaderVerticalSplitter
        '
        Me.pnlEMA5_20STTopHeaderVerticalSplitter.ColumnCount = 15
        Me.pnlEMA5_20STTopHeaderVerticalSplitter.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 6.666668!))
        Me.pnlEMA5_20STTopHeaderVerticalSplitter.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 6.666668!))
        Me.pnlEMA5_20STTopHeaderVerticalSplitter.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 6.666668!))
        Me.pnlEMA5_20STTopHeaderVerticalSplitter.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 6.666668!))
        Me.pnlEMA5_20STTopHeaderVerticalSplitter.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 6.666668!))
        Me.pnlEMA5_20STTopHeaderVerticalSplitter.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 6.666668!))
        Me.pnlEMA5_20STTopHeaderVerticalSplitter.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 6.666668!))
        Me.pnlEMA5_20STTopHeaderVerticalSplitter.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 6.666668!))
        Me.pnlEMA5_20STTopHeaderVerticalSplitter.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 6.666668!))
        Me.pnlEMA5_20STTopHeaderVerticalSplitter.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 6.666668!))
        Me.pnlEMA5_20STTopHeaderVerticalSplitter.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16.70379!))
        Me.pnlEMA5_20STTopHeaderVerticalSplitter.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 0.8166295!))
        Me.pnlEMA5_20STTopHeaderVerticalSplitter.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 1.707498!))
        Me.pnlEMA5_20STTopHeaderVerticalSplitter.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 2.741935!))
        Me.pnlEMA5_20STTopHeaderVerticalSplitter.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 11.29032!))
        Me.pnlEMA5_20STTopHeaderVerticalSplitter.Controls.Add(Me.btnEMA_SupertrendExitAll, 0, 0)
        Me.pnlEMA5_20STTopHeaderVerticalSplitter.Controls.Add(Me.btnEMA_SupertrendStop, 0, 0)
        Me.pnlEMA5_20STTopHeaderVerticalSplitter.Controls.Add(Me.btnEMA_SupertrendStart, 0, 0)
        Me.pnlEMA5_20STTopHeaderVerticalSplitter.Controls.Add(Me.Panel4, 14, 0)
        Me.pnlEMA5_20STTopHeaderVerticalSplitter.Controls.Add(Me.btnEMA_SupertrendSettings, 9, 0)
        Me.pnlEMA5_20STTopHeaderVerticalSplitter.Controls.Add(Me.linklblEMA_SupertrendTradableInstrument, 10, 0)
        Me.pnlEMA5_20STTopHeaderVerticalSplitter.Dock = System.Windows.Forms.DockStyle.Fill
        Me.pnlEMA5_20STTopHeaderVerticalSplitter.Location = New System.Drawing.Point(4, 4)
        Me.pnlEMA5_20STTopHeaderVerticalSplitter.Margin = New System.Windows.Forms.Padding(4)
        Me.pnlEMA5_20STTopHeaderVerticalSplitter.Name = "pnlEMA5_20STTopHeaderVerticalSplitter"
        Me.pnlEMA5_20STTopHeaderVerticalSplitter.RowCount = 1
        Me.pnlEMA5_20STTopHeaderVerticalSplitter.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.pnlEMA5_20STTopHeaderVerticalSplitter.Size = New System.Drawing.Size(1355, 40)
        Me.pnlEMA5_20STTopHeaderVerticalSplitter.TabIndex = 0
        '
        'btnEMA_SupertrendExitAll
        '
        Me.btnEMA_SupertrendExitAll.Dock = System.Windows.Forms.DockStyle.Fill
        Me.btnEMA_SupertrendExitAll.Enabled = False
        Me.btnEMA_SupertrendExitAll.Location = New System.Drawing.Point(184, 4)
        Me.btnEMA_SupertrendExitAll.Margin = New System.Windows.Forms.Padding(4)
        Me.btnEMA_SupertrendExitAll.Name = "btnEMA_SupertrendExitAll"
        Me.btnEMA_SupertrendExitAll.Size = New System.Drawing.Size(82, 32)
        Me.btnEMA_SupertrendExitAll.TabIndex = 13
        Me.btnEMA_SupertrendExitAll.Text = "Exit All"
        Me.btnEMA_SupertrendExitAll.UseVisualStyleBackColor = True
        '
        'btnEMA_SupertrendStop
        '
        Me.btnEMA_SupertrendStop.Dock = System.Windows.Forms.DockStyle.Fill
        Me.btnEMA_SupertrendStop.Location = New System.Drawing.Point(94, 4)
        Me.btnEMA_SupertrendStop.Margin = New System.Windows.Forms.Padding(4)
        Me.btnEMA_SupertrendStop.Name = "btnEMA_SupertrendStop"
        Me.btnEMA_SupertrendStop.Size = New System.Drawing.Size(82, 32)
        Me.btnEMA_SupertrendStop.TabIndex = 10
        Me.btnEMA_SupertrendStop.Text = "Stop"
        Me.btnEMA_SupertrendStop.UseVisualStyleBackColor = True
        '
        'btnEMA_SupertrendStart
        '
        Me.btnEMA_SupertrendStart.Dock = System.Windows.Forms.DockStyle.Fill
        Me.btnEMA_SupertrendStart.Location = New System.Drawing.Point(4, 4)
        Me.btnEMA_SupertrendStart.Margin = New System.Windows.Forms.Padding(4)
        Me.btnEMA_SupertrendStart.Name = "btnEMA_SupertrendStart"
        Me.btnEMA_SupertrendStart.Size = New System.Drawing.Size(82, 32)
        Me.btnEMA_SupertrendStart.TabIndex = 2
        Me.btnEMA_SupertrendStart.Text = "Start"
        Me.btnEMA_SupertrendStart.UseVisualStyleBackColor = True
        '
        'Panel4
        '
        Me.Panel4.Controls.Add(Me.blbEMA_SupertrendTickerStatus)
        Me.Panel4.Controls.Add(Me.lblEMA_SupertrendTickerStatus)
        Me.Panel4.Location = New System.Drawing.Point(1201, 4)
        Me.Panel4.Margin = New System.Windows.Forms.Padding(4)
        Me.Panel4.Name = "Panel4"
        Me.Panel4.Size = New System.Drawing.Size(146, 31)
        Me.Panel4.TabIndex = 9
        '
        'blbEMA_SupertrendTickerStatus
        '
        Me.blbEMA_SupertrendTickerStatus.Color = System.Drawing.Color.Pink
        Me.blbEMA_SupertrendTickerStatus.Dock = System.Windows.Forms.DockStyle.Right
        Me.blbEMA_SupertrendTickerStatus.Location = New System.Drawing.Point(99, 0)
        Me.blbEMA_SupertrendTickerStatus.Margin = New System.Windows.Forms.Padding(4)
        Me.blbEMA_SupertrendTickerStatus.Name = "blbEMA_SupertrendTickerStatus"
        Me.blbEMA_SupertrendTickerStatus.On = True
        Me.blbEMA_SupertrendTickerStatus.Size = New System.Drawing.Size(47, 31)
        Me.blbEMA_SupertrendTickerStatus.TabIndex = 7
        Me.blbEMA_SupertrendTickerStatus.Text = "LedBulb1"
        '
        'lblEMA_SupertrendTickerStatus
        '
        Me.lblEMA_SupertrendTickerStatus.AutoSize = True
        Me.lblEMA_SupertrendTickerStatus.Location = New System.Drawing.Point(9, 9)
        Me.lblEMA_SupertrendTickerStatus.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lblEMA_SupertrendTickerStatus.Name = "lblEMA_SupertrendTickerStatus"
        Me.lblEMA_SupertrendTickerStatus.Size = New System.Drawing.Size(91, 17)
        Me.lblEMA_SupertrendTickerStatus.TabIndex = 9
        Me.lblEMA_SupertrendTickerStatus.Text = "Ticker Status"
        '
        'btnEMA_SupertrendSettings
        '
        Me.btnEMA_SupertrendSettings.Dock = System.Windows.Forms.DockStyle.Fill
        Me.btnEMA_SupertrendSettings.Location = New System.Drawing.Point(814, 4)
        Me.btnEMA_SupertrendSettings.Margin = New System.Windows.Forms.Padding(4)
        Me.btnEMA_SupertrendSettings.Name = "btnEMA_SupertrendSettings"
        Me.btnEMA_SupertrendSettings.Size = New System.Drawing.Size(82, 32)
        Me.btnEMA_SupertrendSettings.TabIndex = 11
        Me.btnEMA_SupertrendSettings.Text = "Settings"
        Me.btnEMA_SupertrendSettings.UseVisualStyleBackColor = True
        '
        'linklblEMA_SupertrendTradableInstrument
        '
        Me.linklblEMA_SupertrendTradableInstrument.AutoSize = True
        Me.linklblEMA_SupertrendTradableInstrument.Dock = System.Windows.Forms.DockStyle.Fill
        Me.linklblEMA_SupertrendTradableInstrument.Enabled = False
        Me.linklblEMA_SupertrendTradableInstrument.Location = New System.Drawing.Point(903, 0)
        Me.linklblEMA_SupertrendTradableInstrument.Name = "linklblEMA_SupertrendTradableInstrument"
        Me.linklblEMA_SupertrendTradableInstrument.Size = New System.Drawing.Size(220, 40)
        Me.linklblEMA_SupertrendTradableInstrument.TabIndex = 12
        Me.linklblEMA_SupertrendTradableInstrument.TabStop = True
        Me.linklblEMA_SupertrendTradableInstrument.Text = "Tradable Instruments: 0"
        Me.linklblEMA_SupertrendTradableInstrument.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'pnlEMA5_20STBodyVerticalSplitter
        '
        Me.pnlEMA5_20STBodyVerticalSplitter.ColumnCount = 2
        Me.pnlEMA5_20STBodyVerticalSplitter.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 70.0!))
        Me.pnlEMA5_20STBodyVerticalSplitter.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30.0!))
        Me.pnlEMA5_20STBodyVerticalSplitter.Controls.Add(Me.pnlEMA5_20STBodyHorizontalSplitter, 0, 0)
        Me.pnlEMA5_20STBodyVerticalSplitter.Dock = System.Windows.Forms.DockStyle.Fill
        Me.pnlEMA5_20STBodyVerticalSplitter.Location = New System.Drawing.Point(4, 52)
        Me.pnlEMA5_20STBodyVerticalSplitter.Margin = New System.Windows.Forms.Padding(4)
        Me.pnlEMA5_20STBodyVerticalSplitter.Name = "pnlEMA5_20STBodyVerticalSplitter"
        Me.pnlEMA5_20STBodyVerticalSplitter.RowCount = 1
        Me.pnlEMA5_20STBodyVerticalSplitter.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.pnlEMA5_20STBodyVerticalSplitter.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 637.0!))
        Me.pnlEMA5_20STBodyVerticalSplitter.Size = New System.Drawing.Size(1355, 637)
        Me.pnlEMA5_20STBodyVerticalSplitter.TabIndex = 1
        '
        'pnlEMA5_20STBodyHorizontalSplitter
        '
        Me.pnlEMA5_20STBodyHorizontalSplitter.ColumnCount = 1
        Me.pnlEMA5_20STBodyHorizontalSplitter.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.pnlEMA5_20STBodyHorizontalSplitter.Controls.Add(Me.lstEMA_SupertrendLog, 0, 1)
        Me.pnlEMA5_20STBodyHorizontalSplitter.Controls.Add(Me.sfdgvEMA_SupertrendMainDashboard, 0, 0)
        Me.pnlEMA5_20STBodyHorizontalSplitter.Dock = System.Windows.Forms.DockStyle.Fill
        Me.pnlEMA5_20STBodyHorizontalSplitter.Location = New System.Drawing.Point(4, 4)
        Me.pnlEMA5_20STBodyHorizontalSplitter.Margin = New System.Windows.Forms.Padding(4)
        Me.pnlEMA5_20STBodyHorizontalSplitter.Name = "pnlEMA5_20STBodyHorizontalSplitter"
        Me.pnlEMA5_20STBodyHorizontalSplitter.RowCount = 2
        Me.pnlEMA5_20STBodyHorizontalSplitter.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 70.0!))
        Me.pnlEMA5_20STBodyHorizontalSplitter.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 30.0!))
        Me.pnlEMA5_20STBodyHorizontalSplitter.Size = New System.Drawing.Size(940, 629)
        Me.pnlEMA5_20STBodyHorizontalSplitter.TabIndex = 0
        '
        'lstEMA_SupertrendLog
        '
        Me.lstEMA_SupertrendLog.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lstEMA_SupertrendLog.ForeColor = System.Drawing.Color.FromArgb(CType(CType(29, Byte), Integer), CType(CType(29, Byte), Integer), CType(CType(29, Byte), Integer))
        Me.lstEMA_SupertrendLog.FormattingEnabled = True
        Me.lstEMA_SupertrendLog.ItemHeight = 16
        Me.lstEMA_SupertrendLog.Location = New System.Drawing.Point(4, 444)
        Me.lstEMA_SupertrendLog.Margin = New System.Windows.Forms.Padding(4)
        Me.lstEMA_SupertrendLog.Name = "lstEMA_SupertrendLog"
        Me.lstEMA_SupertrendLog.Size = New System.Drawing.Size(932, 181)
        Me.lstEMA_SupertrendLog.TabIndex = 9
        '
        'sfdgvEMA_SupertrendMainDashboard
        '
        Me.sfdgvEMA_SupertrendMainDashboard.AccessibleName = "Table"
        Me.sfdgvEMA_SupertrendMainDashboard.AllowDraggingColumns = True
        Me.sfdgvEMA_SupertrendMainDashboard.AllowEditing = False
        Me.sfdgvEMA_SupertrendMainDashboard.AllowFiltering = True
        Me.sfdgvEMA_SupertrendMainDashboard.AllowResizingColumns = True
        Me.sfdgvEMA_SupertrendMainDashboard.AutoGenerateColumnsMode = Syncfusion.WinForms.DataGrid.Enums.AutoGenerateColumnsMode.SmartReset
        Me.sfdgvEMA_SupertrendMainDashboard.AutoSizeColumnsMode = Syncfusion.WinForms.DataGrid.Enums.AutoSizeColumnsMode.AllCells
        Me.sfdgvEMA_SupertrendMainDashboard.Dock = System.Windows.Forms.DockStyle.Fill
        Me.sfdgvEMA_SupertrendMainDashboard.Location = New System.Drawing.Point(4, 4)
        Me.sfdgvEMA_SupertrendMainDashboard.Margin = New System.Windows.Forms.Padding(4)
        Me.sfdgvEMA_SupertrendMainDashboard.Name = "sfdgvEMA_SupertrendMainDashboard"
        Me.sfdgvEMA_SupertrendMainDashboard.PasteOption = Syncfusion.WinForms.DataGrid.Enums.PasteOptions.None
        Me.sfdgvEMA_SupertrendMainDashboard.Size = New System.Drawing.Size(932, 432)
        Me.sfdgvEMA_SupertrendMainDashboard.TabIndex = 6
        Me.sfdgvEMA_SupertrendMainDashboard.Text = "SfDataGrid1"
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
        Me.pnlMomentumReversalTopHeaderVerticalSplitter.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16.70379!))
        Me.pnlMomentumReversalTopHeaderVerticalSplitter.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 0.8166295!))
        Me.pnlMomentumReversalTopHeaderVerticalSplitter.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 1.707498!))
        Me.pnlMomentumReversalTopHeaderVerticalSplitter.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 2.741935!))
        Me.pnlMomentumReversalTopHeaderVerticalSplitter.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 11.29032!))
        Me.pnlMomentumReversalTopHeaderVerticalSplitter.Controls.Add(Me.btnMomentumReversalStop, 0, 0)
        Me.pnlMomentumReversalTopHeaderVerticalSplitter.Controls.Add(Me.btnMomentumReversalStart, 0, 0)
        Me.pnlMomentumReversalTopHeaderVerticalSplitter.Controls.Add(Me.Panel1, 14, 0)
        Me.pnlMomentumReversalTopHeaderVerticalSplitter.Controls.Add(Me.btnMomentumReversalSettings, 9, 0)
        Me.pnlMomentumReversalTopHeaderVerticalSplitter.Controls.Add(Me.linklblMomentumReversalTradableInstrument, 10, 0)
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
        Me.Panel1.Location = New System.Drawing.Point(1189, 4)
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
        'linklblMomentumReversalTradableInstrument
        '
        Me.linklblMomentumReversalTradableInstrument.AutoSize = True
        Me.linklblMomentumReversalTradableInstrument.Dock = System.Windows.Forms.DockStyle.Fill
        Me.linklblMomentumReversalTradableInstrument.Enabled = False
        Me.linklblMomentumReversalTradableInstrument.Location = New System.Drawing.Point(893, 0)
        Me.linklblMomentumReversalTradableInstrument.Name = "linklblMomentumReversalTradableInstrument"
        Me.linklblMomentumReversalTradableInstrument.Size = New System.Drawing.Size(219, 39)
        Me.linklblMomentumReversalTradableInstrument.TabIndex = 12
        Me.linklblMomentumReversalTradableInstrument.TabStop = True
        Me.linklblMomentumReversalTradableInstrument.Text = "Tradable Instruments: 0"
        Me.linklblMomentumReversalTradableInstrument.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'pnlMomentumReversalBodyVerticalSplitter
        '
        Me.pnlMomentumReversalBodyVerticalSplitter.ColumnCount = 2
        Me.pnlMomentumReversalBodyVerticalSplitter.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 70.0!))
        Me.pnlMomentumReversalBodyVerticalSplitter.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30.0!))
        Me.pnlMomentumReversalBodyVerticalSplitter.Controls.Add(Me.pnlMomentumReversalBodyHorizontalSplitter, 0, 0)
        Me.pnlMomentumReversalBodyVerticalSplitter.Dock = System.Windows.Forms.DockStyle.Fill
        Me.pnlMomentumReversalBodyVerticalSplitter.Location = New System.Drawing.Point(4, 51)
        Me.pnlMomentumReversalBodyVerticalSplitter.Margin = New System.Windows.Forms.Padding(4)
        Me.pnlMomentumReversalBodyVerticalSplitter.Name = "pnlMomentumReversalBodyVerticalSplitter"
        Me.pnlMomentumReversalBodyVerticalSplitter.RowCount = 1
        Me.pnlMomentumReversalBodyVerticalSplitter.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.pnlMomentumReversalBodyVerticalSplitter.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 630.0!))
        Me.pnlMomentumReversalBodyVerticalSplitter.Size = New System.Drawing.Size(1347, 630)
        Me.pnlMomentumReversalBodyVerticalSplitter.TabIndex = 1
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
        Me.pnlOHLTopHeaderVerticalSplitter.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16.62955!))
        Me.pnlOHLTopHeaderVerticalSplitter.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 0.8166295!))
        Me.pnlOHLTopHeaderVerticalSplitter.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 1.781737!))
        Me.pnlOHLTopHeaderVerticalSplitter.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 2.741935!))
        Me.pnlOHLTopHeaderVerticalSplitter.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 11.29032!))
        Me.pnlOHLTopHeaderVerticalSplitter.Controls.Add(Me.btnOHLStop, 0, 0)
        Me.pnlOHLTopHeaderVerticalSplitter.Controls.Add(Me.btnOHLStart, 0, 0)
        Me.pnlOHLTopHeaderVerticalSplitter.Controls.Add(Me.Panel2, 14, 0)
        Me.pnlOHLTopHeaderVerticalSplitter.Controls.Add(Me.btnOHLSettings, 9, 0)
        Me.pnlOHLTopHeaderVerticalSplitter.Controls.Add(Me.linklblOHLTradableInstruments, 10, 0)
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
        Me.Panel2.Location = New System.Drawing.Point(1189, 4)
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
        'btnOHLSettings
        '
        Me.btnOHLSettings.Dock = System.Windows.Forms.DockStyle.Fill
        Me.btnOHLSettings.Location = New System.Drawing.Point(805, 4)
        Me.btnOHLSettings.Margin = New System.Windows.Forms.Padding(4)
        Me.btnOHLSettings.Name = "btnOHLSettings"
        Me.btnOHLSettings.Size = New System.Drawing.Size(81, 31)
        Me.btnOHLSettings.TabIndex = 12
        Me.btnOHLSettings.Text = "Settings"
        Me.btnOHLSettings.UseVisualStyleBackColor = True
        '
        'linklblOHLTradableInstruments
        '
        Me.linklblOHLTradableInstruments.AutoSize = True
        Me.linklblOHLTradableInstruments.Dock = System.Windows.Forms.DockStyle.Fill
        Me.linklblOHLTradableInstruments.Enabled = False
        Me.linklblOHLTradableInstruments.Location = New System.Drawing.Point(893, 0)
        Me.linklblOHLTradableInstruments.Name = "linklblOHLTradableInstruments"
        Me.linklblOHLTradableInstruments.Size = New System.Drawing.Size(218, 39)
        Me.linklblOHLTradableInstruments.TabIndex = 13
        Me.linklblOHLTradableInstruments.TabStop = True
        Me.linklblOHLTradableInstruments.Text = "Tradable Instruments: 0"
        Me.linklblOHLTradableInstruments.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
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
        Me.pnlAmiSignalTopHeaderVerticalSplitter.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 17.34317!))
        Me.pnlAmiSignalTopHeaderVerticalSplitter.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 0.5904059!))
        Me.pnlAmiSignalTopHeaderVerticalSplitter.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 1.254613!))
        Me.pnlAmiSignalTopHeaderVerticalSplitter.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 2.741935!))
        Me.pnlAmiSignalTopHeaderVerticalSplitter.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 11.29032!))
        Me.pnlAmiSignalTopHeaderVerticalSplitter.Controls.Add(Me.btnAmiSignalStop, 0, 0)
        Me.pnlAmiSignalTopHeaderVerticalSplitter.Controls.Add(Me.btnAmiSignalStart, 0, 0)
        Me.pnlAmiSignalTopHeaderVerticalSplitter.Controls.Add(Me.Panel3, 14, 0)
        Me.pnlAmiSignalTopHeaderVerticalSplitter.Controls.Add(Me.btnAmiSignalSettings, 9, 0)
        Me.pnlAmiSignalTopHeaderVerticalSplitter.Controls.Add(Me.linklblAmiSignalTradableInstrument, 10, 0)
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
        Me.btnAmiSignalStop.Location = New System.Drawing.Point(94, 4)
        Me.btnAmiSignalStop.Margin = New System.Windows.Forms.Padding(4)
        Me.btnAmiSignalStop.Name = "btnAmiSignalStop"
        Me.btnAmiSignalStop.Size = New System.Drawing.Size(82, 32)
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
        Me.btnAmiSignalStart.Size = New System.Drawing.Size(82, 32)
        Me.btnAmiSignalStart.TabIndex = 2
        Me.btnAmiSignalStart.Text = "Start"
        Me.btnAmiSignalStart.UseVisualStyleBackColor = True
        '
        'Panel3
        '
        Me.Panel3.Controls.Add(Me.blbAmiSignalTickerStatus)
        Me.Panel3.Controls.Add(Me.lblAmiSignalTickerStatus)
        Me.Panel3.Location = New System.Drawing.Point(1201, 4)
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
        'btnAmiSignalSettings
        '
        Me.btnAmiSignalSettings.Dock = System.Windows.Forms.DockStyle.Fill
        Me.btnAmiSignalSettings.Location = New System.Drawing.Point(814, 4)
        Me.btnAmiSignalSettings.Margin = New System.Windows.Forms.Padding(4)
        Me.btnAmiSignalSettings.Name = "btnAmiSignalSettings"
        Me.btnAmiSignalSettings.Size = New System.Drawing.Size(82, 32)
        Me.btnAmiSignalSettings.TabIndex = 12
        Me.btnAmiSignalSettings.Text = "Settings"
        Me.btnAmiSignalSettings.UseVisualStyleBackColor = True
        '
        'linklblAmiSignalTradableInstrument
        '
        Me.linklblAmiSignalTradableInstrument.AutoSize = True
        Me.linklblAmiSignalTradableInstrument.Dock = System.Windows.Forms.DockStyle.Fill
        Me.linklblAmiSignalTradableInstrument.Enabled = False
        Me.linklblAmiSignalTradableInstrument.Location = New System.Drawing.Point(903, 0)
        Me.linklblAmiSignalTradableInstrument.Name = "linklblAmiSignalTradableInstrument"
        Me.linklblAmiSignalTradableInstrument.Size = New System.Drawing.Size(229, 40)
        Me.linklblAmiSignalTradableInstrument.TabIndex = 13
        Me.linklblAmiSignalTradableInstrument.TabStop = True
        Me.linklblAmiSignalTradableInstrument.Text = "Tradable Instruments: 0"
        Me.linklblAmiSignalTradableInstrument.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
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
        'tmrEMA_SupertrendTickerStatus
        '
        Me.tmrEMA_SupertrendTickerStatus.Enabled = True
        '
        'tmrNearFarHedgingTickerStatus
        '
        Me.tmrNearFarHedgingTickerStatus.Enabled = True
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
        Me.tabNearFarHedging.ResumeLayout(False)
        Me.pnlNearFarHedgingMainPanelHorizontalSplitter.ResumeLayout(False)
        Me.pnlNearFarHedgingTopHeaderVerticalSplitter.ResumeLayout(False)
        Me.pnlNearFarHedgingTopHeaderVerticalSplitter.PerformLayout()
        Me.Panel5.ResumeLayout(False)
        Me.Panel5.PerformLayout()
        Me.pnlNearFarHedgingBodyVerticalSplitter.ResumeLayout(False)
        Me.pnlNearFarHedgingBodyHorizontalSplitter.ResumeLayout(False)
        CType(Me.sfdgvNearFarHedgingMainDashboard, System.ComponentModel.ISupportInitialize).EndInit()
        Me.tabEMA_Supertrend.ResumeLayout(False)
        Me.pnlEMA5_20STMainPanelHorizontalSplitter.ResumeLayout(False)
        Me.pnlEMA5_20STTopHeaderVerticalSplitter.ResumeLayout(False)
        Me.pnlEMA5_20STTopHeaderVerticalSplitter.PerformLayout()
        Me.Panel4.ResumeLayout(False)
        Me.Panel4.PerformLayout()
        Me.pnlEMA5_20STBodyVerticalSplitter.ResumeLayout(False)
        Me.pnlEMA5_20STBodyHorizontalSplitter.ResumeLayout(False)
        CType(Me.sfdgvEMA_SupertrendMainDashboard, System.ComponentModel.ISupportInitialize).EndInit()
        Me.tabMomentumReversal.ResumeLayout(False)
        Me.pnlMomentumReversalMainPanelHorizontalSplitter.ResumeLayout(False)
        Me.pnlMomentumReversalTopHeaderVerticalSplitter.ResumeLayout(False)
        Me.pnlMomentumReversalTopHeaderVerticalSplitter.PerformLayout()
        Me.Panel1.ResumeLayout(False)
        Me.Panel1.PerformLayout()
        Me.pnlMomentumReversalBodyVerticalSplitter.ResumeLayout(False)
        Me.pnlMomentumReversalBodyHorizontalSplitter.ResumeLayout(False)
        CType(Me.sfdgvMomentumReversalMainDashboard, System.ComponentModel.ISupportInitialize).EndInit()
        Me.tabOHL.ResumeLayout(False)
        Me.pnlOHLMainPanelHorizontalSplitter.ResumeLayout(False)
        Me.pnlOHLTopHeaderVerticalSplitter.ResumeLayout(False)
        Me.pnlOHLTopHeaderVerticalSplitter.PerformLayout()
        Me.Panel2.ResumeLayout(False)
        Me.Panel2.PerformLayout()
        Me.pnlOHLBodyVerticalSplitter.ResumeLayout(False)
        CType(Me.PictureBox3, System.ComponentModel.ISupportInitialize).EndInit()
        Me.pnlOHLBodyHorizontalSplitter.ResumeLayout(False)
        CType(Me.sfdgvOHLMainDashboard, System.ComponentModel.ISupportInitialize).EndInit()
        Me.tabAmiSignal.ResumeLayout(False)
        Me.pnlAmiSignalMainPanelHorizontalSplitter.ResumeLayout(False)
        Me.pnlAmiSignalTopHeaderVerticalSplitter.ResumeLayout(False)
        Me.pnlAmiSignalTopHeaderVerticalSplitter.PerformLayout()
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
    Friend WithEvents miOptions As ToolStripMenuItem
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
    Friend WithEvents PictureBox3 As PictureBox
    Friend WithEvents miAdvancedOptions As ToolStripMenuItem
    Friend WithEvents linklblMomentumReversalTradableInstrument As LinkLabel
    Friend WithEvents btnOHLSettings As Button
    Friend WithEvents linklblOHLTradableInstruments As LinkLabel
    Friend WithEvents tabEMA_Supertrend As TabPage
    Friend WithEvents pnlEMA5_20STMainPanelHorizontalSplitter As TableLayoutPanel
    Friend WithEvents pnlEMA5_20STTopHeaderVerticalSplitter As TableLayoutPanel
    Friend WithEvents btnEMA_SupertrendStop As Button
    Friend WithEvents btnEMA_SupertrendStart As Button
    Friend WithEvents Panel4 As Panel
    Friend WithEvents blbEMA_SupertrendTickerStatus As Bulb.LedBulb
    Friend WithEvents lblEMA_SupertrendTickerStatus As Label
    Friend WithEvents btnEMA_SupertrendSettings As Button
    Friend WithEvents linklblEMA_SupertrendTradableInstrument As LinkLabel
    Friend WithEvents pnlEMA5_20STBodyVerticalSplitter As TableLayoutPanel
    Friend WithEvents pnlEMA5_20STBodyHorizontalSplitter As TableLayoutPanel
    Friend WithEvents lstEMA_SupertrendLog As ListBox
    Friend WithEvents sfdgvEMA_SupertrendMainDashboard As Syncfusion.WinForms.DataGrid.SfDataGrid
    Friend WithEvents tmrEMA_SupertrendTickerStatus As Timer
    Friend WithEvents btnEMA_SupertrendExitAll As Button
    Friend WithEvents tabNearFarHedging As TabPage
    Friend WithEvents pnlNearFarHedgingMainPanelHorizontalSplitter As TableLayoutPanel
    Friend WithEvents pnlNearFarHedgingTopHeaderVerticalSplitter As TableLayoutPanel
    Friend WithEvents btnNearFarHedgingStop As Button
    Friend WithEvents btnNearFarHedgingStart As Button
    Friend WithEvents Panel5 As Panel
    Friend WithEvents blbNearFarHedgingTickerStatus As Bulb.LedBulb
    Friend WithEvents lblNearFarHedgingTickerStatus As Label
    Friend WithEvents btnNearFarHedgingSettings As Button
    Friend WithEvents linklblNearFarHedgingTradableInstrument As LinkLabel
    Friend WithEvents pnlNearFarHedgingBodyVerticalSplitter As TableLayoutPanel
    Friend WithEvents pnlNearFarHedgingBodyHorizontalSplitter As TableLayoutPanel
    Friend WithEvents lstNearFarHedgingLog As ListBox
    Friend WithEvents sfdgvNearFarHedgingMainDashboard As Syncfusion.WinForms.DataGrid.SfDataGrid
    Friend WithEvents tmrNearFarHedgingTickerStatus As Timer
    Friend WithEvents btnAmiSignalSettings As Button
    Friend WithEvents linklblAmiSignalTradableInstrument As LinkLabel
End Class
