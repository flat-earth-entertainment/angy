﻿using System;
using Rewired;
using UnityEngine;

namespace Player.Input
{
    public class RewiredPlayerInputs : MonoBehaviour, IPlayerInputs
    {
        public event Action AbilityButtonPressed;
        public event Action MapViewButtonPressed;
        public event Action MenuButtonPressed;
        public event Action FireButtonPressed;

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
            if (_thisPlayer.GetButtonDown("AbilityFire"))
            {
                AbilityButtonPressed?.Invoke();
            }

            if (_thisPlayer.GetButtonDown("CameraMode"))
            {
                MapViewButtonPressed?.Invoke();
            }

            if (_thisPlayer.GetButtonDown("Menu"))
            {
                MenuButtonPressed?.Invoke();
            }

            if (_thisPlayer.GetButtonDown("Confirm"))
            {
                FireButtonPressed?.Invoke();
            }
        }
    }
}