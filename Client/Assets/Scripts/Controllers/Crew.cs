using Data;
using UnityEngine;

public class Crew : Creature
{
    #region Field
    public CrewData CrewData => CreatureData as CrewData;
    public CrewStat CrewStatus => (CrewStat)CreatureStat;

    public override void Rpc_SetInfo(int templateID)
    {
        CreatureType = Define.CreatureType.Crew;
        Transform.parent = Managers.ObjectMng.CrewRoot;

        base.Rpc_SetInfo(templateID);
    }
    #endregion

    #region Update
    protected override void UpdateIdle()
    {
    }

    protected override void UpdateMove()
    {
        KCC.Move(Velocity, 0f);

        Vector3 dir = Velocity;
        dir.y = 0;
        Transform.forward = dir;
    }

    protected override void UpdateUseItem()
    {
    }

    protected override void UpdateDead()
    {
    }
    #endregion

    #region Event
    public void OnDamaged(int damage)
    {
        CrewStatus.OnDamage(damage);

        if (CrewStatus.Hp <= 0)
        {
            OnDead();
            return;
        }
    }

    public void OnDead()
    {
        CreatureState = Define.CreatureState.Dead;
    }
    #endregion


    #region Legacy
    /*
    float currentSpeed = 0;    //현재 속도

    float sitSpeed = 0;    //앉는 속도
    float sit_walkSpeed = 0;    //앉아서 걷는 속도

    float rotationSpeed = 5.0f;  //몸 회전 속도
    public float currentHealth = 100;  //현재 체력

    float hAxis;
    float vAxis;

    private bool isRunning = false;
    private bool isSitting = false;
    private bool isDeath = false;
    //private bool isCasting = false;
    //private bool isPicking = false;


    private Animator _animator;
    private CharacterController _controller;
    public Camera Camera;

    public override void Spawned()
    {
        if (HasStateAuthority)
        {
            Camera = Camera.main;
            Camera.GetComponent<CameraFollow>().player = transform;
            _animator = GetComponent<Animator>();
            _controller = GetComponent<CharacterController>();
        }
    }
    void Update()
    {
        GetInput();
    }

    public override void FixedUpdateNetwork()
    {
        Network_Move();
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

    void Network_Move()
    {
        if (HasStateAuthority == false)
        {
            return;
        }

        if (currentHealth == 100)
        {
            _animator.SetFloat("Health", currentHealth);
            if (isSitting)
            {
                float sit_smoothness = 5f; // 조절 가능한 부드러움 계수
                sitSpeed = Mathf.Lerp(sitSpeed, 1, Runner.DeltaTime * sit_smoothness);
                _animator.SetFloat("Sit", sitSpeed);
                if (hAxis == 0 && vAxis == 0)
                {
                    float smoothness = 5f; // 조절 가능한 부드러움 계수
                    sit_walkSpeed = Mathf.Lerp(sit_walkSpeed, 0, Runner.DeltaTime * smoothness);
                    _animator.SetFloat("sitSpeed", sit_walkSpeed);
                }
                else
                {
                    Quaternion cameraRotationY = Quaternion.Euler(0, Camera.transform.rotation.eulerAngles.y, 0);
                    Vector3 move = cameraRotationY * new Vector3(hAxis, 0, vAxis) * Runner.DeltaTime * sit_walkSpeed;

                    _controller.Move(move);

                    if (move != Vector3.zero)
                    {
                        gameObject.transform.forward = move;
                    }
                    float smoothness = 5f; // 조절 가능한 부드러움 계수
                    sit_walkSpeed = Mathf.Lerp(sit_walkSpeed, 1, Runner.DeltaTime * smoothness);
                    _animator.SetFloat("sitSpeed", sit_walkSpeed);
                }
            }
            else
            {
                float sit_smoothness = 5f; // 조절 가능한 부드러움 계수
                sitSpeed = Mathf.Lerp(sitSpeed, 0, Runner.DeltaTime * sit_smoothness);
                _animator.SetFloat("Sit", sitSpeed);
                if (hAxis == 0 && vAxis == 0 && isRunning == false)
                {
                    float smoothness = 5f; // 조절 가능한 부드러움 계수
                    currentSpeed = Mathf.Lerp(currentSpeed, 0, Runner.DeltaTime * smoothness);
                    _animator.SetFloat("moveSpeed", currentSpeed);
                }
                else if (isRunning == false)
                {
                    Quaternion cameraRotationY = Quaternion.Euler(0, Camera.transform.rotation.eulerAngles.y, 0);
                    Vector3 move = cameraRotationY * new Vector3(hAxis, 0, vAxis) * Runner.DeltaTime * currentSpeed;

                    _controller.Move(move);

                    if (move != Vector3.zero)
                    {
                        gameObject.transform.forward = move;
                    }
                    float smoothness = 2f; // 조절 가능한 부드러움 계수
                    currentSpeed = Mathf.Lerp(currentSpeed, 2, Runner.DeltaTime * smoothness);
                    _animator.SetFloat("moveSpeed", currentSpeed);
                }
                else
                {
                    Quaternion cameraRotationY = Quaternion.Euler(0, Camera.transform.rotation.eulerAngles.y, 0);
                    Vector3 move = cameraRotationY * new Vector3(hAxis, 0, vAxis) * Runner.DeltaTime * currentSpeed;

                    _controller.Move(move);

                    if (move != Vector3.zero)
                    {
                        gameObject.transform.forward = move;
                    }
                    float smoothness = 4f; // 조절 가능한 부드러움 계수
                    currentSpeed = Mathf.Lerp(currentSpeed, 3.5f, Runner.DeltaTime * smoothness);
                    _animator.SetFloat("moveSpeed", currentSpeed);
                }
            }
        }
        else if (currentHealth == 50)
        {
            _animator.SetFloat("Health", currentHealth);
            if (isSitting)
            {
                float sit_smoothness = 5f; // 조절 가능한 부드러움 계수
                sitSpeed = Mathf.Lerp(sitSpeed, 1, Runner.DeltaTime * sit_smoothness);
                _animator.SetFloat("Sit", sitSpeed);
                if (hAxis == 0 && vAxis == 0)
                {
                    float smoothness = 5f; // 조절 가능한 부드러움 계수
                    sit_walkSpeed = Mathf.Lerp(sit_walkSpeed, 0, Runner.DeltaTime * smoothness);
                    _animator.SetFloat("sitSpeed", sit_walkSpeed);
                }
                else
                {
                    Quaternion cameraRotationY = Quaternion.Euler(0, Camera.transform.rotation.eulerAngles.y, 0);
                    Vector3 move = cameraRotationY * new Vector3(hAxis, 0, vAxis) * Runner.DeltaTime * sit_walkSpeed;

                    _controller.Move(move);

                    if (move != Vector3.zero)
                    {
                        gameObject.transform.forward = move;
                    }
                    float smoothness = 5f; // 조절 가능한 부드러움 계수
                    sit_walkSpeed = Mathf.Lerp(sit_walkSpeed, 1, Runner.DeltaTime * smoothness);
                    _animator.SetFloat("sitSpeed", sit_walkSpeed);
                }
            }
            else
            {
                float sit_smoothness = 5f; // 조절 가능한 부드러움 계수
                sitSpeed = Mathf.Lerp(sitSpeed, 0, Runner.DeltaTime * sit_smoothness);
                _animator.SetFloat("Sit", sitSpeed);
                if (hAxis == 0 && vAxis == 0 && isRunning == false)
                {
                    float smoothness = 5f; // 조절 가능한 부드러움 계수
                    currentSpeed = Mathf.Lerp(currentSpeed, 0, Runner.DeltaTime * smoothness);
                    _animator.SetFloat("moveSpeed", currentSpeed);
                }
                else if (isRunning == false)
                {
                    Quaternion cameraRotationY = Quaternion.Euler(0, Camera.transform.rotation.eulerAngles.y, 0);
                    Vector3 move = cameraRotationY * new Vector3(hAxis, 0, vAxis) * Runner.DeltaTime * currentSpeed;

                    _controller.Move(move);

                    if (move != Vector3.zero)
                    {
                        gameObject.transform.forward = move;
                    }
                    float smoothness = 2f; // 조절 가능한 부드러움 계수
                    currentSpeed = Mathf.Lerp(currentSpeed, 1.5f, Runner.DeltaTime * smoothness);
                    _animator.SetFloat("moveSpeed", currentSpeed);
                }
                else
                {
                    Quaternion cameraRotationY = Quaternion.Euler(0, Camera.transform.rotation.eulerAngles.y, 0);
                    Vector3 move = cameraRotationY * new Vector3(hAxis, 0, vAxis) * Runner.DeltaTime * currentSpeed;

                    _controller.Move(move);

                    if (move != Vector3.zero)
                    {
                        gameObject.transform.forward = move;
                    }
                    float smoothness = 4f; // 조절 가능한 부드러움 계수
                    currentSpeed = Mathf.Lerp(currentSpeed, 2.5f, Runner.DeltaTime * smoothness);
                    _animator.SetFloat("moveSpeed", currentSpeed);
                }
            }
        }
        else
        {
            isDeath = true;
            _animator.SetBool("IsDeath", true);
        }
    }
    */
    #endregion
}
