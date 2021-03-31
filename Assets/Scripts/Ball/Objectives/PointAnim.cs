using UnityEngine;

namespace Ball.Objectives
{
    public class PointAnim : MonoBehaviour
    {
        private const float HeightIncrease = 2;
        private float _timer;

        private Vector3 _startPos;

        private void Start()
        {
            _startPos = transform.position;
        }

        private void Update()
        {
            if (_timer < 1)
            {
                _timer += Time.deltaTime;
                transform.position =
                    Vector3.Lerp(_startPos, _startPos + new Vector3(0, HeightIncrease, 0), _timer * 3);
            }
        }
    }
}