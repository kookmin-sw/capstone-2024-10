using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UI_RaycastBlock : UI_Popup
{
    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        return true;
    }
}
