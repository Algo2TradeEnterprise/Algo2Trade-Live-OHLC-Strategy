Imports Algo2TradeCore.Entities.UserSettings

Public Class frmAdvancedOptions

    Private _UserInputs As ControllerUserInputs

    Public Sub New(ByVal userInputs As ControllerUserInputs)
        InitializeComponent()
        Me._UserInputs = userInputs
    End Sub

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
        If _UserInputs IsNot Nothing Then
            txtGetInformationDelay.Text = _UserInputs.GetInformationDelay
            txtBackToBackOrderCoolOffDelay.Text = _UserInputs.BackToBackOrderCoolOffDelay
        End If
    End Sub

    Private Sub SaveSettings()
        If _UserInputs Is Nothing Then _UserInputs = New ControllerUserInputs
        _UserInputs.GetInformationDelay = txtGetInformationDelay.Text
        _UserInputs.BackToBackOrderCoolOffDelay = txtBackToBackOrderCoolOffDelay.Text
        Utilities.Strings.SerializeFromCollection(Of ControllerUserInputs)(ControllerUserInputs.Filename, _UserInputs)
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