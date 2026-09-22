using UnityEngine;

/// <summary>
/// Sigue al objetivo manteniendo el desplazamiento configurado.
/// Crea el fondo de cielo con una deriva suave para simular movimiento.
/// </summary>
public class CameraController : MonoBehaviour
{
    [SerializeField] Vector2 offset;
    [SerializeField] Transform target;

    [Header("Deriva del cielo")]
    [Tooltip("Velocidad del vaiven. Mas alto = se mueve mas rapido.")]
    [SerializeField] float driftSpeed = 0.3f;
    [Tooltip("Cuanto se desplaza el cielo a los lados, en unidades de mundo.")]
    [SerializeField] float driftRangeX = 5f;
    [Tooltip("Cuanto se desplaza el cielo arriba y abajo, en unidades de mundo.")]
    [SerializeField] float driftRangeY = 0.8f;

    GameObject skyInstance;
    Vector3 skyBaseLocal;
    float driftTime;

    void Awake()
    {
        var old = GameObject.Find("SkyBackground");
        if (old != null)
            Destroy(old);

        SetupSkyBackground();

        var cam = GetComponent<Camera>();
        if (cam != null)
            cam.backgroundColor = new Color(0.53f, 0.81f, 0.98f, 1f);
    }

    void Start()
    {
        if (skyInstance)
            skyBaseLocal = skyInstance.transform.localPosition;
    }

    void LateUpdate()
    {
        if (target)
        {
            var position = target.position;
            position.x += offset.x;
            position.y += offset.y;
            position.z = -10f;
            transform.position = position;
        }

        DriftSky();
    }

    /// <summary>Vaiven lento del cielo para que parezca que las nubes se mueven solas.</summary>
    void DriftSky()
    {
        if (!skyInstance)
            return;

        driftTime += Time.deltaTime;

        var newLocal = skyBaseLocal;
        newLocal.x += Mathf.Sin(driftTime * driftSpeed) * driftRangeX;
        newLocal.y += Mathf.Sin(driftTime * driftSpeed * 0.6f) * driftRangeY;
        skyInstance.transform.localPosition = newLocal;
    }

    void SetupSkyBackground()
    {
        var tex = Resources.Load<Texture2D>("cielo");
        if (!tex)
        {
            Debug.LogWarning("[CameraController] No se encontro 'cielo.png' en Assets/Resources/");
            return;
        }

        var sprite = Sprite.Create(
            tex,
            new Rect(0, 0, tex.width, tex.height),
            new Vector2(0.5f, 0.5f),
            100f);

        skyInstance = new GameObject("SkyBackground");

        skyInstance.transform.SetParent(transform);

        // La camara esta en z = -10 y su near clip plane es 0.3: si el cielo queda
        // en local z = 0 se solapa con la camara y el recorte lo oculta.
        // Con local z = 10 el cielo cae en el mundo en z = 0, delante del recorte.
        skyInstance.transform.localPosition = new Vector3(0f, 0f, 10f);

        // El sprite mide 16.72 x 9.41 unidades. Con escala 3 (50 x 28) sobra margen
        // para el vaiven sin descubrir los bordes ni en pantallas muy anchas.
        skyInstance.transform.localScale = Vector3.one * 3f;

        var sr = skyInstance.AddComponent<SpriteRenderer>();
        sr.sprite = sprite;
        sr.sortingLayerName = "Default";
        sr.sortingOrder = -100;
    }
}
