using Audio;
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
            if (collision.transform.CompareTag("Lemming"))
            {
                if (collision.impulse.sqrMagnitude > GameConfig.Instance.HitStop.HitStopTriggerImpulse)
                {
                    HitStop(collision.contacts[0].point);
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            GoodNeutralMushroom mush = other.GetComponent<GoodNeutralMushroom>();
            if (mush && !mush.mushroomDisabled)
            {
                HitStop(Vector3.Lerp(other.transform.position, transform.position, 0.5f));
            }
        }

        private static async void HitStop(Vector3 hitPoint)
        {
            if (_isInAction)
                return;

            Destroy(Instantiate(GameConfig.Instance.HitStop.ImpactParticle, hitPoint, Quaternion.identity), 1f);
            AudioManager.PlaySfx(SfxType.HitStopEngaged);

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
                .AppendCallback(delegate { AudioManager.Instance.DoLowPass(hitStopValues.ZoomInTime); })
                .Append(DOTween.To(() => Time.timeScale, t => Time.timeScale = t, hitStopValues.TimeScale,
                    hitStopValues.ZoomInTime))
                .AppendInterval(hitStopValues.StayTime)
                .AppendCallback(delegate { AudioManager.Instance.UndoLowPass(hitStopValues.ZoomOutTime); })
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