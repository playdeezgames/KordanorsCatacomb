# Quirks of the original, and what the port did with them

The Odin port keeps the VB.NET original's rules, screens and text. Where the original had a quirk, it is listed here with what the port does. "Kept" means the port behaves as the 2023 game did.

## Kept

- **Köttbullar bank hit points.** Eating one lowers Wounds by one with no floor, so eating at full health stores hidden extra hit points (HP still shows full). Kept.
- **The splash title changes colour every frame** for its three seconds (a random pick of seven bright colours each frame). Kept. It flashes quickly; worth a look by the author for photosensitive players.
- **Run** faces a random direction first: with a door there, you leave without a counterattack ("Larrikin runs!"); against a wall, every Blob in the room gets a free attack, and those attacks play the enemy-hit sound rather than the player-hit one. Kept.
- **"It is now a Lit Tee."** (sic) when the match lights the tea. Kept as written.
- **Choosing Map** in the mode menu does nothing; the map shows while Map is selected. Kept.
- Blobs can start in the player's first room, so a run can open with a fight. Kept.
- Menu positions (main menu, mode menu, fight menu, item menu) are remembered between visits, as the original's state objects did. Kept.
- Item names are written without umlauts (Koettbulle, Voerda): the fonts are ASCII only. Kept.

## Changed

- **Saving is new.** The world is written to localStorage (`kc:save`) whenever play returns to walking or fighting, and cleared on death or victory. The main menu gains **Continue** while a save exists.
- **SFX volume** is remembered in localStorage (`kc:volume`) instead of `config.json`.
- **Screen Size...** is gone (the page scales the 160 by 90 picture to the window); **Full Screen** takes its place.
- **About** no longer points at `aboot.txt`; its credits are in the page footer.
- **A key pressed in the same frame as a move is no longer lost.** The original resolved the in-between Neutral state only once per frame, so a second key in that frame was swallowed; the port resolves it at once.
- Lists show items in the order they arrived (the original's .NET HashSet order, which is nearly always the same).
- The random number generator is the port's own (SplitMix64), so a seed does not reproduce a 2023 dungeon.
- Characters outside ASCII are skipped when drawn (the original would have crashed); none occur.
