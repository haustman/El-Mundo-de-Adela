# Scripts

Todos los scripts C# del juego (**18 en total**), organizados por tipo (dominio), no por escena: así los scripts compartidos no hay que moverlos cada vez que se crea un nivel nuevo.

> **Regla de oro:** los `.cs` se mueven siempre **junto con su `.meta`** y con Unity cerrado. Unity identifica los scripts por el GUID del `.meta`, no por la ruta → escenas y prefabs no se rompen.

## Estructura

```
Assets/Scripts/
├── Player/          PlayerController.cs
├── Enemies/         Damageable, Enemy, EnemyShooter, Boss, PatrolRoute
├── Projectiles/     Projectile, ProjectileEnemy
├── UI/
│   ├── HUD/         HUDController, HealthBarController,
│   │                EnemyHUDController, EnemyShooterHUDController
│   └── Menu/        Menu, MenuEffects
├── Level/           CameraController, FinishPoint, Heart
└── SelectorNivel/   SelectorNivelController
```

Cada carpeta tiene su propio `README.md` explicando qué hay, para qué sirve y en qué escena se usa.

## Mapa de flujo: qué script va en qué escena

### 🎬 Menu.unity (menú principal)
```
Canvas  ←─────────────── MenuEffects.cs (está en el Canvas)
│                         fade logo, hover botones, transición a negro
│                         └── carga escena "Niveles" (SceneManager)
└── Menu (objeto) ←───── Menu.cs + Image de fondo (Tiles/Fondo.png)
    ├── Image ── logo (fade-in + titileo, animado por MenuEffects)
    ├── Jugar ──▶ Menu.Play() ──▶ MenuEffects.StartTransition("Niveles")
    └── Salir ──▶ Menu.Quit()
```

### 🗂️ Niveles.unity (selector de niveles)
```
Canvas (UI armada en la escena, se edita con el ratón)
└── Panel ── VerticalLayoutGroup ── Titulo, Nivel 1..5, Espacio, Volver

Selector de Niveles ◄── SelectorNivelController.cs (objeto raíz de la escena)
│                         enlaza botonesNivel ↔ levelSceneNames y comprueba con
│                         Application.CanStreamedLevelBeLoaded qué niveles existen
├── Nivel 1 ──▶ LoadScene("Nivel_1") ──▶ SceneManager.LoadScene
├── Nivel 2..5 ──▶ "próximamente" (botón deshabilitado: su escena aún no está en Build Settings)
└── Volver / Esc ──▶ LoadScene("Menu") ──▶ vuelve al menú
```

### 🎮 Nivel_1.unity (nivel jugable)
```
Character (jugador)
└── PlayerController.cs ──► dispara Projectile, muere/gana, recarga escena
        ▲   ▲   ▲
        │   │   └── FinishPoint.cs (meta) y Heart.cs (cura) lo detectan
        │   └────── ProjectileEnemy.cs (bala enemiga) lo daña
        └────────── Enemy / EnemyShooter / Boss (cuerpo a cuerpo) lo dañan

Main Camera
└── CameraController.cs ──► sigue al jugador LateUpdate + color de fondo

Enemigos (Enemy, EnemyShooter, Boss)
├── heredan de ▶ Damageable.cs (vida/daño)
├── patrullan con ▶ PatrolRoute.cs (puntos Patrol1)
├── EnemyShooter instancia ▶ ProjectileEnemy.cs (prefab BulletEnemy)
└── cada uno tiene su barra ▶ EnemyHUDController / EnemyShooterHUDController
                              (heredan de HealthBarController)

Jugador dispara ▶ Projectile.cs (prefabs Bullet 1 / Bullet 2)
                     └── al impactar busca un Damageable y lo daña

HUD del jugador
└── HUDController.cs ──► muestra los corazones de vida
```

## Herencias (lo único que "cruza" carpetas)

```
Damageable (Enemies/)          HealthBarController (UI/HUD/)
├── Enemy (Enemies/)           ├── EnemyHUDController (UI/HUD/)
├── EnemyShooter (Enemies/)    └── EnemyShooterHUDController (UI/HUD/)
└── Boss (Enemies/)
```

Mover archivos **no rompe estas herencias**: C# compila todos los `.cs` de `Assets/` en el mismo ensamblado (`Assembly-CSharp`) sin importar la subcarpeta, y las clases se referencian por nombre, nunca por ruta.

## Qué scripts NO están en ninguna escena todavía

| Script | Estado |
|---|---|
| `Boss.cs` | Listo, sin instancia en el nivel |
| `FinishPoint.cs` | Listo, sin instancia (colocar la meta) |
