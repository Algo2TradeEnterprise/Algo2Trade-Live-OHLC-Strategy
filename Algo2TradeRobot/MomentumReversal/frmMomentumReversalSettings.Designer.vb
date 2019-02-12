<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmMomentumReversalSettings
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmMomentumReversalSettings))
        Me.btnSaveMomentumReversalSettings = New System.Windows.Forms.Button()
        Me.ImageList1 = New System.Windows.Forms.ImageList(Me.components)
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.btnBrowse = New System.Windows.Forms.Button()
        Me.txtInstrumentDetalis = New System.Windows.Forms.TextBox()
        Me.lblInstrumentDetails = New System.Windows.Forms.Label()
        Me.txtSignalTimeFrame = New System.Windows.Forms.TextBox()
        Me.txtTargetMultiplier = New System.Windows.Forms.TextBox()
        Me.txtMaxSLPercentage = New System.Windows.Forms.TextBox()
        Me.txtCandleWickSizePercentage = New System.Windows.Forms.TextBox()
        Me.lblSignalTimeFrame = New System.Windows.Forms.Label()
        Me.lblTargetMultiplier = New System.Windows.Forms.Label()
        Me.lblMaxSLPercentage = New System.Windows.Forms.Label()
        Me.lblCandleWickSizePercentage = New System.Windows.Forms.Label()
        Me.opnFileMRSettings = New System.Windows.Forms.OpenFileDialog()
        Me.lblStatus = New System.Windows.Forms.Label()
        Me.GroupBox1.SuspendLayout()
        Me.SuspendLayout()
        '
        'btnSaveMomentumReversalSettings
        '
        Me.btnSaveMomentumReversalSettings.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnSaveMomentumReversalSettings.ImageKey = "save-icon-36533.png"
        Me.btnSaveMomentumReversalSettings.ImageList = Me.ImageList1
        Me.btnSaveMomentumReversalSettings.Location = New System.Drawing.Point(342, 11)
        Me.btnSaveMomentumReversalSettings.Name = "btnSaveMomentumReversalSettings"
        Me.btnSaveMomentumReversalSettings.Size = New System.Drawing.Size(84, 47)
        Me.btnSaveMomentumReversalSettings.TabIndex = 2
        Me.btnSaveMomentumReversalSettings.Text = "&Save"
        Me.btnSaveMomentumReversalSettings.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.btnSaveMomentumReversalSettings.UseVisualStyleBackColor = True
        '
        'ImageList1
        '
        Me.ImageList1.ImageStream = CType(resources.GetObject("ImageList1.ImageStream"), System.Windows.Forms.ImageListStreamer)
        Me.ImageList1.TransparentColor = System.Drawing.Color.Transparent
        Me.ImageList1.Images.SetKeyName(0, "save-icon-36533.png")
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.btnBrowse)
        Me.GroupBox1.Controls.Add(Me.txtInstrumentDetalis)
        Me.GroupBox1.Controls.Add(Me.lblInstrumentDetails)
        Me.GroupBox1.Controls.Add(Me.txtSignalTimeFrame)
        Me.GroupBox1.Controls.Add(Me.txtTargetMultiplier)
        Me.GroupBox1.Controls.Add(Me.txtMaxSLPercentage)
        Me.GroupBox1.Controls.Add(Me.txtCandleWickSizePercentage)
        Me.GroupBox1.Controls.Add(Me.lblSignalTimeFrame)
        Me.GroupBox1.Controls.Add(Me.lblTargetMultiplier)
        Me.GroupBox1.Controls.Add(Me.lblMaxSLPercentage)
        Me.GroupBox1.Controls.Add(Me.lblCandleWickSizePercentage)
        Me.GroupBox1.Location = New System.Drawing.Point(3, 3)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(338, 184)
        Me.GroupBox1.TabIndex = 3
        Me.GroupBox1.TabStop = False
        '
        'btnBrowse
        '
        Me.btnBrowse.Font = New System.Drawing.Font("Microsoft Sans Serif", 7.8!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnBrowse.Location = New System.Drawing.Point(303, 153)
        Me.btnBrowse.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.btnBrowse.Name = "btnBrowse"
        Me.btnBrowse.Size = New System.Drawing.Size(30, 19)
        Me.btnBrowse.TabIndex = 10
        Me.btnBrowse.Text = "..."
        Me.btnBrowse.UseVisualStyleBackColor = True
        '
        'txtInstrumentDetalis
        '
        Me.txtInstrumentDetalis.Location = New System.Drawing.Point(131, 153)
        Me.txtInstrumentDetalis.Name = "txtInstrumentDetalis"
        Me.txtInstrumentDetalis.Size = New System.Drawing.Size(168, 20)
        Me.txtInstrumentDetalis.TabIndex = 9
        '
        'lblInstrumentDetails
        '
        Me.lblInstrumentDetails.AutoSize = True
        Me.lblInstrumentDetails.Location = New System.Drawing.Point(7, 155)
        Me.lblInstrumentDetails.Name = "lblInstrumentDetails"
        Me.lblInstrumentDetails.Size = New System.Drawing.Size(91, 13)
        Me.lblInstrumentDetails.TabIndex = 8
        Me.lblInstrumentDetails.Text = "Instrument Details"
        '
        'txtSignalTimeFrame
        '
        Me.txtSignalTimeFrame.Location = New System.Drawing.Point(131, 119)
        Me.txtSignalTimeFrame.Name = "txtSignalTimeFrame"
        Me.txtSignalTimeFrame.Size = New System.Drawing.Size(192, 20)
        Me.txtSignalTimeFrame.TabIndex = 7
        '
        'txtTargetMultiplier
        '
        Me.txtTargetMultiplier.Location = New System.Drawing.Point(131, 87)
        Me.txtTargetMultiplier.Name = "txtTargetMultiplier"
        Me.txtTargetMultiplier.Size = New System.Drawing.Size(193, 20)
        Me.txtTargetMultiplier.TabIndex = 6
        '
        'txtMaxSLPercentage
        '
        Me.txtMaxSLPercentage.Location = New System.Drawing.Point(131, 52)
        Me.txtMaxSLPercentage.Name = "txtMaxSLPercentage"
        Me.txtMaxSLPercentage.Size = New System.Drawing.Size(193, 20)
        Me.txtMaxSLPercentage.TabIndex = 5
        '
        'txtCandleWickSizePercentage
        '
        Me.txtCandleWickSizePercentage.Location = New System.Drawing.Point(131, 17)
        Me.txtCandleWickSizePercentage.Name = "txtCandleWickSizePercentage"
        Me.txtCandleWickSizePercentage.Size = New System.Drawing.Size(192, 20)
        Me.txtCandleWickSizePercentage.TabIndex = 4
        '
        'lblSignalTimeFrame
        '
        Me.lblSignalTimeFrame.AutoSize = True
        Me.lblSignalTimeFrame.Location = New System.Drawing.Point(7, 122)
        Me.lblSignalTimeFrame.Name = "lblSignalTimeFrame"
        Me.lblSignalTimeFrame.Size = New System.Drawing.Size(116, 13)
        Me.lblSignalTimeFrame.TabIndex = 3
        Me.lblSignalTimeFrame.Text = "Signal Time Frame(min)"
        '
        'lblTargetMultiplier
        '
        Me.lblTargetMultiplier.AutoSize = True
        Me.lblTargetMultiplier.Location = New System.Drawing.Point(7, 90)
        Me.lblTargetMultiplier.Name = "lblTargetMultiplier"
        Me.lblTargetMultiplier.Size = New System.Drawing.Size(82, 13)
        Me.lblTargetMultiplier.TabIndex = 2
        Me.lblTargetMultiplier.Text = "Target Multiplier"
        '
        'lblMaxSLPercentage
        '
        Me.lblMaxSLPercentage.AutoSize = True
        Me.lblMaxSLPercentage.Location = New System.Drawing.Point(7, 55)
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
        'opnFileMRSettings
        '
        '
        'lblStatus
        '
        Me.lblStatus.AutoSize = True
        Me.lblStatus.Location = New System.Drawing.Point(3, 194)
        Me.lblStatus.Name = "lblStatus"
        Me.lblStatus.Size = New System.Drawing.Size(180, 13)
        Me.lblStatus.TabIndex = 4
        Me.lblStatus.Text = "Please Wait... Validating settings file."
        '
        'frmMomentumReversalSettings
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(431, 212)
        Me.Controls.Add(Me.lblStatus)
        Me.Controls.Add(Me.GroupBox1)
        Me.Controls.Add(Me.btnSaveMomentumReversalSettings)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmMomentumReversalSettings"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Momentum Reversal  - Settings"
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents btnSaveMomentumReversalSettings As Button
    Friend WithEvents GroupBox1 As GroupBox
    Friend WithEvents txtSignalTimeFrame As TextBox
    Friend WithEvents txtTargetMultiplier As TextBox
    Friend WithEvents txtMaxSLPercentage As TextBox
    Friend WithEvents txtCandleWickSizePercentage As TextBox
    Friend WithEvents lblSignalTimeFrame As Label
    Friend WithEvents lblTargetMultiplier As Label
    Friend WithEvents lblMaxSLPercentage As Label
    Friend WithEvents lblCandleWickSizePercentage As Label
    Friend WithEvents ImageList1 As ImageList
    Friend WithEvents btnBrowse As Button
    Friend WithEvents txtInstrumentDetalis As TextBox
    Friend WithEvents lblInstrumentDetails As Label
    Friend WithEvents opnFileMRSettings As OpenFileDialog
    Friend WithEvents lblStatus As Label
End Class
