using System;
using UnityEngine;

namespace Config.Abilities
{
    [Serializable]
    public class IceBlockAbilityConfig
    {
        [field: SerializeField]
        public float Drag { get; private set; }

        [field: SerializeField]
        public Material IceMaterial { get; private set; }
    }
}