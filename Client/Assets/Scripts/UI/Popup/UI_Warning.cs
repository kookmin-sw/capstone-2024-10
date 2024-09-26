using UnityEngine.UI;
using UnityEngine;
using DG.Tweening;

public class UI_Warning : UI_Popup
{
    enum Buttons
    {
        Btn_Yes,
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        Bind<Button>(typeof(Buttons));
        GetButton(Buttons.Btn_Yes).onClick.AddListener(() =>
        {
            Managers.UIMng.ClosePopupUIUntil<UI_Lobby>();

            var nickname = Managers.NetworkMng.PlayerName;
            Managers.NetworkMng.ConnectToLobby(nickname);

            var ui = Managers.UIMng.PeekPopupUI<UI_Lobby>();
            ui.Refresh();

            var scene = Managers.SceneMng.CurrentScene as LobbyScene;
            scene.SceneUI.ShowServerInitializeMessage();
            Managers.NetworkMng.OnSessionUpdated += () => scene.SceneUI.HideServerInitializeMessage();
        });

        return true;
    }
}
