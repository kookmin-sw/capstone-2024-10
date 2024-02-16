using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.Image;

/// <summary>
/// 풀링 매니저는 리소스 매니저에 귀속되어 있는 매니저로 사용자 스크립트에서 직접 호출할 필요가 없다.
/// </summary>
public class PoolManager
{
    Dictionary<string, Pool> _pool = new Dictionary<string, Pool>();
    Transform _root;

    /// <summary>
    /// 모든 풀의 루트를 만들어 주고 씬이 바뀌어도 삭제되지 않게 설정을 한다.
    /// </summary>
    public void Init()
    {
        if (_root == null)
        {
            _root = new GameObject { name = "@Pool_Root"}.transform;
            Object.DontDestroyOnLoad(_root);
        }
    }

    /// <summary>
    /// 프리팹을 주면 해당하는 개수만큼 풀링 오브젝트를 만들어 풀에 넣어 놓는다.
    /// </summary>
    /// <param name="original">풀을 생성할 프리팹</param>
    /// <param name="count">몇 개의 오브젝트를 미리 생성해 놓을지 정한다.</param>
    public void CreatePool(GameObject original, int count = 5)
    {
        Pool pool = new Pool();
        pool.Init(original, count);
        pool.Root.parent = _root;

        _pool.Add(original.name, pool);
    }

    /// <summary>
    /// 풀의 Push 함수를 매핑해 놓은 함수
    /// </summary>
    public void Push(Poolable poolable)
    {
        string name = poolable.gameObject.name;
        if (_pool.ContainsKey(name) == false)
        {
            GameObject.Destroy(poolable.gameObject);
        }

        _pool[name].Push(poolable);
    }

    /// <summary>
    /// 풀의 Pop 함수를 매핑해 놓은 함수
    /// </summary>
    public Poolable Pop(GameObject original, Transform parent = null)
    {
        if (_pool.ContainsKey(original.name) == false)
            CreatePool(original);

        return _pool[original.name].Pop(parent);
    }

    /// <summary>
    /// 풀에 저장되어 있는 프리팹을 불러온다.
    /// </summary>
    public GameObject GetOriginal(string name)
    {
        if (_pool.ContainsKey(name) == false)
            return null;
        return _pool[name].Original;
    }

    /// <summary>
    /// 씬이 바뀌었을 때, 생성해놓은 풀을 초기화해주는 코드
    /// </summary>
    public void Clear()
    {
        foreach (Transform child in _root)
            GameObject.Destroy(child.gameObject);

        _pool.Clear();
    }
}
