using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class SkillController: NetworkBehaviour
{
    public Alien Owner { get; protected set; }

    public Dictionary<int, BaseSkill> Skills { get; set; }

    public override void Spawned()
    {
        Init();
    }

    protected virtual void Init()
    {
        Owner = gameObject.GetComponent<Alien>();
    }

    public bool CheckAndUseSkill(int skillIdx)
    {
        if (Skills[skillIdx] == null)
        {
            Debug.Log("Failed to CheckAndUseSkill");
            return false;
        }

        return Skills[skillIdx].CheckAndUseSkill();
    }
}
