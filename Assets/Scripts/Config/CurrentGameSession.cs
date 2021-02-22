using System.Collections.Generic;
using System.Linq;
using Scenes.Map_Selection;

namespace Config
{
    public readonly struct MapScore
    {
        public readonly string Name;
        public readonly int Player1Score;
        public readonly int Player2Score;

        public MapScore(string name, int player1Score, int player2Score)
        {
            Name = name;
            Player1Score = player1Score;
            Player2Score = player2Score;
        }
    }

    public static class CurrentGameSession
    {
        public static readonly List<MapScore> Leaderboard = new List<MapScore>();
        public static int? NextRoundRewiredPlayerId { get; set; }

        private static MapCollection _mapCollection;

        public static void ClearSession()
        {
            _mapCollection = null;
            Leaderboard.Clear();
        }

        public static string GetNextMap(string currentMap)
        {
            var maps = MapCollection.Maps;
            return maps[(maps.ToList().IndexOf(currentMap) + 1) % maps.Length];
        }

        public static MapCollection MapCollection
        {
            get => _mapCollection ??= GameConfig.Instance.MapCollections[0];
            set => _mapCollection = value;
        }

        public static bool IsLastMapInList(string currentMap)
        {
            var maps = MapCollection.Maps;
            return maps.ToList().IndexOf(currentMap) == maps.Length - 1;
        }
    }
}