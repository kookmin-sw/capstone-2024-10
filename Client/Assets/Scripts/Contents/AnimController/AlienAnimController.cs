using UnityEngine;
using Fusion;

public class AlienAnimController : BaseAnimController
{
    protected override void Init()
    {
        base.Init();
        SetFloat("currentSpeed", 0);
        SetFloat("skill", 0);
    }

    #region Update

    protected override void PlayIdle()
    {
        SetFloat("currentSpeed", 0);
        SetFloat("skill", 0);
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

    protected override void PlayUse()
    {
        switch (CreaturePose)
        {
            case Define.CreaturePose.Stand:
                SetFloat("skill", 5);
                //Debug.Log("Alien PlayInteract anim called");
                break;
            case Define.CreaturePose.Run:
                SetFloat("skill", -5);
                //Debug.Log("Alien PlayInteract anim called");
                break;
            case Define.CreaturePose.Sit:
                SetFloat("skill", 5);
                //Debug.Log("Alien PlayInteract anim called");
                break;

        }

    }

    #endregion
}
