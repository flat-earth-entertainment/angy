﻿using Abilities;
using Logic;
using UnityEngine;
using UnityEngine.UI;

namespace GameDebug
{
    public class DebugOverlay : MonoBehaviour
    {
        [SerializeField]
        private Button expandAbility;

        [SerializeField]
        private Button noGravityAbility;

        [SerializeField]
        private Button iceBlockAbility;

        private Canvas _debugCanvas;
        private GameManager _gameManager;

        private void Awake()
        {
            _gameManager = FindObjectOfType<GameManager>();

            _debugCanvas = GetComponent<Canvas>();
            _debugCanvas.enabled = false;

            expandAbility.onClick.AddListener(delegate
            {
                _gameManager.CurrentTurnPlayer.Ability = new ExpandAbility();
            });

            noGravityAbility.onClick.AddListener(delegate
            {
                _gameManager.CurrentTurnPlayer.Ability = new NoGravityAbility();
            });

            iceBlockAbility.onClick.AddListener(delegate
            {
                _gameManager.CurrentTurnPlayer.Ability = new IceBlockAbility();
            });
        }

        private void Update()
        {
            SetButtonInteractable(_gameManager.CurrentTurnPlayer != null);

            if (Input.GetKeyDown(KeyCode.F3))
            {
                _debugCanvas.enabled = !_debugCanvas.enabled;
            }
        }

        private void SetButtonInteractable(bool state)
        {
            expandAbility.interactable = state;
            noGravityAbility.interactable = state;
            iceBlockAbility.interactable = state;
        }
    }
}