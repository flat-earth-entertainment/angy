using System;
using System.Collections.Generic;
using Config;
using Player.Input;
using UnityEngine;

namespace Player
{
    public class PlayersManager : MonoBehaviour
    {
        [SerializeField]
        private int numberOfPlayers = 2;

        private readonly List<PlayerView> _players = new List<PlayerView>();


        public IReadOnlyList<PlayerView> Players => _players.AsReadOnly();

        private void Awake()
        {
            for (var i = 0; i < numberOfPlayers; i++)
            {
                InitializeNewPlayer(GameConfig.Instance.PlayerPresets[i]);
            }

            InitializedAllPlayers?.Invoke(_players.ToArray());
        }

        public event Action<PlayerView[]> InitializedAllPlayers;

        private void InitializeNewPlayer(PlayerPreset playerPreset)
        {
            var newPlayerObject = Instantiate(GameConfig.Instance.PlayerPrefab);
            var playerId = _players.Count;
            newPlayerObject.name = "Player " + playerId;

            if (newPlayerObject.TryGetComponent(out PlayerView player))
            {
                player.PlayerId = playerId;
                player.PlayerPreset = playerPreset;
                player.Knockback = GameConfig.Instance.ExplosionForceOnPlayerHit;
                player.PlayerState = PlayerState.ShouldSpawnAtSpawn;
                player.PlayerInputs = RewiredPlayerInputs.AttachToPlayer(player);

                player.Hide();

                _players.Add(player);
            }
            else
            {
                Debug.LogError("Can't find Player script in the player prefab!");
            }
        }

        public PlayerView GetNextPlayer(PlayerView currentPlayerView)
        {
            if (currentPlayerView == null)
            {
                if (CurrentGameSession.NextRoundRewiredPlayerId != null)
                {
                    var foundPlayer =
                        _players.Find(p => p.RewiredPlayer.id == CurrentGameSession.NextRoundRewiredPlayerId);
                    return foundPlayer;
                }

                return _players[0];
            }

            return _players[(_players.IndexOf(currentPlayerView) + 1) % _players.Count];
        }

        public void PrepareTrajectoryFor(PlayerView currentPlayerView)
        {
            foreach (var player in _players)
            {
                player.ShouldPlayerActivate(currentPlayerView.PlayerId);
            }
        }
    }
}