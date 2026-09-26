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
- **Recogibles**: corazones que curan (hasta el máximo de 3). *(Hay 1 corazón colocado en el nivel.)*
- **Meta**: al tocarla se acaba el nivel y este vuelve a empezar. *(Código listo; todavía no hay ninguna colocada en el nivel.)*
- **Menú**: logo con fade-in y titileo, botones con hover y sombra, fundido a negro al entrar al juego.
- **Selector de niveles**: pantalla con un botón por nivel (**Nivel 1** carga el nivel; los demás salen "próximamente") y vuelta al menú con **Volver** o **Esc**.
- **Cámara**: sigue a Adela con encuadre en el tercio inferior de la pantalla.
- **Fondo**: imagen `Fondo para juego` como hijo de la cámara (sorting −100).
- **HUD**: corazones de vida.
- **Arte**: hoja de botones troceada en 72 sprites (claro/medio/oscuro), lista para usarse.

### Qué hay realmente colocado en `Nivel_1`

- **Sí**: jugador, cámara (con `Fondo para juego`), tilemap de suelo, tilemaps de decoración `Trees_Back`/`Trees_Front` (vacíos, sin colliders), **1** enemigo cuerpo a cuerpo (`Enemy (1)`), **4** enemigos a distancia (`EnemyShooter`, `(1)`, `(2)`, `(3)`, cada uno con su barra de vida), **1 corazón** (`Hearts`) y el HUD de corazones.
- **No** (el código existe, pero no hay instancia en el nivel): **meta** y **jefe**. Hasta que se coloquen, estas piezas no aparecen en partida.

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
2. **Jugar** hace un fundido a negro y carga **Niveles.unity** (índice 1): el selector de niveles.
3. En el selector, **Nivel 1** carga **Nivel_1.unity** (índice 2). Los niveles 2–5 salen "próximamente" hasta que sus escenas existan en Build Settings; **Volver** o **Esc** regresan al menú.
4. En el nivel, Adela corre, salta y dispara. Hay enemigos de dos tipos.
5. Si **cae al vacío**, pierde 1 corazón y vuelve al punto de inicio.
6. Si **se queda sin corazones**, muere y vuelve al menú.
7. La **meta** (cuando se coloque) recarga el nivel.

---

## Las escenas

### `Assets/Scenes/Menu.unity` — índice 0

Es la pantalla de inicio. Dentro hay:

- Un **Canvas**, con `MenuEffects.cs` colocado en el propio Canvas (fade-in y titileo del logo, escala y sombra en los botones al pasar el ratón, fundido a negro al jugar), y dentro el objeto **Menu**:
  - **Image** (componente del objeto Menu) → fondo de la pantalla, sprite `Assets/Tiles/Fondo.png` (1920×1080).
  - **Menu.cs** (también en el objeto Menu) → gestiona los botones.
  - **Image** (hijo) → el logo: aparece con fade-in y después titila.
  - **Jugar** → botón; al pulsarlo llama a `Menu.Play()` (fundido y carga `Niveles`).
  - **Salir** → botón; al pulsarlo llama a `Menu.Quit()`.
  - Cada botón lleva su **Text (TMP)** con la etiqueta.
- **EventSystem** → hace falta para que los botones reciban clics y hover.
- **Main Camera** → la cámara de la pantalla de menú.
- Scripts: **Menu.cs** y **MenuEffects.cs**.

### `Assets/Scenes/Niveles.unity` — índice 1

Es el **selector de niveles**: la pantalla desde la que se elige qué nivel jugar. Dentro hay:

- **Main Camera** → sin `CameraController` (no sigue a nadie); su único trabajo es la `AudioListener`.
- **EventSystem** → para que los botones reciban clics y hover.
- **Canvas** → la UI está **armada en la escena** (se edita con el ratón): `Fondo`, `Panel` con `VerticalLayoutGroup` → `Titulo` "SELECCIONA UN NIVEL", `Nivel 1`…`Nivel 5`, `Espacio` y `Volver`.
- **Selector de Niveles** → objeto con `SelectorNivelController.cs`. Solo conecta la UI: enlaza cada botón con su escena (`botonesNivel` + `levelSceneNames`) y comprueba con `Application.CanStreamedLevelBeLoaded` qué escenas existen en Build Settings: las que no están salen deshabilitadas con el aviso "próximamente". **Volver** o **Esc** regresan al menú. Listas y referencias se editan en el Inspector (por defecto `Nivel_1` … `Nivel_5`).

