using UnityEngine;

/// <summary>Sigue al objetivo manteniendo el desplazamiento configurado.</summary>
public class CameraController : MonoBehaviour
{
    [SerializeField] Vector2 offset;
    [SerializeField] Transform target;

    // LateUpdate: la camara se coloca despues de que el jugador se haya movido, para que no tiemble.
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
}
