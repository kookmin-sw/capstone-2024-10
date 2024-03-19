using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class Door : NetworkBehaviour, IInteractable
{
    [Networked]
    public NetworkBool IsOpen { get; set; }

    private Animator _animator;
    private int _isOpenParameterId;

    public override void Spawned()
    {
        base.Spawned();
        _animator = transform.GetComponent<Animator>();
        _isOpenParameterId = Animator.StringToHash("isOpen");
    }

    public void Interact(Creature creature)
    {
        RPC_Open();
        creature.CreatureState = Define.CreatureState.Idle;
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    private void RPC_Open()
    {
        IsOpen = !IsOpen;
        _animator.SetBool(_isOpenParameterId, IsOpen);
    }
}
