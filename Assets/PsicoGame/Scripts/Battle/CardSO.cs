using System.Collections.Generic;
using UnityEngine;

public enum EffectPhase { Cost, Positive, Damage, Negative, After }
public enum TargetType { Self, Enemy, Both }
public enum EffectType { Heal, ModifyAttack, ModifyDefense, DirectDamage, ApplyStatus }

[CreateAssetMenu(fileName = "Card", menuName = "Scriptable Objects/Card")]
public class CardSO : ScriptableObject
{
    public string Id;
    public string DisplayName;
    public Sprite Icon;
    public int BaseDamage;
    public List<CardEffectSO> PositiveEffects;
    public List<CardEffectSO> NegativeEffects;
    public bool ApplyNegativesOnSelf = true;
}
