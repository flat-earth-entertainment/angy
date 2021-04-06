namespace GameSession
{
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
}