Public Class World
    Implements IWorld
    Private _data As WorldData
    Sub New(data As WorldData)
        _data = data
    End Sub

    Public Sub Generate() Implements IWorld.Generate
    End Sub
End Class
