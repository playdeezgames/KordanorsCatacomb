Friend Class ItemTypeDescriptor
    ReadOnly Property SpawnCount As Integer
    Sub New(
           name As String,
           onUse As Func(Of WorldData, ICharacter, Boolean),
           Optional spawnCount As Integer = 0,
           Optional minimumExitCount As Integer = 0,
           Optional maximumExitCount As Integer = 4,
           Optional isUsable As Boolean = False,
           Optional canTake As Boolean = True)
        Me.Name = name
        Me.SpawnCount = spawnCount
        Me.MinimumExitCount = minimumExitCount
        Me.MaximumExitCount = maximumExitCount
        Me.IsUsable = isUsable
        Me.OnUse = onUse
        Me.CanTake = canTake
    End Sub

    ReadOnly Property OnUse As Func(Of WorldData, ICharacter, Boolean)
    ReadOnly Property IsUsable As Boolean
    ReadOnly Property MinimumExitCount As Integer
    ReadOnly Property MaximumExitCount As Integer
    ReadOnly Property Name As String
    ReadOnly Property CanTake As Boolean
End Class
