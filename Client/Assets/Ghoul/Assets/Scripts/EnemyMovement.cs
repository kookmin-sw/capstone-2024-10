using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float rotationSpeed = 180f;

    void Update()
    {
        // 키 입력을 받아 이동 및 회전 방향을 계산합니다.
        float verticalInput = Input.GetAxis("Vertical");
        float horizontalInput = Input.GetAxis("Horizontal");

        // 앞 뒤 이동 방향을 계산합니다.
        Vector3 forwardMovement = transform.forward * verticalInput;

        // 좌 우 회전을 계산합니다.
        Vector3 rotation = new Vector3(0, horizontalInput * rotationSpeed * Time.deltaTime, 0);

        // 이동 벡터에 이동 속도를 곱하여 물체를 이동시킵니다.
        transform.Translate(forwardMovement * moveSpeed * Time.deltaTime, Space.World);

        // 회전 벡터에 회전 속도를 곱하여 물체를 회전시킵니다.
        transform.Rotate(rotation);
    }
}