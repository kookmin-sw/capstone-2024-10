using UnityEngine;
using Data;

public class Crew : Creature
{
    #region Field

    public CrewData CrewData => CreatureData as CrewData;
    public CrewStat CrewStat => (CrewStat)BaseStat;
    public bool IsRecoveringStamina { get; protected set; }

    #endregion

    public override void SetInfo(int templateID)
    {
        CreatureType = Define.CreatureType.Crew;
        Transform.parent = Managers.ObjectMng.CrewRoot;

        base.SetInfo(templateID);

        CrewStat.SetStat(CrewData);
        //UICrewStatus = FindObjectOfType<UI_CrewStat>();
        //UICrewStatus.CurrentCrew = this;
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

        if (Input.GetKeyDown(KeyCode.F))
        {
            if (CreatureState == Define.CreatureState.Interact)
                CreatureState = Define.CreatureState.Idle;
            else
                if (RayCast())
                    return;
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            OnDamaged(50); // TODO - Test Code
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            if (CreaturePose != Define.CreaturePose.Sit)
            {
                CreaturePose = Define.CreaturePose.Sit;
                CreatureCamera.transform.position -= new Vector3(0f, 0.5f, 0f); //앉을 경우 카메라도 같이 시점 동기화를 위해 y값 낮추기

            }
            else
            {
                CreaturePose = Define.CreaturePose.Stand;
                CreatureCamera.transform.position += new Vector3(0f, 0.5f, 0f);
            }
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
                if (IsRecoveringStamina)    //스테미너가 0이 되어도 SHIFT를 계속 누르고 있으면 RUN 상태 유지를 해제하기 위해
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
        // TODO
    }

    protected override void UpdateDead()
    {
        // TODO
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
}
