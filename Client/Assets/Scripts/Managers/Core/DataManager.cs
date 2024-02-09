using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// json 데이터와 매칭시킬 원본 클래스는 해당 인터페이스를 구현해야 한다. 구현한 결과물은 Script/Data/ 폴더에 모아둔다.
/// </summary>
/// <typeparam name="Key"></typeparam>
/// <typeparam name="Value"></typeparam>
public interface ILoader<Key, Value>
{
    /// <summary>
    /// 딕셔너리를 만들어주는 함수로 정의해야 할 함수이며, 키와 밸류를 정의할 수 있다.
    /// </summary>
    /// <returns></returns>
    Dictionary<Key, Value> MakeDict();
}

/// <summary>
/// ILoader 형식으로 만들어진 Dictionary를 등록하고 외부에서 바로 사용할 수 있게 만든 매니저
/// </summary>
public class DataManager
{
    public Dictionary<string, Data.Setting> SettingDict { get; private set; } = new Dictionary<string, Data.Setting>();

    /// <summary>
    /// 각 데이터 형식은 딕셔너리 형태로 초기화해주는 작업이 반드시 필요하다.
    /// </summary>
    public void init()
    {
        SettingDict = LoadJson<Data.SettingData, string, Data.Setting>("SettingData").MakeDict();
    }

    /// <summary>
    /// Json 데이터 형식을 딕셔너리로 로드한다. Init 함수 내부에서만 사용될 함수이다.
    /// </summary>
    /// <typeparam name="Loader">Json 데이터 형식과 매칭될 클래스</typeparam>
    /// <typeparam name="Key">딕셔너리로 값을 찾을 때 넣을 키</typeparam>
    /// <typeparam name="Value">딕셔너리가 반환할 밸류</typeparam>
    /// <param name="path">Resources/Data 폴더 내부의 경로로 json 파일이 저장되어 있는 곳</param>
    /// <returns></returns>
    Loader LoadJson<Loader, Key, Value>(string path) where Loader : ILoader<Key, Value>
    {
        TextAsset textAsset = Managers.Resource.Load<TextAsset>($"Data/{path}");
        return JsonUtility.FromJson<Loader>(textAsset.text);
    }
}