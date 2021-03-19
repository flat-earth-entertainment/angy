using System;
using Config;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Abilities
{
    public class FireDashAbility : Ability
    {
        protected override async void InvokeAbility(PlayerView player)
        {
            Active = true;

            var initialTimeScale = Time.timeScale;
            Time.timeScale = 0;

            await DOTween.To(() => Time.timeScale, t => Time.timeScale = t, 0,
                GameConfig.Instance.AbilityValues.FireDashAbilityConfig.EnterTime).SetUpdate(true);

            var fireDash = Object.Instantiate(
                    GameConfig.Instance.AbilityValues.FireDashAbilityConfig.FireDashControlsPrefab,
                    player.Ball.transform.position,
                    Quaternion.identity)
                .GetComponent<FireDashMonoBehaviour>();

            UnityEditor.Selection.activeObject = fireDash.gameObject;

            fireDash.Initialize(player.PlayerInputs);

            await UniTask.Delay(
                TimeSpan.FromSeconds(GameConfig.Instance.AbilityValues.FireDashAbilityConfig.InputWaitTime), true);

            player.BallRigidbody.velocity = fireDash.transform.forward *
                                            GameConfig.Instance.AbilityValues.FireDashAbilityConfig.PushForce;

            Object.Destroy(fireDash.gameObject);
            Time.timeScale = initialTimeScale;

            Active = false;
            Finished = true;
        }
    }
}