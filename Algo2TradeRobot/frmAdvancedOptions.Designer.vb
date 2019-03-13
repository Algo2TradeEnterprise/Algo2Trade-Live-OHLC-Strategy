<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmAdvancedOptions
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmAdvancedOptions))
        Me.tabMain = New System.Windows.Forms.TabControl()
        Me.tabDelaySettings = New System.Windows.Forms.TabPage()
        Me.txtBackToBackOrderCoolOffDelay = New System.Windows.Forms.TextBox()
        Me.lblBackToBackOrderCoolOffDelay = New System.Windows.Forms.Label()
        Me.txtGetInformationDelay = New System.Windows.Forms.TextBox()
        Me.lblGetInformationDelay = New System.Windows.Forms.Label()
        Me.btnSaveDelaySettings = New System.Windows.Forms.Button()
        Me.ImageList1 = New System.Windows.Forms.ImageList(Me.components)
        Me.dtpckrForceRestartTime = New System.Windows.Forms.DateTimePicker()
        Me.lblForceRestartTime = New System.Windows.Forms.Label()
        Me.tabMain.SuspendLayout()
        Me.tabDelaySettings.SuspendLayout()
        Me.SuspendLayout()
        '
        'tabMain
        '
        Me.tabMain.Controls.Add(Me.tabDelaySettings)
        Me.tabMain.Location = New System.Drawing.Point(0, 0)
        Me.tabMain.Name = "tabMain"
        Me.tabMain.SelectedIndex = 0
        Me.tabMain.Size = New System.Drawing.Size(576, 302)
        Me.tabMain.TabIndex = 0
        '
        'tabDelaySettings
        '
        Me.tabDelaySettings.Controls.Add(Me.dtpckrForceRestartTime)
        Me.tabDelaySettings.Controls.Add(Me.lblForceRestartTime)
        Me.tabDelaySettings.Controls.Add(Me.txtBackToBackOrderCoolOffDelay)
        Me.tabDelaySettings.Controls.Add(Me.lblBackToBackOrderCoolOffDelay)
        Me.tabDelaySettings.Controls.Add(Me.txtGetInformationDelay)
        Me.tabDelaySettings.Controls.Add(Me.lblGetInformationDelay)
        Me.tabDelaySettings.Controls.Add(Me.btnSaveDelaySettings)
        Me.tabDelaySettings.Location = New System.Drawing.Point(4, 25)
        Me.tabDelaySettings.Name = "tabDelaySettings"
        Me.tabDelaySettings.Padding = New System.Windows.Forms.Padding(3)
        Me.tabDelaySettings.Size = New System.Drawing.Size(568, 273)
        Me.tabDelaySettings.TabIndex = 0
        Me.tabDelaySettings.Text = "Delay"
        Me.tabDelaySettings.UseVisualStyleBackColor = True
        '
        'txtBackToBackOrderCoolOffDelay
        '
        Me.txtBackToBackOrderCoolOffDelay.Location = New System.Drawing.Point(268, 57)
        Me.txtBackToBackOrderCoolOffDelay.Margin = New System.Windows.Forms.Padding(4)
        Me.txtBackToBackOrderCoolOffDelay.Name = "txtBackToBackOrderCoolOffDelay"
        Me.txtBackToBackOrderCoolOffDelay.Size = New System.Drawing.Size(134, 22)
        Me.txtBackToBackOrderCoolOffDelay.TabIndex = 2
        '
        'lblBackToBackOrderCoolOffDelay
        '
        Me.lblBackToBackOrderCoolOffDelay.AutoSize = True
        Me.lblBackToBackOrderCoolOffDelay.Location = New System.Drawing.Point(6, 58)
        Me.lblBackToBackOrderCoolOffDelay.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lblBackToBackOrderCoolOffDelay.Name = "lblBackToBackOrderCoolOffDelay"
        Me.lblBackToBackOrderCoolOffDelay.Size = New System.Drawing.Size(255, 17)
        Me.lblBackToBackOrderCoolOffDelay.TabIndex = 12
        Me.lblBackToBackOrderCoolOffDelay.Text = "BackToBack Order CoolOff Delay (sec)"
        '
        'txtGetInformationDelay
        '
        Me.txtGetInformationDelay.Location = New System.Drawing.Point(268, 17)
        Me.txtGetInformationDelay.Margin = New System.Windows.Forms.Padding(4)
        Me.txtGetInformationDelay.Name = "txtGetInformationDelay"
        Me.txtGetInformationDelay.Size = New System.Drawing.Size(134, 22)
        Me.txtGetInformationDelay.TabIndex = 1
        '
        'lblGetInformationDelay
        '
        Me.lblGetInformationDelay.AutoSize = True
        Me.lblGetInformationDelay.Location = New System.Drawing.Point(6, 18)
        Me.lblGetInformationDelay.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lblGetInformationDelay.Name = "lblGetInformationDelay"
        Me.lblGetInformationDelay.Size = New System.Drawing.Size(181, 17)
        Me.lblGetInformationDelay.TabIndex = 10
        Me.lblGetInformationDelay.Text = "Get Information Delay (sec)"
        '
        'btnSaveDelaySettings
        '
        Me.btnSaveDelaySettings.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnSaveDelaySettings.ImageKey = "save-icon-36533.png"
        Me.btnSaveDelaySettings.ImageList = Me.ImageList1
        Me.btnSaveDelaySettings.Location = New System.Drawing.Point(453, 3)
        Me.btnSaveDelaySettings.Margin = New System.Windows.Forms.Padding(4)
        Me.btnSaveDelaySettings.Name = "btnSaveDelaySettings"
        Me.btnSaveDelaySettings.Size = New System.Drawing.Size(112, 58)
        Me.btnSaveDelaySettings.TabIndex = 0
        Me.btnSaveDelaySettings.Text = "&Save"
        Me.btnSaveDelaySettings.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.btnSaveDelaySettings.UseVisualStyleBackColor = True
        '
        'ImageList1
        '
        Me.ImageList1.ImageStream = CType(resources.GetObject("ImageList1.ImageStream"), System.Windows.Forms.ImageListStreamer)
        Me.ImageList1.TransparentColor = System.Drawing.Color.Transparent
        Me.ImageList1.Images.SetKeyName(0, "save-icon-36533.png")
        '
        'dtpckrForceRestartTime
        '
        Me.dtpckrForceRestartTime.Format = System.Windows.Forms.DateTimePickerFormat.Time
        Me.dtpckrForceRestartTime.Location = New System.Drawing.Point(268, 96)
        Me.dtpckrForceRestartTime.Name = "dtpckrForceRestartTime"
        Me.dtpckrForceRestartTime.ShowUpDown = True
        Me.dtpckrForceRestartTime.Size = New System.Drawing.Size(134, 22)
        Me.dtpckrForceRestartTime.TabIndex = 20
        Me.dtpckrForceRestartTime.Value = New Date(2019, 3, 12, 0, 0, 0, 0)
        '
        'lblForceRestartTime
        '
        Me.lblForceRestartTime.AutoSize = True
        Me.lblForceRestartTime.Location = New System.Drawing.Point(6, 98)
        Me.lblForceRestartTime.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lblForceRestartTime.Name = "lblForceRestartTime"
        Me.lblForceRestartTime.Size = New System.Drawing.Size(129, 17)
        Me.lblForceRestartTime.TabIndex = 21
        Me.lblForceRestartTime.Text = "Force Restart Time"
        '
        'frmAdvancedOptions
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 16.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(575, 299)
        Me.Controls.Add(Me.tabMain)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmAdvancedOptions"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Advanced Options"
        Me.tabMain.ResumeLayout(False)
        Me.tabDelaySettings.ResumeLayout(False)
        Me.tabDelaySettings.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents tabMain As TabControl
    Friend WithEvents tabDelaySettings As TabPage
    Friend WithEvents ImageList1 As ImageList
    Friend WithEvents btnSaveDelaySettings As Button
    Friend WithEvents txtGetInformationDelay As TextBox
    Friend WithEvents lblGetInformationDelay As Label
    Friend WithEvents txtBackToBackOrderCoolOffDelay As TextBox
    Friend WithEvents lblBackToBackOrderCoolOffDelay As Label
    Friend WithEvents dtpckrForceRestartTime As DateTimePicker
    Friend WithEvents lblForceRestartTime As Label
End Class
