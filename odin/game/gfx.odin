package game

// The software framebuffer: 160x90 hue indices, the original's AOS.UI drawing model (every write clipped),
// its turtle-style line Drawer, fixed-width glyph fonts and character sprites.

FRAME_WIDTH :: 160
FRAME_HEIGHT :: 80
VIEW_WIDTH :: FRAME_WIDTH
VIEW_HEIGHT :: FRAME_HEIGHT + 10

Hue :: enum u8 {
	Black,
	Blue,
	Green,
	Cyan,
	Red,
	Magenta,
	Brown,
	Gray,
	Dark_Gray,
	Light_Blue,
	Light_Green,
	Light_Cyan,
	Light_Red,
	Light_Magenta,
	Yellow,
	White,
}

// Each entry is 0xAABBGGRR, so the bytes in memory are R, G, B, A. The original's CGA table (Program.vb).
PALETTE := [Hue]u32 {
	.Black         = 0xFF000000,
	.Blue          = 0xFF000000 | 170 << 16,
	.Green         = 0xFF000000 | 170 << 8,
	.Cyan          = 0xFF000000 | 170 << 16 | 170 << 8,
	.Red           = 0xFF000000 | 170,
	.Magenta       = 0xFF000000 | 170 << 16 | 170,
	.Brown         = 0xFF000000 | 85 << 8 | 170,
	.Gray          = 0xFF000000 | 170 << 16 | 170 << 8 | 170,
	.Dark_Gray     = 0xFF000000 | 85 << 16 | 85 << 8 | 85,
	.Light_Blue    = 0xFF000000 | 255 << 16 | 85 << 8 | 85,
	.Light_Green   = 0xFF000000 | 85 << 16 | 255 << 8 | 85,
	.Light_Cyan    = 0xFF000000 | 255 << 16 | 255 << 8 | 85,
	.Light_Red     = 0xFF000000 | 85 << 16 | 85 << 8 | 255,
	.Light_Magenta = 0xFF000000 | 255 << 16 | 85 << 8 | 255,
	.Yellow        = 0xFF000000 | 85 << 16 | 255 << 8 | 255,
	.White         = 0xFFFFFFFF,
}

Canvas :: struct {
	pixels: [VIEW_WIDTH * VIEW_HEIGHT]Hue,
}

canvas_set :: proc(c: ^Canvas, x, y: int, hue: Hue) {
	if x < 0 || y < 0 || x >= VIEW_WIDTH || y >= VIEW_HEIGHT {
		return
	}
	c.pixels[x + y * VIEW_WIDTH] = hue
}

canvas_get :: proc(c: ^Canvas, x, y: int) -> Hue {
	if x < 0 || y < 0 || x >= VIEW_WIDTH || y >= VIEW_HEIGHT {
		return .Black
	}
	return c.pixels[x + y * VIEW_WIDTH]
}

canvas_fill :: proc(c: ^Canvas, x, y, w, h: int, hue: Hue) {
	for dy in 0 ..< h {
		for dx in 0 ..< w {
			canvas_set(c, x + dx, y + dy, hue)
		}
	}
}

canvas_clear :: proc(c: ^Canvas) {
	canvas_fill(c, 0, 0, VIEW_WIDTH, VIEW_HEIGHT, .Black)
}

// ---- fonts ----

Font :: enum u8 {
	Font3x5,
	Font4x6,
	Font5x7,
	Font8x8,
}

font_height :: proc(f: Font) -> int {
	return FONTS[f].height
}

text_width :: proc(f: Font, text: string) -> int {
	return len(text) * FONTS[f].width
}

// Characters outside ASCII 32..127 are skipped but still take their width (the original would have thrown).
draw_text :: proc(c: ^Canvas, f: Font, x, y: int, text: string, hue: Hue) {
	data := &FONTS[f]
	x := x
	for i in 0 ..< len(text) {
		index := int(text[i]) - FONT_FIRST
		if index >= 0 && index < FONT_COUNT {
			glyph := &data.glyphs[index]
			for row in 0 ..< data.height {
				bits := glyph[row]
				for col in 0 ..< data.width {
					if bits & (1 << uint(col)) != 0 {
						canvas_set(c, x + col, y + row, hue)
					}
				}
			}
		}
		x += data.width
	}
}

