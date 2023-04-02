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
        Dim font = Fonts(GameFont.Font8x8)
        Dim color = RNG.FromList(New List(Of Hue) From {Hue.LightBlue, Hue.LightGreen, Hue.LightCyan, Hue.LightRed, Hue.LightMagenta, Hue.Yellow, Hue.White})
        font.WriteText(displayBuffer, (0, 41 - 16), "Larrikin of SPLORR!!", color)
        font.WriteText(displayBuffer, (72, 41), "][", color)
        font.WriteText(displayBuffer, (4, 41 + 16), "Kordanor's Catacomb", color)
    End Sub

    Public Overrides Sub Update(elapsedTime As TimeSpan)
        timer -= elapsedTime
        If timer.TotalSeconds <= 0.0 Then
            SetState(GameState.MainMenu)
        End If
    End Sub
End Class
