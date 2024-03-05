using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

/// <summary>
/// 상호작용 시 일정 시간 이상 시간을 소요해야 하는 오브젝트
/// </summary>
public class WorkStation : NetworkBehaviour, IInteractable
{
    [SerializeField]
    private float _requiredWorkAmount;

    [Networked]
    public float WorkProgress { get; set; }

    [Networked]
    public float ElectiricPower { get; set; }

    private bool _isComplete;
    private bool _isContinuable;

    public override void Spawned()
    {
        WorkProgress = 0;
    }

    public virtual void Interact()
    {
        StartCoroutine(ProgressWork());
    }

    public IEnumerator ProgressWork()
    {
        //Managers.UIMng.ShowPopupUI<UI_WorkingBar>("ShowLongWork");
        //UI_WorkingBar ui = Managers.UIMng.PeekPopupUI<UI_WorkingBar>();
        //yield return new WaitUntil(() => ui.Init());

        //isComplete = false;


        //if(!isContinuable) { workingTime = 0; }

        //while (workingTime < requiredTime)
        //{
        //    if(Input.GetKeyDown(KeyCode.Space))
        //    {
        //        Managers.UIMng.ClosePopupUI();
        //        StopAllCoroutines();
        //    }

        //    workingTime += Time.deltaTime * workingSpeed;
        //    ui.CalculateBar(this);
        //    yield return null;
        //}

        //isComplete = true;

        //Managers.UIMng.ClosePopupUI();

        yield break;
    }
}
