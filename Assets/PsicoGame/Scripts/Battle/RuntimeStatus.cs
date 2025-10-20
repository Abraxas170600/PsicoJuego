using UnityEngine;

public class RuntimeStatus
{
    public StatusEffectSO Definition;
    public int RemainingTurns;
    public int Stacks = 1;

    public RuntimeStatus(StatusEffectSO def, int duration)
    {
        Definition = def;
        RemainingTurns = duration;
    }

    public int FlatAttackDelta()
    {
        return Definition.flatAttackDelta * Stacks;
    }

    public void Tick() { RemainingTurns--; }
}
