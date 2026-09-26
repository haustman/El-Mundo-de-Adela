using UnityEngine;

namespace ElMundoDeAdela
{
    /// <summary>Corazón recogible: cura al jugador y desaparece.</summary>
    public class Heart : MonoBehaviour
    {
        [SerializeField] LayerMask characterLayer;
        [SerializeField] float characterDetectionRange = 0.5f;
        [SerializeField] int healAmount = 1;
        [SerializeField] float rotationSpeed = 200f;

        void Update()
        {
            TryPickUp();

            // Giramos sobre el eje Y: el corazón se estrecha y se ensancha como una
            // moneda vista de frente. Sobre Z parecería una rueda rodando.
            transform.Rotate(Vector3.up * (rotationSpeed * Time.deltaTime));
        }

        void TryPickUp()
        {
            var hit = Physics2D.OverlapCircle(transform.position, characterDetectionRange, characterLayer);
            if (!hit || !hit.TryGetComponent<PlayerController>(out var player))
                return;

            // Si Adela ya está al máximo de vida, el corazón se queda donde está:
            // no se gasta para no curar nada.
            if (!player.Heal(healAmount))
                return;

            Destroy(gameObject);
        }
    }
}
