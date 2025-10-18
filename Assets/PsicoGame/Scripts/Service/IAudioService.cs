using DearTroll.Audio;
using UnityEngine;

namespace DearTroll.Services
{
    public enum AudioBus { Master, Music, SFX }

    public interface IAudioService : IService
    {
        void PlayOn(AudioClipData cue, AudioSource target, bool oneShot = true, bool applyCueSettings = false);
        AudioSource PlayOn(GameObject host, AudioClipData cue, bool oneShot = true, bool addIfMissing = true, bool applyCueSettings = false);
        void PlayOneShot(AudioClipData cue, Vector3 position);
        void PlayOneShot2D(AudioClipData cue);
        void PlayMusic(AudioClipData cue, float fadeSeconds = 0.5f);
        void StopMusic(float fadeSeconds = 0.5f);
        void SetBusVolume(AudioBus bus, float volume01);
        float GetBusVolume(AudioBus bus);
    }
}