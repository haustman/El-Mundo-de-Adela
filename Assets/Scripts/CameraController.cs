using UnityEngine;

/// <summary>
/// Sigue al objetivo manteniendo el desplazamiento configurado.
/// Crea el fondo de cielo una sola vez y lo mantiene fijo a la camara.
/// </summary>
public class CameraController : MonoBehaviour
{
    [SerializeField] Vector2 offset;
    [SerializeField] Transform target;

    // Guardamos la referencia para no crear duplicados si Awake se llama
    // varias veces (por recargas de escena o recompilacion en Play Mode).
    GameObject skyInstance;

    void Awake()
    {
        // Si ya existe un SkyBackground de una ejecucion anterior, lo
        // destruimos antes de crear uno nuevo para evitar el efecto fantasma.
        var old = GameObject.Find("SkyBackground");
        if (old != null)
            Destroy(old);

        SetupSkyBackground();

        // Color de limpieza de la camara: si el cielo no carga, al menos
        // no se ve negro sino un azul de cielo de emergencia.
        var cam = GetComponent<Camera>();
        if (cam != null)
            cam.backgroundColor = new Color(0.53f, 0.81f, 0.98f, 1f);
    }

    void LateUpdate()
    {
        if (!target)
            return;

        var position = target.position;
        position.x += offset.x;
        position.y += offset.y;
        position.z = -10f;
        transform.position = position;
    }

    void SetupSkyBackground()
    {
        var tex = Resources.Load<Texture2D>("cielo");
        if (!tex)
        {
            Debug.LogWarning("[CameraController] No se encontro 'cielo.png' en Assets/Resources/");
            return;
        }

        // Creamos el sprite a partir de la textura cargada en tiempo de ejecucion.
        var sprite = Sprite.Create(
            tex,
            new Rect(0, 0, tex.width, tex.height),
            new Vector2(0.5f, 0.5f),
            100f);

        skyInstance = new GameObject("SkyBackground");

        // Hijo de la camara: se mueve con ella automaticamente.
        skyInstance.transform.SetParent(transform);

        // IMPORTANTE: z = 0 en espacio local de la camara.
        // La camara esta en z = -10, asi que el sprite queda en z = -10
        // en world space, bien dentro del frustum.
        skyInstance.transform.localPosition = new Vector3(0f, 0f, 0f);

        // Escala para cubrir toda la pantalla. Ajusta si hace falta.
        skyInstance.transform.localScale = Vector3.one * 6f;

        var sr = skyInstance.AddComponent<SpriteRenderer>();
        sr.sprite = sprite;

        // Sorting order muy bajo para que quede DETRAS de todos los tilemaps
        // y sprites del juego (que suelen estar en order 0 o mas).
        sr.sortingLayerName = "Default";
        sr.sortingOrder = -100;
    }
}

