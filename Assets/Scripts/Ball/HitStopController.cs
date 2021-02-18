using Config;
using DG.Tweening;
using Logic;
using UnityEngine;

namespace Ball
{
    public class HitStopController : MonoBehaviour
    {
        private void OnCollisionEnter(Collision collision)
        {
            if (collision.transform.CompareTag("Lemming"))
            {
                if (collision.impulse.sqrMagnitude > GameConfig.Instance.HitStop.HitStopTriggerImpulse)
                {
                    var hitStopValues = GameConfig.Instance.HitStop;
                    FindObjectOfType<VirtualCamerasController>()
                        .ZoomBoomActiveCamera(20, hitStopValues.ZoomInTime,
                            hitStopValues.ZoomOutTime, Ease.Flash);

                    DOTween.Sequence()
                        .Append(DOTween.To(() => Time.timeScale, t => Time.timeScale = t, hitStopValues.TimeScale,
                            hitStopValues.ZoomInTime))
                        .AppendInterval(hitStopValues.StayTime)
                        .Append(DOTween.To(() => Time.timeScale, t => Time.timeScale = t, GameConfig.Instance.TimeScale,
                            hitStopValues.ZoomOutTime))
                        .SetUpdate(true);
                }
            }
        }
    }
}