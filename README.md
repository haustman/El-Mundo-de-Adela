# Unity 2D Platformer

Un juego 2D tipo platformer hecho con **Unity**. Corre, salta y derrota enemigos mientras recolectas corazones.
Plantilla perfecta para un sidescroller.

## Features
- Control del jugador (correr / saltar / disparar)
- Enemigos cuerpo a distancia con barra de vida
- Corazones recolectables (vida)
- Animaciones: idle, walk, run, jump, attack, danger, death
- Pantalla de menú con logo animado y transición al juego
- Fondo de cielo con efecto parallax
- Optimizado para móvil y PC de bajos recursos

## Controles
- **Mover:** Flechas ← →
- **Saltar:** Espacio
- **Correr:** Shift izquierdo
- **Disparar:** Click izquierdo
- **Reiniciar:** R

## Estructura del Proyecto
```
Assets/
├── Animations/        Clips .anim + Character.controller
├── Prefab/            Prefabs de proyectiles
├── Resources/         Assets cargados desde código (cielo.png)
├── Scenes/            Menu.unity (índice 0) + SampleScene.unity (índice 1)
├── Scripts/           16 scripts C# organizados por categoría
├── Sprites/
│   ├── Character/     Sprite del jugador por animación
│   │   ├── Attack/    5 frames
│   │   ├── Danger/    Spritesheet de daño (3 frames)
│   │   ├── Death/     Spritesheet de muerte (4 frames)
│   │   ├── Idle/      4 frames
│   │   ├── Jump/      7 frames
│   │   ├── Run/       4 frames
│   │   └── Walk/      4 frames
│   └── UI/            Logo, botones, corazones
├── TextMesh Pro/      Importación estándar de TMP
└── Tiles/             Tilesets y paleta de tiles
    ├── Terrain/       Tiles de terreno
    ├── TilePalette/   Paleta para pintar
    └── Tiles/         Assets de tile generados
```

## Getting Started
1. **Clonar**
   ```bash
   git clone https://github.com/Arash-ra03/Unity-2D-Platformer
   cd Unity-2D-Platformer
   ```
2. **Abrir en Unity Hub** (2022 LTS o superior)
3. Abrir la escena: `Assets/Scenes/Menu.unity`
4. **Play** ▶️

## Escenas
| Escena | Índice | Descripción |
|--------|--------|-------------|
| `Menu.unity` | 0 | Pantalla principal con logo, Jugar y Salir |
| `SampleScene.unity` | 1 | Nivel de juego |

---
