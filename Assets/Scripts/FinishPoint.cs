using UnityEngine;

/// <summary>Meta del nivel: al tocarla el jugador reinicia la escena.</summary>
public class FinishPoint : MonoBehaviour
{
    [SerializeField] LayerMask characterLayer;
    [SerializeField] float characterDetectionRange;

    void Update()
    {
        var hit = Physics2D.OverlapCircle(transform.position, characterDetectionRange, characterLayer);
        if (!hit || !hit.TryGetComponent<CharacterController>(out var player))
            return;

        player.Die();
        Destroy(gameObject);
    }
}
