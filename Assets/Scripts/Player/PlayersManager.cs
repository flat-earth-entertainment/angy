using System;
using System.Collections.Generic;
using Config;
using GameSession;
using Photon.Pun;
using Player.Input;
using UnityEngine;

namespace Player
{
    public class PlayersManager : MonoBehaviour
    {
        public IReadOnlyList<PlayerView> Players => _players.Count > 0 ? _players.AsReadOnly() : null;

        private readonly List<PlayerView> _players = new List<PlayerView>();

        public event Action<PlayerView[]> InitializedAllPlayers;

        private int _rewiredPlayerIdCounter;
        private Transform _spawnPoint;

        private void Awake()
        {
            _spawnPoint = GameObject.FindWithTag("Spawn Point").transform;
        }

        private void Start()
        {
            if (!PhotonNetwork.OfflineMode && !PhotonNetwork.IsMasterClient)
            {
#if UNITY_EDITOR
                if (!PhotonNetwork.IsConnected && !PhotonNetwork.OfflineMode)
                {
                    Debug.LogWarning(
                        "Looks like you're testing the map, if not and you see this please contact the programmer!");
                    PhotonNetwork.OfflineMode = true;
                    PhotonNetwork.JoinRandomRoom();
                    GenerateCharacterDummies();
                }
#endif
                return;
            }

            GenerateCharacterDummies();
        }

        private void GenerateCharacterDummies()
        {
            foreach (var player in CurrentGameSession.Players)
            {
                var addresseeActorNumber = player is OnlinePlayer onlinePlayer
                    ? onlinePlayer.PhotonPlayer.ActorNumber
                    : PhotonNetwork.LocalPlayer.ActorNumber;

                SpawnNewPlayer(addresseeActorNumber);
            }
        }


        private void InitializeNewPlayer(PlayerView player, PlayerPreset playerPreset)
        {
            player.PlayerPreset = playerPreset;
            player.Knockback = GameConfig.Instance.ExplosionForceOnPlayerHit;
            player.ChangeStateAndNotify(PlayerState.ShouldSpawnAtSpawn);

            player.Hide();

            _players.Add(player);
        }

        private GameObject SpawnNewPlayer(int addresseeActorNumber)
        {
            var newPlayerObject =
                PhotonNetwork.Instantiate("Player", _spawnPoint.position, Quaternion.identity, 0,
                    new object[] {addresseeActorNumber});

            var playerId = _players.Count;
            newPlayerObject.name = "Player " + playerId;
            return newPlayerObject;
        }

        public PlayerView GetNextPlayer(PlayerView currentPlayerView)
        {
            if (currentPlayerView == null)
            {
                return CurrentGameSession.NextRoundPlayer != null
                    ? CurrentGameSession.NextRoundPlayer.RoundPlayerView
                    : _players[0];
            }

            return _players[(_players.IndexOf(currentPlayerView) + 1) % _players.Count];
        }

        public void PrepareTrajectoryFor(PlayerView currentPlayerView)
        {
            foreach (var player in _players)
            {
                player.ShouldPlayerActivate(currentPlayerView);
            }
        }

        public void AddNewPlayer(PlayerView playerView)
        {
            var presetId = CurrentGameSession.PlayerFromPlayerView(playerView).PresetIndex;
            InitializeNewPlayer(playerView, GameConfig.Instance.PlayerPresets[presetId]);

            if (_players.Count == CurrentGameSession.Players.Length)
            {
                InitializedAllPlayers?.Invoke(_players.ToArray());
            }
        }
    }
}