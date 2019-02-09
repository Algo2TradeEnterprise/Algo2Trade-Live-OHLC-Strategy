Imports System.IO
Imports System.Threading
Imports Utilities.DAL

<Serializable>
Public Class MomentumReversalUserInputs
    Public CandleWickSizePercentage As Decimal
    Public MaxStoplossPercentage As Decimal
    Public TargetMultiplier As Decimal
    Public SignalTimeFrame As Integer
    Public InstrumentDetailsFilePath As String
    Public InstrumentsData As Dictionary(Of String, InstrumentDetails)
    <Serializable>
    Public Class InstrumentDetails
        Public InstrumentName As String
        Public MarketType As InstrumentType
        Public Quantity As Integer
        Public Capital As Decimal
    End Class
    <Serializable>
    Public Enum InstrumentType
        Cash = 1
        Futures
        Both
    End Enum
    Public Sub FillInstrumentDetails(ByVal filePath As String, ByVal canceller As CancellationTokenSource)
        If filePath IsNot Nothing Then
            If File.Exists(filePath) Then
                Dim extension As String = Path.GetExtension(filePath)
                If extension = ".xlsx" OrElse extension = ".xls" Then
                    Dim instrumentDetails(,) As Object = Nothing
                    Using excelReader As New ExcelHelper(filePath, ExcelHelper.ExcelOpenStatus.OpenExistingForReadWrite, ExcelHelper.ExcelSaveType.XLS_XLSX, canceller)
                        instrumentDetails = excelReader.GetExcelInMemory()
                    End Using
                    If instrumentDetails IsNot Nothing AndAlso instrumentDetails.Length > 0 Then
                        Dim excelColumnList As New List(Of String) From {"INSTRUMENT NAME", "CASH", "FUTURES", "QUANTITY", "CAPITAL"}

                        For colCtr = 1 To 5
                            If instrumentDetails(1, colCtr) Is Nothing OrElse Trim(instrumentDetails(1, colCtr).ToString) = "" Then
                                Throw New ApplicationException(String.Format("Invalid format."))
                            Else
                                If Not excelColumnList.Contains(Trim(instrumentDetails(1, colCtr).ToString.ToUpper)) Then
                                    Throw New ApplicationException(String.Format("Invalid format or invalid column at ColumnNumber: {0}", colCtr))
                                End If
                            End If
                        Next
                        For rowCtr = 2 To instrumentDetails.GetLength(0)
                            Dim instrumentName As String = Nothing
                            Dim marketCash As Boolean = False
                            Dim marketFuture As Boolean = False
                            Dim quantity As Integer = Integer.MinValue
                            Dim capital As Decimal = Decimal.MinValue
                            For columnCtr = 1 To instrumentDetails.GetLength(1)
                                If columnCtr = 1 Then
                                    If instrumentDetails(rowCtr, columnCtr) IsNot Nothing AndAlso
                                        Not Trim(instrumentDetails(rowCtr, columnCtr).ToString) = "" Then
                                        instrumentName = instrumentDetails(rowCtr, columnCtr)
                                    Else
                                        If Not rowCtr = instrumentDetails.GetLength(0) Then
                                            Throw New ApplicationException(String.Format("Instrument Name Missing or Blank Row RowNumber: {0}", rowCtr))
                                        End If
                                    End If
                                ElseIf columnCtr = 2 Then
                                    If instrumentDetails(rowCtr, columnCtr) IsNot Nothing AndAlso
                                    instrumentDetails(rowCtr, columnCtr).ToString.ToUpper = "TRUE" Then
                                        marketCash = True
                                    ElseIf instrumentDetails(rowCtr, columnCtr) IsNot Nothing AndAlso
                                     Not instrumentDetails(rowCtr, columnCtr).ToString.ToUpper = "FALSE" Then
                                        Throw New ApplicationException(String.Format("Cash Instrument Type is not valid for {0}", instrumentName))
                                    End If
                                ElseIf columnCtr = 3 Then
                                    If instrumentDetails(rowCtr, columnCtr) IsNot Nothing AndAlso
                                    instrumentDetails(rowCtr, columnCtr).ToString.ToUpper = "TRUE" Then
                                        marketFuture = True
                                    ElseIf instrumentDetails(rowCtr, columnCtr) IsNot Nothing AndAlso
                                     Not instrumentDetails(rowCtr, columnCtr).ToString.ToUpper = "FALSE" Then
                                        Throw New ApplicationException(String.Format("Future Instrument Type is not valid for {0}", instrumentName))
                                    End If
                                ElseIf columnCtr = 4 Then
                                    If instrumentDetails(rowCtr, columnCtr) IsNot Nothing AndAlso
                                    Not Trim(instrumentDetails(rowCtr, columnCtr).ToString) = "" Then
                                        If IsNumeric(instrumentDetails(rowCtr, columnCtr)) AndAlso
                                        Math.Round(instrumentDetails(rowCtr, columnCtr), 0) = instrumentDetails(rowCtr, columnCtr) Then
                                            quantity = instrumentDetails(rowCtr, columnCtr)
                                        Else
                                            Throw New ApplicationException(String.Format("Quantity cannot be of type {0} for {1}", instrumentDetails(rowCtr, columnCtr).GetType, instrumentName))
                                        End If
                                    End If
                                ElseIf columnCtr = 5 Then
                                    If instrumentDetails(rowCtr, columnCtr) IsNot Nothing AndAlso
                                        Not Trim(instrumentDetails(rowCtr, columnCtr).ToString) = "" Then
                                        If IsNumeric(instrumentDetails(rowCtr, columnCtr)) Then
                                            capital = instrumentDetails(rowCtr, columnCtr)
                                        Else
                                            Throw New ApplicationException(String.Format("Capital cannot be of type {0} for {1}", instrumentDetails(rowCtr, columnCtr).GetType, instrumentName))
                                        End If
                                    End If
                                End If
                            Next
                            If instrumentName IsNot Nothing Then
                                Dim instrumentData As New MomentumReversalUserInputs.InstrumentDetails
                                instrumentData.InstrumentName = instrumentName.ToUpper
                                If marketFuture AndAlso marketCash Then
                                    instrumentData.MarketType = MomentumReversalUserInputs.InstrumentType.Both
                                ElseIf marketCash Then
                                    instrumentData.MarketType = MomentumReversalUserInputs.InstrumentType.Cash
                                ElseIf marketFuture Then
                                    instrumentData.MarketType = MomentumReversalUserInputs.InstrumentType.Futures
                                Else
                                    Throw New ApplicationException(String.Format("Intrument Type not mentioned for {0}", instrumentName))
                                End If
                                If quantity = Integer.MinValue AndAlso capital = Decimal.MinValue Then
                                    Throw New ApplicationException(String.Format("Quantity and Capital both cannot be blank for {0}", instrumentName))
                                End If
                                instrumentData.Quantity = quantity
                                instrumentData.Capital = capital
                                If Me.InstrumentsData Is Nothing Then Me.InstrumentsData = New Dictionary(Of String, MomentumReversalUserInputs.InstrumentDetails)
                                If Me.InstrumentsData.ContainsKey(instrumentData.InstrumentName) Then
                                    Throw New ApplicationException(String.Format("Duplicate Instrument Name {0}", instrumentData.InstrumentName))
                                End If
                                Me.InstrumentsData.Add(instrumentData.InstrumentName, instrumentData)
                            End If
                        Next
                    Else
                        Throw New ApplicationException("No valid input in the file")
                    End If
                Else
                    Throw New ApplicationException("File Type not supported. Application only support .xlsx or .xls file.")
                End If
            Else
                Throw New ApplicationException("File does not exists. Please select valid file")
            End If
        Else
            Throw New ApplicationException("No valid file path exists")
        End If
    End Sub
End Class
