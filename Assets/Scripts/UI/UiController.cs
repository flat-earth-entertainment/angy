using Abilities;
using Abilities.Config;
using Audio;
using Config;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UiController : MonoBehaviour
    {
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

        [SerializeField]
        private AbilityUi[] abilityUis;

        private float _angy1Value;
        private float _angy2Value;

        public void WobbleAbilityUi(PlayerView playerView, bool state)
        {
            if (playerView.PlayerId < abilityUis.Length)
            {
                abilityUis[playerView.PlayerId].Wobble = state;
            }
            else
            {
                Debug.LogWarning($"Can't find Ability UI for player with ID {playerView.PlayerId}");
            }
        }

        public void DoSlotMachineFor(PlayerView player, Ability ability)
        {
            abilityUis[player.PlayerId].Visible = true;
            abilityUis[player.PlayerId]
                .DoSlotMachine(3.2f, AbilityConfig.GetConfigSpriteFor(ability), player.PlayerColor);
            AudioManager.PlaySfx(SfxType.RandomActivate);
        }

        private void SetAbilityIconFor(PlayerView player, Sprite icon)
        {
            if (player.PlayerId < abilityUis.Length)
            {
                abilityUis[player.PlayerId].SetAbilityIcon(icon, player.PlayerColor);
            }
            else
            {
                Debug.LogWarning($"Can't find Ability UI for player with ID {player.PlayerId}");
            }
        }

        private void Awake()
        {
            angySlider.minValue = angySlider2.minValue = GameConfig.Instance.AngyValues.MinAngy;
            angySlider.maxValue = angySlider2.maxValue = GameConfig.Instance.AngyValues.MaxAngy;

            playersManager.InitializedAllPlayers += OnPlayersInitialized;
        }

        private void OnEnable()
        {
            PlayerView.NewAbilitySet += OnNewAbilitySet;
        }

        private void OnDisable()
        {
            PlayerView.NewAbilitySet -= OnNewAbilitySet;

            playersManager.Players[0].AngyChanged -= OnPlayer1AngyChanged;
            playersManager.Players[1].AngyChanged -= OnPlayer2AngyChanged;
        }

        private void OnNewAbilitySet(PlayerView player, Ability ability)
        {
            if (ability == null && player.PreviousAbility != null)
            {
                SetAbilityIconFor(player, AbilityConfig.GetConfigSpriteFor(player.PreviousAbility));
            }
            else
            {
                SetAbilityIconFor(player, AbilityConfig.GetConfigSpriteFor(ability));
            }

            if (player.PlayerState == PlayerState.ActiveInMotion)
            {
                WobbleAbilityUi(player, ability != null);
            }
        }

        private void OnPlayersInitialized(PlayerView[] obj)
        {
            playersManager.InitializedAllPlayers -= OnPlayersInitialized;

            obj[0].AngyChanged += OnPlayer1AngyChanged;
            obj[1].AngyChanged += OnPlayer2AngyChanged;

            angySlider.transform.GetChild(1).GetChild(0).GetComponent<Image>().color = obj[0].PlayerColor;
            angySlider2.transform.GetChild(1).GetChild(0).GetComponent<Image>().color = obj[1].PlayerColor;
        }

        public void SetCameraModeActive(bool state)
        {
            cameraModeWarning.SetActive(state);
        }

        public void HideAllUi()
        {
            DisableAngyMeter();
            cameraModeWarning.SetActive(false);
            DisableAbilityUi();
        }

        public void EnableAngyMeter()
        {
            angyMeter.SetActive(true);
        }

        public void DisableAngyMeter()
        {
            angyMeter.SetActive(false);
        }

        private void OnPlayer1AngyChanged(int newAngyValue)
        {
            // angySlider.value = newAngyValue;
            _angy1Value = newAngyValue;
        }

        private void OnPlayer2AngyChanged(int newAngyValue)
        {
            // angySlider2.value = newAngyValue;
            _angy2Value = newAngyValue;
        }

        private void Update()
        {
            LerpSlider(angySlider, _angy1Value);
            LerpSlider(angySlider2, _angy2Value);
        }

        private void LerpSlider(Slider slider, float value)
        {
            if (slider.value < value)
            {
                slider.value += GameConfig.Instance.SliderSpeed * Time.unscaledDeltaTime;
            }

            if (slider.value > value)
            {
                slider.value -= GameConfig.Instance.SliderSpeed * Time.unscaledDeltaTime;
            }
        }

        public void EnableAbilityUi()
        {
            foreach (var abilityUi in abilityUis)
            {
                abilityUi.Visible = true;
            }
        }

        public void DisableAbilityUi()
        {
            foreach (var abilityUi in abilityUis)
            {
                abilityUi.Visible = false;
            }
        }
    }
}