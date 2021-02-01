using System;
using Config;
using UnityEngine;

namespace Ball
{
    public class OutOfBoundsCheck : MonoBehaviour
    {
        public event Action WentOutOfBounds;

        private float _timer;

        private void OnEnable()
        {
            _timer = 0f;
        }

        private void OnDisable()
        {
            _timer = 0f;
        }

        private void FixedUpdate()
        {
            if (!Physics.Raycast(transform.position, Vector3.down, GameConfig.Instance.GroundMask))
            {
                _timer += Time.fixedDeltaTime;

                if (_timer >= GameConfig.Instance.OutOfBoundsReactionTime)
                {
                    WentOutOfBounds?.Invoke();
                }
            }
            else
            {
                _timer = 0f;
            }
        }
    }
}