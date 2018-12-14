Namespace Entity
    Public Class ZerodhaConnection
        Implements IConnection
        Public Property UserId As String Implements IConnection.UserId
        Public Property Password As String Implements IConnection.Password
        Public Property APIKey As String
        Public Property APISecret As String
        Public Property APIVersion As String
        Public Property API2FA As Dictionary(Of String, String)
        Public Property APIRequestToken As String
        Public Property APIUser As ZerodhaUser
    End Class
End Namespace