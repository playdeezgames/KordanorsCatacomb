#!/usr/bin/env python3
"""Draws assets/cover.png (630x500) from the game's own 8x8 font and a real frame (assets/screenshots/04-fight.png,
made by tools/make_shots.sh). Run make_shots.sh first."""
import json, os
from PIL import Image

root = os.path.join(os.path.dirname(__file__), "..")
font = json.load(open(os.path.join(root, "assets", "fonts", "CyFont8x8.json"), encoding="utf-8-sig"))
small = json.load(open(os.path.join(root, "assets", "fonts", "CyFont4x6.json"), encoding="utf-8-sig"))

W, H = 630, 500
img = Image.new("RGB", (W, H), (0, 0, 0))
px = img.load()


def text(f, s, x, y, scale, color):
    for ch in s:
        g = f["Glyphs"].get(ch)
        if g:
            for row, cols in g["Lines"].items():
                for col in cols:
                    for dy in range(scale):
                        for dx in range(scale):
                            X, Y = x + col * scale + dx, y + int(row) * scale + dy
                            if 0 <= X < W and 0 <= Y < H:
                                px[X, Y] = color
        x += g["Width"] * scale if g else 8 * scale


def width(f, s, scale):
    return sum(f["Glyphs"][c]["Width"] for c in s) * scale


def centered(f, s, y, scale, color):
    text(f, s, (W - width(f, s, scale)) // 2, y, scale, color)


YELLOW = (255, 255, 85)
CYAN = (85, 255, 255)
GRAY = (170, 170, 170)

centered(font, "Larrikin of SPLORR!!", 22, 3, YELLOW)
centered(font, "][", 56, 3, YELLOW)
centered(font, "Kordanor's Catacomb", 90, 3, CYAN)

shot = Image.open(os.path.join(root, "assets", "screenshots", "04-fight.png")).convert("RGB")
frame = shot.resize((160 * 3, 90 * 3), Image.NEAREST)
img.paste(frame, ((W - frame.width) // 2, 130))

centered(small, "Make Dew a Lit Tea. Mind the Blobs.", 418, 3, GRAY)
centered(small, "Dungeon Crawler Jam 2023", 450, 3, GRAY)

out = os.path.join(root, "assets", "cover.png")
img.save(out)
print("wrote", out)
