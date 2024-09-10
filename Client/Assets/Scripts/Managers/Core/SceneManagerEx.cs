using Fusion;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 유니티에 존재하는 씬매니저를 래핑한 매니저
/// </summary>
public class SceneManagerEx
{
    /// <summary>
    /// 현재 위치한 씬이 어디인지 알아낸다.
    /// 씬마다 베이스 씬을 상속한 스크립트를 부착한 @Scene 객체를 가지고 있다.
    /// </summary>
    public BaseScene CurrentScene => GameObject.FindObjectOfType<BaseScene>();

    /// <summary>
    /// 씬을 불러온다. 다만 기존의 스트링이 아닌 enum 타입으로 가져올 수 있다.
    /// 내부적으로 메인 매니저의 씬을 초기화시키는 코드를 실행해 씬이 바뀔 때,
    /// 자동으로 초괴화 작업이 이루어지도록 한다.
    /// </summary>
    public void LoadScene(Define.SceneType type)
    {
        Managers.Clear();
        SceneManager.LoadScene(GetSceneName(type));
    }

    public async void LoadNetworkScene(Define.SceneType type)
    {
        await Managers.NetworkMng.Runner.LoadScene(SceneRef.FromIndex(SceneUtility.GetBuildIndexByScenePath($"Assets/Scenes/{GetSceneName(type)}.unity")));
    }

    /// <summary>
    /// 씬의 이름을 가져온다. 씬의 이름에 특정 규칙이 있을 경우 여기에 반영한다.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public string GetSceneName(Define.SceneType type)
    {
        string name = System.Enum.GetName(typeof(Define.SceneType), type);
        return name;
    }

    public SceneRef GetSceneRef(Define.SceneType type)
    {
        return SceneRef.FromIndex(SceneUtility.GetBuildIndexByScenePath($"Assets/Scenes/{GetSceneName(type)}.unity"));
    }

    /// <summary>
    /// 베이스 씬을 상속한 각각의 씬에 정의되어 있는 클리어 함수를 불러온다.
    /// </summary>
    public void Clear()
    {
        CurrentScene.Clear();
    }

    public string GetCurrentSceneName()
    {
        return SceneManager.GetActiveScene().name;
    }
}
