using UnityEngine;
using Fusion;
using Fusion.Addons.SimpleKCC;

public abstract class Creature : NetworkBehaviour
{
    #region Field

    public CreatureCamera CreatureCamera { get; protected set; }
    public WatchingCamera WatchingCamera { get; protected set; }
    public Transform Transform { get; protected set; }
    public CircleCollider2D Collider { get; protected set; }
    public Rigidbody2D RigidBody { get; protected set; }
    public NetworkObject NetworkObject { get; protected set; }
    public SimpleKCC KCC { get; protected set; }
    public CreatureStat CreatureStat { get; protected set; }
    public AnimController AnimController { get; protected set; }

    [Networked] public int DataId { get; set; }
    public Data.CreatureData CreatureData { get; protected set; }
    [Networked] public Define.CreatureType CreatureType { get; set; }

    [Networked] public Define.CreatureState CreatureState { get; set; }
    [Networked] public Define.CreaturePose CreaturePose { get; set; }

    [Networked] public Quaternion CameraQuaternion { get; set; }
    [Networked] public Vector3 Velocity { get; set; }

    #endregion

    public override void Spawned()
    {
        Init();
    }

    protected virtual void Init()
    {
        Transform = gameObject.GetComponent<Transform>();
        Collider = gameObject.GetComponent<CircleCollider2D>();
        RigidBody = gameObject.GetComponent<Rigidbody2D>();
        NetworkObject = gameObject.GetComponent<NetworkObject>();
        KCC = gameObject.GetComponent<SimpleKCC>();

        CreatureStat = gameObject.GetComponent<CreatureStat>();
        AnimController = gameObject.GetComponent<AnimController>();
    }

    public virtual void SetInfo(int templateID)
    {
        DataId = templateID;

        if (CreatureType == Define.CreatureType.Crew)
        {
            CreatureData = Managers.DataMng.CrewDataDict[templateID];
        }
        else
        {
            CreatureData = Managers.DataMng.AlienDataDict[templateID];
        }

        CreatureState = Define.CreatureState.Idle;
        CreaturePose = Define.CreaturePose.Stand;

        Managers.ObjectMng.MyCreature = this;

        //CreatureCamera = Managers.ResourceMng.Instantiate("Cameras/CreatureCamera", gameObject.transform).GetComponent<CreatureCamera>();
        // CreatureCamera.enabled = true;
        // CreatureCamera.Creature = this;

        WatchingCamera = Managers.ResourceMng.Instantiate("Cameras/WatchingCamera", gameObject.transform).GetComponent<WatchingCamera>();
        WatchingCamera.enabled = true;
        WatchingCamera.Creature = this;

        Rpc_SetInfo(templateID);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void Rpc_SetInfo(int templateID)
    {
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
    }

    public override void FixedUpdateNetwork()
    {
        HandleInput();
        UpdateByState();
        AnimController.UpdateAnimation();
        CameraUpdate();
    }

    protected virtual void HandleInput()
    {
        Quaternion cameraRotationY = Quaternion.Euler(0, WatchingCamera.transform.rotation.eulerAngles.y, 0);
        Velocity = cameraRotationY * new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")) * CreatureStat.Speed * Runner.DeltaTime;
    }

    #region Update

    protected void CameraUpdate()
    {
        if (HasStateAuthority == false)
            return;

        if (CreatureCamera != null)
            CreatureCameraUpdate();
        else if (WatchingCamera != null)
            WatchingCameraUpdate();
    }

    protected void CreatureCameraUpdate()
    {
        CameraQuaternion = Quaternion.Euler(0, CreatureCamera.transform.rotation.eulerAngles.y, 0);
    }

    protected void WatchingCameraUpdate()
    {
        CameraQuaternion = Quaternion.Euler(0, WatchingCamera.transform.rotation.eulerAngles.y, 0);
    }

    protected void UpdateByState()
    {
        if (HasStateAuthority == false)
            return;

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
