using System.Collections.Generic;
using System.Linq;
using Config;
using ExitGames.Client.Photon;
using GameSession;
using Logic;
using NaughtyAttributes;
using Photon.Pun;
using Scenes.RoomLobby;
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
        [Scene]
        private string mapSelectionScene;

        [SerializeField]
        [Scene]
        private string roomSelectionScene;

        [SerializeField]
        private GameObject playerListRowPrefab;

        [SerializeField]
        private ScrollRect playerList;

        [SerializeField]
        private Toggle readyToggle;

        private readonly Dictionary<Photon.Realtime.Player, PlayerListRow> _playerRows =
            new Dictionary<Photon.Realtime.Player, PlayerListRow>();

        private void InitializeGameSession(EventData eventData)
        {
            var players = new List<GameSession.Player>();

            var dataArray = (object[]) eventData.CustomData;
            CurrentGameSession.MapCollection = GameConfig.Instance.MapCollections[(byte) dataArray[1]];

            for (var i = 0; i < (byte) dataArray[0]; i++)
            {
                var playerId = (byte) dataArray[1 + i * 2];
                var photonActorNumber = (int) dataArray[2 + i * 2];

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
            PhotonNetwork.LoadLevel(mapSelectionScene);
        }

        private void Awake()
        {
            PhotonEventListener.ListenTo(GameEvent.GameSessionPlayersShouldInitialize, InitializeGameSession);

            leaveButton.onClick.AddListener(delegate
            {
                PhotonNetwork.LeaveRoom();
                SceneManager.LoadScene(roomSelectionScene);
            });

            startButton.gameObject.SetActive(PhotonNetwork.IsMasterClient);
            readyToggle.gameObject.SetActive(!PhotonNetwork.IsMasterClient);

            startButton.onClick.AddListener(delegate
            {
                //Add player count (byte)
                var eventData = new List<object>
                {
                    PhotonNetwork.CurrentRoom.PlayerCount
                };

                //Add (byte)game session player id + (int)photon player id
                for (byte i = 0; i < PhotonNetwork.CurrentRoom.Players.Values.Count; i++)
                {
                    eventData.Add(i);
                    eventData.Add(PhotonNetwork.CurrentRoom.Players.Values.ElementAt(i).ActorNumber);
                }

                PhotonShortcuts.ReliableRaiseEventToAll(GameEvent.GameSessionPlayersShouldInitialize, eventData);
            });

            readyToggle.onValueChanged.AddListener(delegate(bool isOn)
            {
                PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable {{"ready", isOn}});
            });

            foreach (var player in PhotonNetwork.CurrentRoom.Players.Values)
            {
                AddNewPlayer(player);
            }

            UpdateStartButton();
        }

        private void UpdateStartButton()
        {
            var allPlayersReady = PhotonNetwork.CurrentRoom.Players.Values
                .Where(p => !p.Equals(PhotonNetwork.LocalPlayer)).All(p =>
                    p.CustomProperties.ContainsKey("ready") && (bool) p.CustomProperties["ready"]);
            startButton.interactable = PhotonNetwork.CurrentRoom.Players.Count > 1 && allPlayersReady;
        }

        private void AddNewPlayer(Photon.Realtime.Player player)
        {
            var playerListRow = Instantiate(playerListRowPrefab, playerList.content).GetComponent<PlayerListRow>();
            playerListRow.PlayerName =
                string.IsNullOrEmpty(player.NickName) ? "Lemming " + player.ActorNumber : player.NickName;

            if (player.Equals(PhotonNetwork.LocalPlayer))
            {
                playerListRow.PlayerName += " (You)";
            }

            playerListRow.PlayerReadyStatus = "";

            _playerRows.Add(player, playerListRow);
        }

        private void RemovePlayer(Photon.Realtime.Player player)
        {
            PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable {{"ready", true}});
            if (_playerRows.ContainsKey(player))
            {
                Destroy(_playerRows[player].gameObject);
                _playerRows.Remove(player);
            }
        }

        public override void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, Hashtable changedProps)
        {
            if (PhotonNetwork.IsMasterClient)
                UpdateStartButton();
        }

        public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
        {
            AddNewPlayer(newPlayer);
            UpdateStartButton();
        }

        public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
        {
            RemovePlayer(otherPlayer);

            if (PhotonNetwork.IsMasterClient && otherPlayer.IsInactive)
            {
                PhotonNetwork.CloseConnection(otherPlayer);
            }

            UpdateStartButton();
        }
    }
}