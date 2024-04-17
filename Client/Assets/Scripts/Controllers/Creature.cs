using UnityEngine;
using Fusion;
using Fusion.Addons.SimpleKCC;
using DG.Tweening;
using Data;

public abstract class Creature : NetworkBehaviour
{
    #region Field

    public bool IsSpawned { get; protected set; }

    public int DataId { get; set; }
    public CreatureData CreatureData { get; protected set; }
    [Networked] public Define.CreatureType CreatureType { get; set; }

    public Define.CreatureState CreatureState { get; set; }
    public Define.CreaturePose CreaturePose { get; set; }

    public Vector3 Direction { get; set; }
    public Vector3 Velocity { get; set; }

    public Transform Transform { get; protected set; }
    public CapsuleCollider Collider { get; protected set; }
    public Rigidbody RigidBody { get; protected set; }
    public NetworkObject NetworkObject { get; protected set; }
    public SimpleKCC KCC { get; protected set; }
    public BaseStat BaseStat { get; protected set; }
    public BaseAnimController BaseAnimController { get; protected set; }

    public UI_Ingame IngameUI { get; set; }

    public GameObject Head { get; protected set; }
    public CreatureCamera CreatureCamera { get; protected set; }
    public WatchingCamera WatchingCamera { get; protected set; }

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

        BaseStat = gameObject.GetComponent<BaseStat>();
        BaseAnimController = gameObject.GetComponent<BaseAnimController>();
    }

    public virtual void SetInfo(int templateID)
    {
        if (!HasStateAuthority)
            return;

        DataId = templateID;

        CreatureState = Define.CreatureState.Idle;
        CreaturePose = Define.CreaturePose.Stand;

        Managers.ObjectMng.MyCreature = this;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Rpc_SetInfo(templateID);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void Rpc_SetInfo(int templateID)
    {
        if (CreatureType == Define.CreatureType.Crew)
        {
            Transform.parent = Managers.ObjectMng.CrewRoot;
            CreatureData = Managers.DataMng.CrewDataDict[templateID];
        }
        else
        {
            Transform.parent = Managers.ObjectMng.AlienRoot;
            CreatureData = Managers.DataMng.AlienDataDict[templateID];
        }

        gameObject.name = $"{CreatureData.DataId}_{CreatureData.Name}";
    }

    private void Update()
    {
        if (!HasStateAuthority || CreatureState == Define.CreatureState.Dead || !IsSpawned)
            return;

        HandleInput();
    }

    private void LateUpdate()
    {
        if (!HasStateAuthority || CreatureState == Define.CreatureState.Dead || !IsSpawned)
            return;

        CreatureCamera.UpdateCameraAngle();
    }

    public override void FixedUpdateNetwork()
    {
        if (!HasStateAuthority || CreatureState == Define.CreatureState.Dead)
            return;

        UpdateByState();
        BaseAnimController.UpdateAnimation();
    }

    protected virtual void HandleInput()
    {
        if (CreatureCamera == null)
            return;

        Quaternion cameraRotationY = Quaternion.Euler(0, CreatureCamera.transform.rotation.eulerAngles.y, 0);
        Direction = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized;
        Velocity = cameraRotationY * Direction * (BaseStat.Speed * Runner.DeltaTime);
    }

    protected bool CheckInteractable(bool tryInteract)
    {
        if (!HasStateAuthority || CreatureState == Define.CreatureState.Dead || IngameUI == null || CreatureState == Define.CreatureState.Interact)
            return false;

        Ray ray = CreatureCamera.GetComponent<Camera>().ViewportPointToRay(Vector3.one * 0.5f);

        Debug.DrawRay(ray.origin, ray.direction * 2f, Color.red);

        if (Physics.Raycast(ray, out RaycastHit rayHit, maxDistance: 2f, layerMask: LayerMask.GetMask("MapObject")))
        {
            if (rayHit.transform.gameObject.TryGetComponent(out IInteractable interactable))
            {
                if (interactable.CheckInteractable(this) && tryInteract)
                {
                    return interactable.Interact(this);
                }

                return false;
            }
        }

        IngameUI.InteractInfoUI.Hide();
        IngameUI.ErrorTextUI.Hide();
        return false;
    }

    public void ReturnToIdle(float time)
    {
        DOVirtual.DelayedCall(time, () =>
        {
            CreatureState = Define.CreatureState.Idle;
            CreaturePose = Define.CreaturePose.Stand;
        });
    }

    protected abstract void PlayEffectMusic();

    protected abstract void StopEffectMusic();

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
        }
    }

    protected abstract void UpdateIdle();

    protected abstract void UpdateMove();

    #endregion
}
