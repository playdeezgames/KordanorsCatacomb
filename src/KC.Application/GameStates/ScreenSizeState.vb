Friend Class ScreenSizeState
    Inherits BaseGameState(Of Hue, Command, Sfx, GameState)
    Private ReadOnly _saveConfig As Action
    Private Shared ReadOnly _menuItems As IReadOnlyList(Of String) =
        New List(Of String) From
        {
            "640x480",
            "1280x720",
            "1920x1080",
            "2560x1440",
            "3200x1800",
            "3840x2160"
        }
    Private Property CurrentMenuItem As Integer
        Get
            Return (Size.Item1 \ ViewWidth \ 4) - 1
        End Get
        Set(value As Integer)
            Size = ((value + 1) * 4 * ViewWidth, (value + 1) * 4 * ViewHeight)
        End Set
    End Property

    Public Sub New(parent As GameController, setState As Action(Of GameState), saveConfig As Action)
        MyBase.New(parent, setState)
        Me._saveConfig = saveConfig
    End Sub

    Public Overrides Sub HandleCommand(command As Command)
        Select Case command
            Case Command.Up
                CurrentMenuItem = (CurrentMenuItem + _menuItems.Count - 1) Mod _menuItems.Count
            Case Command.Down
                CurrentMenuItem = (CurrentMenuItem + 1) Mod _menuItems.Count
            Case Command.Green, Command.Blue, Command.Red
                _saveConfig()
                SetState(GameState.MainMenu)
        End Select
    End Sub

    Public Overrides Sub Render(displayBuffer As IPixelSink(Of Hue))
        displayBuffer.Fill((0, 0), (ViewWidth, ViewHeight), Hue.Black)
        Dim font = Fonts(GameFont.Font5x7)
        font.WriteText(displayBuffer, (0, 0), "Screen Size:", Hue.Brown)
        Dim row = 0
        For Each menuItem In _menuItems
            font.WriteText(displayBuffer, (ViewWidth \ 2, row * font.Height), menuItem, If(row = CurrentMenuItem, Hue.LightGreen, Hue.Gray))
            row += 1
        Next
    End Sub

    Public Overrides Sub Update(elapsedTime As TimeSpan)
    End Sub
End Class
