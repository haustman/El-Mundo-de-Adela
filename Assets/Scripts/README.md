# Scripts

Todos los scripts C# del juego (17 en total). Cada uno tiene una sola responsabilidad.

### Jugador
- **PlayerController.cs** — Movimiento, salto, disparo, vida y animación del jugador. Maneja la caída al vacío, la secuencia de muerte y la llegada a la meta.
- **CameraController.cs** — Sigue al jugador con un desfase (`offset`) y fija el color de fondo de la cámara. El fondo visible es el sprite `Fondo para juego` (hijo de la cámara), no `cielo.png`.

### Enemigos
- **Enemy.cs** — Enemigo cuerpo a cuerpo. Patrulla y daña al tocar.
- **EnemyShooter.cs** — Enemigo a distancia. Patrulla y dispara.
- **Boss.cs** — Jefe: patrulla opcional, ataca al entrar en rango con animación de ataque. *(Sin instancia en el nivel actual.)*
- **PatrolRoute.cs** — Lógica de patrulla reutilizable (interpolación entre puntos). Define también el tipo serializable `PatrolMovement` (punto + duración).

### Combate
- **Damageable.cs** — Clase base para todo lo que tiene vida y puede recibir daño.
- **Projectile.cs** — Proyectil del jugador.
- **ProjectileEnemy.cs** — Proyectil enemigo.

### UI
- **HUDController.cs** — Corazones de vida del jugador.
- **HealthBarController.cs** — Barra de vida reutilizable (para enemigos).
- **EnemyHUDController.cs** — Barra de vida de enemigos cuerpo a cuerpo.
- **EnemyShooterHUDController.cs** — Barra de vida de enemigos a distancia.

Las dos son subclases vacías de `HealthBarController`: no añaden lógica, solo existen para distinguir una barra de otra en el Inspector.

### Menú
- **Menu.cs** — Lógica del menú principal (Jugar y Salir).
- **MenuEffects.cs** — Efectos del menú: fade-in del logo con parpadeo, sombra en botones al hover, transición con fade a negro.

### Objetos
- **Heart.cs** — Corazón recolectable que cura al jugador. *(Hay 1 instancia: `Hearts` en SampleScene.)*
- **FinishPoint.cs** — Meta del nivel. Llama a `PlayerController.Finish()`, que recarga el nivel. *(Sin instancia en el nivel.)*
