#if UNITY_EDITOR
using UnityEngine;

namespace ComponentGizmos
{
    public class PlaneGizmo : MonoBehaviour
    {
        [SerializeField]
        private Color planeColor = Color.green;

        private static readonly Vector3 BoxSize = new Vector3(1000, .1f, 1000);

        private void OnDrawGizmos()
        {
            Gizmos.color = planeColor;
            Gizmos.DrawCube(transform.position, BoxSize);
        }
    }
}
#endif