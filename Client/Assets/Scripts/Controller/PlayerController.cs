using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    float walkSpeed = 1f;   // 걷기 속도
    float runSpeed = 3f;   // 달리기 속도
    float idleSpeed = 0f;   // 달리기 속도
    float currentSpeed = 0;    //현재 속도
    public float rotationSpeed = 2.0f;  //몸 회전 속도

    public Camera mainCamera;

    float hAxis;
    float vAxis;
    Vector3 moveVec;
    Quaternion targetRotation;

    private bool isRunning = false;


    private Animator animator;
    private Rigidbody rb;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        mainCamera = Camera.main;
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
    }

    void Move()
    {
        if (hAxis == 0 && vAxis == 0 && isRunning == false)
        {
            float smoothness = 5f; // 조절 가능한 부드러움 계수
            currentSpeed = Mathf.Lerp(currentSpeed, idleSpeed, Time.deltaTime * smoothness);
            animator.SetFloat("moveSpeed", currentSpeed);
        }
        else if (isRunning == false)
        {
            moveVec = new Vector3(hAxis, 0, vAxis).normalized;
            transform.position += moveVec * currentSpeed * Time.deltaTime;
            Turn();
            float smoothness = 2f; // 조절 가능한 부드러움 계수
            currentSpeed = Mathf.Lerp(currentSpeed, walkSpeed, Time.deltaTime * smoothness);
            animator.SetFloat("moveSpeed", currentSpeed);
        }
        else
        {
            moveVec = new Vector3(hAxis, 0, vAxis).normalized;
            transform.position += moveVec * currentSpeed * Time.deltaTime;
            Turn();
            float smoothness = 4f; // 조절 가능한 부드러움 계수
            currentSpeed = Mathf.Lerp(currentSpeed, runSpeed, Time.deltaTime * smoothness);
            animator.SetFloat("moveSpeed", currentSpeed);
        }

    }
    void Turn()
    {
        targetRotation = Quaternion.LookRotation(moveVec);
        // 부드러운 회전
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);

    }
}
