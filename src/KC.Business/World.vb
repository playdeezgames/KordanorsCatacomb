Public Class World
    Implements IWorld
    Private ReadOnly _data As WorldData
    Sub New(data As WorldData)
        _data = data
    End Sub

    Public Property PlayerCharacter As ICharacter Implements IWorld.PlayerCharacter
        Get
            If _data.PlayerCharacter.HasValue Then
                Return New Character(_data, _data.PlayerCharacter.Value)
            End If
            Return Nothing
        End Get
        Set(value As ICharacter)
            If value Is Nothing Then
                _data.PlayerCharacter = Nothing
                Return
            End If
            _data.PlayerCharacter = value.Id
        End Set
    End Property

    Public Property Facing As Direction Implements IWorld.Facing
        Get
            Return _data.Facing
        End Get
        Set(value As Direction)
            _data.Facing = value
        End Set
    End Property

    Public ReadOnly Property AheadDirection As Direction Implements IWorld.AheadDirection
        Get
            Return Facing
        End Get
    End Property

    Public ReadOnly Property RightDirection As Direction Implements IWorld.RightDirection
        Get
            Return Facing.ToDescriptor.RightDirection
        End Get
    End Property

    Public ReadOnly Property LeftDirection As Direction Implements IWorld.LeftDirection
        Get
            Return Facing.ToDescriptor.LeftDirection
        End Get
    End Property

    Public Sub Generate() Implements IWorld.Generate
        Clear()
        GenerateMaze()
        PlayerCharacter = CreateCharacter(CharacterType.Larrikin, New Location(_data, _data.DungeonLocations(0, 0)))
        _data.Facing = RNG.FromList(New List(Of Direction) From {Direction.North, Direction.East, Direction.South, Direction.West})
    End Sub

    Public Sub TurnLeft() Implements IWorld.TurnLeft
        Facing = Facing.ToDescriptor.LeftDirection
    End Sub

    Public Sub TurnRight() Implements IWorld.TurnRight
        Facing = Facing.ToDescriptor.RightDirection
    End Sub

    Public Sub Move() Implements IWorld.Move
        Dim location = PlayerCharacter.Location
        Dim border = location.GetBorder(Facing)
        If border.BorderType = BorderType.Door Then
            PlayerCharacter.Location = location.GetNeighbor(Facing)
        End If
    End Sub

    Private Sub Clear()
        _data.Locations = New List(Of LocationData)
    End Sub

    Private Sub GenerateMaze()
        Dim maze As New Maze(Of Direction)(MazeColumns, MazeRows, MazeDirections)
        maze.Generate()
        Dim nsBorders(MazeColumns, MazeRows - 1) As IBorder
        For column = 0 To MazeColumns
            For row = 0 To MazeRows - 1
                nsBorders(column, row) = CreateBorder()
            Next
        Next
        Dim ewBorders(MazeColumns - 1, MazeRows) As IBorder
        For column = 0 To MazeColumns - 1
            For row = 0 To MazeRows
                ewBorders(column, row) = CreateBorder()
            Next
        Next
        ReDim _data.DungeonLocations(MazeColumns - 1, MazeRows - 1)
        Dim locations(MazeColumns - 1, MazeRows - 1) As ILocation
        For column = 0 To MazeColumns - 1
            For row = 0 To MazeRows - 1
                locations(column, row) = CreateLocation()
                _data.DungeonLocations(column, row) = locations(column, row).Id
                locations(column, row).SetBorder(Direction.North, ewBorders(column, row))
                locations(column, row).SetBorder(Direction.East, nsBorders(column + 1, row))
                locations(column, row).SetBorder(Direction.South, ewBorders(column, row + 1))
                locations(column, row).SetBorder(Direction.West, nsBorders(column, row))
            Next
        Next
        For column = 0 To MazeColumns - 1
            For row = 0 To MazeRows - 1
                If row > 0 Then
                    locations(column, row).SetNeighbor(Direction.North, locations(column, row - 1))
                End If
                If column < MazeColumns - 1 Then
                    locations(column, row).SetNeighbor(Direction.East, locations(column + 1, row))
                End If
                If row < MazeRows - 1 Then
                    locations(column, row).SetNeighbor(Direction.South, locations(column, row + 1))
                End If
                If column > 0 Then
                    locations(column, row).SetNeighbor(Direction.West, locations(column - 1, row))
                End If
                Dim westDoor = maze.GetCell(column, row).GetDoor(Direction.West)
                If If(westDoor?.Open, False) Then
                    locations(column, row).GetBorder(Direction.West).BorderType = BorderType.Door
                End If
                Dim northDoor = maze.GetCell(column, row).GetDoor(Direction.North)
                If If(northDoor?.Open, False) Then
                    locations(column, row).GetBorder(Direction.North).BorderType = BorderType.Door
                End If
            Next
        Next
        PlayerCharacter = CreateCharacter(CharacterType.Larrikin, locations(0, 0))
    End Sub

    Private Function CreateCharacter(characterType As CharacterType, location As ILocation) As ICharacter
        Return Character.Create(_data, characterType, location)
    End Function

    Private Function CreateBorder() As IBorder
        Dim borderId = _data.Borders.Count
        _data.Borders.Add(New BorderData)
        Return New Border(_data, borderId)
    End Function

    Private Function CreateLocation() As ILocation
        Dim locationId = _data.Locations.Count
        _data.Locations.Add(New LocationData)
        Return New Location(_data, locationId)
    End Function
End Class
