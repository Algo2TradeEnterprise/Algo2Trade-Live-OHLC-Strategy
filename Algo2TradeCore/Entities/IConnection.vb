Namespace Entities
    Public Interface IConnection
        Property RequestToken As String
        Property AccessToken As String
        Property PublicToken As String

        Property APIUser As IUser
    End Interface
End Namespace
