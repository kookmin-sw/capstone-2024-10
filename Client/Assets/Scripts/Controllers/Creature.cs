using Fusion;
using Fusion.Addons.SimpleKCC;
using UnityEngine;

public abstract class Creature : NetworkBehaviour
{
    #region Field
    public Camera Camera => Camera.main;

    public Transform Transform { get; protected set; }
    public CircleCollider2D Collider { get; protected set; }
    public Rigidbody2D RigidBody { get; protected set; }
    public NetworkObject NetworkObject { get; protected set; }
    public NetworkMecanimAnimator NetworkAnim { get; protected set; }
    public SimpleKCC KCC { get; protected set; }
    public CreatureStat CreatureStat { get; protected set; }

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

        CreatureStat = gameObject.GetOrAddComponent<CreatureStat>();
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

        HandleKeyDown();
        UpdateByState();
    }

    #region Animation
    protected virtual void UpdateAnimation()
    {
        switch (CreatureState)
        {
            case Define.CreatureState.Idle:
                //NetworkAnim.SetTrigger();
                break;
            case Define.CreatureState.Move:
                //NetworkAnim.SetTrigger();
                break;
            case Define.CreatureState.Use:
                //NetworkAnim.SetTrigger();
                break;
            case Define.CreatureState.Dead:
                //NetworkAnim.SetTrigger();
                break;
        }
    }
    #endregion

    #region Input

    protected abstract void HandleKeyDown();

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

    protected virtual void UpdateIdle()
    {
    }

    protected virtual void UpdateMove()
    {
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
