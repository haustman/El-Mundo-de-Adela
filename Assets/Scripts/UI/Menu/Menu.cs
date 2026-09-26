using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Botones del menu principal: Jugar (transicion o carga directa)
/// y Quit (salir del juego/editor).
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
