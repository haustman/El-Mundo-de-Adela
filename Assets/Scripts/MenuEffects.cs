using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// Efectos de la pantalla de menu: fade-in del logo con titilado suave,
/// aparicion escalonada de botones con sombra al hover/click,
/// y transicion al entrar al juego.
/// </summary>
public class MenuEffects : MonoBehaviour
{
    [Header("Fade-in")]
    [SerializeField] float logoFadeDuration = 1.2f;
    [SerializeField] float buttonFadeDuration = 0.6f;
    [SerializeField] float buttonDelay = 0.5f;

    [Header("Titilado del logo")]
    [SerializeField] float pulseMinAlpha = 0.65f;
    [SerializeField] float pulseMaxAlpha = 1f;
    [SerializeField] float pulseSpeed = 0.6f;

    [Header("Hover Scale")]
    [SerializeField] float hoverScale = 1.12f;
    [SerializeField] float scaleSpeed = 8f;

    [Header("Sombra botones")]
    [SerializeField] Vector2 shadowOffsetHover = new Vector2(3f, -3f);
    [SerializeField] Vector2 shadowOffsetNormal = Vector2.zero;
    [SerializeField] float shadowSpeed = 10f;

    [Header("Transicion al jugar")]
    [SerializeField] float transitionDuration = 0.8f;

    CanvasGroup logoGroup;

    RectTransform jugarRect;
    RectTransform salirRect;
    Vector3 jugarOriginal;
    Vector3 salirOriginal;
    bool jugarHover;
    bool salirHover;

    Shadow jugarShadow;
    Shadow salirShadow;

    CanvasGroup fadeOverlay;
    bool transitioning;

    void Start()
    {
        Transform menu = transform.Find("Menu");
        if (!menu) return;

        logoGroup = SetupFade(menu.Find("Image"), logoFadeDuration, 0f);
        SetupFade(menu.Find("Jugar"), buttonFadeDuration, buttonDelay);
        SetupFade(menu.Find("Salir"), buttonFadeDuration, buttonDelay + 0.3f);

        jugarRect = menu.Find("Jugar")?.GetComponent<RectTransform>();
        salirRect = menu.Find("Salir")?.GetComponent<RectTransform>();

        if (jugarRect) jugarOriginal = jugarRect.localScale;
        if (salirRect) salirOriginal = salirRect.localScale;

        SetupHover(menu.Find("Jugar"), "Jugar");
        SetupHover(menu.Find("Salir"), "Salir");

        jugarShadow = GetOrAddShadow(menu.Find("Jugar"));
        salirShadow = GetOrAddShadow(menu.Find("Salir"));

        CreateFadeOverlay();

        if (logoGroup)
            StartCoroutine(PulseLogo());
    }

    void Update()
    {
        AnimateScale(jugarRect, jugarOriginal, jugarHover);
        AnimateScale(salirRect, salirOriginal, salirHover);
        AnimateShadow(jugarShadow, jugarHover);
        AnimateShadow(salirShadow, salirHover);
    }

    // ---------------------------------------------------------------- fade-in

    CanvasGroup SetupFade(Transform target, float duration, float delay)
    {
        if (!target) return null;
        CanvasGroup cg = target.gameObject.GetComponent<CanvasGroup>();
        if (!cg) cg = target.gameObject.AddComponent<CanvasGroup>();
        cg.alpha = 0f;
        StartCoroutine(FadeIn(cg, duration, delay));
        return cg;
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

    // ---------------------------------------------------------------- titilado

    IEnumerator PulseLogo()
    {
        // Espera a que acabe el fade-in: si el titileo arranca antes, pisa el alpha
        // cada frame y el logo nunca llega a verse desvanecerse desde 0.
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
        if (!rect) return;
        float target = hovering ? hoverScale : 1f;
        Vector3 targetScale = original * target;
        rect.localScale = Vector3.Lerp(rect.localScale, targetScale, Time.unscaledDeltaTime * scaleSpeed);
    }

    // ---------------------------------------------------------------- sombra

    Shadow GetOrAddShadow(Transform target)
    {
        if (!target) return null;
        Shadow s = target.gameObject.GetComponent<Shadow>();
        if (!s) s = target.gameObject.AddComponent<Shadow>();
        s.effectColor = new Color(0, 0, 0, 0.6f);
        s.effectDistance = shadowOffsetNormal;
        return s;
    }

    void AnimateShadow(Shadow shadow, bool hovering)
    {
        if (!shadow) return;
        Vector2 target = hovering ? shadowOffsetHover : shadowOffsetNormal;
        shadow.effectDistance = Vector2.Lerp(shadow.effectDistance, target, Time.unscaledDeltaTime * shadowSpeed);
    }

    // ---------------------------------------------------------------- transicion

    void CreateFadeOverlay()
    {
        GameObject overlay = new GameObject("FadeOverlay");
        overlay.transform.SetParent(transform, false);

        RectTransform rt = overlay.AddComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;

        Image img = overlay.AddComponent<Image>();
        img.color = Color.black;

        fadeOverlay = overlay.AddComponent<CanvasGroup>();
        fadeOverlay.alpha = 0f;
        fadeOverlay.blocksRaycasts = false;
    }

    public void StartTransition()
    {
        if (transitioning)
            return;

        // Sin overlay (por ejemplo si no se encontro el objeto "Menu") no hay fundido que hacer.
        if (fadeOverlay == null)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("SampleScene");
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

        yield return new WaitForSecondsRealtime(0.2f);

        UnityEngine.SceneManagement.SceneManager.LoadScene("SampleScene");
    }

    // ---------------------------------------------------------------- hover

    void SetupHover(Transform target, string nombre)
    {
        if (!target) return;
        Button btn = target.GetComponent<Button>();
        if (!btn) return;

        EventTrigger trigger = target.gameObject.GetComponent<EventTrigger>();
        if (!trigger) trigger = target.gameObject.AddComponent<EventTrigger>();

        trigger.triggers.Clear();

        EventTrigger.Entry enterEntry = new EventTrigger.Entry();
        enterEntry.eventID = EventTriggerType.PointerEnter;
        if (nombre == "Jugar")
            enterEntry.callback.AddListener((data) => { jugarHover = true; });
        else
            enterEntry.callback.AddListener((data) => { salirHover = true; });
        trigger.triggers.Add(enterEntry);

        EventTrigger.Entry exitEntry = new EventTrigger.Entry();
        exitEntry.eventID = EventTriggerType.PointerExit;
        if (nombre == "Jugar")
            exitEntry.callback.AddListener((data) => { jugarHover = false; });
        else
            exitEntry.callback.AddListener((data) => { salirHover = false; });
        trigger.triggers.Add(exitEntry);
    }
}
