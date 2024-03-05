using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEngine;

[Serializable]
public class SaveData
{
    public string playerId = "player";
}

/// <summary>
/// 게임 오브젝트의 생성과 소멸에 대한 로직을 수행하고 다이렉트로 게임 오브젝트에 접근할 수 있는 권한을 가지고 있는 매니저
/// </summary>
public class GameManagerEX
{
    public SaveData SaveData { get; protected set; }

    public string SAVEDATA_PATH;

    public void Init()
    {
        SAVEDATA_PATH = Path.Combine(Application.persistentDataPath, "/SaveData.json");

        if (SaveData == null)
        {
            SaveData = new SaveData();
        }
    }

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
