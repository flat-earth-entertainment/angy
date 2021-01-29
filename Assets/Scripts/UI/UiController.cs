using System;
using Config;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI
{
    public class UiController : MonoBehaviour
    {
        [SerializeField]
        private Button restartButton;

        [SerializeField]
        private GameObject winScreen;

        [SerializeField]
        private TextMeshProUGUI winText;

        [SerializeField]
        private GameObject angyMeter;

        [SerializeField]
        private Slider angySlider;

        [SerializeField]
        private Slider angySlider2;

        private PlayerView _activePlayer;

        private void Awake()
        {
            angySlider.minValue = angySlider2.minValue = GameConfig.Instance.AngyValues.MinAngy;
            angySlider.maxValue = angySlider2.maxValue = GameConfig.Instance.AngyValues.MaxAngy;

            restartButton.onClick.AddListener(OnRestartButtonClicked);

            FindObjectOfType<PlayersManager>().InitializedAllPlayers += OnPlayersInitialized;
        }

        private void OnPlayersInitialized(PlayerView[] obj)
        {
            FindObjectOfType<PlayersManager>().InitializedAllPlayers -= OnPlayersInitialized;

            obj[0].AngyChanged += OnPlayer1AngyChanged;
            obj[1].AngyChanged += OnPlayer2AngyChanged;

            angySlider.transform.GetChild(1).GetChild(0).GetComponent<Image>().color = obj[0].PlayerColor;
            angySlider2.transform.GetChild(1).GetChild(0).GetComponent<Image>().color = obj[1].PlayerColor;
        }


        private async void OnRestartButtonClicked()
        {
            await SceneManager.UnloadSceneAsync("Prediction");

            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        public void ShowWinScreen((PlayerView, int) winner, params (PlayerView, int)[] others)
        {
            winText.text = winner.Item1.Nickname + " won!\n";
            winText.text += winner.Item1.Nickname + " : " + winner.Item2 + "\n";

            foreach (var playerAndScore in others)
            {
                winText.text += playerAndScore.Item1.Nickname + " : " + playerAndScore.Item2 + "\n";
            }

            HideAllUi();
            winScreen.SetActive(true);
        }

        public void HideAllUi()
        {
            angyMeter.SetActive(false);
            winScreen.SetActive(false);
        }

        public void EnableAngyMeterFor(PlayerView player)
        {
            _activePlayer = player;
            angySlider.value = player.Angy;
            // player.AngyChanged += OnActivePlayerAngyChanged;

            angyMeter.SetActive(true);
        }

        public void DisableAngyMeter()
        {
            angyMeter.SetActive(false);
            // _activePlayer.AngyChanged -= OnActivePlayerAngyChanged;
            _activePlayer = null;
        }

        private void OnPlayer1AngyChanged(int newAngyValue)
        {
            angySlider.value = newAngyValue;
        }

        private void OnPlayer2AngyChanged(int newAngyValue)
        {
            angySlider2.value = newAngyValue;
        }
    }
}