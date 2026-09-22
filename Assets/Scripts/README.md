# Scripts

Todos los scripts C# del juego (17 en total). Cada uno tiene una sola responsabilidad.

### Jugador
- **CharacterController.cs** — Movimiento, salto, disparo, vida y animación del jugador. Maneja la secuencia de muerte con animación.
- **CameraController.cs** — Sigue al jugador, carga el fondo del cielo y aplica efecto parallax.

### Enemigos
- **Enemy.cs** — Enemigo cuerpo a cuerpo. Patrulla y daña al tocar.
- **EnemyShooter.cs** — Enemigo a distancia. Patrulla y dispara.
- **Boss.cs** — Jefe: patrulla opcional, ataca al entrar en rango con animación de ataque.
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

### Menú
- **Menu.cs** — Lógica del menú principal (Jugar y Salir).
- **MenuEffects.cs** — Efectos del menú: fade-in del logo con parpadeo, sombra en botones al hover, transición con fade a negro.

### Objetos
- **Heart.cs** — Corazón recolectable que cura al jugador.
- **FinishPoint.cs** — Meta del nivel.
