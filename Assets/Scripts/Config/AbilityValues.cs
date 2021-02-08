using System;
using Config.Abilities;
using UnityEngine;

namespace Config
{
    [Serializable]
    public class AbilityValues
    {
        [field: SerializeField]
        public NoGravityAbilityConfig NoGravityAbility { get; private set; }

        [field: SerializeField]
        public ExpandAbilityConfig ExpandAbility { get; private set; }

        [field: SerializeField]
        public IceBlockAbilityConfig IceBlockAbilityConfig { get; private set; }
    }
}