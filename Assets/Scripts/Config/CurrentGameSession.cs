namespace Config
{
    public static class CurrentGameSession
    {
        public static string GetNextMap(string currentMap)
        {
            var maps = GameConfig.Instance.PlayableMaps;
            return maps[(maps.IndexOf(currentMap) + 1) % maps.Count];
        }

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
    }
}