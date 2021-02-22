using System;
using Audio;
using Config;
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
        private Material[] _materials;

        public override void InvokeAbility(PlayerView player)
        {
            AudioManager.PlaySfx(SfxType.IceBlockActivate);

            _playerView = player;

            _playerView.BecameStill += OnBecameStill;

            _initialDrag = player.Drag;
            _playerView.Drag = GameConfig.Instance.AbilityValues.IceBlockAbilityConfig.Drag;

            _otherIceBlockOnCollision = _playerView.Ball.AddComponent<IceBlockOnCollision>();

            //TODO: Replace model/Play animation
            _materials = _playerView.Materials;
            _originalBodyMaterial = _materials[0];
            _materials[0] = GameConfig.Instance.AbilityValues.IceBlockAbilityConfig.IceMaterial;
            _playerView.Materials = _materials;
        }

        public void OnBecameStill()
        {
            AudioManager.PlaySfx(SfxType.IceBlockDeactivate);

            _playerView.BecameStill -= OnBecameStill;

            _playerView.Drag = _initialDrag;

            _materials[0] = _originalBodyMaterial;
            _playerView.Materials = _materials;

            Object.Destroy(_otherIceBlockOnCollision);
        }
    }
}