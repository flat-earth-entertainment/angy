using System;
using NaughtyAttributes;
using UnityEngine;

namespace Config
{
    [Serializable]
    public class ScenesSetup
    {
        [field: SerializeField]
        [field: Scene]
        public string LeaderboardScene { get; private set; }

        [field: SerializeField]
        [field: Scene]
        public string MainMenuScene { get; private set; }

        [field: SerializeField]
        [field: Scene]
        public string VictoryScene { get; private set; }
    }
}