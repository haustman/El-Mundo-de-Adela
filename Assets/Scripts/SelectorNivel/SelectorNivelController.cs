using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ElMundoDeAdela
{
    /// <summary>
    /// Lógica del selector de niveles. La interfaz (panel, botones, orden, colores,
    /// fuente) se arma y se edita en la escena Niveles.unity; este script solo la
    /// conecta: habilita cada botón si su escena existe en Build Settings, marca
    /// los que faltan como "próximamente" y carga la escena elegida o el menú (Esc).
    /// </summary>
    public class SelectorNivelController : MonoBehaviour
    {
        [SerializeField] string menuSceneName = "Menu";
        [SerializeField] string[] levelSceneNames = { "Nivel_1", "Nivel_2", "Nivel_3", "Nivel_4", "Nivel_5" };
        [SerializeField] Button[] botonesNivel;
        [SerializeField] Button botonVolver;

        void Start()
        {
            BindLevelButtons();

            if (botonVolver)
                botonVolver.onClick.AddListener(() => LoadScene(menuSceneName));

            SelectFirstAvailable();
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
                LoadScene(menuSceneName);
        }

        void BindLevelButtons()
        {
            if (botonesNivel == null || levelSceneNames == null)
                return;

            int count = Mathf.Min(botonesNivel.Length, levelSceneNames.Length);

            for (int i = 0; i < count; i++)
            {
                Button button = botonesNivel[i];
                if (!button)
                    continue;

                string sceneName = levelSceneNames[i];
                bool available = Application.CanStreamedLevelBeLoaded(sceneName);

                button.interactable = available;

                if (!available)
                {
                    TMP_Text label = button.GetComponentInChildren<TMP_Text>(true);
                    if (label)
                        label.text += "  ·  próximamente";
                }

                string scene = sceneName;
                button.onClick.AddListener(() => LoadScene(scene));
            }
        }

        void SelectFirstAvailable()
        {
            if (!EventSystem.current || botonesNivel == null)
                return;

            foreach (Button button in botonesNivel)
            {
                if (button && button.interactable)
                {
                    EventSystem.current.SetSelectedGameObject(button.gameObject);
                    return;
                }
            }
        }

        void LoadScene(string sceneName)
        {
            if (Application.CanStreamedLevelBeLoaded(sceneName))
                SceneManager.LoadScene(sceneName);
        }
    }
}
