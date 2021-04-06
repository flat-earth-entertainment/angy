using Cinemachine;
using Config;
using UnityEngine;

namespace Player
{
    public class PanController : MonoBehaviour
    {
        [SerializeField]
        private Vector3 bottomLeftBound;

        [SerializeField]
        private Vector3 topRightBound;

        [field: SerializeField]
        public CinemachineVirtualCamera PanningCamera { get; private set; }

        private PlayerView _player;
        private float _horizontalInput;
        private float _verticalInput;

        private void OnHorizontalInput(float input)
        {
            _horizontalInput = input * Time.deltaTime * GameConfig.Instance.CameraPanningSpeed;
        }

        private void OnVerticalInput(float input)
        {
            _verticalInput = input * Time.deltaTime * GameConfig.Instance.CameraPanningSpeed;
        }

        private void Update()
        {
            var currentPosition = transform.localPosition;

            if (_verticalInput + currentPosition.z <= bottomLeftBound.z ||
                _verticalInput + currentPosition.z >= topRightBound.z)
            {
                _verticalInput = 0f;
            }

            if (_horizontalInput + currentPosition.x <= bottomLeftBound.x ||
                _horizontalInput + currentPosition.x >= topRightBound.x)
            {
                _horizontalInput = 0f;
            }

            transform.position += transform.forward * _verticalInput + transform.right * _horizontalInput;
        }

        public void EnableControls(PlayerView player)
        {
            _player = player;
            enabled = true;

            _player.PlayerInputs.HorizontalAxisInput += OnHorizontalInput;
            _player.PlayerInputs.VerticalAxisInput += OnVerticalInput;
        }

        public void DisableControls()
        {
            _player.PlayerInputs.HorizontalAxisInput -= OnHorizontalInput;
            _player.PlayerInputs.VerticalAxisInput -= OnVerticalInput;

            _player = null;
            enabled = false;
        }
    }
}