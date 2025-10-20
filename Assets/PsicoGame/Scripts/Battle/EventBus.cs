using System;

public class EventBus
{
    public event Action<CardSO> OnPlayerCardPlayStarted; // UI: anim del panel del jugador y la carta
    public event Action OnPlayerCardResolved;            // UI: fin de efectos de la carta

    public event Action OnMonsterAttackStarted;          // UI: anticipación del monstruo
    public event Action<int> OnMonsterAttackHit;         // UI: impacto y daño mostrado
    public event Action<string, int> OnMonsterStatusShown;// UI: badge/tooltip del estado

    public event Action<Combatant, Combatant, int> OnDamage;
    public event Action<Combatant, int> OnHeal;

    public event Action<Combatant> OnHpChanged;
    public event System.Action<Combatant, StatusEffectSO, int> OnStatusApplied;
    public event Action<Combatant> OnStatusesChanged;
    public event Action OnHandChanged;
    public event Action<string> OnLog;

    public void EmitPlayerCardPlayStarted(CardSO c) => OnPlayerCardPlayStarted?.Invoke(c);
    public void EmitPlayerCardResolved() => OnPlayerCardResolved?.Invoke();

    public void EmitMonsterAttackStarted() => OnMonsterAttackStarted?.Invoke();
    public void EmitMonsterAttackHit(int dmg) => OnMonsterAttackHit?.Invoke(dmg);
    public void EmitMonsterStatusShown(string name, int turns) => OnMonsterStatusShown?.Invoke(name, turns);
    public void EmitDamage(Combatant src, Combatant dst, int amount)
    { OnDamage?.Invoke(src, dst, amount); OnHpChanged?.Invoke(dst); }

    public void EmitHeal(Combatant who, int amount)
    { OnHeal?.Invoke(who, amount); OnHpChanged?.Invoke(who); }

    public void EmitStatusApplied(Combatant who, StatusEffectSO status, int turns)
    {
        OnStatusApplied?.Invoke(who, status, turns);
        OnStatusesChanged?.Invoke(who);
    }

    public void EmitStatusesChanged(Combatant who) => OnStatusesChanged?.Invoke(who); // por si quitas/expiras
    public void EmitHandChanged() => OnHandChanged?.Invoke();
    public void Log(string msg) => OnLog?.Invoke(msg);
}
