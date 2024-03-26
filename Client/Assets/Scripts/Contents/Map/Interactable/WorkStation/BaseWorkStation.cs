using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public abstract class BaseWorkStation : BaseInteractable
{
    [Networked] public float RequiredWorkAmount { get; set; }
    [Networked] public float CurrentWorkAmount { get; set; }
    [Networked] public NetworkBool IsRememberWork { get; set; }
    [Networked] public NetworkBool IsCompleted { get; set; }
    [Networked] public NetworkBool IsSomeoneWork { get; set; }

    public Creature CurrentWorkCreature { get; protected set; }
    public UI_WorkProgressBar ProgressBarUI { get; protected set; }

    public override void Spawned()
    {
        Init();
    }

    public virtual void Init()
    {
        CurrentWorkAmount = 0f;
    }

    public override void CheckAndInteract(Creature creature)
    {
        if (IsCompleted || IsSomeoneWork)
            return;

        IsSomeoneWork = true;
        CurrentWorkCreature = creature;
        ((Crew)CurrentWorkCreature).CreatureState = Define.CreatureState.Interact;
        ((Crew)CurrentWorkCreature).CrewAnimController.PlayKeypadUse();

        ProgressBarUI = ((UI_CrewIngame)Managers.UIMng.SceneUI).ShowWorkProgressBar("Fixing Computer", RequiredWorkAmount);

        Debug.Log($"{CurrentWorkCreature.NetworkObject.Id}: Start Work");

        StartCoroutine(WorkProgress());
    }

    public void OnWorkInterrupt()
    {
        IsSomeoneWork = false;
        CurrentWorkCreature.CreatureState = Define.CreatureState.Idle;
        ProgressBarUI.gameObject.SetActive(false);
        StopAllCoroutines();

        if (!IsRememberWork)
            CurrentWorkAmount = 0f;

        Debug.Log($"{CurrentWorkCreature.NetworkObject.Id}: Interrupt Work"); // TODO - Test code
    }

    protected abstract IEnumerator WorkProgress();
}
