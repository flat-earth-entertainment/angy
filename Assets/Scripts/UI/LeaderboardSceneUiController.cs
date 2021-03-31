using Config;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class LeaderboardSceneUiController : MonoBehaviour
    {
        [SerializeField]
        private Button mainMenuButton;

        [SerializeField]
        private GameObject mapRowPrefab;

        [SerializeField]
        private Transform mapTableParent;

        [SerializeField]
        private TextMeshProUGUI totalPlayer1;

        [SerializeField]
        private TextMeshProUGUI totalPlayer2;

        [SerializeField]
        private GameObject player1Highlight;

        [SerializeField]
        private GameObject player2Highlight;

        public static string SceneToLoad { get; set; }

        private void Awake()
        {
            mainMenuButton.onClick.AddListener(LoadNextScene);

            player1Highlight.SetActive(false);
            player2Highlight.SetActive(false);

            FillLeaderboard();
        }

        private void Update()
        {
            // TEMP
            if (Input.anyKeyDown)
            {
                LoadNextScene();
            }

            // TEMP
        }

        private static void LoadNextScene()
        {
            SceneChanger.ChangeScene(SceneToLoad);
            if (SceneToLoad == GameConfig.Instance.Scenes.MainMenuScene)
            {
                CurrentGameSession.ClearSession();
            }
        }

        private void FillLeaderboard()
        {
            int sum2;
            var sum1 = sum2 = 0;

            for (var i = 0; i < CurrentGameSession.CollectionScores.Scores.Length; i++)
            {
                var mapScore = CurrentGameSession.CollectionScores.Scores[i];
                Instantiate(mapRowPrefab, mapTableParent).GetComponent<LeaderboardMapRow>()
                    .SetRow(mapScore.Player1Score, mapScore.Player2Score, i + 1);

                if (mapScore.Player1Score != null) sum1 += mapScore.Player1Score.Value;
                if (mapScore.Player2Score != null) sum2 += mapScore.Player2Score.Value;
            }

            totalPlayer1.text = sum1.ToString();
            totalPlayer2.text = sum2.ToString();

            if (SceneToLoad == GameConfig.Instance.Scenes.MainMenuScene)
            {
                if (sum1 == sum2)
                {
                    player1Highlight.SetActive(true);
                    player2Highlight.SetActive(true);
                }
                else if (sum1 > sum2)
                {
                    player1Highlight.SetActive(true);
                }
                else
                {
                    player2Highlight.SetActive(true);
                }
            }
        }
    }
}