Public Interface ICharacter
    ReadOnly Property Id As Integer
    Property Location As ILocation
    ReadOnly Property HP As Integer
    ReadOnly Property MaximumHP As Integer
    ReadOnly Property CharacterType As CharacterType
End Interface
