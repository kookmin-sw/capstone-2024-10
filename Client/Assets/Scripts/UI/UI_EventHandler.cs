using System;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// UI_Base에서 이벤트를 등록할 때 사용되는 클래스
/// 이벤트를 수신하여 이벤트가 발생할 때마다 등록된 함수를 호출한다.
/// </summary>
public class UI_EventHandler : MonoBehaviour, IPointerClickHandler, IDragHandler, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Action<PointerEventData> OnDragHandler = null;
    public Action<PointerEventData> OnClickHandler = null;
    public Action<PointerEventData> OnDownHandler = null;
    public Action<PointerEventData> OnUpHandler = null;
    public Action<PointerEventData> OnEnterHandler = null;
    public Action<PointerEventData> OnExitHandler = null;

    public void OnDrag(PointerEventData eventData)
    {
        // Debug.Log("OnDrag");
        if (OnDragHandler != null)
            OnDragHandler.Invoke(eventData);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // Debug.Log("OnPointerClick");
        if (OnClickHandler != null)
        {
            OnClickHandler.Invoke(eventData);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // Debug.Log("OnPointerDown");
        if (OnDownHandler != null)
            OnDownHandler.Invoke(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // Debug.Log("OnPointerUp");
        if (OnUpHandler != null)
            OnUpHandler.Invoke(eventData);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Debug.Log("OnPointerEnter");
        if (OnEnterHandler != null)
            OnEnterHandler.Invoke(eventData);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Debug.Log("OnPointerExit");
        if (OnExitHandler != null)
            OnExitHandler.Invoke(eventData);
    }
}
