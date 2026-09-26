# Enemies/

### Qué hay
| Script | Qué es |
|---|---|
| **Damageable.cs** | Clase *abstracta* base: todo lo que tiene vida y puede recibir daño. |
| **Enemy.cs** | Enemigo cuerpo a cuerpo (hereda de `Damageable`). |
| **EnemyShooter.cs** | Enemigo a distancia (hereda de `Damageable`). |
| **Boss.cs** | Jefe: patrulla opcional + ataque en rango con animación (hereda de `Damageable`). |
| **PatrolRoute.cs** | Lógica de patrulla reutilizable. Define también el tipo serializable `PatrolMovement` (punto + duración). |

### Para qué sirven
- **Damageable** centraliza el sistema de vida/daño: `Projectile` (bala del jugador) golpea a cualquier `Damageable` sin importar qué tipo sea.
- **Enemy / EnemyShooter / Boss** patrullan y dañan al jugador al tocarlo o disparándole.
- **PatrolRoute** se reutiliza en cualquier enemigo que patrulle entre puntos.

### Campos de Damageable (inspector)

| Campo | Qué hace |
|---|---|
| `health` | vida inicial (el máximo se toma de aquí) |
| `destroyOnDeath` | destruye el objeto al quedarse sin vida |

La vida se lee desde fuera con las propiedades `Health` y `MaxHealth` (solo lectura): los campos ya no son públicos. El nombre serializado `Health` se conserva con `[FormerlySerializedAs]`, así que los valores de la escena no se pierden.

### Escena donde se usa
- **Nivel_1.unity** → `Enemy (1)`, `EnemyShooter`, `EnemyShooter (1..3)` + sus rutas `Patrol1`.
- **Boss** no tiene instancia en el nivel actual (listo para usarlo cuando aparezca un jefe).

### Jerarquía de herencia
```
Damageable (abstracto)
├── Enemy
├── EnemyShooter
└── Boss
```
