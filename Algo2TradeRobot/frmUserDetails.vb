Imports Utilities.Strings.StringManipulation
Public Class frmZerodhaUserDetails
    Private Sub btnSaveZerodhaUserDetails_Click(sender As Object, e As EventArgs) Handles btnSaveZerodhaUserDetails.Click
        Try
            ValidateAll()
            SaveUserDetails()
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical)
        End Try
    End Sub

    Private Sub frmZerodhaUserDetails_Load(sender As Object, e As EventArgs) Handles Me.Load
        txtZerodhaUserId.Text = My.Settings.ZerodhaUserId
        txtZerodhaPassword.Text = Decrypt(My.Settings.ZerodhaPassword, Common.MASTER_KEY)
        txtZerodhaAPIKey.Text = Decrypt(My.Settings.ZerodhaAPIKey, Common.MASTER_KEY)
        txtZerodhaAPISecret.Text = Decrypt(My.Settings.ZerodhaAPISecret, Common.MASTER_KEY)
    End Sub

    Private Sub SaveUserDetails()
        My.Settings.ZerodhaUserId = txtZerodhaUserId.Text
        My.Settings.ZerodhaPassword = Encrypt(txtZerodhaPassword.Text, Common.MASTER_KEY)
        My.Settings.ZerodhaAPIKey = Encrypt(txtZerodhaAPIKey.Text, Common.MASTER_KEY)
        My.Settings.ZerodhaAPISecret = Encrypt(txtZerodhaAPISecret.Text, Common.MASTER_KEY)
        My.Settings.Save()
        Me.Close()
    End Sub
    Private Sub ValidateAll()
        ValidateTextLength(txtZerodhaUserId, 1, "User Id")
        ValidateTextLength(txtZerodhaPassword, 1, "Password")
        ValidateTextLength(txtZerodhaAPIKey, 1, "API Key")
        ValidateTextLength(txtZerodhaAPISecret, 1, "API Secret")
    End Sub
    Private Sub ValidateTextLength(ByVal txtControl As TextBox, ByVal minLength As Integer, ByVal friendlyNameOfContents As String)
        If txtControl.Text Is Nothing OrElse txtControl.Text.Trim.Count = 0 Then
            Throw New ApplicationException(String.Format("{0} cannot be blank", friendlyNameOfContents))
        ElseIf txtControl.Text IsNot Nothing AndAlso txtControl.Text.Trim.Count < minLength Then
            Throw New ApplicationException(String.Format("{0} cannpot have less than {1} characters", friendlyNameOfContents, minLength))
        End If
    End Sub
End Class