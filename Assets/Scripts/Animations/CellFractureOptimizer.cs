using DG.Tweening;
using UnityEngine;

namespace Animations
{
    public class CellFractureOptimizer : MonoBehaviour
    {
        [SerializeField]
        private float duration = 5f;

        private void Start()
        {
            DOTween.Sequence().Append(transform.DOScale(0f, duration)).AppendCallback(delegate { Destroy(gameObject); });
        }
    }
}