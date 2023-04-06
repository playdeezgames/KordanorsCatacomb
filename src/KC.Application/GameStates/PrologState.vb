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
        font.WriteText(displayBuffer, (0, 7), "Yer quest: find the pedestal,   ", Hue.Gray)
        font.WriteText(displayBuffer, (0, 14), "put a 'T' on it, find a match,  ", Hue.Gray)
        font.WriteText(displayBuffer, (0, 21), "light the 'T' to make it 'a lit ", Hue.Gray)
        font.WriteText(displayBuffer, (0, 28), " T', find a sponge, find some   ", Hue.Gray)
        font.WriteText(displayBuffer, (0, 35), "dew, collect the dew with the   ", Hue.Gray)
        font.WriteText(displayBuffer, (0, 42), "sponge, then squeeze out the dew", Hue.Gray)
        font.WriteText(displayBuffer, (0, 49), "onto the lit T, thus achieving: ", Hue.Gray)
        font.WriteText(displayBuffer, (0, 56), "'Dew a lit T'", Hue.Green)
        font.WriteText(displayBuffer, (0, 70), "Get it?!? IS PUN! IS FUNNY! LUL", Hue.LightBlue)
    End Sub

    Public Overrides Sub Update(elapsedTime As TimeSpan)
    End Sub
End Class
