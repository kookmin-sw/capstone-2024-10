using Fusion;
using System.Collections;
using UnityEngine;

public class Computer : BaseWorkStation
{
    public Crew CurrentWorkCrew => (Crew)CurrentWorkCreature;

    public override void Init()
    {
        base.Init();

        IsRememberWork = true;
        IsCompleted = false;
        IsSomeoneWork = false;

        RequiredWorkAmount = 100f;
    }

    protected override IEnumerator WorkProgress()
    {
        // TODO - Test code
        float time = Time.time;

        while (CurrentWorkAmount < RequiredWorkAmount)
        {
            if (CurrentWorkCreature.CreatureState != Define.CreatureState.Interact)
                OnWorkInterrupt();

            CurrentWorkAmount += Time.deltaTime * CurrentWorkCrew.CrewStat.WorkSpeed;
            ProgressBarUI.CurrentWorkAmount = CurrentWorkAmount;

            // TODO - Test code
            if (time + 1 < Time.time)
            {
                time = Time.time;
                Debug.Log($"CurrentWorkProgress: {CurrentWorkAmount}");
            }

            yield return null;
        }

        OnWorkInterrupt();
        OnWorkComplete();
    }

    protected void OnWorkComplete()
    {
        IsCompleted = true;
        Debug.Log("Computer Work Completed");
    }
}
