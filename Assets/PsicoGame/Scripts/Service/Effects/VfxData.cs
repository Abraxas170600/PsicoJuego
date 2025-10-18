using UnityEngine;

namespace DearTroll.Effects
{
    [CreateAssetMenu(fileName = "vfxData", menuName = "Scriptable Objects/vfxData")]
    public sealed class VfxData : ScriptableObject
    {
        public GameObject prefab; // ParticleSystem o VisualEffect
        public float autoReleaseSeconds = -1f; // si <=0, intentará detectar duración
    }
}