using Fusion;
using Fusion.Addons.SimpleKCC;
using UnityEngine;

public abstract class Creature : NetworkBehaviour
{
    #region Field

    private float _hAxis;
    private float _vAxis;

    public Camera Camera => Camera.main;
    public Transform Transform { get; protected set; }
    public CircleCollider2D Collider { get; protected set; }
    public Rigidbody2D RigidBody { get; protected set; }
    public NetworkObject NetworkObject { get; protected set; }
    public NetworkMecanimAnimator NetworkAnim { get; protected set; }
    public SimpleKCC KCC { get; protected set; }
    public CreatureStat CreatureStat { get; protected set; }
    public Animator Anim { get; protected set; }

    public int DataId { get; protected set; }
    public Data.CreatureData CreatureData { get; protected set; }
    [Networked] public Define.CreatureType CreatureType { get; protected set; }

    [Networked] private Define.CreatureState _creatureState { get; set; }
    public Define.CreatureState CreatureState
    {
        get => _creatureState;
        set
        {
            if (_creatureState != value)
            {
                _creatureState = value;
                UpdateAnimation();
            }
        }
    }
    [Networked] public Define.CreaturePose CreaturePose { get; protected set; }
    [Networked] public NetworkBool _IsDamaged { get; protected set; }

    [Networked] public Vector3 Velocity { get; protected set; }
    #endregion

    private void Awake()
    {
        //Init();
    }

    public override void Spawned()
    {
        Init();
    }

    protected virtual void Init()
    {
        Transform = gameObject.GetOrAddComponent<Transform>();
        Collider = gameObject.GetOrAddComponent<CircleCollider2D>();
        RigidBody = gameObject.GetOrAddComponent<Rigidbody2D>();
        NetworkObject = gameObject.GetOrAddComponent<NetworkObject>();
        NetworkAnim = gameObject.GetOrAddComponent<NetworkMecanimAnimator>();
        KCC = gameObject.GetOrAddComponent<SimpleKCC>();
        Anim = gameObject.GetOrAddComponent<Animator>();

        CreatureStat = gameObject.GetOrAddComponent<CreatureStat>();

        if (Camera.main != null)
        {
            Camera.main.GetComponent<CameraController>().Player = transform;
        }
    }

    [Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.StateAuthority)]
    public virtual void Rpc_SetInfo(int templateID)
    {
        Transform.position = Vector3.zero;
        DataId = templateID;

        if (CreatureType == Define.CreatureType.Crew)
        {
            Managers.ObjectMng.Crews[NetworkObject.Id] = this as Crew;
            CreatureData = Managers.DataMng.CrewDataDict[templateID];
        }
        else
        {
            Managers.ObjectMng.Aliens[NetworkObject.Id] = this as Alien;
            CreatureData = Managers.DataMng.AlienDataDict[templateID];
        }

        gameObject.name = $"{CreatureData.DataId}_{CreatureData.Name}";

        CreatureState = Define.CreatureState.Idle;
    }

    public override void FixedUpdateNetwork()
    {
        if (HasStateAuthority == false)
        {
            return;
        }
        UpdateByState();
    }

    #region Animation
    protected virtual void UpdateAnimation()
    {
        switch (CreatureState)
        {
            case Define.CreatureState.Idle:
                PlayAnimationIdle();
                break;
            case Define.CreatureState.Move:
                PlayAnimationWalk();
                break;
            case Define.CreatureState.Use:
                //NetworkAnim.SetTrigger();
                break;
            case Define.CreatureState.Dead:
                //NetworkAnim.SetTrigger();
                break;
        }
    }
    public virtual void PlayAnimationIdle()
    {
    }
    public virtual void PlayAnimationWalk()
    {
    }

    #endregion

    #region Input
    protected virtual void HandleKeyDown()
    {
        _hAxis = Input.GetAxisRaw("Horizontal");
        _vAxis = Input.GetAxisRaw("Vertical");

        if (_hAxis == 0 && _vAxis == 0)
        {
            CreatureState = Define.CreatureState.Idle;
        }

        if (_hAxis != 0 || _vAxis != 0)
        {
            CreatureState = Define.CreatureState.Move;
        }
    }
    #endregion

    #region Update

    protected void UpdateByState()
    {
        switch (CreatureState)
        {
            case Define.CreatureState.Idle:
                UpdateIdle();
                break;
            case Define.CreatureState.Move:
                UpdateMove();
                break;
            case Define.CreatureState.Use:
                UpdateUse();
                break;
            case Define.CreatureState.Dead:
                UpdateDead();
                break;
        }
    }
    
    #region Update

    protected virtual void UpdateIdle()
    {
    }

    protected virtual void UpdateMove()
    {
        Quaternion cameraRotationY = Quaternion.Euler(0, Camera.transform.rotation.eulerAngles.y, 0);

        Vector3 velocity = cameraRotationY * new Vector3(_hAxis, 0, _vAxis) * Runner.DeltaTime * 50f; //CreatureStat.Speed;

        Velocity = velocity;
    }

    protected virtual void UpdateUse()
    {
    }

    protected virtual void UpdateDead()
    {
    }
    #endregion

    #region Event
    public void OnMove(Vector3 vector)
    {
        Velocity = vector.normalized;
        CreatureState = Define.CreatureState.Move;
    }
    #endregion
}
