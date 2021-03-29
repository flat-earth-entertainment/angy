using System;
using UnityEngine;

namespace Abilities.Config
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