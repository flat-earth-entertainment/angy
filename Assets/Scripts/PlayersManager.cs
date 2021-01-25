using System.Collections.Generic;
using UnityEngine;

public class PlayersManager : MonoBehaviour
{
    [SerializeField]
    private GameObject playerPrefab;

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
    }

    private void InitializeNewPlayer()
    {
        var newPlayerObject = Instantiate(playerPrefab);
        var playerId = _players.Count;
        newPlayerObject.name = "Player " + playerId;

        if (newPlayerObject.TryGetComponent(out PlayerView player))
        {
            player.SetId(playerId);
            // player.SetBallPosition(spawnPointPosition);
            player.PlayerState = PlayerState.Hidden;
            newPlayerObject.GetComponentInChildren<PredictionManager>().obstacles = obstacles;

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
}