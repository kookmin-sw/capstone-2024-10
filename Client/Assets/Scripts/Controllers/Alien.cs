using System.Collections.Generic;
using UnityEngine;
using Data;
using System.Collections;

public abstract class Alien : Creature
{
    #region Field

    public AlienData AlienData => CreatureData as AlienData;
    public AlienStat AlienStat => (AlienStat)BaseStat;

    public List<BaseSkill> Skills { get; protected set; }

    private bool isInputBlocked = false;

    #endregion

    protected override void Init()
    {
        base.Init();

        Managers.ObjectMng.Aliens[NetworkObject.Id] = this;
    }

    public override void SetInfo(int templateID)
    {
        CreatureType = Define.CreatureType.Alien;

        base.SetInfo(templateID);

        Transform.parent = Managers.ObjectMng.AlienRoot;
        Head = Util.FindChild(gameObject, "head", true);
        Head.transform.localScale = Vector3.zero;

        if (IsFirstPersonView)
        {

            CreatureCamera = Managers.ResourceMng.Instantiate("Cameras/CreatureCamera", Util.FindChild(gameObject, "Anglerox_ Neck", true).transform).GetComponent<CreatureCamera>();
            CreatureCamera.transform.localPosition = new Vector3(-0.2f, 0f, 0f);
            CreatureCamera.SetInfo(this);
        }
        else
        {
            WatchingCamera = Managers.ResourceMng.Instantiate("Cameras/WatchingCamera", gameObject.transform).GetComponent<WatchingCamera>();
            WatchingCamera.enabled = true;
            WatchingCamera.Creature = this;
        }

        AlienStat.SetStat(AlienData);

        Skills = new List<BaseSkill>(4);
        for (int i = 0; i < Define.MAX_ITEM_NUM; i++)
        {
            Skills.Add(new BasicAttack(i));
        }
    }

    protected override void HandleInput()
    {
        if (isInputBlocked)
        {
            return;
        }

        base.HandleInput();

        if (Input.GetKeyDown(KeyCode.F))
        {
            if (CheckAndInteract(false))
            {
                CreatureState = Define.CreatureState.Interact;
                return;
            }
        }
        else
            CheckAndInteract(true);

        if (Input.GetMouseButtonDown(0))
        {
            if (CheckAndUseSkill(0))
            {
                CreatureState = Define.CreatureState.Use;
                CreaturePose = Define.CreaturePose.Stand;
                StartCoroutine(BlockInputForSeconds(1.5f));
                return;
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            if (CheckAndUseSkill(1))
            {
                CreatureState = Define.CreatureState.Use;
                return;
            }
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (CheckAndUseSkill(2))
            {
                CreatureState = Define.CreatureState.Use;
                CreaturePose = Define.CreaturePose.Run;
                StartCoroutine(BlockInputForSeconds(1.5f));

                return;
            }
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            if (CheckAndUseSkill(3))
            {
                CreatureState = Define.CreatureState.Use;
                CreaturePose = Define.CreaturePose.Run;
                return;
            }
        }

        if (Velocity == Vector3.zero)
            CreatureState = Define.CreatureState.Idle;
        else
        {
            CreatureState = Define.CreatureState.Move;

            if (Input.GetKey(KeyCode.LeftShift))
            {
                CreaturePose = Define.CreaturePose.Run;
            }
            else
            {
                if (CreaturePose == Define.CreaturePose.Run)
                {
                    CreaturePose = Define.CreaturePose.Stand;
                }
            }
        }
    }
    private IEnumerator BlockInputForSeconds(float seconds)
    {
        isInputBlocked = true;
        yield return new WaitForSeconds(seconds);
        isInputBlocked = false;
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
        //AlienSkill alienSkill = new AlienSkill();
        //alienSkill.Rpc_Use();
    }

    protected override void UpdateUse()
    {

    }

    protected override void UpdateDead()
    {
        // TODO
    }

    #endregion

    protected bool CheckAndUseSkill(int skillIdx)
    {
        if (Skills[skillIdx] == null)
        {
            Debug.Log("No SKill" + skillIdx);
            return false;
        }

        return Skills[skillIdx].CheckAndUseSkill();
    }
}
