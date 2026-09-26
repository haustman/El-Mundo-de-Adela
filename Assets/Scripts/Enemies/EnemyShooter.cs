using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Enemigo a distancia: patrulla, dispara durante el recorrido y se gira al cerrar la vuelta.
/// </summary>
public class EnemyShooter : Damageable
{
    [SerializeField] GameObject shape;
    [SerializeField] ProjectileEnemy projectilePrefab;
    [SerializeField] bool canPatrol;
    [SerializeField] List<PatrolMovement> patrolPositions;
    [SerializeField] LayerMask characterLayer;
    [SerializeField] float characterDetectionRange;
    [SerializeField] int damage;
    [SerializeField] EnemyShooterHUDController HUD;
    [SerializeField] int shootNum;

    PatrolRoute patrolRoute;
    bool isFacingRight = true;
    bool hasShotThisLeg;

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

        ShootOnSchedule();

        if (!Route.HasArrived(transform.position))
            return;

        hasShotThisLeg = false;

        // Con un solo punto no hay vuelta que cerrar: si no se comprueba,
        // el enemigo dispararia y se giraria una vez por frame.
        if (Route.Count > 1 && Route.Next())
        {
            Shoot();
            Flip();
        }
    }

    /// <summary>Un disparo por tramo, a la fraccion del recorrido que marca shootNum.</summary>
    void ShootOnSchedule()
    {
        if (hasShotThisLeg || shootNum <= 0)
            return;

        if (Route.Progress < 1f / shootNum)
            return;

        hasShotThisLeg = true;
        Shoot();
    }

    void Shoot()
    {
        var projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        projectile.Shoot(isFacingRight ? 1 : -1, damage);
    }

    /// <summary>Espeja el sprite y cambia el sentido del disparo.</summary>
    void Flip()
    {
        if (!shape) return;
        var scale = shape.transform.localScale;
        scale.x *= -1f;
        shape.transform.localScale = scale;
        isFacingRight = !isFacingRight;
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
