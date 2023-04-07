Public Interface ICharacter
    ReadOnly Property Id As Integer
    Property Location As ILocation
    ReadOnly Property HP As Integer
    Property MaximumHP As Integer
    ReadOnly Property CharacterType As CharacterType
    Sub Fight(Optional index As Integer? = Nothing)
    Function RollDefend() As Integer
    Function RollAttack() As Integer
    Sub AddWounds(wounds As Integer)
    Sub UseItem(item As IItem)
    ReadOnly Property IsEnemy As Boolean
    ReadOnly Property IsDead As Boolean
    ReadOnly Property Name As String
    ReadOnly Property Inventory As ILocation
    ReadOnly Property MaximumAttack As Integer
    Property MaximumDefend As Integer
    Property Defend As Integer
End Interface
