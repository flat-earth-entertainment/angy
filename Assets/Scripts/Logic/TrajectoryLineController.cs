using UnityEngine;

namespace Logic
{
    public class TrajectoryLineController : MonoBehaviour
    {
        [SerializeField]
        private LineRenderer lineRenderer;

        public void SetTrajectoryActive(bool state)
        {
            lineRenderer.enabled = state;
        }

        public void SetGradientColor(Gradient gradient)
        {
            lineRenderer.colorGradient = gradient;
        }
    }
}