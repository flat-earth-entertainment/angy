using Cinemachine;
using Cinemachine.Utility;
using Rewired;
using UnityEngine;

namespace Player
{
    public class PanController : MonoBehaviour
    {
        [field: SerializeField]
        public CinemachineVirtualCamera PanningCamera { get; private set; }

        [SerializeField]
        private float speed;

        [SerializeField]
        private Vector3 bottomLeftBound;

        [SerializeField]
        private Vector3 topRightBound;


        private Transform _cameraTransform;
        private Rewired.Player _player;

        private void Awake()
        {
            _cameraTransform = PanningCamera.transform;
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

        private void Update()
        {
            var currentPosition = _cameraTransform.localPosition;

            var vertical = _player.GetAxis("Move Vertical") * Time.deltaTime * speed;
            if (vertical + currentPosition.y < bottomLeftBound.y || vertical + currentPosition.y > topRightBound.y)
            {
                vertical = 0f;
            }

            var horizontal = _player.GetAxis("Move Horizontal") * Time.deltaTime * speed;
            if (horizontal + currentPosition.x < bottomLeftBound.x || horizontal + currentPosition.x > topRightBound.x)
            {
                horizontal = 0f;
            }


            _cameraTransform.position += _cameraTransform.up * vertical + _cameraTransform.right * horizontal;

            var newPosition = _cameraTransform.localPosition;
            newPosition.z = Mathf.LerpUnclamped(bottomLeftBound.z, topRightBound.z,
                ClosestPointOnSegment(newPosition, bottomLeftBound, topRightBound));

            _cameraTransform.localPosition = newPosition;
        }

        public static float ClosestPointOnSegment(Vector3 p, Vector3 s0, Vector3 s1)
        {
            Vector3 s = s1 - s0;
            float len2 = Vector3.SqrMagnitude(s);
            if (len2 < Mathf.Epsilon)
                return 0; // degenrate segment
            return Vector3.Dot(p - s0, s) / len2;
        }
    }
}