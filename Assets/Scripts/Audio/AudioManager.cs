using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Audio
{
    public class AudioManager : MonoBehaviour, IAudioManager
    {
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private List<AudioData> _sounds;

        private Dictionary<SoundType, AudioClip[]> _soundsLookup = new();
    
        private void Awake()
        {
            _soundsLookup = _sounds.ToDictionary(key => key.Type, value => value.Clips);
        }

        public void PlaySound(SoundType soundType)
        {
            var audioClip = GetRandomClip(soundType);
            _audioSource.PlayOneShot(audioClip);
        }

        private AudioClip GetRandomClip(SoundType soundType)
        {
            var clips = _soundsLookup[soundType];
            var randomIndex = Random.Range(0, clips.Length);
            return clips[randomIndex];
        }
    }
}
