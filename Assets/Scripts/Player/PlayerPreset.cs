using System;
using UnityEngine;

namespace Player
{
    [Serializable]
    public class PlayerPreset
    {
        [field: SerializeField]
        public string PlayerName { get; set; }

        [field: SerializeField]
        public Color PlayerColor { get; set; }

        [field: SerializeField]
        public Color FresnelColor { get; set; }

        [field: SerializeField]
        public Gradient Gradient { get; set; }

        [field: SerializeField]
        public GameObject Trail { get; set; }

        [field: SerializeField]
        public Material FireMaterial { get; set; }
    }
}