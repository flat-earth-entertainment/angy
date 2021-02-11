using System;
using Abilities;
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

        [SerializeField]
        private Button iceBlockAbility;

        private void Awake()
        {
            expandAbility.onClick.AddListener(delegate
            {
                GameManager.CurrentTurnPlayer.Ability = new ExpandAbility();
            });

            noGravityAbility.onClick.AddListener(delegate
            {
                GameManager.CurrentTurnPlayer.Ability = new NoGravityAbility();
            });

            iceBlockAbility.onClick.AddListener(delegate
            {
                GameManager.CurrentTurnPlayer.Ability = new IceBlockAbility();
            });
        }

        private void SetButtonInteractable(bool state)
        {
            expandAbility.interactable = state;
            noGravityAbility.interactable = state;
            iceBlockAbility.interactable = state;
        }

        private void Update()
        {
            SetButtonInteractable(GameManager.CurrentTurnPlayer != null);
        }
    }
}