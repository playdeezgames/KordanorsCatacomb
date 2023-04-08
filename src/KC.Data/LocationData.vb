Public Class LocationData
    Public Property Borders As New Dictionary(Of Direction, Integer)
    Public Property Neighbors As New Dictionary(Of Direction, Integer)
    Public Property Characters As New HashSet(Of Integer)
    Public Property Items As New HashSet(Of Integer)
    Public Property Visitors As New HashSet(Of Integer)
End Class
