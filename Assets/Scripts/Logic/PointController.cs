using System;
using System.Collections.Generic;
using System.Linq;
using Ball.Objectives;
using GameSession;
using Player;
using TMPro;
using UnityEngine;

namespace Logic
{
    public class PointController : MonoBehaviour
    {
        public List<GoodNeutralMushroom> pointHolders;
        public List<TextMeshProUGUI> pointText;

        private const int RedId = 0, BlueId = 1;

        private readonly Dictionary<PlayerView, int> _playerPoints = new Dictionary<PlayerView, int>();
        private int _enemiesRemaining;

        private void Start()
        {
            pointHolders.AddRange(FindObjectsOfType<GoodNeutralMushroom>());
            _enemiesRemaining = pointHolders.Count;

            FindObjectOfType<PlayersManager>().InitializedAllPlayers += OnPlayersInitialized;
        }

        private void OnPlayersInitialized(PlayerView[] playerViews)
        {
            foreach (var playerView in playerViews)
            {
                _playerPoints.Add(playerView, 0);
            }
        }

        public void EnemyHit()
        {
            _enemiesRemaining--;
            if (_enemiesRemaining == 1)
            {
                foreach (var item in pointHolders)
                {
                    var enemy = item;
                    if (enemy.owner == null)
                    {
                        enemy.SpawnGoal();
                    }
                }
            }
        }

        public void UpdateScore()
        {
            PlayerView[] playerPoints = new PlayerView[_playerPoints.Keys.Count];
            _playerPoints.Keys.CopyTo(playerPoints, 0);

            foreach (var playerPoint in playerPoints)
            {
                _playerPoints[playerPoint] = 0;

                var playerId = CurrentGameSession.PlayerFromPlayerView(playerPoint).PresetIndex;
                if (pointText[0] != null)
                    pointText[playerId].text = _playerPoints[playerPoint].ToString();
            }

            foreach (var enemy in pointHolders)
            {
                if (enemy.owner != null)
                {
                    _playerPoints[enemy.owner] += enemy.pointValue;
                    if (pointText[0] != null)
                        pointText[CurrentGameSession.PlayerFromPlayerView(enemy.owner).PresetIndex].text =
                            _playerPoints[enemy.owner].ToString();
                }
            }
        }

        //TODO: 
        public IReadOnlyDictionary<PlayerView, int> GetPoints()
        {
            UpdateScore();
            return _playerPoints;
        }
    }
}