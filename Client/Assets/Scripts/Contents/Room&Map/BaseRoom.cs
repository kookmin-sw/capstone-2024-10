using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseRoom : MonoBehaviour
{
    //이하 내용 오브젝트들은 프리펩 내부의 인스펙터에서 직접 배정
    public GameObject itsLight;

    private RoomLight roomLight;

    //전력 상태 
    public bool lightPower;

    private void Start()
    {
        roomLight = itsLight.GetComponent<RoomLight>();

        SetRoom();
    }

    private void SetRoom()
    {

    }

    public void SwitchLight()
    {
        if(lightPower)
        {
            if (!roomLight.light.enabled) { roomLight.light.enabled = true; }
            else if (roomLight.light.enabled) { roomLight.light.enabled = false; }
        }
    }
}
