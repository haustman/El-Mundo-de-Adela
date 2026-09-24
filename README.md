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
- **Cámara**: sigue a Adela con encuadre en el tercio inferior de la pantalla.
- **Fondo**: imagen `Fondo para juego` como hijo de la cámara (sorting −100).
- **HUD**: corazones de vida.
- **Arte**: hoja de botones troceada en 72 sprites (claro/medio/oscuro), lista para usarse.

### Qué hay realmente colocado en `SampleScene`

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
| `Fondo.png` | fondo decorativo del menú/nivel |
| `button ver 2 (785x271).png` | hoja de botones de UI |

El fondo del nivel no está en `Tiles/`: es `Assets/Sprites/UI/Fondo para juego.jpg` (objeto `Fondo para juego` en la escena). Los assets de pinchos sin usar están en `Assets/Sprites/Hazards/`.

### Subcarpetas

- **`Tiles/Tiles/`** → un `.asset` por sprite usado en el nivel. El nombre sigue el patrón `<hoja>_N`. Hoy hay la serie completa de `tilemap(18X18)` / `tilemap-backgrounds` / `tilemap-characters`, y solo unos pocos `Elementos_N` (3 de 70) y `Tileset_N` (2 de 8): se generan al pintar o al crear tiles desde el Sprite Editor.
- **`Tiles/Terrain/`** → tiles de terreno individuales (`Terrain.asset`, `Terrain_Fill.asset`, `Terrain_RimTop.asset`, etc.), con sus PNGs en `Terrain/Sprites/`.
- **`Tiles/TilePalette/New Palette.prefab`** → la paleta que usa el Paint Tool para pintar el tilemap.

Si cambias o recortas una hoja, hay que volver a cortar los sprites en el Sprite Editor y regenerar sus `.asset`.

---

## Los scripts

Están todos en `Assets/Scripts/`. Son **17** y cada uno hace una sola cosa.

### Jugador
- **PlayerController.cs** — Todo el jugador: mover, correr, salto y doble salto, disparar, vida, invulnerabilidad con parpadeo, caída al vacío (resta 1 corazón y reaparece), muerte y `Finish()` para la meta.
- **CameraController.cs** — Sigue al jugador con un desfase (`offset`) y fija un color de fondo de cámara. El fondo visible es el sprite `Fondo para juego` (hijo de la cámara), no `cielo.png`.

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
- **Heart.cs** — Corazón que se recoge y cura 1. Gira sobre sí mismo. *(Hay 1 instancia: `Hearts` en SampleScene.)*
- **FinishPoint.cs** — La meta del nivel: al tocarla llama a `PlayerController.Finish()`. *Sin instancia en el nivel.*

---

## Estructura de carpetas

```
Assets/
├── Animations/     Clips .anim + Character.controller y Boss.controller
├── Plugins/        Vacío (reservado)
├── Prefab/         Prefabs de proyectiles (Bullet 1, Bullet 2, BulletEnemy)
├── Resources/      Resources.Load (cielo.png, hoy sin uso en escena)
├── Scenes/         Menu.unity + SampleScene.unity
├── Scripts/        17 scripts C# (jugador, enemigos, jefe, HUD, menú, cámara)
├── Sprites/
│   ├── Boss/       Sprites del jefe por animación
│   ├── Character/  Sprites de Adela por animación
│   ├── Hazards/    Assets CC0 de pinchos (sin implementar)
│   ├── Projectile/ Arma.png (sprite del disparo)
│   └── UI/         Logo, botones, corazones, Fondo para juego.jpg
├── Tiles/          Hojas de tiles, tiles (.asset), terreno y paleta
└── TextMesh Pro/   Importación estándar
```

---

## Configuración vital (dónde modificar cada cosa)

### Cámara — `Assets/Scenes/SampleScene.unity`, objeto **Main Camera**

| Qué | Dónde | Valor actual |
|---|---|---|
| Zoom / encuadre (ortho size) | Inspector → Camera → **Orthographic Size** | **6** (12 unidades de alto a la vista; menor = más zoom) |
| Posición inicial | Transform del Main Camera | `(2.06, -11.37, -10)` — en runtime la sigue el script |
| Sigue al jugador con desfase | `CameraController.offset` (Inspector del Main Camera) | **(2, 2)** → Adela queda en el tercio inferior-derecho |
| Objetivo | `CameraController.target` | Transform del objeto **Character** |
| Color de fondo de la cámara | `CameraController.Awake` (código) | Azul cielo fijo `(0.53, 0.81, 0.98)` |
| Fondo visible | Hijo de la cámara: **Fondo para juego** | Sprite `Sprites/UI/Fondo para juego.jpg`, escala 2×2, local `(0,0,10)`, sorting **−100** |

El script de cámara es `Assets/Scripts/CameraController.cs`: en `LateUpdate` coloca la cámara en `target.position + offset`, con `z = -10`. No carga `cielo.png` ni tiene deriva/parallax.

### Jugador — objeto **Character**, script `Assets/Scripts/PlayerController.cs`

| Campo (Inspector) | Valor | Qué hace |
|---|---|---|
| `movementSpeed` | 10 | velocidad al caminar |
| `runSpeedMultiplier` | 1.6 | multiplicador con Shift |
| `jumpForce` | 20 | impulso del salto (velocidad inicial) |
| `maxJumpCount` | 2 | salto simple + doble salto |
| `groundDetectionRange` | 0.66 | longitud del raycast de suelo |
| `health` / `maxHealth` | 3 | corazones |
| `deathHeight` | -36 | por debajo de Y = -36 cuenta como caída al vacío |

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
