# Scenes

Escenas del juego.

- **Menu.unity** — Pantalla de menú principal (índice 0 en Build Settings). Logo con efecto de parpadeo, botones Jugar y Salir con sombra y escala al hover, transición con fade a negro al jugar (carga `Niveles`).
- **Niveles.unity** — Selector de niveles (índice 1 en Build Settings). Main Camera, EventSystem y un `Canvas` con la UI armada en la escena: `Fondo`, `Panel` (VerticalLayoutGroup) con `Titulo` "SELECCIONA UN NIVEL", `Nivel 1`…`Nivel 5`, `Espacio` y `Volver`. El objeto raíz **Selector de Niveles** tiene `SelectorNivelController.cs`, que enlaza los botones: carga la escena del nivel, marca como "próximamente" las que no existen y vuelve al menú con Volver/Esc.
- **Nivel_1.unity** — Nivel de juego (índice 2 en Build Settings). Jugador, enemigos, tilemaps, cámara, UI, etc.

Si agregás más escenas, acordate de agregarlas en Build Settings (el selector detecta solito cuáles existen).
