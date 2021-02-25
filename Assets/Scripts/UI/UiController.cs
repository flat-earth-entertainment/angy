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
        private GameObject cameraModeWarning;

        [SerializeField]
        private GameObject angyMeter;

        [SerializeField]
        private Slider angySlider;

        [SerializeField]
        private Slider angySlider2;

        [SerializeField]
        private PlayersManager playersManager;

        // private PlayerView _activePlayer;

        private void Awake()
        {
            angySlider.minValue = angySlider2.minValue = GameConfig.Instance.AngyValues.MinAngy;
            angySlider.maxValue = angySlider2.maxValue = GameConfig.Instance.AngyValues.MaxAngy;

            restartButton.onClick.AddListener(OnContinueButtonClicked);

            playersManager.InitializedAllPlayers += OnPlayersInitialized;
        }

        private void OnPlayersInitialized(PlayerView[] obj)
        {
            playersManager.InitializedAllPlayers -= OnPlayersInitialized;

            obj[0].AngyChanged += OnPlayer1AngyChanged;
            obj[1].AngyChanged += OnPlayer2AngyChanged;

            angySlider.transform.GetChild(1).GetChild(0).GetComponent<Image>().color = obj[0].PlayerColor;
            angySlider2.transform.GetChild(1).GetChild(0).GetComponent<Image>().color = obj[1].PlayerColor;
        }

        private void OnDisable()
        {
            playersManager.Players[0].AngyChanged -= OnPlayer1AngyChanged;
            playersManager.Players[1].AngyChanged -= OnPlayer2AngyChanged;
        }


        public static async void OnContinueButtonClicked()
        {
            await SceneManager.UnloadSceneAsync("Prediction");

            var currentMap = SceneManager.GetActiveScene().name;

            if (!CurrentGameSession.IsLastMapInList(currentMap))
            {
                LeaderboardSceneUiController.SceneToLoad = CurrentGameSession.GetNextMap(currentMap);
            }
            else
            {
                LeaderboardSceneUiController.SceneToLoad = GameConfig.Instance.Scenes.MainMenuScene;
            }
        }

        public void SetCameraModeActive(bool state)
        {
            cameraModeWarning.SetActive(state);
        }

        public void HideAllUi()
        {
            angyMeter.SetActive(false);
            winScreen.SetActive(false);
            cameraModeWarning.SetActive(false);
        }

        public void EnableAngyMeterFor(PlayerView player)
        {
            // _activePlayer = player;
            // angySlider.value = player.Angy;
            // player.AngyChanged += OnActivePlayerAngyChanged;

            angyMeter.SetActive(true);
        }

        public void DisableAngyMeter()
        {
            angyMeter.SetActive(false);
            // _activePlayer.AngyChanged -= OnActivePlayerAngyChanged;
            // _activePlayer = null;
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