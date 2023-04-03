Imports System.Runtime.CompilerServices

Friend Module CharacterTypeExtensions
    Private _table As IReadOnlyDictionary(Of CharacterType, CharacterTypeDescriptor) =
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
            }
        }
    <Extension>
    Friend Function ToDescriptor(characterType As CharacterType) As CharacterTypeDescriptor
        Return _table(characterType)
    End Function
End Module
