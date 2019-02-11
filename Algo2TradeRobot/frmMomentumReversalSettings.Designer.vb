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
        Me.GroupBox1.SuspendLayout()
        Me.SuspendLayout()
        '
        'btnSaveMomentumReversalSettings
        '
        Me.btnSaveMomentumReversalSettings.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnSaveMomentumReversalSettings.ImageKey = "save-icon-36533.png"
        Me.btnSaveMomentumReversalSettings.ImageList = Me.ImageList1
        Me.btnSaveMomentumReversalSettings.Location = New System.Drawing.Point(456, 13)
        Me.btnSaveMomentumReversalSettings.Margin = New System.Windows.Forms.Padding(4)
        Me.btnSaveMomentumReversalSettings.Name = "btnSaveMomentumReversalSettings"
        Me.btnSaveMomentumReversalSettings.Size = New System.Drawing.Size(112, 58)
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
        Me.GroupBox1.Location = New System.Drawing.Point(4, 4)
        Me.GroupBox1.Margin = New System.Windows.Forms.Padding(4)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Padding = New System.Windows.Forms.Padding(4)
        Me.GroupBox1.Size = New System.Drawing.Size(451, 226)
        Me.GroupBox1.TabIndex = 3
        Me.GroupBox1.TabStop = False
        '
        'btnBrowse
        '
        Me.btnBrowse.Font = New System.Drawing.Font("Microsoft Sans Serif", 7.8!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnBrowse.Location = New System.Drawing.Point(404, 188)
        Me.btnBrowse.Name = "btnBrowse"
        Me.btnBrowse.Size = New System.Drawing.Size(40, 23)
        Me.btnBrowse.TabIndex = 10
        Me.btnBrowse.Text = "..."
        Me.btnBrowse.UseVisualStyleBackColor = True
        '
        'txtInstrumentDetalis
        '
        Me.txtInstrumentDetalis.Location = New System.Drawing.Point(175, 188)
        Me.txtInstrumentDetalis.Margin = New System.Windows.Forms.Padding(4)
        Me.txtInstrumentDetalis.Name = "txtInstrumentDetalis"
        Me.txtInstrumentDetalis.Size = New System.Drawing.Size(222, 22)
        Me.txtInstrumentDetalis.TabIndex = 9
        '
        'lblInstrumentDetails
        '
        Me.lblInstrumentDetails.AutoSize = True
        Me.lblInstrumentDetails.Location = New System.Drawing.Point(9, 191)
        Me.lblInstrumentDetails.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lblInstrumentDetails.Name = "lblInstrumentDetails"
        Me.lblInstrumentDetails.Size = New System.Drawing.Size(121, 17)
        Me.lblInstrumentDetails.TabIndex = 8
        Me.lblInstrumentDetails.Text = "Instrument Details"
        '
        'txtSignalTimeFrame
        '
        Me.txtSignalTimeFrame.Location = New System.Drawing.Point(175, 147)
        Me.txtSignalTimeFrame.Margin = New System.Windows.Forms.Padding(4)
        Me.txtSignalTimeFrame.Name = "txtSignalTimeFrame"
        Me.txtSignalTimeFrame.Size = New System.Drawing.Size(255, 22)
        Me.txtSignalTimeFrame.TabIndex = 7
        '
        'txtTargetMultiplier
        '
        Me.txtTargetMultiplier.Location = New System.Drawing.Point(175, 107)
        Me.txtTargetMultiplier.Margin = New System.Windows.Forms.Padding(4)
        Me.txtTargetMultiplier.Name = "txtTargetMultiplier"
        Me.txtTargetMultiplier.Size = New System.Drawing.Size(256, 22)
        Me.txtTargetMultiplier.TabIndex = 6
        '
        'txtMaxSLPercentage
        '
        Me.txtMaxSLPercentage.Location = New System.Drawing.Point(175, 64)
        Me.txtMaxSLPercentage.Margin = New System.Windows.Forms.Padding(4)
        Me.txtMaxSLPercentage.Name = "txtMaxSLPercentage"
        Me.txtMaxSLPercentage.Size = New System.Drawing.Size(256, 22)
        Me.txtMaxSLPercentage.TabIndex = 5
        '
        'txtCandleWickSizePercentage
        '
        Me.txtCandleWickSizePercentage.Location = New System.Drawing.Point(175, 21)
        Me.txtCandleWickSizePercentage.Margin = New System.Windows.Forms.Padding(4)
        Me.txtCandleWickSizePercentage.Name = "txtCandleWickSizePercentage"
        Me.txtCandleWickSizePercentage.Size = New System.Drawing.Size(255, 22)
        Me.txtCandleWickSizePercentage.TabIndex = 4
        '
        'lblSignalTimeFrame
        '
        Me.lblSignalTimeFrame.AutoSize = True
        Me.lblSignalTimeFrame.Location = New System.Drawing.Point(9, 150)
        Me.lblSignalTimeFrame.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lblSignalTimeFrame.Name = "lblSignalTimeFrame"
        Me.lblSignalTimeFrame.Size = New System.Drawing.Size(158, 17)
        Me.lblSignalTimeFrame.TabIndex = 3
        Me.lblSignalTimeFrame.Text = "Signal Time Frame(min)"
        '
        'lblTargetMultiplier
        '
        Me.lblTargetMultiplier.AutoSize = True
        Me.lblTargetMultiplier.Location = New System.Drawing.Point(9, 111)
        Me.lblTargetMultiplier.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lblTargetMultiplier.Name = "lblTargetMultiplier"
        Me.lblTargetMultiplier.Size = New System.Drawing.Size(110, 17)
        Me.lblTargetMultiplier.TabIndex = 2
        Me.lblTargetMultiplier.Text = "Target Multiplier"
        '
        'lblMaxSLPercentage
        '
        Me.lblMaxSLPercentage.AutoSize = True
        Me.lblMaxSLPercentage.Location = New System.Drawing.Point(9, 68)
        Me.lblMaxSLPercentage.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lblMaxSLPercentage.Name = "lblMaxSLPercentage"
        Me.lblMaxSLPercentage.Size = New System.Drawing.Size(107, 17)
        Me.lblMaxSLPercentage.TabIndex = 1
        Me.lblMaxSLPercentage.Text = "Max Stoploss %"
        '
        'lblCandleWickSizePercentage
        '
        Me.lblCandleWickSizePercentage.AutoSize = True
        Me.lblCandleWickSizePercentage.Location = New System.Drawing.Point(9, 25)
        Me.lblCandleWickSizePercentage.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lblCandleWickSizePercentage.Name = "lblCandleWickSizePercentage"
        Me.lblCandleWickSizePercentage.Size = New System.Drawing.Size(133, 17)
        Me.lblCandleWickSizePercentage.TabIndex = 0
        Me.lblCandleWickSizePercentage.Text = "Candle Wick Size %"
        '
        'opnFileMRSettings
        '
        '
        'frmMomentumReversalSettings
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 16.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(575, 241)
        Me.Controls.Add(Me.GroupBox1)
        Me.Controls.Add(Me.btnSaveMomentumReversalSettings)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Margin = New System.Windows.Forms.Padding(4)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmMomentumReversalSettings"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Momentum Reversal  - Settings"
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.ResumeLayout(False)

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
End Class
