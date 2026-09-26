using UnityEngine;

namespace ElMundoDeAdela
{
    /// <summary>Meta del nivel: al tocarla, el jugador termina el nivel.</summary>
    public class FinishPoint : MonoBehaviour
    {
        [SerializeField] LayerMask characterLayer;
        [SerializeField] float characterDetectionRange = 0.5f;
        [Tooltip("Destruye la meta cuando el jugador la alcanza.")]
        [SerializeField] bool destroyOnFinish = true;

        void Update()
        {
            var hit = Physics2D.OverlapCircle(transform.position, characterDetectionRange, characterLayer);
            if (!hit || !hit.TryGetComponent<PlayerController>(out var player))
                return;

            player.Finish();

            if (destroyOnFinish)
                Destroy(gameObject);
        }
    }
}
