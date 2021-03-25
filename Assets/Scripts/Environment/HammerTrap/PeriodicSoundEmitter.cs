using System;
using Audio;
using DG.Tweening;
using UnityEngine;

namespace Environment.HammerTrap
{
    public class PeriodicSoundEmitter : MonoBehaviour
    {
        [SerializeField]
        private SfxType sfxType;

        [SerializeField]
        private float interval;

        private Sequence _playSoundSequence;

        private void Start()
        {
            _playSoundSequence = DOTween.Sequence().AppendCallback(PlaySfx).AppendInterval(interval)
                .SetUpdate(UpdateType.Normal).SetLoops(-1);
        }

        private void OnDestroy()
        {
            _playSoundSequence.Kill();
            _playSoundSequence = null;
        }

        private void PlaySfx()
        {
            AudioManager.PlaySfx(sfxType);
        }
    }
}