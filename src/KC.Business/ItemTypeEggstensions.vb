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
            },
            {
                ItemType.Sponge,
                New ItemTypeDescriptor("Sponge", AddressOf UseSponge, spawnCount:=100, isUsable:=True)
            },
            {
                ItemType.WetSponge,
                New ItemTypeDescriptor("Wet Sponge", AddressOf UseWetSponge, isUsable:=True)
            },
            {
                ItemType.Tea,
                New ItemTypeDescriptor("Tea", AddressOf UseTea, spawnCount:=1, isUsable:=True)
            },
            {
                ItemType.Match,
                New ItemTypeDescriptor("Match", AddressOf UseMatch, spawnCount:=1, isUsable:=True)
            },
            {
                ItemType.BurntMatch,
                New ItemTypeDescriptor("Burnt Match", AddressOf DoNothing)
            },
            {
                ItemType.LitTea,
                New ItemTypeDescriptor("Lit Tea", AddressOf DoNothing)
            },
            {
                ItemType.DewALitTea,
                New ItemTypeDescriptor("Dew a lit tea", AddressOf DoNothing)
            },
            {
                ItemType.DewPuddle,
                New ItemTypeDescriptor("Dew Puddle", AddressOf DoNothing, spawnCount:=100, canTake:=False)
            },
            {
                ItemType.Vörda,
                New ItemTypeDescriptor("Voerda", AddressOf DoNothing)
            },
            {
                ItemType.SyltLingon,
                New ItemTypeDescriptor("Sylt Lingon", AddressOf DoNothing)
            }
        }

    Private Sub UseMatch(arg1 As WorldData, arg2 As ICharacter)
        Throw New NotImplementedException()
    End Sub

    Private Sub UseTea(data As WorldData, character As ICharacter)
    End Sub

    Private Sub UseWetSponge(data As WorldData, character As ICharacter)
    End Sub

    Private Sub UseSponge(data As WorldData, character As ICharacter)
    End Sub

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
