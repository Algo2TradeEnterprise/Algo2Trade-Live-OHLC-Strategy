Imports NLog
Imports System.Threading
Imports System.IO

Namespace DAL
    Public Class CSVHelper
        Implements IDisposable
#Region "Logging and Status Progress"
        Public Shared logger As Logger = LogManager.GetCurrentClassLogger
#End Region

#Region "Events"
        Public Event WaitingFor(ByVal elapsedSecs As Integer, ByVal totalSecs As Integer, ByVal msg As String)
        Public Event Heartbeat(ByVal msg As String)
        'The below functions are needed to allow the derived classes to raise the above two events
        Protected Overridable Sub OnWaitingFor(ByVal elapsedSecs As Integer, ByVal totalSecs As Integer, ByVal msg As String)
            RaiseEvent WaitingFor(elapsedSecs, totalSecs, msg)
        End Sub
        Protected Overridable Sub OnHeartbeat(ByVal msg As String)
            RaiseEvent Heartbeat(msg)
        End Sub
#End Region

#Region "Constructors"
        Public Sub New(ByVal CSVFilePath As String, ByVal separator As String, ByVal canceller As CancellationTokenSource)
            _CSVFilePath = CSVFilePath
            _separator = separator
            _canceller = canceller
        End Sub
#End Region

#Region "Private Attributes"
        Private _CSVFilePath As String
        Private _separator As String
        Protected _canceller As CancellationTokenSource
#End Region

#Region "Public Attributes"
#End Region

#Region "Private Methods"
#End Region

#Region "Public Methods"
        Public Function GetDataTableFromCSV(ByVal headerLineNumber As Integer) As DataTable
            logger.Debug("Getting data table from CSV")
            Dim ret As New DataTable
            Dim headerScanned As Boolean = False
            logger.Debug("Checking if file exists (File:{0})", _CSVFilePath)
            If IO.File.Exists(_CSVFilePath) Then
                Dim totalLinesCtr As Long = File.ReadLines(_CSVFilePath).Count()
                _canceller.Token.ThrowIfCancellationRequested()
                OnHeartbeat(String.Format("Opening file (File:{0})", _CSVFilePath))
                Using sr As New IO.StreamReader(_CSVFilePath)
                    Dim runningLineNumber As Integer = 0

                    While Not sr.EndOfStream
                        runningLineNumber += 1
                        _canceller.Token.ThrowIfCancellationRequested()
                        OnHeartbeat(String.Format("Reading file({0}/{1})",
                                                  runningLineNumber,
                                                  totalLinesCtr))
                        logger.Debug("Reading file({0}/{1})",
                                                  runningLineNumber,
                                                  totalLinesCtr)
                        Dim data() As String = sr.ReadLine.Split(_separator)
                        If Not headerScanned And ((headerLineNumber = 0 And runningLineNumber = 1) Or (headerLineNumber > 0 And runningLineNumber = headerLineNumber)) Then
                            headerScanned = True
                            Dim colCtr As Integer = 0
                            For Each col In data
                                _canceller.Token.ThrowIfCancellationRequested()
                                colCtr += 1
                                ret.Columns.Add(New DataColumn(String.Format("{0}_{1}", col, colCtr), GetType(String)))
                            Next
                        End If
                        If headerScanned Then
                            ret.Rows.Add(data.ToArray)
                        End If
                    End While
                End Using
            Else
                Throw New ApplicationException(String.Format("CSV file was not found (File:{0})", _CSVFilePath))
            End If
            Return ret
        End Function
#End Region

#Region "IDisposable Support"
        Private disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not disposedValue Then
                If disposing Then
                    ' TODO: dispose managed state (managed objects).
                    _CSVFilePath = Nothing
                    _separator = Nothing
                End If

                ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                ' TODO: set large fields to null.
            End If
            disposedValue = True
        End Sub

        ' TODO: override Finalize() only if Dispose(disposing As Boolean) above has code to free unmanaged resources.
        'Protected Overrides Sub Finalize()
        '    ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
        '    Dispose(False)
        '    MyBase.Finalize()
        'End Sub

        ' This code added by Visual Basic to correctly implement the disposable pattern.
        Public Sub Dispose() Implements IDisposable.Dispose
            ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
            Dispose(True)
            ' TODO: uncomment the following line if Finalize() is overridden above.
            ' GC.SuppressFinalize(Me)
        End Sub
#End Region
    End Class
End Namespace
