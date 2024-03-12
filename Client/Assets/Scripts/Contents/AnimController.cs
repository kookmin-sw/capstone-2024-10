using UnityEngine;
using Fusion;

public class AnimController : NetworkBehaviour
{
    public NetworkMecanimAnimator NetworkAnim { get; protected set; }

    public Creature Creature { get; protected set; }
    public Define.CreatureState CreatureState => Creature.CreatureState;
    public Define.CreaturePose CreaturePose => Creature.CreaturePose;

    //애니메이션을 위한 변수
    private float _SitDown = 0;
    private float _CurrentSpeed = 0;    //현재 속도
    private float _SitWalkSpeed = 0;    //앉아서 걷는 속도
    private float _CurrentHp;  //현재 체력
    private float _CurrentStamina;    //현재 스테미나

    public override void Spawned()
    {
        Init();
    }

    protected virtual void Init()
    {
        NetworkAnim = gameObject.GetComponent<NetworkMecanimAnimator>();

        Creature = gameObject.GetComponent<Creature>();
        SetFloat("Health", 100);
    }

    #region Update
    public void UpdateAnimation()
    {
        switch (CreatureState)
        {
            case Define.CreatureState.Idle:
                PlayIdle();
                break;
            case Define.CreatureState.Move:
                PlayMove();
                break;
            case Define.CreatureState.Use:
                PlayUse();
                break;
            case Define.CreatureState.Dead:
                PlayDead();
                break;
        }
    }

    protected void PlayIdle()
    {
        switch (CreaturePose)
        {
            case Define.CreaturePose.Stand:
                float smoothness = 5f; // 조절 가능한 부드러움 계수
                _SitDown = Mathf.Lerp(_SitDown, 0, Runner.DeltaTime * smoothness);
                SetFloat("Sit", _SitDown);
                _CurrentSpeed = Mathf.Lerp(_CurrentSpeed, 0, Runner.DeltaTime * smoothness);
                SetFloat("moveSpeed", _CurrentSpeed);
                break;
            case Define.CreaturePose.Sit:
                float sit_smoothness = 5f; // 조절 가능한 부드러움 계수
                _SitDown = Mathf.Lerp(_SitDown, 1, Runner.DeltaTime * sit_smoothness);
                SetFloat("Sit", _SitDown);
                _SitWalkSpeed = Mathf.Lerp(_SitWalkSpeed, 0, Runner.DeltaTime * sit_smoothness);
                SetFloat("sitSpeed", _SitWalkSpeed);
                break;
        }
    }

    protected void PlayMove()
    {
        switch (CreaturePose)
        {
            case Define.CreaturePose.Stand:
                float stand_smoothness = 4f; // 조절 가능한 부드러움 계수
                _SitDown = Mathf.Lerp(_SitDown, 0, Runner.DeltaTime * stand_smoothness);
                SetFloat("Sit", _SitDown);
                _CurrentSpeed = Mathf.Lerp(_CurrentSpeed, 1.5f, Runner.DeltaTime * stand_smoothness);
                SetFloat("moveSpeed", _CurrentSpeed);
                break;
            case Define.CreaturePose.Sit:
                float sit_smoothness = 5f; // 조절 가능한 부드러움 계수
                _SitDown = Mathf.Lerp(_SitDown, 1, Runner.DeltaTime * sit_smoothness);
                SetFloat("Sit", _SitDown);
                _SitWalkSpeed = Mathf.Lerp(_SitWalkSpeed, 1, Runner.DeltaTime * sit_smoothness);
                SetFloat("sitSpeed", _SitWalkSpeed);
                break;
            case Define.CreaturePose.Run:
                float run_smoothness = 2f; // 조절 가능한 부드러움 계수
                _CurrentSpeed = Mathf.Lerp(_CurrentSpeed, 2, Runner.DeltaTime * run_smoothness);
                SetFloat("moveSpeed", _CurrentSpeed);
                break;
        }
    }

    protected void PlayUse()
    {
        // TODO
    }

    protected void PlayDead()
    {
        // TODO
    }

    #endregion

    #region SetParameter

    protected void SetTrigger(string parameter)
    {
        NetworkAnim.SetTrigger("IsRunning");
    }

    protected void SetBool(string parameter, bool value)
    {
        NetworkAnim.Animator.SetBool(parameter, value);
    }

    protected void SetFloat(string parameter, float value)
    {
        NetworkAnim.Animator.SetFloat(parameter, value);
    }

    #endregion
}
