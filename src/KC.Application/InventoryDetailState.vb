Friend Class InventoryDetailState
    Inherits BaseGameState(Of Hue, Command, Sfx, GameState)
    Private Const GoBackText = "Go Back"
    Private Const UseText = "Use"
    Private Const DropText = "Drop"
    Private Shared ReadOnly _menuItems As IReadOnlyList(Of String) =
        New List(Of String) From
        {
            GoBackText,
            UseText,
            DropText
        }
    Private _currentMenuItem As Integer = 0

    Public Sub New(parent As IGameController(Of Hue, Command, Sfx), setState As Action(Of GameState))
        MyBase.New(parent, setState)
    End Sub

    Public Overrides Sub HandleCommand(command As Command)
        Select Case command
            Case Command.Up
                _currentMenuItem = (_currentMenuItem + _menuItems.Count - 1) Mod _menuItems.Count
            Case Command.Down
                _currentMenuItem = (_currentMenuItem + 1) Mod _menuItems.Count
            Case Command.Red
                SetState(GameState.Inventory)
            Case Command.Green, Command.Blue
                Select Case _menuItems(_currentMenuItem)
                    Case GoBackText
                        SetState(GameState.Inventory)
                    Case UseText
                        HandleUse()
                    Case DropText
                        HandleDrop()
                    Case Else
                        Throw New NotImplementedException
                End Select
        End Select
    End Sub

    Private Sub HandleDrop()
        Dim item = World.PlayerCharacter.Inventory.Items.ToList()(InventoryIndex)
        item.Location = World.PlayerCharacter.Location
        GoToNextState()
    End Sub

    Private Sub GoToNextState()
        If Not World.PlayerCharacter.Inventory.HasItems Then
            SetState(GameState.ModeSelect)
            Return
        End If
        InventoryIndex = InventoryIndex Mod World.PlayerCharacter.Inventory.Items.Count
        SetState(GameState.Inventory)
    End Sub

    Private Sub HandleUse()
        Dim item = World.PlayerCharacter.Inventory.Items.ToList()(InventoryIndex)
        If Not item.IsUsable Then
            World.AddMessage((Mood.Gray, "You cannot use that!"))
            SetState(GameState.Neutral)
            Return
        End If
        World.PlayerCharacter.UseItem(item)
        SetState(GameState.Neutral)
    End Sub

    Public Overrides Sub Render(displayBuffer As IPixelSink(Of Hue))
        Dim item = World.PlayerCharacter.Inventory.Items.ToList()(InventoryIndex)
        displayBuffer.Fill((0, 0), (ViewWidth, ViewHeight), Hue.Black)
        Dim font = Fonts(GameFont.Font5x7)
        font.WriteText(displayBuffer, (0, 0), item.Name, Hue.Magenta)
        Dim row = 0
        For Each menuItem In _menuItems
            font.WriteText(displayBuffer, (0, row * font.Height + font.Height), menuItem, If(row = _currentMenuItem, Hue.LightGreen, Hue.Gray))
            row += 1
        Next
        DrawStatusBar(displayBuffer)
    End Sub

    Private Sub DrawStatusBar(displayBuffer As IPixelSink(Of Hue))
        displayBuffer.Fill((0, FrameHeight), (ViewWidth, ViewHeight - FrameHeight), Hue.Green)
        Dim font = Fonts(GameFont.Font4x6)
        Dim text = "Arrows: Select        Space: Do"
        font.WriteText(displayBuffer, (FrameWidth \ 2 - font.TextWidth(text) \ 2, FrameHeight + 2), text, Hue.White)
    End Sub

    Public Overrides Sub Update(elapsedTime As TimeSpan)
    End Sub
End Class
