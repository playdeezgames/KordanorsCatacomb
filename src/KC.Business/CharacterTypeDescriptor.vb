Friend Class CharacterTypeDescriptor
    ReadOnly Property Statistics As IReadOnlyDictionary(Of StatisticType, Integer)
    Sub New(statistics As IReadOnlyDictionary(Of StatisticType, Integer))
        Me.Statistics = statistics
    End Sub
End Class
