# Sprites

Imágenes y spritesheets del juego.

- **suit_hearts.png** — Corazón lleno para la HUD.
- **suit_hearts_broken.png** — Corazón roto (cuando perdés vida).
- **cielo.png** — Imagen de fondo del cielo (cargada desde Resources).

### Sprites/Character/

Carpeta con los sprites del personaje, organizados por animación. Cada subcarpeta tiene los frames de una acción:

- **Attack/** — 5 frames del ataque.
- **Idle/** — 4 frames de idle.
- **Jump/** — 7 frames del salto.
- **Run/** — 4 frames de correr.
- **Walk/** — 4 frames de caminar.

Los nombres de las carpetas deben coincidir con los nombres de los clips en `Animations/`.
