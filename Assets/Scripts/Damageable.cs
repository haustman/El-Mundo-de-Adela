using UnityEngine;

/// <summary>
/// Base para todo lo que tiene vida y puede recibir dano.
/// Los nombres de los campos (Health, MaxHealth) se mantienen porque Unity los guarda en la escena.
/// </summary>
public abstract class Damageable : MonoBehaviour
{
    public int MaxHealth;
    public int Health;

    bool isReady;
    bool isDead;

    /// <summary>
    /// Prepara la vida y la barra. Se llama solo, la primera vez que hace falta:
    /// asi no depende de Start y un recargue de ensamblado (editar un script en Play)
    /// no deja el componente a medio inicializar.
    /// </summary>
    protected void EnsureReady()
    {
        if (isReady)
            return;

        isReady = true;
        MaxHealth = Health;
        SetupHUD();
    }

    /// <summary>Aplica dano. Los golpes sobre algo ya muerto se ignoran.</summary>
    public void GetHit(int amount)
    {
        EnsureReady();

        if (isDead)
            return;

        Health = Mathf.Max(Health - amount, 0);
        RefreshHUD();

        if (Health <= 0)
            Die();
    }

    /// <summary>Prepara la barra de vida. Lo llama EnsureReady una sola vez.</summary>
    protected virtual void SetupHUD() { }

    /// <summary>Refresca la barra de vida tras un cambio de vida.</summary>
    protected virtual void RefreshHUD() { }

    protected virtual void Die()
    {
        isDead = true;
        Destroy(gameObject);
    }
}
