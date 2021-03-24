using System.Linq;
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
        private Material _originalBodyMaterial;
        private GameObject _trail;
        private bool _pressedLaunch;

        protected override async void InvokeAbility(PlayerView player)
        {
            Active = true;

            _player = player;
            PlayerView.NewAbilitySet += OnAbilityOverride;

            _cancellationTokenSource = new CancellationTokenSource();
            Time.timeScale = 0;

            var flatVelocityVector = player.BallRigidbody.velocity;
            flatVelocityVector.y = 0;

            _fireDashClock = Object.Instantiate(
                    GameConfig.Instance.AbilityValues.FireDashAbilityConfig.FireDashControlsPrefab,
                    player.Ball.transform.position,
                    Quaternion.FromToRotation(Vector3.forward, flatVelocityVector))
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

            Launch(_pressedLaunch);
        }

        private void OnAbilityOverride(PlayerView player, Ability ability)
        {
            PlayerView.NewAbilitySet -= OnAbilityOverride;

            if (_player == player && ability != this)
            {
                Object.Destroy(_trail);
                _trail = null;
                _player.SetBodyMaterial(_originalBodyMaterial);
            }
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

            var playerPreset = GameConfig.Instance.PlayerPresets.First(p => p.PlayerColor == _player.PlayerColor);
            _trail = Object.Instantiate(playerPreset.Trail, _player.Ball.transform);

            _originalBodyMaterial = _player.Materials[0];
            _player.SetBodyMaterial(playerPreset.FireMaterial);

            _player.BecameStill += OnBecameStill;

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

        private void OnBecameStill()
        {
            _player.BecameStill -= OnBecameStill;
            _player.SetBodyMaterial(_originalBodyMaterial);

            if (_trail != null)
            {
                Object.Destroy(_trail);
            }
        }

        private void OnLaunchPressed()
        {
            _cancellationTokenSource.Cancel();

            _rotateTween.Kill();
            _rotateTween = null;

            _pressedLaunch = true;
        }
    }
}