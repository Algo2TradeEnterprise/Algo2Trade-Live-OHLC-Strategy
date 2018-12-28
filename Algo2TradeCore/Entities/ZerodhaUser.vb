Imports KiteConnect
Namespace Entities
    Public Class ZerodhaUser
        Implements IUser

        'The below properties are input properties and hence not readyonly unlike other entities
        Public Property UserId As String Implements IUser.UserId
        Public Property Password As String Implements IUser.Password
        Public Property APISecret As String Implements IUser.APISecret
        Public Property APIKey As String Implements IUser.APIKey
        Public Property APIVersion As String Implements IUser.APIVersion
        Public Property API2FA As Dictionary(Of String, String) Implements IUser.API2FA

        Public Property WrappedUser As User
        Public ReadOnly Property Broker As APISource Implements IUser.Broker
            Get
                Return APISource.Zerodha
            End Get
        End Property
    End Class
End Namespace
