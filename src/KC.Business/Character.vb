Imports System.Data

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

    Private ReadOnly Property Wounds As Integer
        Get
            Return GetStatistic(StatisticType.Wounds)
        End Get
    End Property

    Private Function GetStatistic(statisticType As StatisticType) As Integer
        Return CharacterData.Statistics(statisticType)
    End Function

    Friend Shared Function Create(data As WorldData, characterType As CharacterType, location As ILocation) As ICharacter
        Dim characterId = data.Characters.Count
        Dim characterData = New CharacterData With
                             {
                                .Location = location.Id,
                                .CharacterType = characterType
                             }
        For Each entry In characterType.ToDescriptor.Statistics
            characterData.Statistics(entry.Key) = entry.Value
        Next
        data.Characters.Add(characterData)
        Return New Character(data, characterId)
    End Function

    Public ReadOnly Property HP As Integer Implements ICharacter.HP
        Get
            Return Math.Clamp(MaximumHP - Wounds, 0, MaximumHP)
        End Get
    End Property

    Public ReadOnly Property MaximumHP As Integer Implements ICharacter.MaximumHP
        Get
            Return GetStatistic(StatisticType.MaximumHP)
        End Get
    End Property
End Class
