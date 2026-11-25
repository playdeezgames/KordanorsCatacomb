package game

import "core:fmt"

// The rules: the original's KC.Business (World, Location, Character, Item, the item use procedures) and
// SPLORR.Game.Maze. Messages are queued on the world; the front end shows them one at a time.

world_destroy :: proc(w: ^World) {
	for &m in w.messages {
		message_free(&m)
	}
	delete(w.messages)
	delete(w.characters)
	delete(w.items)
	w^ = {}
}

location_index :: proc(column, row: int) -> int {
	return column + row * MAZE_COLUMNS
}

location_column_row :: proc(location: int) -> (int, int) {
	return location % MAZE_COLUMNS, location / MAZE_COLUMNS
}

// The neighbouring location in that direction, or NONE at the edge of the maze.
location_neighbor :: proc(location: int, d: Direction) -> int {
	column, row := location_column_row(location)
	column += DIRECTION_DELTA[d].x
	row += DIRECTION_DELTA[d].y
	if column < 0 || row < 0 || column >= MAZE_COLUMNS || row >= MAZE_ROWS {
		return NONE
	}
	return location_index(column, row)
}

location_exit_count :: proc(w: ^World, location: int) -> int {
	n := 0
	for open in w.locations[location].door {
		if open {
			n += 1
		}
	}
	return n
}

// ---- generation (World.Generate) ----

world_generate :: proc(w: ^World, seed: u64) {
	world_destroy(w)
	rng_seed(&w.rng, seed)
	w.player = NONE
	generate_maze(w)
	for t in ITEM_SPAWN_ORDER {
		for _ in 0 ..< ITEM_DESCRIPTORS[t].spawn_count {
			d := ITEM_DESCRIPTORS[t]
			item_create(w, t, random_location(w, d.minimum_exit_count, d.maximum_exit_count))
		}
	}
	for t in Character_Type {
		for _ in 0 ..< CHARACTER_DESCRIPTORS[t].spawn_count {
			generate_character(w, t)
		}
	}
	w.player = generate_character(w, .Larrikin)
	w.locations[w.characters[w.player].location].visited = true
	randomize_facing(w)
}

// A random location whose exit count is in range (the original retries until one fits).
random_location :: proc(w: ^World, minimum_exits, maximum_exits: int) -> int {
	for {
		column := rng_range(&w.rng, 0, MAZE_COLUMNS - 1)
		row := rng_range(&w.rng, 0, MAZE_ROWS - 1)
		location := location_index(column, row)
		exits := location_exit_count(w, location)
		if exits >= minimum_exits && exits <= maximum_exits {
			return location
		}
	}
}

generate_character :: proc(w: ^World, t: Character_Type) -> int {
	d := CHARACTER_DESCRIPTORS[t]
	append(&w.characters, Character{type = t, location = random_location(w, d.minimum_exit_count, d.maximum_exit_count), stats = d.stats})
	return len(w.characters) - 1
}

randomize_facing :: proc(w: ^World) {
	w.facing = Direction(rng_range(&w.rng, 0, 3))
}

// Maze.Generate: a randomised Prim's maze. Every cell starts walled in; a random cell is "inside", and
// frontier cells are joined to a random inside neighbour one at a time.
generate_maze :: proc(w: ^World) {
	inside: [LOCATION_COUNT]bool
	in_frontier: [LOCATION_COUNT]bool
	frontier := make([dynamic]int, context.temp_allocator)
	add_neighbors :: proc(cell: int, inside, in_frontier: ^[LOCATION_COUNT]bool, frontier: ^[dynamic]int) {
		for d in Direction {
			n := location_neighbor(cell, d)
			if n != NONE && !inside[n] && !in_frontier[n] {
				in_frontier[n] = true
				append(frontier, n)
			}
		}
	}
	start := location_index(rng_range(&w.rng, 0, MAZE_COLUMNS - 1), rng_range(&w.rng, 0, MAZE_ROWS - 1))
	inside[start] = true
	add_neighbors(start, &inside, &in_frontier, &frontier)
	for len(frontier) > 0 {
		pick := rng_range(&w.rng, 0, len(frontier) - 1)
		cell := frontier[pick]
		choices: [4]Direction
		count := 0
		for d in Direction {
			n := location_neighbor(cell, d)
			if n != NONE && inside[n] {
				choices[count] = d
				count += 1
			}
		}
		d := choices[rng_range(&w.rng, 0, count - 1)]
		w.locations[cell].door[d] = true
		w.locations[location_neighbor(cell, d)].door[OPPOSITE[d]] = true
		inside[cell] = true
		ordered_remove(&frontier, pick)
		in_frontier[cell] = false
		add_neighbors(cell, &inside, &in_frontier, &frontier)
	}
}

// ---- items ----

item_create :: proc(w: ^World, t: Item_Type, location: int) -> int {
	append(&w.items, Item{type = t, location = NONE})
	id := len(w.items) - 1
	item_move(w, id, location)
	return id
}

item_move :: proc(w: ^World, id: int, location: int) {
	w.items[id].location = location
	w.items[id].order = w.next_order
	w.next_order += 1
}

