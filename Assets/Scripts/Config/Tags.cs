using System;
using NaughtyAttributes;
using UnityEngine;

namespace Config
{
    [Serializable]
    public class Tags
    {
        [field: SerializeField]
        [field: Tag]
        public string SpawnPointTag { get; private set; }

        [field: SerializeField]
        [field: Tag]
        public string SpawnPointVCamTag { get; private set; }

        [field: SerializeField]
        [field: Tag]
        public string LevelOverviewVCamTag { get; private set; }

        [field: SerializeField]
        [field: Tag]
        public string MapPhysicalLayoutTag { get; private set; }
    }
}