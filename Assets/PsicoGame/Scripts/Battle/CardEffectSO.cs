using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Effect")]
public class CardEffectSO : ScriptableObject
{
    public EffectType Type;
    public TargetType Target;
    public int Magnitude;
    public StatusEffectSO Status; // solo si Type = ApplyStatus
    public int DurationTurns;     // estados
}
