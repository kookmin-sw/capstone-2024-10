using System.Collections.Generic;
using UnityEngine;
using Data;
using Fusion;

public abstract class Alien : Creature
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

        AudioSource = gameObject.GetComponent<AudioSource>();
    }

    public override void SetInfo(int templateID)
    {
        CreatureType = Define.CreatureType.Alien;

        base.SetInfo(templateID);

        Head = Util.FindChild(gameObject, "head", true);
        Head.transform.localScale = Vector3.zero;

        CreatureCamera = Managers.ResourceMng.Instantiate("Cameras/CreatureCamera", Util.FindChild(gameObject, "Anglerox_ Neck", true).transform).GetComponent<CreatureCamera>();
        CreatureCamera.transform.localPosition = new Vector3(0.1f, 0.6f, 0f);
        CreatureCamera.SetInfo(this);

        AlienStat.SetStat(AlienData);

        CurrentSkillRange = 1.5f;
        IsSpawned = true;

        if (Managers.SceneMng.CurrentScene.IsSceneType((int)Define.SceneType.GameScene))
        {
            StartCoroutine(Managers.SceneMng.CurrentScene.OnPlayerSpawn());
        }
    }

    #region Update

    protected override void HandleInput()
    {
        base.HandleInput();

        if (CreatureState == Define.CreatureState.Damaged || CreatureState == Define.CreatureState.Interact || CreatureState == Define.CreatureState.Use)
            return;

        /////////////////////////////////
        // TODO - TEST CODE
        if (Input.GetKeyDown(KeyCode.K))
        {
            Rpc_OnBlind(2f, 3f);
            return;
        }
        /////////////////////////////////

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

        if (Input.GetMouseButtonDown(0))
            if (CheckAndUseSkill(0))
                return;

        if (Input.GetMouseButtonDown(1))
            if (CheckAndUseSkill(1))
                return;

        if (Input.GetKeyDown(KeyCode.Q))
            if (CheckAndUseSkill(2))
                return;

        if (Input.GetKeyDown(KeyCode.R))
            if (CheckAndUseSkill(3))
                return;

        /////////////////////////////////
        // TODO - TEST CODE
        if (Input.GetKeyDown(KeyCode.Z))
        {
            Managers.GameMng.GameEndSystem.Rpc_CrewEndGame(false);
            return;
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            Managers.GameMng.GameEndSystem.Rpc_CrewEndGame(true);
            return;
        }
        ////////////////


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

        KCC.Move(Velocity * (AlienStat.Speed * Runner.DeltaTime), 0f);
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
    public override void Rpc_OnBlind(float blindTime, float backTime)
    {
        base.Rpc_OnBlind(blindTime, backTime);

        CreatureState = Define.CreatureState.Damaged;
        AlienAnimController.PlayAnim(Define.AlienActionType.Damaged);
        AlienSoundController.PlaySound(Define.AlienActionType.Damaged);
        ReturnToIdle(blindTime);
    }

    public void OnEndGame()
    {
        AlienSoundController.StopAllSound();
        AlienSoundController.PlayEndGame();

        AlienIngameUI.UIGameClear();

        Rpc_OnEndGame();
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void Rpc_OnEndGame()
    {
        gameObject.SetActive(false);
    }

    #endregion
}
