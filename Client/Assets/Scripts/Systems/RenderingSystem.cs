using System.Collections;
using DG.Tweening;
using Fusion;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class RenderingSystem : NetworkBehaviour
{
    public Volume Volume { get; protected set; }
    public VolumeProfile VolumeProfile => Volume.sharedProfile;
    public ChromaticAberration ChromaticAberration { get; protected set; }
    public ColorAdjustments ColorAdjustments { get; protected set; }
    public Vignette Vignette { get; protected set; }

    public Material DamageMaterial { get; protected set; }

    private Color _erosionColor = new Color(255.0f / 255.0f, 90.0f / 255.0f, 90.0f / 255.0f);
    private Color _defaultColor = new Color(255.0f / 255.0f, 255.0f / 255.0f, 255.0f / 255.0f);

    private float _defaultVignetteValue = 0.15f;

    private float _blindValue = 15f;
    private float _damageEffectSpeed = 1.5f;

    private Tweener _erosionEffectTweener;
    private Tweener _blindEffectTweener;

    public override void Spawned()
    {
        Init();
    }

    public void Init()
    {
        Managers.GameMng.RenderingSystem = this;

        Volume = GetComponent<Volume>();

        if (VolumeProfile.TryGet<ChromaticAberration>(out var chromaticAberration))
            ChromaticAberration = chromaticAberration;
        if (VolumeProfile.TryGet<ColorAdjustments>(out var colorAdjustments))
            ColorAdjustments = colorAdjustments;
        if (VolumeProfile.TryGet<Vignette>(out var vignette))
            Vignette = vignette;

        DamageMaterial = Managers.ResourceMng.Load<Material>("Materials/DamageMaterial");

        ChromaticAberration.intensity.value = 0f;
        ColorAdjustments.postExposure.value = 0f;
        ColorAdjustments.colorFilter.value = _defaultColor;
        Vignette.intensity.value = _defaultVignetteValue;

        ApplyDamageEffect(3);
    }

    #region Volume

    public void ApplySanityEffect(float sanity)
    {
        Vignette.intensity.value = _defaultVignetteValue + (100f - sanity) * 0.01f * 0.4f;
        ChromaticAberration.intensity.value = (100f - sanity) * 0.01f;
    }

    public void ApplyErosionEffect(bool isApplying)
    {
        Color color = _defaultColor;
        float vignetteValue = _defaultVignetteValue;

        if (isApplying)
        {
            color = _erosionColor;
            vignetteValue = 0f;
        }

        _erosionEffectTweener.Kill();
        _erosionEffectTweener = DOVirtual.Color(ColorAdjustments.colorFilter.value, color, 2f, value =>
        {
            ColorAdjustments.colorFilter.value = value;
        });

        if (Managers.ObjectMng.MyCreature is Alien)
            Vignette.intensity.value = vignetteValue;
    }

    public void ApplyBlindEffect(float blindTime, float backTime)
    {
        _blindEffectTweener.Kill();
        ColorAdjustments.postExposure.value = _blindValue;

        DOVirtual.DelayedCall(blindTime, () =>
        {
            _blindEffectTweener.Kill();
            _blindEffectTweener = DOVirtual.Float(ColorAdjustments.postExposure.value, 0f, backTime, value =>
            {
                ColorAdjustments.postExposure.value = value;
            });
        });
    }

    #endregion

    #region Custom Pass Volume

    public void ApplyDamageEffect(int hp)
    {
        float ratio;
        if (hp >= 3)
            ratio = 0f;
        else if (hp == 2)
            ratio = 0.4f;
        else if (hp == 1)
            ratio = 0.6f;
        else
            ratio = 1f;

        StartCoroutine(ProgressDamageEffect(ratio));
    }

    private IEnumerator ProgressDamageEffect(float intensity)
    {
        float targetRadius = Mathf.Lerp(1.2f, -1f, Mathf.InverseLerp(0, 1, intensity));
        float curRadius = 1; // No damage
        for (float t = 0; curRadius != targetRadius; t += Time.deltaTime * _damageEffectSpeed)
        {
            curRadius = Mathf.Lerp(1, targetRadius, t);
            DamageMaterial.SetFloat("_Vignette_radius", curRadius);
            yield return null;
        }

        if (intensity < 1)
        {
            for (float t = 0; curRadius < 1; t += Time.deltaTime * _damageEffectSpeed)
            {
                curRadius = Mathf.Lerp(targetRadius, 1, t);
                DamageMaterial.SetFloat("_Vignette_radius", curRadius);
                yield return null;
            }
        }
    }

    #endregion
}

