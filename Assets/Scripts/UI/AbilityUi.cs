using Abilities;
using Abilities.Config;
using Config;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Utils;

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

        [SerializeField]
        private GameObject buttonHelper;

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
                    buttonHelper.SetActive(true);
                }
                else
                {
                    DOTween.Sequence().AppendInterval(GameConfig.Instance.AbilityValues.AbilityUiConfig.WobbleInterval *
                                                      _wobbleTween.ElapsedDirectionalPercentage())
                        .AppendCallback(delegate
                        {
                            abilityParent.transform.localScale = _initialScale;
                            _wobbleTween.Pause();
                            buttonHelper.SetActive(false);
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

            Wobble = false;
        }

        public void SetAbilityIcon(Sprite icon, Color backgroundColor)
        {
            abilityImage.sprite = icon;
            backgroundImage.color = backgroundColor;

            abilityImage.enabled = icon != null;
            backgroundImage.enabled = icon != null;
        }

        private static Sprite[] AllAbilitySprites => new[]
        {
            AbilityConfig.GetConfigSpriteFor(new ExpandAbility()),
            AbilityConfig.GetConfigSpriteFor(new NoGravityAbility()),
            AbilityConfig.GetConfigSpriteFor(new FireDashAbility()),
            AbilityConfig.GetConfigSpriteFor(new IceBlockAbility())
        };

        public void DoSlotMachine(float slotDuration, Sprite spriteToSet, Color backgroundColor)
        {
            var timeScale = Time.timeScale;
            Time.timeScale = 0f;
            SetAbilityIcon(AllAbilitySprites.RandomElement(), backgroundColor);

            var initialPosition = abilityImage.rectTransform.position;
            abilityImage.rectTransform.position = initialPosition + Vector3.up * 200;

            DOTween.Sequence()
                .Append(abilityImage.rectTransform
                    .DOMoveY(abilityImage.rectTransform.position.y - 400, slotDuration / 8f)
                    .SetLoops((int) slotDuration * 2)
                    .OnStepComplete(delegate { SetAbilityIcon(AllAbilitySprites.RandomElement(), backgroundColor); })
                    .OnComplete(delegate
                    {
                        abilityImage.rectTransform.position = initialPosition + Vector3.up * 200;
                        SetAbilityIcon(spriteToSet, backgroundColor);
                    })
                )
                .Append(abilityImage.rectTransform.DOMoveY(initialPosition.y, slotDuration / 8f))
                .OnComplete(delegate { Time.timeScale = timeScale; })
                .SetUpdate(true);
        }
    }
}