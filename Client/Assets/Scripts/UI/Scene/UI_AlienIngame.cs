using System;
using System.Collections;
using UnityEngine.UI;
using TMPro;
using UnityEngine;

public class UI_AlienIngame : UI_Ingame
{
    private Alien Alien {
        get => Creature as Alien;
        set => Creature = value;
    }

    public Canvas Canvas;
    public Camera camera;

    enum AlienSubItemUIs
    {

    }


    public override bool Init()
    {
        if (base.Init() == false) { return false; }

        Bind<UI_Base>(typeof(AlienSubItemUIs));

        Canvas = gameObject.GetComponent<Canvas>();

        return true;
    }

    public override void InitAfterNetworkSpawn(Creature creature)
    {
        base.InitAfterNetworkSpawn(creature);
    }

    public void UIGameClear()
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
