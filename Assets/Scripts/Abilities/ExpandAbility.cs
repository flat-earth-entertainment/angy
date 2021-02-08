using System;
using Config;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Abilities
{
    [Serializable]
    public class ExpandAbility : Ability
    {
        public float TimeToInflate => GameConfig.Instance.AbilityValues.ExpandAbility.TimeToInflate;

        public float TimeToDeflate => GameConfig.Instance.AbilityValues.ExpandAbility.TimeToDeflate;

        public float Duration => GameConfig.Instance.AbilityValues.ExpandAbility.Duration;

        public float Scale => GameConfig.Instance.AbilityValues.ExpandAbility.Scale;


        public override async void InvokeAbility(PlayerView player)
        {
            var initialScale = player.Ball.transform.localScale;
            await player.Ball.transform.DOScale(Scale, TimeToInflate).SetEase(Ease.OutElastic).SetUpdate(true);
            await UniTask.Delay(TimeSpan.FromSeconds(Duration), DelayType.UnscaledDeltaTime);
            await player.Ball.transform.DOScale(initialScale, TimeToDeflate).SetUpdate(true);
        }
    }
}