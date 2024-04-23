using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Data;

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
        IsChasing = false;
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

        if (CreatureState == Define.CreatureState.Interact || CreatureState == Define.CreatureState.Use)
            return;

        if (CreatureState == Define.CreatureState.Interact)
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

        CheckChasing();

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

    protected void CheckChasing()
    {
        if (!HasStateAuthority || !IsSpawned)
            return;

        for (float i = 0.2f; i < 0.9f; i += 0.1f)
        {
            for (float j = 0.2f; j < 0.9f; j += 0.1f)
            {
                Ray ray = CreatureCamera.Camera.ViewportPointToRay(
                    new Vector3(i, j, CreatureCamera.Camera.nearClipPlane));

                if (i < 0.25f || j < 0.25f || i > 0.75f || j > 0.75f)
                    Debug.DrawRay(ray.origin, ray.direction * 15f, Color.green);

                if (Physics.Raycast(ray, out RaycastHit rayHit, maxDistance: 15f,
                        layerMask: LayerMask.GetMask("Crew", "MapObject")))
                {
                    if (rayHit.transform.gameObject.TryGetComponent(out Crew crew))
                    {
                        if (!IsChasing)
                        {
                            StopAllCoroutines();
                            IsChasing = true;
                            if (!Managers.SoundMng.IsPlaying(Define.SoundType.Bgm))
                                Managers.SoundMng.Play($"{Define.BGM_PATH}/The Big Clash", Define.SoundType.Bgm, volume: 1,
                                    isLoop: true);
                        }

                        return;
                    }
                }
            }
        }

        if (IsChasing)
            StartCoroutine(CheckNotChasing());

        IsChasing = false;
    }

    protected IEnumerator CheckNotChasing()
    {
        float currentChasingTime = 0f;
        while (currentChasingTime < 8f)
        {
            currentChasingTime += Time.deltaTime;
            yield return null;
        }

        Managers.SoundMng.Stop(Define.SoundType.Bgm);
    }

    protected bool CheckAndUseSkill(int skillIdx)
    {
        if (!HasStateAuthority || !IsSpawned)
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
                AlienStat.Speed = AlienStat.WalkSpeed;
                break;
            case Define.CreaturePose.Run:
                AlienStat.Speed = AlienStat.RunSpeed;
                break;
        }

        KCC.SetLookRotation(0, CreatureCamera.transform.rotation.eulerAngles.y);

        KCC.Move(Velocity * (AlienStat.Speed * Runner.DeltaTime), 0f);
    }

    #endregion
}
