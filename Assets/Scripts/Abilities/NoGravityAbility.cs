using System;
using System.Threading;
using Audio;
using Config;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Abilities
{
    [Serializable]
    public class NoGravityAbility : Ability
    {
        public float DurationTime => GameConfig.Instance.AbilityValues.NoGravityAbility.DurationTime;

        private PlayerView _playerView;
        private Vector3 _initialGravity;
        private CancellationTokenSource _cancellationTokenSource;

        public override async void InvokeAbility(PlayerView player)
        {
            _playerView = player;
            _playerView.PlayerInputs.AbilityButtonPressed += DisableAbility;

            _cancellationTokenSource = new CancellationTokenSource();

            _initialGravity = Physics.gravity;
            Physics.gravity = Vector3.zero;
            AudioManager.Instance.DoLowPass(.5f);
            await UniTask.Delay(TimeSpan.FromSeconds(DurationTime), DelayType.UnscaledDeltaTime,
                cancellationToken: _cancellationTokenSource.Token).SuppressCancellationThrow();

            DisableAbility();
        }

        private void DisableAbility()
        {
            _cancellationTokenSource.Cancel();
            _playerView.PlayerInputs.AbilityButtonPressed -= DisableAbility;
            AudioManager.Instance.UndoLowPass(.5f);
            Physics.gravity = _initialGravity;
        }
    }
}