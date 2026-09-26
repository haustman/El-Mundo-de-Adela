# Projectiles/

### Qué hay
- **Projectile.cs** — Bala disparada por el **jugador**.
- **ProjectileEnemy.cs** — Bala disparada por los **enemigos**.

### Para qué sirven
- `Projectile` al chocar busca un `Damageable` y le resta vida → así daña a cualquier enemigo/jefe sin acoplarse a un tipo concreto.
- `ProjectileEnemy` al chocar busca un `PlayerController` y le resta vida al jugador.

### Campos configurables (en el prefab)

| Campo | Qué hace |
|---|---|
| `speed` | velocidad de avance |
| `lifeDuration` | segundos antes de destruirse sola |
| `detectionLayer` | a qué capa detecta como objetivo |
| `detectionRadius` | radio de detección del impacto |
| `shape` (solo `Projectile`) | sprite que se espeja según el sentido |

### Escena donde se usan
- **Nivel_1.unity** — no están en la escena directamente: viven en los **prefabs** `Assets/Prefab/Bullet 1.prefab`, `Bullet 2.prefab` (jugador) y `BulletEnemy.prefab` (enemigo), que se instancian en tiempo de ejecución con `Instantiate`.

### Flujo
```
PlayerController ──Instantiate──▶ Bullet 1/2 (Projectile) ──▶ golpea a Damageable (enemigos)
EnemyShooter     ──Instantiate──▶ BulletEnemy (ProjectileEnemy) ──▶ golpea a PlayerController
```
