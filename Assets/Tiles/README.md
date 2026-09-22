# Tiles

Todo lo relacionado con tilemaps y tiles para el nivel.

### Spritesheets raíz
- **tilemap(18X18).png** — Tiles de terreno (tierra, hierba). 18×18 px por tile.
- **tilemap-backgrounds(24X24).png** — Tiles de fondo (colores planos). 24×24 px.
- **tilemap-characters(24X24).png** — Tiles de personajes y enemigos. 24×24 px.
- **Elementos.png** — Pack de decoración (tierra/hierba, rocas, arbustos, flores, troncos, valla, estanque). 1254×1254 px, fondo transparente. **Ya cortado en 70 sprites** (`Elementos_0`…`Elementos_69`, celdas ajustadas al contenido, PPU 140, sin solapamientos).
- **Tileset.png** — Tileset de terreno (césped, tierra, pendientes). 128×128 px (el arte ocupa los 128×64 superiores), paleta de 8 colores, alineado a rejilla de 32×32. Import: Multiple, Point, PPU 32. Slice: Grid by Cell Size (32×32) en el Sprite Editor.
- **Fondo.png** — Fondo decorativo del menú/nivel.
- **button ver 2 (785x271).png** — Hoja de botones de UI.

### Tiles/Terrain/

Tiles individuales del terreno recortados del spritesheet. Cada `.asset` es un tile de Unity con su sprite, collider, etc. Las carpetas `Sprites/` tiene los PNGs originales.

### Tiles/Tiles/

Los assets de tile generados para cada sprite. El nombre sigue el patrón `spritesheet_N.asset` donde N es el índice.

### Tiles/TilePalette/

La paleta de tiles que usa Unity Paint Tool para pintar en el tilemap. El palette se llama "New Palette" (se puede renombrar).
