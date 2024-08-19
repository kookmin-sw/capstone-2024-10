using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// UI 매니저는 UI를 어떻게 관리할지에 대한 정책을 가지고 있는 매니저
/// </summary>
public class UIManager
{
    /// <summary>
    /// UI Canvas가 배치되는 순서
    /// </summary>
    private int _order = 10;

    private static Stack<UI_Popup> _popupStack = new Stack<UI_Popup>();
    public UI_Scene SceneUI { get; set; }

    public UI_Panel PanelUI { get; set; }

    /// <summary>
    /// 모든 UI의 부모가 되는 게임 오브젝트
    /// </summary>
    public GameObject Root
    {
        get
        {
            GameObject root = GameObject.Find("@UI_Root");
            if (root == null)
                root = new GameObject { name = "@UI_Root" };
            return root;
        }
    }

    /// <summary>
    /// UI 프리팹에 캔버스를 달아주고 초기 설정을 해준다. 
    /// </summary>
    /// <param name="go">UI 프리팹</param>
    /// <param name="sort">내부 순서에 영향을 받는지 여부</param>
    public void SetCanvas(GameObject go, bool sort = true)
    {
        Canvas canvas = Util.GetOrAddComponent<Canvas>(go);
        //canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.overrideSorting = true;

        if (sort)
        {
            canvas.sortingOrder = _order;
            _order++;
        }
        else
        {
            canvas.sortingOrder = 0;
        }
    }

    /// <summary>
    /// WorldSpace 폴더에 들어간 UI를 호출하는데 사용하는 함수
    /// WorldSpace는 월드에 부속된 UI를 의미한다.
    /// </summary>
    /// <typeparam name="T">UI_Base를 상속한 사용자 정의 UI</typeparam>
    /// <param name="parent">어느 UI에 붙일지</param>
    /// <param name="name">이름을 지정안하면 클래스 이름으로 파일을 불러온다.</param>
    public T MakeWorldSpaceUI<T>(Transform parent = null, string name = null) where T : UI_Base
    {
        if (string.IsNullOrEmpty(name))
            name = typeof(T).Name;

        GameObject go = Managers.ResourceMng.Instantiate($"UI/WorldSpace/{name}");

        if (parent != null)
            go.transform.SetParent(parent);

        Canvas canvas = go.GetOrAddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.worldCamera = Camera.main;

        return Util.GetOrAddComponent<T>(go);
    }

    /// <summary>
    /// SubItem 폴더에 들어간 UI를 호출하는데 사용하는 함수
    /// SubItem은 인벤토리에 들어가는 아이템을 의미한다.
    /// </summary>
    /// <typeparam name="T">UI_Base를 상속한 사용자 정의 UI</typeparam>
    /// <param name="parent">어느 UI에 붙일지</param>
    /// <param name="name">이름을 지정안하면 클래스 이름으로 파일을 불러온다.</param>
    public T MakeSubItem<T>(Transform parent = null, string name = null) where T : UI_Base
    {
        if (string.IsNullOrEmpty(name))
            name = typeof(T).Name;

        GameObject go = Managers.ResourceMng.Instantiate($"UI/SubItem/{name}");

        if (parent != null)
            go.transform.SetParent(parent);

        go.transform.localRotation = Quaternion.Euler(Vector3.zero);
        go.transform.localPosition = Vector3.zero;

        return Util.GetOrAddComponent<T>(go);
    }

    /// <summary>
    /// SceneUI를 호출하는데 사용하는 함수
    /// SceneUI는 게임이 켜지자마자 보이는 메인화면이다.
    /// </summary>
    /// <typeparam name="T">UI_Scene을 상속한 사용자 정의 UI</typeparam>
    /// <param name="name">이름을 지정안하면 클래스 이름으로 파일을 불러온다.</param>
    public T ShowSceneUI<T>(string name = null) where T : UI_Scene
    {
        if (string.IsNullOrEmpty(name))
            name = typeof(T).Name;

        GameObject go = Managers.ResourceMng.Instantiate($"UI/Scene/{name}");
        T sceneUI = Util.GetOrAddComponent<T>(go);
        SceneUI = sceneUI;

        go.transform.SetParent(Root.transform);

        return sceneUI;
    }

    public T ShowPanelUI<T>(Transform parent = null, string name = null) where T : UI_Panel
    {
        if (string.IsNullOrEmpty(name))
            name = typeof(T).Name;

        GameObject go = Managers.ResourceMng.Instantiate($"UI/Panel/{name}");
        T sceneUI = Util.GetOrAddComponent<T>(go);
        PanelUI = sceneUI;

        if (parent != null)
            go.transform.SetParent(parent);

        return sceneUI;
    }

