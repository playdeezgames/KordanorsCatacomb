Imports System.Runtime.CompilerServices

Friend Module CharacterTypeEggstensions
    Private ReadOnly _table As IReadOnlyDictionary(Of CharacterType, CharacterTypeDescriptor) =
        New Dictionary(Of CharacterType, CharacterTypeDescriptor) From
        {
            {
                CharacterType.Larrikin,
                New CharacterTypeDescriptor(
                    "Larrikin",
                    New Dictionary(Of StatisticType, Integer) From
                    {
                        {StatisticType.Wounds, 0},
                        {StatisticType.MaximumHP, 3},
                        {StatisticType.Attack, 3},
                        {StatisticType.MaximumAttack, 1},
                        {StatisticType.Defend, 4},
                        {StatisticType.MaximumDefend, 2}
                    },
                    isEnemy:=False)
            },
            {
                CharacterType.Blob,
                New CharacterTypeDescriptor(
                    "Blob",
                    New Dictionary(Of StatisticType, Integer) From
                    {
                        {StatisticType.Wounds, 0},
                        {StatisticType.MaximumHP, 1},
                        {StatisticType.Attack, 2},
                        {StatisticType.MaximumAttack, 1},
                        {StatisticType.Defend, 1},
                        {StatisticType.MaximumDefend, 1}
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
