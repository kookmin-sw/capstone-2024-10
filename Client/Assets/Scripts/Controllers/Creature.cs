using UnityEngine;
using Fusion;
using Fusion.Addons.SimpleKCC;

public abstract class Creature : NetworkBehaviour
{
    public const bool IsFirstPersonView = true;

    #region Field

    public CreatureCamera CreatureCamera { get; protected set; }
    public WatchingCamera WatchingCamera { get; protected set; }
    public Transform Transform { get; protected set; }
    public CapsuleCollider Collider { get; protected set; }
    public Rigidbody RigidBody { get; protected set; }
    public NetworkObject NetworkObject { get; protected set; }
    public SimpleKCC KCC { get; protected set; }
    public CreatureStat CreatureStat { get; protected set; }
    public AnimController AnimController { get; protected set; }
    public Inventory Inventory { get; protected set; }

    [Networked] public int DataId { get; set; }
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
        Transform = gameObject.GetComponent<Transform>();
        Collider = gameObject.GetComponent<CapsuleCollider>();
        RigidBody = gameObject.GetComponent<Rigidbody>();
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

        if (HasStateAuthority)
        {
            if (IsFirstPersonView)
            {

                CreatureCamera = Managers.ResourceMng.Instantiate("Cameras/CreatureCamera", gameObject.transform).GetComponent<CreatureCamera>();
                CreatureCamera.SetInfo(this);
            }
            else
            {
                WatchingCamera = Managers.ResourceMng.Instantiate("Cameras/WatchingCamera", gameObject.transform).GetComponent<WatchingCamera>();
                WatchingCamera.enabled = true;
                WatchingCamera.Creature = this;
            }
            Camera.main.GetComponent<CreatureCamera>().Creature = this;
        }

        CreatureState = Define.CreatureState.Idle;
        CreaturePose = Define.CreaturePose.Stand;

        Managers.ObjectMng.MyCreature = this;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

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

    private void Update()
    {
        HandleInput();
    }

    public override void FixedUpdateNetwork()
    {
        UpdateByState();
        AnimController.UpdateAnimation();
    }

    protected virtual void HandleInput()
    {
        if (CreatureCamera == null)
            return;

        if (IsFirstPersonView)
        {

            Quaternion cameraRotationY = Quaternion.Euler(0, CreatureCamera.transform.rotation.eulerAngles.y, 0);
            Velocity = cameraRotationY * new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")) *
                       CreatureStat.Speed * Runner.DeltaTime;
        }
        else
        {
            Quaternion cameraRotationY = Quaternion.Euler(0, WatchingCamera.transform.rotation.eulerAngles.y, 0);
            Velocity = cameraRotationY * new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")) *
                       CreatureStat.Speed * Runner.DeltaTime;
        }
    }

    #region Update

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
            case Define.CreatureState.Interact:
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

    #region Interact

    protected bool RayCast()
    {
        Ray ray = CreatureCamera.GetComponent<Camera>().ViewportPointToRay(Vector3.one * 0.5f);

        if (Physics.Raycast(ray, out RaycastHit rayHit, maxDistance: 1f, layerMask: LayerMask.GetMask("Interact")))
        {
            CreatureState = Define.CreatureState.Interact;

            IInteractable interactable = rayHit.transform.gameObject.GetComponent<IInteractable>();
            interactable.Interact(this);

            Debug.DrawLine(ray.origin, rayHit.point, Color.red, 1f); // TODO - Test Code
            return true;
        }
        else
        {
            Debug.Log("Failed to InterAct");
            Debug.DrawRay(ray.origin, ray.direction * 1f, Color.red, 1f); // TODO - Test Code
            return false;
        }
    }

    #endregion
}
