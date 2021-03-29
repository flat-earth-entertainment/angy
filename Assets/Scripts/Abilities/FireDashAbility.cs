using System.Linq;
using System.Threading;
using Audio;
using Config;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Player;
using UnityEngine;

namespace Abilities
{
    public class FireDashAbility : Ability
    {
        private CancellationTokenSource _cancellationTokenSource;
        private AudioSource _cracklingSource;
        private Transform _fireDashClock;
        private Material _originalBodyMaterial;
        private PlayerView _player;
        private bool _pressedLaunch;
        private Tween _rotateTween;
        private GameObject _trail;

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

            player.PlayerInputs.FireButtonPressed += OnLaunchPressed;
            player.PlayerInputs.AbilityButtonPressed += OnLaunchPressed;

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
                WrapInternal();
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

            AudioManager.PlaySfx(SfxType.FireDashActivate);
            _cracklingSource = _player.gameObject.AddComponent<AudioSource>();
            _cracklingSource.clip = GameConfig.Instance.AbilityValues.FireDashAbilityConfig.CracklingClip;
            _cracklingSource.volume = GameConfig.Instance.AbilityValues.FireDashAbilityConfig.CracklingVolume;
            _cracklingSource.Play();
            _cracklingSource.loop = true;

            Object.Destroy(_fireDashClock.gameObject);
            Time.timeScale = GameConfig.Instance.TimeScale;

            var playerPreset = GameConfig.Instance.PlayerPresets.First(p => p.PlayerColor == _player.PlayerColor);
            _trail = Object.Instantiate(playerPreset.Trail, _player.Ball.transform);

            _originalBodyMaterial = _player.Materials[0];
            _player.SetBodyMaterial(playerPreset.FireMaterial);

            _player.BecameStill += WrapInternal;

            _player.PlayerInputs.FireButtonPressed -= OnLaunchPressed;
            _player.PlayerInputs.AbilityButtonPressed -= OnLaunchPressed;

            Active = false;
            Finished = true;
        }

        protected override void WrapInternal()
        {
            Debug.Log(_player == null || _player.Equals(null));
            _player.BecameStill -= WrapInternal;

            _player.SetBodyMaterial(_originalBodyMaterial);

            Object.Destroy(_cracklingSource);

            if (_trail != null)
            {
                Object.Destroy(_trail);
            }

            IsFinalized = true;
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