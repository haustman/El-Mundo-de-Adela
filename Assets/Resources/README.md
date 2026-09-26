# Resources

Carpeta especial de Unity. Los assets acá se cargan desde código con `Resources.Load()`.

- **cielo.png** — Imagen original del cielo. **Hoy no se usa**: ni la escena ni `CameraController.cs` la referencian. El fondo visible es el sprite `Sprites/UI/Fondo.png` (objeto `Fondo para juego` en la escena). Se mantiene por si se reutiliza.
- **cielo.png.bak** — No existe en el repo (los `.bak` están en `.gitignore`).

No meter cosas acá a menos que las necesites cargar desde un script.
