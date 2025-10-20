using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Settings")]
public class CombatSettingsSO : ScriptableObject
{
    [Header("Player")]
    public float playerCardWindup = 0.25f;    // tiempo antes de resolver (animación de cargar)
    public float playerResolvePause = 0.35f;  // mostrar números, sacudidas, etc.

    [Header("Monster")]
    public float monsterWindup = 0.35f;       // anticipación del golpe enemigo
    public float monsterHitPause = 0.25f;     // pausa al impactar
    public float monsterStatusPause = 0.25f;  // mostrar badge del estado aplicado

    [Header("Turn")]
    public float betweenPhases = 0.25f;       // respiro entre fases
}