// The items at a location (or INVENTORY), in arrival order. Allocated in the temp allocator.
items_at :: proc(w: ^World, location: int) -> []int {
	result := make([dynamic]int, context.temp_allocator)
	for item, id in w.items {
		if item.location == location {
			append(&result, id)
		}
	}
	// insertion sort by arrival order (orders are unique; lists are short)
	ids := result[:]
	for i in 1 ..< len(ids) {
		for j := i; j > 0 && w.items[ids[j]].order < w.items[ids[j - 1]].order; j -= 1 {
			ids[j], ids[j - 1] = ids[j - 1], ids[j]
		}
	}
	return ids
}

usable_items_at :: proc(w: ^World, location: int) -> []int {
	result := make([dynamic]int, context.temp_allocator)
	for id in items_at(w, location) {
		if ITEM_DESCRIPTORS[w.items[id].type].is_usable {
			append(&result, id)
		}
	}
	return result[:]
}

// The distinct item types at a location, in order of first arrival.
item_types_at :: proc(w: ^World, location: int) -> []Item_Type {
	seen: [Item_Type]bool
	result := make([dynamic]Item_Type, context.temp_allocator)
	for id in items_at(w, location) {
		t := w.items[id].type
		if !seen[t] {
			seen[t] = true
			append(&result, t)
		}
	}
	return result[:]
}

has_item_type :: proc(w: ^World, location: int, t: Item_Type) -> bool {
	for item in w.items {
		if item.location == location && item.type == t {
			return true
		}
	}
	return false
}

first_item_of_type :: proc(w: ^World, location: int, t: Item_Type) -> int {
	for id in items_at(w, location) {
		if w.items[id].type == t {
			return id
		}
	}
	return NONE
}

// ---- characters ----

character_hp :: proc(c: Character) -> int {
	return clamp(c.stats[.Maximum_HP] - c.stats[.Wounds], 0, c.stats[.Maximum_HP])
}

character_is_dead :: proc(c: Character) -> bool {
	return character_hp(c) == 0
}

character_is_enemy :: proc(c: Character) -> bool {
	return CHARACTER_DESCRIPTORS[c.type].is_enemy
}

character_name :: proc(c: Character) -> string {
	return CHARACTER_DESCRIPTORS[c.type].name
}

// Living enemies (or living allies) at a location, in id order. Allocated in the temp allocator.
characters_at :: proc(w: ^World, location: int, enemies: bool) -> []int {
	result := make([dynamic]int, context.temp_allocator)
	for c, id in w.characters {
		if c.location == location && !character_is_dead(c) && character_is_enemy(c) == enemies {
			append(&result, id)
		}
	}
	return result[:]
}

player :: proc(w: ^World) -> ^Character {
	return &w.characters[w.player]
}

player_location :: proc(w: ^World) -> int {
	return w.characters[w.player].location
}

// Dice that only count on a six: each die adds 1 on a six, and rolling stops once the maximum is reached.
roll_sixes :: proc(w: ^World, dice, maximum: int) -> int {
	dice := dice
	roll := 0
	for dice > 0 && roll < maximum {
		roll += rng_range(&w.rng, 1, 6) / 6
		dice -= 1
	}
	return roll
}

roll_attack :: proc(w: ^World, id: int) -> int {
	c := w.characters[id]
	return roll_sixes(w, c.stats[.Attack], c.stats[.Maximum_Attack])
}

roll_defend :: proc(w: ^World, id: int) -> int {
	c := w.characters[id]
	return roll_sixes(w, c.stats[.Defend], c.stats[.Maximum_Defend])
}

// Character.Fight. index is the counterattack number (0 when this is not a counterattack). The player's
// attack is followed by a counterattack from every living enemy in the room.
character_fight :: proc(w: ^World, id: int, index := 0) {
	counter := index > 0
	hit_cue: Sound_Cue = .Player_Hit if counter else .Enemy_Hit
	death_cue: Sound_Cue = .Player_Death if counter else .Enemy_Death
	if character_is_dead(w.characters[id]) {
		return
	}
	location := w.characters[id].location
	is_enemy := character_is_enemy(w.characters[id])
	targets := characters_at(w, location, !is_enemy)
	if len(targets) == 0 {
		return
	}
	target := targets[rng_range(&w.rng, 0, len(targets) - 1)]
	if character_is_dead(w.characters[target]) {
		return
	}
	name := character_name(w.characters[id])
	target_name := character_name(w.characters[target])
	m := message_new(w)
	if counter {
		message_add(w, m, "Counterattack #%d", index)
	}
	message_add(w, m, "%s attacks %s!", name, target_name)
	attack := roll_attack(w, id)
	message_add(w, m, "%s rolls %d", name, attack)
	defend := roll_defend(w, target)
	message_add(w, m, "%s rolls %d", target_name, defend)
	damage := clamp(attack - defend, 0, attack)
	if damage > 0 {
		w.messages[m].cue = hit_cue
		message_add(w, m, "%s takes %d damage!", target_name, damage)
		w.characters[target].stats[.Wounds] += damage
		if character_is_dead(w.characters[target]) {
			w.messages[m].cue = death_cue
			message_add(w, m, "%s kills %s!", name, target_name)
		}
	} else {
		w.messages[m].cue = .Miss
		message_add(w, m, "%s misses!", name)
	}
	if !is_enemy {
		n := 1
		for enemy in characters_at(w, location, true) {
			character_fight(w, enemy, n)
			n += 1
		}
	}
}

