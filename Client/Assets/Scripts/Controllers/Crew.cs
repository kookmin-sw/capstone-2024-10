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

    [Networked] public bool IsRecoveringStamina { get; protected set; }

    #endregion

    protected override void Init()
    {
        base.Init();

        Inventory = gameObject.GetComponent<Inventory>();

        Managers.ObjectMng.Crews[NetworkObject.Id] = this as Crew;
    }

    public override void SetInfo(int templateID)
    {
        CreatureType = Define.CreatureType.Crew;

        base.SetInfo(templateID);

        Transform.parent = Managers.ObjectMng.CrewRoot;
        Head = Util.FindChild(gameObject, "head.x", true);
        Head.transform.localScale = Vector3.zero;

        if (IsFirstPersonView)
        {

            CreatureCamera = Managers.ResourceMng.Instantiate("Cameras/CreatureCamera", Util.FindChild(gameObject, "neck.x", true).transform).GetComponent<CreatureCamera>();
            CreatureCamera.transform.localPosition = new Vector3(0f, 0.2f, 0f);
            CreatureCamera.SetInfo(this);
        }
        else
        {
            WatchingCamera = Managers.ResourceMng.Instantiate("Cameras/WatchingCamera", gameObject.transform).GetComponent<WatchingCamera>();
            WatchingCamera.enabled = true;
            WatchingCamera.Creature = this;
        }

        CrewStat.SetStat(CrewData);

        IsRecoveringStamina = true;
    }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();

        UpdateStamina();
    }

    protected override void HandleInput()
    {
        base.HandleInput();

        if (CreatureState == Define.CreatureState.Dead)
            return;

        CheckInteract(true);

        // TODO - Test Code
        if (Input.GetKeyDown(KeyCode.E))
        {
            OnDamaged(50);
            return;
        }

        if (CreatureState == Define.CreatureState.Use)
            return;

        if (CreatureState == Define.CreatureState.Interact)
            if (Input.GetKeyDown(KeyCode.F))
            {
                CreatureState = Define.CreatureState.Idle;
                return;
            }

        if (Input.GetKeyDown(KeyCode.F))
        {
            if (CheckInteract(false))
            {
                CreatureState = Define.CreatureState.Interact;
                return;
            }
        }

        //if (Input.GetMouseButtonDown(0))
        //{
        //    if (CheckAndUseItem())
        //    {
        //        CreatureState = Define.CreatureState.Use;
        //        return;
        //    }
        //}

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

            if (Input.GetKey(KeyCode.LeftShift))
            {
                if (CreaturePose != Define.CreaturePose.Sit && !IsRecoveringStamina)
                {
                    CreaturePose = Define.CreaturePose.Run;
                }
                if (IsRecoveringStamina)
                {
                    CreaturePose = Define.CreaturePose.Stand;
                }
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

    #region Update

    protected void UpdateStamina()
    {
        if (CreaturePose == Define.CreaturePose.Run && CreatureState == Define.CreatureState.Move)
        {
            CrewStat.OnUseStamina(Define.RUN_USE_STAMINA * Runner.DeltaTime);
            if (CrewStat.Stamina <= 0)
                IsRecoveringStamina = true;
        }
        else
        {
            CrewStat.OnRecoverStamina(Define.PASIVE_RECOVER_STAMINA * Runner.DeltaTime);
            if (CrewStat.Stamina >= 20) //스테미너가 0이하가 된 뒤 20까지 회복 되면 다시 달리기 가능으로 변경
            {
                IsRecoveringStamina = false;
            }
        }
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
                BaseStat.Speed = CrewData.WalkSpeed;
                break;
            case Define.CreaturePose.Sit:
                BaseStat.Speed = CrewData.SitSpeed;
             	break;
            case Define.CreaturePose.Run:
                BaseStat.Speed = CrewData.RunSpeed;
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

    protected override void UpdateUse()
    {
        CrewAnimController.PlayUseItem();
    }

    protected override void UpdateDead()
    {
        CrewAnimController.PlayDead();
    }

    #endregion

    #region Event

    public void OnDamaged(int damage)
    {
        CrewStat.OnDamage(damage);

        if (CrewStat.Hp <= 0)
        {
            OnDead();
            return;
        }
    }

    public void OnDead()
    {
        CreatureState = Define.CreatureState.Dead;
    }

    #endregion

    protected bool CheckAndUseItem()
    {
        if (Inventory.CurrentItem == null)
        {
            Debug.Log("No Item");
            return false;
        }

        return Inventory.CurrentItem.CheckAndUseItem();
    }
}
