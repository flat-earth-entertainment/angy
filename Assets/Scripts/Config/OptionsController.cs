using System;
using GameSession;
using Photon.Pun;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace Config
{
    public class OptionsController : MonoBehaviour
    {
        public static Action BackButtonClicked;

        private static OptionsController _instance;

        [SerializeField]
        private Button backButton;

        [SerializeField]
        private Slider masterVolume;

        [SerializeField]
        private Slider musicVolume;

        [SerializeField]
        private Slider sfxVolume;

        [SerializeField]
        private GameObject mainMenuGameObject;

        [SerializeField]
        private Button mainMenuButton;

        public static OptionsController Instance
        {
            get
            {
                if (_instance == null || _instance.Equals(null))
                {
                    _instance = FindObjectOfType<OptionsController>();

                    if (_instance == null || _instance.Equals(null))
                    {
                        _instance = Instantiate(GameConfig.Instance.OptionsController)
                            .GetComponentInChildren<OptionsController>();

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
            backButton.onClick.AddListener(delegate { BackButtonClicked?.Invoke(); });

            mainMenuButton.onClick.AddListener(delegate
            {
                SceneChanger.ChangeScene(GameConfig.Instance.Scenes.MainMenuScene);
                CurrentGameSession.ClearSession();
                PhotonNetwork.LeaveRoom();
                PhotonNetwork.Disconnect();
            });

            InitializeAndBindSlider(GameSettings.Settings.MasterVolume, masterVolume);
            InitializeAndBindSlider(GameSettings.Settings.MusicVolume, musicVolume);
            InitializeAndBindSlider(GameSettings.Settings.SfxVolume, sfxVolume);
        }

        public static void Show(bool showMainMenu = true)
        {
            Instance.gameObject.SetActive(true);
            Instance.mainMenuGameObject.SetActive(showMainMenu);
        }

        public static void Hide()
        {
            Instance.gameObject.SetActive(false);
        }

        private static void InitializeAndBindSlider(GameSettings.Settings band, Slider slider)
        {
            slider.value = GameSettings.GetFloat(band);

            slider.onValueChanged.AddListener(delegate(float newValue) { GameSettings.SetFloat(band, newValue); });
        }
    }
}