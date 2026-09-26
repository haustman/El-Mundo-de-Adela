using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Barra de vida reutilizable para cualquier objeto con vida.
/// </summary>
public class HealthBarController : MonoBehaviour
{
    [SerializeField] Slider hpBar;

    /// <summary>Configura la barra con la vida maxima actual (una vez, al arrancar).</summary>
    public void Setup(Damageable damageable)
    {
        if (!hpBar || !damageable)
            return;

        hpBar.minValue = 0;
        hpBar.maxValue = damageable.MaxHealth;
        hpBar.value = damageable.Health;
    }

    /// <summary>Actualiza el valor de la barra tras cambiar la vida.</summary>
    public void Repaint(Damageable damageable)
    {
        if (!hpBar || !damageable)
            return;

        hpBar.value = damageable.Health;
    }
}
