Friend Class ItemTypeDescriptor
    ReadOnly Property SpawnCount As Integer
    Sub New(
           Optional spawnCount As Integer = 0,
           Optional minimumExitCount As Integer = 0,
           Optional maximumExitCount As Integer = 4)
        Me.SpawnCount = spawnCount
        Me.MinimumExitCount = minimumExitCount
        Me.MaximumExitCount = maximumExitCount
    End Sub
    ReadOnly Property MinimumExitCount As Integer
    ReadOnly Property MaximumExitCount As Integer
End Class
