using System.Collections.Generic;
using UnityEngine;
using Data;
using Fusion;
using UnityEngine.EventSystems;
using System.Collections;

public class Alien : Creature
{
    #region Field

    public AlienData AlienData => CreatureData as AlienData;
    public AlienStat AlienStat => (AlienStat)BaseStat;
    public AlienAnimController AlienAnimController => (AlienAnimController)BaseAnimController;
    public AlienSoundController AlienSoundController => (AlienSoundController)BaseSoundController;
    public SkillController SkillController { get; protected set; }

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

        if (TestInputs())
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

    public IEnumerator OnGameEnd()
    {
        if (!HasStateAuthority || !IsSpawned)
            yield break;

        yield return new WaitUntil(() => AlienSoundController != null);

        AlienSoundController.StopAllSound();
        AlienSoundController.PlayEndGame();

        yield return new WaitUntil(() => AlienIngameUI != null);

        AlienIngameUI.HideUI();
        AlienIngameUI.EndGame();

        Rpc_OnGameEnd();
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void Rpc_OnGameEnd()
    {
        gameObject.SetActive(false);
    }

    #endregion

    protected override bool TestInputs()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            Rpc_ApplyBlind(2f, 3f);
            return true;
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            Managers.GameMng.GameEndSystem.Rpc_EndCrewGame(false);
            return true;
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            Managers.GameMng.GameEndSystem.Rpc_EndCrewGame(true);
            return true;
        }

        return false;
    }
}
