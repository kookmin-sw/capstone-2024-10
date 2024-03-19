using UnityEngine;
using Data;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using Unity.VisualScripting;

public class Crew : Creature
{
    #region Field

    public CrewData CrewData => CreatureData as CrewData;
    public CrewStat CrewStat => (CrewStat)CreatureStat;
    public UI_CrewStat UICrewStatus { get; set; }
    public bool CanRun { get; protected set; }
    #endregion
    public override void Spawned()
    {
        base.Init();
    }

    public override void SetInfo(int templateID)
    {
        CreatureType = Define.CreatureType.Crew;
        Transform.parent = Managers.ObjectMng.CrewRoot;
        
        base.SetInfo(templateID);

        CrewStat.SetStat(CrewData);
        UICrewStatus = FindObjectOfType<UI_CrewStat>();
        UICrewStatus.CurrentCrew = this;
        CanRun = true;
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
            OnDamaged(50);
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
                if (CreaturePose != Define.CreaturePose.Sit)
                {
                    StaminaUse();
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

    protected override void UpdateIdle()
    {
        switch (CreaturePose)
        {
            case Define.CreaturePose.Stand:
                StaminaRecover();
                break;
            case Define.CreaturePose.Sit:
                StaminaRecover();
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
                CreatureStat.Speed = CrewData.WalkSpeed;
                StaminaRecover();
                break;
            case Define.CreaturePose.Sit:
                CreatureStat.Speed = CrewData.SitSpeed;
                StaminaRecover();
                break;
            case Define.CreaturePose.Run:
                CreatureStat.Speed = CrewData.RunSpeed;
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

    #region stamina
    public void StaminaUse()
    {
        if (CrewStat.Stamina > 0 && CanRun == true)
        {
            CreaturePose = Define.CreaturePose.Run;
            CrewStat.Stamina -= 20.0f * Runner.DeltaTime;
        }
        else if (CrewStat.Stamina <= 0 || CanRun == false)    //스테미너가 0을 찍고나서 다시 20까지 회복하기 전까진 달리기 불가능
        {
            CreaturePose = Define.CreaturePose.Stand;
            CanRun = false;
        }
    }
    public void StaminaRecover()
    {
        if (CrewStat.Stamina < CrewData.Stamina)
        {
            if (CrewStat.Stamina >= 20.0f)    //스테미너가 0을 찍은 이후 20까지 회복되면 그때 다시 달리기 가능
            {
                CanRun = true;
            }
            CrewStat.Stamina += 15.0f * Runner.DeltaTime;
        }
    }

    #endregion
}
