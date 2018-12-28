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
        Me.components = New System.ComponentModel.Container()
        Me.lstLog = New System.Windows.Forms.ListBox()
        Me.btnStart = New System.Windows.Forms.Button()
        Me.tmrTickerStatus = New System.Windows.Forms.Timer(Me.components)
        Me.lblTickerStatus = New System.Windows.Forms.Label()
        Me.blbTickerStatus = New Bulb.LedBulb()
        Me.dgMainDashboard = New System.Windows.Forms.DataGridView()
        CType(Me.dgMainDashboard, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'lstLog
        '
        Me.lstLog.FormattingEnabled = True
        Me.lstLog.Location = New System.Drawing.Point(13, 299)
        Me.lstLog.Name = "lstLog"
        Me.lstLog.Size = New System.Drawing.Size(1011, 173)
        Me.lstLog.TabIndex = 0
        '
        'btnStart
        '
        Me.btnStart.Location = New System.Drawing.Point(13, 13)
        Me.btnStart.Name = "btnStart"
        Me.btnStart.Size = New System.Drawing.Size(75, 23)
        Me.btnStart.TabIndex = 1
        Me.btnStart.Text = "Start"
        Me.btnStart.UseVisualStyleBackColor = True
        '
        'tmrTickerStatus
        '
        Me.tmrTickerStatus.Enabled = True
        '
        'lblTickerStatus
        '
        Me.lblTickerStatus.AutoSize = True
        Me.lblTickerStatus.Location = New System.Drawing.Point(918, 18)
        Me.lblTickerStatus.Name = "lblTickerStatus"
        Me.lblTickerStatus.Size = New System.Drawing.Size(70, 13)
        Me.lblTickerStatus.TabIndex = 2
        Me.lblTickerStatus.Text = "Ticker Status"
        '
        'blbTickerStatus
        '
        Me.blbTickerStatus.Color = System.Drawing.Color.Pink
        Me.blbTickerStatus.Location = New System.Drawing.Point(994, 13)
        Me.blbTickerStatus.Name = "blbTickerStatus"
        Me.blbTickerStatus.On = True
        Me.blbTickerStatus.Size = New System.Drawing.Size(75, 23)
        Me.blbTickerStatus.TabIndex = 3
        Me.blbTickerStatus.Text = "LedBulb1"
        '
        'dgMainDashboard
        '
        Me.dgMainDashboard.AllowUserToAddRows = False
        Me.dgMainDashboard.AllowUserToDeleteRows = False
        Me.dgMainDashboard.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgMainDashboard.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically
        Me.dgMainDashboard.Location = New System.Drawing.Point(13, 42)
        Me.dgMainDashboard.MultiSelect = False
        Me.dgMainDashboard.Name = "dgMainDashboard"
        Me.dgMainDashboard.ReadOnly = True
        Me.dgMainDashboard.RowHeadersVisible = False
        Me.dgMainDashboard.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.dgMainDashboard.Size = New System.Drawing.Size(1011, 251)
        Me.dgMainDashboard.TabIndex = 4
        '
        'frmMain
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1036, 484)
        Me.Controls.Add(Me.dgMainDashboard)
        Me.Controls.Add(Me.blbTickerStatus)
        Me.Controls.Add(Me.lblTickerStatus)
        Me.Controls.Add(Me.btnStart)
        Me.Controls.Add(Me.lstLog)
        Me.Name = "frmMain"
        Me.Text = "Form1"
        CType(Me.dgMainDashboard, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents lstLog As ListBox
    Friend WithEvents btnStart As Button
    Friend WithEvents tmrTickerStatus As Timer
    Friend WithEvents lblTickerStatus As Label
    Friend WithEvents blbTickerStatus As Bulb.LedBulb
    Friend WithEvents dgMainDashboard As DataGridView
End Class
