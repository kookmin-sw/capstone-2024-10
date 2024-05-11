using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_ManualController : UI_Base
{
    enum GameObjects
    {

    }

    enum Buttons
    {
        Btn_Return,
    }

    private UI_LobbyController _controller;

    public override bool Init()
    {
        if (!base.Init())
            return false;


        _controller = FindObjectOfType<UI_LobbyController>();

        return true;
    }


}
