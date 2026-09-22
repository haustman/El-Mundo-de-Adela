# Sprites

Imágenes y spritesheets del juego.

### Sprites/Character/

Sprites del personaje, organizados por animación. Cada subcarpeta tiene los frames de una acción:

- **Attack/** — 5 frames del ataque.
- **Danger/** — Spritesheet de recibir daño (3 frames, 32x32 cada uno).
- **Death/** — Spritesheet de muerte (4 frames, 32x32 cada uno).
- **Idle/** — 4 frames de idle.
- **Jump/** — 7 frames del salto.
- **Run/** — 4 frames de correr.
- **Walk/** — 4 frames de caminar.

Los nombres de las carpetas deben coincidir con los nombres de los clips en `Animations/`.

### Sprites/Boss/

Sprites del jefe, organizados por animación (5 frames de 32x32 por acción):

- **Attack/** — Ataque del jefe.
- **Idle/** — Jefe parado.
- **Run/** — Jefe corriendo.
- **Walk/** — Jefe caminando.

Los nombres deben coincidir con `BossIdle/Walk/Run/Attack.anim` en `Animations/`.

### Sprites/UI/

Assets de interfaz de usuario:

- **Logo.png** — Logo del juego "El Mundo de Adela" (pantalla de menú).
- **Jugar.png** — Botón de Jugar.
- **Salir.png** — Botón de Salir.
- **suit_hearts.png** — Corazón lleno para la HUD.
- **suit_hearts_broken.png** — Corazón roto (cuando perdés vida).
