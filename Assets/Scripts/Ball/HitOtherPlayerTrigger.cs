using System;
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
                    PlayerHit?.Invoke(shooter.PlayerView, transform.GetChild(0).GetComponent<Shooter>().PlayerView);
                }
            }
        }
    }
}