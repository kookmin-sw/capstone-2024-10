using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

/// <summary>
/// 상호작용 시 일정 시간 이상 시간을 소요해야 하는 오브젝트
/// </summary>
public abstract class WorkStation : NetworkBehaviour, IInteractable
{
    [SerializeField] protected float _requiredWorkAmount;
    [SerializeField] protected bool _isContinuable;
    [SerializeField] protected bool _isMultiWorkAllowed;

    [Networked] public NetworkBool IsCompleted { get; set; }
    [Networked] public float WorkProgress { get; set; }

    [Networked, Capacity(3)] public NetworkLinkedList<NetworkId> WorkingCreatures { get; } = new();
    protected Creature _workingCreature;
    protected UI_WorkProgressBar _progressbar;

    public override void Spawned()
    {
        WorkProgress = 0;
    }

    public virtual void Interact(Creature creature)
    {
        if (IsCompleted) return;
        if (!_isMultiWorkAllowed && WorkingCreatures.Count > 0)
        {
            Debug.Log("Other player is interacting");
            return;
        }
        if (!_isContinuable && WorkingCreatures.Count == 0)
        {
            Debug.Log("Work progress reset");
            WorkProgress = 0;
        }
        WorkingCreatures.Add(creature.NetworkObject.Id);
        _workingCreature = creature;
        StartCoroutine(ProgressWork());
    }

    public void Interrupt()
    {
        Debug.Log($"{_workingCreature.NetworkObject.Id}: Interrupted work");
        _progressbar.gameObject.SetActive(false);
        WorkingCreatures.Remove(_workingCreature.NetworkObject.Id);
        StopAllCoroutines();
    }

    protected abstract IEnumerator ProgressWork();

    protected abstract void OnWorkComplete();
}
