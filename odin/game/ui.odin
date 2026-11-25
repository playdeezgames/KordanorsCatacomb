package game

import "core:fmt"

// The front end: the original's GameController and every KC.Application game state, as one State enum
// and a few switch procedures. Time only comes in through game_frame(dt) and storage through the Storage
// hooks, so all of it runs natively under test. As in the original's Host, a key press is handled first,
// then the state updates (Neutral decides what comes next), then the state draws.

Command :: enum u8 {
	Up,
	Right,
	Down,
	Left,
	A, // Space or Enter (the original's Green and Blue)
	B, // Escape (the original's Red)
}

State :: enum u8 {
	Splash,
	Main_Menu,
	SFX_Volume,
	About,
	Prolog,
	Navigation,
	Neutral,
	Combat,
	Message,
	Game_Over,
	Mode_Select,
	Ground,
	Combat_Inventory,
	Inventory,
	Inventory_Detail,
	Win,
}

// Order must match SOUND_FILES in web/index.html.
Sfx :: enum u8 {
	None,
	Enemy_Death,
	Enemy_Hit,
	Level_Up,
	Miss,
	Player_Death,
	Player_Hit,
}

Storage :: struct {
	get: proc(key: string) -> (string, bool), // the returned string may live in the temp allocator
	set: proc(key: string, value: string),
}

SAVE_KEY :: "kc:save"
VOLUME_KEY :: "kc:volume"
SPLASH_SECONDS :: 3.0

Main_Item :: enum u8 {
	Continue,
	Embark,
	Full_Screen,
	SFX_Volume,
	About,
}

MAIN_ITEM_TEXT := [Main_Item]string {
	.Continue    = "Continue",
	.Embark      = "Embark!",
	.Full_Screen = "Full Screen",
	.SFX_Volume  = "SFX Volume...",
	.About       = "About...",
}

MODE_TEXT := [4]string{"<- Navigate ->", "<- Ground ->", "<- Inventory ->", "<- Map ->"}
COMBAT_TEXT := [3]string{"<- FIGHT! ->", "<- USE! ->", "<- RUN! ->"}
DETAIL_TEXT := [3]string{"Go Back", "Use", "Drop"}

Game :: struct {
	world:              World,
	has_world:          bool,
	state:              State,
	storage:            Storage,
	rng:                Rng, // seeds new worlds and picks the splash colours; the world has its own
	canvas:             Canvas,
	pixels:             [VIEW_WIDTH * VIEW_HEIGHT]u32,
	splash_timer:       f64,
	volume_tenths:      int,
	inventory_index:    int,
	main_menu_index:    int,
	mode_index:         int,
	combat_index:       int,
	detail_index:       int,
	has_save:           bool,
	sfx:                [16]Sfx,
	sfx_count:          int,
	fullscreen_request: bool,
}

game_init :: proc(g: ^Game, seed: u64, storage: Storage) {
	g^ = {}
	g.storage = storage
	rng_seed(&g.rng, seed)
	g.volume_tenths = 10
	if storage.get != nil {
		if text, ok := storage.get(VOLUME_KEY); ok {
			v := 0
			valid := len(text) > 0 && len(text) <= 2
			for ch in transmute([]u8)text {
				if ch < '0' || ch > '9' {
					valid = false
				}
				v = v * 10 + int(ch - '0')
			}
			if valid && v <= 10 {
				g.volume_tenths = v
			}
		}
		if text, ok := storage.get(SAVE_KEY); ok {
			probe: World
			g.has_save = world_load(&probe, text)
			world_destroy(&probe)
		}
	}
	g.state = .Splash
	g.splash_timer = SPLASH_SECONDS
	game_render(g)
}

game_destroy :: proc(g: ^Game) {
	world_destroy(&g.world)
}

volume :: proc(g: ^Game) -> f32 {
	return f32(g.volume_tenths) / 10.0
}

play :: proc(g: ^Game, s: Sfx) {
	if s == .None || g.sfx_count >= len(g.sfx) {
		return
	}
	g.sfx[g.sfx_count] = s
	g.sfx_count += 1
}

