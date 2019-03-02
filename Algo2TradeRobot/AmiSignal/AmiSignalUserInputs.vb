Imports System.IO
Imports System.Threading
Imports Algo2TradeCore.UserSettings
Imports Utilities.DAL

<Serializable>
Public Class AmiSignalUserInputs
    Inherits StrategyUserInputs
    Public MaxStoplossPercentage As Decimal
    Public MaxCapitalPerTrade As Decimal
    Public DefaultBufferPercentage As Decimal
    Public NumberOfTrade As Integer
    Public InstrumentDetailsFilePath As String
    Public InstrumentsData As Dictionary(Of String, InstrumentDetails)
    <Serializable>
    Public Class InstrumentDetails
        Public AmiSymbol As String
        Public InstrumentName As String
        Public MarketType As InstrumentType
    End Class
    <Serializable>
    Public Enum InstrumentType
        Cash = 1
        Futures
    End Enum
    Public Sub FillSettingsDetails(ByVal filePath As String, ByVal canceller As CancellationTokenSource)
        If filePath IsNot Nothing Then
            If File.Exists(filePath) Then
                Dim extension As String = Path.GetExtension(filePath)
                If extension = ".csv" Then
                    Dim instrumentDetails(,) As Object = Nothing
                    Using csvReader As New CSVHelper(filePath, ",", canceller)
                        instrumentDetails = csvReader.Get2DArrayFromCSV(0)
                    End Using
                    If instrumentDetails IsNot Nothing AndAlso instrumentDetails.Length > 0 Then
                        'Dim excelColumnList As New List(Of String) From {"INSTRUMENT NAME", "CASH", "FUTURES", "QUANTITY", "CAPITAL"}

                        'For colCtr = 0 To 4
                        '    If instrumentDetails(0, colCtr) Is Nothing OrElse Trim(instrumentDetails(0, colCtr).ToString) = "" Then
                        '        Throw New ApplicationException(String.Format("Invalid format."))
                        '    Else
                        '        If Not excelColumnList.Contains(Trim(instrumentDetails(0, colCtr).ToString.ToUpper)) Then
                        '            Throw New ApplicationException(String.Format("Invalid format or invalid column at ColumnNumber: {0}", colCtr))
                        '        End If
                        '    End If
                        'Next
                        For rowCtr = 1 To instrumentDetails.GetLength(0) - 1
                            canceller.Token.ThrowIfCancellationRequested()
                            Dim amiSymbol As String = Nothing
                            Dim instrumentName As String = Nothing
                            Dim marketCash As Boolean = False
                            Dim marketFuture As Boolean = False
                            For columnCtr = 0 To instrumentDetails.GetLength(1)
                                canceller.Token.ThrowIfCancellationRequested()
                                If columnCtr = 0 Then
                                    If instrumentDetails(rowCtr, columnCtr) IsNot Nothing AndAlso
                                        Not Trim(instrumentDetails(rowCtr, columnCtr).ToString) = "" Then
                                        amiSymbol = instrumentDetails(rowCtr, columnCtr)
                                    Else
                                        If Not rowCtr = instrumentDetails.GetLength(0) Then
                                            Throw New ApplicationException(String.Format("Ami Symbol Missing or Blank Row. RowNumber: {0}", rowCtr))
                                        End If
                                    End If
                                ElseIf columnCtr = 1 Then
                                    If instrumentDetails(rowCtr, columnCtr) IsNot Nothing AndAlso
                                        Not Trim(instrumentDetails(rowCtr, columnCtr).ToString) = "" Then
                                        instrumentName = instrumentDetails(rowCtr, columnCtr)
                                    Else
                                        Throw New ApplicationException(String.Format("Zerodha Symbol Missing. RowNumber: {0}", rowCtr))
                                    End If
                                ElseIf columnCtr = 2 Then
                                    If instrumentDetails(rowCtr, columnCtr) IsNot Nothing AndAlso
                                        instrumentDetails(rowCtr, columnCtr).ToString.Trim.ToUpper = "FUTURES" Then
                                        marketFuture = True
                                    ElseIf instrumentDetails(rowCtr, columnCtr) IsNot Nothing AndAlso
                                        instrumentDetails(rowCtr, columnCtr).ToString.Trim.ToUpper = "CASH" Then
                                        marketCash = True
                                    ElseIf instrumentDetails(rowCtr, columnCtr) IsNot Nothing AndAlso
                                        Not Trim(instrumentDetails(rowCtr, columnCtr).ToString) = "" Then
                                        Throw New ApplicationException(String.Format("Instrument Type is not valid for {0}", instrumentName))
                                    End If
                                ElseIf columnCtr = 9 Then
                                    If instrumentDetails(rowCtr, columnCtr) IsNot Nothing AndAlso
                                    Not Trim(instrumentDetails(rowCtr, columnCtr).ToString) = "" Then
                                        If IsNumeric(instrumentDetails(rowCtr, columnCtr).ToString.Substring(0, instrumentDetails(rowCtr, columnCtr).ToString.Count - 1)) Then
                                            Me.MaxStoplossPercentage = instrumentDetails(rowCtr, columnCtr).ToString.Substring(0, instrumentDetails(rowCtr, columnCtr).ToString.Count - 1)
                                        Else
                                            Throw New ApplicationException(String.Format("Default SL cannot be of type {0} for {1}", instrumentDetails(rowCtr, columnCtr).GetType, instrumentName))
                                        End If
                                    End If
                                ElseIf columnCtr = 10 Then
                                    If instrumentDetails(rowCtr, columnCtr) IsNot Nothing AndAlso
                                    Not Trim(instrumentDetails(rowCtr, columnCtr).ToString) = "" Then
                                        If IsNumeric(instrumentDetails(rowCtr, columnCtr).ToString.Substring(0, instrumentDetails(rowCtr, columnCtr).ToString.Count - 1)) Then
                                            Me.DefaultBufferPercentage = instrumentDetails(rowCtr, columnCtr).ToString.Substring(0, instrumentDetails(rowCtr, columnCtr).ToString.Count - 1)
                                        Else
                                            Throw New ApplicationException(String.Format("Buffer% cannot be of type {0} for {1}", instrumentDetails(rowCtr, columnCtr).GetType, instrumentName))
                                        End If
                                    End If
                                ElseIf columnCtr = 11 Then
                                    If instrumentDetails(rowCtr, columnCtr) IsNot Nothing AndAlso
                                    Not Trim(instrumentDetails(rowCtr, columnCtr).ToString) = "" Then
                                        If IsNumeric(instrumentDetails(rowCtr, columnCtr)) Then
                                            Me.NumberOfTrade = instrumentDetails(rowCtr, columnCtr)
                                        Else
                                            Throw New ApplicationException(String.Format("Number of trade cannot be of type {0} for {1}", instrumentDetails(rowCtr, columnCtr).GetType, instrumentName))
                                        End If
                                    End If
                                ElseIf columnCtr = 12 Then
                                    If instrumentDetails(rowCtr, columnCtr) IsNot Nothing AndAlso
                                    Not Trim(instrumentDetails(rowCtr, columnCtr).ToString) = "" Then
                                        If IsNumeric(instrumentDetails(rowCtr, columnCtr)) Then
                                            Me.MaxCapitalPerTrade = instrumentDetails(rowCtr, columnCtr)
                                        Else
                                            Throw New ApplicationException(String.Format("Max capital per trade cannot be of type {0} for {1}", instrumentDetails(rowCtr, columnCtr).GetType, instrumentName))
                                        End If
                                    End If
                                End If
                            Next
                            If instrumentName IsNot Nothing Then
                                Dim instrumentData As New AmiSignalUserInputs.InstrumentDetails
                                instrumentData.AmiSymbol = amiSymbol.ToUpper
                                instrumentData.InstrumentName = instrumentName.ToUpper
                                If marketCash Then
                                    instrumentData.MarketType = AmiSignalUserInputs.InstrumentType.Cash
                                ElseIf marketFuture Then
                                    instrumentData.MarketType = AmiSignalUserInputs.InstrumentType.Futures
                                End If
                                If Me.InstrumentsData Is Nothing Then Me.InstrumentsData = New Dictionary(Of String, AmiSignalUserInputs.InstrumentDetails)
                                If Me.InstrumentsData.ContainsKey(instrumentData.InstrumentName) Then
                                    Throw New ApplicationException(String.Format("Duplicate Instrument Name {0}", instrumentData.InstrumentName))
                                End If
                                Me.InstrumentsData.Add(instrumentData.AmiSymbol, instrumentData)
                            End If
                        Next
                    Else
                        Throw New ApplicationException("No valid input in the file")
                    End If
                Else
                    Throw New ApplicationException("File Type not supported. Application only support .csv file.")
                End If
            Else
                Throw New ApplicationException("File does not exists. Please select valid file")
            End If
        Else
            Throw New ApplicationException("No valid file path exists")
        End If
    End Sub
End Class
