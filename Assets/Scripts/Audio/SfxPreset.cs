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
        private AudioClip[] Clips { get; set; }

        [SerializeField]
        private bool limitFrequency;

        [SerializeField]
        private float minimumInterval;

        private float _frequencyTimer;
        private float _previousTime;

        public AudioClip RandomClip
        {
            get
            {
                if (limitFrequency)
                {
                    if (_frequencyTimer < minimumInterval * 2f)
                        _frequencyTimer += Time.unscaledTime - _previousTime;

                    _previousTime = Time.unscaledTime;

                    if (_frequencyTimer >= minimumInterval)
                    {
                        _frequencyTimer = 0f;
                        return Clips.RandomElement();
                    }

                    return null;
                }

                return Clips.RandomElement();
            }
        }
    }
}