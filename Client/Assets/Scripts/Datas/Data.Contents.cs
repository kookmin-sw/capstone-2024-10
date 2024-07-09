using System;
using System.Collections.Generic;
using UnityEngine.Serialization;

namespace Data
{
    #region CreatureData

    [Serializable]
    public class CreatureData
    {
        public int DataId;
        public string Name;
        public float WalkSpeed;
    }

    #endregion

    #region CrewData

    [Serializable]
    public class CrewData : CreatureData
    {
        public float SitSpeed;
        public float RunSpeed;
        public int MaxHp;
        public float MaxStamina;
        public float MaxSanity;
        public float RunUseStamina;
        public float PassiveRecoverStamina;
        public float DamagedRecoverStamina;
        public float ErosionReduceSanity;
        public float SitRecoverSanity;
    }

    [Serializable]
    public class CrewDataLoader : ILoader<int, CrewData>
    {
        public List<CrewData> crews = new List<CrewData>();

        // List형태의 Data를 Dictionary형태로 변환 후 반환
        public Dictionary<int, CrewData> MakeDict()
        {
            Dictionary<int, CrewData> dic = new Dictionary<int, CrewData>();
            foreach (CrewData stat in crews)
                dic.Add(stat.DataId, stat);

            return dic;
        }
    }

    #endregion

    #region AlienData

    [Serializable]
    public class AlienData : CreatureData
    {
    }

    [Serializable]
    public class AlienDataLoader : ILoader<int, AlienData>
    {
        public List<AlienData> aliens = new List<AlienData>();

        public Dictionary<int, AlienData> MakeDict()
        {
            Dictionary<int, AlienData> dic = new Dictionary<int, AlienData>();
            foreach (AlienData stat in aliens)
                dic.Add(stat.DataId, stat);

            return dic;
        }
    }

    #endregion

    #region ItemData

    [Serializable]
    public class ItemData
    {
        public int DataId;
        public string Name;
        public float Value;
    }

    [Serializable]
    public class ItemDataLoader : ILoader<int, ItemData>
    {
        public List<ItemData> items = new List<ItemData>();

        public Dictionary<int, ItemData> MakeDict()
        {
            var dic = new Dictionary<int, ItemData>();
            foreach (ItemData item in items)
                dic.Add(item.DataId, item);

            return dic;
        }
    }

    #endregion

    #region SkillData

    [Serializable]
    public class SkillData
    {
        public int DataId;
        public string Name;
        public float CoolTime;
        public float Range;
        public int Damage;
        public float SanityDamage;
        public float TotalSkillAmount;
        public float TotalReadySkillAmount;

    }

    [Serializable]
    public class SkillDataLoader : ILoader<int, SkillData>
    {
        public List<SkillData> skills = new List<SkillData>();

        public Dictionary<int, SkillData> MakeDict()
        {
            var dic = new Dictionary<int, SkillData>();
            foreach (SkillData skill in skills)
                dic.Add(skill.DataId, skill);

            return dic;
        }
    }

    #endregion
}