main_items :: proc(g: ^Game) -> []Main_Item {
	@(static) with_continue := [?]Main_Item{.Continue, .Embark, .Full_Screen, .SFX_Volume, .About}
	@(static) without := [?]Main_Item{.Embark, .Full_Screen, .SFX_Volume, .About}
	return with_continue[:] if g.has_save else without[:]
}

// ---- the frame ----

game_frame :: proc(g: ^Game, dt: f64) {
	game_update(g, dt)
	game_render(g)
	free_all(context.temp_allocator)
}

game_command :: proc(g: ^Game, command: Command, down: bool) {
	if !down {
		return
	}
	handle_command(g, command)
	// Resolve Neutral at once, so a second key before the next frame is not swallowed by it.
	if g.state == .Neutral {
		game_update(g, 0)
	}
}

wrap :: proc(value, count: int) -> int {
	if count <= 0 {
		return 0
	}
	return ((value % count) + count) % count
}

handle_command :: proc(g: ^Game, command: Command) {
	w := &g.world
	switch g.state {
	case .Splash:
		g.state = .Main_Menu
	case .Main_Menu:
		items := main_items(g)
		g.main_menu_index = wrap(g.main_menu_index, len(items))
		#partial switch command {
		case .Up:
			g.main_menu_index = wrap(g.main_menu_index - 1, len(items))
		case .Down:
			g.main_menu_index = wrap(g.main_menu_index + 1, len(items))
		case .A:
			switch items[g.main_menu_index] {
			case .Continue:
				continue_game(g)
			case .Embark:
				embark(g)
				g.state = .Prolog
			case .Full_Screen:
				g.fullscreen_request = true
			case .SFX_Volume:
				g.state = .SFX_Volume
			case .About:
				g.state = .About
			}
		}
	case .SFX_Volume:
		#partial switch command {
		case .Up:
			g.volume_tenths = wrap(g.volume_tenths - 1, 11)
			play(g, .Player_Hit)
		case .Down:
			g.volume_tenths = wrap(g.volume_tenths + 1, 11)
			play(g, .Player_Hit)
		case .A, .B:
			if g.storage.set != nil {
				g.storage.set(VOLUME_KEY, fmt.tprintf("%d", g.volume_tenths))
			}
			g.state = .Main_Menu
		}
	case .About:
		g.state = .Main_Menu
	case .Prolog:
		g.state = .Neutral
	case .Navigation:
		#partial switch command {
		case .Left:
			world_turn_left(w)
		case .Right:
			world_turn_right(w)
		case .Up:
			world_move(w)
			g.state = .Neutral
		case .A:
			g.state = .Mode_Select
		}
	case .Neutral:
	case .Combat:
		#partial switch command {
		case .Right:
			g.combat_index = wrap(g.combat_index + 1, len(COMBAT_TEXT))
		case .Left:
			g.combat_index = wrap(g.combat_index - 1, len(COMBAT_TEXT))
		case .A:
			switch g.combat_index {
			case 0:
				character_fight(w, w.player)
				g.state = .Neutral
			case 1:
				if len(usable_items_at(w, INVENTORY)) == 0 {
					add_message(w, .None, "You have no items.")
					g.state = .Neutral
				} else {
					g.inventory_index = 0
					g.state = .Combat_Inventory
				}
			case 2:
				world_run(w)
				g.state = .Neutral
			}
		}
	case .Message:
		if command == .A {
			dismiss_message(w)
			g.state = .Neutral
		}
	case .Game_Over, .Win:
		if command == .A {
			g.state = .Main_Menu
		}
	case .Mode_Select:
		#partial switch command {
		case .Right:
			g.mode_index = wrap(g.mode_index + 1, len(MODE_TEXT))
		case .Left:
			g.mode_index = wrap(g.mode_index - 1, len(MODE_TEXT))
		case .A:
			switch g.mode_index {
			case 0:
				g.state = .Neutral
			case 1:
				if len(items_at(w, player_location(w))) == 0 {
					g.mode_index = 0
					add_message(w, .None, "There are no items", "on the ground!")
					g.state = .Neutral
				} else {
					g.inventory_index = 0
					g.state = .Ground
				}
			case 2:
				if len(items_at(w, INVENTORY)) == 0 {
					g.mode_index = 0
					add_message(w, .None, "You have no items.")
					g.state = .Neutral
				} else {
					g.inventory_index = 0
					g.state = .Inventory
				}
			case 3:
			// the map is shown while this is selected; choosing it does nothing
			}
		case .B:
			g.mode_index = 0
			g.state = .Neutral
		}
	case .Ground:
		items := items_at(w, player_location(w))
		#partial switch command {
		case .Up:
			g.inventory_index = wrap(g.inventory_index - 1, len(items))
		case .Down:
			g.inventory_index = wrap(g.inventory_index + 1, len(items))
		case .B:
			g.state = .Mode_Select
		case .A:
			item := items[g.inventory_index]
			if !ITEM_DESCRIPTORS[w.items[item].type].can_take {
				add_message(w, .None, "You cannot take it.")
				g.state = .Neutral
				return
			}
			item_move(w, item, INVENTORY)
			left_over := len(items_at(w, player_location(w)))
			if left_over == 0 {
				g.state = .Mode_Select
				return
			}
			g.inventory_index = g.inventory_index % left_over
		}
	case .Inventory:
		items := items_at(w, INVENTORY)
		#partial switch command {
		case .Up:
			g.inventory_index = wrap(g.inventory_index - 1, len(items))
		case .Down:
			g.inventory_index = wrap(g.inventory_index + 1, len(items))
		case .B:
			g.state = .Mode_Select
		case .A:
			g.state = .Inventory_Detail
		}
	case .Combat_Inventory:
		items := usable_items_at(w, INVENTORY)
		#partial switch command {
		case .Up:
			g.inventory_index = wrap(g.inventory_index - 1, len(items))
		case .Down:
			g.inventory_index = wrap(g.inventory_index + 1, len(items))
		case .B:
			g.state = .Neutral
		case .A:
			use_item(w, items[g.inventory_index])
			g.state = .Neutral
		}
	case .Inventory_Detail:
		#partial switch command {
		case .Up:
			g.detail_index = wrap(g.detail_index - 1, len(DETAIL_TEXT))
		case .Down:
			g.detail_index = wrap(g.detail_index + 1, len(DETAIL_TEXT))
		case .B:
			g.state = .Inventory
		case .A:
			item := items_at(w, INVENTORY)[g.inventory_index]
			switch g.detail_index {
			case 0:
				g.state = .Inventory
			case 1:
				if !ITEM_DESCRIPTORS[w.items[item].type].is_usable {
					add_message(w, .None, "You cannot use that!")
				} else {
					use_item(w, item)
				}
				g.state = .Neutral
			case 2:
				item_move(w, item, player_location(w))
				left_over := len(items_at(w, INVENTORY))
				if left_over == 0 {
					g.state = .Mode_Select
				} else {
					g.inventory_index = g.inventory_index % left_over
					g.state = .Inventory
				}
			}
		}
	}
}

