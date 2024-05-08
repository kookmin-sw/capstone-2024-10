using System;
using System.Collections;
using UnityEngine.UI;
using TMPro;
using UnityEngine;

public class UI_CrewIngame : UI_Ingame
{
    public UI_CrewPlan PlanUI { get; private set; }
    public UI_Inventory UI_Inventory { get; private set; }
    public UI_CrewHP UI_CrewHP { get; private set; }
    public UI_CrewStamina UI_CrewStamina { get; private set; }

    public UI_CrewWin UICrewWin { get; private set; }

    public Canvas Canvas;
    public Camera camera;

    private Crew Crew {
        get => Creature as Crew;
        set => Creature = value;
    }

    enum CrewSubItemUIs
    {
       UI_CrewHP,
       UI_CrewStamina,
       UI_Inventory,
       UI_CrewPlan,
    }


    public override bool Init()
    {
        if (base.Init() == false) { return false; }

        Bind<UI_Base>(typeof(CrewSubItemUIs));

        PlanUI = Get<UI_Base>(Enum.GetNames(typeof(SubItemUIs)).Length + (int)CrewSubItemUIs.UI_CrewPlan) as UI_CrewPlan;
        UI_Inventory = Get<UI_Base>(Enum.GetNames(typeof(SubItemUIs)).Length + (int)CrewSubItemUIs.UI_Inventory) as UI_Inventory;
        UI_CrewHP = Get<UI_Base>(Enum.GetNames(typeof(SubItemUIs)).Length + (int)CrewSubItemUIs.UI_CrewHP) as UI_CrewHP;
        UI_CrewStamina = Get<UI_Base>(Enum.GetNames(typeof(SubItemUIs)).Length + (int)CrewSubItemUIs.UI_CrewStamina) as UI_CrewStamina;

        Canvas = gameObject.GetComponent<Canvas>();

        return true;
    }

    public override void InitAfterNetworkSpawn(Creature creature)
    {
        base.InitAfterNetworkSpawn(creature);

        (Get<UI_Base>(Enum.GetNames(typeof(SubItemUIs)).Length + (int)CrewSubItemUIs.UI_CrewHP) as UI_CrewHP).Crew = Crew;
        (Get<UI_Base>(Enum.GetNames(typeof(SubItemUIs)).Length + (int)CrewSubItemUIs.UI_CrewStamina) as UI_CrewStamina).Crew = Crew;

        PlanUI.EnableBatteryChargePlan();
    }

    public void HideUI()
    {
        PlanUI.gameObject.SetActive(false);
        UI_Inventory.gameObject.SetActive(false);
        UI_CrewHP.gameObject.SetActive(false);
        UI_CrewStamina.gameObject.SetActive(false);
    }

    public void EndGame()
    {
        Canvas.renderMode = RenderMode.ScreenSpaceCamera;
        Transform[] children = gameObject.GetComponentsInChildren<Transform>();

        // 자식 객체들 중에서 Camera 컴포넌트를 가진 객체 찾기
        foreach (Transform child in children)
        {
            // 자식 객체가 Camera 컴포넌트를 가지고 있는지 확인
            Camera childCamera = child.GetComponent<Camera>();
            if (childCamera != null)
            {
                camera = childCamera;
            }
        }
        Canvas.worldCamera = camera;
    }
}
