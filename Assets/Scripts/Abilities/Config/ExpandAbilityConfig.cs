using System;
using Abilities.Config;
using UnityEngine;

namespace Config.Abilities
{
    [Serializable]
    public class ExpandAbilityConfig : AbilityConfig
    {
        [field: SerializeField]
        public uint KnockbackMultiplier { get; private set; }

        [field: SerializeField]
        public float TimeToInflate { get; private set; }

        [field: SerializeField]
        public float TimeToDeflate { get; private set; }

        [field: SerializeField]
        public float Duration { get; private set; }

        [field: SerializeField]
        public float Scale { get; private set; }
    }
}