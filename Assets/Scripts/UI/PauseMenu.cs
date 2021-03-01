using System;
using Config;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class PauseMenu : MonoBehaviour
    {
        public static Action ResumeButtonClicked;

        [SerializeField]
        private Button resumeButton;

        [SerializeField]
        private Button optionsButton;

        [SerializeField]
        private Button helpButton;

        [SerializeField]
        private Button mainMenuButton;

        [SerializeField]
        private GameObject helpUiPrefab;

        private GameObject _helpUi;

        public static void Show(Action resumeButtonAction = null)
        {
            ResumeButtonClicked = resumeButtonAction;
            Instance.gameObject.SetActive(true);
        }

        public static void Hide()
        {
            Instance.gameObject.SetActive(false);
        }

        private void HideHelp()
        {
            Destroy(_helpUi);
            _helpUi = null;
            gameObject.SetActive(true);
            HelpUi.OnClose = null;
        }

        private void Awake()
        {
            optionsButton.onClick.AddListener(delegate
            {
                OptionsController.Show(false);
                OptionsController.BackButtonClicked = OptionsController.Hide;
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
            });

            resumeButton.onClick.AddListener(delegate { ResumeButtonClicked?.Invoke(); });
        }

        public static PauseMenu Instance
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

        private static PauseMenu _instance;
    }
}