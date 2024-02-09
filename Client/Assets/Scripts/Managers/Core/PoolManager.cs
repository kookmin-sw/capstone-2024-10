using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.Image;

/// <summary>
/// 풀링 매니저는 리소스 매니저에 귀속되어 있는 매니저로 사용자 스크립트에서 직접 호출할 필요가 없다.
/// </summary>
public class PoolManager
{
    #region Pool
    class Pool
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
        public void init(GameObject original, int count = 5)
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
                poolable.transform.parent = Managers.Scene.CurrentScene.transform;

            poolable.transform.parent = parent;
            poolable.IsUsing = true;

            return poolable;
        }
    }
    #endregion

    Dictionary<string, Pool> _pool = new Dictionary<string, Pool>();
    Transform _root;

    /// <summary>
    /// 모든 풀의 루트를 만들어 주고 씬이 바뀌어도 삭제되지 않게 설정을 한다.
    /// </summary>
    public void init()
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
        pool.init(original, count);
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
