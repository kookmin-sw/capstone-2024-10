using UnityEngine;
using Data;
using Fusion;

public class Crew : Creature
{
    #region Field

    public CrewData CrewData => CreatureData as CrewData;
    public CrewStat CrewStat => (CrewStat)BaseStat;
    public CrewAnimController CrewAnimController => (CrewAnimController)BaseAnimController;
    public Inventory Inventory { get; protected set; }

    public UI_CrewIngame CrewIngameUI => IngameUI as UI_CrewIngame;

    public GameObject RightHand { get; protected set; }
    public GameObject LeftHand { get; protected set; }

    public AudioSource AudioSource { get; protected set; }
    public CrewStateMusic EffectMusic { get; protected set; }

    #endregion

    protected override void Init()
    {
        base.Init();

        Managers.ObjectMng.Crews[NetworkObject.Id] = this;

        Inventory = gameObject.GetComponent<Inventory>();
        AudioSource = gameObject.GetComponent<AudioSource>();
        EffectMusic = transform.Find("effectmusic").gameObject.GetComponent<CrewStateMusic>();

        Head = Util.FindChild(gameObject, "head.x", true);
        RightHand = Util.FindChild(gameObject, "c_middle1.r", true);
        LeftHand = Util.FindChild(gameObject, "c_middle1.l", true);
    }

    public override void SetInfo(int templateID)
    {
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

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();

        UpdateStaminaAndSanity();
        CheckHpMusic();
        CheckEffectMusic();
        StopEffectMusic();

    }

    protected override void HandleInput()
    {
        base.HandleInput();

        if (CreatureState == Define.CreatureState.Damaged || CreatureState == Define.CreatureState.Dead || CreatureState == Define.CreatureState.Use)
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
        /////////////////////////////////

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

    #region music
    private void CheckHpMusic()
    {
        if (CrewStat.Hp <= 2)
        {
            EffectMusic.CheckHurtMusic();
        }
        else
        {
            EffectMusic.StopHurtMusic();
        }
    }

    protected override void CheckEffectMusic()
    {
        if (CreatureState == Define.CreatureState.Move)
        {
            if (AudioSource.isPlaying == false)
            {
                Rpc_PlayWalkEffectMusic();
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
                        Rpc_ChageMusicVolume(0.5f);
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
                        Rpc_ChageMusicVolume(1.0f);
                        return;
                    }
                }
                return;
            }
        }
    }
    protected override void StopEffectMusic()
    {
        if (CreatureState == Define.CreatureState.Idle || CreatureState == Define.CreatureState.Interact || CreaturePose == Define.CreaturePose.Sit)
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
    private void Rpc_PlayWalkEffectMusic()
    {
        AudioSource.volume = 0.5f;
        AudioSource.spatialBlend = 1.0f;
        AudioSource.clip = Managers.SoundMng.GetOrAddAudioClip("Music/Clicks/Walk");
        AudioSource.loop = true;
        AudioSource.Play();
    }
    [Rpc(RpcSources.All, RpcTargets.All)]
    private void Rpc_PlayHitEffectMusic()
    {
        AudioSource.volume = 1f;
        AudioSource.spatialBlend = 1.0f;
        AudioSource.PlayOneShot(Managers.SoundMng.GetOrAddAudioClip("Music/Clicks/Hit"));
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
    [Rpc(RpcSources.All, RpcTargets.All)]
    private void Rpc_ChageMusicVolume(float value)
    {
        AudioSource.volume = value;
    }
    #endregion

    #region Update

    protected void UpdateStaminaAndSanity()
    {
        if (CreaturePose == Define.CreaturePose.Run && CreatureState == Define.CreatureState.Move)
            CrewStat.ChangeStamina(-Define.RUN_USE_STAMINA * Runner.DeltaTime);
        else
            CrewStat.ChangeStamina(Define.PASIVE_RECOVER_STAMINA * Runner.DeltaTime);

        if (CreatureState == Define.CreatureState.Idle && CreaturePose == Define.CreaturePose.Sit)
            CrewStat.ChangeSanity(Define.SIT_RECOVER_SANITY * Runner.DeltaTime);
        else
            CrewStat.ChangeSanity(-Define.PASIVE_REDUCE_SANITY * Runner.DeltaTime);
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
                BaseStat.Speed = CrewData.WalkSpeed;
                break;
            case Define.CreaturePose.Sit:
                BaseStat.Speed = CrewData.SitSpeed;
                break;
            case Define.CreaturePose.Run:
                BaseStat.Speed = CrewData.RunSpeed;
                break;
        }

        KCC.SetLookRotation(0, CreatureCamera.transform.rotation.eulerAngles.y);

        KCC.Move(Velocity, 0f);
    }

    #endregion

    #region Event

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void Rpc_OnDamaged(int value)
    {
        CrewStat.ChangeHp(-value);

        if (CrewStat.Hp <= 0)
        {
            OnDead();
            return;
        }

		if (CrewStat.Hp == 1)
        {
            UI_DamageScreen.DamageEffects.ScreenDamageEffect(0.5f);
        }
        else
        {
            UI_DamageScreen.DamageEffects.ScreenDamageEffect(0.3f);
        }

        Rpc_PlayHitEffectMusic();
        CrewStat.ChangeStamina(Define.DAMAGED_RECOVER_STAMINA);

        CreatureState = Define.CreatureState.Damaged;
        CrewAnimController.PlayDamaged();
        ReturnToIdle(0.5f);
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void Rpc_OnSanityDamaged(float value)
    {
        CrewStat.ChangeSanity(-value);
    }

    public void OnDead()
    {
        CreatureState = Define.CreatureState.Dead;
        EffectMusic.Rpc_StopEffectMusic();
        CrewAnimController.PlayDead();
    }

    #endregion
}
