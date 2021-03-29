using Cinemachine;
using Config;
using Rewired;
using UnityEngine;

namespace Player
{
    public class PanController : MonoBehaviour
    {
        [SerializeField]
        private Vector3 bottomLeftBound;

        [SerializeField]
        private Vector3 topRightBound;

        private Rewired.Player _player;

        [field: SerializeField]
        public CinemachineVirtualCamera PanningCamera { get; private set; }

        private void Update()
        {
            var currentPosition = transform.localPosition;

            var vertical = _player.GetAxis("Move Vertical") * Time.deltaTime
                                                            * GameConfig.Instance.CameraPanningSpeed;

            if (vertical + currentPosition.z <= bottomLeftBound.z ||
                vertical + currentPosition.z >= topRightBound.z)
            {
                vertical = 0f;
            }

            var horizontal = _player.GetAxis("Move Horizontal") * Time.deltaTime *
                             GameConfig.Instance.CameraPanningSpeed;
            if (horizontal + currentPosition.x <= bottomLeftBound.x ||
                horizontal + currentPosition.x >= topRightBound.x)
            {
                horizontal = 0f;
            }

            transform.position += transform.forward * vertical + transform.right * horizontal;
        }

        public void EnableControls(PlayerView player)
        {
            _player = ReInput.players.GetPlayer(player.PlayerId);
            enabled = true;
        }

        public void DisableControls()
        {
            _player = null;
            enabled = false;
        }
    }
}