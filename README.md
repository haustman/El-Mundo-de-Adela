# El Mundo de Adela

Juego de plataformas 2D hecho en **Unity 2022.3.20f1**.

Adela recorre un mundo lleno de peligros y tiene que encontrar **5 libros**.
Cada libro que encuentra le revela **un secreto**.

---

## Estado actual

**Es jugable, pero sigue siendo un prototipo de un solo nivel.**
La historia de verdad (los 5 libros y sus secretos) **todavía no está implementada**; es lo siguiente en la lista.

### Lo que ya funciona

- **Movimiento**: correr (Shift izq.), salto y doble salto (Espacio), disparo (clic izq.).
- **Vida**: 3 corazones, invulnerabilidad con parpadeo tras recibir daño.
- **Caída al vacío**: resta 1 corazón y Adela reaparece en el punto de inicio del nivel.
- **Muerte**: al quedarse sin corazones, animación de muerte y vuelta al menú.
- **Enemigos**:
  - Cuerpo a cuerpo: patrulla por una ruta y hace daño al tocarte.
  - A distancia: patrulla y dispara.
  - Los dos tienen barra de vida y reciben daño de tus proyectiles.
- **Recogibles**: corazones que curan (hasta el máximo de 3). *(Código listo; todavía no hay ninguno colocado en el nivel.)*
- **Meta**: al tocarla se acaba el nivel y este vuelve a empezar. *(Código listo; todavía no hay ninguna colocada en el nivel.)*
- **Menú**: logo con fade-in y titileo, botones con hover y sombra, fundido a negro al entrar al juego.
- **Cámara**: sigue a Adela con encuadre en el tercio inferior de la pantalla.
- **Cielo**: fondo con deriva suave para que parezca que se mueve.
- **HUD**: corazones de vida.
- **Arte**: hoja de botones troceada en 72 sprites (claro/medio/oscuro), lista para usarse.

### Qué hay realmente colocado en `SampleScene`

- **Sí**: jugador, cámara, tilemap de suelo, 2 enemigos cuerpo a cuerpo, 3 enemigos a distancia (cada uno con su barra de vida) y el HUD de corazones.
- **No** (el código existe, pero no hay instancia en el nivel): **corazones**, **meta** y **jefe**. Hasta que se coloquen, estas piezas no aparecen en partida.

### Lo que falta

**Fase 1 — Núcleo de la historia** *(prioridad)*
1. Sistema de diálogo (cuadro de texto, efecto máquina de escribir, avanzar con clic/tecla).
2. Los **5 libros**: recogibles que suman al contador `N/5` y lanzan su diálogo con el secreto.
3. Menú de pausa.
4. Pantalla de "todos los secretos encontrados".

**Fase 2 — Presentación**
5. Sonido: efectos (salto, disparo, daño, recoger) y música de fondo.
6. Pantalla de Game Over y sistema de vidas.
7. Puntuación.

**Fase 3 — Contenido**
8. Checkpoints.
9. Más niveles (hoy solo hay uno).
10. Guardado de progreso.

**Fase 4 — Extras**
11. Opciones (volumen, pantalla completa).
12. Controles táctiles (el juego está pensado para móvil pero hoy solo usa teclado/ratón).

---

## Controles

| Acción | Tecla |
|---|---|
| Mover | ← → |
| Saltar | Espacio |
| Correr | Shift izquierdo |
| Disparar | Clic izquierdo |
| Reiniciar | (automático al morir) |

Al caer al vacío no se reinicia: se pierde 1 corazón y Adela reaparece donde empezó.

---

## El recorrido del juego

Para entenderlo de un vistazo:

1. Arranca en **Menu.unity** (índice 0): el logo aparece y titila, y hay dos botones, **Jugar** y **Salir**.
2. **Jugar** hace un fundido a negro y carga **SampleScene.unity** (índice 1).
3. En el nivel, Adela corre, salta y dispara. Hay enemigos de dos tipos.
4. Si **cae al vacío**, pierde 1 corazón y vuelve al punto de inicio.
5. Si **se queda sin corazones**, muere y vuelve al menú.
6. La **meta** (cuando se coloque) recarga el nivel.

---

## Las escenas

### `Assets/Scenes/Menu.unity` — índice 0

Es la pantalla de inicio. Dentro hay:

- Un **Canvas** con el objeto **Menu**:
  - **Image** → el logo, con `MenuEffects`: aparece con fade-in y después titila.
  - **Jugar** → botón; al pulsarlo llama a `Menu.Play()`.
  - **Salir** → botón; al pulsarlo llama a `Menu.Quit()`.
  - Cada botón lleva su **Text (TMP)** con la etiqueta.
