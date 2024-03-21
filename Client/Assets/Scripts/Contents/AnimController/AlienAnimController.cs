using UnityEngine;
using Fusion;

public class AlienAnimController : BaseAnimController
{
    protected override void Init()
    {
        base.Init();
        SetFloat("currentSpeed", 0);
    }

    #region Update

    protected override void PlayIdle()
    {
        SetFloat("currentSpeed", 0);
    }

    protected override void PlayMove()
    {
        switch (CreaturePose)
        {
            case Define.CreaturePose.Stand:
                SetFloat("currentSpeed", 5);
                break;
            case Define.CreaturePose.Run:
                SetFloat("currentSpeed", 10);
                break;
        }
    }

    protected override void PlayInteract()
    {
        SetTrigger("attack");
        Debug.Log("Alien PlayInteract anim called");
    }

    protected override void PlayDead()
    {
        // TODO
    }

    #endregion
}
