using GameSession;
using NaughtyAttributes;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI
{
    public class PlaymodeSelectionUiController : MonoBehaviourPunCallbacks
    {
        [SerializeField]
        private Button localButton;

        [SerializeField]
        [Scene]
        private string localModeScene;

        [SerializeField]
        private Button onlineButton;

        [SerializeField]
        [Scene]
        private string roomSelectionScene;

        [SerializeField]
        private Button backButton;

        [SerializeField]
        [Scene]
        private string backScene;

        [SerializeField]
        private GameObject connectionErrorParent;

        [SerializeField]
        private TextMeshProUGUI connectionErrorText;

        [SerializeField]
        private Button connectionErrorCloseButton;

        private void Awake()
        {
            localButton.onClick.AddListener(delegate
            {
                PhotonNetwork.OfflineMode = true;
                PhotonNetwork.JoinRandomRoom();

                CurrentGameSession.Players = new GameSession.Player[]
                {
                    new LocalPlayer(0, 0, 0),
                    new LocalPlayer(1, 1, 1)
                };

                SceneChanger.ChangeScene(localModeScene);
            });

            onlineButton.onClick.AddListener(delegate
            {
                onlineButton.interactable = false;
                PhotonNetwork.OfflineMode = false;
                if (PhotonNetwork.ConnectUsingSettings())
                {
                    SceneManager.LoadScene(roomSelectionScene);
                }
            });

            backButton.onClick.AddListener(delegate { SceneManager.LoadScene(backScene); });

            connectionErrorCloseButton.onClick.AddListener(delegate { connectionErrorParent.SetActive(false); });
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            if (cause == DisconnectCause.DisconnectByClientLogic)
                return;

            onlineButton.interactable = true;
            connectionErrorParent.SetActive(true);
            connectionErrorText.text = "Connection error occured: " + cause;
        }
    }
}