using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 해당 UI 내용은 맵이 바뀔때마다 인스펙터에서 수동으로 바꿔주어야 한다.
/// </summary>
public class UI_ElectricPanel : UI_Popup
{
    public enum Buttons
    {

    }

    public enum Images
    {

    }

    public enum Texts
    {

    }

    public enum GameObjects
    {
        First_Floor,
        Second_Floor,
    }

    private List<GameObject> floors = new List<GameObject>();

    private BaseWorkStation _baseWorkStation;

    public override bool Init()
    {
        if (base.Init() == false) { return false; }

        Bind<Button>(typeof(Buttons));
        Bind<Image>(typeof(Images));
        Bind<TMP_Text>(typeof(Texts));
        Bind<GameObject>(typeof(GameObjects));

        //gameObject.AddComponent<GraphicRaycaster>();


        return true;
    }

    private void SetupButtons()
    {
        //foreach (GameObject nowFloor in floors)
        //{
        //    for (int i = 0; i < nowFloor.transform.GetChild(0).childCount; i++)
        //    {
        //        nowFloor.transform.GetChild(0).GetChild(i).TryGetComponent(out ElectricsystemButton nowButton);
        //        if (nowButton == null) { continue; }

        //        switch(nowButton.buttonType)
        //        {
        //            case ElectricsystemButton.ButtonType.room:
        //                MapManager.baseSystem.rooms.TryGetValue
        //                    (nowFloor.name + nowButton.gameObject.name, out Sector baseRoom);
        //                nowButton.inChargeRoom = baseRoom;
        //                break;

        //            case ElectricsystemButton.ButtonType.door:
        //                MapManager.baseSystem.doors.TryGetValue
        //                    (nowFloor.name + nowButton.gameObject.name, out Gate baseDoor);
        //                nowButton.inChargeDoor = baseDoor;
        //                break;
        //        }

        //        if(nowButton.isOn) { nowButton.GetComponent<Image>().color = Color.green; }
        //        else { nowButton.GetComponent<Image>().color = Color.red; }

        //        nowButton.GetComponent<Button>().onClick.AddListener(() => StartCoroutine(SelectSegment(nowButton)));
        //    }
        //}
    }

    //public IEnumerator SelectSegment(ElectricsystemButton buttonSelf)
    //{
    //    if (!buttonSelf.isOn)
    //    {
    //        yield return StartCoroutine(_workStation.Work());

    //        if(_workStation.isComplete) 
    //        { 
    //            buttonSelf.ChangeIsOn();

    //            if (buttonSelf.isOn) { buttonSelf.GetComponent<Image>().color = Color.green; }
    //            else { buttonSelf.GetComponent<Image>().color = Color.red; }
    //        }
    //    }
    //}

    private void Update()
    {
        //if(Input.GetKeyDown(KeyCode.Escape))
        //{
        //    _workStation.StopAllCoroutines();
        //    StopAllCoroutines();
        //    Managers.UIMng.CloseAllPopupUI();
        //}
    }
}
