using System.Collections.Generic;
using UnityEngine;

namespace ElMundoDeAdela
{
    /// <summary>
    /// Jefe: patrulla si se le configura, persigue al jugador cuando lo ve, ataca
    /// cuerpo a cuerpo con un tiempo de espera entre golpes y avisa al morir.
    /// Todo se ajusta desde el Inspector.
    /// </summary>
    public class Boss : Damageable
    {
        [Header("Patrulla")]
        [SerializeField] bool canPatrol;
        [SerializeField] List<PatrolMovement> patrolPositions = new List<PatrolMovement>();

        [Header("Persecución")]
        [Tooltip("Si está activo, el jefe camina hacia Adela en cuanto la ve.")]
        [SerializeField] bool canChase = true;
        [Tooltip("Velocidad al perseguir. Adela anda a 10 y corre a 16: no la alcanza corriendo.")]
        [SerializeField, Min(0f)] float chaseSpeed = 1.8f;
        [Tooltip("Se para a esta distancia para dar el golpe.")]
        [SerializeField, Min(0f)] float stopChasingRange = 1.2f;

        [Header("Detección")]
        [SerializeField] LayerMask characterLayer;
        [Tooltip("Radio en el que el jefe detecta al jugador (círculo negro).")]
        [SerializeField, Min(0f)] float characterDetectionRange = 6f;

        [Header("Ataque")]
        [Tooltip("Distancia a la que golpea (círculo rojo).")]
        [SerializeField, Min(0f)] float attackRange = 1.5f;
        [Tooltip("Corazones que le quita a Adela por golpe.")]
        [SerializeField, Min(1)] int damage = 1;
        [Tooltip("Segundos entre golpes. Adela es invulnerable ~1,2 s tras recibir daño.")]
        [SerializeField, Min(0f)] float attackCooldown = 1.2f;

        [Header("Al morir")]
        [Tooltip("Objeto que se activa al derrotarlo (la meta, una puerta...).")]
        [SerializeField] GameObject activateOnDeath;
        [Tooltip("Objeto que se desactiva al derrotarlo.")]
        [SerializeField] GameObject deactivateOnDeath;

        [Header("Interfaz")]
        [SerializeField] EnemyHUDController HUD;

        static readonly int IsMovingHash = Animator.StringToHash("isMoving");
        static readonly int IsRunningHash = Animator.StringToHash("isRunning");
        static readonly int AttackHash = Animator.StringToHash("attack");

        Animator animator;
        SpriteRenderer spriteRenderer;
        PatrolRoute route;
        float nextAttackTime;
        bool isChasing;

        PatrolRoute Route => route ??= new PatrolRoute(patrolPositions, transform.position);

        protected override void SetupHUD()
        {
            if (HUD)
                HUD.Setup(this);
        }

        protected override void RefreshHUD()
        {
            if (HUD)
                HUD.Repaint(this);
        }

        void Awake()
        {
            animator = GetComponent<Animator>();
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        void Update()
        {
            EnsureReady();
            isChasing = false;

            var target = FindPlayer();

            if (target)
            {
                FacePlayer(target);

                if (InAttackRange(target))
                    TryAttack(target);
                else
                    Chase(target);
            }
            else
            {
                Patrol();
            }

            UpdateAnimator();
        }

        void Patrol()
        {
            if (!canPatrol || !Route.IsValid)
                return;

            transform.position = Route.Tick(Time.deltaTime);

            if (Route.HasArrived(transform.position))
                Route.Next();
        }

        PlayerController FindPlayer()
        {
            var hit = Physics2D.OverlapCircle(transform.position, characterDetectionRange, characterLayer);
            return hit && hit.TryGetComponent<PlayerController>(out var player) ? player : null;
        }

        bool InAttackRange(PlayerController player)
        {
            return Vector2.Distance(transform.position, player.transform.position) <= attackRange;
        }

        /// <summary>Camina hacia el jugador parándose a golpe de distancia.</summary>
        void Chase(PlayerController player)
        {
            if (!canChase)
                return;

            var toPlayer = player.transform.position.x - transform.position.x;
            if (Mathf.Abs(toPlayer) <= stopChasingRange)
                return;

            isChasing = true;
            transform.position += Vector3.right * Mathf.Sign(toPlayer) * chaseSpeed * Time.deltaTime;
        }

        void TryAttack(PlayerController player)
        {
            if (Time.time < nextAttackTime)
                return;

            nextAttackTime = Time.time + attackCooldown;

            if (animator)
                animator.SetTrigger(AttackHash);

            player.GetHit(damage);
        }

        void UpdateAnimator()
        {
            if (!animator)
                return;

            animator.SetBool(IsMovingHash, isChasing || (canPatrol && Route.IsValid));
            animator.SetBool(IsRunningHash, isChasing);
        }

        void FacePlayer(PlayerController player)
        {
            if (!spriteRenderer)
                return;

            var toPlayer = player.transform.position.x - transform.position.x;
            if (Mathf.Abs(toPlayer) < 0.01f)
                return;

            spriteRenderer.flipX = toPlayer < 0f;
        }

        /// <summary>Avisa a la meta o a la puerta cuando el jefe cae.</summary>
        protected override void Die()
        {
            base.Die();

            if (activateOnDeath)
                activateOnDeath.SetActive(true);

            if (deactivateOnDeath)
                deactivateOnDeath.SetActive(false);
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.black;
            Gizmos.DrawWireSphere(transform.position, characterDetectionRange);

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, stopChasingRange);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);
        }
    }
}
