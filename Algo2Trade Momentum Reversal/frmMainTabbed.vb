Public Class frmMainTabbed
    Private Sub miUserDetails_Click(sender As Object, e As EventArgs) Handles miUserDetails.Click
        Dim newForm As New frmZerodhaUserDetails
        newForm.ShowDialog()
    End Sub
End Class