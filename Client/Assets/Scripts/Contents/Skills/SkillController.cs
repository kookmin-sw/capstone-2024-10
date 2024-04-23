using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class SkillController: NetworkBehaviour
{
    public Dictionary<int, BaseSkill> Skills { get; set; }

    public bool CheckAndUseSkill(int skillIdx)
    {
        if (!Skills.ContainsKey(skillIdx))
        {
            Debug.Log("Failed to CheckAndUseSkill");
            return false;
        }

        return Skills[skillIdx].CheckAndUseSkill();
    }
}
