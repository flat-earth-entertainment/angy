using System.Collections.Generic;
using Config;
using UnityEngine;

public class PlayersManager : MonoBehaviour
{
    [SerializeField]
    private int numberOfPlayers = 2;

    [SerializeField]
    private GameObject obstacles;

    private readonly List<PlayerView> _players = new List<PlayerView>();

    private void Awake()
    {
        for (int i = 0; i < numberOfPlayers; i++)
        {
            InitializeNewPlayer();
        }

        PredictionManager.instance.obstacles = obstacles;
    }

    private void InitializeNewPlayer()
    {
        var newPlayerObject = Instantiate(GameConfig.Instance.PlayerPrefab);
        var playerId = _players.Count;
        newPlayerObject.name = "Player " + playerId;

        if (newPlayerObject.TryGetComponent(out PlayerView player))
        {
            player.SetId(playerId);

            player.PlayerState = PlayerState.ShouldSpawn;

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