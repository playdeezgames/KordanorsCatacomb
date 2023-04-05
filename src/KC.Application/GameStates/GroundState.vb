Friend Class GroundState
    Inherits BaseGameState(Of Hue, Command, Sfx, GameState)

    Public Sub New(parent As IGameController(Of Hue, Command, Sfx), setState As Action(Of GameState))
        MyBase.New(parent, setState)
    End Sub

    Public Overrides Sub HandleCommand(command As Command)
        Select Case command
            Case Command.Up
                InventoryIndex = (InventoryIndex + World.PlayerCharacter.Location.Items.Count - 1) Mod World.PlayerCharacter.Location.Items.Count
            Case Command.Down
                InventoryIndex = (InventoryIndex + 1) Mod World.PlayerCharacter.Location.Items.Count
            Case Command.Red
                SetState(GameState.ModeSelect)
            Case Command.Green, Command.Blue
                TakeItem()
        End Select
    End Sub

    Private Sub TakeItem()
        Dim item = World.PlayerCharacter.Location.Items.ToList()(InventoryIndex)
        item.Location = World.PlayerCharacter.Inventory
        If Not World.PlayerCharacter.Location.HasItems Then
            SetState(GameState.ModeSelect)
            Return
        End If
        InventoryIndex = InventoryIndex Mod World.PlayerCharacter.Location.Items.Count
    End Sub

    Public Overrides Sub Render(displayBuffer As IPixelSink(Of Hue))
        displayBuffer.Fill((0, 0), (ViewWidth, ViewHeight), Hue.Black)
        Dim items = World.PlayerCharacter.Location.Items
        Dim font = Fonts(GameFont.Font5x7)
        Dim y = FrameHeight \ 2 - font.Height \ 2 - GameContext.InventoryIndex * font.Height
        Dim index = 0
        For Each item In items
            Dim text As String = item.Name
            Dim h = If(index = InventoryIndex, Hue.LightCyan, Hue.Gray)
            font.WriteText(displayBuffer, (FrameWidth \ 2 - font.TextWidth(text) \ 2, y), text, h)
            y += font.Height
            index += 1
        Next
        DrawStatusBar(displayBuffer)
    End Sub
    Private Sub DrawStatusBar(displayBuffer As IPixelSink(Of Hue))
        displayBuffer.Fill((0, FrameHeight), (ViewWidth, ViewHeight - FrameHeight), Hue.Green)
        Dim font = Fonts(GameFont.Font4x6)
        Dim text = "Arrows: Select       Space: Take"
        font.WriteText(displayBuffer, (FrameWidth \ 2 - font.TextWidth(text) \ 2, FrameHeight + 2), text, Hue.White)
    End Sub

    Public Overrides Sub Update(elapsedTime As TimeSpan)
    End Sub
End Class
