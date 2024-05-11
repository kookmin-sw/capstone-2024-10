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

    public Player Player { get; set; }
    public List<Player> AllPlayers
    {
        get
        {
            return Managers.NetworkMng.Runner.GetAllBehaviours<Player>();
        }
    }

    public Dictionary<string, SessionProperty> SessionProperty { get; protected set; }

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
        var popup = Managers.UIMng.FindPopup<UI_StartGame>();
        popup.ClosePopupUI();
        Managers.UIMng.ShowPanelUI<UI_Loading>();

        if (Managers.NetworkMng.IsMaster)
        {
            var players = Managers.NetworkMng.Runner.ActivePlayers.ToList();
            var spawnPoints = GameObject.FindObjectsOfType<SpawnPoint>().ToList();
            var respawn = GameObject.FindWithTag("Respawn");

            foreach (var player in players)
            {
                Vector3 spawnPoint = Vector3.zero;
                if (spawnPoints.Count == 0)
                {
                    spawnPoint = respawn.transform.position;
                    Debug.LogError("Not Enough Spawn Points");
                }
                else
                {
                    int rand = UnityEngine.Random.Range(0, spawnPoints.Count);
                    spawnPoint = spawnPoints[rand].transform.position;
                    spawnPoints.RemoveAt(rand);
                }
                Managers.NetworkMng.PlayerSystem.SpawnPoints.Set(player, spawnPoint);
            }

            yield return new WaitForSeconds(1.0f);
            SessionInfo info = Managers.NetworkMng.Runner.SessionInfo;
            info.IsVisible = false;
            SessionProperty = new (info.Properties);
            info.UpdateCustomProperties(SessionProperty);
            StartGame();
        }
    }

    public void StartGame()
    {
        NetworkSceneManagerEx networkScene = Managers.NetworkMng.Runner.SceneManager as NetworkSceneManagerEx;
        networkScene.UnloadScene();
        Managers.SceneMng.LoadNetworkScene(Define.SceneType.GameScene);
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
