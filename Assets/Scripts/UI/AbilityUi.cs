using Abilities;
using Abilities.Config;
using Config;
using DG.Tweening;
using Player;
using TMPro;
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

        [SerializeField]
        private GameObject buttonHelper;

        [SerializeField]
        private TextMeshProUGUI tooltipText;

        private Vector3 _initialScale;
        private Tween _wobbleTween;

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
                    DOTween.Sequence()
                        .AppendInterval(GameConfig.Instance.AbilityValues.AbilityUiConfig.WobbleInterval *
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

        private void Awake()
        {
            _initialScale = abilityParent.transform.localScale;

            _wobbleTween = abilityParent.transform
                .DOScale(_initialScale * GameConfig.Instance.AbilityValues.AbilityUiConfig.WobbleScale,
                    GameConfig.Instance.AbilityValues.AbilityUiConfig.WobbleInterval)
                .SetLoops(-1, LoopType.Yoyo).Pause();

            Wobble = false;
            tooltipText.transform.parent.gameObject.SetActive(false);
        }

        public void SetAbilityUi(PlayerView playerView, Ability ability)
        {
            var abilityImageSprite = AbilityConfig.GetConfigSpriteFor(ability);

            var abilityNotNull = abilityImageSprite != null;

            abilityImage.enabled = abilityNotNull;
            backgroundImage.enabled = abilityNotNull;
            tooltipText.transform.parent.gameObject.SetActive(abilityNotNull);

            if (!abilityNotNull)
                return;

            abilityImage.sprite = abilityImageSprite;
            backgroundImage.color = playerView.PlayerPreset.PlayerColor;
            tooltipText.text = ability.GetType().Name.Replace("Ability", "");
        }

        public void DoSlotMachine(float slotDuration, PlayerView playerView, Ability ability)
        {
            var timeScale = Time.timeScale;
            Time.timeScale = 0f;
            SetAbilityUi(playerView, Ability.RandomAbility());

            var initialPosition = abilityImage.rectTransform.position;
            abilityImage.rectTransform.position = initialPosition + Vector3.up * 200;

            DOTween.Sequence()
                .Append(abilityImage.rectTransform
                    .DOMoveY(abilityImage.rectTransform.position.y - 400, slotDuration / 8f)
                    .SetLoops((int) slotDuration * 2)
                    .OnStepComplete(delegate { SetAbilityUi(playerView, Ability.RandomAbility()); })
                    .OnComplete(delegate
                    {
                        abilityImage.rectTransform.position = initialPosition + Vector3.up * 200;
                        SetAbilityUi(playerView, ability);
                    })
                )
                .Append(abilityImage.rectTransform.DOMoveY(initialPosition.y, slotDuration / 8f))
                .OnComplete(delegate { Time.timeScale = timeScale; })
                .SetUpdate(true);
        }
    }
}