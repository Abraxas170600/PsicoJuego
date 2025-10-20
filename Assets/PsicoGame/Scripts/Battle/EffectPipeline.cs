

using System.Collections.Generic;
using UnityEngine;

public class EffectContext
{
    public PlayerCombatant Player;
    public MonsterCombatant Monster;
    public CardSO Card;
    public EventBus Events;
    public int AttackDeltaThisCard;
    public bool DoubleNextCardNegative; // p.ej., por “Culpa”
}

public static class EffectPipeline
{
    public static void ResolveCard(EffectContext ctx)
    {
        // 1) Cost (si existiera)
        // 2) Positivos
        ctx.AttackDeltaThisCard = SumInstantAtk(ctx.Card.PositiveEffects)
                         + SumInstantAtk(ctx.Card.NegativeEffects);

        ApplyEffects(ctx.Card.PositiveEffects, ctx, targetSelf: true);

        //// 3) Negativos sobre el jugador (por defecto)
        int multiplier = ctx.DoubleNextCardNegative ? 2 : 1;
        ApplyEffects(ctx.Card.NegativeEffects, ctx, targetSelf: true, magnitudeMultiplier: multiplier);
        ctx.DoubleNextCardNegative = false; // se consume

        // 4) Daño base
        int finalDamage = 0;

        if (ctx.Card.BaseDamage > 0)
        {
            finalDamage = ctx.Card.BaseDamage + ctx.Player.GetCurrentAttack() + ctx.AttackDeltaThisCard;
            ctx.Monster.ApplyDamage(finalDamage);
            ctx.Events.EmitDamage(ctx.Player, ctx.Monster, finalDamage);
        }

        // 5) After/Triggers (extensible)
    }

    private static int SumInstantAtk(List<CardEffectSO> effects)
    {
        int sum = 0;
        foreach (var e in effects)
            if (e.Type == EffectType.ModifyAttack && e.DurationTurns == 0) sum += e.Magnitude;
        return sum;
    }

    private static void ApplyEffects(
        List<CardEffectSO> effects,
        EffectContext ctx,
        bool targetSelf,
        int magnitudeMultiplier = 1)
    {
        foreach (var e in effects)
        {
            int mag = e.Magnitude * magnitudeMultiplier;

            switch (e.Type)
            {
                case EffectType.Heal:
                    ctx.Player.Stats.HP = System.Math.Min(ctx.Player.Stats.MaxHP, ctx.Player.Stats.HP + mag);
                    ctx.Events.EmitHeal(ctx.Player, mag);
                    break;

                case EffectType.ModifyAttack:
                    //if (e.DurationTurns > 0)
                    //{
                    // Si quisieras soporte a futuro; por ahora el usuario NO lo quiere
                    //ctx.Player.Statuses.Add(new RuntimeStatus(
                    //    new StatusEffectSO { DisplayName = (mag >= 0 ? "+" : "") + mag + " ATK", flatAttackDelta = mag },
                    //    e.DurationTurns));
                    //ctx.Events.EmitStatusesChanged(ctx.Player);
                    //}
                    //else
                    //{
                    // EFECTO INSTANTÁNEO: SOLO para el daño de ESTA carta
                    if (e.Type == EffectType.ModifyAttack && e.DurationTurns == 0)
                        continue; // ya se contó en AttackDeltaThisCard
                    ctx.AttackDeltaThisCard += mag;
                    //}
                    break;

                case EffectType.ModifyDefense:
                    //if (e.DurationTurns > 0)
                    //{
                        //ctx.Player.Statuses.Add(new RuntimeStatus(
                        //    new StatusEffectSO { DisplayName = (mag >= 0 ? "+" : "") + mag + " DEF", flatDefenseDelta = mag },
                        //    e.DurationTurns));
                        //ctx.Events.EmitStatusesChanged(ctx.Player);
                    //}
                    //else
                    //{
                        // EFECTO DE ESTE TURNO: proteger vs el ataque del monstruo
                        ctx.Player.TempDefenseThisTurn += mag;
                        ctx.Events.EmitStatusesChanged(ctx.Player); // opcional si muestras DEF temporal en algún lado
                    //}
                    break;

                case EffectType.DirectDamage:
                    ctx.Monster.ApplyDamage(mag);
                    ctx.Events.EmitDamage(ctx.Player, ctx.Monster, mag);
                    break;

                case EffectType.ApplyStatus:
                    if (e.Status != null)
                    {
                        var runtime = new RuntimeStatus(e.Status, e.DurationTurns);
                        ctx.Player.Statuses.Add(runtime);
                        ctx.Events.EmitStatusApplied(ctx.Player, e.Status, e.DurationTurns);
                    }
                    break;
            }
        }
    }
}
