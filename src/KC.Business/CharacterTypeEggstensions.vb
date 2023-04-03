Imports System.Runtime.CompilerServices

Friend Module CharacterTypeEggstensions
    Private ReadOnly _table As IReadOnlyDictionary(Of CharacterType, CharacterTypeDescriptor) =
        New Dictionary(Of CharacterType, CharacterTypeDescriptor) From
        {
            {
                CharacterType.Larrikin,
                New CharacterTypeDescriptor(
                    New Dictionary(Of StatisticType, Integer) From
                    {
                        {StatisticType.Wounds, 0},
                        {StatisticType.MaximumHP, 3}
                    })
            },
            {
                CharacterType.Blob,
                New CharacterTypeDescriptor(
                    New Dictionary(Of StatisticType, Integer) From
                    {
                        {StatisticType.Wounds, 0},
                        {StatisticType.MaximumHP, 1}
                    },
                    spawnCount:=40)
            }
        }
    Friend ReadOnly Property AllCharacterTypes As IEnumerable(Of CharacterType)
        Get
            Return _table.Keys
        End Get
    End Property
    <Extension>
    Friend Function ToDescriptor(characterType As CharacterType) As CharacterTypeDescriptor
        Return _table(characterType)
    End Function
End Module
