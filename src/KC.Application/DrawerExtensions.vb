Imports System.Runtime.CompilerServices

Friend Module DrawerExtensions
    <Extension>
    Friend Sub DrawFrame(drawer As Drawer(Of Hue))
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
    <Extension>
    Friend Sub DrawAheadDoor(drawer As Drawer(Of Hue))
        drawer.
            MoveTo(FrameWidth \ 2 - FrameWidth \ 16, FrameHeight \ 2 - FrameHeight \ 8).
            Right(FrameWidth \ 8 - 1).
            Down(FrameHeight * 3 \ 8 - 1).
            Left(FrameWidth \ 8 - 1).
            Up(FrameHeight * 3 \ 8 - 1)
    End Sub
    <Extension>
    Friend Sub DrawLeftDoor(drawer As Drawer(Of Hue))
        drawer.
            MoveTo(FrameWidth \ 8 - FrameWidth \ 32, FrameHeight \ 2 - FrameHeight \ 8).
            Up(7).
            Right(3).
            DownRight(1).
            Right(2).
            DownRight(1).
            Right(2).
            DownRight(1).
            Down(41).
            Left(1).DownLeft(1).
            Left(1).DownLeft(1).
            Left(1).DownLeft(1).
            Left(1).DownLeft(1).
            Left(1).DownLeft(1).
            Up(42)
    End Sub
    <Extension>
    Friend Sub DrawRightDoor(drawer As Drawer(Of Hue))
        drawer.
            MoveTo(FrameWidth - 1 - FrameWidth \ 8 + FrameWidth \ 32, FrameHeight \ 2 - FrameHeight \ 8).
            Up(7).
            Left(3).
            DownLeft(1).
            Left(2).
            DownLeft(1).
            Left(2).
            DownLeft(1).
            Down(41).
            Right(1).DownRight(1).
            Right(1).DownRight(1).
            Right(1).DownRight(1).
            Right(1).DownRight(1).
            Right(1).DownRight(1).
            Up(42)
    End Sub
End Module
