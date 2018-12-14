<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmMain
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
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
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmMain))
        Me.tabMain = New System.Windows.Forms.TabControl()
        Me.tabSelectedStocks = New System.Windows.Forms.TabPage()
        Me.btnSelectedStocksStartAlgo = New System.Windows.Forms.Button()
        Me.btnSelectedStocksStopAlgo = New System.Windows.Forms.Button()
        Me.btnSelectedStocksFilePathBrowse = New System.Windows.Forms.Button()
        Me.txtSelectedStocksFilePath = New System.Windows.Forms.TextBox()
        Me.lblSelectedStocksFilePath = New System.Windows.Forms.Label()
        Me.lblSelectedStocksProgress = New System.Windows.Forms.Label()
        Me.dgrvSelectedStocksMainView = New System.Windows.Forms.DataGridView()
        Me.tabAllNifty50 = New System.Windows.Forms.TabPage()
        Me.opnFileDialog = New System.Windows.Forms.OpenFileDialog()
        Me.lblSelectedStocksTotalPL = New System.Windows.Forms.Label()
        Me.tabMain.SuspendLayout()
        Me.tabSelectedStocks.SuspendLayout()
        CType(Me.dgrvSelectedStocksMainView, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'tabMain
        '
        Me.tabMain.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.tabMain.Controls.Add(Me.tabSelectedStocks)
        Me.tabMain.Controls.Add(Me.tabAllNifty50)
        Me.tabMain.Location = New System.Drawing.Point(3, 2)
        Me.tabMain.Margin = New System.Windows.Forms.Padding(3, 2, 3, 2)
        Me.tabMain.Name = "tabMain"
        Me.tabMain.SelectedIndex = 0
        Me.tabMain.Size = New System.Drawing.Size(1308, 726)
        Me.tabMain.TabIndex = 0
        '
        'tabSelectedStocks
        '
        Me.tabSelectedStocks.Controls.Add(Me.lblSelectedStocksTotalPL)
        Me.tabSelectedStocks.Controls.Add(Me.btnSelectedStocksStartAlgo)
        Me.tabSelectedStocks.Controls.Add(Me.btnSelectedStocksStopAlgo)
        Me.tabSelectedStocks.Controls.Add(Me.btnSelectedStocksFilePathBrowse)
        Me.tabSelectedStocks.Controls.Add(Me.txtSelectedStocksFilePath)
        Me.tabSelectedStocks.Controls.Add(Me.lblSelectedStocksFilePath)
        Me.tabSelectedStocks.Controls.Add(Me.lblSelectedStocksProgress)
        Me.tabSelectedStocks.Controls.Add(Me.dgrvSelectedStocksMainView)
        Me.tabSelectedStocks.Location = New System.Drawing.Point(4, 25)
        Me.tabSelectedStocks.Margin = New System.Windows.Forms.Padding(3, 2, 3, 2)
        Me.tabSelectedStocks.Name = "tabSelectedStocks"
        Me.tabSelectedStocks.Padding = New System.Windows.Forms.Padding(3, 2, 3, 2)
        Me.tabSelectedStocks.Size = New System.Drawing.Size(1300, 697)
        Me.tabSelectedStocks.TabIndex = 0
        Me.tabSelectedStocks.Text = "Seleced Stocks"
        Me.tabSelectedStocks.UseVisualStyleBackColor = True
        '
        'btnSelectedStocksStartAlgo
        '
        Me.btnSelectedStocksStartAlgo.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnSelectedStocksStartAlgo.Font = New System.Drawing.Font("Segoe UI Semibold", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnSelectedStocksStartAlgo.Location = New System.Drawing.Point(1070, 5)
        Me.btnSelectedStocksStartAlgo.Margin = New System.Windows.Forms.Padding(3, 2, 3, 2)
        Me.btnSelectedStocksStartAlgo.Name = "btnSelectedStocksStartAlgo"
        Me.btnSelectedStocksStartAlgo.Size = New System.Drawing.Size(105, 36)
        Me.btnSelectedStocksStartAlgo.TabIndex = 1
        Me.btnSelectedStocksStartAlgo.Text = "Start"
        Me.btnSelectedStocksStartAlgo.UseVisualStyleBackColor = True
        '
        'btnSelectedStocksStopAlgo
        '
        Me.btnSelectedStocksStopAlgo.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnSelectedStocksStopAlgo.Font = New System.Drawing.Font("Segoe UI Semibold", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnSelectedStocksStopAlgo.Location = New System.Drawing.Point(1190, 5)
        Me.btnSelectedStocksStopAlgo.Margin = New System.Windows.Forms.Padding(3, 2, 3, 2)
        Me.btnSelectedStocksStopAlgo.Name = "btnSelectedStocksStopAlgo"
        Me.btnSelectedStocksStopAlgo.Size = New System.Drawing.Size(105, 36)
        Me.btnSelectedStocksStopAlgo.TabIndex = 2
        Me.btnSelectedStocksStopAlgo.Text = "Stop"
        Me.btnSelectedStocksStopAlgo.UseVisualStyleBackColor = True
        '
        'btnSelectedStocksFilePathBrowse
        '
        Me.btnSelectedStocksFilePathBrowse.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnSelectedStocksFilePathBrowse.Location = New System.Drawing.Point(715, 7)
        Me.btnSelectedStocksFilePathBrowse.Margin = New System.Windows.Forms.Padding(3, 2, 3, 2)
        Me.btnSelectedStocksFilePathBrowse.Name = "btnSelectedStocksFilePathBrowse"
        Me.btnSelectedStocksFilePathBrowse.Size = New System.Drawing.Size(103, 30)
        Me.btnSelectedStocksFilePathBrowse.TabIndex = 0
        Me.btnSelectedStocksFilePathBrowse.Text = "Browse ..."
        Me.btnSelectedStocksFilePathBrowse.UseVisualStyleBackColor = True
        '
        'txtSelectedStocksFilePath
        '
        Me.txtSelectedStocksFilePath.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtSelectedStocksFilePath.Location = New System.Drawing.Point(149, 7)
        Me.txtSelectedStocksFilePath.Margin = New System.Windows.Forms.Padding(3, 2, 3, 2)
        Me.txtSelectedStocksFilePath.Name = "txtSelectedStocksFilePath"
        Me.txtSelectedStocksFilePath.Size = New System.Drawing.Size(561, 27)
        Me.txtSelectedStocksFilePath.TabIndex = 1000
        '
        'lblSelectedStocksFilePath
        '
        Me.lblSelectedStocksFilePath.AutoSize = True
        Me.lblSelectedStocksFilePath.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblSelectedStocksFilePath.Location = New System.Drawing.Point(9, 10)
        Me.lblSelectedStocksFilePath.Name = "lblSelectedStocksFilePath"
        Me.lblSelectedStocksFilePath.Size = New System.Drawing.Size(134, 20)
        Me.lblSelectedStocksFilePath.TabIndex = 1003
        Me.lblSelectedStocksFilePath.Text = "Choose a CSV File :"
        '
        'lblSelectedStocksProgress
        '
        Me.lblSelectedStocksProgress.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblSelectedStocksProgress.Location = New System.Drawing.Point(5, 63)
        Me.lblSelectedStocksProgress.Name = "lblSelectedStocksProgress"
        Me.lblSelectedStocksProgress.Size = New System.Drawing.Size(1288, 39)
        Me.lblSelectedStocksProgress.TabIndex = 1002
        Me.lblSelectedStocksProgress.Text = "Progess Status ....."
        '
        'dgrvSelectedStocksMainView
        '
        Me.dgrvSelectedStocksMainView.AllowUserToAddRows = False
        Me.dgrvSelectedStocksMainView.AllowUserToDeleteRows = False
        Me.dgrvSelectedStocksMainView.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.dgrvSelectedStocksMainView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgrvSelectedStocksMainView.Location = New System.Drawing.Point(5, 108)
        Me.dgrvSelectedStocksMainView.Margin = New System.Windows.Forms.Padding(3, 2, 3, 2)
        Me.dgrvSelectedStocksMainView.Name = "dgrvSelectedStocksMainView"
        Me.dgrvSelectedStocksMainView.ReadOnly = True
        Me.dgrvSelectedStocksMainView.RowTemplate.Height = 24
        Me.dgrvSelectedStocksMainView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.dgrvSelectedStocksMainView.Size = New System.Drawing.Size(1288, 549)
        Me.dgrvSelectedStocksMainView.TabIndex = 1001
        '
        'tabAllNifty50
        '
        Me.tabAllNifty50.Location = New System.Drawing.Point(4, 25)
        Me.tabAllNifty50.Margin = New System.Windows.Forms.Padding(3, 2, 3, 2)
        Me.tabAllNifty50.Name = "tabAllNifty50"
        Me.tabAllNifty50.Padding = New System.Windows.Forms.Padding(3, 2, 3, 2)
        Me.tabAllNifty50.Size = New System.Drawing.Size(1300, 697)
        Me.tabAllNifty50.TabIndex = 1
        Me.tabAllNifty50.Text = "All NIFTY 50"
        Me.tabAllNifty50.UseVisualStyleBackColor = True
        '
        'opnFileDialog
        '
        Me.opnFileDialog.Filter = "CSV Files|*.csv"
        '
        'lblSelectedStocksTotalPL
        '
        Me.lblSelectedStocksTotalPL.AutoSize = True
        Me.lblSelectedStocksTotalPL.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblSelectedStocksTotalPL.Location = New System.Drawing.Point(9, 667)
        Me.lblSelectedStocksTotalPL.Name = "lblSelectedStocksTotalPL"
        Me.lblSelectedStocksTotalPL.Size = New System.Drawing.Size(68, 20)
        Me.lblSelectedStocksTotalPL.TabIndex = 1004
        Me.lblSelectedStocksTotalPL.Text = "Total PL :"
        '
        'frmMain
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 16.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1313, 729)
        Me.Controls.Add(Me.tabMain)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Margin = New System.Windows.Forms.Padding(3, 2, 3, 2)
        Me.Name = "frmMain"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Algo2Trade"
        Me.tabMain.ResumeLayout(False)
        Me.tabSelectedStocks.ResumeLayout(False)
        Me.tabSelectedStocks.PerformLayout()
        CType(Me.dgrvSelectedStocksMainView, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents tabMain As TabControl
    Friend WithEvents tabSelectedStocks As TabPage
    Friend WithEvents tabAllNifty50 As TabPage
    Friend WithEvents dgrvSelectedStocksMainView As DataGridView
    Friend WithEvents btnSelectedStocksFilePathBrowse As Button
    Friend WithEvents txtSelectedStocksFilePath As TextBox
    Friend WithEvents lblSelectedStocksFilePath As Label
    Friend WithEvents lblSelectedStocksProgress As Label
    Friend WithEvents btnSelectedStocksStartAlgo As Button
    Friend WithEvents btnSelectedStocksStopAlgo As Button
    Friend WithEvents opnFileDialog As OpenFileDialog
    Friend WithEvents lblSelectedStocksTotalPL As Label
End Class
