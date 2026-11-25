package game

// The original's descriptor tables (CharacterTypeEggstensions, ItemTypeEggstensions), as enumerated arrays.

Character_Descriptor :: struct {
	name:               string,
	stats:              [Statistic]int,
	minimum_exit_count: int,
	maximum_exit_count: int,
	spawn_count:        int,
	is_enemy:           bool,
}

CHARACTER_DESCRIPTORS := [Character_Type]Character_Descriptor {
	.Larrikin = {
		name = "Larrikin",
		stats = {.Wounds = 0, .Maximum_HP = 3, .Attack = 3, .Maximum_Attack = 1, .Defend = 4, .Maximum_Defend = 2},
		minimum_exit_count = 0,
		maximum_exit_count = 4,
		spawn_count = 0,
		is_enemy = false,
	},
	.Blob = {
		name = "Blob",
		stats = {.Wounds = 0, .Maximum_HP = 1, .Attack = 2, .Maximum_Attack = 1, .Defend = 1, .Maximum_Defend = 1},
		minimum_exit_count = 0,
		maximum_exit_count = 4,
		spawn_count = 100,
		is_enemy = true,
	},
}

Item_Descriptor :: struct {
	name:               string,
	spawn_count:        int,
	minimum_exit_count: int,
	maximum_exit_count: int,
	is_usable:          bool,
	can_take:           bool,
}

// Spawn order follows the original table's order (Knorva first), which is also the order its keys enumerate.
ITEM_SPAWN_ORDER := [?]Item_Type{.Knorva, .Koettbulle, .Sponge, .Wet_Sponge, .Tea, .Match, .Burnt_Match, .Lit_Tea, .Dew_A_Lit_Tea, .Dew_Puddle, .Voerda, .Sylt_Lingon, .Compass}

ITEM_DESCRIPTORS := [Item_Type]Item_Descriptor {
	.Knorva        = {name = "Knorva", spawn_count = 3, maximum_exit_count = 4, is_usable = true, can_take = true},
	.Koettbulle    = {name = "Koettbulle", spawn_count = 100, maximum_exit_count = 4, is_usable = true, can_take = true},
	.Sponge        = {name = "Sponge", spawn_count = 1, maximum_exit_count = 4, is_usable = true, can_take = true},
	.Wet_Sponge    = {name = "Wet Sponge", maximum_exit_count = 4, is_usable = true, can_take = true},
	.Tea           = {name = "Tea", spawn_count = 1, maximum_exit_count = 4, can_take = true},
	.Match         = {name = "Match", spawn_count = 1, maximum_exit_count = 4, is_usable = true, can_take = true},
	.Burnt_Match   = {name = "Burnt Match", maximum_exit_count = 4, can_take = true},
	.Lit_Tea       = {name = "Lit Tea", maximum_exit_count = 4, can_take = true},
	.Dew_A_Lit_Tea = {name = "Dew a lit tea", maximum_exit_count = 4, can_take = true},
	.Dew_Puddle    = {name = "Dew Puddle", spawn_count = 1, maximum_exit_count = 4, can_take = false},
	.Voerda        = {name = "Voerda", spawn_count = 4, maximum_exit_count = 4, is_usable = true, can_take = true},
	.Sylt_Lingon   = {name = "Sylt Lingon", spawn_count = 5, maximum_exit_count = 4, is_usable = true, can_take = true},
	.Compass       = {name = "Compass", spawn_count = 5, maximum_exit_count = 4, can_take = true},
}
