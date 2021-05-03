using Abilities;
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

            var currentTurnPlayer = _gameManager.CurrentTurnPlayer;
            var abilityController = FindObjectOfType<AbilityController>();
            expandAbility.onClick.AddListener(delegate
            {
                abilityController.SetNewAbilityAndTryNotify(currentTurnPlayer, new ExpandAbility());
            });

            noGravityAbility.onClick.AddListener(delegate
            {
                abilityController.SetNewAbilityAndTryNotify(currentTurnPlayer, new NoGravityAbility());
            });

            iceBlockAbility.onClick.AddListener(delegate
            {
                abilityController.SetNewAbilityAndTryNotify(currentTurnPlayer, new IceBlockAbility());
            });
        }

        private bool _canvasesActive;

        private void Update()
        {
            SetButtonInteractable(_gameManager.CurrentTurnPlayer != null);

            if (Input.GetKeyDown(KeyCode.F3))
            {
                _debugCanvas.enabled = !_debugCanvas.enabled;
            }

            if (Input.GetKeyDown(KeyCode.F4))
            {
                foreach (var canvas in FindObjectsOfType<Canvas>())
                {
                    canvas.targetDisplay = _canvasesActive ? 0 : 1;
                }

                _canvasesActive = !_canvasesActive;
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