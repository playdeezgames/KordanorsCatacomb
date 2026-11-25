#+build !js
package game

import "core:fmt"
import "core:os"
import "core:strings"
import "core:testing"

// ---- the maze and generation ----

@(test)
maze_is_a_spanning_tree :: proc(t: ^testing.T) {
	for seed in u64(1) ..= 50 {
		w: World
		defer world_destroy(&w)
		world_generate(&w, seed)
		doors := 0
		for l in w.locations {
			for open in l.door {
				if open {doors += 1}
			}
		}
		testing.expect_value(t, doors, 2 * (LOCATION_COUNT - 1)) // a tree has one passage fewer than rooms
		reached: [LOCATION_COUNT]bool
		stack: [dynamic]int
		defer delete(stack)
		append(&stack, 0)
		reached[0] = true
		count := 1
		for len(stack) > 0 {
			here := pop(&stack)
			for d in Direction {
				if !w.locations[here].door[d] {continue}
				n := location_neighbor(here, d)
				testing.expect(t, n != NONE, "a door leads out of the maze")
				testing.expect(t, w.locations[n].door[OPPOSITE[d]], "a door is one-sided")
				if !reached[n] {
					reached[n] = true
					count += 1
					append(&stack, n)
				}
			}
		}
		testing.expect_value(t, count, LOCATION_COUNT)
	}
}

@(test)
generation_spawns_the_original_counts :: proc(t: ^testing.T) {
	w: World
	defer world_destroy(&w)
	world_generate(&w, 7)
	blobs := 0
	for c in w.characters {
		if c.type == .Blob {blobs += 1}
	}
	testing.expect_value(t, blobs, 100)
	testing.expect_value(t, w.characters[w.player].type, Character_Type.Larrikin)
	testing.expect_value(t, character_hp(w.characters[w.player]), 3)
	testing.expect(t, w.locations[player_location(&w)].visited)
	counts: [Item_Type]int
	for item in w.items {
		counts[item.type] += 1
		testing.expect(t, item.location >= 0 && item.location < LOCATION_COUNT)
	}
	for d, it in ITEM_DESCRIPTORS {
		testing.expect_value(t, counts[it], d.spawn_count)
	}
}

@(test)
sixes_never_exceed_the_maximum :: proc(t: ^testing.T) {
	w: World
	defer world_destroy(&w)
	rng_seed(&w.rng, 3)
	total := 0
	for _ in 0 ..< 10_000 {
		r := roll_sixes(&w, 4, 2)
		testing.expect(t, r >= 0 && r <= 2)
		total += r
	}
	// four dice that count on a six, capped at two: about 0.62 a roll
	testing.expect(t, total > 5000 && total < 7500, fmt.tprintf("total %d", total))
}

// ---- combat ----

@(test)
a_fight_is_answered_by_every_enemy_in_the_room :: proc(t: ^testing.T) {
	w: World
	defer world_destroy(&w)
	world_generate(&w, 11)
	here := player_location(&w)
	for &c in w.characters {
		if c.type == .Blob {c.location = (here + 1) % LOCATION_COUNT}
	}
	for i in 1 ..= 3 {
		w.characters[i].location = here
	}
	w.characters[w.player].stats[.Maximum_HP] = 1000 // survive the counterattacks
	before := len(characters_at(&w, here, true))
	testing.expect_value(t, before, 3)
	character_fight(&w, w.player)
	after := len(characters_at(&w, here, true))
	// the player's message, then one counterattack message per enemy still standing
	testing.expect_value(t, len(w.messages), 1 + after)
	for i in 1 ..< len(w.messages) {
		testing.expect_value(t, w.messages[i].lines[0], fmt.tprintf("Counterattack #%d", i))
	}
}

@(test)
koettbullar_bank_hit_points_past_full :: proc(t: ^testing.T) {
	w: World
	defer world_destroy(&w)
	world_generate(&w, 5)
	for _ in 0 ..< 2 {
		use_item(&w, item_create(&w, .Koettbulle, INVENTORY))
	}
	p := player(&w)
	testing.expect_value(t, p.stats[.Wounds], -2)
	testing.expect_value(t, character_hp(p^), 3) // still shows full
	p.stats[.Wounds] += 2
	testing.expect_value(t, character_hp(p^), 3) // two hits absorbed
}

