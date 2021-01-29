using System;
using System.Linq;
using UnityEngine;

namespace Ball.Objectives
{
    public class Hole : MonoBehaviour
    {
        public static event Action<PlayerView> PlayerEnteredHole;

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out Shooter shooter))
            {
                var player = FindObjectsOfType<PlayerView>().FirstOrDefault(p => p.PlayerId == shooter.playerId);

                if (player != null) PlayerEnteredHole?.Invoke(player);
            }
        }
    }
}