// ---- moving ----

can_move :: proc(w: ^World) -> bool {
	return w.locations[player_location(w)].door[w.facing]
}

player_move_to :: proc(w: ^World, location: int) {
	w.characters[w.player].location = location
	w.locations[location].visited = true
}

world_move :: proc(w: ^World) {
	if can_move(w) {
		player_move_to(w, location_neighbor(player_location(w), w.facing))
	}
}

world_turn_left :: proc(w: ^World) {
	w.facing = LEFT_OF[w.facing]
}

world_turn_right :: proc(w: ^World) {
	w.facing = RIGHT_OF[w.facing]
}

// World.Run: face a random way; go if there is a door, otherwise every enemy gets a free attack.
world_run :: proc(w: ^World) {
	randomize_facing(w)
	name := character_name(player(w)^)
	if can_move(w) {
		world_move(w)
		m := message_new(w)
		message_add(w, m, "%s runs!", name)
		return
	}
	m := message_new(w)
	message_add(w, m, "%s cannot run!", name)
	for enemy in characters_at(w, player_location(w), true) {
		character_fight(w, enemy)
	}
}

// ---- using items (Character.UseItem and the OnUse procedures) ----

use_item :: proc(w: ^World, item: int) {
	if !ITEM_DESCRIPTORS[w.items[item].type].is_usable {
		add_message(w, .None, "You cannot use that.")
		return
	}
	if item_on_use(w, item) {
		item_move(w, item, NONE)
	}
}

item_on_use :: proc(w: ^World, item: int) -> bool {
	p := player(w)
	switch w.items[item].type {
	case .Voerda:
		p.stats[.Maximum_Attack] += 1
		p.stats[.Attack] += 3
		add_message(w, .None, "Sharp knife!")
		return true
	case .Knorva:
		p.stats[.Maximum_Defend] += 1
		p.stats[.Defend] += 2
		add_message(w, .None, "Best hat ever!")
		return true
	case .Sylt_Lingon:
		p.stats[.Maximum_HP] += 1
		add_message(w, .None, "It is delicious!")
		return true
	case .Koettbulle:
		// Healing lowers wounds without a floor, so eating at full health banks hit points (as in the original).
		p.stats[.Wounds] -= 1
		add_message(w, .None, "You eat it.")
		return true
	case .Match:
		tea := first_item_of_type(w, INVENTORY, .Tea)
		if tea == NONE {
			add_message(w, .None, "You cannot use that now.")
			return false
		}
		item_move(w, tea, NONE)
		item_create(w, .Lit_Tea, INVENTORY)
		item_create(w, .Burnt_Match, INVENTORY)
		add_message(w, .None, "You use the match on the  ", "tea. It is now a Lit Tee.")
		return true
	case .Wet_Sponge:
		tea := first_item_of_type(w, INVENTORY, .Lit_Tea)
		if tea == NONE {
			add_message(w, .None, "You cannot use that now.")
			return false
		}
		item_move(w, tea, NONE)
		item_create(w, .Dew_A_Lit_Tea, INVENTORY)
		item_create(w, .Sponge, INVENTORY)
		add_message(w, .Win, "You squeeze the dew onto  ", "the Lit Tea. You have made", "Dew a Lit Tea!")
		return true
	case .Sponge:
		puddle := first_item_of_type(w, player_location(w), .Dew_Puddle)
		if puddle == NONE {
			add_message(w, .None, "You cannot use that now.")
			return false
		}
		item_move(w, puddle, NONE)
		item_create(w, .Wet_Sponge, INVENTORY)
		add_message(w, .None, "You sop up the dew.")
		return true
	case .Tea, .Burnt_Match, .Lit_Tea, .Dew_A_Lit_Tea, .Dew_Puddle, .Compass:
		return false
	}
	return false
}

// ---- messages ----

message_new :: proc(w: ^World) -> int {
	append(&w.messages, Message{})
	return len(w.messages) - 1
}

message_add :: proc(w: ^World, m: int, format: string, args: ..any) {
	msg := &w.messages[m]
	if msg.line_count >= MAX_LINES {
		return
	}
	msg.lines[msg.line_count] = fmt.aprintf(format, ..args)
	msg.line_count += 1
}

add_message :: proc(w: ^World, cue: Sound_Cue, lines: ..string) {
	m := message_new(w)
	w.messages[m].cue = cue
	for line in lines {
		message_add(w, m, "%s", line)
	}
}

message_free :: proc(m: ^Message) {
	for i in 0 ..< m.line_count {
		delete(m.lines[i])
	}
	m.line_count = 0
}

dismiss_message :: proc(w: ^World) {
	if len(w.messages) == 0 {
		return
	}
	message_free(&w.messages[0])
	ordered_remove(&w.messages, 0)
}
