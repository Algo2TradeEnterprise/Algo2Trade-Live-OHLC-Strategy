<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmMomentumReversalSettings
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmMomentumReversalSettings))
        Me.btnSaveMomentumReversalSettings = New System.Windows.Forms.Button()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.txtZerodhaAPISecret = New System.Windows.Forms.TextBox()
        Me.txtZerodhaAPIKey = New System.Windows.Forms.TextBox()
        Me.txtZerodhaPassword = New System.Windows.Forms.TextBox()
        Me.txtZerodhaUserId = New System.Windows.Forms.TextBox()
        Me.lblZerodhaAPISecret = New System.Windows.Forms.Label()
        Me.lblTargetMultiplier = New System.Windows.Forms.Label()
        Me.lblMaxSLPercentage = New System.Windows.Forms.Label()
        Me.lblCandleWickSizePercentage = New System.Windows.Forms.Label()
        Me.ImageList1 = New System.Windows.Forms.ImageList(Me.components)
        Me.GroupBox1.SuspendLayout()
        Me.SuspendLayout()
        '
        'btnSaveMomentumReversalSettings
        '
        Me.btnSaveMomentumReversalSettings.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnSaveMomentumReversalSettings.ImageKey = "save-icon-36533.png"
        Me.btnSaveMomentumReversalSettings.ImageList = Me.ImageList1
        Me.btnSaveMomentumReversalSettings.Location = New System.Drawing.Point(354, 8)
        Me.btnSaveMomentumReversalSettings.Name = "btnSaveMomentumReversalSettings"
        Me.btnSaveMomentumReversalSettings.Size = New System.Drawing.Size(84, 47)
        Me.btnSaveMomentumReversalSettings.TabIndex = 2
        Me.btnSaveMomentumReversalSettings.Text = "&Save"
        Me.btnSaveMomentumReversalSettings.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.btnSaveMomentumReversalSettings.UseVisualStyleBackColor = True
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.txtZerodhaAPISecret)
        Me.GroupBox1.Controls.Add(Me.txtZerodhaAPIKey)
        Me.GroupBox1.Controls.Add(Me.txtZerodhaPassword)
        Me.GroupBox1.Controls.Add(Me.txtZerodhaUserId)
        Me.GroupBox1.Controls.Add(Me.lblZerodhaAPISecret)
        Me.GroupBox1.Controls.Add(Me.lblTargetMultiplier)
        Me.GroupBox1.Controls.Add(Me.lblMaxSLPercentage)
        Me.GroupBox1.Controls.Add(Me.lblCandleWickSizePercentage)
        Me.GroupBox1.Location = New System.Drawing.Point(3, 3)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(345, 149)
        Me.GroupBox1.TabIndex = 3
        Me.GroupBox1.TabStop = False
        '
        'txtZerodhaAPISecret
        '
        Me.txtZerodhaAPISecret.Location = New System.Drawing.Point(77, 112)
        Me.txtZerodhaAPISecret.Name = "txtZerodhaAPISecret"
        Me.txtZerodhaAPISecret.Size = New System.Drawing.Size(262, 20)
        Me.txtZerodhaAPISecret.TabIndex = 7
        '
        'txtZerodhaAPIKey
        '
        Me.txtZerodhaAPIKey.Location = New System.Drawing.Point(115, 81)
        Me.txtZerodhaAPIKey.Name = "txtZerodhaAPIKey"
        Me.txtZerodhaAPIKey.Size = New System.Drawing.Size(102, 20)
        Me.txtZerodhaAPIKey.TabIndex = 6
        '
        'txtZerodhaPassword
        '
        Me.txtZerodhaPassword.Location = New System.Drawing.Point(115, 49)
        Me.txtZerodhaPassword.Name = "txtZerodhaPassword"
        Me.txtZerodhaPassword.PasswordChar = Global.Microsoft.VisualBasic.ChrW(36)
        Me.txtZerodhaPassword.Size = New System.Drawing.Size(102, 20)
        Me.txtZerodhaPassword.TabIndex = 5
        '
        'txtZerodhaUserId
        '
        Me.txtZerodhaUserId.Location = New System.Drawing.Point(115, 17)
        Me.txtZerodhaUserId.Name = "txtZerodhaUserId"
        Me.txtZerodhaUserId.Size = New System.Drawing.Size(102, 20)
        Me.txtZerodhaUserId.TabIndex = 4
        '
        'lblZerodhaAPISecret
        '
        Me.lblZerodhaAPISecret.AutoSize = True
        Me.lblZerodhaAPISecret.Location = New System.Drawing.Point(7, 115)
        Me.lblZerodhaAPISecret.Name = "lblZerodhaAPISecret"
        Me.lblZerodhaAPISecret.Size = New System.Drawing.Size(58, 13)
        Me.lblZerodhaAPISecret.TabIndex = 3
        Me.lblZerodhaAPISecret.Text = "API Secret"
        '
        'lblTargetMultiplier
        '
        Me.lblTargetMultiplier.AutoSize = True
        Me.lblTargetMultiplier.Location = New System.Drawing.Point(7, 84)
        Me.lblTargetMultiplier.Name = "lblTargetMultiplier"
        Me.lblTargetMultiplier.Size = New System.Drawing.Size(82, 13)
        Me.lblTargetMultiplier.TabIndex = 2
        Me.lblTargetMultiplier.Text = "Target Multiplier"
        '
        'lblMaxSLPercentage
        '
        Me.lblMaxSLPercentage.AutoSize = True
        Me.lblMaxSLPercentage.Location = New System.Drawing.Point(7, 52)
        Me.lblMaxSLPercentage.Name = "lblMaxSLPercentage"
        Me.lblMaxSLPercentage.Size = New System.Drawing.Size(81, 13)
        Me.lblMaxSLPercentage.TabIndex = 1
        Me.lblMaxSLPercentage.Text = "Max Stoploss %"
        '
        'lblCandleWickSizePercentage
        '
        Me.lblCandleWickSizePercentage.AutoSize = True
        Me.lblCandleWickSizePercentage.Location = New System.Drawing.Point(7, 20)
        Me.lblCandleWickSizePercentage.Name = "lblCandleWickSizePercentage"
        Me.lblCandleWickSizePercentage.Size = New System.Drawing.Size(102, 13)
        Me.lblCandleWickSizePercentage.TabIndex = 0
        Me.lblCandleWickSizePercentage.Text = "Candle Wick Size %"
        '
        'ImageList1
        '
        Me.ImageList1.ImageStream = CType(resources.GetObject("ImageList1.ImageStream"), System.Windows.Forms.ImageListStreamer)
        Me.ImageList1.TransparentColor = System.Drawing.Color.Transparent
        Me.ImageList1.Images.SetKeyName(0, "save-icon-36533.png")
        '
        'frmMomentumReversalSettings
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(444, 161)
        Me.Controls.Add(Me.GroupBox1)
        Me.Controls.Add(Me.btnSaveMomentumReversalSettings)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "frmMomentumReversalSettings"
        Me.Text = "Momentume Reversal  - Settings"
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents btnSaveMomentumReversalSettings As Button
    Friend WithEvents GroupBox1 As GroupBox
    Friend WithEvents txtZerodhaAPISecret As TextBox
    Friend WithEvents txtZerodhaAPIKey As TextBox
    Friend WithEvents txtZerodhaPassword As TextBox
    Friend WithEvents txtZerodhaUserId As TextBox
    Friend WithEvents lblZerodhaAPISecret As Label
    Friend WithEvents lblTargetMultiplier As Label
    Friend WithEvents lblMaxSLPercentage As Label
    Friend WithEvents lblCandleWickSizePercentage As Label
    Friend WithEvents ImageList1 As ImageList
End Class