    /// <summary>
    /// PopupUI를 호출하는데 사용하는 함수
    /// PopupUI는 UI 위에 연속적으로 쌓이는 형태의 UI를 의미한다.
    /// parent와 상관없이 PopupUI는 하나의 연속적인 스택 위에 존재한다.
    /// </summary>
    /// <typeparam name="T">UI_Popup을 상속한 사용자 정의 UI</typeparam>
    /// <param name="parent">어느 UI에 붙일지</param>
    /// <param name="name">이름을 지정안하면 클래스 이름으로 파일을 불러온다.</param>
    public T ShowPopupUI<T>(string name = null, Transform parent = null) where T : UI_Popup
    {
        if (string.IsNullOrEmpty(name))
            name = typeof(T).Name;

        GameObject prefab = Managers.ResourceMng.Load<GameObject>($"Prefabs/UI/Popup/{name}");
        GameObject go = Managers.ResourceMng.Instantiate($"UI/Popup/{name}");
        T popup = Util.GetOrAddComponent<T>(go);
        _popupStack.Push(popup);

        if (parent != null)
            go.transform.SetParent(parent);
        else if (SceneUI != null)
            go.transform.SetParent(SceneUI.transform);
        else
            go.transform.SetParent(Root.transform);

        go.transform.localScale = Vector3.one;
        go.transform.localPosition = prefab.transform.position;
        go.transform.localRotation = Quaternion.Euler(Vector3.zero);

        return popup;
    }

    /// <summary>
    /// UI_Popup을 상속한 PopupUI를 제일 위에 있는 것부터 차례대로 검색해서 일치하는 것을 찾는다.
    /// </summary>
    /// <typeparam name="T">찾을 사용자 정의 PopupUI 타입</typeparam>
    public T FindPopup<T>() where T : UI_Popup
    {
        return _popupStack.Where(x => x.GetType() == typeof(T)).FirstOrDefault() as T;
    }

    /// <summary>
    /// PopupUI 스택의 가장 위에 원하는 PopupUI가 있는지를 확인해본다.
    /// 원하는 PopupUI가 아니면 null을 반환한다.
    /// </summary>
    /// <typeparam name="T">원하는 PopupUI</typeparam>
    public T PeekPopupUI<T>() where T : UI_Popup
    {
        if (_popupStack.Count == 0)
            return null;

        return _popupStack.Peek() as T;
    }

    /// <summary>
    /// PopupUI 스택의 가장 위에 지정한 PopupUI가 있으면 그 팝업을 닫는다.
    /// </summary>
    /// <param name="popup">원하는 PopupUI</param>
    public void ClosePopupUI(UI_Popup popup)
    {
        if (_popupStack.Count == 0)
            return;

        if (_popupStack.Peek() != popup)
        {
            Debug.Log("Close Popup Failed");
            return;
        }

        ClosePopupUI();
    }

    /// <summary>
    /// 가장 위에 있는 팝업 UI를 닫는다.
    /// </summary>
    public void ClosePopupUI()
    {
        if (_popupStack.Count == 0)
            return;

        UI_Popup popup = _popupStack.Pop();
        if (popup != null && popup.gameObject != null)
        {
            Managers.ResourceMng.Destroy(popup.gameObject);
        }
        popup = null;
        _order--;
    }

    public void ClosePopupUIUntil<T>() where T : UI_Popup
    {
        if (_popupStack.Count == 0)
            return;

        if (FindPopup<T>() == null)
            return;

        while (_popupStack.Peek().GetType() != typeof(T))
        {
            ClosePopupUI();
        }
    }

    /// <summary>
    /// 모든 팝업 UI를 닫는다.
    /// </summary>
    public void CloseAllPopupUI()
    {
        while (_popupStack.Count > 0)
            ClosePopupUI();
    }

    public void ActivatePopupUI(bool active)
    {
        _popupStack.ToList().ForEach(x => x.gameObject.SetActive(active));
    }

    public void ClosePanelUI()
    {
        if (PanelUI != null)
        {
            PanelUI.ClosePanelUI();
            PanelUI = null;
        }
    }

    public void ClosePanelUI<T>() where T : UI_Panel
    {
        if (PanelUI != null && PanelUI is T)
        {
            PanelUI.ClosePanelUI();
            PanelUI = null;
        }
    }

    public void CloseSceneUI<T>() where T : UI_Scene
    {
        if (SceneUI != null && SceneUI is T)
        {
            Object.Destroy(SceneUI.gameObject);
            SceneUI = null;
        }
    }

    public void OnLoadingUIDown()
    {
        var loadingUI = Managers.UIMng.PanelUI as UI_Loading;
        // 테스트 씬은 로딩 UI를 띄우지 않음
        if (loadingUI != null)
        {
            loadingUI.shouldWait = false;
        }
    }

    public void OnMapLoadComplete()
    {
        var loadingUI = Managers.UIMng.PanelUI as UI_Loading;
        // 테스트 씬은 로딩 UI를 띄우지 않음
        if (loadingUI != null)
        {
            loadingUI.IsMapLoaded = true;
        }
    }

    /// <summary>
    /// 모든 팝업 UI와 씬 Ui를 닫는 초기화를 수행한다.
    /// </summary>
    public void Clear()
    {
        CloseAllPopupUI();
        if (SceneUI != null)
        {
            Object.Destroy(SceneUI.gameObject);
            SceneUI = null;
        }
        SceneUI = null;
    }
}
