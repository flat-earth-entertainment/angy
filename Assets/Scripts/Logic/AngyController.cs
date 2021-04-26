using System;
using System.Collections.Generic;
using Config;
using ExitGames.Client.Photon;
using GameSession;
using Photon.Pun;
using Photon.Realtime;
using Player;
using UnityEngine;
using Utils;

namespace Logic
{
    public class AngyController : MonoBehaviour, IOnEventCallback
    {
        public event Action<PlayerView, int> AngyChanged;
        public event Action<PlayerView> ReachedMaxAngy;

        [SerializeField]
        private PlayersManager playersManager;

        /// <summary>
        /// Returns PlayerView's angy
        /// </summary>
        public int this[PlayerView index] => _playerAngys[index];

        private readonly Dictionary<PlayerView, int> _playerAngys = new Dictionary<PlayerView, int>();

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
                _playerAngys.Add(playerView, GameConfig.Instance.AngyValues.MinAngy);
            }
        }


        public void AlterAngyIfActive(PlayerView playerView, AngyEvent angyEvent)
        {
            if (!_playerAngys.ContainsKey(playerView))
            {
                Debug.LogError("Can't find Angy for player " + playerView.gameObject.name, playerView.gameObject);
                return;
            }

            if (CurrentGameSession.IsNowPassive)
            {
                return;
            }

            var initialAngy = _playerAngys[playerView];
            switch (angyEvent)
            {
                case AngyEvent.HitBadObject:
                    _playerAngys[playerView] += GameConfig.Instance.AngyValues.HitBadObject;
                    break;
                case AngyEvent.FellOutOfTheMap:
                    _playerAngys[playerView] += GameConfig.Instance.AngyValues.FellOutOfTheMap;
                    break;
                case AngyEvent.AfterFellOutOfTheMapAndReachedMaxAngy:
                    _playerAngys[playerView] = GameConfig.Instance.AngyValues.AfterFellOutOfTheMapAndReachedMaxAngy;
                    break;
                case AngyEvent.ShotMade:
                    _playerAngys[playerView] += GameConfig.Instance.AngyValues.ShotMade;
                    break;
                case AngyEvent.HitSomeone:
                    _playerAngys[playerView] += GameConfig.Instance.AngyValues.PlayerHitSomeone;
                    break;
                case AngyEvent.GotHit:
                    _playerAngys[playerView] += GameConfig.Instance.AngyValues.PlayerGotHit;
                    break;
                case AngyEvent.MushroomHit:
                    _playerAngys[playerView] -= GameConfig.Instance.AngyValues.MushroomHit;
                    break;
            }

            var newAngy = _playerAngys[playerView];
            if (newAngy != initialAngy)
            {
                NotifyAngyChange(playerView, newAngy);
            }
        }

        public void ResetAngyFor(PlayerView playerView)
        {
            NotifyAngyChange(playerView, GameConfig.Instance.AngyValues.MinAngy);
        }

        private void NotifyAngyChange(PlayerView playerView, int newAngy)
        {
            if (newAngy > GameConfig.Instance.AngyValues.MaxAngy)
            {
                _playerAngys[playerView] = GameConfig.Instance.AngyValues.MaxAngy;
                ReachedMaxAngy?.Invoke(playerView);
            }
            else if (newAngy < GameConfig.Instance.AngyValues.MinAngy)
            {
                _playerAngys[playerView] = GameConfig.Instance.AngyValues.MinAngy;
            }
            else
            {
                _playerAngys[playerView] = newAngy;
            }

            AngyChanged?.Invoke(playerView, newAngy);
            PhotonShortcuts.ReliableRaiseEventToOthers(GameEvent.PlayerAngyChanged,
                new int[] {CurrentGameSession.PlayerFromPlayerView(playerView).Id, newAngy});
        }

        public void OnEvent(EventData photonEvent)
        {
            if (CurrentGameSession.IsNowPassive && photonEvent.Code == (byte) GameEvent.PlayerAngyChanged)
            {
                var data = (int[]) photonEvent.CustomData;
                var playerView = CurrentGameSession.PlayerViewFromId(data[0]);
                var newAngy = data[1];
                NotifyAngyChange(playerView, newAngy);
            }
        }
    }
}