using UnityEngine;
using UnityEngine.UI;

namespace ElMundoDeAdela
{
    /// <summary>Corazones de vida del jugador.</summary>
    public class HUDController : MonoBehaviour
    {
        /// <summary>Instancia activa. La escena solo tiene un HUD.</summary>
        public static HUDController Instance { get; private set; }

        [SerializeField] Image[] images;
        [SerializeField] Sprite fullHeart;
        [SerializeField] Sprite brokenHeart;

        void Awake()
        {
            Instance = this;
        }

        void OnDestroy()
        {
            // Sin esto, la referencia estática apuntaría a un objeto ya destruido
            // al recargar la escena.
            if (Instance == this)
                Instance = null;
        }

        /// <summary>Repinta los corazones sin tener que comprobar si el HUD existe.</summary>
        public static void Refresh(int health)
        {
            if (Instance)
                Instance.Repaint(health);
        }

        public void Repaint(int health)
        {
            int fullHearts = Mathf.Clamp(health, 0, images.Length);

            for (int i = 0; i < images.Length; i++)
                images[i].sprite = i < fullHearts ? fullHeart : brokenHeart;
        }
    }
}
