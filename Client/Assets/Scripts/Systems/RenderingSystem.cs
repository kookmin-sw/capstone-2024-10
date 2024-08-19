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
    public Exposure Exposure { get; protected set; }
    public Fog Fog { get; protected set; }
    public ChromaticAberration ChromaticAberration { get; protected set; }
    public ColorAdjustments ColorAdjustments { get; protected set; }
    public Vignette Vignette { get; protected set; }

    public Material DamageMaterial { get; protected set; }

    private Color _erosionColor = new Color(255.0f / 255.0f, 90.0f / 255.0f, 90.0f / 255.0f);
    private Color _defaultColor = new Color(255.0f / 255.0f, 255.0f / 255.0f, 255.0f / 255.0f);

    private float _defaultFixedExposure = -3f;
    private float _defaultFogMeanFreePath = 5.5f;
    private float _defaultVignetteIntensity = 0.1f;

    private float _blindValue = 15f;

    private Tweener _erosionEffectTweener;
    private Tweener _erosionEffectTweener2;
    private Tweener _blindEffectTweener;
    private Tweener _damageEffectTweener;

    public override void Spawned()
    {
        Init();
    }

    public void Init()
    {
        Managers.GameMng.RenderingSystem = this;

        Volume = GetComponent<Volume>();

        if (VolumeProfile.TryGet<Exposure>(out var exposure))
            Exposure = exposure;
        if (VolumeProfile.TryGet<Fog>(out var fog))
            Fog = fog;
        if (VolumeProfile.TryGet<ChromaticAberration>(out var chromaticAberration))
            ChromaticAberration = chromaticAberration;
        if (VolumeProfile.TryGet<ColorAdjustments>(out var colorAdjustments))
            ColorAdjustments = colorAdjustments;
        if (VolumeProfile.TryGet<Vignette>(out var vignette))
            Vignette = vignette;

        DamageMaterial = Managers.ResourceMng.Load<Material>("Materials/DamageMaterial");

        Exposure.fixedExposure.value =_defaultFixedExposure;
        Fog.meanFreePath.value =_defaultFogMeanFreePath;
        ChromaticAberration.intensity.value = 0f;
        ColorAdjustments.postExposure.value = 0f;
        ColorAdjustments.colorFilter.value = _defaultColor;
        Vignette.intensity.value = _defaultVignetteIntensity;

        ApplyDamageEffect(3);
    }

    #region Volume

    public void ApplySanityEffect(float sanity)
    {
        Vignette.intensity.value = _defaultVignetteIntensity + (100f - sanity) * 0.01f * 0.5f;
        //ChromaticAberration.intensity.value = (100f - sanity) * 0.01f;
    }

    public void ApplyErosionEffect(bool isApplying)
    {
        Color color = _defaultColor;
        float fogMeanFreePath = _defaultFogMeanFreePath;

        if (isApplying)
        {
            color = _erosionColor;
            fogMeanFreePath = 10f;
        }

        _erosionEffectTweener.Kill();
        _erosionEffectTweener = DOVirtual.Color(ColorAdjustments.colorFilter.value, color, 1f, value =>
        {
            ColorAdjustments.colorFilter.value = value;
        });

        if (Managers.ObjectMng.MyCreature is Alien)
        {
            _erosionEffectTweener2.Kill();
            _erosionEffectTweener2 = DOVirtual.Float(Fog.meanFreePath.value, fogMeanFreePath, 1f, value =>
            {
                Fog.meanFreePath.value = value;
            });
        }
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
        float ratio, speed = 1.5f;
        if (hp >= 3)
            ratio = 0f;
        else if (hp == 2)
            ratio = 0.4f;
        else if (hp == 1)
            ratio = 0.6f;
        else
        {
            ratio = 1f;
            speed = 0.15f;
        }

        StartCoroutine(ProgressDamageEffect(ratio, speed));

        if (hp >= 3)
            return;

        _damageEffectTweener.Kill();
        ChromaticAberration.intensity.value = 100f;

        DOVirtual.DelayedCall(5.5f, () =>
        {
            _damageEffectTweener.Kill();
            _damageEffectTweener = DOVirtual.Float(ChromaticAberration.intensity.value, 0f, 2f, value =>
            {
                ChromaticAberration.intensity.value = value;
            });
        });
    }

    private IEnumerator ProgressDamageEffect(float intensity, float effectSpeed)
    {
        float targetRadius = Mathf.Lerp(1.2f, -1f, Mathf.InverseLerp(0, 1, intensity));
        float curRadius = 1; // No damage
        for (float t = 0; curRadius != targetRadius; t += Time.deltaTime * effectSpeed)
        {
            curRadius = Mathf.Lerp(1, targetRadius, t);
            DamageMaterial.SetFloat("_Vignette_radius", curRadius);
            yield return null;
        }

        if (intensity < 1)
        {
            for (float t = 0; curRadius < 1; t += Time.deltaTime * effectSpeed)
            {
                curRadius = Mathf.Lerp(targetRadius, 1, t);
                DamageMaterial.SetFloat("_Vignette_radius", curRadius);
                yield return null;
            }
        }
    }

    public void SetAlienOutlinePassVolume()
    {
        CustomPassVolume[] volumes = GetComponents<CustomPassVolume>();

        foreach (var volume in volumes)
        {
            foreach (var pass in volume.customPasses)
            {
                if (pass is DrawRenderersCustomPass dr && dr.name == "OutlineObject")
                {
                    dr.depthCompareFunction = CompareFunction.Disabled;
                    dr.layerMask = LayerMask.GetMask("PlanTargetObject");
                }
            }
        }
    }

    #endregion
}

