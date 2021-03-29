using System;
using UnityEngine;
using Utils;

namespace Ball
{
    public class OutOfBoundsCheck : MonoBehaviour
    {
        private static float _heightThreshold;


        private void Awake()
        {
            _heightThreshold = "Height OB Threshold".SafeFindWithThisTag().transform.position.y;
        }

        private void Update()
        {
            if (transform.position.y < _heightThreshold)
            {
                WentOutOfBounds?.Invoke();
            }
        }

        public event Action WentOutOfBounds;
    }
}