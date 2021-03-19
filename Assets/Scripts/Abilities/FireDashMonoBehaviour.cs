using Player.Input;
using UnityEngine;

namespace Abilities
{
    public class FireDashMonoBehaviour : MonoBehaviour
    {
        [SerializeField]
        private float rotationSpeed;

        private IPlayerInputs _playerInputs;

        public void Initialize(IPlayerInputs playerInputs)
        {
            _playerInputs = playerInputs;
            _playerInputs.HorizontalAxisInput += OnHorizontalInput;
        }

        private void OnDisable()
        {
            _playerInputs.HorizontalAxisInput -= OnHorizontalInput;
        }

        private void OnHorizontalInput(float input)
        {
            transform.Rotate(0f, Time.unscaledDeltaTime * rotationSpeed * input, 0f);
        }
    }
}