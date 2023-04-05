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

    Public ReadOnly Property HasMessages As Boolean Implements IWorld.HasMessages
        Get
            Return _data.Messages.Any
        End Get
    End Property

    Public ReadOnly Property NextMessage As IMessage Implements IWorld.NextMessage
        Get
            Return New Message(_data, 0)
        End Get
    End Property

    Public Sub Generate() Implements IWorld.Generate
        Clear()
        GenerateMaze()
        PopulateMazeItems()
        PopulateMazeCreatures()
        GeneratePlayerCharacter()
    End Sub

    Private Sub PopulateMazeItems()
        For Each itemType In AllItemTypes
            Dim descriptor = itemType.ToDescriptor
            Dim spawnCount = descriptor.SpawnCount
            While spawnCount > 0
                GenerateItem(itemType)
                spawnCount -= 1
            End While
        Next
    End Sub

    Private Function GenerateItem(itemType As ItemType) As IItem
        Dim descriptor = itemType.ToDescriptor
        Dim location As ILocation
        Dim found As Boolean
        Do
            Dim column = RNG.FromRange(0, MazeColumns - 1)
            Dim row = RNG.FromRange(0, MazeRows - 1)
            location = New Location(_data, _data.DungeonLocations(column, row))
            Dim exitcount = location.ExitCount
            found = exitcount >= descriptor.MinimumExitCount AndAlso exitcount <= descriptor.MaximumExitCount
        Loop Until found
        Return Item.Create(_data, itemType, location)
    End Function

    Private Sub PopulateMazeCreatures()
        For Each characterType In AllCharacterTypes
            Dim descriptor = characterType.ToDescriptor
            Dim spawnCount = descriptor.SpawnCount
            While spawnCount > 0
                GenerateCharacter(characterType)
                spawnCount -= 1
            End While
        Next
    End Sub
    Private Sub GeneratePlayerCharacter()
        PlayerCharacter = GenerateCharacter(CharacterType.Larrikin)
        RandomizeFacing()
    End Sub

    Private Sub RandomizeFacing()
        _data.Facing = RNG.FromList(New List(Of Direction) From {Direction.North, Direction.East, Direction.South, Direction.West})
    End Sub

    Private Function GenerateCharacter(characterType As CharacterType) As ICharacter
        Dim descriptor = characterType.ToDescriptor
        Dim location As ILocation
        Dim found As Boolean
        Do
            Dim column = RNG.FromRange(0, MazeColumns - 1)
            Dim row = RNG.FromRange(0, MazeRows - 1)
            location = New Location(_data, _data.DungeonLocations(column, row))
            Dim exitcount = location.ExitCount
            found = exitcount >= descriptor.MinimumExitCount AndAlso exitcount <= descriptor.MaximumExitCount
        Loop Until found
        Return Character.Create(_data, characterType, location)
    End Function

    Public Sub TurnLeft() Implements IWorld.TurnLeft
        Facing = Facing.ToDescriptor.LeftDirection
    End Sub

    Public Sub TurnRight() Implements IWorld.TurnRight
        Facing = Facing.ToDescriptor.RightDirection
    End Sub

    Public Sub Move() Implements IWorld.Move
        If CanMove() Then
            PlayerCharacter.Location = PlayerCharacter.Location.GetNeighbor(Facing)
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
    End Sub

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

    Public Sub DismissMessage() Implements IWorld.DismissMessage
        _data.Messages.RemoveAt(0)
    End Sub

    Public Sub Run() Implements IWorld.Run
        RandomizeFacing()
        Dim msg As IMessage
        If CanMove Then
            Move()
            msg = Message.Create(_data)
            msg.AddLine(Mood.Gray, $"{PlayerCharacter.Name} runs!")
            Return
        End If
        msg = Message.Create(_data)
        msg.AddLine(Mood.Gray, $"{PlayerCharacter.Name} cannot run!")
        For Each enemy In PlayerCharacter.Location.Enemies
            enemy.Fight()
        Next
    End Sub

    Private Function CanMove() As Boolean
        Dim location = PlayerCharacter.Location
        Dim border = location.GetBorder(Facing)
        Return border.BorderType = BorderType.Door
    End Function

    Public Sub AddMessage(ParamArray lines() As (Mood, String)) Implements IWorld.AddMessage
        Dim msg = Message.Create(Me._data)
        For Each line In lines
            msg.AddLine(line.Item1, line.Item2)
        Next
    End Sub
End Class
