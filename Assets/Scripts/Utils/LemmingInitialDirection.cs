using Ball;
using UnityEngine;

namespace Utils
{
    public class LemmingInitialDirection : MonoBehaviour
    {
        public void RotateLemming(Shooter lemming)
        {
            lemming.horSnap = transform.localEulerAngles.y + 180;
            lemming.Predict();
        }
    }
}