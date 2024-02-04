using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_SessionEntry : UI_Base
{
    #region UI ¸ñ·Ïµé
    public enum Buttons
    {
        JoinButton,
    }

    public enum Images
    {
    }

    public enum Texts
    {
        RoomName,
        PlayerCount,
    }

    public enum GameObjects
    {
    }
    #endregion

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        Bind<Button>(typeof(Buttons));
        Bind<Image>(typeof(Images));
        Bind<TMP_Text>(typeof(Texts));
        Bind<GameObject>(typeof(GameObjects));

        transform.localScale = Vector3.one;
        transform.localPosition = Vector3.zero;

        return true;
    }

    private void JoinSession()
    {
        FusionConnection.instance.ConnectToSession(GetText((int)Texts.RoomName).text);
    }
}
