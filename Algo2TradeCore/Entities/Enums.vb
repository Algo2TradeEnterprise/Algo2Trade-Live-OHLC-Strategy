Namespace Entities
    Public Module Enums
        Public Enum APISource
            Zerodha = 1
            Upstox
            None
        End Enum
        Enum TypeOfExchage
            NSE = 1
            MCX
            CDS
            None
        End Enum
        'Enum Indicator
        '    SMA = 1
        '    None
        'End Enum
        Enum TypeOfField
            Open
            Low
            High
            Close
            Volume
            SMA
            EMA
            ATR
            Supertrend
        End Enum
        Public Enum CrossDirection
            Above = 1
            Below
        End Enum
    End Module
End Namespace