using System;
using Audio;
using Ball;
using Player;
using UnityEngine;

namespace Environment
{
    public class KillingTrigger : MonoBehaviour
    {
        public static Action<PlayerView> HitKillTrigger;

        [SerializeField]
        private SfxType soundToPlayOnHit;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Lemming"))
            {
                if (other.transform.GetChild(0).TryGetComponent(out Shooter otherShooter))
                {
                    HitKillTrigger?.Invoke(otherShooter.PlayerView);
                    AudioManager.PlaySfx(soundToPlayOnHit);
                }
            }
        }
    }
}