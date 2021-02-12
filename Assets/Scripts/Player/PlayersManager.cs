using System;
using System.Collections.Generic;
using Config;
using Player;
using Player.Input;
using UnityEngine;

public class PlayersManager : MonoBehaviour
{
    public event Action<PlayerView[]> InitializedAllPlayers;

    [SerializeField]
    private int numberOfPlayers = 2;


    public IReadOnlyList<PlayerView> Players => _players.AsReadOnly();

    private readonly List<PlayerView> _players = new List<PlayerView>();

    private void Awake()
    {
        for (int i = 0; i < numberOfPlayers; i++)
        {
            InitializeNewPlayer(GameConfig.Instance.PlayerPresets[i]);
        }

        InitializedAllPlayers?.Invoke(_players.ToArray());
    }

    private void InitializeNewPlayer(PlayerPreset playerPreset)
    {
        var newPlayerObject = Instantiate(GameConfig.Instance.PlayerPrefab);
        var playerId = _players.Count;
        newPlayerObject.name = "Player " + playerId;

        if (newPlayerObject.TryGetComponent(out PlayerView player))
        {
            player.PlayerId = playerId;
            player.Nickname = playerPreset.PlayerName;
            player.PlayerColor = playerPreset.PlayerColor;
            player.PlayerState = PlayerState.ShouldSpawnFirstTime;
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