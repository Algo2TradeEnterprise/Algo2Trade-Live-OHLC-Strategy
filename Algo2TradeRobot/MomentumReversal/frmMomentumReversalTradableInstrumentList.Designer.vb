<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmMomentumReversalTradableInstrumentList
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmMomentumReversalTradableInstrumentList))
        Me.lstMomentumReversalTradableInstruments = New System.Windows.Forms.ListBox()
        Me.SuspendLayout()
        '
        'lstMomentumReversalTradableInstruments
        '
        Me.lstMomentumReversalTradableInstruments.FormattingEnabled = True
        Me.lstMomentumReversalTradableInstruments.ItemHeight = 16
        Me.lstMomentumReversalTradableInstruments.Location = New System.Drawing.Point(12, 12)
        Me.lstMomentumReversalTradableInstruments.Name = "lstMomentumReversalTradableInstruments"
        Me.lstMomentumReversalTradableInstruments.Size = New System.Drawing.Size(224, 420)
        Me.lstMomentumReversalTradableInstruments.TabIndex = 0
        '
        'frmMomentumReversalTradableInstrumentList
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 16.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(249, 439)
        Me.Controls.Add(Me.lstMomentumReversalTradableInstruments)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmMomentumReversalTradableInstrumentList"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Tradable Instruments"
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents lstMomentumReversalTradableInstruments As ListBox
End Class
