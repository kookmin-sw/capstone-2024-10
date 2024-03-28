using System;
using UnityEngine;

public class CreatureCamera : MonoBehaviour
{
    public Creature Creature { get; set; }

    public Transform CameraTransform { get; protected set; }
    public Camera Camera { get; protected set; }

    public float MouseSensitivity { get; protected set; }
    public float XRotation { get; protected set; } // 카메라의 상하 회전 값
    public float CurrentAngle { get; protected set; }

    public Vector3 LastForward { get; protected set; }

    private void Awake()
    {
        Init();
    }

    protected void Init()
    {
        CameraTransform = transform;
        Camera = GetComponent<Camera>();

        CurrentAngle = 0;
        MouseSensitivity = 1.5f;
        XRotation = 0;
    }

    public void SetInfo(Creature creature)
    {
        enabled = true;
        Creature = creature;
    }

    public void UpdateCameraAngle()
    {
        try
        {
            if (Creature.CreatureState == Define.CreatureState.Damaged || Creature.CreatureState == Define.CreatureState.Dead)
            {
                CameraTransform.forward = Creature.transform.forward;
                return;
            }

            if (Creature.CreatureState == Define.CreatureState.Interact || Creature.CreatureState == Define.CreatureState.Use)
            {
                CameraTransform.forward = LastForward;
                return;
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to UpdateCameraAngle: " + e);
        }

        // 마우스 입력을 받아와 회전 각도 계산
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");
        CurrentAngle += mouseX * MouseSensitivity; //좌우 회전 값 계산
        XRotation -= mouseY * MouseSensitivity; // 상하 회전 값 계산

        CurrentAngle = Mathf.Repeat(CurrentAngle, 360f);    // 각도를 0부터 360도 사이로 유지
        XRotation = Mathf.Clamp(XRotation, -60f, 60f);  // 상하 회전 범위를 -90도에서 90도로 제한

        Quaternion rotation = Quaternion.Euler(XRotation, CurrentAngle, 0);
        CameraTransform.rotation = rotation; // 카메라 회전 적용

        LastForward = CameraTransform.forward;
    }
}
