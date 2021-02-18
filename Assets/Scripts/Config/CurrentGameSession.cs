using System.Collections.Generic;

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
            _chosenMap = null;
            Leaderboard.Clear();
        }

        public static string GetNextMap(string currentMap)
        {
            var maps = GameConfig.Instance.PlayableMaps;
            return maps[(maps.IndexOf(currentMap) + 1) % maps.Count];
        }

        public static readonly List<MapScore> Leaderboard = new List<MapScore>();

        public static string ChosenMap
        {
            get
            {
                if (string.IsNullOrEmpty(_chosenMap))
                {
                    _chosenMap = GameConfig.Instance.MapPreviews[0].Scene;
                }

                return _chosenMap;
            }
            set => _chosenMap = value;
        }


        private static string _chosenMap;

        public static bool IsLastMapInList(string currentMap)
        {
            var maps = GameConfig.Instance.PlayableMaps;
            return maps.IndexOf(currentMap) == maps.Count - 1;
        }
    }
}