using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 모든 매니저들을 포함하고 있는 메인 매니저
/// 메인 매니저는 씬의 @Mangers에 컴포넌트로 붙어 있다.
/// </summary>
public class Managers : MonoBehaviour
{
    static Managers s_instance;
    static Managers Instance { get { init(); return s_instance; } }

    // 다른 매니저를 정의해서 넣을 때 초기화 작업과 클리어 작업을 구현해서 Init과 Clear에서 호출한다.
    // 하부 매니저들은 Monobehaviour를 절대 상속하지 않는다.
    // 콘텐츠 부분은 더 이상 매니저를 할당하지 않고 게임오브젝트에 할당한다.
    #region Contents
    GameManagerEX _game = new GameManagerEX();

    public static GameManagerEX Game { get { return Instance._game; } }
    #endregion

    // 코어 부분은 게임엔진 쪽에 가까운 매니저들이 할당된다.
    #region Core
    DataManager _data = new DataManager();
    InputManager _input = new InputManager();
    PoolManager _pool = new PoolManager();
    ResourceManager _resource = new ResourceManager();
    SceneManagerEx _scene = new SceneManagerEx();
    SoundManager _sound = new SoundManager();
    UIManager _ui = new UIManager();

    public static DataManager Data { get { return Instance._data; } }
    public static InputManager Input { get { return Instance._input; } }
    public static PoolManager Pool { get { return Instance._pool; } }
    public static ResourceManager Resource { get { return Instance._resource; } }
    public static SceneManagerEx Scene { get { return Instance._scene; } }
    public static SoundManager Sound { get { return Instance._sound; } }
    public static UIManager UI { get { return Instance._ui; } }

    #endregion

    void Start()
    {
        init();
    }

    void Update()
    {
        _input.OnUpdate();
    }

    /// <summary>
    /// 처음에 화면이 로드될 때 매니저들의 초기화에 사용되는 함수, 
    /// 만약 메인 매니저 파일이 존재하지 않으면 자동으로 생성한다.
    /// </summary>
    static void init()
    {
        if (s_instance == null)
        {
            GameObject go = GameObject.Find("@Managers");
            if (go == null)
            {
                go = new GameObject { name = "@Managers" };
                go.AddComponent<Managers>();
            }

            DontDestroyOnLoad(go);
            s_instance = go.GetComponent<Managers>();

            s_instance._game.init();
            s_instance._pool.init();
            s_instance._sound.init();
            s_instance._data.init();
        }
    }

    /// <summary>
    /// 각 매니저들을 초기화해주는 함수
    /// 순서를 임의로 바꾸면 문제가 발생할 수 있다.
    /// </summary>
    public static void Clear()
    {
        Game.Clear();
        Sound.Clear();
        Input.Clear();
        Scene.Clear();
        UI.Clear();
        Pool.Clear();
    }
}
