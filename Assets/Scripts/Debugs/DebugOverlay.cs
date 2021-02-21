using System;
using Abilities;
using Config;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Logic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

namespace Debugs
{
    public class DebugOverlay : MonoBehaviour
    {
        [SerializeField]
        private Button expandAbility;

        [SerializeField]
        private Button noGravityAbility;

        [SerializeField]
        private Button iceBlockAbility;

        [SerializeField]
        private AudioSource widePSource;

        private void Awake()
        {
            expandAbility.onClick.AddListener(delegate
            {
                GameManager.CurrentTurnPlayer.Ability = new ExpandAbility();
            });

            noGravityAbility.onClick.AddListener(delegate
            {
                GameManager.CurrentTurnPlayer.Ability = new NoGravityAbility();
            });

            iceBlockAbility.onClick.AddListener(delegate
            {
                GameManager.CurrentTurnPlayer.Ability = new IceBlockAbility();
            });
        }

        private void SetButtonInteractable(bool state)
        {
            expandAbility.interactable = state;
            noGravityAbility.interactable = state;
            iceBlockAbility.interactable = state;
        }

        private bool _isWide;

        private void Update()
        {
            SetButtonInteractable(GameManager.CurrentTurnPlayer != null);

            if (Input.GetKeyDown(KeyCode.P) && !_isWide)
            {
                EngageWidePMode();
            }
        }

        private async void EngageWidePMode()
        {
            if (FindObjectOfType<Volume>().sharedProfile.TryGet<LensDistortion>(out var lensDistortion))
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
                await UniTask.Delay(TimeSpan.FromSeconds(GameConfig.Instance.WidePMode.Duration), DelayType.Realtime);

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