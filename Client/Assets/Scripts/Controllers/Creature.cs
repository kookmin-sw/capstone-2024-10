using UnityEngine;
using Fusion;
using Fusion.Addons.SimpleKCC;

public abstract class Creature : NetworkBehaviour
{
    #region Field

    public Camera Camera => Camera.main;
    public Transform Transform { get; protected set; }
    public CircleCollider2D Collider { get; protected set; }
    public Rigidbody2D RigidBody { get; protected set; }
    public NetworkObject NetworkObject { get; protected set; }
    public SimpleKCC KCC { get; protected set; }
    public CreatureStat CreatureStat { get; protected set; }
    public AnimController AnimController { get; protected set; }

    [Networked] public int DataId { get; protected set; }
    public Data.CreatureData CreatureData { get; protected set; }
    [Networked] public Define.CreatureType CreatureType { get; set; }

    [Networked] public Define.CreatureState CreatureState { get; set; }
    [Networked] public Define.CreaturePose CreaturePose { get; set; }

    [Networked] public Vector3 Velocity { get; set; }

    #endregion

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
        KCC = gameObject.GetOrAddComponent<SimpleKCC>();

        CreatureStat = gameObject.GetOrAddComponent<CreatureStat>();
        AnimController = gameObject.GetOrAddComponent<AnimController>();

        if (Camera.main != null)
        {
            Camera.main.GetComponent<CreatureCamera>().Creature = this;
        }
    }

    //[Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.StateAuthority)]
    public virtual void Rpc_SetInfo(int templateID)
    {
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
        UpdateByState();
        AnimController.UpdateAnimation();
    }

    #region Input

    protected abstract void HandleKeyDown();

    #endregion

    #region Update

    protected void UpdateByState()
    {
        if (HasStateAuthority == false)
        {
            return;
        }

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

    protected abstract void UpdateIdle();

    protected abstract void UpdateMove();

    protected abstract void UpdateUse();

    protected abstract void UpdateDead();

    #endregion
}
