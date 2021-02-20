using System;
using NaughtyAttributes;
using UnityEngine;

namespace Map_Selection
{
    [Serializable]
    public class MapCollection
    {
        [field: SerializeField]
        public string Name { get; private set; }

        [field: SerializeField, Scene]
        public string[] Maps { get; private set; }

        [field: SerializeField]
        public Sprite PreviewImage { get; private set; }
    }
}