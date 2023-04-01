Friend Class Location
    Implements ILocation

    Private ReadOnly _data As WorldData
    Private ReadOnly _locationId As Integer
    Private ReadOnly Property LocationData As LocationData
        Get
            Return _data.Locations(_locationId)
        End Get
    End Property

    Public Sub New(data As WorldData, locationId As Integer)
        _data = data
        _locationId = locationId
    End Sub

    Public Sub SetBorder(direction As Direction, border As IBorder) Implements ILocation.SetBorder
        LocationData.Borders(direction) = border.Id
    End Sub
End Class
