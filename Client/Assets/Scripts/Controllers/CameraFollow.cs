using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{

    public Transform player; // 플레이어의 Transform을 저장할 변수
    public float rotationSpeed = 1.0f; // 카메라 회전 속도
    private float currentAngle = 180f; // 현재 각도

    public float initialXRotation = 10.0f; // 초기 x축 회전값


    void Start()
    {
        // 초기 카메라 위치 설정
        UpdateCameraPosition();
    }
    void Update()
    {
        // 마우스 입력을 받아와 회전 각도 계산
        float mouseX = Input.GetAxis("Mouse X");
        currentAngle += mouseX * rotationSpeed;

        // 각도를 0부터 360도 사이로 유지
        currentAngle = Mathf.Repeat(currentAngle, 360f);

        // 카메라 위치 갱신
        UpdateCameraPosition();

    }

    void UpdateCameraPosition()
    {
        // 원형 궤도 상의 위치 계산
        float radianAngle = Mathf.Deg2Rad * currentAngle;
        float distance = 2.5f; // 원의 반지름 설정 (조절 가능)
        Vector3 cameraPosition = new Vector3(Mathf.Sin(radianAngle) * distance, 1.7f, Mathf.Cos(radianAngle) * distance);

        // 플레이어를 중심으로 하는 원형 궤도에 따라 카메라 이동
        transform.position = player.position + cameraPosition;

        // 플레이어를 바라보는 방향 벡터 설정
        Vector3 lookDirection = (player.position + Vector3.up * 1.7f) - transform.position;

        // 방향 벡터를 회전 각도로 변환
        Quaternion rotation = Quaternion.LookRotation(lookDirection);

        // 초기 x축 회전값 적용
        rotation *= Quaternion.Euler(initialXRotation, 0, 0);

        // 카메라 회전 적용
        transform.rotation = rotation;
    }

}
