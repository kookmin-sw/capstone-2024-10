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
    }

    public void SetChromaticAberration(float value)
    {
        ChromaticAberration.intensity.value = (100f - value) * 0.01f;
    }

    public void SetColorAdjustments(bool value)
    {
        if (value)
            ColorAdjustments.colorFilter.value = new Color(76f, 76f, 255f);
        else
            ColorAdjustments.colorFilter.value = new Color(255f, 255f, 255f);
    }

    public void SetVignette(float value)
    {
        Vignette.intensity.value = (100f - value) * 0.01f * 0.7f;
    }
}

