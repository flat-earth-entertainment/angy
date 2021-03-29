using System.Data;
using NaughtyAttributes;
using Player;
using Scenes.Map_Selection;
using UnityEngine;

namespace Config
{
    public class GameConfig : ScriptableObject
    {
        private const string ConfigPath = "Game Config";

        private static GameConfig _instance;

        [field: SerializeField]
        [field: BoxGroup("General")]
        public float JumpInTime { get; private set; }

        [field: SerializeField]
        [field: BoxGroup("General")]
        public float LevelOverviewTime { get; private set; }

        [field: SerializeField]
        [field: BoxGroup("Technical")]
        public GameObject PlayerPrefab { get; private set; }

        [field: SerializeField]
        [field: BoxGroup("General")]
        public float FlyToNextPlayerTime { get; private set; }

        [field: SerializeField]
        [field: BoxGroup("Angy")]
        public AngyValues AngyValues { get; private set; }

        [field: SerializeField]
        [field: BoxGroup("Angy")]
        public float SliderMoveInterval { get; private set; }

        [field: SerializeField]
        [field: BoxGroup("General")]
        public float ExplosionForceOnPlayerHit { get; private set; }

        [field: SerializeField]
        [field: BoxGroup("General")]
        public float CameraPanningSpeed { get; private set; }

        [field: SerializeField]
        [field: BoxGroup("Technical")]
        public Tags Tags { get; private set; }

        [field: SerializeField]
        public PlayerPreset[] PlayerPresets { get; private set; }

        [field: SerializeField]
        public MapCollection[] MapCollections { get; private set; }

        [field: SerializeField]
        [field: BoxGroup("General")]
        public float PreNextTurnDelay { get; private set; }

        [field: SerializeField]
        [field: BoxGroup("General")]
        public float TimeScale { get; private set; }

        [field: SerializeField]
        [field: BoxGroup("General")]
        public float HoleOrbitTime { get; private set; }

        [field: SerializeField]
        [field: BoxGroup("Mechanics")]
        public AbilityValues AbilityValues { get; private set; }

        [field: SerializeField]
        [field: BoxGroup("Technical")]
        public GameObject AudioManager { get; private set; }

        [field: SerializeField]
        [field: BoxGroup("Technical")]
        public GameObject OptionsController { get; private set; }

        [field: SerializeField]
        [field: BoxGroup("Technical")]
        public ScenesSetup Scenes { get; private set; }

        [field: SerializeField]
        [field: BoxGroup("Mechanics")]
        public HitStopValues HitStop { get; private set; }

        [field: SerializeField]
        [field: BoxGroup("Technical")]
        public GameObject SceneChanger { get; private set; }

        [field: SerializeField]
        [field: BoxGroup("Technical")]
        public GameObject PauseMenu { get; private set; }

        [field: SerializeField]
        [field: BoxGroup("Technical")]
        public GameObject BloodSplat { get; private set; }

        [field: SerializeField]
        [field: BoxGroup("Mechanics")]
        public WidePModeConfig WidePMode { get; private set; }


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
    }
}