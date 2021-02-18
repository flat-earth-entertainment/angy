using Logic;
using UnityEngine;

namespace Environment
{
    public class TurnBasedBlock : MonoBehaviour
    {
        [SerializeField]
        private int turnsToDisable;

        private int _timer;

        private void Awake()
        {
            _timer = turnsToDisable;
        }

        private void OnEnable()
        {
            GameManager.RoundPassed += OnRoundPassed;
        }

        private void OnDisable()
        {
            GameManager.RoundPassed -= OnRoundPassed;
        }

        private void OnRoundPassed()
        {
            _timer--;

            if (_timer <= 0)
            {
                gameObject.SetActive(false);
            }
        }
    }
}