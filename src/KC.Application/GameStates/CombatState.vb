Friend Class CombatState
    Inherits BaseGameState(Of Hue, Command, Sfx, GameState)

    Public Sub New(parent As IGameController(Of Hue, Command, Sfx), setState As Action(Of GameState))
        MyBase.New(parent, setState)
    End Sub

    Public Overrides Sub HandleCommand(command As Command)
        SetState(GameState.Navigation)
    End Sub

    Public Overrides Sub Render(displayBuffer As IPixelSink(Of Hue))
        Frame.Draw(displayBuffer)
        DrawStatusBar(displayBuffer)
    End Sub

    Private Sub DrawStatusBar(displayBuffer As IPixelSink(Of Hue))
        displayBuffer.Fill((0, FrameHeight), (ViewWidth, ViewHeight - FrameHeight), Hue.Green)
        Dim font = Fonts(GameFont.Font4x6)
        font.WriteText(displayBuffer, (0, FrameHeight + 2), "In Combat", Hue.White)
    End Sub

    Public Overrides Sub Update(elapsedTime As TimeSpan)
    End Sub
End Class
