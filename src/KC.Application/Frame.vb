Friend Module Frame
    Friend Sub Draw(displayBuffer As IPixelSink(Of Hue))
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
        Dim enemy = location.Enemies.FirstOrDefault
        If enemy IsNot Nothing Then
            Dim sprite = CharacterSprites.GetSprite(enemy.CharacterType)
            sprite.StretchTo(displayBuffer, (56, 22), (4, 4), Function(x) x <> Hue.LightMagenta)
        End If
    End Sub
End Module
