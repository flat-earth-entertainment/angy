using System;
using UnityEngine;

namespace Abilities.Config
{
    [Serializable]
    public class FireDashAbilityConfig : AbilityConfig
    {
        [field: SerializeField, Range(0, 1)]
        public float CracklingVolume { get; private set; }

        [field: SerializeField]
        public AudioClip CracklingClip { get; private set; }

        [field: SerializeField]
        public bool LaunchAfterNoInput { get; private set; }

        [field: SerializeField]
        public GameObject FireDashControlsPrefab { get; private set; }

        [field: SerializeField]
        public float EnterTime { get; private set; }

        [field: SerializeField]
        public float PushForce { get; private set; }

        [field: SerializeField]
        public float RotationTime { get; private set; }
    }
}