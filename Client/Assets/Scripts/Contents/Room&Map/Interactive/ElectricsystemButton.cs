using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricsystemButton : MonoBehaviour
{
    public enum ButtonType
    {
        room,
        door,
    }
    public ButtonType buttonType;  //인스펙터창에서 직접 지정해줘야 함
    
    [HideInInspector]
    public BaseRoom inChargeRoom;
    [HideInInspector]
    public BaseDoor inChargeDoor;

    /// <summary>
    /// 반드시 연결되어있는 룸 오브젝트의 인스펙터 상 이름과 정확히 일치해야 함
    /// </summary>
    public string segmentName;

    [HideInInspector]
    public bool isOn
    {
        get
        {
            if (inChargeRoom) { return inChargeRoom.isLightPower; }
            else { return inChargeDoor.isLightPower; }
        }
    }

    public void ChangeIsOn()
    {
        switch(buttonType)
        {
            case ButtonType.room:
                inChargeRoom.isLightPower = !inChargeRoom.isLightPower;
                break;

            case ButtonType.door:
                inChargeDoor.isLightPower = !inChargeDoor.isLightPower;
                break;
        }
    }
}
