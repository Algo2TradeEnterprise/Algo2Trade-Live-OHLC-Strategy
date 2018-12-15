Namespace Collections
    Public Class LimitedStack(Of T)
        Public ReadOnly Limit As Integer
        Private ReadOnly _stack As List(Of T)

        Public Sub New(ByVal Optional limit As Integer = 32)
            limit = limit
            _stack = New List(Of T)(limit)
        End Sub

        Public Sub Push(ByVal item As T)
            If _stack.Count = Limit Then _stack.RemoveAt(0)
            _stack.Add(item)
        End Sub

        Public Function Peek() As T
            If _stack.Count > 0 Then
                Return _stack(_stack.Count - 1)
            Else
                Return Nothing
            End If
        End Function

        Public Sub Pop()
            If _stack.Count > 0 Then
                _stack.RemoveAt(_stack.Count - 1)
            Else
                Throw New ApplicationException("Cannot peek as no items are present")
            End If
        End Sub

        Public ReadOnly Property Count As Integer
            Get
                Return _stack.Count
            End Get
        End Property
    End Class
End Namespace