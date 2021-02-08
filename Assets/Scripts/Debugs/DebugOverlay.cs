using System;
using Config;
using Logic;
using UnityEngine;
using UnityEngine.UI;

namespace Debugs
{
    public class DebugOverlay : MonoBehaviour
    {
        [SerializeField]
        private Button expandAbility;


        [SerializeField]
        private Button noGravityAbility;

        private void Awake()
        {
            expandAbility.onClick.AddListener(delegate
            {
                GameManager.CurrentTurnPlayer.Ability = GameConfig.Instance.AbilityValues.ExpandAbility;
            });
            noGravityAbility.onClick.AddListener(delegate
            {
                GameManager.CurrentTurnPlayer.Ability = GameConfig.Instance.AbilityValues.NoGravityAbility;
            });
        }
    }
}