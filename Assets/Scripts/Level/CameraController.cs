using UnityEngine;

namespace ElMundoDeAdela
{
    /// <summary>
    /// Sigue al objetivo manteniendo el desplazamiento configurado y fija el color de fondo.
    /// </summary>
    public class CameraController : MonoBehaviour
    {
        [SerializeField] Transform target;
        [Tooltip("Desplazamiento de la cámara respecto al objetivo.")]
        [SerializeField] Vector2 offset = new Vector2(2f, 2f);
        [Tooltip("Tiempo de suavizado del seguimiento. 0 = rígido.")]
        [SerializeField] float smoothing = 0f;
        [SerializeField] Color backgroundColor = new Color(0.53f, 0.81f, 0.98f, 1f);

        Camera cam;
        Vector3 velocity;

        void Awake()
        {
            cam = GetComponent<Camera>();
            if (cam)
                cam.backgroundColor = backgroundColor;
        }

        void LateUpdate()
        {
            if (!target)
                return;

            var desired = target.position + (Vector3)offset;
            desired.z = -10f;

            transform.position = smoothing > 0f
                ? Vector3.SmoothDamp(transform.position, desired, ref velocity, smoothing)
                : desired;
        }
    }
}
