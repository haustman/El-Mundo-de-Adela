using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Enemigo cuerpo a cuerpo: patrulla por una ruta y dana al jugador al tocarlo.
/// </summary>
public class Enemy : Damageable
{
    [SerializeField] bool canPatrol;
    [SerializeField] List<PatrolMovement> patrolPositions;
    [SerializeField] LayerMask characterLayer;
    [SerializeField] float characterDetectionRange;
    [SerializeField] int damage;
    [SerializeField] EnemyHUDController HUD;

    PatrolRoute patrolRoute;

    /// <summary>La ruta se construye la primera vez que se usa, sin depender del orden de Start.</summary>
    PatrolRoute Route => patrolRoute ??= new PatrolRoute(patrolPositions);

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
        if (hit && hit.TryGetComponent<CharacterController>(out var player))
            player.GetHit(damage);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(transform.position, characterDetectionRange);
    }
}
