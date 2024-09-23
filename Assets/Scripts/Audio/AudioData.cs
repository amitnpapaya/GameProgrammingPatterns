using System;
using UnityEngine;

namespace Audio
{
    [Serializable]
    public class AudioData
    {
        public SoundType Type;
        public AudioClip[] Clips;
    }
}