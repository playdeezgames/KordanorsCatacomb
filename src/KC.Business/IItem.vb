Public Interface IItem
    ReadOnly Property Id As Integer
    Property Location As ILocation
    ReadOnly Property ItemType As ItemType
    ReadOnly Property Name As String
End Interface
