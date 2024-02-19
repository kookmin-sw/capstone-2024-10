using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.PlayerLoop;

// 게임 내 모든 입력 처리
public class InputManager
{
    public Action<Define.MouseEvent> MouseAction;

    private bool _pressed;
    private float _pressedTime;

    public void Init()
    {
        MouseAction = null;

        _pressed = false;
        _pressedTime = 0;
    }

    // 입력이 없다면 바로 리턴, 입력이 있다면 KeyAction/MouseAction을 Invoke
    public void OnUpdate()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;

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
                    if (Time.time < _pressedTime + 0.2f)
                        MouseAction.Invoke(Define.MouseEvent.Click);
                    MouseAction.Invoke(Define.MouseEvent.PointerUp);

                }

                _pressed = false;
                _pressedTime = 0;
            }
        }
    }

    public void Clear()
    {
        MouseAction = null;
    }
}