embark :: proc(g: ^Game) {
	world_generate(&g.world, rng_next(&g.rng))
	g.has_world = true
	g.mode_index = 0
	g.combat_index = 0
	g.detail_index = 0
	g.inventory_index = 0
}

continue_game :: proc(g: ^Game) {
	if g.storage.get == nil {
		return
	}
	text, ok := g.storage.get(SAVE_KEY)
	if !ok || !world_load(&g.world, text) {
		g.has_save = false
		g.main_menu_index = 0
		return
	}
	g.has_world = true
	g.mode_index = 0
	g.combat_index = 0
	g.detail_index = 0
	g.inventory_index = 0
	g.state = .Neutral
}

write_save :: proc(g: ^Game) {
	if g.storage.set == nil {
		return
	}
	g.storage.set(SAVE_KEY, world_save(&g.world))
	g.has_save = true
}

clear_save :: proc(g: ^Game) {
	if g.storage.set == nil || !g.has_save {
		return
	}
	g.storage.set(SAVE_KEY, "")
	g.has_save = false
}

CUE_SFX := [Sound_Cue]Sfx {
	.None         = .None,
	.Miss         = .Miss,
	.Player_Hit   = .Player_Hit,
	.Enemy_Hit    = .Enemy_Hit,
	.Player_Death = .Player_Death,
	.Enemy_Death  = .Enemy_Death,
	.Win          = .Level_Up,
}

