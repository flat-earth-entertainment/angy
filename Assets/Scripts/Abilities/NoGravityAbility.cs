using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Abilities
{
    [Serializable]
    public class NoGravityAbility : Ability
    {
        [field: SerializeField]
        public float DurationTime { get; private set; }

        public override async void InvokeAbility(PlayerView player)
        {
            Debug.Log(GetType().Name.Replace("Ability","") + " ability invoked!");

            var gravity = Physics.gravity;
            Physics.gravity = Vector3.zero;
            await UniTask.Delay(TimeSpan.FromSeconds(DurationTime));
            Physics.gravity = gravity;
        }
    }
}