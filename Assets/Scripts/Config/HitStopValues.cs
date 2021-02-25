using System;
using Cinemachine;
using UnityEngine;

namespace Config
{
    [Serializable]
    public class HitStopValues
    {
        [field: SerializeField]
        public float ZoomInTime { get; private set; }

        [field: SerializeField]
        public float TimeScale { get; private set; }

        [field: SerializeField]
        public float StayTime { get; private set; }

        [field: SerializeField]
        public float ZoomOutTime { get; private set; }

        [field: SerializeField]
        public float HitStopTriggerImpulse { get; private set; }

        [field: SerializeField]
        public NoiseSettings HitStopNoiseSettings { get; private set; }

        [field: SerializeField]
        public GameObject ImpactParticle { get; private set; }
    }
}