using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Map : UI_Base
{
    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        return true;
    }

    public void Show()
    {
        gameObject?.SetActive(true);
    }

    public void Hide()
    {
        gameObject?.SetActive(false);
    }
}
