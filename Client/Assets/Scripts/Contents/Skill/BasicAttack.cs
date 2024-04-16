using Fusion;
using System.Collections;
using UnityEngine;

public class BasicAttack : BaseSkill
{
    protected override void Init()
    {
        base.Init();

        SkillDescription = "BASIC ATTACK";
        CoolTime = 2f;
        TotalSkillAmount = 1.2f;
        TotalReadySkillAmount = -1f;
        AttackRange = 1.5f;
    }

    public override bool CheckAndUseSkill()
    {
        if (!Ready)
            return false;

        Owner.CurrentSkillRange = AttackRange;
        UseSkill();
        return true;
    }

    public override void UseSkill()
    {
        base.UseSkill();

        Owner.AlienAnimController.PlayBasicAttack();
        Rpc_PlayEffectMusic();
        StartCoroutine(ProgressSkill());
    }

    protected override IEnumerator ProgressSkill()
    {
        while (CurrentSkillAmount < TotalSkillAmount)
        {
            Vector3 attackPosition = Owner.transform.position + ForwardDirection * AttackRange;
            Collider[] hitColliders = new Collider[3];

            if (!IsHit && Physics.OverlapSphereNonAlloc(attackPosition, AttackRange, hitColliders, LayerMask.GetMask("Crew")) > 0)
            {
                if (hitColliders[0].gameObject.TryGetComponent(out Crew crew))
                {
                    IsHit = true;
                    crew.Rpc_OnDamaged(Owner.AlienStat.AttackDamage);
                }
            }

            UpdateWorkAmount(Time.deltaTime);
            yield return null;
        }

        SkillInterrupt();
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    private void Rpc_PlayEffectMusic()
    {
        Owner.AudioSource_1.volume = 1f;
        Owner.AudioSource_1.pitch = 1f;
        Owner.AudioSource_1.clip = Managers.SoundMng.GetOrAddAudioClip("Music/Clicks/Monster_Attack1");
        Owner.AudioSource_1.Play();
    }
}
