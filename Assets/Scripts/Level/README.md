# Level/

### Qué hay
| Script | Qué es |
|---|---|
| **CameraController.cs** | Sigue al jugador (mueve la cámara detrás). |
| **FinishPoint.cs** | Meta del nivel. |
| **Heart.cs** | Corazón recolectable que cura al jugador. |

### Para qué sirven
- `CameraController` mueve la cámara en `LateUpdate` hacia `target + offset` (con `z = -10`) y en `Awake` fija el color de fondo. El fondo visible es el objeto `Fondo para juego` (hijo de la cámara, sprite `Sprites/UI/Fondo.png`), no `cielo.png`.
- `FinishPoint` detecta al jugador y llama a `PlayerController.Finish()` para completar el nivel.
- `Heart` detecta al jugador y le devuelve vida.

### Escena donde se usan
- **Nivel_1.unity** → `Main Camera` (CameraController) y `Hearts` (Heart).
- **FinishPoint** no tiene instancia en el nivel actual (listo para colocar la meta).

### Nota
Los tres son objetos/elementos del **mundo del nivel** (no UI en pantalla), por eso están juntos aquí y no en `UI/`.
