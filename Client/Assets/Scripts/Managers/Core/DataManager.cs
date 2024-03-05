using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public interface ILoader<Key, Value>
{
    Dictionary<Key, Value> MakeDict();
}

// 시작하면 바로 데이터를 Load하여 Dict로 관리
public class DataManager
{
    public Dictionary<int, Data.CrewData> CrewDataDict { get; private set; }
    public Dictionary<int, Data.AlienData> AlienDataDict { get; private set; }
    public Dictionary<int, Data.ItemData> ItemDataDict { get; private set; }

    public void Init()
    {
        CrewDataDict = LoadJson<Data.CrewDataLoader, int, Data.CrewData>("JsonData/CrewData").MakeDict();
        AlienDataDict = LoadJson<Data.AlienDataLoader, int, Data.AlienData>("JsonData/AlienData").MakeDict();
        ItemDataDict = LoadJson<Data.ItemDataLoader, int, Data.ItemData>("JsonData/ItemData").MakeDict();
    }

    // path 위치의 Json 파일을 TextAsset 타입으로 로드
    private Loader LoadJson<Loader, Key, Value>(string path) where Loader : ILoader<Key, Value>
    {
        TextAsset textAsset = Managers.ResourceMng.Load<TextAsset>($"Datas/{path}");
        //return JsonUtility.FromJson<Data>(textAsset.text);
        return JsonConvert.DeserializeObject<Loader>(textAsset.text);
    }
}
