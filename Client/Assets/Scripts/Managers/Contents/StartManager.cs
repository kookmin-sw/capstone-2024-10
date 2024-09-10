using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Fusion;
using System.Linq;

[Serializable]
public class SaveData
{
    public string playerId = "player";
}

public class StartManager
{
    public SaveData SaveData { get; protected set; }

    public string SAVEDATA_PATH;

    public Dictionary<string, SessionProperty> SessionProperty { get; protected set; }

    public bool SessionVisible
    {
        get
        {
            SessionInfo info = Managers.NetworkMng.Runner.SessionInfo;
            return info.IsVisible;
        }
        set
        {
            if (Managers.NetworkMng.IsMaster)
            {
                SessionInfo info = Managers.NetworkMng.Runner.SessionInfo;
                info.IsVisible = value;
                SessionProperty = new (info.Properties);
                info.UpdateCustomProperties(SessionProperty);
            }
        }
    }

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

        Debug.Log("Game Setting Start");
        Managers.UIMng.ShowPanelUI<UI_Loading>();
        Managers.Clear();
        Managers.NetworkMng.InitializeCreatureCount();

        if (Managers.NetworkMng.IsMaster)
        {
            var players = Managers.NetworkMng.Runner.ActivePlayers.ToList();
            var spawnPoints = GameObject.FindObjectsOfType<SpawnPoint>().ToList();

            var respawn = GameObject.FindWithTag("Respawn");

            foreach (var player in players)
            {
                SpawnPoint.SpawnPointData spawnPoint;
                if (spawnPoints.Count == 0)
                {
                    spawnPoint = respawn.GetComponent<SpawnPoint>().Data;
                    Debug.LogError("Not Enough Spawn Points");
                }
                else
                {
                    int rand = UnityEngine.Random.Range(0, spawnPoints.Count);
                    spawnPoint = spawnPoints[rand].Data;
                    spawnPoints.RemoveAt(rand);
                }

                Managers.NetworkMng.PlayerSystem.SpawnPoints.Set(player, spawnPoint);
            }

            SessionVisible = false;
            yield return new WaitForSeconds(0.5f);
            StartGame();
        }
    }

    public void StartGame()
    {
        NetworkSceneManagerEx networkScene = Managers.NetworkMng.Runner.SceneManager as NetworkSceneManagerEx;
        networkScene.UnloadScene();
        Managers.SceneMng.LoadNetworkScene(Define.SceneType.GameScene);
    }

    public async void ExitGame()
    {
        await Managers.NetworkMng.Runner.Shutdown();
        Managers.SceneMng.LoadScene(Define.SceneType.LobbyScene);
    }
    #endregion

    #region Save & Load
    public void SaveGame()
    {
        string jsonStr = JsonUtility.ToJson(Managers.StartMng.SaveData);
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
            Managers.StartMng.SaveData = data;
        }

        Debug.Log($"Save Game Loaded : {SAVEDATA_PATH}");
        return true;
    }

    public void Clear()
    {

    }
    #endregion
}
