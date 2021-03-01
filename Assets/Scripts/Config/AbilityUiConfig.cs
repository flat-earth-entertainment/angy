using System;
using UnityEngine;

namespace Config
{
    [Serializable]
    public class AbilityUiConfig
    {
        [field: SerializeField]
        public float WobbleScale { get; private set; }

        [field: SerializeField]
        public float WobbleInterval { get; private set; }
    }
}