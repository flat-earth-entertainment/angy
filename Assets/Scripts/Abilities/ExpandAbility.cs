using System;
using System.Threading;
using Audio;
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

        protected override async void InvokeAbility(PlayerView player)
        {
            _endOfTurn = new CancellationTokenSource();
            _player = player;
            player.BecameStill += OnPlayerBecameStill;

            _initialScale = player.Ball.transform.localScale;

            AudioManager.PlaySfx(SfxType.ExpandActivate);

            player.Animator.SetBool("isInflated", true);

            DOTween.To(() => player.ExpandPercent, f => player.ExpandPercent = f, 100, TimeToInflate)
                .SetEase(Ease.OutElastic)
                .SetUpdate(UpdateType.Fixed);

            DOTween.To(() => _player.Knockback, f => _player.Knockback = f,
                    _player.Knockback * GameConfig.Instance.AbilityValues.ExpandAbility.KnockbackMultiplier,
                    TimeToInflate)
                .SetEase(Ease.OutElastic)
                .SetUpdate(UpdateType.Fixed);

            // Expand mass
            _player.BallRigidbody.mass = 10;

            DOTween.To(f => _player.BallRigidbody.mass = f, 1f, (float) 1e+8, 1f)
                .SetUpdate(UpdateType.Fixed);

            await player.Ball.transform.DOScale(Scale, TimeToInflate).SetEase(Ease.OutElastic)
                .SetUpdate(UpdateType.Fixed);

            await UniTask.Delay(TimeSpan.FromSeconds(Duration), DelayType.UnscaledDeltaTime,
                cancellationToken: _endOfTurn.Token).SuppressCancellationThrow();

            await Deflate();
        }

        private async UniTask Deflate()
        {
            // Lower Mass
            DOTween.To(() => _player.BallRigidbody.mass, f => _player.BallRigidbody.mass = f, 1f, 1f)
                .SetUpdate(UpdateType.Fixed).OnComplete(delegate { _player.BallRigidbody.mass = 1; });

            AudioManager.PlaySfx(SfxType.ExpandDeactivate);

            _player.Animator.SetBool("isInflated", false);

            DOTween.To(() => _player.ExpandPercent, f => _player.ExpandPercent = f, 0, TimeToInflate)
                .SetEase(Ease.OutElastic)
                .SetUpdate(UpdateType.Fixed);

            DOTween.To(() => _player.Knockback, f => _player.Knockback = f,
                    GameConfig.Instance.ExplosionForceOnPlayerHit, TimeToInflate)
                .SetEase(Ease.OutElastic)
                .SetUpdate(UpdateType.Fixed);

            await _player.BallRigidbody.transform.DOScale(_initialScale, TimeToDeflate).SetUpdate(UpdateType.Fixed);
            _deflated = true;
            Finished = true;
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