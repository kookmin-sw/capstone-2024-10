using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_StartGame : UI_Popup
{
    enum Buttons
    {
        ReadyGame,
        ExitGame,
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
        GetButton((int)Buttons.ExitGame).onClick.AddListener(ExitGame);

        SetInfo(0);
        StartCoroutine(Reserve());

        return true;
    }

    private void Update()
    {
        if (Input.GetButtonDown("Submit"))
        {
            SimulateButtonClick(GetButton((int)Buttons.ReadyGame));
        }
        else if (Input.GetButton("Cancel"))
        {
            SimulateButtonClick(GetButton((int)Buttons.ExitGame));
        }
    }

    private void SimulateButtonClick(Button button)
    {
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
        ExecuteEvents.Execute(button.gameObject, pointerEventData, ExecuteEvents.pointerClickHandler);
    }

    private IEnumerator Reserve()
    {
        yield return new WaitUntil(() => Managers.NetworkMng.PlayerSystem != null);
        Managers.NetworkMng.PlayerSystem.OnReadyCountUpdate += () => SetInfo(Managers.NetworkMng.PlayerSystem.ReadyCount);
        SetInfo(Managers.NetworkMng.PlayerSystem.ReadyCount);
    }

    public void ReadyGame()
    {
        if (Managers.GameMng.Player == null)
        {
            Debug.Log("Player is null");
            return;
        }

        Managers.GameMng.Player.GetReady();
    }

    public void ExitGame()
    {
        if (Managers.GameMng.Player == null)
        {
            Debug.Log("Player is null");
            return;
        }

        Managers.GameMng.Player.ExtiGame();
    }

    public void SetInfo(int count)
    {
        GetText((int)Texts.ReadyCount).text = $"{count} / {Define.PLAYER_COUNT}";
    }
}
