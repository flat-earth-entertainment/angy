using System;
using Abilities.Config;
using UnityEngine;

namespace Config.Abilities
{
    [Serializable]
    public class NoGravityAbilityConfig : AbilityConfig
    {
        [field: SerializeField]
        public float DurationTime { get; private set; }

        [field: SerializeField]
        public Material Material { get; private set; }
    }
}