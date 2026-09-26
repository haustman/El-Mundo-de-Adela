# Assets

Carpeta raíz de todos los assets del proyecto Unity.

```
Assets/
├── Animations/    Clips y controller del Animator
├── Plugins/       Vacío (reservado)
├── Prefab/        Prefabs de proyectiles
├── Resources/     Assets que se cargan desde código (cielo.png, sin uso actual)
├── Scenes/        Escenas (Menu + Niveles + Nivel_1)
├── Scripts/       Código C# (18 scripts) organizado por tipo
│   ├── Player/      PlayerController
│   ├── Enemies/     Damageable, Enemy, EnemyShooter, Boss, PatrolRoute
│   ├── Projectiles/ Projectile, ProjectileEnemy
│   ├── UI/HUD/      HUD y barras de vida
│   ├── UI/Menu/     Menu, MenuEffects
│   ├── Level/       CameraController, FinishPoint, Heart
│   └── SelectorNivel/ SelectorNivelController
├── Sprites/       Imágenes y spritesheets
│   ├── Boss/        Sprites del jefe por animación
│   ├── Character/   Sprite del jugador por animación
│   ├── Hazards/     Assets CC0 de pinchos (sin implementar)
│   ├── Projectile/  Sprite del disparo (Arma.png)
│   └── UI/          Logo y corazones (en uso), Fondo.png (nivel); Jugar/Salir.png y Fondo para juego.jpg sin uso
├── TextMesh Pro/  Importación estándar de TMP
└── Tiles/         Tiles, tilemaps y paleta
```

No crear carpetas nuevas sin sentido. Si algo no entra en las carpetas existentes, preguntar antes de crear una nueva.
