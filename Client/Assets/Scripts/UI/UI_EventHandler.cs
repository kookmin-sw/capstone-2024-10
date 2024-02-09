using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// UI_Base에서 이벤트를 등록할 때 사용되는 클래스
/// 이벤트를 수신하여 이벤트가 발생할 때마다 등록된 함수를 호출한다.
/// </summary>
public class UI_EventHandler : MonoBehaviour, IPointerClickHandler, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    /// <summary>
    /// 드래그 이벤트 구독
    /// </summary>
    public Action<PointerEventData> OnDragHandler = null;
    /// <summary>
    /// 클릭 이벤트 구독
    /// </summary>
    public Action<PointerEventData> OnClickHandler = null;
    /// <summary>
    /// 눌림 이벤트 구독
    /// </summary>
    public Action<PointerEventData> OnDownHandler = null;
    /// <summary>
    /// 떼짐 이벤트 구독
    /// </summary>
    public Action<PointerEventData> OnUpHandler = null;

    /// <summary>
    /// UI 요소가 드래그될 때마다 이 함수가 호출된다.
    /// </summary>
    public void OnDrag(PointerEventData eventData)
    {
        // Debug.Log("OnDrag");
        if (OnDragHandler != null)
            OnDragHandler.Invoke(eventData);

    }

    /// <summary>
    /// UI 요소가 클릭될 때마다 이 함수가 호출된다.
    /// </summary>
    public void OnPointerClick(PointerEventData eventData)
    {
        // Debug.Log("OnPointerClick");
        if (OnClickHandler != null)
        {
            OnClickHandler.Invoke(eventData);
        }
    }

    /// <summary>
    /// UI 요소를 눌렀을 때 이 함수가 호출된다.
    /// </summary>
    public void OnPointerDown(PointerEventData eventData)
    {
        // Debug.Log("OnPointerDown");
        if (OnDownHandler != null)
            OnDownHandler.Invoke(eventData);
    }

    /// <summary>
    /// UI 요소에서 뗐을 때 이 함수가 호출된다.
    /// </summary>
    public void OnPointerUp(PointerEventData eventData)
    {
        // Debug.Log("OnPointerUp");
        if (OnUpHandler != null)
            OnUpHandler.Invoke(eventData);
    }
}
