using UnityEngine;
using Fusion;
using Data;

public class Crew : Creature
{
    #region Field

    public CrewData CrewData => CreatureData as CrewData;
    public CrewStat CrewStat => (CrewStat)CreatureStat;

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
                InterAct();

            return;
        }

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
                CreaturePose = Define.CreaturePose.Run;
            else
                if (CreaturePose == Define.CreaturePose.Run)
                    CreaturePose = Define.CreaturePose.Stand;
        }
    }

    #region Update

    protected override void UpdateIdle()
    {
        switch (CreaturePose)
        {
            case Define.CreaturePose.Stand:
                // TODO
                break;
            case Define.CreaturePose.Sit:
                // TODO
                break;
            case Define.CreaturePose.Run:
                // TODO
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
                break;
            case Define.CreaturePose.Sit:
                CreatureStat.Speed = CrewData.SitSpeed;
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

    protected void InterAct()
    {
        Ray ray = CreatureCamera.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit rayHit, maxDistance: 3f, layerMask: LayerMask.GetMask("Interact")))
        {
            CreatureState = Define.CreatureState.Interact;

            IInteractable interactable = rayHit.transform.gameObject.GetComponent<IInteractable>();
            interactable.Interact();

            Debug.DrawLine(ray.origin, rayHit.point, Color.red, 1f); // TODO - Test Code
        }
        else
        {
            Debug.Log("Failed to InterAct");
            Debug.DrawRay(ray.origin, ray.direction * 3f, Color.red, 1f); // TODO - Test Code
        }
    }

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
