using System;
using System.Threading;
using System.Threading.Tasks;
using Config;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Abilities
{
    [Serializable]
    public class ExpandAbility : Ability
    {
        public float TimeToInflate => GameConfig.Instance.AbilityValues.ExpandAbility.TimeToInflate;

        public float TimeToDeflate => GameConfig.Instance.AbilityValues.ExpandAbility.TimeToDeflate;

        public float Duration => GameConfig.Instance.AbilityValues.ExpandAbility.Duration;

        public float Scale => GameConfig.Instance.AbilityValues.ExpandAbility.Scale;

        private CancellationTokenSource _endOfTurn;
        private PlayerView _player;
        private Vector3 _initialScale;
        private bool _deflated;

        public override async void InvokeAbility(PlayerView player)
        {
            _endOfTurn = new CancellationTokenSource();
            _player = player;
            player.BecameStill += OnPlayerBecameStill;

            _initialScale = player.Ball.transform.localScale;
            await player.Ball.transform.DOScale(Scale, TimeToInflate).SetEase(Ease.OutElastic)
                .SetUpdate(UpdateType.Fixed);

            await UniTask.Delay(TimeSpan.FromSeconds(Duration), DelayType.UnscaledDeltaTime,
                cancellationToken: _endOfTurn.Token).SuppressCancellationThrow();

            await Deflate();

            _deflated = true;
        }

        private async UniTask Deflate()
        {
            await _player.Ball.transform.DOScale(_initialScale, TimeToDeflate).SetUpdate(UpdateType.Fixed);
        }

        private void OnPlayerBecameStill()
        {
            _player.BecameStill -= OnPlayerBecameStill;

            if (!_deflated)
            {
                _endOfTurn.Cancel();
#pragma warning disable CS4014
                Deflate();
#pragma warning restore CS4014
            }
        }
    }
}