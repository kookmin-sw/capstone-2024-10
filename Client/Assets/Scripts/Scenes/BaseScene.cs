using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 모든 씬에 하나씩은 있어야 하는 씬 스크립트를 만들기 위한 상속 원형.
/// 해당 클래스를 상속해서 만든 씬 스크립트는 반드시 씬마다 하나씩
/// @Scene 오브젝트 파일에 붙어 있어야 한다.
/// </summary>
public abstract class BaseScene : MonoBehaviour
{
    /// <summary>
    /// enum으로 정의된 씬 타입을 가져올 수 있다.
    /// </summary>
    public Define.Scene SceneType { get; protected set; } = Define.Scene.Unknown;

    void Awake()
    {
        init();
    }

    /// <summary>
    /// EventSystem이 없으면 @EventSystem으로 생성한다.
    /// 씬을 깔끔하게 관리하기 위해서 기존의 EventSystem을 프리팹화 시켰다.
    /// 씬 스크립트를 구현할 때 새로운 Init을 정의한다면 함수 내부에서 호출해줘야 한다.
    /// </summary>
    protected virtual void init()
    {
        Object obj = GameObject.FindObjectOfType(typeof(EventSystem));
        if (obj == null)
            Managers.Resource.Instantiate("UI/EventSystem").name = "@EventSystem";
    }

    /// <summary>
    /// 각 씬 스크립트가 구현해야 할 함수
    /// </summary>
    public abstract void Clear();
}
