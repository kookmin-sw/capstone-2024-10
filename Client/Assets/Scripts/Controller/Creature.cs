using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using UnityEngine.AI;

public abstract class Creature : NetworkBehaviour
{
    public Define.CreatureType WorldObjectType { get; protected set; }

    private void Start()
    {
        Init();
    }

    protected virtual void Init()
    {
    }
}
