using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_CameraPanel : UI_Panel
{
    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        return true;
    }

    public override void ClosePanelUI()
    {
        var camera = transform.Find("UICamera");
        Destroy(camera.gameObject);

        base.ClosePanelUI();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        if (Managers.UIMng.SceneUI != null)
            Managers.UIMng.SceneUI.gameObject.SetActive(true);
        Managers.UIMng.ActivatePopupUI(true);
    }
}
