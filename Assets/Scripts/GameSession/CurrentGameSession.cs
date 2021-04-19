using System.Linq;
using Config;
using Player;
using Scenes.Map_Selection;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameSession
{
    public static class CurrentGameSession
    {
        public static MapCollectionScores CollectionScores =>
            _mapCollectionScores ??= new MapCollectionScores(MapCollection);

        public static MapCollection MapCollection
        {
            get
            {
                if (_mapCollection == null)
                {
                    _mapCollection = GameConfig.Instance.MapCollections[0];
#if UNITY_EDITOR
                    var mapCollectionOfCurrentScene = GameConfig.Instance.MapCollections.ToList()
                        .Find(m => m.Maps.Contains(SceneManager.GetActiveScene().name));

                    if (mapCollectionOfCurrentScene != default)
                    {
                        _mapCollection = mapCollectionOfCurrentScene;
                    }
#endif
                }

                return _mapCollection;
            }
            set
            {
                _mapCollection = value;
                _mapCollectionScores = new MapCollectionScores(value);
            }
        }

        public static Player NextRoundPlayer { get; private set; }
        public static Material WinnerMaterial { get; set; }
        public static Material LoserMaterial { get; set; }
        public static bool IsNowPassive { get; set; }

        private static Player[] _players;
        private static MapCollectionScores _mapCollectionScores;
        private static MapCollection _mapCollection;

        public static void ClearSession()
        {
            WinnerMaterial = null;
            LoserMaterial = null;
            _mapCollection = null;
            _mapCollectionScores = null;
            NextRoundPlayer = null;
            _players = null;
        }

        public static Player[] Players
        {
            get
            {
#if UNITY_EDITOR
                if (_players == null)
                {
                    Debug.LogWarning("Current Session players were not set! Trying to use default...");
                    _players = new Player[]
                    {
                        new LocalPlayer(0, 0, 0),
                        new LocalPlayer(1, 1, 1)
                    };
                }
#endif
                return _players;
            }
            set => _players = value;
        }

        public static void ResetPlayerViews()
        {
            foreach (var player in Players)
            {
                player.RoundPlayerView = null;
            }
        }

        public static Player PlayerFromPlayerView(PlayerView playerView)
        {
            return Players.FirstOrDefault(p => p.RoundPlayerView == playerView);
        }

        public static PlayerView PlayerViewFromId(int id)
        {
            return Players.FirstOrDefault(p => p.Id == id)?.RoundPlayerView;
        }

        public static void SetNextRoundPlayer(Player player)
        {
            NextRoundPlayer = player;
        }

        public static void SetNextRoundPlayer(PlayerView playerView)
        {
            if (Players != null)
            {
                NextRoundPlayer = PlayerFromPlayerView(playerView);
            }
            else
            {
                Debug.LogError("Current Session players are not set!");
            }
        }

        public static string GetNextMap(string currentMap)
        {
            var maps = MapCollection.Maps;
            return maps[(maps.ToList().IndexOf(currentMap) + 1) % maps.Length];
        }

        public static bool IsLastMapInList(string currentMap)
        {
            var maps = MapCollection.Maps;
            return maps.ToList().IndexOf(currentMap) == maps.Length - 1;
        }
    }
}