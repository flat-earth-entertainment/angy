using System;
using Audio;
using Config;
using Player;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Abilities
{
    [Serializable]
    public class IceBlockAbility : Ability
    {
        private float _initialDrag;
        private PlayerView _playerView;
        private IceBlockOnCollision _otherIceBlockOnCollision;
        private Material _originalBodyMaterial;

        protected override void InvokeAbility(PlayerView player)
        {
            Active = true;

            AudioManager.PlaySfx(SfxType.IceBlockActivate);

            _playerView = player;

            _playerView.BecameStill += OnBecameStill;

            _initialDrag = player.Drag;
            _playerView.Drag = GameConfig.Instance.AbilityValues.IceBlockAbility.Drag;

            _otherIceBlockOnCollision = _playerView.Ball.AddComponent<IceBlockOnCollision>();

            //TODO: Replace model/Play animation
            _originalBodyMaterial = player.Materials[0];
            player.SetBodyMaterial(GameConfig.Instance.AbilityValues.IceBlockAbility.IceMaterial);
        }

        public void OnBecameStill()
        {
            AudioManager.PlaySfx(SfxType.IceBlockDeactivate);

            _playerView.BecameStill -= OnBecameStill;

            _playerView.Drag = _initialDrag;

            _playerView.SetBodyMaterial(_originalBodyMaterial);

            Object.Destroy(_otherIceBlockOnCollision);

            Finished = true;
            Active = false;
        }
    }
}