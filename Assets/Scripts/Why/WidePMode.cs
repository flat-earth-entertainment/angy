using System;
using Config;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Why
{
    public class WidePMode : MonoBehaviour
    {
        [SerializeField]
        private AudioSource widePSource;

        [SerializeField]
        private VolumeProfile widePProfile;


        private bool _isWide;

        private void Update()
        {
            if (Input.GetKeyDown(GameConfig.Instance.WidePMode.Key) && !_isWide)
            {
                EngageWidePMode();
            }
        }

        private async void EngageWidePMode()
        {
            if (widePProfile.TryGet<LensDistortion>(out var lensDistortion))
            {
                _isWide = true;
                lensDistortion.active = true;

                DOTween.To(f => lensDistortion.intensity.value = f, 0f, 1f, GameConfig.Instance.WidePMode.InTime)
                    .SetUpdate(true);
                DOTween.To(f => lensDistortion.scale.value = f, 1f, 5f, GameConfig.Instance.WidePMode.InTime)
                    .SetUpdate(true);

                widePSource.volume = 0;
                widePSource.Play();
                widePSource.DOFade(1, GameConfig.Instance.WidePMode.InTime).SetUpdate(true);
                await UniTask.Delay(TimeSpan.FromSeconds(GameConfig.Instance.WidePMode.Duration), DelayType.UnscaledDeltaTime);

                widePSource.DOFade(0, GameConfig.Instance.WidePMode.OutTime).SetUpdate(true);
                DOTween.To(f => lensDistortion.intensity.value = f, 1f, 0f, GameConfig.Instance.WidePMode.OutTime)
                    .SetUpdate(true);
                await DOTween.To(f => lensDistortion.scale.value = f, 5f, 1f, GameConfig.Instance.WidePMode.OutTime)
                    .SetUpdate(true);

                lensDistortion.active = false;
                _isWide = false;
            }
            else
            {
                Debug.LogError("Current Post-Processing profile does not have lens distortion added!");
            }
        }
    }
}