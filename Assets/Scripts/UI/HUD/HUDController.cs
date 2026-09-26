using UnityEngine;
using UnityEngine.UI;

/// <summary>Corazones de vida del jugador.</summary>
public class HUDController : MonoBehaviour
{
    /// <summary>Instancia activa. La escena solo tiene un HUD.</summary>
    public static HUDController instance { get; private set; }

    [SerializeField] Image[] images;
    [SerializeField] Sprite fullHeart;
    [SerializeField] Sprite brokenHeart;

    void Awake()
    {
        instance = this;
    }

    /// <summary>Repinta los corazones sin tener que comprobar si el HUD existe.</summary>
    public static void Refresh(int health)
    {
        if (instance)
            instance.Repaint(health);
    }

    public void Repaint(int health)
    {
        var fullHearts = Mathf.Clamp(health, 0, images.Length);

        for (int i = 0; i < images.Length; i++)
            images[i].sprite = i < fullHearts ? fullHeart : brokenHeart;
    }
}
