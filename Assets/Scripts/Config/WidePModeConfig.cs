using System;
using UnityEngine;

namespace Config
{
    [Serializable]
    public class WidePModeConfig
    {
        [field: SerializeField]
        public KeyCode Key { get; private set; }

        [field: SerializeField]
        public float InTime { get; private set; }

        [field: SerializeField]
        public float Duration { get; private set; }

        [field: SerializeField]
        public float OutTime { get; private set; }
    }
}