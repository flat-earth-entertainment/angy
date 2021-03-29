using UnityEngine;

namespace Player
{
    public class LastStandablePositionSetter : MonoBehaviour
    {
        private static int _notStandableLayerMask;

        [SerializeField]
        private LayerMask ground;

        [SerializeField]
        private PlayerView player;

        private void Awake()
        {
            _notStandableLayerMask = LayerMask.GetMask("NotRespawnable");
        }

        private void FixedUpdate()
        {
            if (Physics.Raycast(transform.position + Vector3.up * 50, Vector3.down, 200, _notStandableLayerMask))
            {
                return;
            }

            if (Physics.Raycast(transform.position + Vector3.up * 50, Vector3.down, out var hit, 200, ground))
            {
                if (hit.transform.gameObject.layer == LayerMask.NameToLayer("IgnoredMap")
                    || hit.transform.gameObject.layer == LayerMask.NameToLayer("Ground"))
                {
                    player.LastStandablePosition = hit.point;
                }
            }
        }
    }
}