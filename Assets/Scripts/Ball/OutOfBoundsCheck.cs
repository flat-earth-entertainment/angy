using System;
using Config;
using UnityEngine;
using Utils;

namespace Ball
{
    public class OutOfBoundsCheck : MonoBehaviour
    {
        private static float _heightThreshold;
        public event Action WentOutOfBounds;


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

        // private float _timer;
        // private void OnEnable()
        // {
        //     _timer = 0f;
        // }
        //
        // private void OnDisable()
        // {
        //     _timer = 0f;
        // }
        //
        // private void FixedUpdate()
        // {
        //     if (!Physics.Raycast(transform.position, Vector3.down, GameConfig.Instance.GroundMask))
        //     {
        //         _timer += Time.fixedDeltaTime;
        //
        //         if (_timer >= GameConfig.Instance.OutOfBoundsReactionTime)
        //         {
        //             WentOutOfBounds?.Invoke();
        //         }
        //     }
        //     else
        //     {
        //         _timer = 0f;
        //     }
        // }
    }
}