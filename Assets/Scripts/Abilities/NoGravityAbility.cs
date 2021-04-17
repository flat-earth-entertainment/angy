﻿using System;
using System.Threading;
using Audio;
using Config;
using Cysharp.Threading.Tasks;
using GameSession;
using Logic;
using Player;
using UnityEngine;
using Utils;

namespace Abilities
{
    [Serializable]
    public class NoGravityAbility : Ability
    {
        private CancellationTokenSource _cancellationTokenSource;
        private Material _originalMaterial;
        private PlayerView _playerView;
        private PhotonEventListener _photonEventListener;

        protected override async void InvokeAbility(PlayerView player)
        {
            Active = true;
            _playerView = player;
            _playerView.PlayerInputs.AbilityButtonPressed += WrapInternal;
            _photonEventListener =
                PhotonEventListener.ListenTo(GameEvent.PlayerAbilityButtonPressed, data =>
                {
                    if (CurrentGameSession.PlayerFromPlayerView(_playerView).Id == (int) data.CustomData)
                    {
                        WrapInternal();
                    }
                });

            _cancellationTokenSource = new CancellationTokenSource();

            _originalMaterial = player.Materials[0];
            player.SetBodyMaterial(GameConfig.Instance.AbilityValues.NoGravityAbility.Material);
            player.BallRigidbody.useGravity = false;
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
            _photonEventListener.StopListening();

            AudioManager.Instance.UndoLowPass(.5f);
            _playerView.BallRigidbody.useGravity = true;

            Finished = true;
            Active = false;
            IsFinalized = true;
        }
    }
}