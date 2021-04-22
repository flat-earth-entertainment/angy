using DG.Tweening;
using NaughtyAttributes;
using Photon.Pun;
using TMPro;
using UI;
using UnityEngine;

namespace Network
{
    [RequireComponent(typeof(Canvas))]
    public class DisconnectController : MonoBehaviourPunCallbacks
    {
        [SerializeField]
        private int secondsToDisconnect;

        [Scene]
        [SerializeField]
        private string sceneToLoad;

        [SerializeField]
        private TextMeshProUGUI disconnectTimerText;

        private Canvas _canvas;
        private Sequence _disconnectSequence;

        private void Awake()
        {
            _canvas = GetComponent<Canvas>();
            _canvas.enabled = false;
        }

        public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
        {
            _canvas.enabled = true;
            _disconnectSequence = DOTween.Sequence()
                .AppendInterval(secondsToDisconnect)
                .OnKill(LeaveSession)
                .OnUpdate(() =>
                {
                    disconnectTimerText.text =
                        ((int) (secondsToDisconnect - _disconnectSequence.Elapsed())).ToString();

                    if (_disconnectSequence.Elapsed() > 1 && Input.anyKeyDown)
                    {
                        _disconnectSequence.Kill();
                    }
                })
                .SetUpdate(true);
        }

        private void LeaveSession()
        {
            SceneChanger.ChangeScene(sceneToLoad);
            PhotonNetwork.LeaveRoom();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                OnPlayerLeftRoom(null);
            }
        }
    }
}