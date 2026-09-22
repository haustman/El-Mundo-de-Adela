# El Mundo de Adela

Juego de plataformas 2D en **Unity 2022.3.20f1**.

Adela recorre un mundo lleno de peligros y debe encontrar **5 libros**.
Cada libro que encuentra le revela **un secreto**.

---

## Estado actual

**Jugable, pero es un prototipo de un solo nivel.**
La historia (los 5 libros y sus secretos) **todavía no está implementada**; es lo siguiente en la lista.

### Lo que ya funciona

- **Movimiento**: correr (Shift izq.), salto y doble salto (Espacio), disparo (clic izq.).
- **Vida**: 3 corazones, invulnerabilidad con parpadeo tras recibir daño.
- **Muerte**: al quedarse sin vida o caer al vacío se vuelve al menú.
- **Enemigos**:
  - Cuerpo a cuerpo: patrulla por una ruta y hace daño al tocarte.
  - A distancia: patrulla y dispara.
  - Los dos tienen barra de vida y reciben daño de tus proyectiles.
- **Recogibles**: corazones que curan (hasta el máximo de 3).
- **Meta**: al tocarla se reinicia el nivel.
- **Menú**: logo con fade-in y titileo, botones con hover y sombra, transición al juego.
- **Cámara**: sigue a Adela con encuadre en el tercio inferior de la pantalla.
- **Cielo**: fondo con deriva suave para que parezca que se mueve.
- **HUD**: corazones de vida.
- **Arte**: hoja de botones troceada en 72 sprites (claro/medio/oscuro), lista para usarse.

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

---

## Escenas

| Escena | Índice | Qué es |
|---|---|---|
| `Assets/Scenes/Menu.unity` | 0 | Menú principal |
| `Assets/Scenes/SampleScene.unity` | 1 | Nivel de juego |

---

## Estructura del proyecto

```
Assets/
├── Animations/     Clips .anim + Character.controller
├── Prefab/         Prefabs de proyectiles (Bullet 1, Bullet 2, BulletEnemy)
├── Resources/      Assets cargados desde código (cielo.png)
├── Scenes/         Menu.unity + SampleScene.unity
├── Scripts/        17 scripts C# (jugador, enemigos, jefe, HUD, menú, cámara)
├── Sprites/
│   ├── Boss/       Sprites del jefe por animación
│   ├── Character/  Sprites de Adela por animación
│   ├── Projectile/ Arma.png (sprite del disparo)
│   └── UI/         Logo, botones, corazones
├── Tiles/          Tilesets + hoja de botones de UI
└── TextMesh Pro/   Importación estándar
```

---

## Configuración vital (dónde modificar cada cosa)

### Cámara — `Assets/Scenes/SampleScene.unity`, objeto **Main Camera**

| Qué | Dónde | Valor actual |
|---|---|---|
| Zoom / encuadre (ortho size) | Inspector → Camera → **Orthographic Size** (o `SampleScene.unity` línea ~1271) | **6** (12 unidades de alto a la vista; menor = más zoom) |
| Posición inicial | Transform del Main Camera | `(0, -0.53, -10)` — en runtime la sigue el script |
| Sigue al jugador con desfase | `CameraController.offset` (Inspector del Main Camera) | **(2, 2)** → Adela queda en el tercio inferior-derecho |
| Objetivo | `CameraController.target` | Transform del objeto **Character** |
| Deriva del cielo | `CameraController` → `driftSpeed`, `driftRangeX/Y` | `0.3`, `5`, `0.8` |
| Fondo del cielo | `Assets/Resources/cielo.png` (cargado por código) | reemplazar el PNG cambia el fondo |

El script de cámara es `Assets/Scripts/CameraController.cs`: en `LateUpdate` coloca la cámara en `target.position + offset`, con `z = -10`.

### Jugador — objeto **Character**, script `Assets/Scripts/CharacterController.cs`

| Campo (Inspector) | Valor | Qué hace |
|---|---|---|
| `movementSpeed` | 10 | velocidad al caminar |
| `runSpeedMultiplier` | 1.6 | multiplicador con Shift |
| `jumpForce` | 20 | impulso del salto (velocidad inicial) |
| `maxJumpCount` | 2 | salto simple + doble salto |
| `groundDetectionRange` | 0.66 | longitud del raycast de suelo |
| `health` / `maxHealth` | 3 | corazones |
| `deathHeight` | -30 | por debajo de Y=-30 cuenta como caída al vacío |

- **Gravedad del jugador**: componente `Rigidbody2D` del Character → **Gravity Scale = 5** (con jumpForce 20: sube ~4 unidades en ~0.4 s).
- **Gravedad global**: `ProjectSettings/Physics2DSettings.asset` (`-9.81`).

### Capas — `ProjectSettings/TagManager.asset`

| Índice | Capa | Uso |
|---|---|---|
| 6 | Character | jugador (máscara de detección de enemigos/boss) |
| 7 | Ground | tilemap; la máscara `groundMask` del jugador solo mira esta |
| 8 | Enemy | enemigos |
| 9 | Background | decoración de fondo |

### Animator — `Assets/Animations/Character.controller`

Parámetros (los pone `CharacterController.cs`): `isMoving`, `isRunning`, `isGrounded` (bool), `attack`, `takeDamage` (trigger), `isDead` (bool).
Boss: `Assets/Animations/Boss.controller` con `isMoving`, `isRunning`, `attack` (los pone `Boss.cs`).

---

## Cómo abrirlo

1. Abrir el proyecto en **Unity Hub** (2022.3 LTS o superior).
2. Abrir la escena `Assets/Scenes/Menu.unity`.
3. **Play** ▶
