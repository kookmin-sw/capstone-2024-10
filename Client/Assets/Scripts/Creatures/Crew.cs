using UnityEngine;
using Data;
using Fusion;

public class Crew : Creature
{
    #region Field

    public bool IsGameScene { get; set; }

    public CrewData CrewData => CreatureData as CrewData;
    public CrewStat CrewStat => (CrewStat)BaseStat;
    public CrewAnimController CrewAnimController => (CrewAnimController)BaseAnimController;
    public CrewSoundController CrewSoundController => (CrewSoundController)BaseSoundController;
    public Inventory Inventory { get; protected set; }

    public UI_CrewIngame CrewIngameUI => IngameUI as UI_CrewIngame;

    public GameObject RightHand { get; protected set; }
    public GameObject LeftHand { get; protected set; }

    #endregion

    protected override void Init()
    {
        base.Init();

        Inventory = gameObject.GetComponent<Inventory>();
        AudioSource = gameObject.GetComponent<AudioSource>();

        Head = Util.FindChild(gameObject, "head.x", true);
        RightHand = Util.FindChild(gameObject, "c_middle1.r", true);
        LeftHand = Util.FindChild(gameObject, "c_middle1.l", true);
    }

    public void SetInfo(int templateID, bool isGameScene)
    {
        IsGameScene = isGameScene;

        CreatureType = Define.CreatureType.Crew;

        base.SetInfo(templateID);

        Head.transform.localScale = Vector3.zero;

        CreatureCamera = Managers.ResourceMng.Instantiate("Cameras/CreatureCamera", Util.FindChild(gameObject, "neck.x", true).transform).GetComponent<CreatureCamera>();
        CreatureCamera.transform.localPosition = new Vector3(0f, 0.2f, 0f);
        CreatureCamera.SetInfo(this);

        CrewStat.SetStat(CrewData);
        Inventory.SetInfo();

        IsSpawned = true;

        if (HasStateAuthority && Managers.SceneMng.CurrentScene.IsSceneType((int)Define.SceneType.GameScene | (int)Define.SceneType.ReadyScene))
        {
            StartCoroutine(Managers.SceneMng.CurrentScene.OnPlayerSpawn());
        }
    }

    #region Update

    protected override void OnUpdate()
    {
        base.OnUpdate();

        UpdateStat();
    }

    protected override void HandleInput()
    {
        base.HandleInput();

        if (CreatureState == Define.CreatureState.Damaged || CreatureState == Define.CreatureState.Dead)
            return;

        /////////////////////////////////
        // TODO - TEST CODE
        if (Input.GetKeyDown(KeyCode.U))
        {
            Rpc_OnDamaged(1);
            return;
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            CrewStat.ChangeHp(1);
            return;
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            CrewStat.ChangeSanity(-10f);
            return;
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            CrewStat.ChangeSanity(10f);
            return;
        }
        if (Input.GetKeyDown(KeyCode.H))
        {
            Managers.GameMng.RenderingSystem.ApplyErosion(true);
            return;
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            Managers.GameMng.RenderingSystem.ApplyErosion(false);
            return;
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            Rpc_OnBlind(2f, 3f);
            return;
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            IsGameScene = true;
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            OnWin();
            return;
        }
        /////////////////////////////////

        if (CreatureState == Define.CreatureState.Interact || CreatureState == Define.CreatureState.Use)
        {
            if (Input.GetKeyDown(KeyCode.F))
                CreatureState = Define.CreatureState.Idle;
            return;
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            if (CheckInteractable(true))
                return;
        }
        else
        {
            CheckInteractable(false);
        }

        BaseSoundController.CheckChasing();

        if (Input.GetKeyDown(KeyCode.G))
            if (Inventory.DropItem())
                return;

        if (Input.GetMouseButtonDown(0))
            if (CheckAndUseItem())
                return;

        if (Input.GetKeyDown(KeyCode.Alpha1))
            Inventory.ChangeItem(0);
        if (Input.GetKeyDown(KeyCode.Alpha2))
            Inventory.ChangeItem(1);
        if (Input.GetKeyDown(KeyCode.Alpha3))
            Inventory.ChangeItem(2);
        if (Input.GetKeyDown(KeyCode.Alpha4))
            Inventory.ChangeItem(3);

        if (Input.GetAxis("Mouse ScrollWheel") > 0)
            Inventory.ChangeItem(Mathf.Clamp(Inventory.CurrentItemIdx - 1, 0, 3));

        if (Input.GetAxis("Mouse ScrollWheel") < 0)
            Inventory.ChangeItem(Mathf.Clamp(Inventory.CurrentItemIdx + 1, 0, 3));

        if (Input.GetKeyDown(KeyCode.C))
        {
            if (CreaturePose != Define.CreaturePose.Sit)
                CreaturePose = Define.CreaturePose.Sit;
            else
                CreaturePose = Define.CreaturePose.Stand;
            return;
        }

        if (Velocity == Vector3.zero)
            CreatureState = Define.CreatureState.Idle;
        else
        {
            CreatureState = Define.CreatureState.Move;

            if (Input.GetKey(KeyCode.LeftShift) && Direction.z > 0)
            {
                if (CreaturePose != Define.CreaturePose.Sit && CrewStat.IsRunnable)
                    CreaturePose = Define.CreaturePose.Run;
                if (!CrewStat.IsRunnable)
                    CreaturePose = Define.CreaturePose.Stand;
            }
            else
                if (CreaturePose == Define.CreaturePose.Run)
                CreaturePose = Define.CreaturePose.Stand;
        }

    }