game_update :: proc(g: ^Game, dt: f64) {
	#partial switch g.state {
	case .Splash:
		g.splash_timer -= dt
		if g.splash_timer <= 0 {
			g.state = .Main_Menu
		}
	case .Neutral:
		w := &g.world
		p := player(w)^
		if len(w.messages) > 0 {
			play(g, CUE_SFX[w.messages[0].cue])
			g.state = .Message
		} else if character_is_dead(p) {
			clear_save(g)
			g.state = .Game_Over
		} else if has_item_type(w, INVENTORY, .Dew_A_Lit_Tea) {
			clear_save(g)
			g.state = .Win
		} else if len(characters_at(w, p.location, true)) > 0 {
			write_save(g)
			g.state = .Combat
		} else {
			write_save(g)
			g.state = .Navigation
		}
	}
}

// ---- drawing ----

game_render :: proc(g: ^Game) {
	c := &g.canvas
	w := &g.world
	switch g.state {
	case .Neutral:
	// draws nothing, as in the original (it never lasts a frame)
	case .Splash:
		canvas_clear(c)
		colors := [?]Hue{.Light_Blue, .Light_Green, .Light_Cyan, .Light_Red, .Light_Magenta, .Yellow, .White}
		hue := colors[rng_range(&g.rng, 0, len(colors) - 1)]
		draw_text(c, .Font8x8, 0, 41 - 16, "Larrikin of SPLORR!!", hue)
		draw_text(c, .Font8x8, 72, 41, "][", hue)
		draw_text(c, .Font8x8, 4, 41 + 16, "Kordanor's Catacomb", hue)
	case .Main_Menu:
		canvas_clear(c)
		items := main_items(g)
		g.main_menu_index = wrap(g.main_menu_index, len(items))
		for item, row in items {
			draw_text(c, .Font5x7, 0, row * font_height(.Font5x7), MAIN_ITEM_TEXT[item], .Light_Green if row == g.main_menu_index else .Gray)
		}
	case .SFX_Volume:
		canvas_clear(c)
		draw_text(c, .Font5x7, 0, 0, "SFX Volume:", .Brown)
		for row in 0 ..= 10 {
			draw_text(c, .Font5x7, VIEW_WIDTH / 2, row * font_height(.Font5x7), fmt.tprintf("%d%%", row * 10), .Light_Green if row == g.volume_tenths else .Gray)
		}
	case .About:
		canvas_clear(c)
		draw_text(c, .Font5x7, 0, 0, "About Kordanor's Catacomb:", .Brown)
		draw_text(c, .Font5x7, 0, 16, "By TheGrumpyGameDev", .Light_Blue)
		draw_text(c, .Font5x7, 0, 32, "Dungeon Crawler Jam 2023", .Light_Blue)
		draw_text(c, .Font5x7, 0, 48, "Odin port, 2026: Claude", .Light_Blue)
		draw_text(c, .Font5x7, 0, 64, "Credits below the game!", .Light_Blue)
	case .Prolog:
		canvas_clear(c)
		draw_text(c, .Font4x6, 0, 0, "PROLOG", .Brown)
		draw_text(c, .Font4x6, 0, 7, "Yer quest: find a cup of tea,   ", .Gray)
		draw_text(c, .Font4x6, 0, 14, "find a match, light the tea to ", .Gray)
		draw_text(c, .Font4x6, 0, 21, "make it 'a lit tea', find a    ", .Gray)
		draw_text(c, .Font4x6, 0, 28, "sponge, find some dew, collect ", .Gray)
		draw_text(c, .Font4x6, 0, 35, "the dew with the sponge, then  ", .Gray)
		draw_text(c, .Font4x6, 0, 42, "squeeze out the dew onto the   ", .Gray)
		draw_text(c, .Font4x6, 0, 49, "lit tea, thus achieving: ", .Gray)
		draw_text(c, .Font4x6, 0, 56, "'Dew a lit tea'", .Green)
		draw_text(c, .Font4x6, 0, 70, "Get it?!? IS PUN! IS FUNNY! LUL", .Light_Blue)
	case .Navigation:
		draw_frame(c, w)
		status_bar(c, "Space/Enter: Change Mode")
	case .Combat:
		draw_frame(c, w)
		status_bar(c, COMBAT_TEXT[g.combat_index])
	case .Mode_Select:
		draw_frame(c, w)
		status_bar(c, MODE_TEXT[g.mode_index])
		if g.mode_index == 3 {
			draw_map(c, w)
		}
	case .Message:
		canvas_clear(c)
		if len(w.messages) > 0 {
			m := &w.messages[0]
			for i in 0 ..< m.line_count {
				draw_text(c, .Font5x7, 0, i * font_height(.Font5x7), m.lines[i], .Gray)
			}
		}
	case .Game_Over:
		canvas_clear(c)
		draw_text(c, .Font5x7, 0, 0, "Yer Dead!", .Red)
	case .Win:
		canvas_clear(c)
		draw_text(c, .Font5x7, 0, 0, "You made Dew a Lit Tea!", .Green)
		draw_text(c, .Font5x7, 0, 8, "You Win!", .Green)
	case .Ground:
		draw_item_list(c, w, items_at(w, player_location(w)), g.inventory_index)
		status_bar(c, "Arrows: Select       Space: Take")
	case .Inventory:
		draw_item_list(c, w, items_at(w, INVENTORY), g.inventory_index)
		status_bar(c, "Arrows: Select      Space: View")
	case .Combat_Inventory:
		draw_item_list(c, w, usable_items_at(w, INVENTORY), g.inventory_index)
		status_bar(c, "Arrows: Select        Space: Use")
	case .Inventory_Detail:
		canvas_clear(c)
		items := items_at(w, INVENTORY)
		if g.inventory_index < len(items) {
			draw_text(c, .Font5x7, 0, 0, ITEM_DESCRIPTORS[w.items[items[g.inventory_index]].type].name, .Magenta)
		}
		for text, row in DETAIL_TEXT {
			draw_text(c, .Font5x7, 0, (row + 1) * font_height(.Font5x7), text, .Light_Green if row == g.detail_index else .Gray)
		}
		status_bar(c, "Arrows: Select        Space: Do")
	}
	canvas_to_rgba(c, &g.pixels)
}

