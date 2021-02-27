using System;
using Abilities.Config;
using UnityEngine;

namespace Config.Abilities
{
    [Serializable]
    public class IceBlockAbilityConfig : AbilityConfig
    {
        [field: SerializeField]
        public float Drag { get; private set; }

        [field: SerializeField]
        public Material IceMaterial { get; private set; }
    }
}