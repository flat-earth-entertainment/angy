using System;
using UnityEngine;

namespace Ball.Objectives
{
    public class Hole : MonoBehaviour
    {
        public static event Action<PlayerView> PlayerEnteredHole;
        public static event Action<PlayerView> PlayerLeftHole;

        private void OnTriggerEnter(Collider other)
        {
            if (other.transform.childCount >= 1)
            {
                if (other.transform.GetChild(0).TryGetComponent(out Shooter shooter))
                {
                    PlayerEnteredHole?.Invoke(shooter.PlayerView);
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.transform.childCount >= 1)
            {
                if (other.transform.GetChild(0).TryGetComponent(out Shooter shooter))
                {
                    PlayerLeftHole?.Invoke(shooter.PlayerView);
                }
            }
        }
    }
}