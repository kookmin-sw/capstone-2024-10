using System;
using UnityEngine;
using Fusion;
using Fusion.Addons.SimpleKCC;
using DG.Tweening;
using Data;

public abstract class Creature : NetworkBehaviour
{
    public const bool IsFirstPersonView = true;
    #region Field

    public bool IsSpawned { get; protected set; } = false;
    public UI_Ingame IngameUI { get; set; }

    public GameObject Head { get; protected set; }
    public CreatureCamera CreatureCamera { get; protected set; }
    public WatchingCamera WatchingCamera { get; protected set; }

    public Transform Transform { get; protected set; }
    public CapsuleCollider Collider { get; protected set; }
    public Rigidbody RigidBody { get; protected set; }
    public NetworkObject NetworkObject { get; protected set; }
    public SimpleKCC KCC { get; protected set; }
    public BaseStat BaseStat { get; protected set; }
    public BaseAnimController BaseAnimController { get; protected set; }

    [Networked] public int DataId { get; set; }
    public CreatureData CreatureData { get; protected set; }
    [Networked] public Define.CreatureType CreatureType { get; set; }

    [Networked] public Define.CreatureState CreatureState { get; set; }
    [Networked] public Define.CreaturePose CreaturePose { get; set; }

    [Networked] public Vector3 Direction { get; set; }
    [Networked] public Vector3 Velocity { get; set; }

    public BaseWorkStation CurrentWorkStation { get; set; }

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
        DataId = templateID;

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
            CreatureData = Managers.DataMng.CrewDataDict[templateID];
        }
        else
        {
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

        if (IsFirstPersonView)
        {
            Quaternion cameraRotationY = Quaternion.Euler(0, CreatureCamera.transform.rotation.eulerAngles.y, 0);
            Direction = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized;
            Velocity = cameraRotationY * Direction * (BaseStat.Speed * Runner.DeltaTime);
        }
        else
        {
            Quaternion cameraRotationY = Quaternion.Euler(0, WatchingCamera.transform.rotation.eulerAngles.y, 0);
            Velocity = cameraRotationY * new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")) * (BaseStat.Speed * Runner.DeltaTime);
        }
    }

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
            case Define.CreatureState.Interact:
                UpdateInteract();
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

    protected abstract void UpdateInteract();
    protected abstract void UpdateUse();

    protected virtual void UpdateDead() { }

    #endregion

    protected bool CheckAndInteract(bool isOnlyCheck)
    {
        if (!HasStateAuthority || CreatureState == Define.CreatureState.Dead || IngameUI == null)
            return false;

        Ray ray = CreatureCamera.GetComponent<Camera>().ViewportPointToRay(Vector3.one * 0.5f);

        if (Physics.Raycast(ray, out RaycastHit rayHit, maxDistance:1.5f, layerMask:LayerMask.GetMask("MapObject")))
        {
            if (rayHit.transform.gameObject.TryGetComponent(out BaseInteractable interactable))
            {
                if (!isOnlyCheck && interactable.CheckAndInteract(this))
                {
                    IngameUI.InteractInfo.Hide();
                    CreatureState = Define.CreatureState.Interact;
                    CreaturePose = Define.CreaturePose.Stand;
                    CurrentWorkStation = interactable as BaseWorkStation;

                    if (CurrentWorkStation != null){
                        CurrentWorkStation.StartInteract(this);
                        IngameUI.WorkProgressBar.Show(CurrentWorkStation.WorkingDescription.ToString(), CurrentWorkStation.TotalWorkAmount);
                    }

                    Debug.Log("Interact Success");
                    Debug.DrawRay(ray.origin, ray.direction * 1.5f, Color.green, 1f);

                    return true;
                }

                IngameUI.InteractInfo.Show(interactable.InteractDescription);
            }
        }
        else
        {
            IngameUI.InteractInfo.Hide();
        }

        Debug.DrawRay(ray.origin, ray.direction * 1.5f, Color.red);

        return false;
    }

    public void InterruptInteract()
    {
        if (!HasStateAuthority || CurrentWorkStation == null || CreatureState == Define.CreatureState.Dead)
            return;

        IngameUI.WorkProgressBar.Hide();
        CreatureState = Define.CreatureState.Idle;

        CurrentWorkStation = null;
    }

    public void ReturnToIdle(float time)
    {
        DOVirtual.DelayedCall(time, () => { CreatureState = Define.CreatureState.Idle; });
    }
}
