# Prefabs

Prefabs de proyectiles que se instancian cuando se dispara.

- **Bullet 1.prefab** — Proyectil del jugador (hacia la derecha).
- **Bullet 2.prefab** — Proyectil del jugador (hacia la izquierda, espejo del 1).
- **BulletEnemy.prefab** — Proyectil enemigo.

Los dos bullets del jugador existen porque cada uno apunta a un lado distinto. Se crean desde `PlayerController.Shoot()`.
