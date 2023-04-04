Friend Class MessageState
    Inherits BaseGameState(Of Hue, Command, Sfx, GameState)

    Public Sub New(parent As IGameController(Of Hue, Command, Sfx), setState As Action(Of GameState))
        MyBase.New(parent, setState)
    End Sub

    Public Overrides Sub HandleCommand(command As Command)
        Select Case command
            Case Command.Green, Command.Blue
                World.DismissMessage()
                SetState(GameState.Neutral)
        End Select
    End Sub

    Public Overrides Sub Render(displayBuffer As IPixelSink(Of Hue))
        displayBuffer.Fill((0, 0), (ViewWidth, ViewHeight), Hue.Black)
        Dim font = Fonts(GameFont.Font5x7)
        Dim message As IMessage = World.NextMessage
        Dim y As Integer = 0
        For Each line In message.Lines
            font.WriteText(displayBuffer, (0, y), line.Text, Hue.Gray)
            y += font.Height
        Next
    End Sub

    Public Overrides Sub Update(elapsedTime As TimeSpan)
    End Sub
End Class
