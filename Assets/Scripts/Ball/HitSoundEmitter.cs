using Audio;
using Player;
using UnityEngine;

namespace Ball
{
    public class HitSoundEmitter : MonoBehaviour
    {
        [SerializeField]
        private PlayerView playerView;

        private void OnCollisionEnter()
        {
            if (playerView.PlayerState != PlayerState.ActiveAiming)
                AudioManager.PlaySfx(SfxType.LemmingHitGround);
        }
    }
}