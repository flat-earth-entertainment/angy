using System;
using Audio;
using Ball;
using GameSession;
using Logic;
using Player;
using UnityEngine;
using Utils;

namespace Environment
{
    public class KillingTrigger : MonoBehaviour
    {
        public static event Action<PlayerView> HitKillTrigger;

        [SerializeField]
        private SfxType soundToPlayOnHit;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Lemming"))
            {
                if (other.transform.GetChild(0).TryGetComponent(out Shooter otherShooter))
                {
                    if (!CurrentGameSession.IsNowPassive)
                    {
                        HitKillTrigger?.Invoke(otherShooter.PlayerView);
                        PhotonShortcuts.ReliableRaiseEventToOthers(GameEvent.PlayerHitKillTrigger,
                            CurrentGameSession.PlayerFromPlayerView(otherShooter.PlayerView).Id);
                    }

                    AudioManager.PlaySfx(soundToPlayOnHit);
                }
            }
        }
    }
}