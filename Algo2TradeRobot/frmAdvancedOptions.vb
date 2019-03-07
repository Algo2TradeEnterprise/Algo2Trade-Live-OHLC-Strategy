Public Class frmAdvancedOptions
    Private Sub frmAdvancedOptions_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        LoadSettings()
    End Sub

    Private Sub btnSaveDelaySettings_Click(sender As Object, e As EventArgs) Handles btnSaveDelaySettings.Click
        Try
            ValidateInputs()
            SaveSettings()
            Me.Close()
        Catch ex As Exception
            MsgBox(String.Format("The following error occurred: {0}", ex.Message), MsgBoxStyle.Critical)
        End Try
    End Sub

    Private Sub LoadSettings()
        txtGetInformationDelay.Text = My.Settings.GetInformationDelay
        txtBackToBackOrderCoolOffDelay.Text = My.Settings.BackToBackOrderCoolOffTime
    End Sub

    Private Sub SaveSettings()
        My.Settings.GetInformationDelay = txtGetInformationDelay.Text
        My.Settings.BackToBackOrderCoolOffTime = txtBackToBackOrderCoolOffDelay.Text
    End Sub

    Private Sub ValidateInputs()
        ValidateNumbers(1, 1000, txtGetInformationDelay)
        ValidateNumbers(1, 1000, txtBackToBackOrderCoolOffDelay)
    End Sub

    Private Function ValidateNumbers(ByVal startNumber As Integer, ByVal endNumber As Integer, ByVal inputTB As TextBox) As Boolean
        Dim ret As Boolean = False
        If IsNumeric(inputTB.Text) Then
            If Val(inputTB.Text) >= startNumber And Val(inputTB.Text) <= endNumber Then
                ret = True
            End If
        End If
        If Not ret Then Throw New ApplicationException(String.Format("{0} cannot have a value < {1} or > {2}", inputTB.Tag, startNumber, endNumber))
        Return ret
    End Function
End Class