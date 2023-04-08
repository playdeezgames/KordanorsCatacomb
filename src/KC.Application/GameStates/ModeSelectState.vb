Friend Class ModeSelectState
    Inherits BaseGameState(Of Hue, Command, Sfx, GameState)
    Private Const NavigationText = "<- Navigate ->"
    Private Const GroundText = "<- Ground ->"
    Private Const InventoryText = "<- Inventory ->"
    Private Const MapText = "<- Map ->"
    Private ReadOnly _table As IReadOnlyDictionary(Of Integer, Sprite(Of Hue)) =
        New Dictionary(Of Integer, Sprite(Of Hue)) From
        {
            {
                0,
                New Sprite(Of Hue)(New List(Of String) From
                {
                    "........",
                    ".XXXXXX.",
                    ".X    X.",
                    ".X    X.",
                    ".X    X.",
                    ".X    X.",
                    ".XXXXXX.",
                    "........"
                }, AddressOf ColorizeMap)},
            {
                1,
                New Sprite(Of Hue)(New List(Of String) From
                {
                    "..X  X..",
                    ".XX  XX.",
                    ".X    X.",
                    ".X    X.",
                    ".X    X.",
                    ".X    X.",
                    ".XXXXXX.",
                    "........"
                }, AddressOf ColorizeMap)},
            {
                2,
                New Sprite(Of Hue)(New List(Of String) From
                {
                    "........",
                    ".XXXXXX.",
                    ".X    XX",
                    ".X      ",
                    ".X      ",
                    ".X    XX",
                    ".XXXXXX.",
                    "........"
                }, AddressOf ColorizeMap)},
            {
                3,
                New Sprite(Of Hue)(New List(Of String) From
                {
                    "..X  X..",
                    ".XX  XX.",
                    ".X    XX",
                    ".X      ",
                    ".X      ",
                    ".X    XX",
                    ".XXXXXX.",
                    "........"
                }, AddressOf ColorizeMap)},
                            {
                4,
                New Sprite(Of Hue)(New List(Of String) From
                {
                    "........",
                    ".XXXXXX.",
                    ".X    X.",
                    ".X    X.",
                    ".X    X.",
                    ".X    X.",
                    ".XX  XX.",
                    "..X  X.."
                }, AddressOf ColorizeMap)},
            {
                5,
                New Sprite(Of Hue)(New List(Of String) From
                {
                    "..X  X..",
                    ".XX  XX.",
                    ".X    X.",
                    ".X    X.",
                    ".X    X.",
                    ".X    X.",
                    ".XX  XX.",
                    "..X  X.."
                }, AddressOf ColorizeMap)},
            {
                6,
                New Sprite(Of Hue)(New List(Of String) From
                {
                    "........",
                    ".XXXXXX.",
                    ".X    XX",
                    ".X      ",
                    ".X      ",
                    ".X    XX",
                    ".XX  XX.",
                    "..X  X.."
                }, AddressOf ColorizeMap)},
            {
                7,
                New Sprite(Of Hue)(New List(Of String) From
                {
                    "..X  X..",
                    ".XX  XX.",
                    ".X    XX",
                    ".X      ",
                    ".X      ",
                    ".X    XX",
                    ".XX  XX.",
                    "..X  X.."
                }, AddressOf ColorizeMap)},
            {
                8,
                New Sprite(Of Hue)(New List(Of String) From
                {
                    "........",
                    ".XXXXXX.",
                    "XX    X.",
                    "      X.",
                    "      X.",
                    "XX    X.",
                    ".XXXXXX.",
                    "........"
                }, AddressOf ColorizeMap)},
            {
                9,
                New Sprite(Of Hue)(New List(Of String) From
                {
                    "..X  X..",
                    ".XX  XX.",
                    "XX    X.",
                    "      X.",
                    "      X.",
                    "XX    X.",
                    ".XXXXXX.",
                    "........"
                }, AddressOf ColorizeMap)},
            {
                10,
                New Sprite(Of Hue)(New List(Of String) From
                {
                    "........",
                    ".XXXXXX.",
                    "XX    XX",
                    "        ",
                    "        ",
                    "XX    XX",
                    ".XXXXXX.",
                    "........"
                }, AddressOf ColorizeMap)},
            {
                11,
                New Sprite(Of Hue)(New List(Of String) From
                {
                    "..X  X..",
                    ".XX  XX.",
                    "XX    XX",
                    "        ",
                    "        ",
                    "XX    XX",
                    ".XXXXXX.",
                    "........"
                }, AddressOf ColorizeMap)},
                            {
                12,
                New Sprite(Of Hue)(New List(Of String) From
                {
                    "........",
                    ".XXXXXX.",
                    "XX    X.",
                    "      X.",
                    "      X.",
                    "XX    X.",
                    ".XX  XX.",
                    "..X  X.."
                }, AddressOf ColorizeMap)},
            {
                13,
                New Sprite(Of Hue)(New List(Of String) From
                {
                    "..X  X..",
                    ".XX  XX.",
                    "XX    X.",
                    "      X.",
                    "      X.",
                    "XX    X.",
                    ".XX  XX.",
                    "..X..X.."
                }, AddressOf ColorizeMap)},
            {
                14,
                New Sprite(Of Hue)(New List(Of String) From
                {
                    "........",
                    ".XXXXXX.",
                    "XX    XX",
                    "        ",
                    "        ",
                    "XX    XX",
                    ".XX  XX.",
                    "..X  X.."
                }, AddressOf ColorizeMap)},
            {
                15,
                New Sprite(Of Hue)(New List(Of String) From
                {
                    "..X  X..",
                    ".XX  XX.",
                    "XX    XX",
                    "        ",
                    "        ",
                    "XX    XX",
                    ".XX  XX.",
                    "..X  X.."
                }, AddressOf ColorizeMap)}
        }

    Private Function ColorizeMap(character As Char) As Hue
        Select Case character
            Case "X"c
                Return Hue.White
            Case " "c
                Return Hue.DarkGray
            Case Else
                Return Hue.Black
        End Select
    End Function

    Private ReadOnly _menuItems As IReadOnlyList(Of String) =
        New List(Of String) From
        {
            NavigationText,
            GroundText,
            InventoryText,
            MapText
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
                    Case InventoryText
                        HandleInventory()
                End Select
            Case Command.Red
                _currentMenuItem = 0
                SetState(GameState.Neutral)
        End Select
    End Sub

    Private Sub HandleInventory()
        If Not World.PlayerCharacter.Inventory.HasItems Then
            _currentMenuItem = 0
            World.AddMessage(
                (Mood.Gray, "You have no items."))
            SetState(GameState.Neutral)
            Return
        End If
        GameContext.InventoryIndex = 0
        SetState(GameState.Inventory)
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
        GameContext.InventoryIndex = 0
        SetState(GameState.Ground)
    End Sub

    Public Overrides Sub Render(displayBuffer As IPixelSink(Of Hue))
        Frame.Draw(displayBuffer)
        DrawStatusBar(displayBuffer)
        If _menuItems(_currentMenuItem) = MapText Then
            DrawMap(displayBuffer)
        End If
    End Sub

    Private Sub DrawMap(displayBuffer As IPixelSink(Of Hue))
        Dim playerLocationId = World.PlayerCharacter.Location.Id
        For column = 0 To MazeColumns - 1
            For row = 0 To MazeRows - 1
                Dim location = World.GetDungeonLocation(column, row)
                If location.IsVisitedBy(World.PlayerCharacter) Then
                    Dim index = 0
                    If location.GetBorder(Direction.North).BorderType = BorderType.Door Then
                        index += 1
                    End If
                    If location.GetBorder(Direction.East).BorderType = BorderType.Door Then
                        index += 2
                    End If
                    If location.GetBorder(Direction.South).BorderType = BorderType.Door Then
                        index += 4
                    End If
                    If location.GetBorder(Direction.West).BorderType = BorderType.Door Then
                        index += 8
                    End If
                    _table(index).StretchTo(displayBuffer, (FrameWidth \ 2 - 32 + column * 8, 4 + row * 8), (1, 1), Function(x) True)
                    If location.Id = playerLocationId Then
                        displayBuffer.Fill((FrameWidth \ 2 - 32 + column * 8 + 3, 4 + row * 8 + 3), (2, 2), Hue.Magenta)
                    End If
                Else
                    displayBuffer.Fill((FrameWidth \ 2 - 32 + column * 8, 4 + row * 8), (8, 8), Hue.Black)
                End If
            Next
        Next
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
