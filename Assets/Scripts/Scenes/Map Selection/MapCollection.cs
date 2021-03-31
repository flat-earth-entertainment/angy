using System;
using NaughtyAttributes;
using UnityEngine;

namespace Scenes.Map_Selection
{
    [Serializable]
    public class MapCollection
    {
        [field: SerializeField]
        public string Name { get; private set; }

        [field: SerializeField]
        [field: Scene]
        public string[] Maps { get; private set; }

        [field: SerializeField]
        public Sprite PreviewImage { get; private set; }
    }
}