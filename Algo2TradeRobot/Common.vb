Imports Algo2TradeCore.Entities
Imports Utilities.Strings.StringManipulation

Module Common
    Public Const MASTER_KEY = "JOYMA"
    Public Const LOGIN_PENDING = "Login pending"
    Public Function IsZerodhaUserDetailsPopulated() As Boolean
        Return My.Settings.ZerodhaUserId IsNot Nothing AndAlso My.Settings.ZerodhaUserId.Trim.Count > 0 AndAlso
        My.Settings.ZerodhaPassword IsNot Nothing AndAlso My.Settings.ZerodhaPassword.Trim.Count > 0 AndAlso
            My.Settings.ZerodhaAPIKey IsNot Nothing AndAlso My.Settings.ZerodhaAPIKey.Trim.Count > 0 AndAlso
            My.Settings.ZerodhaAPISecret IsNot Nothing AndAlso My.Settings.ZerodhaAPISecret.Trim.Count > 0
    End Function
    Public Function GetZerodhaCredentialsFromSettings() As ZerodhaUser
        Return New ZerodhaUser With {.UserId = My.Settings.ZerodhaUserId,
                .Password = Decrypt(My.Settings.ZerodhaPassword, MASTER_KEY),
                .APIVersion = "3",
                .APIKey = Decrypt(My.Settings.ZerodhaAPIKey, MASTER_KEY),
                .APISecret = Decrypt(My.Settings.ZerodhaAPISecret, MASTER_KEY),
                .API2FAPin = Decrypt(My.Settings.Zerodha2FAPin, MASTER_KEY)}
    End Function
End Module
