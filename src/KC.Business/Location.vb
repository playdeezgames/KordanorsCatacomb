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

    Public ReadOnly Property ExitCount As Integer Implements ILocation.ExitCount
        Get
            Return LocationData.Borders.Keys.
                Where(Function(x) GetBorder(x).BorderType.ToDescriptor.IsExit).
                Count()
        End Get
    End Property

    Private ReadOnly Property Characters As IEnumerable(Of ICharacter)
        Get
            Return LocationData.Characters.Select(Function(x) New Character(_data, x))
        End Get
    End Property


    Public ReadOnly Property Enemies As IEnumerable(Of ICharacter) Implements ILocation.Enemies
        Get
            Return Characters.Where(Function(x) Not x.IsDead AndAlso x.IsEnemy)
        End Get
    End Property

    Public ReadOnly Property HasEnemies As Boolean Implements ILocation.HasEnemies
        Get
            Return Enemies.Any
        End Get
    End Property

    Public ReadOnly Property Allies As IEnumerable(Of ICharacter) Implements ILocation.Allies
        Get
            Return Characters.Where(Function(x) Not x.IsDead And Not x.IsEnemy)
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

    Public Function GetNeighbor(direction As Direction) As ILocation Implements ILocation.GetNeighbor
        Dim locationId As Integer = 0
        If LocationData.Neighbors.TryGetValue(direction, locationId) Then
            Return New Location(_data, locationId)
        End If
        Return Nothing
    End Function
End Class
