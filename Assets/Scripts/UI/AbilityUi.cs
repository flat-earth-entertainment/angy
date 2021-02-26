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
                            abilityParent.transform.localScale = Vector3.one;
                            _wobbleTween.Pause();
                        });
                }
            }
        }

        private Tween _wobbleTween;

        private void Awake()
        {
            _wobbleTween = abilityParent.transform.DOScale(1.5f, .5f).SetLoops(-1, LoopType.Yoyo).Pause();
        }

        public void SetAbilityIcon(Sprite icon)
        {
            abilityImage.sprite = icon;
        }
    }
}