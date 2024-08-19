using UnityEngine;

public class Managers : MonoBehaviour
{
    public static bool Initialized { get; set; } = false;

    private static Managers s_instance;
    public static Managers Instance { get { Init(); return s_instance; } }

    #region Contents
    private StartManager _startMng = new StartManager();
    private ObjectManager _objectMng = new ObjectManager();

    public static StartManager StartMng => Instance._startMng;
    public static ObjectManager ObjectMng => Instance._objectMng;
    #endregion

    #region Core
    private DataManager _dataMng = new DataManager();
    private ResourceManager _resourceMng = new ResourceManager();
    private SceneManagerEx _sceneMng = new SceneManagerEx();
    private SoundManager _soundMng = new SoundManager();
    private UIManager _uiMng = new UIManager();
    private GameManager _gameMng = new GameManager();
    private NetworkManager _networkMng;
    private TutorialManager _tutorialMng = new TutorialManager();

    public static DataManager DataMng => Instance._dataMng;
    public static ResourceManager ResourceMng => Instance._resourceMng;
    public static SceneManagerEx SceneMng => Instance._sceneMng;
    public static SoundManager SoundMng => Instance._soundMng;
    public static UIManager UIMng => Instance._uiMng;
    public static NetworkManager NetworkMng => Instance._networkMng;
    public static GameManager GameMng => Instance._gameMng;
    public static TutorialManager TutorialMng => Instance._tutorialMng;
    #endregion

    public static void Init()
    {
        if (s_instance == null || Initialized == false)
        {
            Initialized = true;

            GameObject go = GameObject.Find("@Managers");
            if (go == null)
            {
                go = new GameObject { name = "@Managers" };
                go.AddComponent<Managers>();
                go.AddComponent<NetworkManager>();
            }

            DontDestroyOnLoad(go);
            s_instance = go.GetComponent<Managers>();
            s_instance._networkMng = go.GetOrAddComponent<NetworkManager>();

            NetworkMng.Init();
            DataMng.Init();
            SoundMng.Init();
        }
    }

    public static void Clear()
    {
        StartMng.Clear();
        SoundMng.Clear();
        SceneMng.Clear();
        UIMng.Clear();
    }
}
