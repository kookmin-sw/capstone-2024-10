using UnityEngine;
using Fusion;

public class CrewAnimController : BaseAnimController
{
    [Networked] public float SitDown { get; protected set; }
    [Networked] public float SitSpeedParameter { get; protected set; }

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
        if (Creature.CreatureType == Define.CreatureType.Crew)
        {
            switch (CreaturePose)
            {
                case Define.CreaturePose.Stand:
                case Define.CreaturePose.Run:
                    float smoothness = 5f; // 조절 가능한 부드러움 계수
                    SitDown = Mathf.Lerp(SitDown, 0, Runner.DeltaTime * smoothness);
                    SetFloat("Sit", SitDown);
                    StandSpeedParameter = Mathf.Lerp(StandSpeedParameter, 0, Runner.DeltaTime * smoothness);
                    SetFloat("moveSpeed", StandSpeedParameter);
                    break;
                case Define.CreaturePose.Sit:
                    float sit_smoothness = 5f; // 조절 가능한 부드러움 계수
                    SitDown = Mathf.Lerp(SitDown, 1, Runner.DeltaTime * sit_smoothness);
                    SetFloat("Sit", SitDown);
                    SitSpeedParameter = Mathf.Lerp(SitSpeedParameter, 0, Runner.DeltaTime * sit_smoothness);
                    SetFloat("sitSpeed", SitSpeedParameter);
                    break;
            }
        }
        else
        {

        }

    }

    protected override void PlayMove()
    {
        switch (CreaturePose)
        {
            case Define.CreaturePose.Stand:
                float stand_smoothness = 4f; // 조절 가능한 부드러움 계수
                SitDown = Mathf.Lerp(SitDown, 0, Runner.DeltaTime * stand_smoothness);
                SetFloat("Sit", SitDown);
                StandSpeedParameter = Mathf.Lerp(StandSpeedParameter, 1.5f, Runner.DeltaTime * stand_smoothness);
                SetFloat("moveSpeed", StandSpeedParameter);
                break;
            case Define.CreaturePose.Sit:
                float sit_smoothness = 5f; // 조절 가능한 부드러움 계수
                SitDown = Mathf.Lerp(SitDown, 1, Runner.DeltaTime * sit_smoothness);
                SetFloat("Sit", SitDown);
                SitSpeedParameter = Mathf.Lerp(SitSpeedParameter, 1, Runner.DeltaTime * sit_smoothness);
                SetFloat("sitSpeed", SitSpeedParameter);
                break;
            case Define.CreaturePose.Run:
                float run_smoothness = 2f; // 조절 가능한 부드러움 계수
                StandSpeedParameter = Mathf.Lerp(StandSpeedParameter, 2, Runner.DeltaTime * run_smoothness);
                SetFloat("moveSpeed", StandSpeedParameter);
                break;
        }
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
