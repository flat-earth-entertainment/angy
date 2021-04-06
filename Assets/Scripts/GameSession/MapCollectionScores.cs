using System;
using System.Linq;
using Scenes.Map_Selection;

namespace GameSession
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
}