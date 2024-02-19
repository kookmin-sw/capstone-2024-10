using UnityEngine;

public abstract class CreatureStat
{
    public string Name { get; set; }
    public int Hp { get; set; }
    public int MaxHp { get; set; }
    public int Speed { get; set; }

    public virtual void SetStat(Data.CreatureData creatureData)
    {
        Name = creatureData.Name;
        //Hp = creatureData.Hp;
        //MaxHp = creatureData.Hp;
        //Speed = creatureData.Speed;
    }

    public void OnDamage(int damage)
    {
        int trueDamage = Mathf.Max(damage, 0);

        Hp = Mathf.Clamp(Hp - trueDamage, 0, MaxHp);
    }

    public void OnHeal(int amount)
    {
        Hp = Mathf.Clamp(Hp + amount, 0, MaxHp);
    }
}
