using System.Collections.Generic;
using Ball.Objectives;
using Player;
using TMPro;
using UnityEngine;

namespace Logic
{
    public class PointController : MonoBehaviour
    {
        public List<GoodNeutralMushroom> pointHolders;
        public List<int> pointIds;
        public List<TextMeshProUGUI> pointText;

        private int _enemiesRemaining;

        private void Start()
        {
            pointHolders.AddRange(FindObjectsOfType<GoodNeutralMushroom>());
            _enemiesRemaining = pointHolders.Count;
            pointIds = new List<int>();
            for (var i = 0; i < GetComponent<PlayersManager>().Players.Count; i++)
            {
                pointIds.Add(0);
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
                    if (enemy.ownerId == 99)
                    {
                        enemy.SpawnGoal();

                        // Tell camera to focus on goal and freeze balls
                    }
                }
            }
        }

        public void UpdateScore()
        {
            for (var i = 0; i < pointIds.Count; i++)
            {
                pointIds[i] = 0;
                if (pointText[0] != null)
                    pointText[i].text = pointIds[i].ToString();
            }

            foreach (var item in pointHolders)
            {
                var enemy = item;
                if (enemy.ownerId < 98)
                {
                    pointIds[enemy.ownerId] += enemy.pointValue;
                    if (pointText[0] != null)
                        pointText[enemy.ownerId].text = pointIds[enemy.ownerId].ToString();
                }
            }
        }

        public List<int> GetPoints()
        {
            UpdateScore();
            return pointIds;
        }
    }
}