status_bar :: proc(c: ^Canvas, text: string) {
	canvas_fill(c, 0, FRAME_HEIGHT, VIEW_WIDTH, VIEW_HEIGHT - FRAME_HEIGHT, .Green)
	draw_text(c, .Font4x6, FRAME_WIDTH / 2 - text_width(.Font4x6, text) / 2, FRAME_HEIGHT + 2, text, .White)
}

draw_item_list :: proc(c: ^Canvas, w: ^World, items: []int, selected: int) {
	canvas_clear(c)
	h := font_height(.Font5x7)
	y := FRAME_HEIGHT / 2 - h / 2 - selected * h
	for item, index in items {
		text := ITEM_DESCRIPTORS[w.items[item].type].name
		draw_text(c, .Font5x7, FRAME_WIDTH / 2 - text_width(.Font5x7, text) / 2, y, text, .Light_Cyan if index == selected else .Gray)
		y += h
	}
}

// Frame.Draw: the first-person view of the room.
draw_frame :: proc(c: ^Canvas, w: ^World) {
	canvas_fill(c, 0, 0, FRAME_WIDTH, FRAME_HEIGHT, .Black)
	d := Drawer{canvas = c, hue = .Dark_Gray}
	// the back wall and the four corner edges
	pen_path(pen_to(&d, FRAME_WIDTH / 4, FRAME_HEIGHT / 4), "R79 D39 L79 U39")
	pen_to(&d, 0, 0)
	for _ in 0 ..< FRAME_WIDTH / 8 {right(&d, 1);down_right(&d, 1)}
	pen_to(&d, 0, FRAME_HEIGHT - 1)
	for _ in 0 ..< FRAME_WIDTH / 8 {right(&d, 1);up_right(&d, 1)}
	pen_to(&d, FRAME_WIDTH - 1, 0)
	for _ in 0 ..< FRAME_WIDTH / 8 {left(&d, 1);down_left(&d, 1)}
	pen_to(&d, FRAME_WIDTH - 1, FRAME_HEIGHT - 1)
	for _ in 0 ..< FRAME_WIDTH / 8 {left(&d, 1);up_left(&d, 1)}
	location := player_location(w)
	doors := w.locations[location].door
	if doors[w.facing] {
		pen_path(pen_to(&d, FRAME_WIDTH / 2 - FRAME_WIDTH / 16, FRAME_HEIGHT / 2 - FRAME_HEIGHT / 8), "R19 D29 L19 U29")
	}
	if doors[LEFT_OF[w.facing]] {
		pen_path(pen_to(&d, FRAME_WIDTH / 8 - FRAME_WIDTH / 32, FRAME_HEIGHT / 2 - FRAME_HEIGHT / 8), "U7 R3 DR1 R2 DR1 R2 DR1 D41 L1 DL1 L1 DL1 L1 DL1 L1 DL1 L1 DL1 U42")
	}
	if doors[RIGHT_OF[w.facing]] {
		pen_path(pen_to(&d, FRAME_WIDTH - 1 - FRAME_WIDTH / 8 + FRAME_WIDTH / 32, FRAME_HEIGHT / 2 - FRAME_HEIGHT / 8), "U7 L3 DL1 L2 DL1 L2 DL1 D41 R1 DR1 R1 DR1 R1 DR1 R1 DR1 R1 DR1 U42")
	}
	draw_items(c, w, location)
	enemies := characters_at(w, location, true)
	if len(enemies) > 0 {
		draw_sprite(c, CHARACTER_SPRITES[w.characters[enemies[0]].type], BLOB_PALETTE, 56, 22, 4, Hue.Light_Magenta)
	}
	p := player(w)^
	draw_text(c, .Font3x5, 0, 0, fmt.tprintf("HP: %d/%d", character_hp(p), p.stats[.Maximum_HP]), .Red)
	draw_text(c, .Font3x5, 0, 6, fmt.tprintf("ATK: %d", p.stats[.Maximum_Attack]), .Green)
	draw_text(c, .Font3x5, 0, 12, fmt.tprintf("DEF: %d", p.stats[.Maximum_Defend]), .Yellow)
	if has_item_type(w, INVENTORY, .Compass) {
		text := DIRECTION_NAMES[w.facing]
		draw_text(c, .Font3x5, FRAME_WIDTH / 2 - text_width(.Font3x5, text) / 2, 0, text, .Magenta)
	}
	if len(enemies) > 0 {
		text := fmt.tprintf("Enemy: %d", len(enemies))
		draw_text(c, .Font4x6, FRAME_WIDTH - text_width(.Font4x6, text), 0, text, .Blue)
	}
}

