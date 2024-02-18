using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseSystem : MonoBehaviour
{
    public bool isInteracting = false;
    
    [HideInInspector]
    public List<GameObject> floors = new List<GameObject>();  

    [HideInInspector]
    public Dictionary<string, BaseRoom> rooms = new Dictionary<string, BaseRoom>();

    [HideInInspector]
    public Dictionary<string, BaseWall> walls = new Dictionary<string, BaseWall>();

    [HideInInspector]
    public Dictionary<string, BaseDoor> doors = new Dictionary<string, BaseDoor>();

    // Start is called before the first frame update
    private void Start()
    {
        floors.Add(GameObject.Find("First_Floor"));
        floors.Add(GameObject.Find("Second_Floor"));
        
        FindElements();
        SettingSystem();
    }

    private void FindElements()
    {
        //foreach (GameObject g in floors)
        //{
        //    Transform motherRoom = Util.FindChild(g, "Rooms").transform;
        //    Transform motherWall = Util.FindChild(g, "Walls").transform;
        //    Transform motherDoor = Util.FindChild(g, "Doors").transform;

        //    for (int i = 0; i < motherRoom.childCount; i++)
        //    {
        //        GameObject nowRoom = motherRoom.GetChild(i).gameObject;
        //        rooms.Add(g.name + nowRoom.name, nowRoom.GetComponent<BaseRoom>());
        //    }

        //    for (int i = 0; i < motherWall.childCount; i++)
        //    {
        //        GameObject nowWall = motherWall.GetChild(i).gameObject;
        //        walls.Add(g.name + nowWall.name, nowWall.GetComponent<BaseWall>());
        //    }

        //    for (int i = 0; i < motherDoor.childCount; i++)
        //    {
        //        GameObject nowDoor = motherDoor.GetChild(i).gameObject;
        //        doors.Add(g.name + nowDoor.name, nowDoor.GetComponent<BaseDoor>());
        //    }
        //}
    }

    private void SettingSystem()
    {
        foreach (BaseRoom baseRoom in rooms.Values)
        {
            if(Random.Range(1,10) < 5)
            {
                baseRoom.isLightPower = true;
            }
        }

        foreach (BaseDoor baseDoor in doors.Values)
        {
            if (Random.Range(1, 10) < 5)
            {
                baseDoor.isLightPower = true;
            }
        }
    }
}
