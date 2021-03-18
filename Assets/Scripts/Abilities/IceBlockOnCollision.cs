using Audio;
using Config;
using Player;
using UnityEngine;

namespace Abilities
{
    public class IceBlockOnCollision : MonoBehaviour
    {
        private float _initialDrag;
        private PlayerView _otherPlayerView;
        private IceBlockOnCollision _otherIceBlockOnCollision;
        private Material _originalBodyMaterial;
        private Material[] _materials;

        private void OnCollisionEnter(Collision other)
        {
            if (other.collider.CompareTag("Lemming") && !other.transform.GetComponent<IceBlockOnCollision>())
            {
                AudioManager.PlaySfx(SfxType.IceBlockActivate);

                _initialDrag = other.rigidbody.drag;
                other.rigidbody.drag = GameConfig.Instance.AbilityValues.IceBlockAbility.Drag;

                _otherPlayerView = other.transform.GetComponentInChildren<Shooter>().PlayerView;
                _otherPlayerView.BecameStill += OnBecameStill;

                _otherIceBlockOnCollision = other.gameObject.AddComponent<IceBlockOnCollision>();

                _materials = _otherPlayerView.Materials;
                _originalBodyMaterial = _materials[0];
                _materials[0] = GameConfig.Instance.AbilityValues.IceBlockAbility.IceMaterial;
                _otherPlayerView.Materials = _materials;
            }
        }

        private void OnBecameStill()
        {
            AudioManager.PlaySfx(SfxType.IceBlockDeactivate);

            _otherPlayerView.BecameStill -= OnBecameStill;
            _otherPlayerView.Drag = _initialDrag;

            _materials[0] = _originalBodyMaterial;
            _otherPlayerView.Materials = _materials;

            Destroy(_otherIceBlockOnCollision);
        }
    }
}