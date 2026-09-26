# Level/

### Qué hay
| Script | Qué es |
|---|---|
| **CameraController.cs** | Sigue al jugador (mueve la cámara detrás). |
| **FinishPoint.cs** | Meta del nivel. |
| **Heart.cs** | Corazón recolectable que cura al jugador. |

### Para qué sirven
- `CameraController` coloca la cámara en `LateUpdate` en `target + offset` (con `z = -10`) y en `Awake` aplica `backgroundColor`. Con `smoothing = 0` el seguimiento es rígido; con un valor mayor se suaviza. El fondo visible es el objeto `Fondo para juego` (hijo de la cámara, sprite `Sprites/UI/Fondo.png`), no `cielo.png`.
- `FinishPoint` detecta al jugador y llama a `PlayerController.Finish()` para completar el nivel.
- `Heart` detecta al jugador y le devuelve `healAmount` de vida.

### Campos en el Inspector

| Script | Campo | Valor por defecto | Qué hace |
|---|---|---|---|
| `CameraController` | `target` | Character | a quién sigue |
| `CameraController` | `offset` | (2, 2) | desplazamiento respecto al objetivo |
| `CameraController` | `smoothing` | 0 | suavizado del seguimiento (0 = rígido) |
| `CameraController` | `backgroundColor` | azul cielo | color de fondo de la cámara |
| `Heart` | `healAmount` | 1 | vida que cura |
| `Heart` | `rotationSpeed` | 200 | velocidad de giro |
| `FinishPoint` | `destroyOnFinish` | true | destruye la meta al alcanzarla |

### Escena donde se usan
- **Nivel_1.unity** → `Main Camera` (CameraController) y `Hearts` (Heart).
- **FinishPoint** no tiene instancia en el nivel actual (listo para colocar la meta).

### Nota
Los tres son objetos/elementos del **mundo del nivel** (no UI en pantalla), por eso están juntos aquí y no en `UI/`.
