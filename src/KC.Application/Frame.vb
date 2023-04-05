Imports System.ComponentModel

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
        DrawItems(displayBuffer, location)
        DrawEnemy(displayBuffer, location)
        DrawPlayerStats(displayBuffer)
        DrawEnemyCount(displayBuffer)
    End Sub

    Private Sub DrawEnemy(displayBuffer As IPixelSink(Of Hue), location As ILocation)
        Dim enemy = location.Enemies.FirstOrDefault
        If enemy IsNot Nothing Then
            Dim sprite = CharacterSprites.GetSprite(enemy.CharacterType)
            sprite.StretchTo(displayBuffer, (56, 22), (4, 4), Function(x) x <> Hue.LightMagenta)
        End If
    End Sub

    Private ReadOnly drawTable As IReadOnlyDictionary(Of ItemType, Action(Of Drawer(Of Hue))) =
        New Dictionary(Of ItemType, Action(Of Drawer(Of Hue))) From
        {
            {
                ItemType.Köttbulle,
                Sub(d)
                    d.
                        MoveTo(FrameWidth \ 4, FrameHeight * 7 \ 8).
                        Color(Hue.Brown).
                        Right(1).
                        DownRight(1).
                        Down(1).
                        DownLeft(1).
                        Left(1).
                        UpLeft(1).
                        Up(1).
                        UpRight(1)
                End Sub
            },
            {
                ItemType.Knorva,
                Sub(d)

                End Sub
            }
        }

    Private Sub DrawItems(displayBuffer As IPixelSink(Of Hue), location As ILocation)
        Dim itemTypes As IEnumerable(Of ItemType) = location.ItemTypes
        Dim drawer As New Drawer(Of Hue)(displayBuffer, (0, 0), Hue.White)
        For Each itemType In itemTypes
            drawTable(itemType)(drawer)
        Next
    End Sub

    Private Sub DrawEnemyCount(displayBuffer As IPixelSink(Of Hue))
        Dim enemyCount = World.PlayerCharacter.Location.Enemies.Count
        If enemyCount = 0 Then
            Return
        End If
        Dim font = Fonts(GameFont.Font4x6)
        Dim text = $"Enemy: {enemyCount}"
        font.WriteText(displayBuffer, (FrameWidth - font.TextWidth(text), 0), text, Hue.Blue)
    End Sub

    Private Sub DrawPlayerStats(displayBuffer As IPixelSink(Of Hue))
        Dim character = World.PlayerCharacter
        Dim font = Fonts(GameFont.Font4x6)
        font.WriteText(displayBuffer, (0, 0), $"HP: {character.HP}/{character.MaximumHP}", Hue.Red)
    End Sub
End Module
