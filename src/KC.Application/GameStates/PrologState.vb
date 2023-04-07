Friend Class PrologState
    Inherits BaseGameState(Of Hue, Command, Sfx, GameState)

    Public Sub New(parent As IGameController(Of Hue, Command, Sfx), setState As Action(Of GameState))
        MyBase.New(parent, setState)
    End Sub

    Public Overrides Sub HandleCommand(command As Command)
        SetState(GameState.Neutral)
    End Sub

    Public Overrides Sub Render(displayBuffer As IPixelSink(Of Hue))
        displayBuffer.Fill((0, 0), (ViewWidth, ViewHeight), Hue.Black)
        Dim font = Fonts(GameFont.Font4x6)
        font.WriteText(displayBuffer, (0, 0), "PROLOG", Hue.Brown)
        font.WriteText(displayBuffer, (0, 7), "Yer quest: find a cup of tea,   ", Hue.Gray)
        font.WriteText(displayBuffer, (0, 14), "find a match, light the tea to ", Hue.Gray)
        font.WriteText(displayBuffer, (0, 21), "make it 'a lit tea', find a    ", Hue.Gray)
        font.WriteText(displayBuffer, (0, 28), "sponge, find some dew, collect ", Hue.Gray)
        font.WriteText(displayBuffer, (0, 35), "the dew with the sponge, then  ", Hue.Gray)
        font.WriteText(displayBuffer, (0, 42), "squeeze out the dew onto the   ", Hue.Gray)
        font.WriteText(displayBuffer, (0, 49), "lit tea, thus achieving: ", Hue.Gray)
        font.WriteText(displayBuffer, (0, 56), "'Dew a lit tea'", Hue.Green)
        font.WriteText(displayBuffer, (0, 70), "Get it?!? IS PUN! IS FUNNY! LUL", Hue.LightBlue)
    End Sub

    Public Overrides Sub Update(elapsedTime As TimeSpan)
    End Sub
End Class
