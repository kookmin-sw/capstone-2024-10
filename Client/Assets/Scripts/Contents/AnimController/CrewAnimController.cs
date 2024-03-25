using UnityEngine;
using Fusion;

public class CrewAnimController : BaseAnimController
{
    [Networked] public float SitParameter { get; protected set; }
    public CrewStat CrewStat { get; protected set; }

    protected override void Init()
    {
        base.Init();
        CrewStat = gameObject.GetComponent<CrewStat>();
    }

    #region Update

    protected override void PlayIdle()
    {
        //SetFloat("Health", CrewStat.Hp);
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

        XParameter = Mathf.Lerp(XParameter, 0, Runner.DeltaTime * 5);
        ZParameter = Mathf.Lerp(ZParameter, 0, Runner.DeltaTime * 5);
        SpeedParameter = Mathf.Lerp(SpeedParameter, 0, Runner.DeltaTime * 5);

        SetFloat("X", XParameter);
        SetFloat("Z", ZParameter);
        SetFloat("SitParameter", SitParameter);
        SetFloat("Speed", SpeedParameter);
    }

    protected override void PlayMove()
    {
        //SetFloat("Health", CrewStat.Hp);
        XParameter = Mathf.Lerp(XParameter, Creature.Direction.x, Runner.DeltaTime * 5);
        ZParameter = Mathf.Lerp(ZParameter, Creature.Direction.z, Runner.DeltaTime * 5);

        switch (CreaturePose)
        {
            case Define.CreaturePose.Stand:
                SetFloat("Z", ZParameter);
                SitParameter = Mathf.Lerp(SitParameter, 0, Runner.DeltaTime * 5);
                SpeedParameter = Mathf.Lerp(SpeedParameter, 1f, Runner.DeltaTime * 5);
                break;
            case Define.CreaturePose.Sit:
                SetFloat("Z", ZParameter);
                SitParameter = Mathf.Lerp(SitParameter, 1, Runner.DeltaTime * 5);
                SpeedParameter = Mathf.Lerp(SpeedParameter, 1, Runner.DeltaTime * 5);
                break;
            case Define.CreaturePose.Run:
                SetFloat("Z", ZParameter * 2);
                SitParameter = Mathf.Lerp(SitParameter, 0, Runner.DeltaTime * 5);
                SpeedParameter = Mathf.Lerp(SpeedParameter, 2, Runner.DeltaTime * 5);
                break;
        }


        SetFloat("X", XParameter);
        SetFloat("SitParameter", SitParameter);
        SetFloat("Speed", SpeedParameter);
    }

    public void PlayInteract()
    {

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
