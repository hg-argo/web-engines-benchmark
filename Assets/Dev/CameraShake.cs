using UnityEngine;

public class CameraShake : MonoBehaviour
{

    public Camera cameraToShake = null;

    [Tooltip("The furation (in seconds) of the effect.")]
    public float duration = 0.5f;

    [Tooltip("The maximum distance (in units) the cameraToShake can travel from its initial position.")]
    public float intensity = 0.3f;

    [Tooltip("Defines how often the cameraToShake changes direction during the effect.")]
    public float vibration = 25f;

    [Tooltip("Defines how the effects fades over time (X is the duration of the effect between 0 and 1, Y is the effect factor between 0 and 1).")]
    public AnimationCurve fade = AnimationCurve.EaseInOut(0, 1, 1, 0);

    private Vector3 _initialPosition;
    private float _timer;
    private bool _isPlaying;
    private float _noiseTime;
    private float _seedX;
    private float _seedY;

    public bool IsPlaying => _isPlaying && _timer < duration;

    private void Awake()
    {
        if (cameraToShake == null)
        {
            cameraToShake = GetComponent<Camera>();
            if (cameraToShake == null)
                cameraToShake = Camera.main;
        }

        _initialPosition = cameraToShake.transform.localPosition;
        _seedX = Random.Range(0f, 1000f);
        _seedY = Random.Range(0f, 1000f);
    }

    private void OnValidate()
    {
        if (cameraToShake == null)
            cameraToShake = GetComponent<Camera>();
    }

    private void Update()
    {
        // Cancel if the effect is not playing
        if (!_isPlaying)
            return;

        // Evaluate timer
        _timer += Time.deltaTime;
        float normalizedTime = Mathf.Clamp01(_timer / duration);
        float fade = this.fade.Evaluate(normalizedTime);

        // Calculate pseudorandom position
        _noiseTime += Time.deltaTime * vibration;
        float x = (Mathf.PerlinNoise(_seedX, _noiseTime) - 0.5f) * 2f;
        float y = (Mathf.PerlinNoise(_seedY, _noiseTime) - 0.5f) * 2f;

        // Apply offset
        Vector3 offset = new Vector3(x, y, 0f) * (intensity * fade);
        cameraToShake.transform.localPosition = _initialPosition + offset;

        // Stop effect if the timer is elapsed
        if (_timer >= duration)
            Stop();
    }

    /// <summary>
    /// Restarts the effect.
    /// </summary>
    public void Play()
    {
        _timer = 0f;
        _noiseTime = 0f;
        _isPlaying = true;
        cameraToShake.transform.localPosition = _initialPosition;
    }

    /// <summary>
    /// Stops the effect.
    /// </summary>
    public void Stop()
    {
        _isPlaying = false;
        _timer = 0f;
        cameraToShake.transform.localPosition = _initialPosition;
    }
}