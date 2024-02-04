using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    public IEnumerator Interact();
}

/// <summary>
/// 데바데식처럼 일정 시간 이상 시간을 소요해야 하는 작업은 MonoBehaviour대신 이 클래스를 상속받아 사용
/// </summary>
public class BaseWorking : MonoBehaviour
{
    public float requiredTime = 5.0f;
    public float workingTime;

    protected float workingSpeed = 1.0f;

    public bool isComplete = false;
    protected bool isContinuable = false;

    private Coroutine coroutine;

    private void Start()
    {
        workingTime = 0;
    }

    protected void CheckWorking()
    {
        if(coroutine == null)
        {
            coroutine = StartCoroutine(Working());
        }
        else
        {
            StopCoroutine(coroutine);
            coroutine = null;
        }
    }

    /// <summary>
    /// 데바데식 작업 시스템 함수 (필요할 때만 호출됨)
    /// </summary>
    /// <returns></returns>
    public IEnumerator Working()
    {
        Managers.UI.ShowPopupUI<UI_WorkingBar>("ShowLongWork");
        UI_WorkingBar ui = Managers.UI.PeekPopupUI<UI_WorkingBar>();
        yield return new WaitUntil(() => ui.Init());
        
        isComplete = false;

        MapManager.baseSystem.isInteracting = true;
        
        if(!isContinuable) { workingTime = 0; }

        while (workingTime < requiredTime)
        {
            if(Input.GetKeyDown(KeyCode.Space))
            {
                MapManager.baseSystem.isInteracting = false;
                Managers.UI.ClosePopupUI();
                StopAllCoroutines();
            }
            
            workingTime += Time.deltaTime * workingSpeed;
            ui.CalculateBar(this);
            yield return null;
        }

        isComplete = true;

        MapManager.baseSystem.isInteracting = false;
        Managers.UI.ClosePopupUI();           
    }
}
