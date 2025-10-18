using DearTroll.Effects;
using System.Collections.Generic;
using UnityEngine;

namespace DearTroll.Services
{
    public sealed class VfxService : IVfxService, IInitializable, ITeardown
    {
        [System.Serializable]
        public struct Settings
        {
            public int initialPool;
            public int maxPool;
        }

        private readonly Settings _settings;
        private readonly GameObject _root;
        private readonly Dictionary<GameObject, IPool<GameObject>> _pools = new();
        private MonoBehaviour _runner;

        internal sealed class PooledVfxMeta : MonoBehaviour
        {
            public GameObject prefabKey;
        }

        public VfxService(Settings settings, GameObject contextRoot)
        {
            _settings = settings;
            _root = new GameObject("VfxService");
            _root.transform.SetParent(contextRoot.transform);
            _runner = contextRoot.GetComponent<MonoBehaviourHelper>()
                     ?? contextRoot.AddComponent<MonoBehaviourHelper>();
        }

        public void Initialize() { }

        public void Teardown()
        {
            if (_root != null) Object.Destroy(_root);
        }

        public void Play(VfxData cue, Vector3 position, Quaternion rotation, Transform parent = null)
        {
            if (!cue || !cue.prefab) return;

            var pool = GetOrCreatePool(cue);
            var go = pool.Get();

            var t = go.transform;
            t.SetParent(parent ? parent : _root.transform, false);
            t.SetPositionAndRotation(position, rotation);

            RewindAndPlay(go);

            var duration = ResolveDuration(go, cue.autoReleaseSeconds);
            _runner.StartCoroutine(ReleaseAfter(go, cue.prefab, duration));
        }

        private IPool<GameObject> GetOrCreatePool(VfxData cue)
        {
            var prefab = cue.prefab;
            if (_pools.TryGetValue(prefab, out var found)) return found;

            var pool = new ServiceObjectPool<GameObject>(
                factory: () =>
                {
                    var go = Object.Instantiate(prefab, _root.transform);
                    go.name = $"{prefab.name}_Pooled";
                    var meta = go.GetComponent<PooledVfxMeta>() ?? go.AddComponent<PooledVfxMeta>();
                    meta.prefabKey = prefab;
                    go.SetActive(false);
                    return go;
                },
                onGet: go =>
                {
                    go.SetActive(true);
                },
                onRelease: go =>
                {
                    var psArr = go.GetComponentsInChildren<ParticleSystem>(true);
                    foreach (var ps in psArr)
                        ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
#if UNITY_VISUAL_EFFECT_GRAPH
                    var vfxArr = go.GetComponentsInChildren<UnityEngine.VFX.VisualEffect>(true);
                    foreach (var v in vfxArr)
                    {
                        v.Stop();
                        v.Reinit();
                    }
#endif
                    var tr = go.transform;
                    tr.SetParent(_root.transform, false);
                    tr.localPosition = Vector3.zero;
                    tr.localRotation = Quaternion.identity;
                    go.SetActive(false);
                },
                initial: Mathf.Clamp(_settings.initialPool, 0, Mathf.Max(0, _settings.maxPool)),
                maxSize: Mathf.Max(1, _settings.maxPool)
            );

            _pools[prefab] = pool;
            return pool;
        }

        private void RewindAndPlay(GameObject go)
        {
            var psArr = go.GetComponentsInChildren<ParticleSystem>(true);
            foreach (var ps in psArr)
            {
                ps.Simulate(0f, true, true);
                ps.Play(true);
            }
#if UNITY_VISUAL_EFFECT_GRAPH
            var vfxArr = go.GetComponentsInChildren<UnityEngine.VFX.VisualEffect>(true);
            foreach (var v in vfxArr)
            {
                v.Reinit();
                v.Play();
            }
#endif
        }

        private float ResolveDuration(GameObject go, float fallback)
        {
            if (fallback > 0f) return fallback;
            float maxDur = 0f;
            var psArr = go.GetComponentsInChildren<ParticleSystem>(true);
            foreach (var ps in psArr)
            {
                var main = ps.main;
                if (main.loop)
                {
                    maxDur = Mathf.Max(maxDur, 3f);
                    continue;
                }
                float dur = main.duration;
                var sl = main.startLifetime;
                float life = sl.mode switch
                {
                    ParticleSystemCurveMode.TwoConstants => Mathf.Max(sl.constantMin, sl.constantMax),
                    ParticleSystemCurveMode.TwoCurves => Mathf.Max(sl.curveMax.Evaluate(1f), sl.curveMin.Evaluate(1f)),
                    _ => sl.constant
                };
                maxDur = Mathf.Max(maxDur, dur + life);
            }
#if UNITY_VISUAL_EFFECT_GRAPH
            // Para VFX Graph no hay API directa de duración; estimamos 3s si no hay partículas.
            var vfx = go.GetComponentsInChildren<UnityEngine.VFX.VisualEffect>(true);
            if (vfx.Length > 0) maxDur = Mathf.Max(maxDur, 3f);
#endif
            return maxDur > 0.01f ? maxDur : 3f;
        }

        private System.Collections.IEnumerator ReleaseAfter(GameObject go, GameObject prefabKey, float seconds)
        {
            yield return new WaitForSeconds(seconds);
            if (go == null) yield break;

            if (_pools.TryGetValue(prefabKey, out var pool))
                pool.Release(go);
            else
                Object.Destroy(go);
        }
    }
}