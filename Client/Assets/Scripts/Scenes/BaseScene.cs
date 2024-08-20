using System.Collections;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class BaseScene : MonoBehaviour
{
    public Define.SceneType SceneType { get; protected set; }

    private void Awake()
    {
        Init();
    }

    protected virtual void Init()
    {
        Object obj = FindObjectOfType(typeof(EventSystem));

        if (obj == null)
            Managers.ResourceMng.Instantiate("UI/EventSystem").name = "@EventSystem";
    }

    public virtual IEnumerator OnPlayerSpawn()
    {
        yield return null;
    }

    public abstract void Clear();
}