Item_Drawing :: struct {
	x, y: int,
	path: string,
}

// The original's drawTable, as pen paths ("c" + hue index in hex sets the colour).
TEA_DRAWING :: Item_Drawing{FRAME_WIDTH / 2, FRAME_HEIGHT * 3 / 4, "cf L2 D3 DR1 R3 UR1 R1 UR1 UL1 L1 D1 U2 L3"}
SPONGE_DRAWING :: Item_Drawing{FRAME_WIDTH / 4, FRAME_HEIGHT * 3 / 4, "ce R3 D5 L3 U5"}

ITEM_DRAWINGS := [Item_Type]Item_Drawing {
	.Koettbulle    = {FRAME_WIDTH / 4, FRAME_HEIGHT * 7 / 8, "c6 R1 DR1 D1 DL1 L1 UL1 U1 UR1"},
	.Knorva        = {FRAME_WIDTH * 3 / 8, FRAME_HEIGHT * 3 / 4, "c9 L2 R4 DR1 L1 c1 L5 c9 D1 R6 D1 L6 DL1 R1 ce R1 c9 R5 ce R1 c9 DR1 L3 ce L5 c9 L2 D1 R10 DL1 L8"},
	.Sponge        = SPONGE_DRAWING,
	.Wet_Sponge    = SPONGE_DRAWING,
	.Tea           = TEA_DRAWING,
	.Lit_Tea       = TEA_DRAWING,
	.Dew_A_Lit_Tea = TEA_DRAWING,
	.Match         = {FRAME_WIDTH / 2, FRAME_HEIGHT * 7 / 8, "c4 DR1 c6 DR4"},
	.Burnt_Match   = {FRAME_WIDTH / 2, FRAME_HEIGHT * 7 / 8, "c8 DR2 c6 DR3"},
	.Dew_Puddle    = {FRAME_WIDTH * 3 / 4, FRAME_HEIGHT * 7 / 8, "c1 R2 DR1 R1 DR1 R2 DR1 D1 DL1 L1 DL1 L4 UL3 U1 UR2"},
	.Sylt_Lingon   = {FRAME_WIDTH * 5 / 8, FRAME_HEIGHT * 3 / 4, "c7 L2 R5 DL1 c4 L3 DL1 R5 D1 L5 D1 R5 D1 L5 D1 R6"},
	.Voerda        = {FRAME_WIDTH * 3 / 8, FRAME_HEIGHT * 7 / 8, "c8 U5 D2 L1 D6 U3 L1 D3 L1 R2 D1 c6 D2 R1 U3 c8 U4"},
	.Compass       = {FRAME_WIDTH * 5 / 8, FRAME_HEIGHT * 7 / 8, "c7 R2 D1 R1 D4 L1 D1 L4 U1 L1 U4 R1 U1 R2 c4 D4"},
}

