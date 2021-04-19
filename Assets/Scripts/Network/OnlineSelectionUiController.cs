using NaughtyAttributes;
using Photon.Pun;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Network
{
    public class OnlineSelectionUiController : MonoBehaviour
    {
        [SerializeField]
        private Button rankingsButton;

        [SerializeField]
        [Scene]
        private string rankingsScene;

        [SerializeField]
        private Button profileButton;

        [SerializeField]
        [Scene]
        private string profileScene;

        [SerializeField]
        private Button roomsButton;

        [SerializeField]
        [Scene]
        private string roomSelectionScene;

        [SerializeField]
        private Button backButton;

        [SerializeField]
        [Scene]
        private string backScene;

        private void Awake()
        {
            rankingsButton.onClick.AddListener(delegate { SceneChanger.ChangeScene(rankingsScene); });

            roomsButton.onClick.AddListener(delegate { SceneManager.LoadScene(roomSelectionScene); });

            profileButton.onClick.AddListener(delegate { SceneManager.LoadScene(profileScene); });

            backButton.onClick.AddListener(delegate
            {
                SessionHandler.CancelSession();
                PhotonNetwork.Disconnect();
                SceneManager.LoadScene(backScene);
            });

            PhotonNetwork.LocalPlayer.NickName = CurrentPlayer.LeaderboardPlayer.Nickname;
        }
    }
}