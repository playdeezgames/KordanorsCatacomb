Friend Class Character
    Implements ICharacter

    Private ReadOnly _data As WorldData
    Private ReadOnly _characterId As Integer
    Private ReadOnly Property CharacterData As CharacterData
        Get
            Return _data.Characters(_characterId)
        End Get
    End Property

    Public Sub New(data As WorldData, characterId As Integer)
        Me._data = data
        Me._characterId = characterId
    End Sub

    Public ReadOnly Property Id As Integer Implements ICharacter.Id
        Get
            Return _characterId
        End Get
    End Property

    Public Property Location As ILocation Implements ICharacter.Location
        Get
            If CharacterData.Location.HasValue Then
                Return New Location(_data, CharacterData.Location.Value)
            End If
            Return Nothing
        End Get
        Set(value As ILocation)
            If value Is Nothing Then
                CharacterData.Location = Nothing
                Return
            End If
            CharacterData.Location = value.Id
        End Set
    End Property
End Class
