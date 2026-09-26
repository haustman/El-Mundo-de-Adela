# Player/

### Qué hay
- **PlayerController.cs** — Único script de la carpeta.

### Para qué sirve
Controla todo el personaje de Adela:
- Movimiento (caminar/correr), salto y disparo.
- Vida: recibe daño, muere y ejecuta su animación de muerte.
- Caída al vacío (respawn) y llegada a la meta (`Finish()`).

### Escena donde se usa
- **Nivel_1.unity** → objeto `Character`.

### Campos en el Inspector

| Campo | Valor por defecto | Qué hace |
|---|---|---|
| `movementSpeed` | 10 | velocidad al caminar |
| `runSpeedMultiplier` | 1.6 | multiplicador con Shift |
| `runKey` | Left Shift | tecla para correr |
| `groundDetectionRange` | 0.66 | longitud del raycast de suelo |
| `jumpForce` | 20 | impulso del salto |
| `maxJumpCount` | 2 | salto simple + doble salto |
| `jumpKey` | Space | tecla de salto |
| `projectilePrefabRight` / `Left` | Bullet 1 / Bullet 2 | prefabs de proyectil por sentido |
| `firePoint` | (vacío) | punto de salida del proyectil; vacío = centro del jugador |
| `damage` | 1 | daño de cada proyectil |
| `shotDelay` | 0.13 | retardo entre la animación de ataque y la salida del proyectil (s) |
| `health` / `maxHealth` | 3 | corazones |
| `blinkCount` | 3 | parpadeos de invulnerabilidad tras un golpe |
| `blinkHalfPeriod` | 0.2 | duración de cada medio parpadeo (s) |
| `deathHeight` | -36 | por debajo de Y = -36 cuenta como caída al vacío |
| `deathAnimationDuration` | 0.5 | espera antes de volver al menú |
| `menuSceneName` | Menu | escena a la que vuelve al morir |
| `fallDamage` | 1 | corazones que resta caer al vacío |
| `respawnPoint` | (vacío) | dónde reaparece; vacío = posición inicial |

### Notas
- Es la clase más referenciada del proyecto: `Enemy`, `EnemyShooter`, `Boss`, `ProjectileEnemy`, `Heart` y `FinishPoint` lo detectan con `TryGetComponent<PlayerController>` para saber cuándo interactúan con el jugador.
- El disparo sale `shotDelay` segundos después de arrancar el clip `Attack` (ver `Assets/Animations/README.md`), para sincronizarlo con la animación.
- Ojo: el `Animator` del jugador cuelga del hijo **`Circle`** (el que lleva el `SpriteRenderer`), no del objeto `Character`. Por eso no se usan Animation Events.
- Los parámetros del Animator se envían por `Animator.StringToHash`, no por cadena.
- Al morir o ganar recarga escenas por nombre (`SceneManager`).
