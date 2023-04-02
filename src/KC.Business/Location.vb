Friend Class Location
    Implements ILocation

    Private ReadOnly _data As WorldData
    Private ReadOnly _locationId As Integer
    Private ReadOnly Property LocationData As LocationData
        Get
            Return _data.Locations(_locationId)
        End Get
    End Property

    Public ReadOnly Property Id As Integer Implements ILocation.Id
        Get
            Return _locationId
        End Get
    End Property

    Public Sub New(data As WorldData, locationId As Integer)
        _data = data
        _locationId = locationId
    End Sub

    Public Sub SetBorder(direction As Direction, border As IBorder) Implements ILocation.SetBorder
        LocationData.Borders(direction) = border.Id
    End Sub

    Public Sub SetNeighbor(direction As Direction, location As ILocation) Implements ILocation.SetNeighbor
        LocationData.Neighbors(direction) = location.Id
    End Sub

    Public Function GetBorder(direction As Direction) As IBorder Implements ILocation.GetBorder
        Dim borderId As Integer = 0
        If LocationData.Borders.TryGetValue(direction, borderId) Then
            Return New Border(_data, borderId)
        End If
        Return Nothing
    End Function
End Class
