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
            if (Physics.Raycast(transform.position, Vector3.down, out var hit, 50f, ground))
            {
                player.LastStandablePosition = hit.point;
            }
        }
    }
}