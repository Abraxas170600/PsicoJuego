using UnityEngine;

public static class StatusSystem
{
    public static void Tick(Combatant who, StatusTiming timing, EventBus events)
    {
        for (int i = who.Statuses.Count - 1; i >= 0; i--)
        {
            var st = who.Statuses[i];
            if (st.Definition.Timing != timing) continue;

            // Aquí aplicas efectos “vivos” del estado en su timing.
            // Ej.: Poison, Burn, Regeneration, etc. (fácil de extender).
            // Para “Miedo: -2 ATK 2 turnos”, el delta ya se aplica vía GetCurrentAttack().

            st.Tick();
            if (st.RemainingTurns <= 0) { who.Statuses.RemoveAt(i); events.EmitStatusesChanged(who); }
        }
    }
}
