using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Pool
{
    /// <summary>
    /// 풀링 오브젝트의 프리팹
    /// </summary>
    public GameObject Original { get; private set; }
    /// <summary>
    /// 풀링 오브젝트의 루트
    /// </summary>
    public Transform Root { get; set; }
    /// <summary>
    /// 풀링 오브젝트를 관리할 스택
    /// </summary>
    Stack<Poolable> _poolStack = new Stack<Poolable>();

    /// <summary>
    /// 각각의 지정한 오브젝트마다 루트를 만들어 같은 부모 아래에 모아 둔다.
    /// </summary>
    /// <param name="original"></param>
    /// <param name="count"></param>
    public void Init(GameObject original, int count = 5)
    {
        Original = original;
        Root = new GameObject().transform;
        Root.name = $"{original.name}_Root";

        for (int i = 0; i < count; i++)
            Push(Create());
    }

    /// <summary>
    /// 풀링 오브젝트를 생성한다.
    /// </summary>
    /// <returns>Poolable 스크립트를 통해 관리한다</returns>
    Poolable Create()
    {
        GameObject go = Object.Instantiate<GameObject>(Original);
        go.name = Original.name;
        return go.GetOrAddComponent<Poolable>();
    }

    /// <summary>
    /// 풀링 오브젝트를 스택에 넣는다.
    /// </summary>
    public void Push(Poolable poolable)
    {
        if (poolable == null)
            return;

        poolable.transform.parent = Root;
        poolable.gameObject.SetActive(false);
        poolable.IsUsing = false;

        _poolStack.Push(poolable);
    }

    /// <summary>
    /// 지정한 부모 밑에서 풀링 오브젝트를 찾아 뽑아온다.
    /// </summary>
    public Poolable Pop(Transform parent)
    {
        Poolable poolable;

        if (_poolStack.Count > 0)
            poolable = _poolStack.Pop();
        else
            poolable = Create();

        poolable.gameObject.SetActive(true);

        // DontDestroyOnLoad 해제 용도
        if (parent == null)
            poolable.transform.parent = Managers.SceneMng.CurrentScene.transform;

        poolable.transform.parent = parent;
        poolable.IsUsing = true;

        return poolable;
    }
}
