using DG.Tweening;
using UnityEngine;

namespace Models.Berries
{
    public class BerryAnim : MonoBehaviour
    {
        public float rotSpeed = 50;
        public float duration = 3f;
        public int vibrato = 10;
        public float elasticity = 0.5f;
        public Vector3 punchScale = new Vector3(1, 1, 1);

        private void Update()
        {
            transform.Rotate(new Vector3(0, rotSpeed * Time.deltaTime, 0), Space.World);
        }

        public void BerryHit()
        {
            if (!DOTween.IsTweening(transform))
            {
                transform.DOPunchScale(punchScale, duration, vibrato, elasticity);
            }
        }
    }
}