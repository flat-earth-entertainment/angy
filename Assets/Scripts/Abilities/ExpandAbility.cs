using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Abilities
{
    [Serializable]
    public class ExpandAbility : Ability
    {
        [field: SerializeField]
        public float TimeToInflate { get; private set; }

        [field: SerializeField]
        public float TimeToDeflate { get; private set; }

        [field: SerializeField]
        public float Duration { get; private set; }

        [field: SerializeField]
        public float Scale { get; private set; }


        public override async void InvokeAbility(PlayerView player)
        {
            var initialScale = player.Ball.transform.localScale;
            await player.Ball.transform.DOScale(Scale, TimeToInflate).SetEase(Ease.OutElastic).SetUpdate(true);
            await UniTask.Delay(TimeSpan.FromSeconds(Duration), DelayType.UnscaledDeltaTime);
            await player.Ball.transform.DOScale(initialScale, TimeToDeflate).SetUpdate(true);
        }
    }
}