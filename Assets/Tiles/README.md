# Tiles

Todo lo relacionado con tilemaps y tiles para el nivel.

### Spritesheets raíz
- **tilemap(18X18).png** — Tiles de terreno (tierra, hierba). 18×18 px por tile.
- **tilemap-backgrounds(24X24).png** — Tiles de fondo (colores planos). 24×24 px.
- **tilemap-characters(24X24).png** — Tiles de personajes y enemigos. 24×24 px.

### Tiles/Terrain/

Tiles individuales del terreno recortados del spritesheet. Cada `.asset` es un tile de Unity con su sprite, collider, etc. Las carpetas `Sprites/` tiene los PNGs originales.

### Tiles/Tiles/

Los assets de tile generados para cada sprite. El nombre sigue el patrón `spritesheet_N.asset` donde N es el índice.

### Tiles/TilePalette/

La paleta de tiles que usa Unity Paint Tool para pintar en el tilemap. El palette se llama "New Palette" (se puede renombrar).
