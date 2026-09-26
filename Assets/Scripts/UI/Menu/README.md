# UI/Menu/

### Qué hay
- **Menu.cs** — Lógica de los botones del menú principal (Jugar y Salir).
- **MenuEffects.cs** — Efectos visuales del menú.

### Para qué sirven
- `Menu` busca `MenuEffects` con `FindObjectOfType` y, al pulsar **Jugar**, lanza la transición y carga la escena del selector de niveles (`Niveles`). **Salir** cierra el juego/editor.
- `MenuEffects` anima la interfaz: fade-in del logo con parpadeo, sombra y escala en los botones al pasar el ratón, y transición con fade a negro antes de cambiar de escena.

### Escena donde se usan
- **Menu.unity** → `MenuEffects.cs` está en el **Canvas**; `Menu.cs` está en el objeto **Menu** (hijo del Canvas), que también lleva el `Image` de fondo (`Assets/Tiles/Fondo.png`). El logo es el hijo `Menu/Image`.

### Flujo
```
Botón "Jugar" ──▶ Menu.Play() ──▶ MenuEffects.StartTransition("Niveles") ──▶ fade a negro ──▶ SceneManager.LoadScene
```
