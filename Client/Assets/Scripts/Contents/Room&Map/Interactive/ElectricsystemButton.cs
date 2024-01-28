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
    
    public BaseRoom inChargeRoom;
    public BaseDoor inChargeDoor;
}
