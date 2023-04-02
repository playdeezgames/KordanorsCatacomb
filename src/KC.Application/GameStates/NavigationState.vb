Friend Class NavigationState
    Inherits BaseGameState(Of Hue, Command, Sfx, GameState)

    Public Sub New(parent As IGameController(Of Hue, Command, Sfx), setState As Action(Of GameState))
        MyBase.New(parent, setState)
    End Sub

    Public Overrides Sub HandleCommand(command As Command)
    End Sub

    Public Overrides Sub Render(displayBuffer As IPixelSink(Of Hue))
        displayBuffer.Fill((0, 0), (FrameWidth, FrameHeight), Hue.Black)
        displayBuffer.Fill((0, FrameHeight), (ViewWidth, ViewHeight - FrameHeight), Hue.Green)
        Dim drawer As New Drawer(Of Hue)(displayBuffer, hue:=Hue.DarkGray)
        drawer.DrawFrame()
        drawer.DrawAheadDoor()
        drawer.DrawLeftDoor()
        drawer.DrawRightDoor()
    End Sub

    Public Overrides Sub Update(elapsedTime As TimeSpan)
    End Sub
End Class
