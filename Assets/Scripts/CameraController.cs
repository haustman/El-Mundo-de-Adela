using UnityEngine;

/// <summary>
/// Sigue al objetivo manteniendo el desplazamiento configurado.
/// Crea el fondo de cielo con efecto parallax suave.
/// </summary>
public class CameraController : MonoBehaviour
{
    [SerializeField] Vector2 offset;
    [SerializeField] Transform target;

    [Header("Parallax del cielo")]
    [SerializeField] float parallaxX = 0.15f;
    [SerializeField] float parallaxY = 0.05f;

    GameObject skyInstance;
    Vector3 lastTargetPos;
    Vector3 skyBaseLocal;

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
        if (target)
            lastTargetPos = target.position;

        if (skyInstance)
            skyBaseLocal = skyInstance.transform.localPosition;
    }

    void LateUpdate()
    {
        if (!target)
            return;

        Vector3 delta = target.position - lastTargetPos;
        lastTargetPos = target.position;

        var position = target.position;
        position.x += offset.x;
        position.y += offset.y;
        position.z = -10f;
        transform.position = position;

        if (skyInstance)
        {
            Vector3 newLocal = skyBaseLocal;
            newLocal.x += delta.x * parallaxX;
            newLocal.y += delta.y * parallaxY;
            skyInstance.transform.localPosition = newLocal;
        }
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
        skyInstance.transform.localPosition = Vector3.zero;
        skyInstance.transform.localScale = Vector3.one * 6f;

        var sr = skyInstance.AddComponent<SpriteRenderer>();
        sr.sprite = sprite;
        sr.sortingLayerName = "Default";
        sr.sortingOrder = -100;
    }
}
