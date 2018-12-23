Namespace Entities
    Public Class ZerodhaConnection
        Implements IConnection

        Public Property ZerodhaRequestToken As String Implements IConnection.RequestToken
        Public Property ZerodhaAccessToken As String Implements IConnection.AccessToken
        Public Property ZerodhaPublicToken As String Implements IConnection.PublicToken
        Public Property ZerodhaUser As IUser Implements IConnection.APIUser
    End Class
End Namespace