using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 기존의 Load 함수와 Instantiate 함수를 매핑한 매니저
/// poolable 스크립트 부착 여부에 따라 오브젝트 풀링을 실시함
/// </summary>
public class ResourceManager
{
    /// <summary>
    /// poolable 객체면 미리 저장된 원본 오브젝트를 가져온다.
    /// </summary>
    public T Load<T>(string path) where T : Object
    {
        if (typeof(T) == typeof(GameObject))
        {
            string name = path;
            int index = name.LastIndexOf('/');
            if (index >= 0)
                name = name.Substring(index + 1);

            GameObject go = Managers.Pool.GetOriginal(name);
            if (go != null)
                return go as T;
        }
        return Resources.Load<T>(path);
    }

    /// <summary>
    /// poolable 객체면 풀링된 오브젝트를 가져온다.
    /// </summary>
    public GameObject Instantiate(string path, Transform parent = null)
    {
        GameObject original = Load<GameObject>($"Prefabs/{path}");
        if (original == null)
        {
            Debug.Log($"Failed to load prefab : {path}");
            return null;
        }

        if (original.GetComponent<Poolable>() != null)
            return Managers.Pool.Pop(original, parent).gameObject;

        GameObject go = Object.Instantiate(original, parent);
        go.name = original.name;
        return go;
    }

    /// <summary>
    /// poolable 객체면 풀링된 오브젝트를 다시 풀로 되돌린다.
    /// </summary>
    public void Destroy(GameObject go)
    {
        if (go == null)
            return;

        Poolable poolable = go.GetComponent<Poolable>();
        if (poolable != null)
        {
            Managers.Pool.Push(poolable);
            return;
        }
        Object.Destroy(go);
    }
}
