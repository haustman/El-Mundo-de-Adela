using UnityEngine;
using UnityEngine.SceneManagement;

namespace ElMundoDeAdela
{
    /// <summary>
    /// Botones del menú principal: Jugar (con transición o carga directa) y Salir.
    /// </summary>
    public class Menu : MonoBehaviour
    {
        [SerializeField] string gameSceneName = "Niveles";

        MenuEffects effects;

        void Start()
        {
            effects = FindObjectOfType<MenuEffects>();
        }

        public void Play()
        {
            if (effects)
                effects.StartTransition(gameSceneName);
            else
                SceneManager.LoadScene(gameSceneName);
        }

        public void Quit()
        {
            Application.Quit();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
    }
}
