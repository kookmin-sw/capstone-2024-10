using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    static MapManager _mapManager;
    static MapManager mapManager
    {
        get
        {
            if(_mapManager == null) { Init(); }
            return _mapManager;
        }
    }

    static BaseSystem _baseSystem;
    public static BaseSystem baseSystem 
    { 
        get 
        {
            if (_baseSystem == null) { Init(); }
            return _baseSystem; 
        } 
    }

    
    // Start is called before the first frame update
    void Start()
    {
        Managers.UI.ShowSceneUI<UI_InGame>("UI_MainMap");
    }

    //초기화 부분
    static void Init()
    {
        GameObject g = GameObject.Find("MainManager");

        _mapManager = g.GetOrAddComponent<MapManager>();
        _baseSystem = g.GetOrAddComponent<BaseSystem>();
    }
}
