Imports System.IO
Imports System.Runtime.Serialization.Formatters.Binary

Public Class frmMomentumReversalSettings

    Private _MRSettings As MomentumReversalSettings = Nothing
    Private _MRSettingsFilename As String = Path.Combine(My.Application.Info.DirectoryPath, "MomentumReversalSettings.a2t")
    Private Sub frmMomentumReversalSettings_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        LoadSettings()
    End Sub
    Private Sub btnSaveMomentumReversalSettings_Click(sender As Object, e As EventArgs) Handles btnSaveMomentumReversalSettings.Click
        Try
            ValidateInputs()
            SaveSettings()
            Me.Close()
        Catch ex As Exception
            MsgBox(String.Format("The following error occurred: {0}", ex.Message), MsgBoxStyle.Critical)
        End Try
    End Sub
    Private Sub LoadSettings()
        If File.Exists(_MRSettingsFilename) Then
            Dim fs As Stream = New FileStream(_MRSettingsFilename, FileMode.Open)
            Dim bf As BinaryFormatter = New BinaryFormatter()
            _MRSettings = CType(bf.Deserialize(fs), MomentumReversalSettings)
            fs.Close()
            txtCandleWickSizePercentage.Text = _MRSettings.CandleWickSizePercentage
            txtMaxSLPercentage.Text = _MRSettings.MaxStoplossPercentage
            txtTargetMultiplier.Text = _MRSettings.TargetMultiplier
            txtSignalTimeFrame.Text = _MRSettings.SignalTimeFrame
        End If
    End Sub
    Private Sub SaveSettings()
        If _MRSettings Is Nothing Then _MRSettings = New MomentumReversalSettings
        _MRSettings.CandleWickSizePercentage = txtCandleWickSizePercentage.Text
        _MRSettings.MaxStoplossPercentage = txtMaxSLPercentage.Text
        _MRSettings.TargetMultiplier = txtTargetMultiplier.Text
        _MRSettings.SignalTimeFrame = txtSignalTimeFrame.Text

        Dim fs As Stream = New FileStream(_MRSettingsFilename, FileMode.OpenOrCreate)
        Dim bf As BinaryFormatter = New BinaryFormatter()
        bf.Serialize(fs, _MRSettings)
        fs.Close()
    End Sub

    <Serializable>
    Public Class MomentumReversalSettings
        Public CandleWickSizePercentage As Decimal
        Public MaxStoplossPercentage As Decimal
        Public TargetMultiplier As Decimal
        Public SignalTimeFrame As Integer
    End Class
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
    Private Sub ValidateInputs()
        ValidateNumbers(0, 999, txtCandleWickSizePercentage)
        ValidateNumbers(0, 999, txtMaxSLPercentage)
        ValidateNumbers(0, 999, txtTargetMultiplier)
        ValidateNumbers(1, 60, txtSignalTimeFrame)
    End Sub
End Class