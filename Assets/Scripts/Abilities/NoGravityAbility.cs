using System;
using Audio;
using Config;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Abilities
{
    [Serializable]
    public class NoGravityAbility : Ability
    {
        public float DurationTime => GameConfig.Instance.AbilityValues.NoGravityAbility.DurationTime;

        public override async void InvokeAbility(PlayerView player)
        {
            var gravity = Physics.gravity;
            Physics.gravity = Vector3.zero;
            AudioManager.Instance.DoLowPass(.5f);
            await UniTask.Delay(TimeSpan.FromSeconds(DurationTime), DelayType.UnscaledDeltaTime);
            AudioManager.Instance.UndoLowPass(.5f);
            Physics.gravity = gravity;
        }
    }
}