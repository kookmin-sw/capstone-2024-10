using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class SectorLight: NetworkBehaviour
{
    private Light[] _lights;

    public override void Spawned()
    {
        _lights = GetComponentsInChildren<Light>();
    }
}
