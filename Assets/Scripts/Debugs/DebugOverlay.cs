using Abilities;
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

        private Canvas _debugCanvas;

        private void Awake()
        {
            _debugCanvas = GetComponent<Canvas>();
            _debugCanvas.enabled = false;

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

            if (Input.GetKeyDown(KeyCode.F3))
            {
                _debugCanvas.enabled = !_debugCanvas.enabled;
            }
        }
    }
}