Public Class DirectionDescriptor
    Public ReadOnly RightDirection As Direction
    Public ReadOnly LeftDirection As Direction
    Sub New(leftDirection As Direction, rightDirection As Direction)
        Me.LeftDirection = leftDirection
        Me.RightDirection = rightDirection
    End Sub
End Class
