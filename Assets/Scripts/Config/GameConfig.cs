using System.Data;
using Player;
using UnityEngine;

namespace Config
{
    public class GameConfig : ScriptableObject
    {
        [field: SerializeField]
        public float JumpInTime { get; private set; }

        [field: SerializeField]
        public float LevelOverviewTime { get; private set; }

        [field: SerializeField]
        public GameObject PlayerPrefab { get; private set; }

        [field: SerializeField]
        public float FlyToNextPlayerTime { get; private set; }

        [field: SerializeField]
        public AngyValues AngyValues { get; private set; }


        private const string ConfigPath = "Game Config";

        public static GameConfig Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = Resources.Load<GameConfig>(ConfigPath);

                    if (_instance == null)
                    {
                        throw new DataException(
                            "Can't find Game Config! Please check that it exists and is located at: Assets/Resources/" +
                            ConfigPath);
                    }
                }

                return _instance;
            }
        }

        private static GameConfig _instance;
    }
}