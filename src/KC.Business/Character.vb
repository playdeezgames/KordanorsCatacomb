Friend Class Character
    Implements ICharacter

    Private ReadOnly _data As WorldData
    Private ReadOnly _characterId As Integer

    Public Sub New(data As WorldData, characterId As Integer)
        Me._data = data
        Me._characterId = characterId
    End Sub

    Public ReadOnly Property Id As Integer Implements ICharacter.Id
        Get
            Return _characterId
        End Get
    End Property
End Class
