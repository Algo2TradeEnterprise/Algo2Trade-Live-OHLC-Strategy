Imports System.IO
Imports Algo2TradeCore.Entities

Namespace Entities.UserSettings
    <Serializable>
    Public Class ControllerUserInputs
        Public Shared Property Filename As String = Path.Combine(My.Application.Info.DirectoryPath, "UserInputs.Controller.a2t")
        Public Property UserDetails As IUser
        Public Property GetInformationDelay As Integer
        Public Property BackToBackOrderCoolOffDelay As Integer
    End Class
End Namespace
