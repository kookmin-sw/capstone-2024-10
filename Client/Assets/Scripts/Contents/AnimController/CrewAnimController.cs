using UnityEngine;
using Fusion;

public class CrewAnimController : BaseAnimController
{
    [Networked] public float SitParameter { get; protected set; }

    protected override void Init()
    {
        base.Init();

        SetFloat("Health", 100);
        SetFloat("Sit", 0);
        SetFloat("sitSpeed", 0);
    }

    #region Update

    protected override void PlayIdle()
    {
        switch (CreaturePose)
        {
            case Define.CreaturePose.Stand:
            case Define.CreaturePose.Run:
                SitParameter = Mathf.Lerp(SitParameter, 0, Runner.DeltaTime * 5);
                break;
            case Define.CreaturePose.Sit:
                SitParameter = Mathf.Lerp(SitParameter, 1, Runner.DeltaTime * 5);
                break;
        }

        SetFloat("X", 0);
        SetFloat("Z", 0);
        SetFloat("SitParameter", SitParameter);
        SpeedParameter = Mathf.Lerp(SpeedParameter, 0, Runner.DeltaTime * 5);
        SetFloat("Speed", SpeedParameter);
    }

    protected override void PlayMove()
    {
        switch (CreaturePose)
        {
            case Define.CreaturePose.Stand:
                SetFloat("Z", Creature.Direction.z);
                SitParameter = Mathf.Lerp(SitParameter, 0, Runner.DeltaTime * 5);
                SpeedParameter = Mathf.Lerp(SpeedParameter, 1f, Runner.DeltaTime * 5);
                break;
            case Define.CreaturePose.Sit:
                SetFloat("Z", Creature.Direction.z);
                SitParameter = Mathf.Lerp(SitParameter, 1, Runner.DeltaTime * 5);
                SpeedParameter = Mathf.Lerp(SpeedParameter, 1, Runner.DeltaTime * 5);

                break;
            case Define.CreaturePose.Run:
                SetFloat("Z", Creature.Direction.z * 2);
                SpeedParameter = Mathf.Lerp(SpeedParameter, 2, Runner.DeltaTime * 5);
                break;
        }

        SetFloat("X", Creature.Direction.x);
        SetFloat("SitParameter", SitParameter);
        SetFloat("Speed", SpeedParameter);
    }

    public void PlayInteract()
    {
        // TODO
    }

    public void PlayUseItem()
    {
        // TODO
    }

    public void PlayDead()
    {
        // TODO
    }

    #endregion
}
