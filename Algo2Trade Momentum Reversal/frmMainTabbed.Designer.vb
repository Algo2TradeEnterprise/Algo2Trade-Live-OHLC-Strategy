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
        Me.msMainMenuStrip = New System.Windows.Forms.MenuStrip()
        Me.miSecurity = New System.Windows.Forms.ToolStripMenuItem()
        Me.miUserDetails = New System.Windows.Forms.ToolStripMenuItem()
        Me.miAbout = New System.Windows.Forms.ToolStripMenuItem()
        Me.msMainMenuStrip.SuspendLayout()
        Me.SuspendLayout()
        '
        'msMainMenuStrip
        '
        Me.msMainMenuStrip.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.miSecurity, Me.miAbout})
        Me.msMainMenuStrip.Location = New System.Drawing.Point(0, 0)
        Me.msMainMenuStrip.Name = "msMainMenuStrip"
        Me.msMainMenuStrip.Size = New System.Drawing.Size(1032, 24)
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
        Me.miUserDetails.Size = New System.Drawing.Size(152, 22)
        Me.miUserDetails.Text = "User Details"
        '
        'miAbout
        '
        Me.miAbout.Name = "miAbout"
        Me.miAbout.Size = New System.Drawing.Size(52, 20)
        Me.miAbout.Text = "About"
        '
        'frmMainTabbed
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1032, 499)
        Me.Controls.Add(Me.msMainMenuStrip)
        Me.MainMenuStrip = Me.msMainMenuStrip
        Me.Name = "frmMainTabbed"
        Me.Text = "frmMainTabbed"
        Me.msMainMenuStrip.ResumeLayout(False)
        Me.msMainMenuStrip.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents msMainMenuStrip As MenuStrip
    Friend WithEvents miSecurity As ToolStripMenuItem
    Friend WithEvents miUserDetails As ToolStripMenuItem
    Friend WithEvents miAbout As ToolStripMenuItem
End Class
