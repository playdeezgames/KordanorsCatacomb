Imports System.Runtime.CompilerServices

Friend Module ItemTypeEggstensions
    Private Sub DoNothing(data As WorldData, character As ICharacter)
        'do nothing, literally
    End Sub
    Private ReadOnly table As IReadOnlyDictionary(Of ItemType, ItemTypeDescriptor) =
        New Dictionary(Of ItemType, ItemTypeDescriptor) From
        {
            {
                ItemType.Knorva,
                New ItemTypeDescriptor("Knorva", AddressOf DoNothing)
            },
            {
                ItemType.Köttbulle,
                New ItemTypeDescriptor("Koettbulle", AddressOf EatKoetbulle, spawnCount:=100, isUsable:=True)
            }
        }

    Private Sub EatKoetbulle(data As WorldData, character As ICharacter)
        character.AddWounds(-1)
        Dim msg = Message.Create(data)
        msg.AddLine(Mood.Gray, "You eat it.")
    End Sub

    <Extension>
    Function ToDescriptor(itemType As ItemType) As ItemTypeDescriptor
        Return table(itemType)
    End Function
    Friend ReadOnly Property AllItemTypes As IEnumerable(Of ItemType)
        Get
            Return table.Keys
        End Get
    End Property
End Module
