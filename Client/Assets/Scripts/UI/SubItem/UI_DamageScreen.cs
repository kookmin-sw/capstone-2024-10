using System.Collections;
using UnityEngine;

public class UI_DamageScreen : UI_Base
{
    public Material screenDamageMat;
    //public CinemachineImpulseSource impulseSource;
    private Coroutine screenDamageTask;

    private static UI_DamageScreen instance;

    private float speed = 1.5f;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        instance = this;
        screenDamageMat.SetFloat("_Vignette_radius", 1f);
        return true;
    }

    private void ScreenDamageEffect(float intensity)
    {
        if (screenDamageTask != null)
        {
            StopCoroutine(screenDamageTask);
        }
        screenDamageTask = StartCoroutine(ScreenDamage(intensity));
    }

    private IEnumerator ScreenDamage(float intensity)
    {
        // Camera shake
        //Vector3 velocity = new Vector3(0, -0.5f, -1);
        //velocity.Normalize();
        //impulseSource.GenerateImpulse(velocity * intensity * 0.4f);

        // Screen effect
        float targetRadius = Remap(intensity, 0, 1, 1.2f, -1f);
        float curRadius = 1; // No damage
        for (float t = 0; curRadius != targetRadius; t += Time.deltaTime * speed)
        {
            curRadius = Mathf.Lerp(1, targetRadius, t);
            screenDamageMat.SetFloat("_Vignette_radius", curRadius);
            yield return null;
        }

        if (intensity < 1)
        {
            for (float t = 0; curRadius < 1; t += Time.deltaTime * speed)
            {
                curRadius = Mathf.Lerp(targetRadius, 1, t);
                screenDamageMat.SetFloat("_Vignette_radius", curRadius);
                yield return null;
            }
        }
    }

    private float Remap(float value, float fromMin, float fromMax, float toMin, float toMax)
    {
        return Mathf.Lerp(toMin, toMax, Mathf.InverseLerp(fromMin, fromMax, value));
    }

    public static class DamageEffects
    {
        public static void ScreenDamageEffect(float intensity) => instance.ScreenDamageEffect(intensity);
    }
}
