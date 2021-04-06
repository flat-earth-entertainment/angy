using DG.Tweening;
using UnityEngine;

namespace Animations
{
    public class CellFractureOptimizer : MonoBehaviour
    {
        [SerializeField]
        private float duration = 5f;

        private Sequence _sequence;

        private void Start()
        {
            _sequence = DOTween.Sequence().Append(transform.DOScale(0f, duration))
                .AppendCallback(delegate { Destroy(gameObject); });
        }

        private void OnDisable()
        {
            _sequence.Kill();
        }
    }
}