using UnityEngine;

namespace Player
{
    public class ShadowAligner : MonoBehaviour
    {
        [SerializeField]
        private Transform followPositionTarget;

        private void Update()
        {
            transform.position = followPositionTarget.position;
        }
    }
}