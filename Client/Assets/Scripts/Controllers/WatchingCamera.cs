using Fusion;
using UnityEngine;
using UnityEngine.Serialization;

public class WatchingCamera : MonoBehaviour
{
    public Creature Creature { get; set; }

    public Camera Camera { get; protected set; }

    public float MouseSensitivity { get; protected set; }
    public float XRotation { get; protected set; } // 카메라의 상하 회전을 위한 변수
    public float CurrentAngle { get; protected set; }

    float _distance = 2f; // 원의 반지름 설정 (조절 가능)

    private void Awake()
    {
        Init();
    }

    protected void Init()
    {
        Camera = GetComponent<Camera>();

        CurrentAngle = 180f;
        MouseSensitivity = 1.5f;
        XRotation = 0f;
    }

    public void SetInfo(Creature creature)
    {
        enabled = true;
        Creature = creature;

        transform.position = Util.FindChild(Creature.gameObject, "head").transform.position;
    }

    private void LateUpdate()
    {
        if (Creature != null)
        {
            UpdateCameraAngle();
        }
    }

    void UpdateCameraAngle()
    {
        // 마우스 입력을 받아와 회전 각도 계산
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");
        CurrentAngle += mouseX * MouseSensitivity; //좌우 회전 값 계산
        XRotation -= mouseY * MouseSensitivity; // 상하 회전 값 계산

        CurrentAngle = Mathf.Repeat(CurrentAngle, 360f);    // 각도를 0부터 360도 사이로 유지
        XRotation = Mathf.Clamp(XRotation, -90f, 90f);  // 상하 회전 범위를 -90도에서 90도로 제한

        float radianAngle = Mathf.Deg2Rad * CurrentAngle; // 원형 궤도 상의 위치 계산
        Vector3 cameraPosition = new Vector3(Mathf.Sin(radianAngle) * _distance, 1.7f, Mathf.Cos(radianAngle) * _distance);

        transform.position = Creature.transform.position + cameraPosition; // 플레이어를 중심으로 하는 원형 궤도에 따라 카메라 이동
        Vector3 lookDirection = (Creature.transform.position + Vector3.up * 1.7f) - transform.position; // 플레이어를 바라보는 방향 벡터 설정

        Quaternion rotation = Quaternion.LookRotation(lookDirection); // 방향 벡터를 회전 각도로 변환
        rotation *= Quaternion.Euler(XRotation, 0, 0); // 초기 x축 회전값 적용
        transform.rotation = rotation; // 카메라 회전 적용
    }
}
