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

        private PlayerView _activePlayer;
        private Slider _angySlider;

        private void Awake()
        {
            _angySlider = angyMeter.GetComponentInChildren<Slider>();
            _angySlider.minValue = GameConfig.Instance.AngyValues.MinAngy;
            _angySlider.maxValue = GameConfig.Instance.AngyValues.MaxAngy;

            restartButton.onClick.AddListener(OnRestartButtonClicked);
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
            _angySlider.value = player.Angy;
            player.AngyChanged += OnActivePlayerAngyChanged;

            angyMeter.SetActive(true);
        }

        public void DisableAngyMeter()
        {
            angyMeter.SetActive(false);
            _activePlayer.AngyChanged -= OnActivePlayerAngyChanged;
            _activePlayer = null;
        }

        private void OnActivePlayerAngyChanged(int newAngyValue)
        {
            _angySlider.value = newAngyValue;
        }
    }
}