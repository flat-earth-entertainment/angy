using System;
using Audio;
using Config;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI
{
    public class OptionsController : MonoBehaviour
    {
        public static Action BackButtonClicked;

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

        public static void Show(bool showMainMenu = true)
        {
            Instance.gameObject.SetActive(true);
            Instance.mainMenuGameObject.SetActive(showMainMenu);
        }

        public static void Hide()
        {
            Instance.gameObject.SetActive(false);
        }

        private void Awake()
        {
            backButton.onClick.AddListener(delegate { BackButtonClicked?.Invoke(); });

            mainMenuButton.onClick.AddListener(delegate
            {
                SceneManager.LoadScene(GameConfig.Instance.Scenes.MainMenuScene);
                CurrentGameSession.ClearSession();
            });

            InitializeAndBindSlider(GameSettings.Settings.MasterVolume, masterVolume);
            InitializeAndBindSlider(GameSettings.Settings.MusicVolume, musicVolume);
            InitializeAndBindSlider(GameSettings.Settings.SfxVolume, sfxVolume);
        }

        private static void InitializeAndBindSlider(GameSettings.Settings band, Slider slider)
        {
            slider.value = GameSettings.GetFloat(band);

            slider.onValueChanged.AddListener(delegate(float newValue) { GameSettings.SetFloat(band, newValue); });
        }

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
                            .GetComponent<OptionsController>();

                        if (_instance == null || _instance.Equals(null))
                        {
                            Debug.LogError("Cannot instantiate Options! Check the Game Config or call Gabe!");
                        }
                    }
                }

                return _instance;
            }
        }

        private static OptionsController _instance;
    }
}