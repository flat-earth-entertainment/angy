using System;
using System.Threading;
using Abilities.Config;
using Config;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Abilities
{
    public class FireDashAbility : Ability
    {
        private Tween _rotateTween;
        private PlayerView _player;
        private CancellationTokenSource _cancellationTokenSource;
        private Transform _fireDashClock;

        protected override async void InvokeAbility(PlayerView player)
        {
            Active = true;

            _player = player;

            _cancellationTokenSource = new CancellationTokenSource();
            Time.timeScale = 0;

            _fireDashClock = Object.Instantiate(
                    GameConfig.Instance.AbilityValues.FireDashAbilityConfig.FireDashControlsPrefab,
                    player.Ball.transform.position,
                    Quaternion.Euler(0, player.Animator.transform.parent.transform.rotation.eulerAngles.y, 0))
                .transform;

            await DOTween.To(() => Time.timeScale, t => Time.timeScale = t, 0,
                GameConfig.Instance.AbilityValues.FireDashAbilityConfig.EnterTime).SetUpdate(true);

            switch (GameConfig.Instance.AbilityValues.FireDashAbilityConfig.LaunchButton)
            {
                case FireDashAbilityConfig.LaunchButtonEnum.Shoot:
                    player.PlayerInputs.FireButtonPressed += OnLaunchPressed;
                    break;
                case FireDashAbilityConfig.LaunchButtonEnum.Ability:
                    player.PlayerInputs.AbilityButtonPressed += OnLaunchPressed;
                    break;
            }

            _rotateTween = _fireDashClock.DORotate(new Vector3(0, 360, 0),
                    GameConfig.Instance.AbilityValues.FireDashAbilityConfig.RotationTime)
                .SetRelative()
                .SetUpdate(true)
                .SetEase(Ease.Linear);

            await _rotateTween.ToUniTask(cancellationToken: _cancellationTokenSource.Token).SuppressCancellationThrow();

            Debug.Log("did not press");
            Launch(false);
        }

        private void Launch(bool pressedButton)
        {
            if (!pressedButton && GameConfig.Instance.AbilityValues.FireDashAbilityConfig.LaunchAfterNoInput ||
                pressedButton)
            {
                _player.BallRigidbody.velocity = _fireDashClock.transform.forward *
                                                 GameConfig.Instance.AbilityValues.FireDashAbilityConfig.PushForce;
            }

            Object.Destroy(_fireDashClock.gameObject);
            Time.timeScale = GameConfig.Instance.TimeScale;

            switch (GameConfig.Instance.AbilityValues.FireDashAbilityConfig.LaunchButton)
            {
                case FireDashAbilityConfig.LaunchButtonEnum.Shoot:
                    _player.PlayerInputs.FireButtonPressed -= OnLaunchPressed;
                    break;
                case FireDashAbilityConfig.LaunchButtonEnum.Ability:
                    _player.PlayerInputs.AbilityButtonPressed -= OnLaunchPressed;
                    break;
            }

            Active = false;
            Finished = true;
        }

        private void OnLaunchPressed()
        {
            _rotateTween.Kill();
            _rotateTween = null;

            _cancellationTokenSource.Cancel();

            Launch(true);
        }
    }
}