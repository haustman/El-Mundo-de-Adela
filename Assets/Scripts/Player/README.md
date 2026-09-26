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

### Notas
- Es la clase más referenciada del proyecto: `Enemy`, `EnemyShooter`, `Boss`, `ProjectileEnemy`, `Heart` y `FinishPoint` lo detectan con `TryGetComponent<PlayerController>` para saber cuándo interactúan con el jugador.
- Al morir o ganar recarga escenas por índice/nombre (`SceneManager`).
