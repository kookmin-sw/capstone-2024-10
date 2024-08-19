using System.Collections.Generic;
using UnityEngine;
using Data;
using Fusion;
using UnityEngine.EventSystems;
using System.Threading.Tasks;

public class Alien : Creature
{
    #region Field

    public AlienData AlienData => CreatureData as AlienData;
    public AlienStat AlienStat => (AlienStat)BaseStat;
    public AlienAnimController AlienAnimController => (AlienAnimController)BaseAnimController;
    public AlienSoundController AlienSoundController => (AlienSoundController)BaseSoundController;
    public SkillController SkillController { get; protected set; }

    public GameObject RoarRangeIndicator { get; protected set; }

    public UI_AlienIngame AlienIngameUI => IngameUI as UI_AlienIngame;

    public float CurrentSkillRange { get; set; }

    #endregion

    protected override void Init()
    {
        base.Init();

        SkillController = gameObject.GetComponent<SkillController>();
        SkillController.Skills = new Dictionary<int, BaseSkill>(Define.MAX_SKILL_NUM);
        SkillController.Skills[0] = gameObject.GetComponent<BasicAttack>();
        SkillController.Skills[1] = gameObject.GetComponent<Roar>();
        SkillController.Skills[2] = gameObject.GetComponent<CursedHowl>();
        SkillController.Skills[3] = gameObject.GetComponent<LeapAttack>();

        Head = Util.FindChild(gameObject, "Anglerox_ Head", true);
    }

    public override void SetInfo(int templateID)
    {
        CreatureType = Define.CreatureType.Alien;

        base.SetInfo(templateID);

        CreatureCamera = Managers.ResourceMng.Instantiate("Cameras/CreatureCamera", Util.FindChild(gameObject, "Anglerox_ Neck", true).transform).GetComponent<CreatureCamera>();
        CreatureCamera.transform.localPosition = new Vector3(0.1f, 0.6f, 0f);
        CreatureCamera.SetInfo(this);

        RoarRangeIndicator = Util.FindChild(gameObject, "RoarRangeIndicator");
        RoarRangeIndicator.transform.localScale = new Vector3(0f, 0f, 0f);

        AlienStat.SetStat(AlienData);

        SkillController.Skills[0].SetInfo(Define.SKILL_BASIC_ATTACK_ID);
        SkillController.Skills[1].SetInfo(Define.SKILL_ROAR_ID);
        SkillController.Skills[2].SetInfo(Define.SKILL_CURSED_HOWL_ID);
        SkillController.Skills[3].SetInfo(Define.SKILL_LEAP_ATTACK_ID);

        CurrentSkillRange = SkillController.Skills[0].SkillData.Range;

        if (Managers.SceneMng.CurrentScene.IsSceneType((int)Define.SceneType.GameScene))
        {
            StartCoroutine(Managers.SceneMng.CurrentScene.OnPlayerSpawn());
        }

        Managers.GameMng.RenderingSystem.SetAlienOutlinePassVolume();
        IsSpawned = true;
    }

    #region Update

    protected override void OnLateUpdate()
    {
        if (!CreatureCamera || !HasStateAuthority || CreatureState == Define.CreatureState.Dead || !IsSpawned)
            return;

        base.OnLateUpdate();

        if (HasStateAuthority)
            Head.transform.localScale = Vector3.zero;
    }

    protected override void HandleInput()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        base.HandleInput();

        if (EventSystem.current.IsPointerOverGameObject())
            return;

        if (CreatureState == Define.CreatureState.Damaged || CreatureState == Define.CreatureState.Interact || CreatureState == Define.CreatureState.Use)
            return;

        if (Managers.SceneMng.IsTestScene && TestInputs())
            return;

        if (Input.GetKeyDown(KeyCode.F))
        {
            if (CheckInteractable(true))
                return;
        }
        else
        {
            CheckInteractable(false);
        }

        if (Input.GetMouseButtonDown(0))
            if (CheckAndUseSkill(0))
                return;

        if (Input.GetKeyDown(KeyCode.Q))
            if (CheckAndUseSkill(1))
                return;

        if (Input.GetKeyDown(KeyCode.E))
            if (CheckAndUseSkill(2))
                return;

        if (Input.GetKeyDown(KeyCode.R))
            if (CheckAndUseSkill(3))
                return;

        if (Velocity == Vector3.zero)
            CreatureState = Define.CreatureState.Idle;
        else
        {
            CreatureState = Define.CreatureState.Move;
        }
    }

    protected override void UpdateIdle()
    {
        KCC.SetLookRotation(0, CreatureCamera.transform.rotation.eulerAngles.y);
    }

    protected override void UpdateMove()
    {
        switch (CreaturePose)
        {
            case Define.CreaturePose.Stand:
                AlienStat.Speed = AlienStat.WalkSpeed;
                break;
        }

        KCC.SetLookRotation(0, CreatureCamera.transform.rotation.eulerAngles.y);

        KCC.Move(Velocity * (AlienStat.Speed * Runner.DeltaTime));
    }

    protected bool CheckAndUseSkill(int skillIdx)
    {
        if (!HasStateAuthority || !IsSpawned)
            return false;

        return SkillController.CheckAndUseSkill(skillIdx);
    }

    #endregion

    #region Event

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public override void Rpc_ApplyBlind(float blindTime, float backTime)
    {
        base.Rpc_ApplyBlind(blindTime, backTime);

        CreatureState = Define.CreatureState.Damaged;
        AlienAnimController.PlayAnim(Define.AlienActionType.GetBlind);
        AlienSoundController.PlaySound(Define.AlienActionType.GetBlind);
        ReturnToIdle(blindTime);
    }

    public async void OnGameEnd()
    {
        if (!HasStateAuthority || !IsSpawned)
            return;

        AlienSoundController.StopAllSound();
        AlienSoundController.PlaySound(Define.AlienActionType.GameEnd);

        while (AlienIngameUI == null)
        {
            await Task.Delay(500);
        }

        Managers.UIMng.ClosePanelUI<UI_CameraPanel>();
        AlienIngameUI.HideUi();
        //AlienIngameUI.EndGame();

        Rpc_DisableSelf();
        Managers.SceneMng.LoadScene(Define.SceneType.EndingScene);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void Rpc_DisableSelf()
    {
        gameObject.SetActive(false);
    }

    #endregion

    protected override bool TestInputs()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            Rpc_ApplyBlind(3.5f, 1f);
            return true;
        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            Managers.GameMng.GameEndSystem.KilledCrewNum = Define.PLAYER_COUNT - 1;
            Managers.GameMng.GameEndSystem.EndGame();
            return true;
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            Managers.GameMng.GameEndSystem.EndGame();
            return true;
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            Debug.Log(BaseStat.Speed);
            return true;
        }

        return false;
    }
}