@(test)
stat_items_raise_the_stats :: proc(t: ^testing.T) {
	w: World
	defer world_destroy(&w)
	world_generate(&w, 5)
	use_item(&w, item_create(&w, .Voerda, INVENTORY))
	use_item(&w, item_create(&w, .Knorva, INVENTORY))
	use_item(&w, item_create(&w, .Sylt_Lingon, INVENTORY))
	p := player(&w)^
	testing.expect_value(t, p.stats[.Maximum_Attack], 2)
	testing.expect_value(t, p.stats[.Attack], 6)
	testing.expect_value(t, p.stats[.Maximum_Defend], 3)
	testing.expect_value(t, p.stats[.Defend], 6)
	testing.expect_value(t, p.stats[.Maximum_HP], 4)
	testing.expect_value(t, len(items_at(&w, INVENTORY)), 0)
}

// ---- the quest ----

@(test)
the_quest_chain_makes_dew_a_lit_tea :: proc(t: ^testing.T) {
	w: World
	defer world_destroy(&w)
	world_generate(&w, 9)
	here := player_location(&w)
	match := item_create(&w, .Match, INVENTORY)
	sponge := item_create(&w, .Sponge, INVENTORY)
	use_item(&w, match) // no tea yet
	testing.expect_value(t, w.items[match].location, INVENTORY)
	use_item(&w, sponge) // no puddle here yet
	testing.expect_value(t, w.items[sponge].location, INVENTORY)
	item_create(&w, .Tea, INVENTORY)
	use_item(&w, match)
	testing.expect(t, has_item_type(&w, INVENTORY, .Lit_Tea))
	testing.expect(t, has_item_type(&w, INVENTORY, .Burnt_Match))
	testing.expect(t, !has_item_type(&w, INVENTORY, .Tea))
	item_create(&w, .Dew_Puddle, here)
	use_item(&w, sponge)
	testing.expect(t, !has_item_type(&w, here, .Dew_Puddle))
	wet := first_item_of_type(&w, INVENTORY, .Wet_Sponge)
	testing.expect(t, wet != NONE)
	use_item(&w, wet)
	testing.expect(t, has_item_type(&w, INVENTORY, .Dew_A_Lit_Tea))
	testing.expect(t, has_item_type(&w, INVENTORY, .Sponge))
	testing.expect_value(t, w.messages[len(w.messages) - 1].cue, Sound_Cue.Win)
}

// ---- saves ----

@(test)
saves_round_trip_and_reject_garbage :: proc(t: ^testing.T) {
	w: World
	defer world_destroy(&w)
	world_generate(&w, 21)
	world_move(&w)
	text := world_save(&w)
	loaded: World
	defer world_destroy(&loaded)
	testing.expect(t, world_load(&loaded, text))
	testing.expect_value(t, world_save(&loaded), text)
	testing.expect(t, !world_load(&loaded, ""))
	testing.expect(t, !world_load(&loaded, "KC1 1 2 3"))
	testing.expect(t, !world_load(&loaded, text[:strings.last_index_byte(text, ' ')])) // the last value missing
	testing.expect(t, !world_load(&loaded, fmt.tprintf("%s 5", text)))
	testing.expect_value(t, world_save(&loaded), text) // failed loads left it alone
}

// ---- a bot plays whole runs through the rules ----

Bot_Result :: enum {
	Won,
	Died,
	Stuck,
}

