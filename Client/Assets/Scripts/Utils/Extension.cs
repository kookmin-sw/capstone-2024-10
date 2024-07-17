using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Fusion;

public static class Extension
{
    public static void AddUIEvent(this GameObject go, Action<PointerEventData> action, Define.UIEvent type = Define.UIEvent.Click)
    {
        UI_Base.BindEvent(go, action, type);
    }

    public static void BindEvent(this GameObject go, Action<PointerEventData> action, Define.UIEvent type = Define.UIEvent.Click)
    {
        UI_Base.BindEvent(go, action, type);
    }

    public static T GetOrAddComponent<T>(this GameObject go) where T : UnityEngine.Component
    {
        return Util.GetOrAddComponent<T>(go);
    }

    public static List<GameObject> FindObjectsWithTag(this Transform parent, string tag)
    {
        List<GameObject> taggedGameObjects = new List<GameObject>();

        for (int i = 0; i < parent.childCount; i++)
        {
            Transform child = parent.GetChild(i);
            if (child.CompareTag(tag))
            {
                taggedGameObjects.Add(child.gameObject);
            }
            if (child.childCount > 0)
            {
                taggedGameObjects.AddRange(FindObjectsWithTag(child, tag));
            }
        }
        return taggedGameObjects;
    }

    public static bool IsSceneType(this BaseScene scene, int sceneIndex)
    {
        int idx = (int)scene.SceneType;
        return (idx | sceneIndex) == sceneIndex;
    }

    // SimulationBehaviour를 Runner에 할당
    // Runner.Spawn 등 Runner를 사용하는 데 필요함
    public static void RegisterRunner(this SimulationBehaviour sb)
    {
        var runner = NetworkRunner.GetRunnerForGameObject(sb.gameObject);
        if (runner.IsRunning)
        {
            runner.AddGlobal(sb);
        }
    }

    public static void SetLayerRecursive(this GameObject root, int layer)
    {
        if (root == null) return;

        if (root.layer != LayerMask.NameToLayer("AlienCollision"))
            root.layer = layer;

        // 모든 자식 레이어 변경
        foreach (Transform child in root.transform)
        {
            SetLayerRecursive(child.gameObject, layer);
        }
    }

    public static void SetLayerRecursive(this GameObject[] root, int layer)
    {
        foreach (var go in root)
        {
            go.SetLayerRecursive(layer);
        }
    }
}
