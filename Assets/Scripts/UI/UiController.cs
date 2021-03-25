using Abilities;
using Abilities.Config;
using Audio;
using Config;
using UnityEngine;

namespace UI
{
    public class UiController : MonoBehaviour
    {
        public bool CameraModeHelperActive
        {
            set => cameraModeHelper.SetActive(value);
        }

        [SerializeField]
        private GameObject cameraModeWarning;

        [SerializeField]
        private GameObject cameraModeHelper;

        [SerializeField]
        private GameObject angyMeter;

        [SerializeField]
        private AngyUi angyUi1;

        [SerializeField]
        private AngyUi angyUi2;

        [SerializeField]
        private PlayersManager playersManager;

        [SerializeField]
        private AbilityUi[] abilityUis;


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
            angyUi1.Initialize(GameConfig.Instance.AngyValues.MinAngy, GameConfig.Instance.AngyValues.MaxAngy);
            angyUi2.Initialize(GameConfig.Instance.AngyValues.MinAngy, GameConfig.Instance.AngyValues.MaxAngy);

            playersManager.InitializedAllPlayers += OnPlayersInitialized;
        }

        private void OnEnable()
        {
            PlayerView.NewAbilitySet += OnNewAbilitySet;
        }

        private void OnDisable()
        {
            PlayerView.NewAbilitySet -= OnNewAbilitySet;
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

            obj[0].AngyChanged += angyUi1.OnAngyChanged;
            obj[1].AngyChanged += angyUi2.OnAngyChanged;
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
            CameraModeHelperActive = false;
        }

        public void EnableAngyMeter()
        {
            angyMeter.SetActive(true);
        }

        public void DisableAngyMeter()
        {
            angyMeter.SetActive(false);
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