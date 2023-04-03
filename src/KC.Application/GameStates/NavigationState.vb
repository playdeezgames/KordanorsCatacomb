﻿Friend Class NavigationState
    Inherits BaseGameState(Of Hue, Command, Sfx, GameState)

    Public Sub New(parent As IGameController(Of Hue, Command, Sfx), setState As Action(Of GameState))
        MyBase.New(parent, setState)
    End Sub

    Public Overrides Sub HandleCommand(command As Command)
        Select Case command
            Case Command.Left
                World.TurnLeft()
            Case Command.Right
                World.TurnRight()
            Case Command.Up
                World.Move()
        End Select
    End Sub

    Public Overrides Sub Render(displayBuffer As IPixelSink(Of Hue))
        'frame
        DrawFrame(displayBuffer)

        Dim character = World.PlayerCharacter
        Dim font = Fonts(GameFont.Font4x6)
        font.WriteText(displayBuffer, (0, 0), $"HP: {character.HP}/{character.MaximumHP}", Hue.Red)

        'status bar
        DrawStatusBar(displayBuffer)
    End Sub

    Private Sub DrawFrame(displayBuffer As IPixelSink(Of Hue))
        displayBuffer.Fill((0, 0), (FrameWidth, FrameHeight), Hue.Black)
        Dim drawer As New Drawer(Of Hue)(displayBuffer, hue:=Hue.DarkGray)
        drawer.DrawFrame()
        Dim character = World.PlayerCharacter
        Dim location = character.Location
        Dim aheadBorder = location.GetBorder(World.AheadDirection)
        If aheadBorder.BorderType = Data.BorderType.Door Then
            drawer.DrawAheadDoor()
        End If
        Dim leftBorder = location.GetBorder(World.LeftDirection)
        If leftBorder.BorderType = Data.BorderType.Door Then
            drawer.DrawLeftDoor()
        End If
        Dim rightBorder = location.GetBorder(World.RightDirection)
        If rightBorder.BorderType = Data.BorderType.Door Then
            drawer.DrawRightDoor()
        End If
    End Sub

    Private Sub DrawStatusBar(displayBuffer As IPixelSink(Of Hue))
        displayBuffer.Fill((0, FrameHeight), (ViewWidth, ViewHeight - FrameHeight), Hue.Green)
        Dim font = Fonts(GameFont.Font4x6)
        font.WriteText(displayBuffer, (0, FrameHeight + 2), "Arrows: Move/Turn      Esc: Menu", Hue.White)
    End Sub

    Public Overrides Sub Update(elapsedTime As TimeSpan)
    End Sub
End Class
