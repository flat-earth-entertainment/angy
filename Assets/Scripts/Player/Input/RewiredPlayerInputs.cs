using System;
using Rewired;
using UnityEngine;

namespace Player.Input
{
    public class RewiredPlayerInputs : MonoBehaviour, IPlayerInputs
    {
        public event Action AbilityButtonPressed;
        public event Action MapViewButtonPressed;

        private Rewired.Player _thisPlayer;

        /// <summary>
        /// Should be called after setting the Player ID!
        /// </summary>
        public static IPlayerInputs AttachToPlayer(PlayerView playerView)
        {
            if (playerView.gameObject.TryGetComponent(out RewiredPlayerInputs rewiredPlayerInputs))
            {
                rewiredPlayerInputs._thisPlayer = ReInput.players.GetPlayer(playerView.PlayerId);
            }
            else
            {
                rewiredPlayerInputs = playerView.gameObject.AddComponent<RewiredPlayerInputs>();
            }

            rewiredPlayerInputs._thisPlayer = ReInput.players.GetPlayer(playerView.PlayerId);

            return rewiredPlayerInputs;
        }

        private void Update()
        {
            if (_thisPlayer.GetButton("AbilityFire"))
            {
                AbilityButtonPressed?.Invoke();
            }

            if (_thisPlayer.GetButtonDown("CameraMode"))
            {
                MapViewButtonPressed?.Invoke();
            }
        }
    }
}