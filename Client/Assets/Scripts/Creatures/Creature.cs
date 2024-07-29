using UnityEngine;
using Fusion;
using Fusion.Addons.SimpleKCC;
using DG.Tweening;
using Data;
using UnityEngine.EventSystems;

public abstract class Creature : NetworkBehaviour
{
    #region Field

    public bool IsSpawned { get; protected set; } = false;

    public int DataId { get; set; }
    public CreatureData CreatureData { get; protected set; }
    [Networked] public Define.CreatureType CreatureType { get; set; }

    protected Define.CreatureState _creatureState;
    protected Define.CreaturePose _creaturePose;
    protected Define.SectorName _currentSector;

    public Define.CreatureState CreatureState
    {
        get => _creatureState;
        set
        {
            if (_creatureState == value || BaseSoundController == null)
                return;

            _creatureState = value;

            if (value == Define.CreatureState.Move)
                BaseSoundController.PlayMove();
            else
                BaseSoundController.Rpc_StopEffectSound();
        }
    }
    public virtual Define.CreaturePose CreaturePose
    {
        get => _creaturePose;
        set
        {
            if (_creaturePose == value || BaseSoundController == null)
                return;

            _creaturePose = value;

            if (CreatureState == Define.CreatureState.Move)
                BaseSoundController.PlayMove();
        }
    }
    public Define.SectorName CurrentSector
    {
        get => _currentSector;
        set
        {
            _currentSector = value;
            IngameUI?.CurrentSectorUI.SetSector(value);
        }
    }

    public Quaternion CameraRotationY { get; set; }
    public Vector3 Direction { get; set; }
    public Vector3 Velocity { get; set; }

    public Transform Transform { get; protected set; }
    public AudioSource AudioSource { get; protected set; }
    public CapsuleCollider Collider { get; protected set; }
    public CapsuleCollider KCCCollider { get; protected set; }
    public Rigidbody RigidBody { get; protected set; }
    public NetworkObject NetworkObject { get; protected set; }
    public SimpleKCC KCC { get; protected set; }
    public BaseStat BaseStat { get; protected set; }
    public BaseAnimController BaseAnimController { get; protected set; }
    public BaseSoundController BaseSoundController { get; protected set; }

    public UI_Ingame IngameUI { get; set; }

    public GameObject Head { get; protected set; }
    public CreatureCamera CreatureCamera { get; protected set; }

    #endregion

    public override void Spawned()
    {
        Init();
    }

    protected virtual void Init()
    {
        Transform = transform;
        AudioSource = gameObject.GetComponent<AudioSource>();
        Collider = gameObject.GetComponent<CapsuleCollider>();
        RigidBody = gameObject.GetComponent<Rigidbody>();
        NetworkObject = gameObject.GetComponent<NetworkObject>();
        KCC = gameObject.GetComponent<SimpleKCC>();

        KCCCollider = Util.FindChild(gameObject, "KCCCollider").GetComponent<CapsuleCollider>();

        BaseStat = gameObject.GetComponent<BaseStat>();
        BaseAnimController = gameObject.GetComponent<BaseAnimController>();
        BaseSoundController = gameObject.GetComponent<BaseSoundController>();
    }

    public virtual void SetInfo(int templateID)
    {
        if (!HasStateAuthority)
            return;

        DataId = templateID;

        CreatureState = Define.CreatureState.Idle;
        CreaturePose = Define.CreaturePose.Stand;

        Managers.ObjectMng.MyCreature = this;

        Rpc_SetInfo(templateID);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void Rpc_SetInfo(int templateID)
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

    public void ReturnToIdle(float time)
    {
        DOVirtual.DelayedCall(time, () =>
        {
            CreatureState = Define.CreatureState.Idle;
            CreaturePose = Define.CreaturePose.Stand;
        });
    }

    #region Update
    private void Update()
    {
        OnUpdate();
    }

    private void LateUpdate()
    {
        OnLateUpdate();
    }

    protected virtual void OnUpdate()
    {
        if (!CreatureCamera || !HasStateAuthority || CreatureState == Define.CreatureState.Dead || !IsSpawned || IngameUI == null)
            return;

        HandleInput();
        BaseSoundController.CheckChasing();
    }

    protected virtual void OnLateUpdate()
    {
        if (!CreatureCamera || !HasStateAuthority || CreatureState == Define.CreatureState.Dead || !IsSpawned || IngameUI == null)
            return;

        CreatureCamera.UpdateCameraAngle();
    }

    public override void FixedUpdateNetwork()
    {
        if (!HasStateAuthority || CreatureState == Define.CreatureState.Dead || !IsSpawned)
            return;

        UpdateByState();
        ApplyAnimation();
    }

    protected virtual void HandleInput()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        CameraRotationY = Quaternion.Euler(0f, CreatureCamera.transform.rotation.eulerAngles.y, 0);
        Direction = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized;
        Velocity = (CameraRotationY * Direction).normalized;

        if (Input.GetKey(KeyCode.Tab))
            IngameUI.MapUI.Show();
        else
            IngameUI.MapUI.Hide();
    }

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

    protected void ApplyAnimation()
    {
        switch (CreatureState)
        {
            case Define.CreatureState.Idle:
                BaseAnimController.PlayIdle();
                break;
            case Define.CreatureState.Move:
                BaseAnimController.PlayMove();
                break;
        }
    }

    protected bool CheckInteractable(bool tryInteract)
    {
        if (IngameUI == null || CreatureState == Define.CreatureState.Interact)
            return false;

        Ray ray = CreatureCamera.Camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, CreatureCamera.Camera.nearClipPlane));

        //Debug.DrawRay(ray.origin, ray.direction * 1.5f, Color.red);

        if (Physics.Raycast(ray, out RaycastHit rayHit, maxDistance: 1.5f, layerMask: LayerMask.GetMask("MapObject", "InteractableObject", "PlanTargetObject")))
        {
            if (rayHit.transform.gameObject.TryGetComponent(out IInteractable interactable))
            {
                if (interactable.IsInteractable(this) && tryInteract)
                {
                    return interactable.Interact(this);
                }

                return false;
            }
        }

        IngameUI.InteractInfoUI.Hide();
        IngameUI.ErrorTextUI.Hide();
        IngameUI.ObjectNameUI.Hide();

        return false;
    }

    #endregion

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void Rpc_ApplyErosion(bool isApplying)
    {
        BaseStat.IsUnderErosion = isApplying;
        Managers.GameMng.RenderingSystem.ApplyErosionEffect(isApplying);
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public virtual void Rpc_ApplyBlind(float blindTime, float backTime)
    {
        if (!HasStateAuthority || CreatureState == Define.CreatureState.Dead || !IsSpawned)
            return;

        Managers.GameMng.RenderingSystem.ApplyBlindEffect(blindTime, backTime);
    }

    protected virtual bool TestInputs()
    {
        return false;
    }
}
