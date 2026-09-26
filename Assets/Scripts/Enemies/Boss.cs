using System.Collections.Generic;
using UnityEngine;

namespace ElMundoDeAdela
{
    /// <summary>
    /// Jefe: patrulla si se le configura, mira al jugador y ataca cuando lo tiene cerca,
    /// con un tiempo de espera entre golpes.
    /// </summary>
    public class Boss : Damageable
    {
        [Header("Patrulla")]
        [SerializeField] bool canPatrol;
        [SerializeField] List<PatrolMovement> patrolPositions = new List<PatrolMovement>();

        [Header("Detección")]
        [SerializeField] LayerMask characterLayer;
        [Tooltip("Radio en el que el jefe detecta al jugador.")]
        [SerializeField] float characterDetectionRange = 6f;

        [Header("Ataque")]
        [SerializeField] float attackRange = 1.5f;
        [SerializeField] int damage = 1;
        [SerializeField] float attackCooldown = 1f;

        [Header("Interfaz")]
        [SerializeField] EnemyHUDController HUD;

        static readonly int IsMovingHash = Animator.StringToHash("isMoving");
        static readonly int IsRunningHash = Animator.StringToHash("isRunning");
        static readonly int AttackHash = Animator.StringToHash("attack");

        Animator animator;
        SpriteRenderer spriteRenderer;
        PatrolRoute route;
        float nextAttackTime;

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
            Patrol();

            var target = FindPlayer();
            if (target)
            {
                FacePlayer(target);
                TryAttack(target);
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

        void TryAttack(PlayerController player)
        {
            if (Time.time < nextAttackTime)
                return;

            if (Vector2.Distance(transform.position, player.transform.position) > attackRange)
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

            animator.SetBool(IsMovingHash, canPatrol && Route.IsValid);
            animator.SetBool(IsRunningHash, false);
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

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.black;
            Gizmos.DrawWireSphere(transform.position, characterDetectionRange);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);
        }
    }
}
