using DearTroll.Effects;
using UnityEngine;

namespace DearTroll.Services
{
    public interface IVfxService : IService
    {
        /// <summary>
        /// Instancia un VFX y lo devuelve al pool al finalizar.
        /// </summary>
        void Play(VfxData cue, Vector3 position, Quaternion rotation, Transform parent = null);
    }
}