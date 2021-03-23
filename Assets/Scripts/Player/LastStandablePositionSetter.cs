using UnityEngine;

namespace Player
{
    public class LastStandablePositionSetter : MonoBehaviour
    {
        [SerializeField]
        private LayerMask ground;

        [SerializeField]
        private PlayerView player;

        private void FixedUpdate()
        {
            if (Physics.Raycast(transform.position + Vector3.up * 50, Vector3.down, out var hit, 200, ground))
            {
                player.LastStandablePosition = hit.point;
            }
        }
    }
}