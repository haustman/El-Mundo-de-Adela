# Tiles

Todo lo relacionado con tilemaps y tiles para el nivel.

### Spritesheets raíz
- **tilemap(18X18).png** — Tiles de terreno (tierra, hierba). 18×18 px por tile.
- **tilemap-backgrounds(24X24).png** — Tiles de fondo (colores planos). 24×24 px.
- **tilemap-characters(24X24).png** — Tiles de personajes y enemigos. 24×24 px.
- **Elementos.png** — Pack de decoración (tierra/hierba, rocas, arbustos, flores, troncos, valla, estanque). 1254×1254 px, fondo transparente. **Ya cortado en 70 sprites** (`Elementos_0`…`Elementos_69`, celdas ajustadas al contenido, PPU 140, sin solapamientos).
- **Tileset.png** — Tileset de terreno (césped, tierra, pendientes). 128×128 px (el arte ocupa los 128×64 superiores), paleta de 8 colores, alineado a rejilla de 32×32. Import: Multiple, Point, PPU 32. Slice: Grid by Cell Size (32×32) en el Sprite Editor.
- **Fondo.png** — Fondo decorativo (1920×1080). Se usa como `Image` de fondo del menú (`Menu.unity`). *No confundir con `Sprites/UI/Fondo.png`, que es el fondo del nivel.*
- **button ver 2 (785x271).png** — Hoja de botones de UI.

### Tiles/Terrain/

Tiles individuales del terreno recortados del spritesheet. Cada `.asset` es un tile de Unity con su sprite, collider, etc. Las carpetas `Sprites/` tiene los PNGs originales.

- **`Terrain.asset`** es un **RuleTile** (el que realmente se usa: lo referencia la escena y la paleta) y es el que mantiene vivos los PNGs de `Terrain/Sprites/`.
- Los otros 16 (`Terrain_Fill.asset`, `Terrain_RimTop.asset`, …) son tiles sueltos **sin uso**: nada los referencia. Los sprites no se deben borrar (los usa el RuleTile).

### Tiles/Tiles/

Los assets de tile generados para sprites usados en el nivel. El nombre sigue el patrón `<hoja>_N.asset`.

Hoy hay:

- **Serie completa** de `tilemap(18X18)_N` (180) y `tilemap-backgrounds(24X24)_N` (24); casi completa de `tilemap-characters(24X24)_N` (26 de 27, falta `_2`).
- **Parcial** de decoración/terreno: solo algunos `Elementos_N` (p. ej. `_24`, `_33`, `_63`) y `Tileset_N` (`_0`, `_2`). El resto se genera al pintar o al crear tiles desde el Sprite Editor de `Elementos.png` / `Tileset.png`.

Los tiles nuevos deben colocarse aquí, no en la raíz de `Tiles/`, para no mezclarlos con las hojas de sprites.

### Tiles/TilePalette/

La paleta de tiles que usa Unity Paint Tool para pintar en el tilemap. El palette se llama "New Palette" (se puede renombrar).
