Namespace Entities
    Public Interface IUser
        Property UserId As String
        Property Password As String
        Property APISecret As String
        Property APIKey As String
        Property APIVersion As String
        Property API2FA As Dictionary(Of String, String)
    End Interface
End Namespace
