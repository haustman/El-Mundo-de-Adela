# SelectorNivel/

### Qué hay
- **SelectorNivelController.cs** — Lógica del selector de niveles. La interfaz (panel, botones, orden, colores, fuente) **se arma y se edita en la escena** `Niveles.unity`; este script solo la conecta.

### Escena donde se usa
- **Niveles.unity** (índice 1 en Build Settings). La UI vive en el `Canvas` de la escena: `Fondo`, `Panel` (con `VerticalLayoutGroup`) → `Titulo`, `Nivel 1`…`Nivel 5`, `Espacio`, `Volver`. El script cuelga del objeto raíz **Selector de Niveles** y apunta a los botones por referencia en el Inspector.

### Qué hace
- Al arrancar enlaza cada botón de `botonesNivel` con la escena de `levelSceneNames` en la misma posición: `onClick → SceneManager.LoadScene`.
- Comprueba cada escena con `Application.CanStreamedLevelBeLoaded`: las que están en Build Settings salen activas; las demás salen deshabilitadas (`interactable = false`) con el aviso **"próximamente"** añadido al texto. Al agregar una escena nueva a Build Settings se activa sola, sin tocar código.
- **Volver** o tecla **Esc** → carga la escena `menuSceneName` (por defecto `Menu`).
- El primer nivel disponible queda seleccionado al arrancar (se puede navegar con flechas/Tab y activar con Enter).

### Campos en el Inspector

| Campo | Valor por defecto | Qué es |
|---|---|---|
| `menuSceneName` | `Menu` | escena a la que vuelven **Volver** y **Esc** |
| `levelSceneNames` | `Nivel_1` … `Nivel_5` | un nombre de escena por nivel, en orden |
| `botonesNivel` | 5 botones | referencias a los botones de la escena, en el mismo orden que `levelSceneNames` |
| `botonVolver` | `Volver` | botón de vuelta al menú |

### Cómo retocar la UI (en el editor)
Todo se edita con el ratón sobre la escena `Niveles.unity`: tamaño del panel y posiciones, orden de los hijos del `Panel` (el `VerticalLayoutGroup` los apila), alturas vía `Layout Element → Preferred Height`, imagen de fondo (`Fondo` usa `Tiles/Fondo.png`), colores del `Image`, texto/tamaño/fuente/color de cada `TextMeshProUGUI` y estados de los `Button` (transition = **Sprite Swap**: normal `pill_light_6`, hover/selección `pill_mid_3`, pulsado/deshabilitado `pill_dark_3`, del sheet `button ver 2 (785x271).png`). **Atención:** al reordenar o agregar botones hay que reasignar `botonesNivel` en el objeto *Selector de Niveles* respetando el orden de `levelSceneNames`.

### Para agregar un nivel nuevo
1. Crear la escena `Assets/Scenes/Nivel_2.unity` (mismo patrón que `Nivel_1`).
2. Agregarla en **File → Build Settings**.
3. Duplicar un botón existente dentro del `Panel`, ponerle el texto y agregarlo a `botonesNivel`; añadir `Nivel_2` a `levelSceneNames` en la misma posición. Ya sale activa, siempre que los nombres coincidan.
