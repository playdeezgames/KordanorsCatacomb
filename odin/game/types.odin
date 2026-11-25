package game

// The world, as the original's KC.Data DTOs: flat arrays addressed by index. A 9 by 9 maze of locations,
// characters (the Larrikin and 100 Blobs) and items. Items live in a dungeon location, in the player's
// inventory, or nowhere (used up).

MAZE_COLUMNS :: 9
MAZE_ROWS :: 9
LOCATION_COUNT :: MAZE_COLUMNS * MAZE_ROWS

NONE :: -1
INVENTORY :: -2 // item location: the player's inventory (the original's lazily created inventory location)

Direction :: enum u8 {
	North,
	East,
	South,
	West,
}

DIRECTION_NAMES := [Direction]string {
	.North = "North",
	.East  = "East",
	.South = "South",
	.West  = "West",
}

DIRECTION_DELTA := [Direction][2]int {
	.North = {0, -1},
	.East  = {1, 0},
	.South = {0, 1},
	.West  = {-1, 0},
}

OPPOSITE := [Direction]Direction {
	.North = .South,
	.East  = .West,
	.South = .North,
	.West  = .East,
}

LEFT_OF := [Direction]Direction {
	.North = .West,
	.East  = .North,
	.South = .East,
	.West  = .South,
}

RIGHT_OF := [Direction]Direction {
	.North = .East,
	.East  = .South,
	.South = .West,
	.West  = .North,
}

Character_Type :: enum u8 {
	Larrikin,
	Blob,
}

Statistic :: enum u8 {
	Wounds,
	Maximum_HP,
	Attack,
	Maximum_Attack,
	Defend,
	Maximum_Defend,
}

Item_Type :: enum u8 {
	Koettbulle,
	Knorva,
	Tea,
	Match,
	Sponge,
	Wet_Sponge,
	Lit_Tea,
	Dew_A_Lit_Tea,
	Dew_Puddle,
	Burnt_Match,
	Voerda,
	Sylt_Lingon,
	Compass,
}

Sound_Cue :: enum u8 {
	None,
	Miss,
	Player_Hit,
	Enemy_Hit,
	Player_Death,
	Enemy_Death,
	Win,
}

Location :: struct {
	door:    [Direction]bool, // the border on that side is a door (an exit), else a wall
	visited: bool, // visited by the player (IsVisitedBy is only ever asked about the player)
}

Character :: struct {
	type:     Character_Type,
	location: int,
	stats:    [Statistic]int,
}

Item :: struct {
	type:     Item_Type,
	location: int, // 0..80 a dungeon location, INVENTORY, or NONE
	order:    int, // when it arrived where it is; lists show items in arrival order, like the original's HashSet
}

MAX_LINES :: 8

Message :: struct {
	cue:        Sound_Cue,
	line_count: int,
	lines:      [MAX_LINES]string, // heap allocated, owned by the message
}

World :: struct {
	rng:        Rng,
	locations:  [LOCATION_COUNT]Location,
	characters: [dynamic]Character,
	items:      [dynamic]Item,
	player:     int,
	facing:     Direction,
	messages:   [dynamic]Message,
	next_order: int,
}
