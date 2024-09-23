using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_LobbyScene : UI_Scene
{
    #region UI 목록들
    public enum SubItemUI
    {
        UI_Notification,
    }
    #endregion

    UI_Notification UI_Notification;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        Bind<UI_Base>(typeof(SubItemUI));
        UI_Notification = Get<UI_Base>(SubItemUI.UI_Notification) as UI_Notification;
        UI_Notification.Init();

        return true;
    }

    public void ShowServerInitializeMessage()
    {
        UI_Notification.ShowServerConnectMessage();
    }

    public void HideServerInitializeMessage()
    {
        UI_Notification.HideServerConnectMessage();
    }
}
