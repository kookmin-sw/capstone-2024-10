using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;

public class UI_SessionList : UI_Popup
{
    public enum GameObjects
    {
        RoomContent,
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        Bind<GameObject>(typeof(GameObjects));

        transform.localPosition = new Vector3(220, 300, 0);

        return true;
    }

    public void RefreshSessionLIst(string keyword = "")
    {
        foreach (Transform child in GetObject((int)GameObjects.RoomContent).transform)
        {
            Destroy(child.gameObject);
        }

        foreach (SessionInfo session in Managers.NetworkMng.Sessions)
        {
            if (session.IsVisible)
            {
                if (!keyword.IsNullOrEmpty() && !session.Name.Contains(keyword))
                    continue;

                UI_SessionEntry entry = Managers.UIMng.MakeSubItem<UI_SessionEntry>(GetObject((int)GameObjects.RoomContent).transform);
                var args = new SessionEntryArgs()
                {
                    session = session
                };
                StartCoroutine(entry.SetInfo(args));
            }
        }
    }
}
