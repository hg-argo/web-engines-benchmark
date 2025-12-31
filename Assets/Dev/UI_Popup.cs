using System.Collections;

using UnityEngine;
using UnityEngine.UI;

public class UI_Popup : MonoBehaviour
{

    private const string IsVisibleParam = "isVisible";

    [Header("References")]

    public RectTransform animatedContainer = null;
    public Text titleText = null;
    public Text messageText = null;

    [Header("Settings")]

    [Tooltip("The time (in seconds) to wait after the player shot for the UI popup appears on screen.")]
    public float uiDelay = 6f;

    [Header("Animations")]

    public float slideInDuration = .6f;
    public AnimationCurve slideInYPositionCurve;

    [Space]

    public float slideOutDuration = .6f;
    public AnimationCurve slideOutYPositionCurve;

    private Vector2 _initAnchoredPosition = Vector2.zero;

    private Vector2 HiddenAnchoredPosition
    {
        get
        {
            float parentContainerHeight = (animatedContainer.parent is RectTransform containerRectTransform)
                ? containerRectTransform.rect.height
                : 0f;
            float selfHeight = animatedContainer.rect.height / 2;
            return _initAnchoredPosition + Vector2.down * (parentContainerHeight / 2 + selfHeight);
        }
    }

    private void Awake()
    {
        if (animatedContainer == null)
            animatedContainer = GetComponent<RectTransform>();

        animatedContainer.anchoredPosition = HiddenAnchoredPosition;
        animatedContainer.gameObject.SetActive(false);
    }

    private void OnValidate()
    {
        if (animatedContainer == null)
            animatedContainer = GetComponent<RectTransform>();
    }

    public void OnShoot(ShotResult result)
    {
        titleText.text = result.IsSuccess ? "Victory!" : "Missed!";
        messageText.text = result.IsSuccess
            ? "Congratulations! The crowd is cheering for you!"
            : "You didn't make it... Let's try again!";

        StartCoroutine(SlideIn(uiDelay));
    }

    public void HidePopup()
    {
        StartCoroutine(SlideOut());
    }

    private IEnumerator SlideIn(float delay = 0f)
    {
        if (delay > 0)
            yield return new WaitForSeconds(delay);

        animatedContainer.anchoredPosition = HiddenAnchoredPosition;
        yield return AnimateSlide(slideInDuration, slideInYPositionCurve, false);
    }

    private IEnumerator SlideOut()
    {
        yield return AnimateSlide(slideOutDuration, slideOutYPositionCurve, true);
    }

    private IEnumerator AnimateSlide(float durationn, AnimationCurve curve, bool slideOut)
    {
        Vector2 from = slideOut ? _initAnchoredPosition : HiddenAnchoredPosition;
        Vector2 to = slideOut ? HiddenAnchoredPosition : _initAnchoredPosition;
        animatedContainer.gameObject.SetActive(true);

        float timer = 0f;
        while (timer < durationn)
        {
            float timerRatio = Mathf.Clamp01(timer / durationn);
            animatedContainer.anchoredPosition = Vector2.LerpUnclamped(from, to, curve.Evaluate(timerRatio));
            timer += Time.deltaTime;
            yield return null;
        }

        animatedContainer.anchoredPosition = to;
        animatedContainer.gameObject.SetActive(!slideOut);
    }

}
