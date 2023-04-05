Public Class CharacterData
    Public Property Location As Integer?
    Public Property Statistics As New Dictionary(Of StatisticType, Integer)
    Public Property CharacterType As CharacterType
    Public Property Inventory As Integer?
End Class
