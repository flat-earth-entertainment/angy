﻿using System;
using System.Threading;
using Audio;
using Config;
using Cysharp.Threading.Tasks;
using Player;
using UnityEngine;

namespace Abilities
{
    [Serializable]
    public class NoGravityAbility : Ability
    {
        private CancellationTokenSource _cancellationTokenSource;
        private Vector3 _initialGravity;
        private Material _originalMaterial;
        private PlayerView _playerView;

        protected override async void InvokeAbility(PlayerView player)
        {
            Active = true;
            _playerView = player;
            _playerView.PlayerInputs.AbilityButtonPressed += WrapInternal;

            _cancellationTokenSource = new CancellationTokenSource();

            _originalMaterial = player.Materials[0];
            player.SetBodyMaterial(GameConfig.Instance.AbilityValues.NoGravityAbility.Material);
            _initialGravity = Physics.gravity;
            Physics.gravity = Vector3.zero;
            AudioManager.Instance.DoLowPass(.5f);
            await UniTask.Delay(TimeSpan.FromSeconds(GameConfig.Instance.AbilityValues.NoGravityAbility.DurationTime),
                DelayType.UnscaledDeltaTime,
                cancellationToken: _cancellationTokenSource.Token).SuppressCancellationThrow();

            WrapInternal();
        }

        protected override void WrapInternal()
        {
            _cancellationTokenSource.Cancel();
            _playerView.SetBodyMaterial(_originalMaterial);
            _playerView.PlayerInputs.AbilityButtonPressed -= WrapInternal;
            AudioManager.Instance.UndoLowPass(.5f);
            Physics.gravity = _initialGravity;
            Finished = true;
            Active = false;
            IsFinalized = true;
        }
    }
}