### `Assets/Scenes/Nivel_1.unity` — índice 2

Es el nivel. Dentro hay:

- **Main Camera** → con `CameraController`; sigue al jugador. Como hijo lleva el sprite **Fondo para juego** (sorting −100).
- **Character** → el jugador, con `PlayerController`, un `Rigidbody2D` y su `Animator`.
- **HUD** → con `HUDController`, dibuja los corazones de vida.
- **Grid** → con el tilemap de suelo (colliders) y los tilemaps de decoración **Trees_Back** (sorting −1) y **Trees_Front** (sorting +1), sin colliders (para caminar entre árboles).
- **Hearts** → 1 corazón recolectable (`Heart.cs`).
- **Enemy (1)** → enemigo cuerpo a cuerpo. Patrulla y tiene su `Slider` de vida (`EnemyHUDController`).
- **EnemyShooter**, **EnemyShooter (1)**, **EnemyShooter (2)** y **EnemyShooter (3)** → 4 enemigos a distancia, cada uno con su `Slider` (`EnemyShooterHUDController`) y `shootNum = 3`.
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
| `Fondo.png` | fondo decorativo (hay dos: este de `Tiles/`, 1920×1080, es el `Image` de fondo del menú; y `Sprites/UI/Fondo.png`, 1024×1536, es el fondo del nivel) |
| `button ver 2 (785x271).png` | hoja de botones de UI |

El fondo del nivel es el sprite `Assets/Sprites/UI/Fondo.png` (objeto `Fondo para juego` en la escena, hijo de la Main Camera); el antiguo `Fondo para juego.jpg` ya no se referencia en ninguna escena. `Assets/Tiles/Fondo.png` (1920×1080) es otra imagen distinta: el `Image` de fondo del menú. Los assets de pinchos sin usar están en `Assets/Sprites/Hazards/`.

### Subcarpetas

- **`Tiles/Tiles/`** → un `.asset` por sprite usado en el nivel. El nombre sigue el patrón `<hoja>_N`. Hoy hay la serie completa de `tilemap(18X18)_N` (180) y `tilemap-backgrounds(24X24)_N` (24), casi completa de `tilemap-characters(24X24)_N` (26 de 27, falta `_2`), y solo unos pocos `Elementos_N` (3 de 70) y `Tileset_N` (2 de 8): se generan al pintar o al crear tiles desde el Sprite Editor.
- **`Tiles/Terrain/`** → tiles de terreno individuales (`Terrain.asset`, `Terrain_Fill.asset`, `Terrain_RimTop.asset`, etc.), con sus PNGs en `Terrain/Sprites/`.
- **`Tiles/TilePalette/New Palette.prefab`** → la paleta que usa el Paint Tool para pintar el tilemap.

Si cambias o recortas una hoja, hay que volver a cortar los sprites en el Sprite Editor y regenerar sus `.asset`.

---

## Los scripts

Están organizados por tipo (dominio) en subcarpetas de `Assets/Scripts/` — **18 en total**, cada uno con una sola cosa. Todos viven en el namespace `ElMundoDeAdela`. Cada subcarpeta tiene su propio `README.md`, y el mapa de flujo completo (qué script va en qué escena y cómo se conectan) está en `Assets/Scripts/README.md`.

```
Assets/Scripts/
├── Player/        PlayerController.cs
├── Enemies/       Damageable, Enemy, EnemyShooter, Boss, PatrolRoute
├── Projectiles/   Projectile, ProjectileEnemy
├── UI/HUD/        HUDController, HealthBarController, EnemyHUD, EnemyShooterHUD
├── UI/Menu/       Menu, MenuEffects
├── Level/         CameraController, FinishPoint, Heart
└── SelectorNivel/ SelectorNivelController
```

