# Scripts

Todos los scripts C# del juego. Cada uno tiene una sola responsabilidad.

### Jugador
- **CharacterController.cs** — Movimiento, salto, disparo, vida y animación del jugador.
- **CameraController.cs** — Sigue al jugador y carga el fondo del cielo.

### Enemigos
- **Enemy.cs** — Enemigo cuerpo a cuerpo. Patrulla por una ruta y daña al jugador al tocarlo.
- **EnemyShooter.cs** — Enemigo a distancia. Patrulla y dispara.
- **PatrolRoute.cs** — Lógica de patrulla reutilizable (interpolación entre puntos).

### Combate
- **Damageable.cs** — Clase base para todo lo que tiene vida y puede recibir daño.
- **Projectile.cs** — Proyectil del jugador.
- **ProjectileEnemy.cs** — Proyectil enemigo.

### UI
- **HUDController.cs** — Corazones de vida del jugador.
- **HealthBarController.cs** — Barra de vida reutilizable (para enemigos).
- **EnemyHUDController.cs** — Barra de vida de enemigos cuerpo a cuerpo.
- **EnemyShooterHUDController.cs** — Barra de vida de enemigos a distancia.

### Objetos
- **Heart.cs** — Corazón recogible que cura al jugador.
- **FinishPoint.cs** — Meta del nivel.
