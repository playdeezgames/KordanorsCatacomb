Friend Class SFXVolumeState
    Inherits BaseGameState(Of Hue, Command, Sfx, GameState)
    Private _saveConfig As Action
    Private Shared ReadOnly _menuItems As IReadOnlyList(Of String) =
        New List(Of String) From
        {
            "0%",
            "10%",
            "20%",
            "30%",
            "40%",
            "50%",
            "60%",
            "70%",
            "80%",
            "90%",
            "100%"
        }
    Private Property CurrentMenuItem As Integer
        Get
            Return CInt(Math.Clamp(Parent.Volume, 0.0, 1.0) * 10.0)
        End Get
        Set(value As Integer)
            Parent.Volume = CSng(value) / 10.0F
        End Set
    End Property
    Public Sub New(parent As IGameController(Of Hue, Command, Sfx), setState As Action(Of GameState), saveConfig As Action)
        MyBase.New(parent, setState)
        _saveConfig = saveConfig
    End Sub

    Public Overrides Sub HandleCommand(command As Command)
        Select Case command
            Case Command.Up
                CurrentMenuItem = (CurrentMenuItem + _menuItems.Count - 1) Mod _menuItems.Count
                PlaySfx(Sfx.PlayerHit)
            Case Command.Down
                CurrentMenuItem = (CurrentMenuItem + 1) Mod _menuItems.Count
                PlaySfx(Sfx.PlayerHit)
            Case Command.Green, Command.Blue, Command.Red
                _saveConfig()
                SetState(GameState.MainMenu)
        End Select
    End Sub

    Public Overrides Sub Render(displayBuffer As IPixelSink(Of Hue))
        displayBuffer.Fill((0, 0), (ViewWidth, ViewHeight), Hue.Black)
        Dim font = Fonts(GameFont.Font5x7)
        font.WriteText(displayBuffer, (0, 0), "SFX Volume:", Hue.Brown)
        Dim row = 0
        For Each menuItem In _menuItems
            font.WriteText(displayBuffer, (ViewWidth \ 2, row * font.Height), menuItem, If(row = CurrentMenuItem, Hue.LightGreen, Hue.Gray))
            row += 1
        Next
    End Sub

    Public Overrides Sub Update(elapsedTime As TimeSpan)
    End Sub
End Class
