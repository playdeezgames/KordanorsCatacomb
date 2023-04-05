Friend Class ModeSelectState
    Inherits BaseGameState(Of Hue, Command, Sfx, GameState)
    Private Const NavigationText = "<- Navigate ->"
    Private Const GroundText = "<- Ground ->"
    Private Const InventoryText = "<- Inventory ->"
    Private ReadOnly _menuItems As IReadOnlyList(Of String) =
        New List(Of String) From
        {
            NavigationText,
            GroundText,
            InventoryText
        }
    Private _currentMenuItem As Integer = 0

    Public Sub New(parent As IGameController(Of Hue, Command, Sfx), setState As Action(Of GameState))
        MyBase.New(parent, setState)
    End Sub

    Public Overrides Sub HandleCommand(command As Command)
        Select Case command
            Case Command.Right
                _currentMenuItem = (_currentMenuItem + 1) Mod _menuItems.Count
            Case Command.Left
                _currentMenuItem = (_currentMenuItem + _menuItems.Count - 1) Mod _menuItems.Count
            Case Command.Green, Command.Blue
                Select Case _menuItems(_currentMenuItem)
                    Case NavigationText
                        SetState(GameState.Neutral)
                    Case GroundText
                        HandleGround()
                End Select
            Case Command.Red
                _currentMenuItem = 0
                SetState(GameState.Neutral)
        End Select
    End Sub

    Private Sub HandleGround()
        If Not World.PlayerCharacter.Location.HasItems Then
            _currentMenuItem = 0
            World.AddMessage(
                (Mood.Gray, "There are no items"),
                (Mood.Gray, "on the ground!"))
            SetState(GameState.Neutral)
            Return
        End If
    End Sub

    Public Overrides Sub Render(displayBuffer As IPixelSink(Of Hue))
        Frame.Draw(displayBuffer)
        DrawStatusBar(displayBuffer)
    End Sub

    Private Sub DrawStatusBar(displayBuffer As IPixelSink(Of Hue))
        displayBuffer.Fill((0, FrameHeight), (ViewWidth, ViewHeight - FrameHeight), Hue.Green)
        Dim font = Fonts(GameFont.Font4x6)
        Dim text = _menuItems(_currentMenuItem)
        font.WriteText(displayBuffer, (FrameWidth \ 2 - font.TextWidth(text) \ 2, FrameHeight + 2), text, Hue.White)
    End Sub

    Public Overrides Sub Update(elapsedTime As TimeSpan)
    End Sub
End Class
