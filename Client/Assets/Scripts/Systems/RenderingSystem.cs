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
    public Fog Fog { get; protected set; }
    public ChromaticAberration ChromaticAberration { get; protected set; }
    public ColorAdjustments ColorAdjustments { get; protected set; }
    public IndirectLightingController IndirectLightingController { get; protected set; }
    public Vignette Vignette { get; protected set; }

    public Material DamageMaterial { get; protected set; }

    private Tweener _setChromaticAberrationTweener;
    private Tweener _setColorAdjustmentsTweener;
    private Tweener _setVignetteTweener;
    private Tweener _getBlindTweener;
    private Color _erosionColor = new Color(255.0f / 255.0f, 76.0f / 255.0f, 76.0f / 255.0f);
    private Color _erosionColor2 = new Color(76.0f / 255.0f, 76.0f / 255.0f, 255.0f / 255.0f);
    private Color _defaultColor = new Color(255.0f / 255.0f, 255.0f / 255.0f, 255.0f / 255.0f);
    private float _damageEffectSpeed = 1.5f;


    public override void Spawned()
    {
        Init();
    }

    public void Init()
    {
        Managers.GameMng.RenderingSystem = this;

        Volume = GetComponent<Volume>();

        if (VolumeProfile.TryGet<Fog>(out var fog))
            Fog = fog;
        if (VolumeProfile.TryGet<ChromaticAberration>(out var chromaticAberration))
            ChromaticAberration = chromaticAberration;
        if (VolumeProfile.TryGet<ColorAdjustments>(out var colorAdjustments))
            ColorAdjustments = colorAdjustments;
        if (VolumeProfile.TryGet<IndirectLightingController>(out var indirectLightingController))
            IndirectLightingController = indirectLightingController;
        if (VolumeProfile.TryGet<Vignette>(out var vignette))
            Vignette = vignette;

        DamageMaterial = Managers.ResourceMng.Load<Material>("Material/DamageMaterial");

        ChromaticAberration.intensity.value = 0f;
        ColorAdjustments.colorFilter.value = _defaultColor;
        Vignette.intensity.value = 0.2f;
        IndirectLightingController.indirectDiffuseLightingMultiplier.value = 0f;

        DamageEffect(3);
    }

    #region Volume

    public void SetChromaticAberration(float sanity)
    {
        ChromaticAberration.intensity.value = (100f - sanity) * 0.01f;

        _setChromaticAberrationTweener.Kill();
        _setChromaticAberrationTweener = DOVirtual.Float(ChromaticAberration.intensity.value,
            (100f - sanity) * 0.01f, 1f, value =>
            {
                ChromaticAberration.intensity.value = value;
            });
    }

    public void SetColorAdjustments(bool isErosion)
    {
        Color color = _defaultColor;

        if (isErosion)
            color = _erosionColor;

        _setColorAdjustmentsTweener.Kill();
        _setColorAdjustmentsTweener = DOVirtual.Color(ColorAdjustments.colorFilter.value, color, 2f, value =>
        {
            ColorAdjustments.colorFilter.value = value;
        });
    }

    public void SetVignette(float sanity)
    {
        _setVignetteTweener.Kill();
        _setVignetteTweener = DOVirtual.Float(Vignette.intensity.value,
            0.2f + (100f - sanity) * 0.01f * 0.55f, 1f, value =>
        {
            Vignette.intensity.value = value;
        });
    }

    public void GetBlind(float blindTime, float backTime)
    {
        _getBlindTweener.Kill();
        IndirectLightingController.indirectDiffuseLightingMultiplier.value = 5000f;

        DOVirtual.DelayedCall(blindTime, () =>
        {
            _getBlindTweener.Kill();
            _getBlindTweener = DOVirtual.Float(IndirectLightingController.indirectDiffuseLightingMultiplier.value, 0, backTime, value =>
            {
                IndirectLightingController.indirectDiffuseLightingMultiplier.value = value;
            });
        });
    }

    #endregion

    #region Custom Pass Volume

    public void DamageEffect(int hp)
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

        StartCoroutine(DamageEffectProgress(ratio));
    }

    private IEnumerator DamageEffectProgress(float intensity)
    {
        float targetRadius = Mathf.Lerp(1.2f, -1f, Mathf.InverseLerp(0, 1, intensity));
        //float targetRadius = Remap(intensity, 0, 1, 1.2f, -1f);
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

        // Camera shake
        //Vector3 velocity = new Vector3(0, -0.5f, -1);
        //velocity.Normalize();
        //impulseSource.GenerateImpulse(velocity * intensity * 0.4f);
    }

    #endregion
}

