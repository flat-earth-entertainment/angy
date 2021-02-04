using System;
using NaughtyAttributes;
using UnityEngine;

namespace Config
{
    [Serializable]
    public class Tags
    {
        [field: SerializeField, Tag]
        public string SpawnPointTag { get; private set; }

        [field: SerializeField, Tag]
        public string SpawnPointVCamTag { get; private set; }

        [field: SerializeField, Tag]
        public string LevelOverviewVCamTag { get; private set; }

        [field: SerializeField, Tag]
        public string MapPhysicalLayoutTag { get; private set; }
    }
}