// Explores toward the nearest unvisited room (or a needed item), takes everything, uses stat items at
// once, eats when hurt, and makes the tea as soon as it can.
bot_play :: proc(w: ^World) -> (result: Bot_Result, steps: int) {
	for steps = 0; steps < 20_000; steps += 1 {
		for len(w.messages) > 0 {dismiss_message(w)}
		p := player(w)
		if character_is_dead(p^) {return .Died, steps}
		if has_item_type(w, INVENTORY, .Dew_A_Lit_Tea) {return .Won, steps}
		here := p.location
		if len(characters_at(w, here, true)) > 0 {
			if character_hp(p^) <= 1 {
				if k := first_item_of_type(w, INVENTORY, .Koettbulle); k != NONE {
					use_item(w, k)
					continue
				}
			}
			character_fight(w, w.player)
			continue
		}
		for id in items_at(w, here) {
			if ITEM_DESCRIPTORS[w.items[id].type].can_take {item_move(w, id, INVENTORY)}
		}
		used := false
		for id in items_at(w, INVENTORY) {
			#partial switch w.items[id].type {
			case .Voerda, .Knorva, .Sylt_Lingon:
				use_item(w, id);used = true
			case .Koettbulle:
				if character_hp(p^) < p.stats[.Maximum_HP] {use_item(w, id);used = true}
			case .Match:
				if has_item_type(w, INVENTORY, .Tea) {use_item(w, id);used = true}
			case .Sponge:
				if has_item_type(w, here, .Dew_Puddle) {use_item(w, id);used = true}
			case .Wet_Sponge:
				if has_item_type(w, INVENTORY, .Lit_Tea) {use_item(w, id);used = true}
			}
			if used {break}
		}
		if used {continue}
		// head for the nearest unvisited room, or else the puddle, by breadth-first search
		want_puddle := has_item_type(w, INVENTORY, .Sponge)
		goal :: proc(w: ^World, l: int, want_puddle: bool) -> bool {
			return !w.locations[l].visited || (want_puddle && has_item_type(w, l, .Dew_Puddle))
		}
		first_step: [LOCATION_COUNT]int
		seen: [LOCATION_COUNT]bool
		queue := make([dynamic]int, context.temp_allocator)
		append(&queue, here)
		seen[here] = true
		target := NONE
		for head := 0; head < len(queue) && target == NONE; head += 1 {
			l := queue[head]
			for d in Direction {
				if !w.locations[l].door[d] {continue}
				n := location_neighbor(l, d)
				if seen[n] {continue}
				seen[n] = true
				first_step[n] = int(d) if l == here else first_step[l]
				if goal(w, n, want_puddle) {target = n;break}
				append(&queue, n)
			}
		}
		if target == NONE {return .Stuck, steps}
		w.facing = Direction(first_step[target])
		world_move(w)
	}
	return .Stuck, steps
}

@(test)
the_bot_can_finish_and_never_gets_stuck :: proc(t: ^testing.T) {
	results: [Bot_Result]int
	for seed in u64(1) ..= 300 {
		w: World
		world_generate(&w, seed)
		r, steps := bot_play(&w)
		results[r] += 1
		if r == .Stuck && results[.Stuck] <= 3 {
			visited := 0
			for l in w.locations {if l.visited {visited += 1}}
			fmt.printfln("stuck seed %d after %d steps: visited %d, inventory %v, here %d", seed, steps, visited, item_types_at(&w, INVENTORY), player_location(&w))
			for item in w.items {if item.type == .Dew_Puddle || item.type == .Tea || item.type == .Match || item.type == .Sponge {fmt.printfln("  %v at %d", item.type, item.location)}}
		}
		world_destroy(&w)
		free_all(context.temp_allocator)
	}
	fmt.printfln("bot over 300 seeds: %v", results)
	testing.expect_value(t, results[.Stuck], 0)
	testing.expect(t, results[.Won] > 0, "the bot never won")
}

// ---- the front end ----

memory_store: map[string]string

test_get :: proc(key: string) -> (string, bool) {
	v, ok := memory_store[key]
	return v, ok
}

test_set :: proc(key: string, value: string) {
	if key in memory_store {
		old_key, old_value := delete_key(&memory_store, key)
		delete(old_key)
		delete(old_value)
	}
	memory_store[clone_string(key)] = clone_string(value)
}

clone_string :: proc(s: string) -> string {
	b := make([]u8, len(s))
	copy(b, s)
	return string(b)
}

clear_store :: proc() {
	for k, v in memory_store {
		delete(k)
		delete(v)
	}
	delete(memory_store)
	memory_store = nil
}

press :: proc(g: ^Game, c: Command) {
	game_command(g, c, true)
	game_command(g, c, false)
	game_frame(g, 1.0 / 60.0)
}

@(test)
splash_menu_prolog_and_into_the_catacomb :: proc(t: ^testing.T) {
	defer clear_store()
	g: Game
	defer game_destroy(&g)
	game_init(&g, 42, Storage{get = test_get, set = test_set})
	testing.expect_value(t, g.state, State.Splash)
	for _ in 0 ..< 200 {game_frame(&g, 1.0 / 60.0)}
	testing.expect_value(t, g.state, State.Main_Menu)
	testing.expect_value(t, len(main_items(&g)), 4) // no Continue yet
	press(&g, .A) // Embark!
	testing.expect_value(t, g.state, State.Prolog)
	press(&g, .A)
	testing.expect(t, g.state == .Navigation || g.state == .Combat || g.state == .Message)
	testing.expect(t, g.has_save)
	testing.expect_value(t, len(main_items(&g)), 5)
}

