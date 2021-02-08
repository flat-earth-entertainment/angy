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
    }
}