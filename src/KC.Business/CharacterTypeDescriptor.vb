Friend Class CharacterTypeDescriptor
    ReadOnly Property Statistics As IReadOnlyDictionary(Of StatisticType, Integer)
    ReadOnly Property MaximumExitCount As Integer
    ReadOnly Property MinimumExitCount As Integer
    Sub New(
           statistics As IReadOnlyDictionary(Of StatisticType, Integer),
           Optional maximumExitCount As Integer = 4,
           Optional minimumExitCount As Integer = 0)
        Me.Statistics = statistics
        Me.MaximumExitCount = maximumExitCount
        Me.MinimumExitCount = minimumExitCount
    End Sub
End Class
