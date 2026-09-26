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
        transform.Rotate(Vector3.up * (rotationSpeed * Time.deltaTime));
    }

    void TryPickUp()
    {
        var hit = Physics2D.OverlapCircle(transform.position, characterDetectionRange, characterLayer);
        if (!hit || !hit.TryGetComponent<CharacterController>(out var player))
            return;

        player.Heal(1);
        Destroy(gameObject);
    }
}