// ---- the Drawer: a pen that sets a pixel and then steps, for each step ----

Drawer :: struct {
	canvas: ^Canvas,
	x, y:   int,
	hue:    Hue,
}

pen_to :: proc(d: ^Drawer, x, y: int) -> ^Drawer {
	d.x, d.y = x, y
	return d
}

pen_color :: proc(d: ^Drawer, hue: Hue) -> ^Drawer {
	d.hue = hue
	return d
}

pen_walk :: proc(d: ^Drawer, dx, dy, steps: int) -> ^Drawer {
	for _ in 0 ..< steps {
		canvas_set(d.canvas, d.x, d.y, d.hue)
		d.x += dx
		d.y += dy
	}
	return d
}

up :: proc(d: ^Drawer, n: int) -> ^Drawer {return pen_walk(d, 0, -1, n)}
down :: proc(d: ^Drawer, n: int) -> ^Drawer {return pen_walk(d, 0, 1, n)}
left :: proc(d: ^Drawer, n: int) -> ^Drawer {return pen_walk(d, -1, 0, n)}
right :: proc(d: ^Drawer, n: int) -> ^Drawer {return pen_walk(d, 1, 0, n)}
up_left :: proc(d: ^Drawer, n: int) -> ^Drawer {return pen_walk(d, -1, -1, n)}
up_right :: proc(d: ^Drawer, n: int) -> ^Drawer {return pen_walk(d, 1, -1, n)}
down_left :: proc(d: ^Drawer, n: int) -> ^Drawer {return pen_walk(d, -1, 1, n)}
down_right :: proc(d: ^Drawer, n: int) -> ^Drawer {return pen_walk(d, 1, 1, n)}

// A pen path as text: "c<Hue index in hex>" sets the colour, otherwise <direction><count> pairs separated by
// spaces, directions U D L R and the diagonals UL UR DL DR. Keeps the item drawings close to the original's
// fluent chains while staying data.
pen_path :: proc(d: ^Drawer, path: string) {
	i := 0
	for i < len(path) {
		for i < len(path) && path[i] == ' ' {
			i += 1
		}
		if i >= len(path) {
			break
		}
		start := i
		for i < len(path) && path[i] != ' ' {
			i += 1
		}
		token := path[start:i]
		if token[0] == 'c' {
			d.hue = Hue(hex_digit(token[1]))
			continue
		}
		letters := 0
		for letters < len(token) && (token[letters] < '0' || token[letters] > '9') {
			letters += 1
		}
		n := 0
		for ch in token[letters:] {
			n = n * 10 + int(ch - '0')
		}
		switch token[:letters] {
		case "U":
			up(d, n)
		case "D":
			down(d, n)
		case "L":
			left(d, n)
		case "R":
			right(d, n)
		case "UL":
			up_left(d, n)
		case "UR":
			up_right(d, n)
		case "DL":
			down_left(d, n)
		case "DR":
			down_right(d, n)
		}
	}
}

hex_digit :: proc(b: u8) -> int {
	switch b {
	case '0' ..= '9':
		return int(b - '0')
	case 'a' ..= 'f':
		return int(b - 'a') + 10
	case 'A' ..= 'F':
		return int(b - 'A') + 10
	}
	return 0
}

// ---- sprites: rows of characters mapped to hues, stretched; `skip` hues are transparent ----

Sprite_Palette :: struct {
	key:  string, // characters
	hues: []Hue, // hue for each key character
	rest: Hue, // hue for any other character
}

draw_sprite :: proc(c: ^Canvas, rows: []string, p: Sprite_Palette, x, y, scale: int, skip: Maybe(Hue)) {
	for row, ry in rows {
		for i in 0 ..< len(row) {
			hue := p.rest
			for k in 0 ..< len(p.key) {
				if p.key[k] == row[i] {
					hue = p.hues[k]
				}
			}
			if s, ok := skip.?; ok && s == hue {
				continue
			}
			canvas_fill(c, x + i * scale, y + ry * scale, scale, scale, hue)
		}
	}
}

canvas_to_rgba :: proc(c: ^Canvas, out: ^[VIEW_WIDTH * VIEW_HEIGHT]u32) {
	for p, i in c.pixels {
		out[i] = PALETTE[p]
	}
}
