# Resources

Carpeta especial de Unity. Los assets acá se cargan desde código con `Resources.Load()`.

- **cielo.png** — Imagen original del cielo. **Hoy no se usa**: ni la escena ni `CameraController.cs` la referencian. El fondo visible es `Sprites/UI/Fondo para juego.jpg` (objeto en la escena). Se mantiene por si se reutiliza.
- **cielo.png.bak** — No existe en el repo (los `.bak` están en `.gitignore`).

No meter cosas acá a menos que las necesites cargar desde un script.
