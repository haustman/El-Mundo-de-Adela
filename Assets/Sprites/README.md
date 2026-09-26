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

### Sprites/Projectile/

- **Arma.png** — Sprite del disparo del jugador (se usa en los prefabs `Bullet 1` / `Bullet 2`).

### Sprites/UI/

Assets de interfaz y fondos de escena:

- **Logo.png** — Logo del juego "El Mundo de Adela" (pantalla de menú).
- **Jugar.png** — Botón de Jugar. **Sin usar**: no lo referencia ninguna escena (los botones usan la hoja `Tiles/button ver 2 (785x271).png`).
- **Salir.png** — Botón de Salir. **Sin usar**, igual que `Jugar.png`.
- **suit_hearts.png** — Corazón lleno para la HUD.
- **suit_hearts_broken.png** — Corazón roto (cuando perdés vida).
- **Fondo.png** — Fondo del nivel (1024×1536). Es el sprite del objeto `Fondo para juego`, hijo de la Main Camera con sorting −100.
- **Fondo para juego.jpg** — Fondo original del nivel (1366×768). **Ya no se usa**: ninguna escena lo referencia (lo reemplazó `Fondo.png`). Se mantiene por si se reutiliza.

### Sprites/Hazards/

Assets CC0 de peligros **guardados sin implementar** (pinchos): PNGs sueltos, pack Bevouliin y Kenney Pixel Platformer. Ver `Hazards/README.md` e `Hazards/INDICE.txt`.
