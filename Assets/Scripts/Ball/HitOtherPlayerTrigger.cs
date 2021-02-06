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
            if (other.transform.childCount >= 1)
            {
                if (other.transform.GetChild(0).TryGetComponent(out Shooter shooter))
                {
                    if (GameConfig.Instance.ExplosionForceOnPlayerHit != 0)
                    {
                        GetComponent<Rigidbody>().AddExplosionForce(GameConfig.Instance.ExplosionForceOnPlayerHit,
                            other.contacts[0].point, 1f);
                        other.rigidbody.AddExplosionForce(GameConfig.Instance.ExplosionForceOnPlayerHit,
                            other.contacts[0].point, 1f);
                    }

                    shooter.SetBallFormActive(true);
                    PlayerHit?.Invoke(shooter.PlayerView, transform.GetChild(0).GetComponent<Shooter>().PlayerView);
                }
            }
        }
    }
}