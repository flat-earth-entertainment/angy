using System;
using UnityEngine;

namespace Player
{
    [Serializable]
    public class PlayerPreset
    {
        [field: SerializeField]
        public string PlayerName { get; private set; }

        [field: SerializeField]
        public Color PlayerColor { get; private set; }

        [field: SerializeField]
        public Color FresnelColor { get; private set; }

        [field: SerializeField]
        public Gradient Gradient { get; private set; }
    }
}