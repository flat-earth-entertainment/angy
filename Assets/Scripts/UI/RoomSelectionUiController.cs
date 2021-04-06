using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using Photon.Pun;
using Photon.Realtime;
using Scenes.RoomSelection;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace UI
{
    public class RoomSelectionUiController : MonoBehaviourPunCallbacks
    {
        [SerializeField]
        private Button backButton;

        [SerializeField]
        [Scene]
        private string backScene;

        [SerializeField]
        private Button createRoomButton;

        [SerializeField]
        [Scene]
        private string roomLobbyScene;

        [SerializeField]
        private GameObject roomListRowPrefab;

        [SerializeField]
        private ScrollRect roomScrollRect;

        private readonly Dictionary<string, RoomListRow> _roomViewDictionary = new Dictionary<string, RoomListRow>();

        private async void Awake()
        {
            backButton.onClick.AddListener(delegate
            {
                PhotonNetwork.Disconnect();

                SceneManager.LoadScene(backScene);
            });

            createRoomButton.interactable = false;

            createRoomButton.onClick.AddListener(CreateRoom);

            await UniTask.WaitUntil(() => PhotonNetwork.NetworkClientState == ClientState.ConnectedToMasterServer);
            PhotonNetwork.JoinLobby();
        }

        public override void OnJoinedLobby()
        {
            Debug.Log("joined lobby");
            createRoomButton.interactable = true;
        }

        public override void OnJoinedRoom()
        {
            SceneManager.LoadScene(roomLobbyScene);
        }

        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            Debug.LogError(message);
        }

        public override void OnCreatedRoom()
        {
            Debug.Log("room created");
        }

        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            Debug.Log("room list updated");

            foreach (var roomInfo in roomList)
            {
                if (_roomViewDictionary.ContainsKey(roomInfo.Name))
                {
                    if (roomInfo.RemovedFromList)
                    {
                        Destroy(_roomViewDictionary[roomInfo.Name].gameObject);
                        _roomViewDictionary.Remove(roomInfo.Name);
                        continue;
                    }

                    var roomView = _roomViewDictionary[roomInfo.Name];
                    roomView.PlayerCount = roomInfo.PlayerCount + "/" + roomInfo.MaxPlayers;
                    roomView.RoomState = roomInfo.IsOpen ? "Open" : "Closed";
                }
                else
                {
                    var roomView = Instantiate(roomListRowPrefab, roomScrollRect.content).GetComponent<RoomListRow>();

                    roomView.RoomName = roomInfo.Name;
                    roomView.PlayerCount = roomInfo.PlayerCount + "/" + roomInfo.MaxPlayers;
                    roomView.RoomState = roomInfo.IsOpen ? "Open" : "Closed";

                    roomView.Selected = delegate { PhotonNetwork.JoinRoom(roomInfo.Name); };

                    _roomViewDictionary.Add(roomInfo.Name, roomView);
                }
            }
        }

        private void CreateRoom()
        {
            PhotonNetwork.CreateRoom(Random.Range(100, 1000).ToString(), new RoomOptions {MaxPlayers = 2});
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                CreateRoom();
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                PhotonNetwork.LeaveRoom();
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                Debug.Log(PhotonNetwork.CountOfPlayersOnMaster);
            }
        }
    }
}