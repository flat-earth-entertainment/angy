using Config;
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
                    var hitStopValues = GameConfig.Instance.HitStop;
                    FindObjectOfType<VirtualCamerasController>()
                        .ZoomBoomActiveCamera(20, hitStopValues.ZoomInTime,
                            hitStopValues.ZoomOutTime, Ease.Flash);

                    var initialFixedDeltaTime = Time.fixedDeltaTime;
                    Time.fixedDeltaTime = initialFixedDeltaTime / 10f;
                    _isInAction = true;
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
    }
}