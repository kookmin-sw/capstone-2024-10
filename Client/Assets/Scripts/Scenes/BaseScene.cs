using UnityEngine;
using UnityEngine.EventSystems;
using Object = UnityEngine.Object;

// 모든 Scene의 조상 클래스
public abstract class BaseScene : MonoBehaviour
{
    public Define.SceneType SceneType { get; protected set; } = Define.SceneType.UnknownScene;

    private void Awake()
    {
        Init();
    }

    private void Update()
    {
        Managers.InputMng.OnUpdate();
    }

    protected virtual void Init()
    {
        // TODO - TEST CODE: 나중에는 최초 Scene에서만 실행
        Managers.InputMng.Init();
        Managers.DataMng.Init();
        Managers.SoundMng.Init();
        Managers.PoolMng.Init();
        Managers.ObjectMng.Init();

        Object obj = FindObjectOfType(typeof(EventSystem));

        if (obj == null)
            Managers.ResourceMng.Instantiate("UI/EventSystem").name = "@EventSystem";
    }

    public abstract void Clear();
}
