Imports System.Runtime.CompilerServices

Namespace Collections
    Public Module CollectionUtil
        <Extension()>
        Public Function ConcatSingle(Of T)(ByVal e As IEnumerable(Of T), ByVal elem As T) As IEnumerable(Of T)
            Dim arr As T() = New T() {elem}
            Return e.Concat(arr)
        End Function

    End Module
End Namespace
