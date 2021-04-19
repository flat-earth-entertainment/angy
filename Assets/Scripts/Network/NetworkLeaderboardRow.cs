using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Network
{
    public class NetworkLeaderboardRow : MonoBehaviour
    {
        [SerializeField]
        private Image avatarImage;

        [SerializeField]
        private TextMeshProUGUI nameText;

        [SerializeField]
        private TextMeshProUGUI scoreText;

        public void Setup(LeaderboardPlayer leaderboardPlayer)
        {
            avatarImage.color = leaderboardPlayer.Avatar.AsColor();
            nameText.text = leaderboardPlayer.Nickname;
            scoreText.text = leaderboardPlayer.Highscore.ToString();
        }
    }
}