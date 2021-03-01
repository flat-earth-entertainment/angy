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
                    DOTween.Sequence().AppendInterval(0.5f * _wobbleTween.ElapsedDirectionalPercentage())
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
            _wobbleTween = abilityParent.transform.DOScale(1.5f, .5f).SetLoops(-1, LoopType.Yoyo).Pause();
            _initialScale = abilityParent.transform.localScale;
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