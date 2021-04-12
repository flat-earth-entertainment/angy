using System.Collections.Generic;
using Abilities;
using Abilities.Config;
using Audio;
using Config;
using Player;
using UnityEngine;

namespace UI
{
    public class UiController : MonoBehaviour
    {
        [SerializeField]
        private GameObject cameraModeWarning;

        [SerializeField]
        private GameObject cameraModeHelper;

        [SerializeField]
        private GameObject angyMeter;

        [SerializeField]
        private AngyUi[] angyUis;

        [SerializeField]
        private AbilityUi[] abilityUis;

        [SerializeField]
        private PlayersManager playersManager;

        private Dictionary<PlayerView, AbilityUi> _abilityPlayerUis = new Dictionary<PlayerView, AbilityUi>();
        private Dictionary<PlayerView, AngyUi> _angyPlayerUis = new Dictionary<PlayerView, AngyUi>();

        public bool CameraModeHelperActive
        {
            set => cameraModeHelper.SetActive(value);
        }

        private void Awake()
        {
            foreach (var angyUi in angyUis)
            {
                angyUi.Initialize(GameConfig.Instance.AngyValues.MinAngy, GameConfig.Instance.AngyValues.MaxAngy);
            }

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


        public void WobbleAbilityUi(PlayerView playerView, bool state)
        {
            if (_abilityPlayerUis.ContainsKey(playerView))
            {
                _abilityPlayerUis[playerView].Wobble = state;
            }
            else
            {
                Debug.LogWarning($"Can't find Ability UI for player {playerView.PlayerPreset.PlayerName}",
                    playerView.gameObject);
            }
        }

        public void DoSlotMachineFor(PlayerView player, Ability ability)
        {
            if (_abilityPlayerUis.ContainsKey(player))
            {
                _abilityPlayerUis[player].Visible = true;
                _abilityPlayerUis[player]
                    .DoSlotMachine(3.2f, AbilityConfig.GetConfigSpriteFor(ability), player.PlayerPreset.PlayerColor);
                AudioManager.PlaySfx(SfxType.RandomActivate);
            }
        }

        private void SetAbilityIconFor(PlayerView player, Sprite icon)
        {
            if (_abilityPlayerUis.ContainsKey(player))

            {
                _abilityPlayerUis[player].SetAbilityIcon(icon, player.PlayerPreset.PlayerColor);
            }
            else
            {
                Debug.LogWarning($"Can't find Ability UI for player {player.PlayerPreset.PlayerName}",
                    player.gameObject);
            }
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

        private void OnPlayersInitialized(PlayerView[] players)
        {
            playersManager.InitializedAllPlayers -= OnPlayersInitialized;

            players[0].AngyChanged += angyUis[0].OnAngyChanged;
            players[1].AngyChanged += angyUis[1].OnAngyChanged;

            _angyPlayerUis.Add(players[0], angyUis[0]);
            _angyPlayerUis.Add(players[1], angyUis[1]);

            _abilityPlayerUis.Add(players[0], abilityUis[0]);
            _abilityPlayerUis.Add(players[1], abilityUis[1]);
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