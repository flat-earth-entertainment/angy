using System;
using Config;
using UnityEngine;

namespace Ball
{
    public class HitOtherPlayerTrigger : MonoBehaviour
    {
        public static event Action<PlayerView, PlayerView> PlayerHit;

        private void OnCollisionEnter(Collision other)
        {
            if (other.transform.CompareTag("Lemming"))
            {
                if (other.transform.GetChild(0).TryGetComponent(out Shooter otherShooter))
                {
                    if (GameConfig.Instance.ExplosionForceOnPlayerHit != 0)
                    {
                        GetComponent<Rigidbody>()
                            .AddForce((other.transform.position - transform.position) *
                                      otherShooter.PlayerView.Knockback);
                    }

                    otherShooter.SetBallFormActive(true);
                    PlayerHit?.Invoke(otherShooter.PlayerView,
                        transform.GetChild(0).GetComponent<Shooter>().PlayerView);
                }
            }
        }
    }
}