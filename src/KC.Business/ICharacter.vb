Public Interface ICharacter
    ReadOnly Property Id As Integer
    Property Location As ILocation
    ReadOnly Property HP As Integer
    ReadOnly Property MaximumHP As Integer
    ReadOnly Property CharacterType As CharacterType
    Sub Fight()
    Function RollDefend() As Integer
    Function RollAttack() As Integer
    Sub AddWounds(wounds As Integer)
    ReadOnly Property IsDead As Boolean
    ReadOnly Property Name As String
End Interface
