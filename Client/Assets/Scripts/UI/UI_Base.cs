using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 모든 UI의 베이스가 되는 클래스
/// 씬 UI, 팝업 UI도 해당 UI를 상속해서 구현되어 있다.
/// UI 요소를 변수에 연결하는 오브젝트 바인딩 기능과
/// UI 요소를 입력 이벤트와 연결하는 함수 바인딩 기능을 포함하고 있다.
/// </summary>
public abstract class UI_Base : MonoBehaviour
{
    protected Dictionary<Type, UnityEngine.Object[]> _objects = new Dictionary<Type, UnityEngine.Object[]>();

    protected bool _init = false;

    private void Start()
    {
        Init();
    }

    public virtual bool Init()
    {
        if (_init)
            return false;

        return _init = true;
    }

    private void Update()
    {
        OnUpdate();
    }

    public virtual void OnUpdate() {}

    /// <summary>
    /// enum 타입의 이름들을 스트링으로 변환해 UI 요소에 연결한다.
    /// </summary>
    /// <typeparam name="T">UI 요소</typeparam>
    /// <param name="type">enum 타입</param>
    protected void Bind<T>(Type type) where T : UnityEngine.Object
    {
        string[] names = Enum.GetNames(type);

        if (names.Length == 0) return;

        int startIdx = 0;

        if (!_objects.TryGetValue(typeof(T), out UnityEngine.Object[] objects))
        {
            // T key값이 _objects에 없음
            objects = new UnityEngine.Object[names.Length];
            _objects[typeof(T)] = objects;
        }
        else
        {
            // T key값이 이미 _objects에 있음: 배열 길이 늘리기
            startIdx = objects.Length;
            Array.Resize(ref objects, objects.Length + names.Length);
            _objects[typeof(T)] = objects;
        }

        for (int i = 0; i < names.Length; i++)
        {
            if (typeof(T) == typeof(GameObject))
                objects[i + startIdx] = Util.FindChild(gameObject, names[i], true);
            else
                objects[i + startIdx] = Util.FindChild<T>(gameObject, names[i], true);

            if (objects[i + startIdx] == null)
                Debug.Log($"Failed to bind({names[i]})");
        }
    }

    /// <summary>
    /// 등록된 enum 타입 중에서 해당하는 인덱스를 가진 UI 요소를 가져온다.
    /// 인덱스는 enum 타입 앞에 (int)를 붙이면 얻어올 수 있다.
    /// GetObject, GetButton, GetText, GetImage 시리즈 함수를 사용하는 걸 추천한다.
    /// </summary>
    /// <typeparam name="T">UI 요소</typeparam>
    /// <param name="idx">enum 타입 인덱스</param>
    /// <returns></returns>
    protected T Get<T>(int idx) where T : UnityEngine.Object
    {
        UnityEngine.Object[] objects = null;
        if (_objects.TryGetValue(typeof(T), out objects) == false)
            return null;

        return objects[idx] as T;
    }
    protected T Get<T>(Enum idx) where T : UnityEngine.Object
    {
        UnityEngine.Object[] objects;
        if (_objects.TryGetValue(typeof(T), out objects) == false)
            return null;

        return objects[Convert.ToInt32(idx)] as T;
    }

    /// <summary>
    /// Get<GameObject> 함수를 래핑한 함수
    /// </summary>
    /// <param name="idx">enum 타입 인덱스</param>
    /// <returns></returns>
    protected GameObject GetObject(int idx) { return Get<GameObject>(idx); }
    protected GameObject GetObject(Enum idx) { return Get<GameObject>(idx); }
    /// <summary>
    /// Get<TMP_Text> 함수를 래핑한 함수
    /// </summary>
    /// <param name="idx">enum 타입 인덱스</param>
    /// <returns></returns>
    protected TMP_Text GetText(int idx) { return Get<TMP_Text>(idx); }
    protected TMP_Text GetText(Enum idx) { return Get<TMP_Text>(idx); }
    /// <summary>
    /// Get<Button> 함수를 래핑한 함수
    /// </summary>
    /// <param name="idx">enum 타입 인덱스</param>
    /// <returns></returns>
    protected Button GetButton(int idx) { return Get<Button>(idx); }
    protected Button GetButton(Enum idx) { return Get<Button>(idx); }
    /// <summary>
    /// Get<Image> 함수를 래핑한 함수
    /// </summary>
    /// <param name="idx">enum 타입 인덱스</param>
    /// <returns></returns>
    protected Image GetImage(int idx) { return Get<Image>(idx); }
    protected Image GetImage(Enum idx) { return Get<Image>(idx); }

    /// <summary>
    /// UI 요소에 UI_EventHanlder를 부착해 Event를 수신하게 만들고 파라미터로 주어진 이벤트를 등록한다.
    /// </summary>
    /// <param name="go">UI 요소</param>
    /// <param name="action">이벤트 함수</param>
    /// <param name="type">구독할 이벤트 종류</param>
    protected Slider GetSlider(int idx) { return Get<Slider>(idx); }
    protected Slider GetSlider(Enum idx) { return Get<Slider>(idx); }
    /// <summary>
    /// Get<Image> 함수를 래핑한 함수
    /// </summary>
    /// <param name="idx">enum 타입 인덱스</param>
    /// <returns></returns>
    public static void BindEvent(GameObject go, Action<PointerEventData> action, Define.UIEvent type = Define.UIEvent.Click)
    {
        UI_EventHandler evt = Util.GetOrAddComponent<UI_EventHandler>(go);

        switch (type)
        {
            case Define.UIEvent.Click:
                evt.OnClickHandler -= action;
                evt.OnClickHandler += action;
                break;
            case Define.UIEvent.Press:
                evt.OnDragHandler -= action;
                evt.OnDragHandler += action;
                break;
            case Define.UIEvent.PointerDown:
                evt.OnDownHandler -= action;
                evt.OnDownHandler += action;
                break;
            case Define.UIEvent.PointerUp:
                evt.OnUpHandler -= action;
                evt.OnUpHandler += action;
                break;
            case Define.UIEvent.PointerEnter:
                evt.OnEnterHandler -= action;
                evt.OnEnterHandler += action;
                break;
            case Define.UIEvent.PointerExit:
                evt.OnExitHandler -= action;
                evt.OnExitHandler += action;
                break;
            case Define.UIEvent.Drag:
                evt.OnDragHandler -= action;
                evt.OnDragHandler += action;
                break;
        }
    }
}