### Player/
- **PlayerController.cs** — Todo el jugador: mover, correr, salto y doble salto, disparar, vida, invulnerabilidad con parpadeo, caída al vacío (resta 1 corazón y reaparece), muerte y `Finish()` para la meta.

### Enemies/
- **Damageable.cs** — Base de todo lo que tiene vida y recibe daño. Guarda `Health`/`MaxHealth`, aplica `GetHit` y prepara la barra la primera vez que hace falta. No se coloca en la escena: de aquí heredan los demás.
- **Enemy.cs** — Enemigo cuerpo a cuerpo. Recorre una ruta y hace daño al tocarte.
- **EnemyShooter.cs** — Enemigo a distancia. Recorre una ruta, dispara una vez por tramo y se gira al cerrar la vuelta.
- **Boss.cs** — Jefe. Patrulla si se le configura, mira al jugador y ataca cuando lo tiene cerca, con un tiempo de espera entre golpes. *Sin instancia en el nivel.*
- **PatrolRoute.cs** — Las piezas de la patrulla: `PatrolMovement` (un punto y cuánto tarda en recorrerlo) y `PatrolRoute` (recorre la lista en bucle interpolando).

### Projectiles/
- **Projectile.cs** — Proyectil del jugador: avanza en línea recta y hace daño al primero que toca (un `Damageable`).
- **ProjectileEnemy.cs** — Lo mismo, pero el que disparan los enemigos (busca al `PlayerController`).

### UI/HUD/
- **HUDController.cs** — Los corazones del jugador. Se puede llamar desde cualquier sitio con `HUDController.Refresh(vida)`.
- **HealthBarController.cs** — Barra de vida reutilizable. De aquí heredan las dos siguientes.
- **EnemyHUDController.cs** — Barra del enemigo cuerpo a cuerpo. Es una subclase vacía: solo existe para distinguirla en el Inspector.
- **EnemyShooterHUDController.cs** — Barra del enemigo a distancia. Igual, otra subclase vacía.

### UI/Menu/
- **Menu.cs** — Los dos botones del menú: `Play()` (Jugar) y `Quit()` (Salir).
- **MenuEffects.cs** — Los adornos del menú: fade-in y titileo del logo, sombra y escala en los botones al pasar el ratón, y el fundido a negro antes de entrar al juego.

### Level/
- **CameraController.cs** — Sigue al jugador con un desfase (`offset`) y fija un color de fondo de cámara. El fondo visible es el sprite `Fondo para juego` (hijo de la cámara), no `cielo.png`.
- **Heart.cs** — Corazón que se recoge y cura 1. Gira sobre sí mismo. *(Hay 1 instancia: `Hearts` en Nivel_1.)*
- **FinishPoint.cs** — La meta del nivel: al tocarla llama a `PlayerController.Finish()`. *Sin instancia en el nivel.*

---

## Estructura de carpetas

```
Assets/
├── Animations/     Clips .anim + Character.controller y Boss.controller
├── Plugins/        Vacío (reservado)
├── Prefab/         Prefabs de proyectiles (Bullet 1, Bullet 2, BulletEnemy)
├── Resources/      Resources.Load (cielo.png, hoy sin uso en escena)
├── Scenes/         Menu.unity + Niveles.unity (selector) + Nivel_1.unity
├── Scripts/        18 scripts C# organizados por tipo
│   ├── Player/       PlayerController
│   ├── Enemies/      Damageable, Enemy, EnemyShooter, Boss, PatrolRoute
│   ├── Projectiles/  Projectile, ProjectileEnemy
│   ├── UI/HUD/       HUDController, HealthBarController, EnemyHUD x2
│   ├── UI/Menu/      Menu, MenuEffects
│   ├── Level/        CameraController, FinishPoint, Heart
│   └── SelectorNivel/ SelectorNivelController
├── Sprites/
│   ├── Boss/       Sprites del jefe por animación
│   ├── Character/  Sprites de Adela por animación
│   ├── Hazards/    Assets CC0 de pinchos (sin implementar)
│   ├── Projectile/ Arma.png (sprite del disparo)
│   └── UI/         Logo, botones, corazones, Fondo.png y Fondo para juego.jpg
├── Tiles/          Hojas de tiles, tiles (.asset), terreno y paleta
└── TextMesh Pro/   Importación estándar
```

