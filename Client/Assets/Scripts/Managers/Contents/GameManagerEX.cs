using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEngine;
using Fusion;
using System.Data.SqlTypes;
using System.Linq;

[Serializable]
public class SaveData
{
    public string playerId = "player";
}

public class GameManagerEX
{
    public SaveData SaveData { get; protected set; }

    public string SAVEDATA_PATH;

    public Player Player { get; set; }
    public void Init()
    {
        SAVEDATA_PATH = Path.Combine(Application.persistentDataPath, "/SaveData.json");

        if (SaveData == null)
        {
            SaveData = new SaveData();
        }
    }

    #region Start

    public IEnumerator TryStartGame()
    {
        yield return new WaitUntil(() => Managers.NetworkMng != null);

        yield return new WaitUntil(() => Managers.NetworkMng.PlayerSystem != null);

        while (Managers.NetworkMng.NumPlayers != Define.PLAYER_COUNT || Managers.NetworkMng.PlayerSystem.ReadyCount != Define.PLAYER_COUNT)
        {
            yield return null;
        }

        StartGame();
    }

    public void StartGame()
    {
        Debug.Log("Game Setting Start");
        Managers.UIMng.ClosePopupUI();

        if (Managers.NetworkMng.IsMaster)
        {
            var players = Managers.NetworkMng.Runner.ActivePlayers.ToList();
            int random = UnityEngine.Random.Range(0, players.Count);
            Player.RPC_ChangePlayerToAlien(Managers.NetworkMng.Runner, players[random], Define.ALIEN_STALKER_ID);
        }
    }
    #endregion

    #region Save & Load
    public void SaveGame()
    {
        string jsonStr = JsonUtility.ToJson(Managers.GameMng.SaveData);
        File.WriteAllText(SAVEDATA_PATH, jsonStr);
        Debug.Log($"Save Game Completed : {SAVEDATA_PATH}");
    }

    public bool LoadGame()
    {
        if (File.Exists(SAVEDATA_PATH) == false)
            return false;

        string fileStr = File.ReadAllText(SAVEDATA_PATH);
        SaveData data = JsonUtility.FromJson<SaveData>(fileStr);
        if (data != null)
        {
            Managers.GameMng.SaveData = data;
        }

        Debug.Log($"Save Game Loaded : {SAVEDATA_PATH}");
        return true;
    }

    public void Clear()
    {

    }
    #endregion
}
