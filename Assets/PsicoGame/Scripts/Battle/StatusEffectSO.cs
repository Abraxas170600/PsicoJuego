using UnityEngine;
public enum StatusTiming { OnTurnStartPlayer, OnTurnEndPlayer, OnTurnStartMonster, OnTurnEndMonster }
public enum StatusSign { Positive, Negative, Neutral }

[CreateAssetMenu(menuName = "Scriptable Objects/StatusEffect")]
public class StatusEffectSO : ScriptableObject
{
    public string Id;
    public string DisplayName;
    public StatusSign Sign;
    public StatusTiming Timing;
    public bool Stackable;
    public int MaxStacks = 1;

    // Lógica del estado (data-driven + callbacks)
    public int flatAttackDelta;   // p.ej., Miedo: -2 ataque
    public bool doubleNextCardNegative; // p.ej., Culpa
}
