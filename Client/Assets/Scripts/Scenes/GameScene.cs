using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;

[Serializable]
public class TempGameSceneData
{
    public Vector3 playerPos;
    public Vector3 backgroundPos;
    public Vector3 postBackPos;
    public Vector3 preBackPos;
}

public class GameScene : BaseScene
{
    // 씬이 초기에 생성될 때 수행될 목록
    protected override void init()
    {
        base.init();
        SceneType = Define.Scene.Game;

        //UI 세팅
        // Managers.UI.ShowSceneUI<UI_MainScene>();
    }

    // 씬이 바뀔 때 정리해야 하는 목록
    public override void Clear()
    {
    }

    private void OnApplicationQuit()
    {
        Clear();
    }
}
