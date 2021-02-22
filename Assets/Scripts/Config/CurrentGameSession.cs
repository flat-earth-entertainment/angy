using System.Collections.Generic;
using System.Linq;
using Scenes.Map_Selection;

namespace Config
{
    public class MapScore
    {
        public string Name;
        public int Player1Score;
        public int Player2Score;

        public MapScore(string name, int player1Score, int player2Score)
        {
            Name = name;
            Player1Score = player1Score;
            Player2Score = player2Score;
        }
    }

    public static class CurrentGameSession
    {
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

        public static readonly List<MapScore> Leaderboard = new List<MapScore>();

        public static MapCollection MapCollection
        {
            get
            {
                if (_mapCollection == null)
                {
                    _mapCollection = GameConfig.Instance.MapCollections[0];
                }

                return _mapCollection;
            }
            set => _mapCollection = value;
        }


        private static MapCollection _mapCollection;

        public static bool IsLastMapInList(string currentMap)
        {
            var maps = MapCollection.Maps;
            return maps.ToList().IndexOf(currentMap) == maps.Length - 1;
        }
    }
}