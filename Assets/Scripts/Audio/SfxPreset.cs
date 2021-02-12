using System;
using UnityEngine;
using Utils;

namespace Audio
{
    [Serializable]
    public class SfxPreset
    {
        [field: SerializeField]
        public SfxType SfxType { get; private set; }

        [field: SerializeField]
        public AudioClip[] Clips { get; private set; }

        public AudioClip RandomClip => Clips.RandomElement();
    }
}