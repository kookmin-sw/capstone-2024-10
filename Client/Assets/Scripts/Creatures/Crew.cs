using System.Collections;
using UnityEngine;
using Data;
using Fusion;
using UnityEngine.EventSystems;
using System.Threading.Tasks;
using DG.Tweening;

public class Crew : Creature
{
    #region Field

    public bool IsGameScene { get; set; }

    public CrewData CrewData => CreatureData as CrewData;
    public CrewStat CrewStat => (CrewStat)BaseStat;
    public override Define.CreaturePose CreaturePose
    {
        get => _creaturePose;
        set
        {
            if (_creaturePose == value || BaseSoundController == null)
                return;

            _creaturePose = value;

            //MoveCameraByPose();

            if (CreatureState == Define.CreatureState.Move)
                BaseSoundController.PlayMove();
        }
    }
    public CrewAnimController CrewAnimController => (CrewAnimController)BaseAnimController;
    public CrewSoundController CrewSoundController => (CrewSoundController)BaseSoundController;
    public Inventory Inventory { get; protected set; }

    public UI_CrewIngame CrewIngameUI => IngameUI as UI_CrewIngame;

    public GameObject RightHand { get; protected set; }
    public GameObject LeftHand { get; protected set; }

    private Tweener _onDamagedTweener;
    private Tween _damagedTween;

    #endregion

    protected override void Init()
    {
        base.Init();

        Inventory = gameObject.GetComponent<Inventory>();

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

        if (HasStateAuthority &&
            Managers.SceneMng.CurrentScene.IsSceneType((int)Define.SceneType.GameScene |
            (int)Define.SceneType.ReadyScene | (int)Define.SceneType.TutorialScene))
        {
            StartCoroutine(Managers.SceneMng.CurrentScene.OnPlayerSpawn());
        }
    }

    #region Update

    protected override void OnUpdate()
    {
        if (!CreatureCamera || !HasStateAuthority || CreatureState == Define.CreatureState.Dead || !IsSpawned)
            return;

        base.OnUpdate();

        UpdateStat();
        MoveCameraByPose();
    }

    protected override void HandleInput()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        base.HandleInput();

        if (EventSystem.current.IsPointerOverGameObject())
            return;

        if (CreatureState == Define.CreatureState.Damaged || CreatureState == Define.CreatureState.Dead)
            return;

        if (Managers.SceneMng.IsTestScene && TestInputs())
            return;

        if (CreatureState == Define.CreatureState.Interact || CreatureState == Define.CreatureState.Use)
        {
            if (Input.GetKeyDown(KeyCode.F))
                ReturnToIdle(0f);
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
            {
                CreaturePose = Define.CreaturePose.Sit;
            }
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
                CrewStat.ChangeStamina(-CrewStat.RunUseStamina * Time.deltaTime);
        }
        else
            CrewStat.ChangeStamina(CrewStat.PassiveRecoverStamina * Time.deltaTime);

        if (CrewStat.IsUnderErosion)
            CrewStat.ChangeSanity(-CrewStat.ErosionReduceSanity * Time.deltaTime);
        else if (CreaturePose == Define.CreaturePose.Sit)
            CrewStat.ChangeSanity(CrewStat.SitRecoverSanity * Time.deltaTime);

        if (CreaturePose == Define.CreaturePose.Sit)
            CrewStat.ChangeStamina(CrewStat.PassiveRecoverStamina * Time.deltaTime);
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

        if (CrewStat.DamagedBoost)
            CrewStat.Speed *= 1.5f;

        KCC.SetLookRotation(0, CreatureCamera.transform.rotation.eulerAngles.y);

        KCC.Move(Velocity * (CrewStat.Speed * Runner.DeltaTime));
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

        CreatureState = Define.CreatureState.Damaged;
        CrewAnimController.PlayAnim(Define.CrewActionType.Damaged);
        CrewSoundController.PlaySound(Define.CrewActionType.Damaged);
        ReturnToIdle(0.5f);

        _damagedTween.Kill();
        _damagedTween = DOVirtual.DelayedCall(0.3f, () =>
        {
            CrewSoundController.PlaySound(Define.CrewActionType.Exhaust);
        });
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void Rpc_OnSanityDamaged(float value)
    {
        if (!HasStateAuthority || CreatureState == Define.CreatureState.Dead || !IsSpawned)
            return;

        CrewStat.ChangeSanity(-value);
    }

    public async void OnDefeat()
    {
        if (!HasStateAuthority || CreatureState == Define.CreatureState.Dead || !IsSpawned)
            return;

        CreatureState = Define.CreatureState.Dead;
        Managers.GameMng.GameResult = Define.GameResultType.CrewDefeat;

        CrewAnimController.PlayAnim(Define.CrewActionType.Dead);
        CrewSoundController.StopAllSound();
        CrewSoundController.PlaySound(Define.CrewActionType.Dead);

        CrewIngameUI.HideUi();
        Managers.UIMng.ClosePanelUI<UI_CameraPanel>();
        Managers.GameMng.GameEndSystem.EndCrewGame(false);

        Rpc_DisableSelf(12f);
        StartCoroutine(Inventory.DropAllItems());

        await Task.Delay(8000);

        Managers.SceneMng.LoadScene(Define.SceneType.EndingScene);
    }

    public virtual async void OnWin()
    {
        if (!HasStateAuthority || CreatureType != Define.CreatureType.Crew)
            return;

        CreatureState = Define.CreatureState.Idle;
        Managers.GameMng.GameResult = Define.GameResultType.CrewWin;

        CrewSoundController.StopAllSound();
        CrewSoundController.PlaySound(Define.CrewActionType.GameEnd);

        Managers.UIMng.ClosePanelUI<UI_CameraPanel>();
        Managers.GameMng.GameEndSystem.EndCrewGame(true);

        while (CrewIngameUI == null)
        {
            await Task.Delay(500);
        }

        CrewIngameUI.HideUi();
        //CrewIngameUI.EndGame();

        Rpc_DisableSelf();
        Managers.SceneMng.LoadScene(Define.SceneType.EndingScene);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void Rpc_DisableSelf(float time = 0f)
    {
        Collider.enabled = false;
        KCCCollider.enabled = false;

        DOVirtual.DelayedCall(time, () =>
        {
            gameObject.SetActive(false);
        });
    }

    #endregion

    protected void MoveCameraByPose()
    {
        Transform cameraTransform = CreatureCamera.transform;
        float to = 0.2f;

        if (CreaturePose == Define.CreaturePose.Sit || CreaturePose == Define.CreaturePose.Run)
            to = 0.08f;

        DOVirtual.Float(cameraTransform.localPosition.y, to, 0.4f, value =>
        {
            Vector3 pos = cameraTransform.localPosition;
            pos.y = value;
            cameraTransform.localPosition = pos;
        });
    }

    protected override bool TestInputs()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            Rpc_OnDamaged(1);
            return true;
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            CrewStat.ChangeHp(1);
            return true;
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            CrewStat.ChangeSanity(-10f);
            return true;
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            CrewStat.ChangeSanity(10f);
            return true;
        }
        if (Input.GetKeyDown(KeyCode.H))
        {
            Managers.GameMng.RenderingSystem.ApplyErosionEffect(true);
            return true;
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            Managers.GameMng.RenderingSystem.ApplyErosionEffect(false);
            return true;
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            Rpc_ApplyBlind(3.5f, 1f);
            return true;
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            IsGameScene = true;
        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            OnWin();
            return true;
        }

        return false;
    }
}
