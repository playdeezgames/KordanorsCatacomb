Imports System.Runtime.CompilerServices

Friend Module ItemTypeEggstensions
    Private Function DoNothing(data As WorldData, character As ICharacter) As Boolean
        Return False
    End Function
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
                New ItemTypeDescriptor("Sponge", AddressOf UseSponge, spawnCount:=1, isUsable:=True)
            },
            {
                ItemType.WetSponge,
                New ItemTypeDescriptor("Wet Sponge", AddressOf UseWetSponge, isUsable:=True)
            },
            {
                ItemType.Tea,
                New ItemTypeDescriptor("Tea", AddressOf DoNothing, spawnCount:=1)
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
                New ItemTypeDescriptor("Dew Puddle", AddressOf DoNothing, spawnCount:=1, canTake:=False)
            },
            {
                ItemType.Vörda,
                New ItemTypeDescriptor("Voerda", AddressOf DoNothing)
            },
            {
                ItemType.SyltLingon,
                New ItemTypeDescriptor("Sylt Lingon", AddressOf UseSyltLingon, spawnCount:=5, isUsable:=True)
            },
            {
                ItemType.Compass,
                New ItemTypeDescriptor("Compass", AddressOf DoNothing, spawnCount:=5)
            }
        }

    Private Function UseSyltLingon(data As WorldData, character As ICharacter) As Boolean
        Dim world = New World(data)
        character.MaximumHP += 1
        world.AddMessage((Mood.Gray, "It is delicious!"))
        Return True
    End Function

    Private Function UseMatch(data As WorldData, character As ICharacter) As Boolean
        Dim world = New World(data)
        If Not character.Inventory.ItemTypes.Contains(ItemType.Tea) Then
            world.AddMessage((Mood.Gray, "You cannot use that now."))
            Return False
        End If
        Dim teaItem = character.Inventory.Items.First(Function(x) x.ItemType = ItemType.Tea)
        teaItem.Location = Nothing
        Item.Create(data, ItemType.LitTea, character.Inventory)
        Item.Create(data, ItemType.BurntMatch, character.Inventory)
        world.AddMessage((Mood.Gray, "You use the match on the  "), (Mood.Gray, "tea. It is now a Lit Tee."))
        Return True
    End Function

    Private Function UseWetSponge(data As WorldData, character As ICharacter) As Boolean
        Dim world = New World(data)
        If Not character.Inventory.ItemTypes.Contains(ItemType.LitTea) Then
            world.AddMessage((Mood.Gray, "You cannot use that now."))
            Return False
        End If
        Dim teaItem = character.Inventory.Items.First(Function(x) x.ItemType = ItemType.LitTea)
        teaItem.Location = Nothing
        Item.Create(data, ItemType.DewALitTea, character.Inventory)
        Item.Create(data, ItemType.Sponge, character.Inventory)
        world.AddMessage((Mood.Gray, "You squeeze the dew onto  "), (Mood.Gray, "the Lit Tea. You have made"), (Mood.Gray, "Dew a Lit Tea!"))
        Return True
    End Function

    Private Function UseSponge(data As WorldData, character As ICharacter) As Boolean
        Dim world = New World(data)
        If Not character.Location.ItemTypes.Contains(ItemType.DewPuddle) Then
            world.AddMessage((Mood.Gray, "You cannot use that now."))
            Return False
        End If
        Dim puddle = character.Location.Items.First(Function(x) x.ItemType = ItemType.DewPuddle)
        puddle.Location = Nothing
        Item.Create(data, ItemType.WetSponge, character.Inventory)
        World.AddMessage((Mood.Gray, "You sop up the dew."))
        Return True
    End Function

    Private Function EatKoetbulle(data As WorldData, character As ICharacter) As Boolean
        character.AddWounds(-1)
        Dim msg = Message.Create(data)
        msg.AddLine(Mood.Gray, "You eat it.")
        Return True
    End Function

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
