using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 배전판에 붙이는 클래스임
/// </summary>
public class ElectricSystem : BaseWorking, IInteractable
{    
    public BaseSystem baseSystem;

    private void Start()
    {
        
    }

    public IEnumerator Interact()
    {
        yield return null;

        //전력시스템 설정 코드
        Managers.UI.ShowPopupUI<UI_ElectricPanel>("Electric_Control");
        Debug.Log("dddd");
    }
}
