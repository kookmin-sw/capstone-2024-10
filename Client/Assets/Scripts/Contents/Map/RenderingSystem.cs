using Fusion;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class RenderingSystem : NetworkBehaviour
{
    public Volume Volume { get; protected set; }
    public VolumeProfile VolumeProfile { get; protected set; }
    public Bloom Bloom { get; protected set; }
    public ChromaticAberration ChromaticAberration { get; protected set; }
    public LiftGammaGain LiftGammaGain { get; protected set; }
    public ColorAdjustments ColorAdjustments { get; protected set; }
    public Fog Fog { get; protected set; }
    public Vignette Vignette { get; protected set; }

    public void Init()
    {
        Managers.MapMng.RenderingSystem = this;

        Volume = Util.FindChild(gameObject, "Volume").GetComponent<Volume>();
        VolumeProfile = Volume.GetComponent<Volume>().sharedProfile;
    }
}

