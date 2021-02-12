using System;
using Config;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI
{
    public class LeaderboardSceneUiController : MonoBehaviour
    {
        public static string SceneToLoad { get; set; }

        [SerializeField]
        private TextMeshProUGUI leaderboardText;

        [SerializeField]
        private Button mainMenuButton;

        [SerializeField, Scene]
        private string mainMenuScene;

        private void Awake()
        {
            mainMenuButton.onClick.AddListener(delegate
            {
                SceneManager.LoadScene(SceneToLoad);
                if (SceneToLoad == GameConfig.Instance.Scenes.MainMenuScene)
                {
                    CurrentGameSession.ClearSession();
                }
            });
        }

        private void Start()
        {
            if (CurrentGameSession.Leaderboard.Count > 0)
            {
                leaderboardText.text = "";
                foreach (var mapScore in CurrentGameSession.Leaderboard)
                {
                    leaderboardText.text += $"{mapScore.Name}\n";
                    leaderboardText.text +=
                        $"Player 1: <color=#ffa500ff>{mapScore.Player1Score} : {mapScore.Player2Score}</color> :Player 2\n";
                }
            }
        }

        private void Update()
        {
#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.Space))
            {
                for (int i = 0; i < 3; i++)
                {
                    CurrentGameSession.Leaderboard.Add(new MapScore(i.ToString(), i * 2, i * 3));
                    Start();
                }
            }
#endif
        }
    }
}