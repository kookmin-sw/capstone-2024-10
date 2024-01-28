using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseSystem : MonoBehaviour
{
    //이건 인스펙터창에서 직접 설정해줘야 함
    public GameObject[] floors;  

    [HideInInspector]
    public Dictionary<string, BaseRoom> rooms;

    [HideInInspector]
    public Dictionary<string, BaseWall> walls;

    [HideInInspector]
    public Dictionary<string, BaseDoor> doors;

    // Start is called before the first frame update
    private void Start()
    {
        FindElements();
    }

    private void FindElements()
    {
        foreach (GameObject g in floors)
        {
            Transform motherRoom = Util.FindChild(g, "Rooms").transform;
            Transform motherWall = Util.FindChild(g, "Walls").transform;
            Transform motherDoor = Util.FindChild(g, "Doors").transform;

            for (int i = 0; i < motherRoom.childCount; i++)
            {
                GameObject nowRoom = motherRoom.GetChild(i).gameObject;
                rooms.Add(nowRoom.name, nowRoom.GetComponent<BaseRoom>());
            }

            for (int i = 0; i < motherWall.childCount; i++)
            {
                GameObject nowWall = motherWall.GetChild(i).gameObject;
                walls.Add(nowWall.name, nowWall.GetComponent<BaseWall>());
            }

            for (int i = 0; i < motherDoor.childCount; i++)
            {
                GameObject nowDoor = motherDoor.GetChild(i).gameObject;
                doors.Add(nowDoor.name, nowDoor.GetComponent<BaseDoor>());
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
