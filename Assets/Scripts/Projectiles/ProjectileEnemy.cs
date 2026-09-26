using UnityEngine;

namespace ElMundoDeAdela
{
    /// <summary>Proyectil enemigo: avanza en línea recta y daña al primero que toca.</summary>
    public class ProjectileEnemy : MonoBehaviour
    {
        [SerializeField] float speed = 12f;
        [SerializeField] float lifeDuration = 2f;
        [SerializeField] LayerMask detectionLayer;
        [SerializeField] float detectionRadius = 0.1f;

        bool launched;
        float remainingLife;
        int direction;
        int damage;

        /// <summary>Lanza el proyectil hacia la derecha (1) o hacia la izquierda (-1).</summary>
        public void Shoot(int direction, int damage)
        {
            launched = true;
            remainingLife = lifeDuration;
            this.direction = direction;
            this.damage = damage;
        }

        void Update()
        {
            if (!launched)
                return;

            transform.position += Vector3.right * (speed * direction * Time.deltaTime);

            if (TryDamage())
                return;

            remainingLife -= Time.deltaTime;
            if (remainingLife <= 0f)
                Die();
        }

        /// <summary>Daña al objetivo que tenga delante. Devuelve true si ha impactado.</summary>
        bool TryDamage()
        {
            var hit = Physics2D.OverlapCircle(transform.position, detectionRadius, detectionLayer);
            if (!hit)
                return false;

            if (hit.TryGetComponent<PlayerController>(out var player))
                player.GetHit(damage);

            Die();
            return true;
        }

        void Die()
        {
            Destroy(gameObject);
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, detectionRadius);
        }
    }
}
