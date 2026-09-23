using UnityEngine;

/// <summary>
/// Sigue al objetivo manteniendo el desplazamiento configurado.
/// </summary>
public class CameraController : MonoBehaviour
{
    [SerializeField] Vector2 offset;
    [SerializeField] Transform target;

    void Awake()
    {
        var cam = GetComponent<Camera>();
        if (cam != null)
            cam.backgroundColor = new Color(0.53f, 0.81f, 0.98f, 1f);
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
    }
}