- **EventSystem** → hace falta para que los botones reciban clics y hover.
- **Main Camera** → la cámara de la pantalla de menú.
- Scripts: **Menu.cs** y **MenuEffects.cs**.

### `Assets/Scenes/SampleScene.unity` — índice 1

Es el nivel. Dentro hay:

- **Main Camera** → con `CameraController`; sigue al jugador y fabrica el fondo de cielo.
- **Character** → el jugador, con `PlayerController`, un `Rigidbody2D` y su `Animator`.
- **HUD** → con `HUDController`, dibuja los corazones de vida.
- **Tilemap** → el suelo, pintado con la paleta de tiles.
- **Enemy** y **Enemy (1)** → enemigos cuerpo a cuerpo. Cada uno patrulla entre `Patrol1` y `Patrol1 (1)` y tiene su `Slider` de vida (`EnemyHUDController`).
- **EnemyShooter**, **EnemyShooter (1)** y **EnemyShooter (2)** → enemigos a distancia, cada uno con su `Slider` (`EnemyShooterHUDController`) y `shootNum = 3`.
- **EventSystem** → necesario para la UI.

---

## El mapa y los tiles

Todo lo del escenario vive en `Assets/Tiles/` y se pinta sobre el **Tilemap** de la escena con la paleta.

### Hojas de sprites (los PNG de origen)

| Archivo | Qué trae |
|---|---|
| `tilemap(18X18).png` | terreno (tierra, hierba), celdas de 18×18 |
| `tilemap-backgrounds(24X24).png` | fondos de color plano, 24×24 |
| `tilemap-characters(24X24).png` | personajes y enemigos, 24×24 |
| `Elementos.png` | pack de decoración (rocas, arbustos, flores, troncos, valla, estanque). Ya cortado en 70 sprites, `Elementos_0` … `Elementos_69` |
| `Tileset.png` | terreno (césped, tierra, pendientes). Cortado en 8 sprites de 32×32 |
| `Fondo.png` | fondo decorativo del menú/nivel |
| `button ver 2 (785x271).png` | hoja de botones de UI |

### Subcarpetas

- **`Tiles/Tiles/`** → un `.asset` por sprite. Son los "ladrillos" que se colocan. El nombre sigue el patrón `<hoja>_N`: `tilemap(18X18)_N.asset`, `tilemap-backgrounds(24X24)_N.asset`, `tilemap-characters(24X24)_N.asset`, `Elementos_N.asset` y `Tileset_N.asset`.
- **`Tiles/Terrain/`** → tiles de terreno individuales (`Terrain.asset`, `Terrain_Fill.asset`, `Terrain_RimTop.asset`, etc.), con sus PNGs en `Terrain/Sprites/`.
- **`Tiles/TilePalette/New Palette.prefab`** → la paleta que usa el Paint Tool para pintar el tilemap.

Si cambias o recortas una hoja, hay que volver a cortar los sprites en el Sprite Editor y regenerar sus `.asset`.

---

## Los scripts

Están todos en `Assets/Scripts/`. Son **17** y cada uno hace una sola cosa.

### Jugador
- **PlayerController.cs** — Todo el jugador: mover, correr, salto y doble salto, disparar, vida, invulnerabilidad con parpadeo, caída al vacío (resta 1 corazón y reaparece), muerte y `Finish()` para la meta.
- **CameraController.cs** — Sigue al jugador con un desfase, crea el fondo del cielo desde `Resources/cielo.png` y le da una deriva suave.

### Enemigos
- **Damageable.cs** — Base de todo lo que tiene vida y recibe daño. Guarda `Health`/`MaxHealth`, aplica `GetHit` y prepara la barra la primera vez que hace falta. No se coloca en la escena: de aquí heredan los demás.
- **Enemy.cs** — Enemigo cuerpo a cuerpo. Recorre una ruta y hace daño al tocarte.
- **EnemyShooter.cs** — Enemigo a distancia. Recorre una ruta, dispara una vez por tramo y se gira al cerrar la vuelta.
- **Boss.cs** — Jefe. Patrulla si se le configura, mira al jugador y ataca cuando lo tiene cerca, con un tiempo de espera entre golpes. *Sin instancia en el nivel.*
- **PatrolRoute.cs** — Las piezas de la patrulla: `PatrolMovement` (un punto y cuánto tarda en recorrerlo) y `PatrolRoute` (recorre la lista en bucle interpolando).

### Combate
- **Projectile.cs** — Proyectil del jugador: avanza en línea recta y hace daño al primero que toca.
- **ProjectileEnemy.cs** — Lo mismo, pero el que disparan los enemigos.

