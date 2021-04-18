using System.Collections.Generic;
using System.Linq;
using Config;
using Cysharp.Threading.Tasks;
using ExitGames.Client.Photon;
using GameSession;
using Logic;
using NaughtyAttributes;
using Photon.Pun;
using Scenes.RoomLobby;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Utils;

namespace UI
{
    public class RoomLobbyUiController : MonoBehaviourPunCallbacks
    {
        [SerializeField]
        private Button leaveButton;

        [SerializeField]
        private Button startButton;

        [SerializeField]
        private GameObject mapCollectionParent;

        [SerializeField]
        private TMP_InputField mapCollection;

        [SerializeField]
        [Scene]
        private string roomSelectionScene;

        [SerializeField]
        [Scene]
        private string nextScene;

        [SerializeField]
        private GameObject playerListRowPrefab;

        [SerializeField]
        private ScrollRect playerList;

        private readonly Dictionary<Photon.Realtime.Player, PlayerListRow> _playerRows =
            new Dictionary<Photon.Realtime.Player, PlayerListRow>();

        private static void InitializeGameSession(EventData eventData)
        {
            var players = new List<GameSession.Player>();

            var dataArray = (object[]) eventData.CustomData;
            CurrentGameSession.MapCollection = GameConfig.Instance.MapCollections[(byte) dataArray[1]];

            for (var i = 0; i < (byte) dataArray[0]; i++)
            {
                var playerId = (byte) dataArray[2 + i * 2];
                var photonActorNumber = (int) dataArray[3 + i * 2];

                if (PhotonNetwork.LocalPlayer.ActorNumber == photonActorNumber)
                {
                    players.Add(new LocalPlayer(playerId, playerId, 0));
                }
                else
                {
                    players.Add(new OnlinePlayer(playerId, playerId,
                        PhotonNetwork.CurrentRoom.GetPlayer(photonActorNumber)));
                }
            }

            CurrentGameSession.Players = players.ToArray();
            CurrentGameSession.SetNextRoundPlayer(players[0]);
            PhotonNetwork.LoadLevel(CurrentGameSession.MapCollection.Maps[0]);
        }

        private void Awake()
        {
            PhotonEventListener.ListenTo(GameEvent.GameSessionPlayersShouldInitialize, InitializeGameSession);

            leaveButton.onClick.AddListener(delegate
            {
                if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
                {
                }

                PhotonNetwork.LeaveRoom();
            });

            startButton.gameObject.SetActive(PhotonNetwork.IsMasterClient);
            mapCollectionParent.SetActive(PhotonNetwork.IsMasterClient);

            //TODO: Make ready button

            startButton.onClick.AddListener(delegate
            {
                //Add player count (byte)
                var eventData = new List<object> {PhotonNetwork.CurrentRoom.PlayerCount};

                eventData.Add(byte.Parse(mapCollection.text));

                //Add (byte)game session player id + (int)photon player id
                for (byte i = 0; i < PhotonNetwork.CurrentRoom.Players.Values.Count; i++)
                {
                    eventData.Add(i);
                    eventData.Add(PhotonNetwork.CurrentRoom.Players.Values.ElementAt(i).ActorNumber);
                }

                PhotonShortcuts.ReliableRaiseEventToAll(GameEvent.GameSessionPlayersShouldInitialize, eventData);
                // SceneChanger.ChangeScene(nextScene);
            });

            foreach (var player in PhotonNetwork.CurrentRoom.Players.Values)
            {
                AddNewPlayer(player);
            }

            UpdateStartButton();
        }

        private void UpdateStartButton()
        {
            startButton.interactable = PhotonNetwork.CurrentRoom.Players.Count > 1;
        }

        private void AddNewPlayer(Photon.Realtime.Player player)
        {
            var playerListRow = Instantiate(playerListRowPrefab, playerList.content).GetComponent<PlayerListRow>();
            playerListRow.PlayerName =
                string.IsNullOrEmpty(player.NickName) ? "Player " + player.ActorNumber : player.NickName;

            if (player.Equals(PhotonNetwork.LocalPlayer))
            {
                playerListRow.PlayerName += " (You)";
            }

            playerListRow.PlayerReadyStatus = "";

            _playerRows.Add(player, playerListRow);
        }

        private void RemovePlayer(Photon.Realtime.Player player)
        {
            if (_playerRows.ContainsKey(player))
            {
                Destroy(_playerRows[player]);
                _playerRows.Remove(player);
            }
        }

        public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
        {
            AddNewPlayer(newPlayer);
            UpdateStartButton();
        }

        public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
        {
            RemovePlayer(otherPlayer);
            UpdateStartButton();
        }

        public override void OnLeftRoom()
        {
            SceneManager.LoadScene(roomSelectionScene);
        }
    }
}