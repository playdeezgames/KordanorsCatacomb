Public Interface ILocation
    ReadOnly Property Id As Integer
    Sub SetBorder(direction As Direction, border As IBorder)
    Function GetBorder(direction As Direction) As IBorder
    Sub SetNeighbor(direction As Direction, location As ILocation)
    Function GetNeighbor(direction As Direction) As ILocation
End Interface
