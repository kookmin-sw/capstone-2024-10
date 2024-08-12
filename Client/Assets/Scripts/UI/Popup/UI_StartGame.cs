using Fusion;
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
        RoomName,
        UserName,
    }

    enum GameObjects
    {
        NotReadySignal,
        ReadySignal,
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        Bind<Button>(typeof(Buttons));
        Bind<TMP_Text>(typeof(Texts));
        Bind<GameObject>(typeof(GameObjects));

        GetButton((int)Buttons.ReadyGame).onClick.AddListener(ReadyGame);
        GetButton((int)Buttons.ExitGame).onClick.AddListener(ExitGame);
        GetObject((int)GameObjects.ReadySignal).SetActive(false);

        SetInfo(0);
        StartCoroutine(Reserve());

        return true;
    }

    private void Update()
    {
        if (Input.GetButtonDown("Submit"))
        {
            SimulateButtonClick(GetButton((int)Buttons.ReadyGame));
            GetObject((int)GameObjects.NotReadySignal).SetActive(false);
            GetObject((int)GameObjects.ReadySignal).SetActive(true);
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
        yield return new WaitUntil(() => Managers.NetworkMng.Player != null);

        Managers.NetworkMng.PlayerSystem.OnReadyCountUpdated += () => SetInfo(Managers.NetworkMng.PlayerSystem.ReadyCount);
        SetInfo(Managers.NetworkMng.PlayerSystem.ReadyCount);
        
    }

    public void ReadyGame()
    {
        if (Managers.NetworkMng.Player == null)
        {
            Debug.Log("Player is null");
            return;
        }

        Managers.NetworkMng.Player.GetReady();
    }

    public void ExitGame()
    {
        Managers.StartMng.ExitGame();
    }

    public void SetInfo(int count)
    {
        GetText(Texts.ReadyCount).text = $"{count} / {Define.PLAYER_COUNT}";
        SessionInfo info = Managers.NetworkMng.Runner.SessionInfo;
        GetText(Texts.RoomName).text = info.Name;

        if (Managers.NetworkMng.Player != null && Managers.NetworkMng.Runner.IsRunning)
        {
            GetText(Texts.UserName).text = Managers.NetworkMng.Player.PlayerName.Value;
        }
    }

    private void OnDestroy()
    {
        // Managers.NetworkMng.PlayerSystem.OnReadyCountUpdated -= () => SetInfo(Managers.NetworkMng.PlayerSystem.ReadyCount);
    }
}
