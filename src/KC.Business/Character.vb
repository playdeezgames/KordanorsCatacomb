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
            If CharacterData.Location.HasValue Then
                _data.Locations(CharacterData.Location.Value).Characters.Remove(Id)
            End If
            If value Is Nothing Then
                CharacterData.Location = Nothing
                Return
            End If
            CharacterData.Location = value.Id
            If CharacterData.Location.HasValue Then
                _data.Locations(CharacterData.Location.Value).Characters.Add(Id)
            End If
        End Set
    End Property

    Private Property Wounds As Integer
        Get
            Return GetStatistic(StatisticType.Wounds)
        End Get
        Set(value As Integer)
            SetStatistic(StatisticType.Wounds, value)
        End Set
    End Property

    Private Sub SetStatistic(statisticType As StatisticType, value As Integer)
        CharacterData.Statistics(statisticType) = value
    End Sub

    Private Function GetStatistic(statisticType As StatisticType) As Integer
        Return CharacterData.Statistics(statisticType)
    End Function

    Friend Shared Function Create(data As WorldData, characterType As CharacterType, location As ILocation) As ICharacter
        Dim characterId = data.Characters.Count
        Dim characterData = New CharacterData With
                             {
                                .CharacterType = characterType
                             }
        For Each entry In characterType.ToDescriptor.Statistics
            characterData.Statistics(entry.Key) = entry.Value
        Next
        data.Characters.Add(characterData)
        Return New Character(data, characterId) With {
            .Location = location
        }
    End Function

    Private Function DetermineAttackTargets() As IEnumerable(Of ICharacter)
        If CharacterType.ToDescriptor.IsEnemy Then
            Return Location.Allies
        Else
            Return Location.Enemies
        End If
    End Function

    Public Function RollDefend() As Integer Implements ICharacter.RollDefend
        Dim dice = GetStatistic(StatisticType.Defend)
        Dim maximum = GetStatistic(StatisticType.MaximumDefend)
        Dim roll = 0
        While dice > 0 AndAlso roll < maximum
            roll = RNG.RollDice("1d6/6")
            dice -= 1
        End While
        Return roll
    End Function

    Public Function RollAttack() As Integer Implements ICharacter.RollAttack
        Dim dice = GetStatistic(StatisticType.Attack)
        Dim maximum = GetStatistic(StatisticType.MaximumAttack)
        Dim roll = 0
        While dice > 0 AndAlso roll < maximum
            roll = RNG.RollDice("1d6/6")
            dice -= 1
        End While
        Return roll
    End Function

    Public Sub AddWounds(wounds As Integer) Implements ICharacter.AddWounds
        Me.Wounds += wounds
    End Sub

    Public Sub Fight(Optional index As Integer? = Nothing) Implements ICharacter.Fight
        If IsDead Then
            Return
        End If
        Dim targets = DetermineAttackTargets()
        If Not targets.Any Then
            Return
        End If
        Dim target = RNG.FromEnumerable(targets)
        If target.IsDead Then
            Return
        End If
        Dim msg As IMessage = Message.Create(_data)
        If index IsNot Nothing Then
            msg.AddLine(Mood.Gray, $"Counterattack #{index.Value}")
        End If
        msg.AddLine(Mood.Gray, $"{Name} attacks {target.Name}!")
        Dim attackRoll = RollAttack()
        msg.AddLine(Mood.Gray, $"{Name} rolls {attackRoll}")
        Dim defendRoll = target.RollDefend()
        msg.AddLine(Mood.Gray, $"{target.Name} rolls {defendRoll}")
        Dim damage = Math.Clamp(attackRoll - defendRoll, 0, attackRoll)
        If damage > 0 Then
            msg.AddLine(Mood.Gray, $"{target.Name} takes {damage} damage!")
            target.AddWounds(damage)
            If target.IsDead Then
                msg.AddLine(Mood.Gray, $"{Name} kills {target.Name}!")
            End If
        Else
            msg.AddLine(Mood.Gray, $"{Name} misses!")
        End If
        If Not CharacterType.ToDescriptor.IsEnemy Then
            Dim counter As Integer = 1
            For Each enemy In Location.Enemies
                enemy.Fight(counter)
                counter += 1
            Next
        End If
    End Sub

    Public Sub UseItem(item As IItem) Implements ICharacter.UseItem
        If Not item.IsUsable Then
            Dim msg = Message.Create(_data)
            msg.AddLine(Mood.Gray, "You cannot use that.")
            Return
        End If
        If item.OnUse(Me) Then
            item.Location = Nothing
        End If
    End Sub

    Public ReadOnly Property HP As Integer Implements ICharacter.HP
        Get
            Return Math.Clamp(MaximumHP - Wounds, 0, MaximumHP)
        End Get
    End Property

    Public Property MaximumHP As Integer Implements ICharacter.MaximumHP
        Get
            Return GetStatistic(StatisticType.MaximumHP)
        End Get
        Set(value As Integer)
            SetStatistic(StatisticType.MaximumHP, value)
        End Set
    End Property

    Public ReadOnly Property CharacterType As CharacterType Implements ICharacter.CharacterType
        Get
            Return CharacterData.CharacterType
        End Get
    End Property

    Public ReadOnly Property Name As String Implements ICharacter.Name
        Get
            Return CharacterType.ToDescriptor.Name
        End Get
    End Property

    Public ReadOnly Property IsDead As Boolean Implements ICharacter.IsDead
        Get
            Return HP = 0
        End Get
    End Property

    Public ReadOnly Property IsEnemy As Boolean Implements ICharacter.IsEnemy
        Get
            Return CharacterType.ToDescriptor.IsEnemy
        End Get
    End Property

    Public ReadOnly Property Inventory As ILocation Implements ICharacter.Inventory
        Get
            If Not CharacterData.Inventory.HasValue Then
                CharacterData.Inventory = Business.Location.Create(_data).Id
            End If
            Return New Location(_data, CharacterData.inventory.value)
        End Get
    End Property

    Public ReadOnly Property MaximumAttack As Integer Implements ICharacter.MaximumAttack
        Get
            Return GetStatistic(StatisticType.MaximumAttack)
        End Get
    End Property

    Public ReadOnly Property MaximumDefend As Integer Implements ICharacter.MaximumDefend
        Get
            Return GetStatistic(StatisticType.MaximumDefend)
        End Get
    End Property
End Class
