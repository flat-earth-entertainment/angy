using System;
using Abilities;
using UnityEngine;

namespace Config
{
    [Serializable]
    public class AbilityValues
    {
        [field: SerializeField]
        public NoGravityAbility NoGravityAbility { get; private set; }

        [field: SerializeField]
        public ExpandAbility ExpandAbility { get; private set; }
    }
}