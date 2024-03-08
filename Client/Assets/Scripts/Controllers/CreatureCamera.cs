using Fusion;
using UnityEngine;
using UnityEngine.Serialization;

public class CreatureCamera : MonoBehaviour
{
    public Creature Creature { get; set; }

    public float MouseSensitivity { get; protected set; }
    public float XRotation { get; protected set; }// 카메라의 상하 회전을 위한 변수

    private void Awake()
    {
        Init();
    }

    protected void Init()
    {
        MouseSensitivity = 100f;
        XRotation = 0f;
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
        float mouseX = Input.GetAxis("Mouse X") * MouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * MouseSensitivity * Time.deltaTime;

        XRotation -= mouseY; // 상하 회전 값 계산
        XRotation = Mathf.Clamp(XRotation, -90f, 90f); // 상하 회전 범위를 -90도에서 90도로 제한

        // 카메라의 상하 회전 적용
        transform.localRotation = Quaternion.Euler(XRotation, 0f, 0f);

        // 플레이어 오브젝트의 좌우 회전 적용
        Creature.transform.Rotate(Vector3.up * mouseX);
    }

    #region Legacy
    // void UpdateCameraAngle()
    // {
    //     float radianAngle = Mathf.Deg2Rad * CurrentAngle; // 원형 궤도 상의 위치 계산
    //     float distance = 2.5f; // 원의 반지름 설정 (조절 가능)
    //     Vector3 cameraPosition = new Vector3(Mathf.Sin(radianAngle) * distance, 1.7f, Mathf.Cos(radianAngle) * distance);
    //
    //     transform.position = Creature.transform.position + cameraPosition; // 플레이어를 중심으로 하는 원형 궤도에 따라 카메라 이동
    //     Vector3 lookDirection = (Creature.transform.position + Vector3.up * 1.7f) - transform.position; // 플레이어를 바라보는 방향 벡터 설정
    //
    //     Quaternion rotation = Quaternion.LookRotation(lookDirection); // 방향 벡터를 회전 각도로 변환
    //     rotation *= Quaternion.Euler(_initialXRotation, 0, 0); // 초기 x축 회전값 적용
    //     transform.rotation = rotation; // 카메라 회전 적용
    // }
    #endregion
}
