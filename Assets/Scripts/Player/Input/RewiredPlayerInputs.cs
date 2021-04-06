using System;
using Logic;
using Rewired;
using UnityEngine;
using Utils;

namespace Player.Input
{
    public class RewiredPlayerInputs : MonoBehaviour, IPlayerInputs
    {
        public event Action AbilityButtonPressed;
        public event Action MapViewButtonPressed;
        public event Action MenuButtonPressed;
        public event Action FireButtonPressed;
        public event Action<float> HorizontalAxisInput;
        public event Action<float> VerticalAxisInput;

        private Rewired.Player _thisPlayer;

        public static IPlayerInputs AttachToPlayer(PlayerView playerView, int rewiredPlayerId)
        {
            RewiredPlayerInputs rewiredPlayerInputs;
            if (playerView.gameObject.TryGetComponent(out rewiredPlayerInputs))
            {
            }
            else
            {
                rewiredPlayerInputs = playerView.gameObject.AddComponent<RewiredPlayerInputs>();
            }

            rewiredPlayerInputs._thisPlayer = ReInput.players.GetPlayer(rewiredPlayerId);

            return rewiredPlayerInputs;
        }

        private void Update()
        {
            if (_thisPlayer.GetButtonDown("AbilityFire"))
            {
                AbilityButtonPressed?.Invoke();
                PhotonShortcuts.ReliableRaiseEventToOthers(GameEvent.PlayerAbilityButtonPressed);
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

            HorizontalAxisInput?.Invoke(_thisPlayer.GetAxis("Move Horizontal"));
            VerticalAxisInput?.Invoke(_thisPlayer.GetAxis("Move Vertical"));
        }
    }
}