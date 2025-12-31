using System.Collections;

using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PostProcessFeedbacks : MonoBehaviour
{

    public Volume postProcessVolume = null;

    [Header("Success Anim")]

    public float successAnimDuration = 1f;
    public AnimationCurve successChromaticAberrationCurve;
    public AnimationCurve successBloomIntensityCurve;

    private ChromaticAberration chromaticAberration = null;
    private Bloom bloom = null;
    private Coroutine postProcessCoroutine = null;

    private void Awake()
    {
        if (postProcessVolume == null)
            postProcessVolume = GetComponent<Volume>();

        postProcessVolume.profile.TryGet(out chromaticAberration);
        postProcessVolume.profile.TryGet(out bloom);
    }

    private void OnValidate()
    {
        if (postProcessVolume == null)
            postProcessVolume = GetComponent<Volume>();
    }

    public void OnSuccess()
    {
        if (postProcessCoroutine != null)
            return;

        postProcessCoroutine = StartCoroutine(SuccessAnim());
    }

    private IEnumerator SuccessAnim()
    {
        float chromaticAberrationInitValue = chromaticAberration.intensity.value;
        float bloomInitValue = bloom.intensity.value;

        float timer = 0;
        while (timer < successAnimDuration)
        {
            float timerRatio = Mathf.Clamp01(timer / successAnimDuration);
            chromaticAberration.intensity.value = successChromaticAberrationCurve.Evaluate(timerRatio);
            bloom.intensity.value = successBloomIntensityCurve.Evaluate(timerRatio);
            timer += Time.deltaTime;
            yield return null;
        }

        chromaticAberration.intensity.value = chromaticAberrationInitValue;
        bloom.intensity.value = bloomInitValue;
        postProcessCoroutine = null;
    }

}
