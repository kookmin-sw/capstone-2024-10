using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Computer : WorkStation
{
    //TODO: test code
    private const float WORK_SPEED = 0.5f;

    protected override IEnumerator ProgressWork()
    {
        //TODO: Test code
        float time = Time.time;
        float tempworkamount = WorkProgress;
        ////////////

        Debug.Log($"{_workingCreature.NetworkObject.Id}: starting work...");
        UI_CrewIngame uiCrew = Managers.UIMng.SceneUI as UI_CrewIngame;
        _progressbar = uiCrew.ShowWorkProgressBar("Fixing Computer", _requiredWorkAmount);
        while (WorkProgress < _requiredWorkAmount && !IsCompleted)
        {

            if (_workingCreature.Velocity.magnitude >= 1f)
            {
                Interrupt();
            }

            Rpc_ProgressWork(Time.deltaTime, WORK_SPEED);
            _progressbar.CurrentWorkAmount = WorkProgress;

            //TODO: Test code
            if (time + 1 < Time.time)
            {
                time = Time.time;
                Debug.Log($"Work progress for 1sec: {WorkProgress - tempworkamount}");
                tempworkamount = WorkProgress;
            }
            /////////////////
       
            yield return null;
        }

        _progressbar.gameObject.SetActive(false);
        Rpc_CompleteWork();
    }

    protected override void OnWorkComplete()
    {
        Debug.Log("Computer Work Completed");
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    private void Rpc_CompleteWork()
    {
        if (!IsCompleted)
        {
            IsCompleted = true;
            OnWorkComplete();
        }
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    private void Rpc_ProgressWork(float deltatime, float workSpeed)
    {
        WorkProgress += deltatime * workSpeed;
    }
}
