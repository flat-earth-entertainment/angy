using System;
using NaughtyAttributes;
using UnityEngine;

namespace Map_Selection
{
    [Serializable]
    public class MapPreview
    {
        [field: SerializeField]
        public string Name { get; private set; }

        [field: SerializeField, Scene]
        public string Scene { get; private set; }

        [field: SerializeField]
        public Sprite PreviewImage { get; private set; }
    }
}