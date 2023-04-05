Friend Class ItemTypeDescriptor
    ReadOnly Property SpawnCount As Integer
    Sub New(
           name As String,
           Optional spawnCount As Integer = 0,
           Optional minimumExitCount As Integer = 0,
           Optional maximumExitCount As Integer = 4)
        Me.Name = name
        Me.SpawnCount = spawnCount
        Me.MinimumExitCount = minimumExitCount
        Me.MaximumExitCount = maximumExitCount
    End Sub
    ReadOnly Property MinimumExitCount As Integer
    ReadOnly Property MaximumExitCount As Integer
    ReadOnly Property Name As String
End Class
