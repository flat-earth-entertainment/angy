using System;
using UnityEngine;

namespace Abilities.Config
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