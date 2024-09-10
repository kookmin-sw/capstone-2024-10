using System.Collections;
using UnityEngine;

public class SectorLight : MonoBehaviour
{
    private Light _light;
    private float _currentIntensity => _light.intensity;
    private float _originalIntensity;

    public void Init()
    {
        _light = GetComponent<Light>();
        _originalIntensity = _light.intensity;
    }

    public void EnableLight()
    {
        StopAllCoroutines();
        _light.enabled = true;
        StartCoroutine(LightIntensityLerp(1, _originalIntensity));
    }

    public void DisableLight()
    {
        StopAllCoroutines();
        StartCoroutine(LightIntensityLerp(1, 0));
    }

    private IEnumerator LightIntensityLerp(float duration, float target)
    {
        var t = 0f;
        var start = _currentIntensity;

        while (t < 1)
        {
            t += Time.deltaTime / duration;

            if (t > 1) t = 1;

            _light.intensity = Mathf.Lerp(start, target, t);

            yield return null;
        }

        _light.intensity = target;
        if (target == 0) _light.enabled = false;
    }
}
