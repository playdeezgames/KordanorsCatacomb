Public Class World
    Implements IWorld
    Private _data As WorldData
    Sub New(data As WorldData)
        _data = data
    End Sub

    Public Sub Generate() Implements IWorld.Generate
        Clear()
        GenerateMaze()
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
        Dim locations(MazeColumns - 1, MazeRows - 1) As ILocation
        For column = 0 To MazeColumns - 1
            For row = 0 To MazeRows - 1
                locations(column, row) = CreateLocation()
                locations(column, row).SetBorder(Direction.North, ewBorders(column, row))
                locations(column, row).SetBorder(Direction.East, nsBorders(column + 1, row))
                locations(column, row).SetBorder(Direction.South, ewBorders(column, row + 1))
                locations(column, row).SetBorder(Direction.West, nsBorders(column, row))
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
                Dim eastDoor = maze.GetCell(column, row).GetDoor(Direction.East)
                If If(eastDoor?.Open, False) Then
                    locations(column, row).GetBorder(Direction.East).BorderType = BorderType.Door
                End If
                Dim southDoor = maze.GetCell(column, row).GetDoor(Direction.South)
                If If(southDoor?.Open, False) Then
                    locations(column, row).GetBorder(Direction.East).BorderType = BorderType.Door
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
End Class
