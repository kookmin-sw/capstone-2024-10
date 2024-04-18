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

    public AudioSource AudioSource { get; set; }

    #endregion

    protected override void Init()
    {
        base.Init();

        Managers.ObjectMng.Aliens[NetworkObject.Id] = this;
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
        CreatureCamera.transform.localPosition = new Vector3(0.02f, 0.5f, 0f);
        CreatureCamera.SetInfo(this);

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

        CheckEffectMusic();
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

    #region music
    protected override void CheckEffectMusic()
    {
        if (CreatureState == Define.CreatureState.Move)
        {
            if (AudioSource.isPlaying == false)
            {
                Rpc_PlayEffectMusic();
            }
            else
            {
                if (CreaturePose == Define.CreaturePose.Stand)
                {
                    if (AudioSource.pitch == 1.0f)
                    {
                        return;
                    }
                    else
                    {
                        Rpc_ChageMusicPitch(1.0f);
                        return;
                    }
                }
                if (CreaturePose == Define.CreaturePose.Run)
                {
                    if (AudioSource.pitch == 2.0f)
                    {
                        return;
                    }
                    else
                    {
                        Rpc_ChageMusicPitch(2.0f);
                        return;
                    }
                }
                return;
            }
        }
    }
    protected override void StopEffectMusic()
    {
        if (CreatureState == Define.CreatureState.Idle || CreatureState == Define.CreatureState.Interact)
        {
            if (AudioSource.isPlaying == true)
            {
                Rpc_StopEffectMusic();
            }
            else
            {
                return;
            }

        }
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    private void Rpc_PlayEffectMusic()
    {
        AudioSource.volume = 1f;
        AudioSource.spatialBlend = 1.0f;
        AudioSource.clip = Managers.SoundMng.GetOrAddAudioClip("Music/Clicks/Monster_Walk");
        AudioSource.loop = true;
        AudioSource.Play();
    }
    [Rpc(RpcSources.All, RpcTargets.All)]
    private void Rpc_StopEffectMusic()
    {
        AudioSource.Stop();
    }
    [Rpc(RpcSources.All, RpcTargets.All)]
    private void Rpc_ChageMusicPitch(float value)
    {
        AudioSource.pitch = value;
    }
    #endregion

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

        KCC.SetLookRotation(0, CreatureCamera.transform.rotation.eulerAngles.y);
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

        KCC.SetLookRotation(0, CreatureCamera.transform.rotation.eulerAngles.y);

        KCC.Move(Velocity, 0f);
    }

    #endregion
}
