using System.Collections.Generic;
using UnityEngine;

namespace ElMundoDeAdela
{
    /// <summary>
    /// Enemigo cuerpo a cuerpo: patrulla por una ruta y daña al jugador al tocarlo.
    /// </summary>
    public class Enemy : Damageable
    {
        [Header("Patrulla")]
        [SerializeField] bool canPatrol;
        [SerializeField] List<PatrolMovement> patrolPositions = new List<PatrolMovement>();

        [Header("Detección")]
        [SerializeField] LayerMask characterLayer;
        [SerializeField] float characterDetectionRange = 1f;

        [Header("Combate")]
        [SerializeField] int damage = 1;

        [Header("Interfaz")]
        [SerializeField] EnemyHUDController HUD;

        PatrolRoute route;

        // La ruta se construye la primera vez que se usa, sin depender del orden de Start.
        PatrolRoute Route => route ??= new PatrolRoute(patrolPositions);

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

        void Update()
        {
            EnsureReady();
            Patrol();
            DamagePlayerInRange();
        }

        void Patrol()
        {
            if (!canPatrol || !Route.IsValid)
                return;

            transform.position = Route.Tick(Time.deltaTime);

            if (Route.HasArrived(transform.position))
                Route.Next();
        }

        void DamagePlayerInRange()
        {
            var hit = Physics2D.OverlapCircle(transform.position, characterDetectionRange, characterLayer);
            if (hit && hit.TryGetComponent<PlayerController>(out var player))
                player.GetHit(damage);
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.black;
            Gizmos.DrawWireSphere(transform.position, characterDetectionRange);
        }
    }
}
