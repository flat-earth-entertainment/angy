using System;
using NaughtyAttributes;
using UnityEngine;

namespace Config
{
    [Serializable]
    public class ScenesSetup
    {
        [field: SerializeField, Scene]
        public string LeaderboardScene { get; private set; }

        [field: SerializeField, Scene]
        public string MainMenuScene { get; private set; }
    }
}