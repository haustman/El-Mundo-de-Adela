using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    [SerializeField] string gameSceneName = "SampleScene";

    MenuEffects effects;

    void Start()
    {
        effects = FindObjectOfType<MenuEffects>();
    }

    public void Play()
    {
        if (effects)
            effects.StartTransition();
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
