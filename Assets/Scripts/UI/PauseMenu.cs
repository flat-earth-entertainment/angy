using System;
using Config;
using GameSession;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI
{
    [RequireComponent(typeof(Canvas))]
    public class PauseMenu : MonoBehaviour
    {
        public static Action ResumeButtonClicked;

        public static bool IsShowing { get; private set; }

        private static PauseMenu _instance;

        [SerializeField]
        private Button resumeButton;

        [SerializeField]
        private Button restartButton;

        [SerializeField]
        private Button optionsButton;

        [SerializeField]
        private Button helpButton;

        [SerializeField]
        private Button mainMenuButton;

        [SerializeField]
        private GameObject helpUiPrefab;

        private GameObject _helpUi;
        private Canvas _canvas;

        private static PauseMenu Instance
        {
            get
            {
                if (_instance == null || _instance.Equals(null))
                {
                    _instance = FindObjectOfType<PauseMenu>();

                    if (_instance == null || _instance.Equals(null))
                    {
                        _instance = Instantiate(GameConfig.Instance.PauseMenu)
                            .GetComponent<PauseMenu>();

                        if (_instance == null || _instance.Equals(null))
                        {
                            Debug.LogError("Cannot instantiate Options! Check the Game Config or call Gabe!");
                        }
                    }
                }

                return _instance;
            }
        }

        private void Awake()
        {
            _canvas = GetComponent<Canvas>();
            _instance = this;

            Hide();

            optionsButton.onClick.AddListener(delegate
            {
                OptionsController.Show(false);
                gameObject.SetActive(false);
                OptionsController.BackButtonClicked = delegate
                {
                    gameObject.SetActive(true);
                    OptionsController.Hide();
                };
            });

            helpButton.onClick.AddListener(delegate
            {
                HelpUi.OnClose = HideHelp;
                _helpUi = Instantiate(helpUiPrefab);
                gameObject.SetActive(false);
            });

            mainMenuButton.onClick.AddListener(delegate
            {
                SceneChanger.ChangeScene(GameConfig.Instance.Scenes.MainMenuScene);
                CurrentGameSession.ClearSession();
                PhotonNetwork.LeaveRoom();
                PhotonNetwork.Disconnect();
            });

            resumeButton.onClick.AddListener(delegate { ResumeButtonClicked?.Invoke(); });

            restartButton.onClick.AddListener(delegate
            {
                SceneChanger.ChangeScene(SceneManager.GetActiveScene().name);
            });
        }

        public static void Show(Action resumeButtonAction = null)
        {
            ResumeButtonClicked = resumeButtonAction;
            Instance.gameObject.SetActive(true);
            IsShowing = true;
        }

        public static void Hide()
        {
            IsShowing = false;
            Instance.gameObject.SetActive(false);
        }

        private void HideHelp()
        {
            Destroy(_helpUi);
            _helpUi = null;
            gameObject.SetActive(true);
            HelpUi.OnClose = null;
        }
    }
}