# UI/HUD/

### Qué hay
| Script | Qué es |
|---|---|
| **HUDController.cs** | Corazones de vida del **jugador**. |
| **HealthBarController.cs** | Barra de vida reutilizable (base). |
| **EnemyHUDController.cs** | Barra de vida de enemigos cuerpo a cuerpo. |
| **EnemyShooterHUDController.cs** | Barra de vida de enemigos a distancia. |

### Para qué sirven
- `HUDController` muestra la vida del jugador en pantalla.
- `HealthBarController` dibuja una barra de vida genérica que sigue a su enemigo.
- `EnemyHUDController` y `EnemyShooterHUDController` son **subclases vacías** (`{ }`): no añaden lógica, solo existen para distinguir cada tipo de barra en el Inspector.

### Escena donde se usan
- **Nivel_1.unity** → Canvas `HUD` (jugador) y los HUDs hijos de cada enemigo (`Enemy (1)`, `EnemyShooter...`).

### Jerarquía de herencia
```
HealthBarController
├── EnemyHUDController
└── EnemyShooterHUDController
```
