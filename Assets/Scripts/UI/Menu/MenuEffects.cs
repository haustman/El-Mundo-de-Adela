using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ElMundoDeAdela
{
    /// <summary>
    /// Efectos de la pantalla de menú: fade-in del logo con titileo suave,
    /// aparición escalonada de botones con sombra al pasar el ratón y transición al entrar al juego.
    /// </summary>
    public class MenuEffects : MonoBehaviour
    {
        [Header("Fade-in")]
        [SerializeField] float logoFadeDuration = 1.2f;
        [SerializeField] float buttonFadeDuration = 0.6f;
        [SerializeField] float buttonDelay = 0.5f;
        [SerializeField] float buttonDelayStep = 0.3f;

        [Header("Titileo del logo")]
        [SerializeField] float pulseMinAlpha = 0.65f;
        [SerializeField] float pulseMaxAlpha = 1f;
        [SerializeField] float pulseSpeed = 0.6f;

        [Header("Escala al pasar el ratón")]
        [SerializeField] float hoverScale = 1.12f;
        [SerializeField] float scaleSpeed = 8f;

        [Header("Sombra de los botones")]
        [SerializeField] Color shadowColor = new Color(0f, 0f, 0f, 0.6f);
        [SerializeField] Vector2 shadowOffsetHover = new Vector2(3f, -3f);
        [SerializeField] Vector2 shadowOffsetNormal = Vector2.zero;
        [SerializeField] float shadowSpeed = 10f;

        [Header("Transición al jugar")]
        [SerializeField] Color transitionColor = Color.black;
        [SerializeField] float transitionDuration = 0.8f;
        [SerializeField] float transitionHoldTime = 0.2f;

        CanvasGroup logoGroup;

        RectTransform playRect;
        RectTransform quitRect;
        Vector3 playOriginal;
        Vector3 quitOriginal;
        bool playHover;
        bool quitHover;

        Shadow playShadow;
        Shadow quitShadow;

        CanvasGroup fadeOverlay;
        bool transitioning;
        string sceneNameToLoad;

        void Start()
        {
            Transform menu = transform.Find("Menu");
            if (!menu)
                return;

            logoGroup = SetupFade(menu.Find("Image"), logoFadeDuration, 0f);
            SetupFade(menu.Find("Jugar"), buttonFadeDuration, buttonDelay);
            SetupFade(menu.Find("Salir"), buttonFadeDuration, buttonDelay + buttonDelayStep);

            playRect = menu.Find("Jugar")?.GetComponent<RectTransform>();
            quitRect = menu.Find("Salir")?.GetComponent<RectTransform>();

            if (playRect) playOriginal = playRect.localScale;
            if (quitRect) quitOriginal = quitRect.localScale;

            SetupHover(menu.Find("Jugar"), play: true);
            SetupHover(menu.Find("Salir"), play: false);

            playShadow = GetOrAddShadow(menu.Find("Jugar"));
            quitShadow = GetOrAddShadow(menu.Find("Salir"));

            CreateFadeOverlay();

            if (logoGroup)
                StartCoroutine(PulseLogo());
        }

        void Update()
        {
            AnimateScale(playRect, playOriginal, playHover);
            AnimateScale(quitRect, quitOriginal, quitHover);
            AnimateShadow(playShadow, playHover);
            AnimateShadow(quitShadow, quitHover);
        }

        // ---------------------------------------------------------------- fade-in

        CanvasGroup SetupFade(Transform target, float duration, float delay)
        {
            if (!target)
                return null;

            CanvasGroup group = target.gameObject.GetComponent<CanvasGroup>();
            if (!group)
                group = target.gameObject.AddComponent<CanvasGroup>();

            group.alpha = 0f;
            StartCoroutine(FadeIn(group, duration, delay));
            return group;
        }

        IEnumerator FadeIn(CanvasGroup group, float duration, float delay)
        {
            if (delay > 0f)
                yield return new WaitForSeconds(delay);

            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                group.alpha = Mathf.Clamp01(elapsed / duration);
                yield return null;
            }

            group.alpha = 1f;
        }

        // ---------------------------------------------------------------- titileo

        IEnumerator PulseLogo()
        {
            // Espera a que acabe el fade-in: si el titileo arranca antes, pisa el alpha
            // cada frame y el logo nunca llega a verse desvanecerse desde cero.
            yield return new WaitForSecondsRealtime(logoFadeDuration);

            while (true)
            {
                float t = (Mathf.Sin(Time.unscaledTime * pulseSpeed * Mathf.PI * 2f) + 1f) * 0.5f;
                logoGroup.alpha = Mathf.Lerp(pulseMinAlpha, pulseMaxAlpha, t);
                yield return null;
            }
        }

        // ---------------------------------------------------------------- escala

        void AnimateScale(RectTransform rect, Vector3 original, bool hovering)
        {
            if (!rect)
                return;

            float target = hovering ? hoverScale : 1f;
            rect.localScale = Vector3.Lerp(rect.localScale, original * target, Time.unscaledDeltaTime * scaleSpeed);
        }

        // ---------------------------------------------------------------- sombra

        Shadow GetOrAddShadow(Transform target)
        {
            if (!target)
                return null;

            Shadow shadow = target.gameObject.GetComponent<Shadow>();
            if (!shadow)
                shadow = target.gameObject.AddComponent<Shadow>();

            shadow.effectColor = shadowColor;
            shadow.effectDistance = shadowOffsetNormal;
            return shadow;
        }

        void AnimateShadow(Shadow shadow, bool hovering)
        {
            if (!shadow)
                return;

            Vector2 target = hovering ? shadowOffsetHover : shadowOffsetNormal;
            shadow.effectDistance = Vector2.Lerp(shadow.effectDistance, target, Time.unscaledDeltaTime * shadowSpeed);
        }

        // ---------------------------------------------------------------- transición

        void CreateFadeOverlay()
        {
            GameObject overlay = new GameObject("FadeOverlay");
            overlay.transform.SetParent(transform, false);

            RectTransform rect = overlay.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            Image image = overlay.AddComponent<Image>();
            image.color = transitionColor;

            fadeOverlay = overlay.AddComponent<CanvasGroup>();
            fadeOverlay.alpha = 0f;
            fadeOverlay.blocksRaycasts = false;
        }

        public void StartTransition(string sceneName)
        {
            if (transitioning)
                return;

            sceneNameToLoad = sceneName;

            // Sin overlay (por ejemplo si no se encontró el objeto "Menu") no hay fundido que hacer.
            if (fadeOverlay == null)
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(sceneNameToLoad);
                return;
            }

            StartCoroutine(TransitionToGame());
        }

        IEnumerator TransitionToGame()
        {
            transitioning = true;
            fadeOverlay.blocksRaycasts = true;

            float elapsed = 0f;
            while (elapsed < transitionDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                fadeOverlay.alpha = Mathf.Clamp01(elapsed / transitionDuration);
                yield return null;
            }

            fadeOverlay.alpha = 1f;

            yield return new WaitForSecondsRealtime(transitionHoldTime);

            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneNameToLoad);
        }

        // ---------------------------------------------------------------- hover

        void SetupHover(Transform target, bool play)
        {
            if (!target)
                return;

            Button button = target.GetComponent<Button>();
            if (!button)
                return;

            EventTrigger trigger = target.gameObject.GetComponent<EventTrigger>();
            if (!trigger)
                trigger = target.gameObject.AddComponent<EventTrigger>();

            trigger.triggers.Clear();

            EventTrigger.Entry enter = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
            enter.callback.AddListener(_ => SetHover(play, true));
            trigger.triggers.Add(enter);

            EventTrigger.Entry exit = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };
            exit.callback.AddListener(_ => SetHover(play, false));
            trigger.triggers.Add(exit);
        }

        void SetHover(bool play, bool hovering)
        {
            if (play)
                playHover = hovering;
            else
                quitHover = hovering;
        }
    }
}
