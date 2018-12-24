Imports NLog
Namespace Time
    Public Module TimeManipulation
#Region "Logging and Status Progress"
        Public logger As Logger = LogManager.GetCurrentClassLogger
#End Region

#Region "Private Attributes"
        Private INDIAN_ZONE As TimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time")
#End Region

#Region "Public Methods"
        Public Function ConcatenateDateTime(ByVal dateToConcatenate As Date, ByVal timeToConcatenate As TimeSpan) As DateTime
            logger.Debug("Concatenating date time")
            Return New Date(dateToConcatenate.Date.Year, dateToConcatenate.Date.Month, dateToConcatenate.Date.Day,
                            timeToConcatenate.Hours, timeToConcatenate.Minutes, timeToConcatenate.Seconds)
        End Function

        Public Function IsDateTimeEqualTillMinutes(ByVal datetime1 As Date, ByVal datetime2 As Date) As Boolean
            logger.Debug("Checking if date time is equal till minutes")
            Return datetime1.Date = datetime2.Date And
                    datetime1.Hour = datetime2.Hour And
                    datetime1.Minute = datetime2.Minute
        End Function
        Public Function GetCurrentISTTime() As DateTime
            logger.Debug("Getting current IST time as datetime")
            Return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE)
        End Function
        Public Function UnixToDateTime(ByVal unixTimeStamp As UInt64) As DateTime
            logger.Debug("Converting Unix time to normal datetime")
            Dim dateTime As DateTime = New DateTime(1970, 1, 1, 5, 30, 0, 0, DateTimeKind.Unspecified)
            dateTime = dateTime.AddSeconds(unixTimeStamp)
            Return dateTime
        End Function
        Public Function IsTimeEqualTillSeconds(ByVal timespan1 As TimeSpan, ByVal timespan2 As TimeSpan) As Boolean
            logger.Debug("Checking if time is equal till seconds")
            Return Math.Floor(timespan1.TotalSeconds) = Math.Floor(timespan2.TotalSeconds)
        End Function
        Public Function IsTimeEqualTillSeconds(ByVal datetime1 As DateTime, ByVal datetime2 As DateTime) As Boolean
            logger.Debug("Checking if time is equal till seconds")
            Return Math.Floor(datetime1.TimeOfDay.TotalSeconds) = Math.Floor(datetime2.TimeOfDay.TotalSeconds)
        End Function
        Public Function IsTimeEqualTillSeconds(ByVal datetime1 As DateTime, ByVal timespan2 As TimeSpan) As Boolean
            logger.Debug("Checking if time is equal till seconds")
            Return Math.Floor(datetime1.TimeOfDay.TotalSeconds) = Math.Floor(timespan2.TotalSeconds)
        End Function
        Public Function IsTimeEqualTillSeconds(ByVal timespan1 As TimeSpan, ByVal datetime2 As DateTime) As Boolean
            logger.Debug("Checking if time is equal till seconds")
            Return Math.Floor(timespan1.TotalSeconds) = Math.Floor(datetime2.TimeOfDay.TotalSeconds)
        End Function
#End Region
    End Module
End Namespace