Public Interface IWorld
    Sub Generate()
    Sub TurnLeft()
    Sub TurnRight()
    Sub Move()
    Sub DismissMessage()
    Property PlayerCharacter As ICharacter
    Property Facing As Direction
    ReadOnly Property AheadDirection As Direction
    ReadOnly Property RightDirection As Direction
    ReadOnly Property LeftDirection As Direction
    ReadOnly Property HasMessages As Boolean
    ReadOnly Property NextMessage As IMessage
End Interface
