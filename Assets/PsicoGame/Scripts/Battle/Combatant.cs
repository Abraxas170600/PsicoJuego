using System.Collections.Generic;
using UnityEngine;

public class Stats
{
    public int MaxHP;
    public int HP;
    public int Attack;
    public int Defense;
}

public abstract class Combatant
{
    public string Name;
    public Sprite Portrait;
    public Stats Stats;
    public List<RuntimeStatus> Statuses = new();

    public virtual int GetTurnDefenseBonus() => 0;
    public int GetCurrentAttack()
    {
        int mod = 0;
        foreach (var s in Statuses) mod += s.FlatAttackDelta();
        return Stats.Attack + mod;
    }

    //public int GetCurrentDefense()
    //{
    //    int mod = 0;
    //    foreach (var s in Statuses) mod += s.FlatDefenseDelta(); // si usas DEF en estados
    //    return Stats.Defense + mod + GetTurnDefenseBonus();
    //}

    public void ApplyDamage(int incoming)
    {
        int mitigated = System.Math.Max(0, incoming - Stats.Defense);
        Stats.HP = System.Math.Max(0, Stats.HP - mitigated);
    }
}

public class PlayerCombatant : Combatant 
{
    public int TempDefenseThisTurn = 0;
    public override int GetTurnDefenseBonus() => TempDefenseThisTurn;
    public void ResetTurnTemps() { TempDefenseThisTurn = 0; }
}
public class MonsterCombatant : Combatant { }
