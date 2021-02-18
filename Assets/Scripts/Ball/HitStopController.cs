using Config;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Logic;
using UnityEngine;

namespace Ball
{
    public class HitStopController : MonoBehaviour
    {
        private static bool _isInAction;

        private void OnCollisionEnter(Collision collision)
        {
            if (_isInAction)
                return;

            if (collision.transform.CompareTag("Lemming"))
            {
                if (collision.impulse.sqrMagnitude > GameConfig.Instance.HitStop.HitStopTriggerImpulse)
                {
                    HitStop();
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            GoodNeutralMushroom mush = other.GetComponent<GoodNeutralMushroom>();
            if (mush && !mush.mushroomDisabled)
            {
                HitStop();
            }
        }

        private static async void HitStop()
        {
            var hitStopValues = GameConfig.Instance.HitStop;
            FindObjectOfType<VirtualCamerasController>().ShakeFor(hitStopValues.ZoomOutTime);

            var initialFixedDeltaTime = Time.fixedDeltaTime;
            Time.fixedDeltaTime = initialFixedDeltaTime / 10f;
            _isInAction = true;

            if (Time.timeScale == 0)
            {
                await UniTask.WaitUntil(() => Time.timeScale != 0);
            }

            DOTween.Sequence()
                .Append(DOTween.To(() => Time.timeScale, t => Time.timeScale = t, hitStopValues.TimeScale,
                    hitStopValues.ZoomInTime))
                .AppendInterval(hitStopValues.StayTime)
                .Append(DOTween.To(() => Time.timeScale, t => Time.timeScale = t, GameConfig.Instance.TimeScale,
                    hitStopValues.ZoomOutTime))
                .SetUpdate(true)
                .OnComplete(delegate
                {
                    _isInAction = false;
                    Time.fixedDeltaTime = initialFixedDeltaTime;
                });
        }
    }
}