using TMPro;
using UnityEngine;

namespace UI
{
    public class LeaderboardMapRow : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI scorePlayer1;

        [SerializeField]
        private TextMeshProUGUI mapNumber;

        [SerializeField]
        private TextMeshProUGUI scorePlayer2;

        public void SetRow(int score1, int score2, int roundNumber)
        {
            scorePlayer1.text = score1.ToString();
            scorePlayer2.text = score2.ToString();
            mapNumber.text = roundNumber.ToString();
        }
    }
}