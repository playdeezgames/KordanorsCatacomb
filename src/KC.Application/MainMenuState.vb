Friend Class MainMenuState
    Inherits BaseGameState(Of Hue, Command, Sfx, GameState)
    Private Const EmbarkText = "Embark!"
    Private Const ScreenSizeText = "Screen Size..."
    Private Const SFXVolumeText = "SFX Volume..."
    Private Const AboutText = "About..."
    Private Shared ReadOnly _menuItems As IReadOnlyList(Of String) =
        New List(Of String) From
        {
            EmbarkText,
            ScreenSizeText,
            SFXVolumeText,
            AboutText
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
            Case Command.Green, Command.Blue
                Select Case _menuItems(_currentMenuItem)
                    Case SFXVolumeText
                        SetState(GameState.SFXVolume)
                    Case Else
                        'do nothing... yet
                End Select
        End Select
    End Sub

    Public Overrides Sub Render(displayBuffer As IPixelSink(Of Hue))
        displayBuffer.Fill((0, 0), (ViewWidth, ViewHeight), Hue.Black)
        Dim font = Fonts(GameFont.Font5x7)
        Dim row = 0
        For Each menuItem In _menuItems
            font.WriteText(displayBuffer, (0, row * font.Height), menuItem, If(row = _currentMenuItem, Hue.LightGreen, Hue.Gray))
            row += 1
        Next
    End Sub

    Public Overrides Sub Update(elapsedTime As TimeSpan)
    End Sub
End Class
