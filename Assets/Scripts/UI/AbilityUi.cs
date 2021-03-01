using Config;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class AbilityUi : MonoBehaviour
    {
        [SerializeField]
        private GameObject abilityParent;

        [SerializeField]
        private Image backgroundImage;

        [SerializeField]
        private Image abilityImage;

        public bool Visible
        {
            set => abilityParent.SetActive(value);
        }

        public bool Wobble
        {
            set
            {
                if (value)
                {
                    _wobbleTween.Play();
                }
                else
                {
                    DOTween.Sequence().AppendInterval(GameConfig.Instance.AbilityValues.AbilityUiConfig.WobbleInterval *
                                                      _wobbleTween.ElapsedDirectionalPercentage())
                        .AppendCallback(delegate
                        {
                            abilityParent.transform.localScale = _initialScale;
                            _wobbleTween.Pause();
                        });
                }
            }
        }

        private Tween _wobbleTween;
        private Vector3 _initialScale;

        private void Awake()
        {
            _initialScale = abilityParent.transform.localScale;

            _wobbleTween = abilityParent.transform
                .DOScale(_initialScale * GameConfig.Instance.AbilityValues.AbilityUiConfig.WobbleScale,
                    GameConfig.Instance.AbilityValues.AbilityUiConfig.WobbleInterval)
                .SetLoops(-1, LoopType.Yoyo).Pause();
        }

        public void SetAbilityIcon(Sprite icon, Color backgroundColor)
        {
            abilityImage.sprite = icon;
            backgroundImage.color = backgroundColor;

            abilityImage.enabled = icon != null;
            backgroundImage.enabled = icon != null;
        }
    }
}