using UnityEngine;

public static class MonsterAI
{
    public static void ExecuteBasic(MonsterCombatant m, PlayerCombatant p, EventBus events)
    {
        // 1) Ataque: daño base del monstruo + su ataque
        int baseDamage = m.Stats.Attack + 6; // ajusta por nivel
        p.ApplyDamage(baseDamage);
        events.EmitDamage(m, p, baseDamage);

        // 2) Aplica un estado negativo (ej.: Miedo: -2 ATK por 2 turnos)
        var miedo = new StatusEffectSO
        {
            Id = "fear",
            DisplayName = "Miedo",
            Sign = StatusSign.Negative,
            Timing = StatusTiming.OnTurnEndPlayer,
            Stackable = false,
            MaxStacks = 1,
            flatAttackDelta = -2
        };
        p.Statuses.Add(new RuntimeStatus(miedo, duration: 2));
        events.EmitStatusApplied(p, miedo, 2);
    }
}