### Interfaz (UI)
- **HUDController.cs** — Los corazones del jugador. Se puede llamar desde cualquier sitio con `HUDController.Refresh(vida)`.
- **HealthBarController.cs** — Barra de vida reutilizable. De aquí heredan las dos siguientes.
- **EnemyHUDController.cs** — Barra del enemigo cuerpo a cuerpo. Es una subclase vacía: solo existe para distinguirla en el Inspector.
- **EnemyShooterHUDController.cs** — Barra del enemigo a distancia. Igual, otra subclase vacía.

### Menú
- **Menu.cs** — Los dos botones del menú: `Play()` (Jugar) y `Quit()` (Salir).
- **MenuEffects.cs** — Los adornos del menú: fade-in y titileo del logo, sombra y escala en los botones al pasar el ratón, y el fundido a negro antes de entrar al juego.

### Objetos
- **Heart.cs** — Corazón que se recoge y cura 1. Gira sobre sí mismo. *Sin instancia en el nivel.*
- **FinishPoint.cs** — La meta del nivel: al tocarla llama a `PlayerController.Finish()`. *Sin instancia en el nivel.*

---

## Estructura de carpetas

```
Assets/
├── Animations/     Clips .anim + Character.controller y Boss.controller
├── Prefab/         Prefabs de proyectiles (Bullet 1, Bullet 2, BulletEnemy)
├── Resources/      Assets cargados desde código (cielo.png)
├── Scenes/         Menu.unity + SampleScene.unity
├── Scripts/        17 scripts C# (jugador, enemigos, jefe, HUD, menú, cámara)
├── Sprites/
│   ├── Boss/       Sprites del jefe por animación
│   ├── Character/  Sprites de Adela por animación
│   ├── Projectile/ Arma.png (sprite del disparo)
│   └── UI/         Logo, botones, corazones
├── Tiles/          Hojas de tiles, tiles (.asset), terreno y paleta
└── TextMesh Pro/   Importación estándar
```

---

## Configuración vital (dónde modificar cada cosa)

### Cámara — `Assets/Scenes/SampleScene.unity`, objeto **Main Camera**

| Qué | Dónde | Valor actual |
|---|---|---|
| Zoom / encuadre (ortho size) | Inspector → Camera → **Orthographic Size** | **6** (12 unidades de alto a la vista; menor = más zoom) |
| Posición inicial | Transform del Main Camera | `(0, -0.53, -10)` — en runtime la sigue el script |
| Sigue al jugador con desfase | `CameraController.offset` (Inspector del Main Camera) | **(2, 2)** → Adela queda en el tercio inferior-derecho |
| Objetivo | `CameraController.target` | Transform del objeto **Character** |
| Deriva del cielo | `CameraController` → `driftSpeed`, `driftRangeX/Y` | `0.3`, `5`, `0.8` |
| Fondo del cielo | `Assets/Resources/cielo.png` (cargado por código) | reemplazar el PNG cambia el fondo |

El script de cámara es `Assets/Scripts/CameraController.cs`: en `LateUpdate` coloca la cámara en `target.position + offset`, con `z = -10`.

### Jugador — objeto **Character**, script `Assets/Scripts/PlayerController.cs`

| Campo (Inspector) | Valor | Qué hace |
|---|---|---|
| `movementSpeed` | 10 | velocidad al caminar |
| `runSpeedMultiplier` | 1.6 | multiplicador con Shift |
| `jumpForce` | 20 | impulso del salto (velocidad inicial) |
| `maxJumpCount` | 2 | salto simple + doble salto |
| `groundDetectionRange` | 0.66 | longitud del raycast de suelo |
| `health` / `maxHealth` | 3 | corazones |
| `deathHeight` | -30 | por debajo de Y = -30 cuenta como caída al vacío |

- **Gravedad del jugador**: componente `Rigidbody2D` del Character → **Gravity Scale = 5** (con jumpForce 20 sube unas 4 unidades en ~0.4 s).
- **Gravedad global**: `ProjectSettings/Physics2DSettings.asset` (`-9.81`).

### Capas — `ProjectSettings/TagManager.asset`

| Índice | Capa | Uso |
|---|---|---|
| 6 | Character | jugador (máscara de detección de enemigos/boss) |
| 7 | Ground | tilemap; la máscara `groundMask` del jugador solo mira esta |
| 8 | Enemy | enemigos |
| 9 | Background | decoración de fondo |

### Animator — `Assets/Animations/`

`Character.controller` usa los parámetros que pone `PlayerController.cs`: `isMoving`, `isRunning`, `isGrounded` (bool), `attack`, `takeDamage` (trigger) e `isDead` (bool).
`Boss.controller` usa `isMoving`, `isRunning` y `attack`, que pone `Boss.cs`.

---

## Cómo abrirlo

1. Abrir el proyecto en **Unity Hub** (2022.3 LTS o superior).
2. Abrir la escena `Assets/Scenes/Menu.unity`.
3. **Play** ▶
