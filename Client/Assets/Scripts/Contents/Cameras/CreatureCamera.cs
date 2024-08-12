using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class CreatureCamera : MonoBehaviour
{
    public Creature Creature { get; set; }

    public Transform Transform { get; protected set; }
    public Camera Camera { get; protected set; }

    public float MouseSensitivity { get; protected set; } = 1.5f;
    public float XRotation { get; protected set; } = 0f; // 카메라의 상하 회전 값
    public float CurrentAngle { get; protected set; } = 0f;

    public Vector3 LastForward { get; protected set; }

    private void Awake()
    {
        Init();
    }

    protected void Init()
    {
        Transform = transform;
        Camera = GetComponent<Camera>();
    }

    public void SetInfo(Creature creature)
    {
        enabled = true;
        Creature = creature;
    }

    public void UpdateCameraAngle()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        if (!Creature.HasStateAuthority || Creature.CreatureState == Define.CreatureState.Dead || !Creature.IsSpawned)
            return;

        try
        {
            if (Creature.CreatureState == Define.CreatureState.Damaged || Creature.CreatureState == Define.CreatureState.Interact || Creature.CreatureState == Define.CreatureState.Use)
            {
                Transform.forward = LastForward;
                return;
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to UpdateCameraAngle: " + e);
        }

        // 마우스 입력을 받아와 회전 각도 계산
        float mouseX = Input.GetAxis("Mouse X") * Managers.GameMng.SettingSystem.Sensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * Managers.GameMng.SettingSystem.Sensitivity;
        CurrentAngle += mouseX * MouseSensitivity; //좌우 회전 값 계산
        XRotation -= mouseY * MouseSensitivity; // 상하 회전 값 계산

        CurrentAngle = Mathf.Repeat(CurrentAngle, 360f);    // 각도를 0부터 360도 사이로 유지
        XRotation = Mathf.Clamp(XRotation, -60f, 60f);  // 상하 회전 범위를 -90도에서 90도로 제한

        Quaternion rotation = Quaternion.Euler(XRotation, CurrentAngle, 0);
        Transform.rotation = rotation; // 카메라 회전 적용

        LastForward = Transform.forward;
    }
}
