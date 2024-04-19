using System.Collections;
using Fusion;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class RenderingSystem : NetworkBehaviour
{
    public Volume Volume { get; protected set; }
    public VolumeProfile VolumeProfile => Volume.sharedProfile;
    public Fog Fog { get; protected set; }
    public ChromaticAberration ChromaticAberration { get; protected set; }
    public ColorAdjustments ColorAdjustments { get; protected set; }
    public Vignette Vignette { get; protected set; }

    public Material DamageMaterial { get; protected set; }

    private float speed = 1.5f;

    public override void Spawned()
    {
        Init();
    }

    public void Init()
    {
        Managers.MapMng.RenderingSystem = this;

        Volume = Util.FindChild(gameObject, "Volume").GetComponent<Volume>();

        if (VolumeProfile.TryGet<Fog>(out var fog))
            Fog = fog;
        if (VolumeProfile.TryGet<ChromaticAberration>(out var chromaticAberration))
            ChromaticAberration = chromaticAberration;
        if (VolumeProfile.TryGet<ColorAdjustments>(out var colorAdjustments))
            ColorAdjustments = colorAdjustments;
        if (VolumeProfile.TryGet<Vignette>(out var vignette))
            Vignette = vignette;

        DamageMaterial = Managers.ResourceMng.Load<Material>("Material/DamageMaterial");

        StartCoroutine(DamageEffectProgress(0f));
    }

    #region Volume

    public void SetChromaticAberration(float sanity)
    {
        ChromaticAberration.intensity.value = (100f - sanity) * 0.01f;
    }

    public void SetColorAdjustments(bool value)
    {
        if (value)
            ColorAdjustments.colorFilter.value = new Color(76f, 76f, 255f);
        else
            ColorAdjustments.colorFilter.value = new Color(255f, 255f, 255f);
    }

    public void SetVignette(float sanity)
    {
        Vignette.intensity.value = (100f - sanity) * 0.01f * 0.7f;
    }

    #endregion

    #region Custom Pass Volume

    public void DamageEffect(int hp)
    {
        float ratio = 0f;

        if (hp >= 2)
            ratio = 0.3f;
        else if (hp == 1)
            ratio = 0.5f;
        else if (hp <= 0)
            ratio = 1f;

        StartCoroutine(DamageEffectProgress(ratio));
    }

    private IEnumerator DamageEffectProgress(float intensity)
    {
        float targetRadius = Mathf.Lerp(1.2f, -1f, Mathf.InverseLerp(0, 1, intensity));
        //float targetRadius = Remap(intensity, 0, 1, 1.2f, -1f);
        float curRadius = 1; // No damage
        for (float t = 0; curRadius != targetRadius; t += Time.deltaTime * speed)
        {
            curRadius = Mathf.Lerp(1, targetRadius, t);
            DamageMaterial.SetFloat("_Vignette_radius", curRadius);
            yield return null;
        }

        if (intensity < 1)
        {
            for (float t = 0; curRadius < 1; t += Time.deltaTime * speed)
            {
                curRadius = Mathf.Lerp(targetRadius, 1, t);
                DamageMaterial.SetFloat("_Vignette_radius", curRadius);
                yield return null;
            }
        }

        // Camera shake
        //Vector3 velocity = new Vector3(0, -0.5f, -1);
        //velocity.Normalize();
        //impulseSource.GenerateImpulse(velocity * intensity * 0.4f);
    }

    private float Remap(float value, float fromMin, float fromMax, float toMin, float toMax)
    {
        return Mathf.Lerp(toMin, toMax, Mathf.InverseLerp(fromMin, fromMax, value));
    }

    #endregion
}