    protected void UpdateStat()
    {
        if (!IsGameScene)
            return;

        if (CreatureState == Define.CreatureState.Move && CreaturePose == Define.CreaturePose.Run)
        {
            if (!CrewStat.Doped)
                CrewStat.ChangeStamina(-Define.RUN_USE_STAMINA * Runner.DeltaTime);
        }
        else
            CrewStat.ChangeStamina(Define.PASIVE_RECOVER_STAMINA * Runner.DeltaTime);

        if (CreatureState == Define.CreatureState.Idle && CreaturePose == Define.CreaturePose.Sit)
        {
            CrewStat.ChangeStamina(Define.PASIVE_RECOVER_STAMINA * Runner.DeltaTime);
            CrewStat.ChangeSanity(Define.SIT_RECOVER_SANITY * Runner.DeltaTime);
        }
        else if (CrewStat.Erosion)
            CrewStat.ChangeSanity(-Define.EROSION_REDUCE_SANITY * Runner.DeltaTime);
    }

    protected override void UpdateIdle()
    {
        switch (CreaturePose)
        {
            case Define.CreaturePose.Stand:
                break;
            case Define.CreaturePose.Sit:
                break;
            case Define.CreaturePose.Run:
                CreaturePose = Define.CreaturePose.Stand;
                break;
        }

        KCC.SetLookRotation(0, CreatureCamera.transform.rotation.eulerAngles.y);
    }

    protected override void UpdateMove()
    {
        switch (CreaturePose)
        {
            case Define.CreaturePose.Stand:
                CrewStat.Speed = CrewStat.WalkSpeed;
                break;
            case Define.CreaturePose.Sit:
                CrewStat.Speed = CrewStat.SitSpeed;
                break;
            case Define.CreaturePose.Run:
                CrewStat.Speed = CrewStat.RunSpeed;
                break;
        }

        KCC.SetLookRotation(0, CreatureCamera.transform.rotation.eulerAngles.y);

        KCC.Move(Velocity * (CrewStat.Speed * Runner.DeltaTime), 0f);
    }

    protected bool CheckAndUseItem()
    {
        if (!HasStateAuthority || CreatureState == Define.CreatureState.Dead || !IsSpawned)
            return false;

        if (Inventory.CurrentItem == null)
        {
            Debug.Log("No Item");
            return false;
        }

        return Inventory.CheckAndUseItem();
    }

    #endregion

    #region Event

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void Rpc_OnDamaged(int value)
    {
        if (!HasStateAuthority || CreatureState == Define.CreatureState.Dead || !IsSpawned)
            return;

        CrewStat.ChangeHp(-value);

        if (CrewStat.Hp <= 0)
        {
            OnDefeat();
            return;
        }

        CrewStat.ChangeStamina(Define.DAMAGED_RECOVER_STAMINA);

        CreatureState = Define.CreatureState.Damaged;
        CrewAnimController.PlayAnim(Define.CrewActionType.Damaged);
        CrewSoundController.PlaySound(Define.CrewActionType.Damaged);
        ReturnToIdle(0.5f);
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void Rpc_OnSanityDamaged(float value)
    {
        if (!HasStateAuthority || CreatureState == Define.CreatureState.Dead || !IsSpawned)
            return;

        CrewStat.ChangeSanity(-value);
    }

    public void OnDefeat()
    {
        CreatureState = Define.CreatureState.Dead;

        CrewAnimController.PlayAnim(Define.CrewActionType.Dead);
        CrewSoundController.StopAllSound();
        CrewSoundController.PlaySound(Define.CrewActionType.Dead);

        CrewIngameUI.HideUI();
        Managers.GameMng.GameEndSystem.EndCrewGame(false);

        Rpc_OnDefeat();
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void Rpc_OnDefeat()
    {
        Collider.enabled = false;
    }

    public void OnWin()
    {
        CreatureState = Define.CreatureState.Idle;

        CrewSoundController.StopAllSound();
        CrewSoundController.PlayEndGame();

        CrewIngameUI.HideUI();
        Managers.GameMng.GameEndSystem.EndCrewGame(true);
        CrewIngameUI.EndGame();

        Rpc_OnWin();
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void Rpc_OnWin()
    {
        gameObject.SetActive(false);
    }

    #endregion
}
