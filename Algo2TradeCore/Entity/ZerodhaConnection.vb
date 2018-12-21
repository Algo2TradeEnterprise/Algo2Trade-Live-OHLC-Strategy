Namespace Entity
    Public Class ZerodhaConnection
        Implements IConnection
        Public Property ZerodhaRequestToken As String
        Public Property ZerodhaAccessToken As String
        Public Property ZerodhaPublicToken As String
        Public Property ZerodhaUser As IUser Implements IConnection.APIUser
    End Class
End Namespace