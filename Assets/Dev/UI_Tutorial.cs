using System.Collections;

using UnityEngine;

public class UI_Tutorial : MonoBehaviour
{

    public CanvasGroup tutorialObject = null;
    public float showTutorialDelay = 5f;
    public float fadeDuration = .6f;

    private Coroutine fadeCoroutine = null;

    private void Awake()
    {
        tutorialObject.alpha = 0;
        tutorialObject.gameObject.SetActive(false);
    }

    private void Start()
    {
        OnRestart();
    }

    public void OnShoot()
    {
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(Fade(0));
    }

    public void OnRestart()
    {
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(Fade(1, showTutorialDelay));
    }

    private IEnumerator Fade(float targetAlpha, float delay = 0)
    {
        float originAlpha = tutorialObject.alpha;
        if (originAlpha != targetAlpha)
        {
            if (delay > 0)
                yield return new WaitForSeconds(delay);

            tutorialObject.gameObject.SetActive(true);

            float timer = 0f;
            while (timer < fadeDuration)
            {
                tutorialObject.alpha = Mathf.Lerp(originAlpha, targetAlpha, timer / fadeDuration);
                timer += Time.deltaTime;
                yield return null;
            }
        }

        tutorialObject.alpha = targetAlpha;
        tutorialObject.gameObject.SetActive(targetAlpha > 0);
        fadeCoroutine = null;
    }

}
