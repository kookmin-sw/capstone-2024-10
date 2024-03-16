using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class Door : WorkStation
{
    [Networked]
    public NetworkBool IsOpen { get; set; }

    public override void Interact(Creature creature)
    {

    }
}
