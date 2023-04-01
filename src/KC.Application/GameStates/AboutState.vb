Friend Class AboutState
    Inherits BaseGameState(Of Hue, Command, Sfx, GameState)

    Public Sub New(parent As IGameController(Of Hue, Command, Sfx), setState As Action(Of GameState))
        MyBase.New(parent, setState)
    End Sub

    Public Overrides Sub HandleCommand(command As Command)
        SetState(GameState.MainMenu)
    End Sub

    Public Overrides Sub Render(displayBuffer As IPixelSink(Of Hue))
        displayBuffer.Fill((0, 0), (ViewWidth, ViewHeight), Hue.Black)
        Dim font = Fonts(GameFont.Font5x7)
        font.WriteText(displayBuffer, (0, 0), "About Kordanor's Catacomb:", Hue.Brown)
        font.WriteText(displayBuffer, (0, 16), "By TheGrumpyGameDev", Hue.LightBlue)
        font.WriteText(displayBuffer, (0, 32), "Dungeon Crawler Jam 2023", Hue.LightBlue)
        font.WriteText(displayBuffer, (0, 64), "See 'aboot.txt' for more!", Hue.LightBlue)
    End Sub

    Public Overrides Sub Update(elapsedTime As TimeSpan)
    End Sub
End Class
