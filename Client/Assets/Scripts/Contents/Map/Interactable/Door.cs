using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class Door : BaseWorkStation
{
    public Crew CurrentWorkCrew => (Crew)CurrentWorkCreature;

    [Networked] public NetworkBool IsOpen { get; set; }

    public NetworkMecanimAnimator NetworkAnim { get; protected set; }

    public override void Init()
    {
        base.Init();

        NetworkAnim = transform.GetComponent<NetworkMecanimAnimator>();

        IsRememberWork = false;
        IsCompleted = false;
        IsSomeoneWork = false;

        RequiredWorkAmount = 5f;
    }

    protected override IEnumerator WorkProgress()
    {
        while (CurrentWorkAmount < RequiredWorkAmount)
        {
            if (CurrentWorkCreature.CreatureState != Define.CreatureState.Interact)
                OnWorkInterrupt();

            CurrentWorkAmount += Time.deltaTime * CurrentWorkCrew.CrewStat.WorkSpeed;
            ProgressBarUI.CurrentWorkAmount = CurrentWorkAmount;

            yield return null;
        }

        OnWorkInterrupt();
        OnWorkComplete();
    }

    protected void OnWorkComplete()
    {
        IsOpen = !IsOpen;
        NetworkAnim.Animator.SetBool("OpenParameter", IsOpen);

        Debug.Log("Door Opened");
    }
}
