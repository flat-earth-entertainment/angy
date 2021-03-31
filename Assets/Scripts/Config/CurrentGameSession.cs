using System;
using System.Linq;
using Scenes.Map_Selection;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Config
{
    public class MapCollectionScores
    {
        private readonly MapCollection _mapCollection;

        public MapCollectionScores(MapCollection mapCollection)
        {
            _mapCollection = mapCollection;
            Scores = new MapScore[mapCollection.Maps.Length];
        }

        public MapScore[] Scores { get; }

        public void SetMapScore(string mapSceneName, MapScore mapScore)
        {
            if (_mapCollection.Maps.Contains(mapSceneName))
            {
                var mapIndex = Array.IndexOf(_mapCollection.Maps, mapSceneName);
                Scores[mapIndex] = mapScore;
            }
        }
    }

    public readonly struct MapScore
    {
        public readonly int? Player1Score;
        public readonly int? Player2Score;

        public MapScore(int player1Score, int player2Score)
        {
            Player1Score = player1Score;
            Player2Score = player2Score;
        }
    }

    public static class CurrentGameSession
    {
        private static MapCollectionScores _mapCollectionScores;

        private static MapCollection _mapCollection;

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

        public static int? NextRoundRewiredPlayerId { get; set; }

        public static Material WinnerMaterial { get; set; }
        public static Material LoserMaterial { get; set; }

        public static void ClearSession()
        {
            WinnerMaterial = null;
            LoserMaterial = null;
            _mapCollection = null;
            _mapCollectionScores = null;
            NextRoundRewiredPlayerId = null;
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