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
        drawer.
            MoveTo(FrameWidth \ 4, FrameHeight \ 4).
            Right(FrameWidth \ 2 - 1).
            Down(FrameHeight \ 2 - 1).
            Left(FrameWidth \ 2 - 1).
            Up(FrameHeight \ 2 - 1).
            MoveTo(0, 0).
            Repeat(FrameWidth \ 8, Function(x) x.Right(1).DownRight(1)).
            MoveTo(0, FrameHeight - 1).
            Repeat(FrameWidth \ 8, Function(x) x.Right(1).UpRight(1)).
            MoveTo(FrameWidth - 1, 0).
            Repeat(FrameWidth \ 8, Function(x) x.Left(1).DownLeft(1)).
            MoveTo(FrameWidth - 1, FrameHeight - 1).
            Repeat(FrameWidth \ 8, Function(x) x.Left(1).UpLeft(1))


    End Sub

    Public Overrides Sub Update(elapsedTime As TimeSpan)
    End Sub
End Class
