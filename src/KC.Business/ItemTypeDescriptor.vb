Friend Class ItemTypeDescriptor
    ReadOnly Property SpawnCount As Integer
    Sub New(
           name As String,
           onUse As Action(Of WorldData, ICharacter),
           Optional spawnCount As Integer = 0,
           Optional minimumExitCount As Integer = 0,
           Optional maximumExitCount As Integer = 4,
           Optional isUsable As Boolean = False)
        Me.Name = name
        Me.SpawnCount = spawnCount
        Me.MinimumExitCount = minimumExitCount
        Me.MaximumExitCount = maximumExitCount
        Me.IsUsable = isUsable
        Me.OnUse = onUse
    End Sub

    ReadOnly Property OnUse As Action(Of WorldData, ICharacter)
    ReadOnly Property IsUsable As Boolean
    ReadOnly Property MinimumExitCount As Integer
    ReadOnly Property MaximumExitCount As Integer
    ReadOnly Property Name As String
End Class
