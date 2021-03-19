using System;
using UnityEngine;

namespace Abilities.Config
{
    [Serializable]
    public class FireDashAbilityConfig : AbilityConfig
    {
        [field: SerializeField]
        public GameObject FireDashControlsPrefab { get; private set; }

        [field: SerializeField]
        public float InputWaitTime { get; private set; }

        [field: SerializeField]
        public float EnterTime { get; private set; }

        [field: SerializeField]
        public float PushForce { get; private set; }
    }
}