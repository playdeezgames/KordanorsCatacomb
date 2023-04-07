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
            },
            {
                ItemType.Sponge,
                Sub(d)
                    d.MoveTo(FrameWidth \ 4, FrameHeight * 3 \ 4).Color(Hue.Yellow).Right(3).Down(5).Left(3).Up(5)
                End Sub
            },
            {
                ItemType.WetSponge,
                Sub(d)
                    d.MoveTo(FrameWidth \ 4, FrameHeight * 3 \ 4).Color(Hue.Yellow).Right(3).Down(5).Left(3).Up(5)
                End Sub
            },
            {
                ItemType.Tea,
                Sub(d)
                    DrawBaseTea(d)
                End Sub
            },
            {
                ItemType.LitTea,
                Sub(d)
                    DrawBaseTea(d)
                End Sub
            },
            {
                ItemType.DewALitTea,
                Sub(d)
                    DrawBaseTea(d)
                End Sub
            },
            {
                ItemType.Match,
                Sub(d)
                    d.MoveTo(FrameWidth \ 2, FrameHeight * 7 \ 8).Color(Hue.Red).DownRight(1).Color(Hue.Brown).DownRight(4)
                End Sub
            },
            {
                ItemType.BurntMatch,
                Sub(d)
                    d.MoveTo(FrameWidth \ 2, FrameHeight * 7 \ 8).Color(Hue.DarkGray).DownRight(2).Color(Hue.Brown).DownRight(3)
                End Sub
            },
            {
                ItemType.DewPuddle,
                Sub(d)
                    d.MoveTo(FrameWidth * 3 \ 4, FrameHeight * 7 \ 8).Color(Hue.Blue).Right(2).DownRight(1).Right(1).DownRight(1).Right(2).DownRight(1).Down(1).DownLeft(1).Left(1).DownLeft(1).Left(4).UpLeft(3).Up(1).UpRight(2)
                End Sub
            },
            {
                ItemType.SyltLingon,
                Sub(d)
                    Throw New NotImplementedException
                End Sub
            },
            {
                ItemType.Vörda,
                Sub(d)
                    Throw New NotImplementedException
                End Sub
            },
            {
                ItemType.Compass,
                Sub(d)
                    d.MoveTo(FrameWidth * 5 \ 8, FrameHeight * 7 \ 8).Color(Hue.Gray).Right(2).Down(1).Right(1).Down(4).Left(1).Down(1).Left(4).Up(1).Left(1).Up(4).Right(1).Up(1).Right(2).Color(Hue.Red).Down(4)
                End Sub
            }
        }

    Private Sub DrawBaseTea(d As Drawer(Of Hue))
        d.MoveTo(FrameWidth \ 2, FrameHeight * 3 \ 4).Color(Hue.White).Left(2).Down(3).DownRight(1).Right(3).UpRight(1).Right(1).UpRight(1).UpLeft(1).Left(1).Down(1).Up(2).Left(3)
    End Sub

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
        Dim font = Fonts(GameFont.Font3x5)
        font.WriteText(displayBuffer, (0, 0), $"HP: {character.HP}/{character.MaximumHP}", Hue.Red)
        font.WriteText(displayBuffer, (0, 6), $"ATK: {character.MaximumAttack}", Hue.Green)
        font.WriteText(displayBuffer, (0, 12), $"DEF: {character.MaximumDefend}", Hue.Yellow)
        If character.Inventory.ItemTypes.Contains(ItemType.Compass) Then
            Dim text = World.Facing.ToString
            font.WriteText(displayBuffer, (FrameWidth \ 2 - font.TextWidth(text) \ 2, 0), text, Hue.Magenta)
        End If
    End Sub
End Module
