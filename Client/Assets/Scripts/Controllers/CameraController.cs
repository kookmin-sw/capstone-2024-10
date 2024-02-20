using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Serialization;

public class CameraController : MonoBehaviour
{
    public Transform Player { get; set; }
    public float RotationSpeed { get; protected set; }
    public float CurrentAngle { get; protected set; }

    private float _initialXRotation = 10.0f;

    private void Awake()
    {
        Init();
    }

    protected void Init()
    {
        RotationSpeed = 1f;
        CurrentAngle = 180f;
    }

    private void LateUpdate()
    {
        float mouseX = Input.GetAxis("Mouse X"); // 마우스 입력을 받아와 회전 각도 계산
        CurrentAngle += mouseX * RotationSpeed;
        CurrentAngle = Mathf.Repeat(CurrentAngle, 360f); // 각도를 0부터 360도 사이로 유지

        if (Managers.NetworkMng.Player != null)
        {
            UpdateCameraPosition();
        }
    }

    void UpdateCameraPosition()
    {
        float radianAngle = Mathf.Deg2Rad * CurrentAngle; // 원형 궤도 상의 위치 계산
        float distance = 2.5f; // 원의 반지름 설정 (조절 가능)
        Vector3 cameraPosition = new Vector3(Mathf.Sin(radianAngle) * distance, 1.7f, Mathf.Cos(radianAngle) * distance);

        transform.position = Player.position + cameraPosition; // 플레이어를 중심으로 하는 원형 궤도에 따라 카메라 이동
        Vector3 lookDirection = (Player.position + Vector3.up * 1.7f) - transform.position; // 플레이어를 바라보는 방향 벡터 설정

        Quaternion rotation = Quaternion.LookRotation(lookDirection); // 방향 벡터를 회전 각도로 변환
        rotation *= Quaternion.Euler(_initialXRotation, 0, 0); // 초기 x축 회전값 적용
        transform.rotation = rotation; // 카메라 회전 적용
    }

}
