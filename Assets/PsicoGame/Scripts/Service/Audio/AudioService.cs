using System;
using DearTroll.Audio;
using UnityEngine;
using UnityEngine.Audio;
using Object = UnityEngine.Object;

namespace DearTroll.Services
{
    public class AudioService : IAudioService, IInitializable, ITeardown
    {
        [System.Serializable]
        public struct Settings
        {
            public int sfxPoolSize;
            public int musicSources;
        }

        private readonly Settings _settings;
        private readonly GameObject _root;
        private IPool<AudioSource> _sfxPool;
        private AudioSource[] _music;
        private AudioClipData _currentMusic;

        private float _master = 1f, _musicVol = 1f, _sfxVol = 1f;

        public AudioService(Settings settings, GameObject contextRoot)
        {
            _settings = settings;
            _root = new GameObject("AudioService");
            _root.transform.SetParent(contextRoot.transform);
        }

        public void Initialize()
        {
            var poolParent = new GameObject("SFXPool");
            poolParent.transform.SetParent(_root.transform);

            _sfxPool = new ServiceObjectPool<AudioSource>(
            factory: () =>
            {
                var go = new GameObject("SFXSource");
                go.transform.SetParent(poolParent.transform);
                var src = go.AddComponent<AudioSource>();
                src.playOnAwake = false;
                return src;
            },
            onGet: s => { s.gameObject.SetActive(true); },
            onRelease: s => { s.Stop(); s.clip = null; s.gameObject.SetActive(false); },
            initial: Mathf.Max(4, _settings.sfxPoolSize),
            maxSize: Mathf.Max(8, _settings.sfxPoolSize * 2)
            );

            _music = new AudioSource[Mathf.Max(2, _settings.musicSources)];
            for (int i = 0; i < _music.Length; i++)
            {
                var go = new GameObject($"MusicSource_{i}");
                go.transform.SetParent(_root.transform);
                var src = go.AddComponent<AudioSource>();
                src.loop = true; src.playOnAwake = false; src.spatialBlend = 0f;
                _music[i] = src;
            }
        }

        public void Teardown()
        {
            if (_root != null) Object.Destroy(_root);
        }

        public void PlayOn(AudioClipData cue, AudioSource target, bool oneShot = true, bool applyCueSettings = true)
        {
            if (cue == null || cue.GetAudioClip() == null || target == null) return;

            if (oneShot)
            {
                target.PlayOneShot(cue.GetAudioClip());
                return;
            }

            if (applyCueSettings)
            {
                ApplyCue(target, cue, !oneShot);
            }
            else
            {
                target.clip = cue.GetAudioClip();
            }
            target.Play();
        }

        public AudioSource PlayOn(GameObject host, AudioClipData cue, bool oneShot = true, bool addIfMissing = true, bool applyCueSettings = true)
        {
            if (!host) return null;
            var src = host.GetComponent<AudioSource>();
            if (!src && addIfMissing) src = host.AddComponent<AudioSource>();
            if (!src) return null;

            PlayOn(cue, src, oneShot, applyCueSettings);
            return src;
        }

        public void PlayOneShot(AudioClipData cue, Vector3 position)
        {
            if (cue == null || cue.GetAudioClip() == null) return;
            var src = _sfxPool.Get();
            src.transform.position = position;
            ApplyCue(src, cue, isMusic: false);
            src.Play();
            _root.GetComponentInParent<MonoBehaviourHelper>().StartCoroutine(ReturnWhenDone(src));
        }

        public void PlayOneShot2D(AudioClipData cue)
        {
            if (cue == null || cue.GetAudioClip() == null) return;
            var src = _sfxPool.Get();
            src.transform.localPosition = Vector3.zero;
            ApplyCue(src, cue, isMusic: false);
            src.spatialBlend = 0f;
            src.Play();
            _root.GetComponentInParent<MonoBehaviourHelper>().StartCoroutine(ReturnWhenDone(src));
        }

        public void PlayMusic(AudioClipData cue, float fadeSeconds = 0.5f)
        {
            if (cue == null || cue.GetAudioClip() == null || CheckCurrentMusic(cue)) return;
            _currentMusic = cue;
            var next = GetInactiveMusicSource();
            ApplyCue(next, cue, isMusic: true);
            next.volume = 0f;
            next.Play();
            // crossfade
            _root.GetComponentInParent<MonoBehaviourHelper>().StartCoroutine(Crossfade(next, fadeSeconds));
        }

        public void StopMusic(float fadeSeconds = 0.5f)
        {
            foreach (var m in _music)
            {
                if (m.isPlaying)
                    _root.GetComponentInParent<MonoBehaviourHelper>().StartCoroutine(FadeOutAndStop(m, fadeSeconds));
            }
        }

        public void SetBusVolume(AudioBus bus, float volume01)
        {
            volume01 = Mathf.Clamp01(volume01);
            switch (bus)
            {
                case AudioBus.Master: _master = volume01; break;
                case AudioBus.Music: _musicVol = volume01; break;
                case AudioBus.SFX: _sfxVol = volume01; break;
            }
            ApplyVolumes();
        }

        public float GetBusVolume(AudioBus bus) => bus switch
        {
            AudioBus.Master => _master,
            AudioBus.Music => _musicVol,
            _ => _sfxVol
        };

        private void ApplyCue(AudioSource src, AudioClipData cue, bool isMusic)
        {
            src.clip = cue.GetAudioClip();
            src.pitch = cue.GetPitchOffset();
        }

        private AudioSource GetInactiveMusicSource()
        {
            foreach (var m in _music) if (!m.isPlaying) return m;
            return _music[0];
        }

        private System.Collections.IEnumerator Crossfade(AudioSource next, float seconds)
        {
            float t = 0f;
            var active = new System.Collections.Generic.List<AudioSource>();
            foreach (var a in _music) if (a != next && a.isPlaying) active.Add(a);
            while (t < seconds)
            {
                t += Time.unscaledDeltaTime;
                float k = seconds <= 0.0001f ? 1f : Mathf.Clamp01(t / seconds);
                next.volume = k * next.volume;
                foreach (var a in active) a.volume = (1f - k) * a.volume;
                yield return null;
            }
            foreach (var a in active) a.Stop();
            ApplyVolumes();
        }

        private bool CheckCurrentMusic(AudioClipData cue)
        {
            if (_currentMusic == cue) return true;
            return false;
        }

        private System.Collections.IEnumerator FadeOutAndStop(AudioSource a, float seconds)
        {
            float start = a.volume;
            float t = 0f;
            while (t < seconds)
            {
                t += Time.unscaledDeltaTime;
                float k = seconds <= 0.0001f ? 1f : Mathf.Clamp01(t / seconds);
                a.volume = Mathf.Lerp(start, 0f, k);
                yield return null;
            }
            a.Stop();
            ApplyVolumes();
        }

        private System.Collections.IEnumerator ReturnWhenDone(AudioSource s)
        {
            yield return new WaitWhile(() => s.isPlaying);
            _sfxPool.Release(s);
        }

        private void ApplyVolumes()
        {
            foreach (var m in _music)
            {
                if (!m.clip) continue;
                m.volume = (m.volume <= 0f ? 1f : m.volume);
                m.volume = _master * _musicVol * (m.clip ? 1f : 0f);
            }
        }
    }
}