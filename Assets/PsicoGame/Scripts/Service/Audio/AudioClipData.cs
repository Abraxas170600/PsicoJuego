using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace DearTroll.Audio
{
    [CreateAssetMenu(fileName = "AudioClip", menuName = "Scriptable Objects/AudioClip")]
    public class AudioClipData : ScriptableObject
    {
        [SerializeField] private List<AudioClip> _audioClips;
        [SerializeField, Range(-3f, 3f)] private float _pitchBase = 1f;
        [SerializeField] private float _pitchVariation = 0f;
        [SerializeField] private AudioMixerGroupType type = AudioMixerGroupType.World3D;

        public AudioMixerGroupType Type => type;

        public AudioClip GetAudioClip()
        {
            return _audioClips[Random.Range(0, _audioClips.Count)];
        }

        public float GetPitchOffset()
        {
            float pitchVariationHalf = _pitchVariation / 2f;
            return _pitchBase + Random.Range(-pitchVariationHalf, pitchVariationHalf);
        }

        public void SetAudioClip(AudioClip audioClip)
        {
            _audioClips.Clear();
            _audioClips.Add(audioClip);
        }
    }

    public enum AudioMixerGroupType
    {
        None = 0,
        Music = 1,
        UI = 2,
        World3D = 3,
    }
}