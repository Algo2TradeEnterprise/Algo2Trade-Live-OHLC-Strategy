<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmZerodhaUserDetails
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmZerodhaUserDetails))
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.txtZerodhaAPISecret = New System.Windows.Forms.TextBox()
        Me.txtZerodhaAPIKey = New System.Windows.Forms.TextBox()
        Me.txtZerodhaPassword = New System.Windows.Forms.TextBox()
        Me.txtZerodhaUserId = New System.Windows.Forms.TextBox()
        Me.lblZerodhaAPISecret = New System.Windows.Forms.Label()
        Me.lblZerodhaAPIKey = New System.Windows.Forms.Label()
        Me.lblZerodhaPassword = New System.Windows.Forms.Label()
        Me.lblZerodhaUserId = New System.Windows.Forms.Label()
        Me.btnSaveZerodhaUserDetails = New System.Windows.Forms.Button()
        Me.ImageList1 = New System.Windows.Forms.ImageList(Me.components)
        Me.GroupBox1.SuspendLayout()
        Me.SuspendLayout()
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.txtZerodhaAPISecret)
        Me.GroupBox1.Controls.Add(Me.txtZerodhaAPIKey)
        Me.GroupBox1.Controls.Add(Me.txtZerodhaPassword)
        Me.GroupBox1.Controls.Add(Me.txtZerodhaUserId)
        Me.GroupBox1.Controls.Add(Me.lblZerodhaAPISecret)
        Me.GroupBox1.Controls.Add(Me.lblZerodhaAPIKey)
        Me.GroupBox1.Controls.Add(Me.lblZerodhaPassword)
        Me.GroupBox1.Controls.Add(Me.lblZerodhaUserId)
        Me.GroupBox1.Location = New System.Drawing.Point(7, 6)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(345, 149)
        Me.GroupBox1.TabIndex = 0
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
        Me.txtZerodhaAPIKey.Location = New System.Drawing.Point(77, 81)
        Me.txtZerodhaAPIKey.Name = "txtZerodhaAPIKey"
        Me.txtZerodhaAPIKey.Size = New System.Drawing.Size(262, 20)
        Me.txtZerodhaAPIKey.TabIndex = 6
        '
        'txtZerodhaPassword
        '
        Me.txtZerodhaPassword.Location = New System.Drawing.Point(77, 49)
        Me.txtZerodhaPassword.Name = "txtZerodhaPassword"
        Me.txtZerodhaPassword.PasswordChar = Global.Microsoft.VisualBasic.ChrW(36)
        Me.txtZerodhaPassword.Size = New System.Drawing.Size(140, 20)
        Me.txtZerodhaPassword.TabIndex = 5
        '
        'txtZerodhaUserId
        '
        Me.txtZerodhaUserId.Location = New System.Drawing.Point(77, 17)
        Me.txtZerodhaUserId.Name = "txtZerodhaUserId"
        Me.txtZerodhaUserId.Size = New System.Drawing.Size(140, 20)
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
        'lblZerodhaAPIKey
        '
        Me.lblZerodhaAPIKey.AutoSize = True
        Me.lblZerodhaAPIKey.Location = New System.Drawing.Point(7, 84)
        Me.lblZerodhaAPIKey.Name = "lblZerodhaAPIKey"
        Me.lblZerodhaAPIKey.Size = New System.Drawing.Size(45, 13)
        Me.lblZerodhaAPIKey.TabIndex = 2
        Me.lblZerodhaAPIKey.Text = "API Key"
        '
        'lblZerodhaPassword
        '
        Me.lblZerodhaPassword.AutoSize = True
        Me.lblZerodhaPassword.Location = New System.Drawing.Point(7, 52)
        Me.lblZerodhaPassword.Name = "lblZerodhaPassword"
        Me.lblZerodhaPassword.Size = New System.Drawing.Size(53, 13)
        Me.lblZerodhaPassword.TabIndex = 1
        Me.lblZerodhaPassword.Text = "Password"
        '
        'lblZerodhaUserId
        '
        Me.lblZerodhaUserId.AutoSize = True
        Me.lblZerodhaUserId.Location = New System.Drawing.Point(7, 20)
        Me.lblZerodhaUserId.Name = "lblZerodhaUserId"
        Me.lblZerodhaUserId.Size = New System.Drawing.Size(38, 13)
        Me.lblZerodhaUserId.TabIndex = 0
        Me.lblZerodhaUserId.Text = "UserId"
        '
        'btnSaveZerodhaUserDetails
        '
        Me.btnSaveZerodhaUserDetails.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnSaveZerodhaUserDetails.ImageKey = "save-icon-36533.png"
        Me.btnSaveZerodhaUserDetails.ImageList = Me.ImageList1
        Me.btnSaveZerodhaUserDetails.Location = New System.Drawing.Point(360, 13)
        Me.btnSaveZerodhaUserDetails.Name = "btnSaveZerodhaUserDetails"
        Me.btnSaveZerodhaUserDetails.Size = New System.Drawing.Size(84, 47)
        Me.btnSaveZerodhaUserDetails.TabIndex = 1
        Me.btnSaveZerodhaUserDetails.Text = "&Save"
        Me.btnSaveZerodhaUserDetails.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.btnSaveZerodhaUserDetails.UseVisualStyleBackColor = True
        '
        'ImageList1
        '
        Me.ImageList1.ImageStream = CType(resources.GetObject("ImageList1.ImageStream"), System.Windows.Forms.ImageListStreamer)
        Me.ImageList1.TransparentColor = System.Drawing.Color.Transparent
        Me.ImageList1.Images.SetKeyName(0, "save-icon-36533.png")
        '
        'frmZerodhaUserDetails
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(452, 164)
        Me.Controls.Add(Me.btnSaveZerodhaUserDetails)
        Me.Controls.Add(Me.GroupBox1)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmZerodhaUserDetails"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Zerodha User Details"
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents GroupBox1 As GroupBox
    Friend WithEvents lblZerodhaUserId As Label
    Friend WithEvents lblZerodhaAPIKey As Label
    Friend WithEvents lblZerodhaPassword As Label
    Friend WithEvents txtZerodhaAPISecret As TextBox
    Friend WithEvents txtZerodhaAPIKey As TextBox
    Friend WithEvents txtZerodhaPassword As TextBox
    Friend WithEvents txtZerodhaUserId As TextBox
    Friend WithEvents lblZerodhaAPISecret As Label
    Friend WithEvents btnSaveZerodhaUserDetails As Button
    Friend WithEvents ImageList1 As ImageList
End Class
