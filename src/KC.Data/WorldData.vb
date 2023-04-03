Public Class WorldData
    Public Property Locations As New List(Of LocationData)
    Public Property Borders As New List(Of BorderData)
    Public Property Characters As New List(Of CharacterData)
    Public Property PlayerCharacter As Integer?
    Public Property Facing As Direction
    Public Property DungeonLocations As Integer(,)
End Class