---

## Configuración vital (dónde modificar cada cosa)

### Cámara — `Assets/Scenes/Nivel_1.unity`, objeto **Main Camera**

| Qué | Dónde | Valor actual |
|---|---|---|
| Zoom / encuadre (ortho size) | Inspector → Camera → **Orthographic Size** | **6** (12 unidades de alto a la vista; menor = más zoom) |
| Posición inicial | Transform del Main Camera | `(3.2, -10.4, -10)` — en runtime la sigue el script |
| Sigue al jugador con desfase | `CameraController.offset` (Inspector del Main Camera) | **(2, 2)** → Adela queda en el tercio inferior-derecho |
| Suavizado del seguimiento | `CameraController.smoothing` | **0** = rígido; mayor = más suave |
| Objetivo | `CameraController.target` | Transform del objeto **Character** |
| Color de fondo de la cámara | `CameraController.backgroundColor` (Inspector) | Azul cielo `(0.53, 0.81, 0.98)` |
| Fondo visible | Hijo de la cámara: **Fondo para juego** | Sprite `Sprites/UI/Fondo.png`, escala `(1.72, 1.28, 0.25)`, local `(0.01, 1.18, 10)`, sorting **−100** |

El script de cámara es `Assets/Scripts/Level/CameraController.cs`: en `LateUpdate` coloca la cámara en `target.position + offset`, con `z = -10` (con `smoothing` opcional). El color de fondo sale del campo `backgroundColor` del Inspector, no del código. No carga `cielo.png` ni tiene deriva/parallax.

### Jugador — objeto **Character**, script `Assets/Scripts/Player/PlayerController.cs`

| Campo (Inspector) | Valor por defecto | Qué hace |
|---|---|---|
| `movementSpeed` | 10 | velocidad al caminar |
| `runSpeedMultiplier` | 1.6 | multiplicador con Shift |
| `runKey` | Left Shift | tecla para correr |
| `jumpForce` | 20 | impulso del salto (velocidad inicial) |
| `maxJumpCount` | 2 | salto simple + doble salto |
| `jumpKey` | Space | tecla de salto |
| `groundDetectionRange` | 0.66 | longitud del raycast de suelo |
| `damage` | 1 | daño de cada proyectil |
| `firePoint` | (vacío) | punto de salida del proyectil; vacío = centro del jugador |
| `shotDelay` | 0.13 | retardo entre la animación de ataque y la salida del proyectil (s) |
| `health` / `maxHealth` | 3 | corazones |
| `blinkCount` | 3 | parpadeos de invulnerabilidad tras un golpe |
| `blinkHalfPeriod` | 0.2 | duración de cada medio parpadeo (s) |
| `deathHeight` | -36 | por debajo de Y = -36 cuenta como caída al vacío |
| `deathAnimationDuration` | 0.5 | espera antes de volver al menú |
| `menuSceneName` | Menu | escena a la que vuelve al morir |
| `fallDamage` | 1 | corazones que resta caer al vacío |
| `respawnPoint` | (vacío) | dónde reaparece; vacío = posición inicial |

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

El ataque del jugador suelta el proyectil tras un retardo configurable (`shotDelay`), no al pulsar el botón, y las transiciones AnyState están ordenadas para que **muerte y daño tengan prioridad sobre el salto**. Detalles en `Assets/Animations/README.md`.

---

## Cómo abrirlo

1. Abrir el proyecto en **Unity Hub** (2022.3 LTS o superior).
2. Abrir la escena `Assets/Scenes/Menu.unity`.
3. **Play** ▶