draw_items :: proc(c: ^Canvas, w: ^World, location: int) {
	d := Drawer{canvas = c, hue = .White}
	for t in item_types_at(w, location) {
		drawing := ITEM_DRAWINGS[t]
		pen_path(pen_to(&d, drawing.x, drawing.y), drawing.path)
	}
}

CHARACTER_SPRITES := [Character_Type][]string {
	.Larrikin = nil,
	.Blob = {
		"            ",
		"            ",
		"            ",
		"   ......   ",
		"  .XXXXXX.  ",
		" .XXXXXXXX. ",
		".XX.X.XXXXX.",
		".XX.X.XXXXX.",
		".XX.X.XXXXX.",
		".XXXXXXXXXX.",
		" .XXXXXXXX. ",
		"  ........  ",
	},
}

BLOB_PALETTE :: Sprite_Palette{key = ".X", hues = {.Black, .Cyan}, rest = .Light_Magenta}
MAP_PALETTE :: Sprite_Palette{key = "X ", hues = {.White, .Dark_Gray}, rest = .Brown}

// The 16 map cells of ModeSelectState: a walled room with a gap on each side that has a door.
map_cell :: proc(doors: [Direction]bool) -> [8][8]u8 {
	base := [8]string{"........", ".XXXXXX.", ".X    X.", ".X    X.", ".X    X.", ".X    X.", ".XXXXXX.", "........"}
	if doors[.North] {
		base[0] = "..X  X.."
		base[1] = ".XX  XX."
	}
	if doors[.South] {
		base[6] = ".XX  XX."
		base[7] = "..X  X.."
	}
	cell: [8][8]u8
	for i in 0 ..< 8 {
		copy(cell[i][:], base[i])
	}
	if doors[.East] {
		cell[2][6], cell[2][7] = 'X', 'X'
		cell[3][6], cell[3][7] = ' ', ' '
		cell[4][6], cell[4][7] = ' ', ' '
		cell[5][6], cell[5][7] = 'X', 'X'
	}
	if doors[.West] {
		cell[2][0], cell[2][1] = 'X', 'X'
		cell[3][0], cell[3][1] = ' ', ' '
		cell[4][0], cell[4][1] = ' ', ' '
		cell[5][0], cell[5][1] = 'X', 'X'
	}
	return cell
}

draw_map :: proc(c: ^Canvas, w: ^World) {
	here := player_location(w)
	for column in 0 ..< MAZE_COLUMNS {
		for row in 0 ..< MAZE_ROWS {
			x := FRAME_WIDTH / 2 - 32 + column * 8
			y := 4 + row * 8
			location := location_index(column, row)
			if !w.locations[location].visited {
				canvas_fill(c, x, y, 8, 8, .Brown)
				continue
			}
			cell := map_cell(w.locations[location].door)
			rows: [8]string
			for i in 0 ..< 8 {
				rows[i] = string(cell[i][:])
			}
			draw_sprite(c, rows[:], MAP_PALETTE, x, y, 1, nil)
			if location == here {
				canvas_fill(c, x + 3, y + 3, 2, 2, .Magenta)
			}
		}
	}
}
