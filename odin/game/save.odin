package game

import "core:strconv"
import "core:strings"

// Saves are new in the port (the jam original had none): the whole World as one line of integers, written
// to localStorage every time play returns to Navigation or Combat, and cleared on death or victory.
// world_load checks every value before it accepts a save.

SAVE_MAGIC :: "KC1"

world_save :: proc(w: ^World) -> string {
	b := strings.builder_make(context.temp_allocator)
	strings.write_string(&b, SAVE_MAGIC)
	put :: proc(b: ^strings.Builder, v: int) {
		strings.write_byte(b, ' ')
		strings.write_int(b, v)
	}
	strings.write_byte(&b, ' ')
	strings.write_u64(&b, w.rng.state)
	put(&b, int(w.facing))
	put(&b, w.player)
	put(&b, w.next_order)
	for l in w.locations {
		bits := 0
		for open, d in l.door {
			if open {
				bits |= 1 << uint(d)
			}
		}
		put(&b, bits)
		put(&b, 1 if l.visited else 0)
	}
	put(&b, len(w.characters))
	for c in w.characters {
		put(&b, int(c.type))
		put(&b, c.location)
		for s in Statistic {
			put(&b, c.stats[s])
		}
	}
	put(&b, len(w.items))
	for item in w.items {
		put(&b, int(item.type))
		put(&b, item.location)
		put(&b, item.order)
	}
	return strings.to_string(b)
}

Save_Reader :: struct {
	rest: string,
	ok:   bool,
}

read_token :: proc(r: ^Save_Reader) -> string {
	r.rest = strings.trim_left_space(r.rest)
	end := strings.index_byte(r.rest, ' ')
	if end < 0 {
		end = len(r.rest)
	}
	token := r.rest[:end]
	r.rest = r.rest[end:]
	if len(token) == 0 {
		r.ok = false
	}
	return token
}

read_int :: proc(r: ^Save_Reader, lo, hi: int) -> int {
	v, parsed := strconv.parse_int(read_token(r), 10)
	if !parsed || v < lo || v > hi {
		r.ok = false
		return lo
	}
	return v
}

MAX_SAVED_THINGS :: 100_000
MAX_STAT :: 1_000_000

// Replaces *w with the saved world and returns true, or leaves *w untouched and returns false.
world_load :: proc(w: ^World, text: string) -> bool {
	r := Save_Reader{rest = text, ok = true}
	if read_token(&r) != SAVE_MAGIC || !r.ok {
		return false
	}
	loaded: World
	state, state_ok := strconv.parse_u64(read_token(&r), 10)
	loaded.rng.state = state
	r.ok &&= state_ok
	loaded.facing = Direction(read_int(&r, 0, 3))
	loaded.player = read_int(&r, 0, MAX_SAVED_THINGS)
	loaded.next_order = read_int(&r, 0, max(int))
	for &l in loaded.locations {
		bits := read_int(&r, 0, 15)
		for d in Direction {
			l.door[d] = bits & (1 << uint(d)) != 0
		}
		l.visited = read_int(&r, 0, 1) == 1
	}
	character_count := read_int(&r, 1, MAX_SAVED_THINGS)
	for _ in 0 ..< character_count {
		if !r.ok {
			break
		}
		c: Character
		c.type = Character_Type(read_int(&r, 0, len(Character_Type) - 1))
		c.location = read_int(&r, 0, LOCATION_COUNT - 1)
		for s in Statistic {
			c.stats[s] = read_int(&r, -MAX_STAT, MAX_STAT)
		}
		append(&loaded.characters, c)
	}
	item_count := read_int(&r, 0, MAX_SAVED_THINGS)
	for _ in 0 ..< item_count {
		if !r.ok {
			break
		}
		item: Item
		item.type = Item_Type(read_int(&r, 0, len(Item_Type) - 1))
		item.location = read_int(&r, INVENTORY, LOCATION_COUNT - 1)
		item.order = read_int(&r, 0, max(int))
		append(&loaded.items, item)
	}
	r.ok &&= len(strings.trim_space(r.rest)) == 0
	r.ok &&= loaded.player < len(loaded.characters) && loaded.characters[loaded.player].type == .Larrikin
	// doors come in pairs and never lead out of the maze
	for l, i in loaded.locations {
		for open, d in l.door {
			if !open {
				continue
			}
			n := location_neighbor(i, d)
			r.ok &&= n != NONE && loaded.locations[n].door[OPPOSITE[d]]
		}
	}
	if !r.ok {
		world_destroy(&loaded)
		return false
	}
	world_destroy(w)
	w^ = loaded
	return true
}