@(test)
continue_resumes_the_saved_world :: proc(t: ^testing.T) {
	defer clear_store()
	g: Game
	game_init(&g, 42, Storage{get = test_get, set = test_set})
	press(&g, .A) // skip the splash
	press(&g, .A) // Embark!
	press(&g, .A) // past the prolog
	saved := world_save(&g.world)
	saved = clone_string(saved)
	defer delete(saved)
	game_destroy(&g)

	g2: Game
	defer game_destroy(&g2)
	game_init(&g2, 99, Storage{get = test_get, set = test_set})
	testing.expect(t, g2.has_save)
	press(&g2, .A) // skip the splash
	testing.expect_value(t, main_items(&g2)[g2.main_menu_index], Main_Item.Continue)
	press(&g2, .A)
	testing.expect(t, g2.state == .Navigation || g2.state == .Combat)
	testing.expect_value(t, world_save(&g2.world), saved)
}

@(test)
volume_is_remembered :: proc(t: ^testing.T) {
	defer clear_store()
	g: Game
	defer game_destroy(&g)
	game_init(&g, 1, Storage{get = test_get, set = test_set})
	press(&g, .A) // splash
	press(&g, .Down)
	press(&g, .Down) // SFX Volume...
	press(&g, .A)
	testing.expect_value(t, g.state, State.SFX_Volume)
	press(&g, .Up)
	testing.expect_value(t, g.volume_tenths, 9)
	press(&g, .A)
	g2: Game
	defer game_destroy(&g2)
	game_init(&g2, 1, Storage{get = test_get, set = test_set})
	testing.expect_value(t, g2.volume_tenths, 9)
}

// ---- screenshots for the itch page: tools/make_shots.sh ----

DUMP_DIR :: #config(DUMP_DIR, "")

write_ppm :: proc(g: ^Game, name: string) {
	data := make([dynamic]u8, context.temp_allocator)
	header := fmt.tprintf("P6\n%d %d\n255\n", VIEW_WIDTH, VIEW_HEIGHT)
	append(&data, ..transmute([]u8)header)
	for p in g.pixels {
		append(&data, u8(p), u8(p >> 8), u8(p >> 16))
	}
	_ = os.write_entire_file(fmt.tprintf("%s/%s.ppm", DUMP_DIR, name), data[:])
}

@(test)
dump_screens :: proc(t: ^testing.T) {
	when DUMP_DIR == "" {
		return
	} else {
		defer clear_store()
		g: Game
		defer game_destroy(&g)
		game_init(&g, 2023, Storage{get = test_get, set = test_set})
		write_ppm(&g, "01-title")
		press(&g, .A)
		press(&g, .A) // Embark!
		write_ppm(&g, "02-prolog")
		press(&g, .A)
		w := &g.world
		// a staged room: one blob, a few items, doors left and ahead, the compass in hand
		here := player_location(w)
		for &c in w.characters {
			if c.type == .Blob && c.location == here {c.location = (here + 40) % LOCATION_COUNT}
		}
		for len(w.messages) > 0 {dismiss_message(w)}
		item_create(w, .Compass, INVENTORY)
		item_create(w, .Koettbulle, here)
		item_create(w, .Tea, here)
		item_create(w, .Sylt_Lingon, here)
		for d in Direction {
			if w.locations[here].door[d] {w.facing = d;break}
		}
		g.state = .Neutral
		game_frame(&g, 0)
		write_ppm(&g, "03-room")
		for &c, id in w.characters {
			if c.type == .Blob && id < 3 {c.location = here}
		}
		g.state = .Neutral
		game_frame(&g, 0)
		write_ppm(&g, "04-fight")
		press(&g, .A) // fight
		write_ppm(&g, "05-message")
		for g.state == .Message {press(&g, .A)}
		for &c in w.characters {
			if c.type == .Blob && c.location == here {c.location = (here + 40) % LOCATION_COUNT}
		}
		for l in 0 ..< LOCATION_COUNT / 2 {w.locations[l].visited = true}
		g.state = .Mode_Select
		g.mode_index = 3
		game_frame(&g, 0)
		write_ppm(&g, "06-map")
	}
}
