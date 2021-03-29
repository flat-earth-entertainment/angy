#if UNITY_EDITOR
using UnityEngine;

namespace ComponentGizmos
{
    public class PlaneGizmo : MonoBehaviour
    {
        private static readonly Vector3 BoxSize = new Vector3(1000, .1f, 1000);

        [SerializeField]
        private Color planeColor = Color.green;

        private void OnDrawGizmos()
        {
            Gizmos.color = planeColor;
            Gizmos.DrawCube(transform.position, BoxSize);
        }
    }
}
#endif