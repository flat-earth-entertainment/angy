using System;
using System.Collections.Generic;
using Abilities;
using ExitGames.Client.Photon;
using GameSession;
using Photon.Pun;
using Photon.Realtime;
using Player;
using UI;
using UnityEngine;
using Utils;

namespace Logic
{
    public class AbilityController : MonoBehaviour, IOnEventCallback
    {
        public event Action<PlayerView, Ability> NewAbilitySet;

        [SerializeField]
        private PlayersManager playersManager;

        private readonly Dictionary<PlayerView, Ability> _playerAbilities =
            new Dictionary<PlayerView, Ability>();

        private readonly Dictionary<PlayerView, Ability> _previousPlayerAbilities =
            new Dictionary<PlayerView, Ability>();

        private PhotonEventListener _photonEventListener;

        public void SetNewAbility(PlayerView playerView, Ability ability)
        {
            _previousPlayerAbilities[playerView] = ability;
            _playerAbilities[playerView]?.Wrap();
            _playerAbilities[playerView] = ability;
        }

        public void SetNewAbilityAndTryNotify(PlayerView playerView, Ability ability)
        {
            SetNewAbility(playerView, ability);

            NewAbilitySet?.Invoke(playerView, ability);

            if (!CurrentGameSession.IsNowPassive)
            {
                PhotonShortcuts.ReliableRaiseEventToOthers(GameEvent.PlayerAbilitySet,
                    new int[]
                    {
                        CurrentGameSession.PlayerFromPlayerView(playerView).Id,
                        (int) Ability.AbilityCodeFromInstance(ability)
                    });
            }
        }

        public bool HasAbility(PlayerView playerView) => _playerAbilities[playerView] != null;
        public Ability GetPlayerAbility(PlayerView playerView) => _playerAbilities[playerView];
        public Ability GetPreviousPlayerAbility(PlayerView playerView) => _previousPlayerAbilities[playerView];

        public bool TryInvokeAbility(PlayerView playerView)
        {
            var playerAbility = _playerAbilities[playerView];
            if (HasAbility(playerView) && !playerAbility.WasFired)
            {
                playerAbility.Invoke(playerView);
                PhotonShortcuts.ReliableRaiseEventToOthers(GameEvent.PlayerAbilityFired,
                    CurrentGameSession.PlayerFromPlayerView(playerView).Id);
                return true;
            }

            return false;
        }

        public void CopyPreviousAbilityToCurrent(PlayerView playerView)
        {
            SetNewAbilityAndTryNotify(playerView, Ability.Copy(_previousPlayerAbilities[playerView]));
        }

        private void Awake()
        {
            _photonEventListener = PhotonEventListener.ListenTo(GameEvent.PlayerAbilityFired,
                data => TryInvokeAbility(CurrentGameSession.PlayerViewFromId((int) data.CustomData)), false);
        }

        private void OnEnable()
        {
            playersManager.InitializedAllPlayers += OnPlayersInitialized;

            PhotonNetwork.AddCallbackTarget(this);
        }

        private void OnDisable()
        {
            PhotonNetwork.RemoveCallbackTarget(this);
        }

        private void OnPlayersInitialized(PlayerView[] playerViews)
        {
            playersManager.InitializedAllPlayers -= OnPlayersInitialized;

            foreach (var playerView in playerViews)
            {
                _playerAbilities.Add(playerView, null);
                _previousPlayerAbilities.Add(playerView, null);
            }
        }

        public void OnEvent(EventData photonEvent)
        {
            if (CurrentGameSession.IsNowPassive && photonEvent.Code == (byte) GameEvent.PlayerAbilitySet)
            {
                var data = (int[]) photonEvent.CustomData;
                var playerView = CurrentGameSession.PlayerViewFromId(data[0]);
                var newAbility = Ability.InstanceFromAbilityCode((AbilityCode) data[1]);

                if (newAbility is RandomAbility)
                {
                    newAbility = Ability.InstanceFromAbilityCode((AbilityCode) data[2]);
                    FindObjectOfType<UiController>().DoSlotMachineFor(playerView, newAbility);
                }

                SetNewAbilityAndTryNotify(playerView, newAbility);
            }
        }
    }
}