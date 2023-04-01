Friend Class SplashState
    Inherits BaseGameState(Of Hue, Command, Sfx, GameState)
    Private timer As TimeSpan = TimeSpan.FromSeconds(3.0)

    Public Sub New(parent As IGameController(Of Hue, Command, Sfx), setState As Action(Of GameState))
        MyBase.New(parent, setState)
    End Sub

    Public Overrides Sub HandleCommand(command As Command)
        SetState(GameState.MainMenu)
    End Sub

    Public Overrides Sub Render(displayBuffer As IPixelSink(Of Hue))
        displayBuffer.Fill((0, 0), (ViewWidth, ViewHeight), Hue.Black)
        Fonts(GameFont.Font8x8).WriteText(displayBuffer, (0, 0), "Kordanor's Catacomb", Hue.White)
    End Sub

    Public Overrides Sub Update(elapsedTime As TimeSpan)
        timer -= elapsedTime
        If timer.TotalSeconds <= 0.0 Then
            SetState(GameState.MainMenu)
        End If
    End Sub
End Class
