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
    public SkillController SkillController { get; protected set; }
    public UI_AlienIngame AlienIngameUI => IngameUI as UI_AlienIngame;
    public float CurrentSkillRange { get; set; }

    public AudioSource AudioSource_1 { get; set; }

    #endregion

    protected override void Init()
    {
        base.Init();

        Managers.ObjectMng.Aliens[NetworkObject.Id] = this;
        SkillController = gameObject.GetComponent<SkillController>();
        SkillController.Skills = new Dictionary<int, BaseSkill>(Define.MAX_SKILL_NUM);

        AudioSource_1 = gameObject.GetComponent<AudioSource>();
    }

    public override void SetInfo(int templateID)
    {
        CreatureType = Define.CreatureType.Alien;

        base.SetInfo(templateID);

        Head = Util.FindChild(gameObject, "head", true);
        Head.transform.localScale = Vector3.zero;

        if (IsFirstPersonView)
        {

            CreatureCamera = Managers.ResourceMng.Instantiate("Cameras/CreatureCamera", Util.FindChild(gameObject, "Anglerox_ Neck", true).transform).GetComponent<CreatureCamera>();
            CreatureCamera.transform.localPosition = new Vector3(0.02f, 0.5f, 0f);
            CreatureCamera.SetInfo(this);
        }
        else
        {
            WatchingCamera = Managers.ResourceMng.Instantiate("Cameras/WatchingCamera", gameObject.transform).GetComponent<WatchingCamera>();
            WatchingCamera.enabled = true;
            WatchingCamera.Creature = this;
        }

        AlienStat.SetStat(AlienData);

        CurrentSkillRange = 1.5f;
        IsSpawned = true;

        if (Managers.SceneMng.CurrentScene.IsSceneType((int)Define.SceneType.GameScene))
        {
            StartCoroutine(Managers.SceneMng.CurrentScene.OnPlayerSpawn());
        }
    }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();

        PlayEffectMusic();
        StopEffectMusic();
    }

    protected override void HandleInput()
    {
        base.HandleInput();

        if (CreatureState == Define.CreatureState.Interact || CreatureState == Define.CreatureState.Use)
            return;

        CheckInteractable(false);

        if (Input.GetKeyDown(KeyCode.F))
            if (CheckInteractable(true))
                return;

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

        if (Velocity == Vector3.zero)
            CreatureState = Define.CreatureState.Idle;
        else
        {
            CreatureState = Define.CreatureState.Move;

            if (Input.GetKey(KeyCode.LeftShift) && Direction.z > 0)
                CreaturePose = Define.CreaturePose.Run;
            else
                if (CreaturePose == Define.CreaturePose.Run)
                    CreaturePose = Define.CreaturePose.Stand;
        }
    }

    protected bool CheckAndUseSkill(int skillIdx)
    {
        if (!HasStateAuthority)
            return false;

        return SkillController.CheckAndUseSkill(skillIdx);
    }

    public void OnDrawGizmos()
    {
        if (!IsSpawned)
            return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere( Head.transform.position + transform.forward * CurrentSkillRange, CurrentSkillRange);
    }

    #region Update

    protected override void UpdateIdle()
    {
        switch (CreaturePose)
        {
            case Define.CreaturePose.Stand:
                break;
            case Define.CreaturePose.Run:
                CreaturePose = Define.CreaturePose.Stand;
                break;
        }

        if (IsFirstPersonView)
        {
            KCC.SetLookRotation(0, CreatureCamera.transform.rotation.eulerAngles.y);
        }
    }

    protected override void UpdateMove()
    {
        switch (CreaturePose)
        {
            case Define.CreaturePose.Stand:
                BaseStat.Speed = AlienData.WalkSpeed;
                break;
            case Define.CreaturePose.Run:
                BaseStat.Speed = AlienData.RunSpeed;
                break;
        }

        if (IsFirstPersonView)
        {
            KCC.SetLookRotation(0, CreatureCamera.transform.rotation.eulerAngles.y);
        }
        else
        {
            if (Velocity != Vector3.zero)
            {
                Quaternion newRotation = Quaternion.LookRotation(Velocity);
                KCC.SetLookRotation(newRotation);
            }
        }

        KCC.Move(Velocity, 0f);
    }

    protected override void UpdateInteract()
    {
    }

    protected override void UpdateUse()
    {
    }

    #endregion

    private void PlayEffectMusic()
    {
        if (CreatureState == Define.CreatureState.Move)
        {
            if (AudioSource_1.isPlaying == false)
            {
                AudioSource_1.volume = 1f;
                AudioSource_1.clip = Managers.SoundMng.GetOrAddAudioClip("Music/Clicks/Monster_Walk");
                AudioSource_1.Play();
            }
            else
            {
                if (CreaturePose == Define.CreaturePose.Stand)
                {
                    AudioSource_1.pitch = 1f;
                    AudioSource_1.volume = 1f;
                }
                if (CreaturePose == Define.CreaturePose.Run)
                {
                    AudioSource_1.pitch = 2f;
                    AudioSource_1.volume = 1f;
                }
                return;
            }
        }
    }

    private void StopEffectMusic()
    {
        if (CreatureState == Define.CreatureState.Idle || CreatureState == Define.CreatureState.Interact)
        {
            AudioSource_1.Stop();
        }
    }
}
