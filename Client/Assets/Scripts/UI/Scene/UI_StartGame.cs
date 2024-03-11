using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_StartGame : UI_Scene
{
    enum Buttons
    {
        ReadyGame,
    }

    enum Texts
    {
        ReadyCount,
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        Bind<Button>(typeof(Buttons));
        Bind<TMP_Text>(typeof(Texts));

        GetButton((int)Buttons.ReadyGame).onClick.AddListener(ReadyGame);

        return true;
    }

    private IEnumerator Start()
    {
        yield return new WaitUntil(() => Managers.GameMng.Player != null);
        Managers.GameMng.Player.OnReadyCountUpdate += () => SetInfo(Managers.GameMng.Player.ReadyCount);
    }

    public void ReadyGame()
    {
        Managers.GameMng.Player.GetReady();
    }

    public void SetInfo(int count)
    {
        GetText((int)Texts.ReadyCount).text = $"{count} / {Define.PLAYER_COUNT}";
    }
}
