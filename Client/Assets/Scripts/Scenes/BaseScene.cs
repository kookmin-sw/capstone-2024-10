using UnityEngine;
using UnityEngine.EventSystems;

public abstract class BaseScene : MonoBehaviour
{
    public Define.SceneType SceneType { get; protected set; }

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
        Managers.NetworkMng.Init();
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
