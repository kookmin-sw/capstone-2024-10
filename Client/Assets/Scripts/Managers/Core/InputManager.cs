using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 자주 사용되는 복잡한 인풋을 이벤트 핸들러 방식으로 호출하게 만든 매니저
/// </summary>
public class InputManager
{
    /// <summary>
    /// 키보드에 이벤트가 들어왔을 때, 어떤 동작을 취할지 함수를 등록한다.
    /// </summary>
    public Action KeyAction = null;
    /// <summary>
    /// 마우스에 이벤트가 들어왔을 때, 어떤 동작을 취할지 함수를 등록한다.
    /// </summary>
    public Action<Define.MouseEvent> MouseAction = null;

    /// <summary>
    /// 내부에서 상태를 저장하기 위해서 만든 변수 외부에서 수정하면 안 된다.
    /// </summary>
    bool _pressed = false;
    /// <summary>
    /// 내부에서 상태를 저장하기 위해서 만든 변수 외부에서 수정하면 안 된다.
    /// </summary>
    float _pressedTime = 0;
    
    /// <summary>
    /// 지정된 로직을 순회하면서 각각의 이벤트에 알맞은 함수들을 부른다.
    /// 매니저가 사용하는 함수로 일반 사용자 스크립트에서 사용하면 안 된다.
    /// </summary>
    public void OnUpdate()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        if (Input.anyKey && KeyAction != null)
            KeyAction.Invoke();

        if (MouseAction != null)
        {
            if (Input.GetMouseButton(0))
            {
                if (!_pressed)
                {
                    MouseAction.Invoke(Define.MouseEvent.PointerDown);
                    _pressedTime = Time.time;
                }
                MouseAction.Invoke(Define.MouseEvent.Press);
                _pressed = true;
            }
            else
            {
                if (_pressed)
                {
                    if (Time.time < _pressedTime * 0.2f)
                        MouseAction.Invoke(Define.MouseEvent.Click);
                    MouseAction.Invoke(Define.MouseEvent.PointerUp);
                }
                _pressed = false;
                _pressedTime = 0;
            }
        }
    }

    /// <summary>
    /// 화면의 초기화가 일어날 때 데이터를 초기화하는 함수
    /// </summary>
    public void Clear()
    {
        KeyAction = null;
        MouseAction = null;
    }
}
