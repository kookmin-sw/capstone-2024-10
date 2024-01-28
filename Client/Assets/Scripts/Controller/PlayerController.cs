using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    float walkSpeed = 1f;   // 걷기 속도
    float runSpeed = 3f;   // 달리기 속도
    float idleSpeed = 0f;   // 달리기 속도
    float currentSpeed = 0;    //현재 속도
    float sitSpeed = 0;    //앉는 속도
    float sit_walkSpeed = 0;    //앉아서 걷는 속도
    float rotationSpeed = 5.0f;  //몸 회전 속도


    float hAxis;
    float vAxis;
    Vector3 moveVec;
    Quaternion targetRotation;


    private bool isRunning = false;
    private bool isSitting = false;

    private Animator animator;
    private Rigidbody rb;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        GetInput();
        Move();
    }

    void GetInput() // 키보드 값 받기
    {
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
        isRunning = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        if (Input.GetKeyDown(KeyCode.C))
        {
            isSitting = !isSitting;
        }
    }

    void Move()
    {
        
        if (isSitting)
        {
            float sit_smoothness = 5f; // 조절 가능한 부드러움 계수
            sitSpeed = Mathf.Lerp(sitSpeed, 1, Time.deltaTime * sit_smoothness);
            animator.SetFloat("Sit", sitSpeed);
            if (hAxis == 0 && vAxis == 0)
            {
                float smoothness = 5f; // 조절 가능한 부드러움 계수
                sit_walkSpeed = Mathf.Lerp(sit_walkSpeed, 0, Time.deltaTime * smoothness);
                animator.SetFloat("sitSpeed", sit_walkSpeed);
            }
            else
            {
                // 마우스 입력 받기
                float mouseX = Input.GetAxis("Mouse X");
                // 카메라 회전
                transform.Rotate(Vector3.up, mouseX * rotationSpeed);
                // 로컬 방향을 전역 방향으로 변환
                Vector3 inputDirection = new Vector3(hAxis, 0.0f, vAxis);
                Vector3 worldDirection = transform.TransformDirection(inputDirection);
                // 이동
                transform.Translate(worldDirection * sit_walkSpeed * Time.deltaTime, Space.World);

                float smoothness = 5f; // 조절 가능한 부드러움 계수
                sit_walkSpeed = Mathf.Lerp(sit_walkSpeed, 1, Time.deltaTime * smoothness);
                animator.SetFloat("sitSpeed", sit_walkSpeed);

            }
            
        }
        else
        {
            float sit_smoothness = 5f; // 조절 가능한 부드러움 계수
            sitSpeed = Mathf.Lerp(sitSpeed, 0, Time.deltaTime * sit_smoothness);
            animator.SetFloat("Sit", sitSpeed);

            if (hAxis == 0 && vAxis == 0 && isRunning == false)
            {
                float smoothness = 5f; // 조절 가능한 부드러움 계수
                currentSpeed = Mathf.Lerp(currentSpeed, idleSpeed, Time.deltaTime * smoothness);
                animator.SetFloat("moveSpeed", currentSpeed);
            }
            else if (isRunning == false)
            {
                // 마우스 입력 받기
                float mouseX = Input.GetAxis("Mouse X");

                // 카메라 회전
                transform.Rotate(Vector3.up, mouseX * rotationSpeed);
                // 로컬 방향을 전역 방향으로 변환
                Vector3 inputDirection = new Vector3(hAxis, 0.0f, vAxis);
                Vector3 worldDirection = transform.TransformDirection(inputDirection);
                // 이동
                transform.Translate(worldDirection * currentSpeed * Time.deltaTime, Space.World);

                float smoothness = 2f; // 조절 가능한 부드러움 계수
                currentSpeed = Mathf.Lerp(currentSpeed, walkSpeed, Time.deltaTime * smoothness);
                animator.SetFloat("moveSpeed", currentSpeed);


            }
            else
            {
                // 마우스 입력 받기
                float mouseX = Input.GetAxis("Mouse X");
                // 카메라 회전
                transform.Rotate(Vector3.up, mouseX * rotationSpeed);
                // 로컬 방향을 전역 방향으로 변환
                Vector3 inputDirection = new Vector3(hAxis, 0.0f, vAxis);
                Vector3 worldDirection = transform.TransformDirection(inputDirection);
                // 이동
                transform.Translate(worldDirection * currentSpeed * Time.deltaTime, Space.World);

                float smoothness = 4f; // 조절 가능한 부드러움 계수
                currentSpeed = Mathf.Lerp(currentSpeed, runSpeed, Time.deltaTime * smoothness);
                animator.SetFloat("moveSpeed", currentSpeed);

            }
        }
        

    }
    
}
