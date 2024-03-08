using UnityEngine;
using Fusion;

public class CrewStat : CreatureStat
{
    private int _hp;
    private int _maxHp;
    private NetworkBool _sit;

    [Networked] public int Hp { get => _hp; set { _hp = value; StatChangeAction?.Invoke(this); } }
    [Networked] public int MaxHp { get => _maxHp; set { _maxHp = value; StatChangeAction?.Invoke(this); } }

    public override void SetStat(Data.CreatureData creatureData)
    {
        base.SetStat(creatureData);

        Data.CrewData crewData = (Data.CrewData)creatureData;

        Hp = crewData.Hp;
        MaxHp = crewData.Hp;
    }

    public override float GetStatByDefine(Define.Stat stat)
    {
        switch (stat)
        {
            case Define.Stat.Speed:
                return Speed;
            case Define.Stat.Hp:
                return Hp;
            case Define.Stat.MaxHp:
                return MaxHp;
        }

        return -1;
    }

    #region Event
    public void OnDamage(int damage, int attackCount = 1)
    {
        if (damage < 0)
        {
            Debug.Log("Invalid OnDamage");
            return;
        }

        Hp = Mathf.Clamp(Hp - damage, 0, MaxHp);
    }

    public void OnHeal(int heal)
    {
        if (heal < 0)
        {
            Debug.Log("Invalid OnHeal");
            return;
        }

        Hp = Mathf.Clamp(Hp + heal, 0, MaxHp);
    }
    #endregion
}
