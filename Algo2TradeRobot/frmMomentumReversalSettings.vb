Imports Utilities.DAL
Imports System.IO
Imports System.Runtime.Serialization.Formatters.Binary
Imports System.Threading

Public Class frmMomentumReversalSettings

    Private _cts As CancellationTokenSource = Nothing
    Private _MRSettings As MomentumReversalUserInputs = Nothing
    Private _MRSettingsFilename As String = Path.Combine(My.Application.Info.DirectoryPath, "MomentumReversalSettings.a2t")
    'Private _InstrumentsData As Dictionary(Of String, MomentumReversalUserInputs.InstrumentDetails) = Nothing

    Public Sub New(ByRef MRUserInputs As MomentumReversalUserInputs)
        ' This call is required by the designer.
        InitializeComponent()
        ' Add any initialization after the InitializeComponent() call.
        _MRSettings = MRUserInputs
    End Sub

    Private Sub frmMomentumReversalSettings_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        LoadSettings()
    End Sub
    Private Sub btnSaveMomentumReversalSettings_Click(sender As Object, e As EventArgs) Handles btnSaveMomentumReversalSettings.Click
        Try
            _cts = New CancellationTokenSource
            If _MRSettings Is Nothing Then _MRSettings = New MomentumReversalUserInputs
            _MRSettings.InstrumentsData = Nothing
            ValidateInputs()
            'SaveSettings()
            Me.Close()
        Catch ex As Exception
            MsgBox(String.Format("The following error occurred: {0}", ex.Message), MsgBoxStyle.Critical)
        End Try
    End Sub
    Private Sub LoadSettings()
        If File.Exists(_MRSettingsFilename) Then
            Dim fs As Stream = New FileStream(_MRSettingsFilename, FileMode.Open)
            Dim bf As BinaryFormatter = New BinaryFormatter()
            _MRSettings = CType(bf.Deserialize(fs), MomentumReversalUserInputs)
            fs.Close()
            txtCandleWickSizePercentage.Text = _MRSettings.CandleWickSizePercentage
            txtMaxSLPercentage.Text = _MRSettings.MaxStoplossPercentage
            txtTargetMultiplier.Text = _MRSettings.TargetMultiplier
            txtSignalTimeFrame.Text = _MRSettings.SignalTimeFrame
            txtInstrumentDetalis.Text = _MRSettings.InstrumentDetailsFilePath
        End If
    End Sub
    Private Sub SaveSettings()
        _MRSettings.CandleWickSizePercentage = txtCandleWickSizePercentage.Text
        _MRSettings.MaxStoplossPercentage = txtMaxSLPercentage.Text
        _MRSettings.TargetMultiplier = txtTargetMultiplier.Text
        _MRSettings.SignalTimeFrame = txtSignalTimeFrame.Text
        _MRSettings.InstrumentDetailsFilePath = txtInstrumentDetalis.Text

        Dim fs As Stream = New FileStream(_MRSettingsFilename, FileMode.OpenOrCreate)
        Dim bf As BinaryFormatter = New BinaryFormatter()
        bf.Serialize(fs, _MRSettings)
        fs.Close()
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
    Private Sub ValidateFile()
        _MRSettings.FillInstrumentDetails(txtInstrumentDetalis.Text, _cts)
    End Sub
    Private Sub ValidateInputs()
        'ValidateNumbers(0, 999, txtCandleWickSizePercentage)
        'ValidateNumbers(0, 999, txtMaxSLPercentage)
        'ValidateNumbers(0, 999, txtTargetMultiplier)
        'ValidateNumbers(1, 60, txtSignalTimeFrame)
        ValidateFile()
    End Sub

    Private Sub btnBrowse_Click(sender As Object, e As EventArgs) Handles btnBrowse.Click
        opnFileMRSettings.ShowDialog()
    End Sub

    Private Sub opnFileMRSettings_FileOk(sender As Object, e As System.ComponentModel.CancelEventArgs) Handles opnFileMRSettings.FileOk
        Dim extension As String = Path.GetExtension(opnFileMRSettings.FileName)
        If extension = ".xlsx" OrElse extension = ".xls" Then
            txtInstrumentDetalis.Text = opnFileMRSettings.FileName
        Else
            MsgBox("File Type not supported. Please Try again.", MsgBoxStyle.Critical)
        End If
    End Sub
End Class