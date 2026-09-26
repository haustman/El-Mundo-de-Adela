using UnityEngine;

/// <summary>Corazon recogible: cura al jugador y desaparece.</summary>
public class Heart : MonoBehaviour
{
    [SerializeField] LayerMask characterLayer;
    [SerializeField] float characterDetectionRange;
    [SerializeField] float rotationSpeed = 200f;

    void Update()
    {
        TryPickUp();

        // Giro sobre el eje Y: el corazon se estrecha y se ensancha como una moneda
        // vista de frente, que es el clasico de los recogibles. Sobre Z pareceria
        // una rueda rodando.
        transform.Rotate(Vector3.up * (rotationSpeed * Time.deltaTime));
    }

    void TryPickUp()
    {
        var hit = Physics2D.OverlapCircle(transform.position, characterDetectionRange, characterLayer);
        if (!hit || !hit.TryGetComponent<PlayerController>(out var player))
            return;

        // Si Adela ya esta al maximo de vida, el corazon se queda donde esta:
        // no se gasta para no curar nada.
        if (!player.Heal(1))
            return;

        Destroy(gameObject);
    }
}
